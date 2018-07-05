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
    public class Garbage_CollectionModel : PageModel
    {
        private readonly IMetricService<DataTransfer.GC> _gcMetricService = new MetricService<DataTransfer.GC>();
        
        public List<DataTransfer.GC> gc { get; set; } = new List<DataTransfer.GC>();

        // Counter that detects when 5 seconds pass so HTTP get requests are sent every 5 seconds
        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<DataTransfer.GC> addOn = await FetchDataService.getUpdatedData<DataTransfer.GC>(oldStamp, newStamp);

            foreach (DataTransfer.GC g in addOn)
            {
                gc.Add(g);
            }
        }
    }
}