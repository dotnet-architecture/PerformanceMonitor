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

namespace DataTransfer
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
        public static List<Exceptions> ExceptionVals = new List<Exceptions>();

        // HTTP Request block:
        public static List<Http_Request> RequestVals = new List<Http_Request>();

        // Contention block:
        public static List<Contention> ContentionVals = new List<Contention>();

        // Garbage Collection block:
        public static List<GC> GCVals = new List<GC>();

        // Jit block:
        public static List<Jit> JitVals = new List<Jit>();



        /*
         * METHOD DECLARATION BLOCK
         */
        public void Record()  // sets timer that calls Collect every five seconds
        {
            // sets base address for HTTP requests - in local testing, this may need to be changed periodically
            client.BaseAddress = new Uri("http://localhost:54022/");

            // starts event collection via TraceEvent in separate task
            Task.Factory.StartNew(() =>
            {
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
                            list.requests = RequestVals;
                            list.contentions = ContentionVals;
                            list.gc = GCVals;
                            list.jit = JitVals;

                            SendHTTP(list);

                            CPUVals.Clear();
                            MemVals.Clear();
                            ExceptionVals.Clear();
                            RequestVals.Clear();
                            ContentionVals.Clear();
                            GCVals.Clear();
                            JitVals.Clear();
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

                // subscribe to all exception start events
                clrParser.ExceptionStart += delegate (ExceptionTraceData data)
                {
                    // if exception was in user process, add it to list of exceptions
                    if (data.ProcessID == myProcess.Id)
                    {
                        Exceptions e = new Exceptions();
                        e.type = data.ExceptionType;
                        e.timestamp = DateTime.Now;
                        ExceptionVals.Add(e);
                    }
                };

                // subscribe to all contention start events
                clrParser.ContentionStart += delegate (ContentionTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        Contention c = new Contention();
                        c.type = "Start";
                        c.timestamp = DateTime.Now;
                        ContentionVals.Add(c);
                    }
                };
                // subscribe to all contention stop events
                clrParser.ContentionStop += delegate (ContentionTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        Contention c = new Contention();
                        c.type = "Stop";
                        c.timestamp = DateTime.Now;
                        ContentionVals.Add(c);
                    }
                };

                // subscribe to all GC start events
                clrParser.GCStart += delegate (GCStartTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Start";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all GC stop events
                clrParser.GCStop += delegate (GCEndTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Stop";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all GC memory allocation ticks
                clrParser.GCAllocationTick += delegate (GCAllocationTickTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Allocation Tick";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all creations of concurrent threads for GC
                clrParser.GCCreateConcurrentThread += delegate (GCCreateConcurrentThreadTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Create Concurrent Thread";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all restart starts
                clrParser.GCRestartEEStart += delegate (GCNoUserDataTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Restart EE Start";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all restart stops
                clrParser.GCRestartEEStop += delegate (GCNoUserDataTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Restart EE Stop";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all suspension starts
                clrParser.GCSuspendEEStart += delegate (GCSuspendEETraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Suspend EE Start";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all suspension stops
                clrParser.GCSuspendEEStop += delegate (GCNoUserDataTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Suspend EE Stop";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all concurrent thread terminations
                clrParser.GCTerminateConcurrentThread += delegate (GCTerminateConcurrentThreadTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Concurrent Thread Termination";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };
                // subscribe to all GC triggers
                clrParser.GCTriggered += delegate (GCTriggeredTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        GC gc = new GC();
                        gc.type = "Triggered";
                        gc.timestamp = DateTime.Now;
                        gc.id = data.ThreadID;
                        GCVals.Add(gc);
                    }
                };

                // subscribe to all Jit start events
                clrParser.MethodJittingStarted += delegate (MethodJittingStartedTraceData data)
                {
                    if (data.ProcessID == myProcess.Id)
                    {
                        Jit j = new Jit();
                        j.method = data.MethodName;
                        j.timestamp = DateTime.Now;
                        JitVals.Add(j);
                    }
                };

                // subscribe to all dynamic events (used for HTTP request event tracking)
                session.Source.Dynamic.All += delegate (TraceEvent data) {
                    if (data.ProcessID == myProcess.Id && data.EventName == "Request/Start")
                    {
                        Http_Request request = new Http_Request();
                        request.type = "Start";
                        request.timestamp = DateTime.Now;
                        request.id = data.ActivityID;
                        // event message parsing to fetch method and path of request
                        String datas = data.ToString();
                        int index = datas.IndexOf("method");
                        int index2 = datas.IndexOf("\"", index);
                        request.method = datas.Substring(index2 + 1, datas.IndexOf("\"", index2 + 1) - index2);
                        index = datas.IndexOf("path");
                        index2 = datas.IndexOf("\"", index);
                        request.path = datas.Substring(index2 + 1, datas.IndexOf("\"", index2 + 1) - index2);
                        RequestVals.Add(request);
                    }
                    else if (data.ProcessID == myProcess.Id && data.EventName == "Request/Stop")
                    {
                        Http_Request request = new Http_Request();
                        request.type = "Stop";
                        request.timestamp = DateTime.Now;
                        request.id = data.ActivityID;
                        RequestVals.Add(request);
                    }
                };

                // set up providers for events using GUIDs
                session.EnableProvider(new Guid("2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5"), TraceEventLevel.Informational, 0x80);
                var AspSourceGuid = TraceEventProviders.GetEventSourceGuidFromName("Microsoft-AspNetCore-Hosting");
                session.EnableProvider(AspSourceGuid);
                session.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Verbose, (0x8000 | 0x4000 | 0x1 | 0x10));
                session.Source.Process();    // call the callbacks for each event
            }
        }

        public void SendHTTP(Metric_List metricList)  // sends collected data to API
        {
            if (metricList.cpu.Count != 0)
            {
                // converts list of metric measurements into a JSON object string
                string output = JsonConvert.SerializeObject(metricList);
                Console.WriteLine(output);

                // escapes string so that JSON object is interpreted as a single string
                output = JsonConvert.ToString(output);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/v1/General");
                request.Content = new StringContent(output, System.Text.Encoding.UTF8, "application/json");
                // sends POST request to server, containing JSON representation of events
                try
                {
                    HttpResponseMessage response = client.SendAsync(request).Result;
                }
                catch {}
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
            CPUVals.Add(cpu);
        }

        private static void FetchMem()  // fetches Memory usage
        {
            Mem_Usage mem = new Mem_Usage();
            mem.usage = process.WorkingSet64;
            mem.timestamp = DateTime.Now;
            MemVals.Add(mem);
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
