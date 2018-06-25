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



        /*
         * METHOD DECLARATION BLOCK
         */ 
        public void Record()  // sets timer that calls Collect every five seconds
        {
            client.BaseAddress = new Uri("http://localhost:51249/");
            while (true)
            {
                // if a second has passed since data collection
                if (DateTime.Now.Subtract(metricTime).TotalMilliseconds >= 1000)
                {
                    metricTime = DateTime.Now;
                    FetchCPU();
                    FetchMem();

                    // if five seconds have passed since HTTP request was made
                    if (DateTime.Now.Subtract(httpTime).TotalMilliseconds >= 5000)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            httpTime = DateTime.Now;
                            // creates object that will store all event instances
                            Metric_List list = new Metric_List();
                            list.cpu = CPUVals;
                            list.mem = MemVals;
                            Send_HTTP(list);
                            CPUVals.Clear();
                            MemVals.Clear();
                        });
                    }
                }
            }
        }
        public void Send_HTTP(Metric_List metricList)  // sends collected data to API
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
    }
}
