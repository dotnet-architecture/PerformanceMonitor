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
        public String process;
        public String app;
        public int sampleRate;
        public int sendRate;

        public Monitor()
        {
            this.process = "MyProcess";
            this.app = "UnnamedApp";
            this.sampleRate = 1000;
            this.sendRate = 5000;
        }
        public Monitor(String process = "MyProcess", String app = "UnnamedApp", int sampleRate = 1000, int sendRate = 5000)
        {
            this.process = process;
            this.app = app;
            this.sampleRate = sampleRate;
            this.sendRate = sendRate;
        }

        public Monitor(int sampleRate = 1000, int sendRate = 5000)
        {
            this.process = "MyProcess";
            this.app = "UnnamedApp";
            this.sampleRate = sampleRate;
            this.sendRate = sendRate;
        }

        public Monitor(String process = "MyProcess", int sampleRate = 1000, int sendRate = 5000)
        {
            this.process = process;
            this.app = "UnnamedApp";
            this.sampleRate = sampleRate;
            this.sendRate = sendRate;
        }



        /*
         * VARIABLE DECLARATION BLOCK
         */

        // by default, all metric tracking enabled
        private static int CPUEnabled = 1;
        private static int MemEnabled = 1;
        private static int ContentionEnabled = 0;
        private static int ExceptionEnabled = 0;
        private static int GCEnabled = 0;
        private static int HttpEnabled = 0;
        private static int JitEnabled = 0;

        // these methods all turn metric tracking on or off
        public void DisableCPU()
        {
            CPUEnabled = 0;
        }
        public void DisableMem()
        {
            MemEnabled = 0;
        }
        public void EnableContention()
        {
            ContentionEnabled = 1;
        }
        public void EnableException()
        {
            ExceptionEnabled = 1;
        }
        public void EnableGC()
        {
            GCEnabled = 1;
        }
        public void EnableHttp()
        {
            HttpEnabled = 1;
        }
        public void EnableJit()
        {
            JitEnabled = 1;
        }

        // fetching properties unique to current data collection session
        private static Process myProcess = Process.GetCurrentProcess();
        private static String myOS = Environment.OSVersion.ToString();
        private static Session instance = new Session();

        // variable used to detect data sending for testing purposes
        public static int hold = 0;
        public int getHold()
        {
            return hold;
        }
        
        // creates an HTTP client so that server requests can be made
        private static HttpClient client = new HttpClient();
        // time object used to check if data should be transmitted
        Stopwatch timer = new Stopwatch();
        // tracks duration of HTTP requests to adjust for delays
        Stopwatch duration = new Stopwatch();

        // CPU block:
        // fetches the processor count for the machine for total CPU calculation
        private static int processorTotal = Environment.ProcessorCount;
        private static double oldTime = 0;
        private static DateTime oldStamp = DateTime.Now;
        private static double newTime = 0;
        private static DateTime newStamp = DateTime.Now;
        private static double change = 0;
        private static double period = 0;
        // list containing instances of CPU readings
        public static List<CPU_Usage> CPUVals = new List<CPU_Usage>();
        public int getCPUCount()
        {
            return CPUVals.Count;
        }

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

        // lock object to keep things thread-safe
        Object lockObject = new object();



        /*
         * METHOD DECLARATION BLOCK
         */
        public void Record()  // sets timer that calls Collect every five seconds
        {
            // sets base address for HTTP requests - in local testing, this may need to be changed periodically
            client.BaseAddress = new Uri("http://localhost:54022/");

            // assign all properties of the current process to the Session class instance
            instance.process = (this.process);
            instance.sampleRate = sampleRate;
            instance.sendRate = sendRate;
            instance.processorCount = processorTotal;
            instance.os = myOS;
            instance.application = app;

            // starts event collection via TraceEvent in separate task
            Task.Factory.StartNew(() =>
            {
                TraceEvents();
            });

            Task.Factory.StartNew(async () =>
            {
                timer.Start();
                while (true)
                {
                    if (CPUEnabled == 1)
                    {
                        FetchCPU();
                    }
                    if (MemEnabled == 1)
                    {
                        FetchMem();
                    }

                    // if five seconds have passed since HTTP request was made
                    if (timer.ElapsedMilliseconds >= sendRate)
                    {
                        timer.Restart();
                        // creates object that will store all event instances
                        Metric_List list = new Metric_List();

                        lock (lockObject)
                        {
                            list.session = instance;
                            list.cpu = CPUVals;
                            list.mem = MemVals;
                            list.exceptions = ExceptionVals;
                            list.requests = RequestVals;
                            list.contentions = ContentionVals;
                            list.gc = GCVals;
                            list.jit = JitVals;

                            CPUVals = new List<CPU_Usage>();
                            MemVals = new List<Mem_Usage>();
                            ExceptionVals = new List<Exceptions>();
                            RequestVals = new List<Http_Request>();
                            ContentionVals = new List<Contention>();
                            GCVals = new List<GC>();
                            JitVals = new List<Jit>();
                        }

                        duration.Start();
                        hold = 1;

                        String output;
                        try
                        {
                            output = JsonConvert.SerializeObject(list);

                            hold = 0;

                            // escapes string so that JSON object is interpreted as a single string
                            output = JsonConvert.ToString(output);

                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/v1/General");
                            request.Content = new StringContent(output, System.Text.Encoding.UTF8, "application/json");
                            // sends POST request to server, containing JSON representation of events
                            try
                            {
                                HttpResponseMessage response = client.SendAsync(request).Result;
                            }
                            catch { }
                        }
                        catch { }

                        if (sampleRate < duration.ElapsedMilliseconds)
                        {
                            duration.Reset();
                        } else
                        {
                            await Task.Delay(sampleRate - (int)duration.ElapsedMilliseconds);
                            duration.Reset();
                        }
                    } else
                    {
                        await Task.Delay(sampleRate);
                    }
                }
            });
        }

        private void TraceEvents()
        {
            using (var session = new TraceEventSession("MySession"))
            {
                // set up Ctrl-C to stop the session
                SetupCtrlCHandler(() => { session.Stop(); });

                // set up parser to read CLR events
                var clrParser = new ClrTraceEventParser(session.Source);

                if (ExceptionEnabled == 1)
                {
                    // subscribe to all exception start events
                    clrParser.ExceptionStart += delegate (ExceptionTraceData data)
                    {
                        // if exception was in user process, add it to list of exceptions
                        if (data.ProcessID == myProcess.Id)
                        {
                            Exceptions e = new Exceptions();
                            e.type = data.ExceptionType;
                            e.timestamp = DateTime.Now;
                            e.App = instance;
                            lock (lockObject)
                            {
                                ExceptionVals.Add(e);
                            }
                        }
                    };
                }

                if (ContentionEnabled == 1)
                {
                    // subscribe to all contention start events
                    clrParser.ContentionStart += delegate (ContentionTraceData data)
                    {
                        if (data.ProcessID == myProcess.Id)
                        {
                            Contention c = new Contention();
                            c.type = "Start";
                            c.id = data.ActivityID;
                            c.timestamp = DateTime.Now;
                            c.App = instance;
                            lock (lockObject)
                            {
                                ContentionVals.Add(c);
                            }
                        }
                    };
                    // subscribe to all contention stop events
                    clrParser.ContentionStop += delegate (ContentionTraceData data)
                    {
                        if (data.ProcessID == myProcess.Id)
                        {
                            Contention c = new Contention();
                            c.type = "Stop";
                            c.id = data.ActivityID;
                            c.timestamp = DateTime.Now;
                            c.App = instance;
                            lock (lockObject)
                            {
                                ContentionVals.Add(c);
                            }
                        }
                    };
                }

                if (GCEnabled == 1)
                {
                    // subscribe to all GC start events
                    clrParser.GCStart += delegate (GCStartTraceData data)
                    {
                        if (data.ProcessID == myProcess.Id)
                        {
                            GC gc = new GC();
                            gc.type = "Start";
                            gc.timestamp = DateTime.Now;
                            gc.id = data.ThreadID;
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
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
                            gc.App = instance;
                            lock (lockObject)
                            {
                                GCVals.Add(gc);
                            }
                        }
                    };
                }

                if (JitEnabled == 1)
                {
                    // subscribe to all Jit start events
                    clrParser.MethodJittingStarted += delegate (MethodJittingStartedTraceData data)
                    {
                        if (data.ProcessID == myProcess.Id)
                        {
                            Jit j = new Jit();
                            j.method = data.MethodName;
                            j.timestamp = DateTime.Now;
                            j.App = instance;
                            lock (lockObject)
                            {
                                JitVals.Add(j);
                            }
                        }
                    };
                }

                if (HttpEnabled == 1)
                {
                    // subscribe to all dynamic events (used for HTTP request event tracking)
                    session.Source.Dynamic.All += delegate (TraceEvent data) {
                        if (data.ProcessID == myProcess.Id && data.EventName == "Request/Start")
                        {
                            Http_Request request = new Http_Request();
                            request.type = "Start";
                            request.timestamp = DateTime.Now;
                            request.id = data.ActivityID;
                            request.App = instance;
                            request.method = data.PayloadString(0);
                            request.path = data.PayloadString(1);
                            lock (lockObject)
                            {
                                RequestVals.Add(request);
                            }
                        }
                        else if (data.ProcessID == myProcess.Id && data.EventName == "Request/Stop")
                        {
                            Http_Request request = new Http_Request();
                            request.type = "Stop";
                            request.timestamp = DateTime.Now;
                            request.id = data.ActivityID;
                            request.App = instance;
                            lock (lockObject)
                            {
                                RequestVals.Add(request);
                            }
                        }
                    };
                }

                // set up providers for events using GUIDs - first EnableProvider command is to receive activity IDs for associated events
                session.EnableProvider(new Guid("2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5"), TraceEventLevel.Informational, 0x80);
                var AspSourceGuid = TraceEventProviders.GetEventSourceGuidFromName("Microsoft-AspNetCore-Hosting");
                session.EnableProvider(AspSourceGuid);
                session.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Verbose, (0x8000 | 0x4000 | 0x1 | 0x10));
                session.Source.Process();    // call the callbacks for each event
            }
        }

        private void FetchCPU()  // calculates CPU usage
        {
            if (CPUEnabled == 1)
            {
                // clear the process' cached information
                myProcess.Refresh();
                CPU_Usage cpu = new CPU_Usage();
                newTime = myProcess.TotalProcessorTime.TotalMilliseconds;
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
                lock (lockObject)
                {
                    CPUVals.Add(cpu);
                }
            }
        }

        private void FetchMem()  // fetches Memory usage
        {
            if (MemEnabled == 1)
            {
                Mem_Usage mem = new Mem_Usage();
                mem.usage = myProcess.WorkingSet64;
                mem.timestamp = DateTime.Now;
                lock (lockObject)
                {
                    MemVals.Add(mem);
                }
            }
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
