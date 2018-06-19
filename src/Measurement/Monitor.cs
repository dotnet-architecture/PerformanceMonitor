using System;
using System.Diagnostics;
using System.Timers;
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
        // creates object that will store all event instances
        public static Metric_List list = new Metric_List();

        // creates an HTTP client so that server requests can be made
        private static readonly HttpClient client = new HttpClient();

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
        // timer for collecting machine data (every 1000 milliseconds)
        private static Timer dataTimer = new Timer(1000);
        // list containing instances of CPU readings
        public static List<CPU_Usage> CPUVals = new List<CPU_Usage>();

        // Mem block:
        // list containing instances of Memory readings
        public static List<Mem_Usage> MemVals = new List<Mem_Usage>();

        // timer for sending of HTTP requests (every 5000 milliseconds)
        private static Timer HttpTimer = new Timer(5000);



        /*
         * METHOD DECLARATION BLOCK
         */ 
        public void Record()  // sets timer that calls Collect every five seconds
        {
            Run();  // initiates data collection
            HttpTimer.Elapsed += Collect;
            HttpTimer.AutoReset = true;
            HttpTimer.Enabled = true;  // sets timer for data transmission
        }
        public async void Collect(object source, ElapsedEventArgs e)  // sends collected data to API
        {
            list.cpu = CPUVals;
            list.mem = MemVals;
            if (list.cpu.Count != 0)
            {
                string output = JsonConvert.SerializeObject(list);
                Console.WriteLine(output);
                //var stringContent = new StringContent(output);
                // sends POST request to server, containing JSON representation of events
                //HttpResponseMessage response = await client.PostAsync("sample uri", stringContent);
                CPUVals.Clear();
                MemVals.Clear();
            }
        }
        private static void Run()  // sets timer that calls FetchCPU every second
        {
            // timer will call FetchCPU every second. note: FetchCPU calls FetchMem as well
            dataTimer.Elapsed += FetchCPU;
            dataTimer.AutoReset = true;
            dataTimer.Enabled = true;
        }
        private static void FetchCPU(object source, ElapsedEventArgs e)  // calculates CPU usage and calls other data collection functions
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
            // fetches memory usage (called here to be done every second without another timer)
            FetchMem();
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
    }
}
