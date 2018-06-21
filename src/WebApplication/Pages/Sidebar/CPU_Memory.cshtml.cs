using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfMonitor;

namespace WebApplication.Pages.Metrics
{
    public class CPU_MemoryModel : PageModel
    {
        CPU_Usage cpu = new CPU_Usage();
        Mem_Usage mem = new Mem_Usage();
        public async void OnGet()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:44334/");

            //HttpResponseMessage response = await client.GetAsync("INSERT API ADDRESS HERE"); 

            // Deserialize JSON object from response and update cpu and mem

        }
    }
}