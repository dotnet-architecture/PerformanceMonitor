using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace PerfMonitor
{
    public class Monitor
    {
        /*
         * VARIABLE DECLARATION BLOCK
         */
        public Process myProcess = Process.GetCurrentProcess();
        // creates an HTTP client so that server requests can be made
        HttpClient client = new HttpClient();
        // time object used to check if data should be transmitted (would be done every five seconds)
        public static DateTime httpTime = DateTime.Now;

        // CPU block:
        // fetches the processor count for the machine for total CPU calculation
        public static int processorTotal = Environment.ProcessorCount;
        static Process process = Process.GetCurrentProcess();
        static double oldTime = 0;
        static DateTime oldStamp = DateTime.Now;
        static double newTime = 0;
        static DateTime newStamp = DateTime.Now;
        static double change = 0;
        static double period = 0;
        // time object used to check if data should be collected (would be done every second)
        public static DateTime metricTime = DateTime.Now;
        // list containing instances of CPU readings
        public static List<CPU_Usage> CPUVals = new List<CPU_Usage>();

        // Mem block:
        // list containing instances of Memory readings
        public static List<Mem_Usage> MemVals = new List<Mem_Usage>();

        // Exception block:
        // list containing instances of exceptions
        public static List<Exceptions> ExceptionVals = new List<Exceptions>();



        /*
         * METHOD DECLARATION BLOCK
         */ 
        public void Record()  // sets timer that calls Collect every five seconds
        {
            // sets base address for HTTP requests - in local testing, this will need to be changed periodically
            client.BaseAddress = new Uri("http://localhost:51249/");
            Task.Factory.StartNew(() =>
            {
                // starts event collection via TraceEvent
                TraceEvents();
            });

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // if a second has passed since data collection
                    if (DateTime.Now.Subtract(metricTime).TotalMilliseconds >= 1000)
                    {
                        // reset timer and fetch metrics
                        metricTime = DateTime.Now;
                        FetchCPU();
                        FetchMem();

                        // if five seconds have passed since HTTP request was made
                        if (DateTime.Now.Subtract(httpTime).TotalMilliseconds >= 5000)
                        {
                            httpTime = DateTime.Now;
                            // creates object that will store all event instances
                            Metric_List list = new Metric_List();
                            list.cpu = CPUVals;
                            list.mem = MemVals;
                            list.exceptions = ExceptionVals;
                            //SendHTTP(list);
                            CPUVals.Clear();
                            MemVals.Clear();
                            ExceptionVals.Clear();
                        }
                    }
                }
            });
        }

        public void TraceEvents()
        {
            using (var session = new TraceEventSession("MySession"))
            {
                // set up Ctrl-C to stop the session
                SetupCtrlCHandler(() => { session.Stop(); });

                // set up parser to read CLR events
                var clrParser = new ClrTraceEventParser(session.Source);

                // subscribe to all GC allocation events
                clrParser.ExceptionStart += delegate (ExceptionTraceData data)
                {
                    // if exception was in user process, add it to list of exceptions
                    if (data.ProcessID == myProcess.Id)
                    {
                        Exceptions e = new Exceptions();
                        e.type = data.ExceptionType;
                        e.timestamp = DateTime.Now;
                        // adds exception to list of exceptions found
                        ExceptionVals.Add(e);
                        Console.WriteLine("Exception found: {0} at {1}", e.type, e.timestamp);
                    }
                };
                /*
                // subscribe to all dynamic events (used for HTTP request event tracking)
                session.Source.Dynamic.All += delegate (TraceEvent data) {
                    if (data.ProcessID == myProcess.Id)
                    {
                        Console.WriteLine("EVENT FOUND: {0}", data.EventName);
                    }
                };
                */
                // set up providers for events using GUIDs
                var AspSourceGuid = TraceEventProviders.GetEventSourceGuidFromName("Microsoft-AspNetCore-Hosting");
                session.EnableProvider(AspSourceGuid);
                session.EnableProvider(ClrTraceEventParser.ProviderGuid);
                session.Source.Process();    // call the callbacks for each event
            }
        }

        public void SendHTTP(Metric_List metricList)  // sends collected data to API
        {
            if (metricList.cpu.Count != 0)
            {
                // converts list of metric measurements into a JSON object string
                string output = JsonConvert.SerializeObject(metricList);
                // escapes string so that JSON object is interpreted as a single string
                output = JsonConvert.ToString(output);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/v1/CPU/CPUJSON/");
                request.Content = new StringContent(output, System.Text.Encoding.UTF8, "application/json");
                // sends POST request to server, containing JSON representation of events
                HttpResponseMessage response = client.SendAsync(request).Result;
            }
        }

        private static void FetchCPU()  // calculates CPU usage
        {
            // clear the process' cached information
            process.Refresh();
            CPU_Usage cpu = new CPU_Usage();
            newTime = process.TotalProcessorTime.TotalMilliseconds;
            newStamp = DateTime.Now;
            // calculates CPU usage since last measurement
            change = newTime - oldTime;
            // calculates time between CPU measurements
            period = newStamp.Subtract(oldStamp).TotalMilliseconds;
            oldTime = newTime;
            oldStamp = newStamp;

            // finds CPU usage for process as a percentage of total CPU time across the machine
            cpu.usage = (change / (period * processorTotal) * 100.0);
            cpu.timestamp = newStamp;
            // adds CPU value to list of instances
            CPUVals.Add(cpu);
            Console.WriteLine("CPU: {0}; time: {1}", cpu.usage, cpu.timestamp);
        }

        private static void FetchMem()  // fetches Memory usage
        {
            Mem_Usage mem = new Mem_Usage();
            mem.usage = process.WorkingSet64;
            mem.timestamp = DateTime.Now;
            // adds Memory reading to list of instances
            MemVals.Add(mem);
            Console.WriteLine("Mem: {0}; time: {1}", mem.usage, mem.timestamp);
        }

        // setup to stop TraceEvent session upon application termination
        private static bool s_bCtrlCExecuted;
        private static ConsoleCancelEventHandler s_CtrlCHandler;
        private static void SetupCtrlCHandler(Action action)
        {
            s_bCtrlCExecuted = false;
            // uninstall previous handler
            if (s_CtrlCHandler != null)
                Console.CancelKeyPress -= s_CtrlCHandler;

            s_CtrlCHandler =
                (object sender, ConsoleCancelEventArgs cancelArgs) =>
                {
                    if (!s_bCtrlCExecuted)
                    {
                        s_bCtrlCExecuted = true;    // ensure non-reentrant

                        Console.WriteLine("Stopping monitor");

                        action();                   // execute custom action

                        // terminate normally (i.e. when the monitoring tasks complete b/c we've stopped the sessions)
                        cancelArgs.Cancel = true;
                    }
                };
            Console.CancelKeyPress += s_CtrlCHandler;
        }
    }
}
