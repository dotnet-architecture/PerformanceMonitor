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
        // List of cpu data that will be shown on page when it loads
        public List<CPU_Usage> cpu { get; set; } = new List<CPU_Usage>();

        // List of mem data that will be shown on page when it loads
        public List<Mem_Usage> mem { get; set; } = new List<Mem_Usage>();

        // Average CPU data, calculated by (total cpu usage/ total period of time)
        public double avgCPU;
        // Average percentage of mem usage, calculated by (total percentage of mem used/ total period of time)
        public double avgMem;
        // Total time that is accounted for in the average CPU and mem. Used to update to new avgCPU and avgMem
        public int timeAccounted; 

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        public DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        public DateTime newStamp = DateTime.Now.ToUniversalTime(); // Gets current time

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime(); // Updating newStamp

            // Getting cpu and mem data based off of dates (oldStamp and newStamp)
            List<CPU_Usage> cpu_addOn = await FetchDataService.getUpdatedData<CPU_Usage>(oldStamp, newStamp);
            List<Mem_Usage> mem_addOn = await FetchDataService.getUpdatedData<Mem_Usage>(oldStamp, newStamp); 

            double totalCPU = avgCPU * timeAccounted; // Weighting previous avgCPU
            double totalMem = avgMem * timeAccounted; // Weighting previous avgMem

            // Updates CPU_Usage list and totalCPU to calculate new average
            foreach (CPU_Usage c in cpu_addOn)
            {
                totalCPU += c.usage;
                cpu.Add(c);
            }

            // Calculating new avgCPUs
            this.timeAccounted += cpu_addOn.Count;
            this.avgCPU = totalCPU / (double)timeAccounted;

            // Updates Mem_Usage list and totalMem to calculate new average
            foreach (Mem_Usage m in mem_addOn)
            {
                totalMem += m.usage;
                mem.Add(m);
            }

            // Calculating new avgMem
            this.timeAccounted += mem_addOn.Count;
            this.avgMem = totalMem / (double)timeAccounted;
        }
    }
}