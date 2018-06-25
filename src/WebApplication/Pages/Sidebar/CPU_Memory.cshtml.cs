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
        private readonly IMetricService _metricService;

        /* 
        public CPU_MemoryModel(IMetricService metricService)
        {
            _metricService = metricService;
        }
        */        
        public CPU_Usage cpu { get; set; } = new CPU_Usage();

        public Mem_Usage mem { get; set; } = new Mem_Usage();
        
        public async Task OnGet()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:58026/");

            HttpResponseMessage response = await client.GetAsync("api/v1/CPU/CPU");
            Console.WriteLine(response);
            response.EnsureSuccessStatusCode();

            _metricService.updateUsingHttpResponse(response);

            // Deserialize JSON object from response and update cpu and mem
            cpu = await _metricService.getCPUUsage();
            mem = await _metricService.getMemUsage();

        }
    }
}