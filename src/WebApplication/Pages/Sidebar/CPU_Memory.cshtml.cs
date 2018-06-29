using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfMonitor;
using WebApplication.Interfaces;
using WebApplication.Services;

namespace WebApplication.Pages.Metrics
{
    public class CPU_MemoryModel : PageModel
    {
        private readonly IMetricService<CPU_Usage> _cpuMetricService = new MetricService<CPU_Usage>();
        private readonly IMetricService<Mem_Usage> _memMetricService = new MetricService<Mem_Usage>();
        public List<CPU_Usage> cpu { get; set; } = new List<CPU_Usage>();
        public List<Mem_Usage> mem { get; set; } = new List<Mem_Usage>();

        // Setting up a counter that will detect when 5 seconds pass so that HTTP get requests
        // are sent every 5 seconds and update cpu and mem
        private static DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private static DateTime newStamp = DateTime.Now.ToUniversalTime();
        public async Task OnGet()
        {
            // Gets all data until now
            newStamp = DateTime.Now.ToUniversalTime();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/");

            String httpGetRequestEnd = FetchDataService.convertDateTime(oldStamp) + "&end=" 
                + FetchDataService.convertDateTime(newStamp);

            HttpResponseMessage cpu_response = await client.GetAsync("api/v1/CPU/Daterange?start=" + httpGetRequestEnd);
            _cpuMetricService.updateUsingHttpResponse(cpu_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (cpu_response.IsSuccessStatusCode)
            {
                List<CPU_Usage> addOn = await _cpuMetricService.getServiceUsage(); 
                foreach (CPU_Usage c in addOn)
                {
                    cpu.Add(c);                   
                }
            }
        }

        // Repeatedly sends data fetch request every 5 seconds
        public async Task getInfo()
        {
            while (true)
            {
                newStamp = DateTime.Now;
                if (newStamp.Subtract(oldStamp).TotalMilliseconds >= 5000)
                {
                    // Make new HTTP get request and update cpu and mem

                    // await getCPUUpdatedData(oldStamp, newStamp);
                    // await getMemoryUpdatedData(oldStamp, newStamp);

                    List<CPU_Usage> cpu_addOn = await FetchDataService.getUpdatedData<CPU_Usage>(oldStamp, newStamp);
                    List<Mem_Usage> mem_addOn = await FetchDataService.getUpdatedData<Mem_Usage>(oldStamp, newStamp);

                    foreach (CPU_Usage c in cpu_addOn)
                    {
                        cpu.Add(c);
                    }

                    foreach (Mem_Usage m in mem_addOn)
                    {
                        mem.Add(m);
                    }
                    // Reset timers
                    oldStamp = newStamp;
                    newStamp = DateTime.Now.ToUniversalTime();
                }
            }
        }

        /*
        public async Task getCPUUpdatedData(DateTime oldStamp, DateTime newStamp)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/");

            String httpGetRequestEnd = convertDateTime(oldStamp) + "&end=" + convertDateTime(newStamp);

            HttpResponseMessage cpu_response = await client.GetAsync("api/v1/CPU/Daterange?start=" + httpGetRequestEnd);
            _cpuMetricService.updateUsingHttpResponse(cpu_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (cpu_response.IsSuccessStatusCode)
            {
                List<CPU_Usage> addOn = await _cpuMetricService.getServiceUsage();
                foreach (CPU_Usage c in addOn)
                {
                    cpu.Add(c);
                }
            }
        }

        public async Task getMemoryUpdatedData(DateTime oldStamp, DateTime newStamp)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/");

            String httpGetRequestEnd = convertDateTime(oldStamp) + "&end=" + convertDateTime(newStamp);

            HttpResponseMessage mem_response = await client.GetAsync("api/v1/Memory/MEMBYUSAGE?start=" + httpGetRequestEnd);
            _memMetricService.updateUsingHttpResponse(mem_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (mem_response.IsSuccessStatusCode)
            {
                List<Mem_Usage> addOn = await _memMetricService.getServiceUsage();
                foreach(Mem_Usage m in addOn)
                {
                    mem.Add(m);
                }
            }
        }

        public String convertDateTime(DateTime d)
        {
            String s = "";
            s += d.Year.ToString("D4") + "-" + d.Month.ToString("D2") + "-"
                + d.Day.ToString("D2") + "T" + d.Hour.ToString("D2") + "%3A" +
                d.Minute.ToString("D2") + "%3A" + d.Second.ToString("D2") + "." +
                d.Millisecond.ToString("D3");
            return s;
        }
        */

    }
}