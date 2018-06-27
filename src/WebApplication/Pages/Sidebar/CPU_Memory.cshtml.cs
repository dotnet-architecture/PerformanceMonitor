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
        private readonly ICPUService _cpu_metricService = new CPUService();
        private readonly IMemoryService _mem_metricService = new MemoryService();
        public List<CPU_Usage> cpu { get; set; } = new List<CPU_Usage>();
        public Mem_Usage mem { get; set; } = new Mem_Usage();

        // Setting up a counter that will detect when 5 seconds pass so that HTTP get requests
        // are sent every 5 seconds and update cpu and mem
        private static DateTime oldStamp = DateTime.Now;
        private static DateTime newStamp = DateTime.Now;
        public async Task OnGet()
        {
            Console.WriteLine("stamp initialize: " + oldStamp.ToString());

            oldStamp = DateTime.MinValue.ToUniversalTime();
            newStamp = DateTime.Now.ToUniversalTime();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/");

            String httpGetRequestEnd = convertDateTime(oldStamp) + "&end=" + convertDateTime(newStamp);

            HttpResponseMessage cpu_response = await client.GetAsync("api/v1/CPU/Daterange?start=" + httpGetRequestEnd);
            _cpu_metricService.updateUsingHttpResponse(cpu_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (cpu_response.IsSuccessStatusCode)
            {
                List<CPU_Usage> addOn = await _cpu_metricService.getCPUUsage(); 
                foreach (CPU_Usage c in addOn)
                {
                    cpu.Add(c);                   
                }

            }

        }
        public Task getInfo()
        {
            while (true)
            {
                newStamp = DateTime.Now;
                if (newStamp.Subtract(oldStamp).TotalMilliseconds >= 5000)
                {

                    // Make new HTTP get request and update cpu and mem
                    getCPUUpdatedData(oldStamp, newStamp);
                    //await getMemoryUpdatedData(oldStamp, newStamp);

                    // Reset timers
                    oldStamp = newStamp;
                    newStamp = DateTime.Now;
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
        public async Task getCPUUpdatedData(DateTime oldStamp, DateTime newStamp)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/api/v1/CPU/Daterange?start=");

            String httpGetRequestEnd = oldStamp + "&end=" + newStamp; 

            HttpResponseMessage cpu_response = await client.GetAsync(httpGetRequestEnd);
            _cpu_metricService.updateUsingHttpResponse(cpu_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (cpu_response.IsSuccessStatusCode)
            {
                cpu = await _cpu_metricService.getCPUUsage();
            }
        }

        public async Task getMemoryUpdatedData(DateTime oldStamp, DateTime newStamp)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/api/v1/Memory/MEM?d=");

            HttpResponseMessage mem_response = await client.GetAsync("api/v1/Memory/MEMBYUSAGE?usage=0");
            _mem_metricService.updateUsingHttpResponse(mem_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (mem_response.IsSuccessStatusCode)
            {
                mem = await _mem_metricService.getMemoryUsage();
            }
        }

    }
}