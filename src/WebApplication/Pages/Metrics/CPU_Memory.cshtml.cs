using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using Newtonsoft.Json;
using System.Net.Http;

namespace WebApplication.Pages.Metrics
{
    public class CPU_MemoryModel : PageModel
    {
        public List<CPU_Usage> cpu { get; set; } = new List<CPU_Usage>();
        public List<Mem_Usage> mem { get; set; } = new List<Mem_Usage>();

        public double avgCPU;
        public int timeAccounted; // Total time that is accounted for in the average CPU. Used to update to new avgCPU

        // Counter that detects when 5 seconds pass so HTTP get requests are sent every 5 seconds
        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();

            List<CPU_Usage> cpu_addOn = await FetchDataService.getUpdatedData<CPU_Usage>(oldStamp, newStamp);
            List<Mem_Usage> mem_addOn = await FetchDataService.getUpdatedData<Mem_Usage>(oldStamp, newStamp); 

            double totalCPU = avgCPU * timeAccounted; // Weighting previous avgCPU

            // Updates CPU_Usage list and totalCPU to calculate new average
            foreach (CPU_Usage c in cpu_addOn)
            {
                totalCPU += c.usage;
                cpu.Add(c);
            }

            // Calculating new avgCPUs
            this.timeAccounted += cpu_addOn.Count;
            this.avgCPU = totalCPU / (double)timeAccounted;

            foreach (Mem_Usage m in mem_addOn)
            {
                mem.Add(m);
            }

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();
        }
        
        // Returns cpu usage list as a double and reversed
        public Array getCPUUsage()
        {
            List<double> cpuUsage = new List<double>();
            for (int i = cpu.Count - 1; i >= 0; i--)
            {
                cpuUsage.Add(cpu[i].usage);
            }
            return cpuUsage.ToArray();
        }

        // Returns cpu time stamp list as increasing integers for now
        public Array getCPUTimeStamps()
        {
            List<int> cpuTime = new List<int>();
            for (int i = 0; i < cpu.Count; i++)
            {
                cpuTime.Add(i);
            }
            return cpuTime.ToArray();
        }

        /*
        // Attempting to use SignalR
        public void useSignalR(string httpGetRequestEnd)
        {
            String url = "http://localhost:58026/api/v1/CPU/Daterange?start=" + httpGetRequestEnd + "/";
            var hubConnection = new HubConnection(url);
            IHubProxy cpuHubProxy = hubConnection.CreateHubProxy("CPU");
            cpuHubProxy.On<CPU_Usage>("UpdateCPU", cpu => Console.WriteLine("cpu update for {0} new price {1}", cpu.usage, cpu.timestamp));
            await hubConnection.Start();
        }
        */
    }
}