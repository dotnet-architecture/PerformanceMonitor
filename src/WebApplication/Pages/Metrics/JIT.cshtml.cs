using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using WebApplication.Services;
using WebApplication.Interfaces; 

namespace WebApplication.Pages.Metrics
{
    public class JITModel : PageModel
    {
        private readonly IMetricService<Jit> _gcMetricService = new MetricService<Jit>();
        public List<Jit> jit { get; set; } = new List<Jit>();

        // Counter that detects when 5 seconds pass so HTTP get requests are sent every 5 seconds
        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Jit> addOn = await FetchDataService.getUpdatedData<Jit>(oldStamp, newStamp);

            foreach (Jit j in addOn)
            {
                jit.Add(j);
            }
        }

    }
}