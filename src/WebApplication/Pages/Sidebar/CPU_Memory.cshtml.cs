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
        public CPU_Usage cpu { get; set; } = new CPU_Usage();
        public Mem_Usage mem { get; set; } = new Mem_Usage();
        
        public async Task OnGet()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:58026/");

            HttpResponseMessage cpu_response = await client.GetAsync("api/v1/CPU/CPUBYUSAGE?usage=0");
            _cpu_metricService.updateUsingHttpResponse(cpu_response);

            HttpResponseMessage mem_response = await client.GetAsync("api/v1/Memory/MEMBYUSAGE?usage=0");
            _mem_metricService.updateUsingHttpResponse(mem_response);

            // Deserialize JSON object from response and update cpu and mem if response is successfull
            if (cpu_response.IsSuccessStatusCode)
            {
                cpu = await _cpu_metricService.getCPUUsage();
            }

            if (mem_response.IsSuccessStatusCode)
            {
                mem = await _mem_metricService.getMemoryUsage();
            }

        }
    }
}