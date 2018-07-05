using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using WebApplication.Interfaces;
using WebApplication.Services;

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

        // Repeatedly sends data fetch request every 5 seconds
        public async Task fiveSecondFetcher()
        {
            while (true)
            {
                this.newStamp = DateTime.Now.ToUniversalTime();
                if (newStamp.Subtract(oldStamp).TotalMilliseconds >= 5000)
                {
                    await getInfo(newStamp);
                }
            }
        }

        public async Task getInfo(DateTime updatedStamp)
        {
            // Make new HTTP get request and update cpu and mem
            List<CPU_Usage> cpu_addOn = await FetchDataService.getUpdatedData<CPU_Usage>(oldStamp, updatedStamp);
            List<Mem_Usage> mem_addOn = await FetchDataService.getUpdatedData<Mem_Usage>(oldStamp, updatedStamp);

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
            this.oldStamp = updatedStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();
        }

    }
}