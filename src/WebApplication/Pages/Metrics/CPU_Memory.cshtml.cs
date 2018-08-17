using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using Newtonsoft.Json;
using System.Net.Http;
using System.Globalization;

namespace WebApplication.Pages.Metrics
{
    public class CPU_MemoryModel : PageModel
    {
        // List of cpu and mem data that will be shown on page when it loads
        public List<CPU_Usage> cpu { get; set; } = new List<CPU_Usage>();
        public List<Mem_Usage> mem { get; set; } = new List<Mem_Usage>();
        public Dictionary<String, Tuple<CPU_Usage, Mem_Usage>> dataByTime =
            new Dictionary<String, Tuple<CPU_Usage, Mem_Usage>>();
        public List<KeyValuePair<DateTime, Tuple<CPU_Usage, Mem_Usage>>> dataByTimeSorted =
            new List<KeyValuePair<DateTime, Tuple<CPU_Usage, Mem_Usage>>>();

        public int pointsOnGraph = 100; // Number of points that will be shown on graph, keeps only current data on graph

        // Average CPU and percentage of mem
        public double avgCPU;
        public double avgMem;
        // Total time that is accounted for in the average CPU and mem. Used to update to new avgCPU and avgMem
        public int timeAccounted;

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        public DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        public DateTime newStamp; // Represents current time

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime(); // Updating newStamp

            // Getting cpu and mem data based off of dates (oldStamp and newStamp)
            List<CPU_Usage> cpu_addOn = await FetchDataService.getData<CPU_Usage>(oldStamp, newStamp);
            List<Mem_Usage> mem_addOn = await FetchDataService.getData<Mem_Usage>(oldStamp, newStamp);

            double totalCPU = avgCPU * timeAccounted; // Weighting previous avgCPU
            double totalMem = avgMem * timeAccounted; // Weighting previous avgMem

            // Updates CPU_Usage list and totalCPU to calculate new average
            foreach (CPU_Usage c in cpu_addOn)
            {
                totalCPU += c.usage;
                cpu.Add(c);
                String dateString = c.timestamp.ToString("yyyyMMddHHmmssFFF");
                if (dataByTime.ContainsKey(dateString))
                {
                    Tuple<CPU_Usage, Mem_Usage> val = dataByTime[dateString];
                    val = new Tuple<CPU_Usage, Mem_Usage>(c, val.Item2);

                    dataByTime.Remove(dateString);
                    dataByTime.Add(dateString, val);
                }
                else
                {
                    Tuple<CPU_Usage, Mem_Usage> val = new Tuple<CPU_Usage, Mem_Usage>(c, null);
                    dataByTime.Add(dateString, val);
                }
            }

            // Updates Mem_Usage list and totalMem to calculate new average
            foreach (Mem_Usage m in mem_addOn)
            {
                totalMem += m.usage;
                mem.Add(m);
                String dateString = m.timestamp.ToString("yyyyMMddHHmmssFFF");
                if (dataByTime.ContainsKey(dateString))
                {
                    Tuple<CPU_Usage, Mem_Usage> val = dataByTime[dateString];
                    val = new Tuple<CPU_Usage, Mem_Usage>(val.Item1, m);

                    dataByTime.Remove(dateString);
                    dataByTime.Add(dateString, val);
                }
                else
                {
                    Tuple<CPU_Usage, Mem_Usage> val = new Tuple<CPU_Usage, Mem_Usage>(null, m);
                    dataByTime.Add(dateString, val);
                }
            }

            // Calculating new avgMem
            this.timeAccounted += mem_addOn.Count;
            this.avgMem = totalMem / (double)timeAccounted;
            // Calculating new avgCPUs
            this.timeAccounted += cpu_addOn.Count;
            this.avgCPU = totalCPU / (double)timeAccounted;

            foreach (KeyValuePair<String, Tuple<CPU_Usage, Mem_Usage>> p in dataByTime)
            {
                string format = "yyyyMMddHHmmssFFF";
                DateTime d = DateTime.ParseExact(p.Key, format, CultureInfo.InvariantCulture);
                dataByTimeSorted.Add(new KeyValuePair<DateTime, Tuple<CPU_Usage, Mem_Usage>>(d, p.Value));
            }
            dataByTimeSorted.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));
        }
    }
}