using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransfer;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNet.SignalR.Client;

namespace WebApplication.Pages.Metrics
{
    public class ContentionsModel : PageModel
    {
        public List<Contention> contentions { get; set; } = new List<Contention>();

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Contention> addOn = await FetchDataService.getUpdatedData<Contention>(oldStamp, newStamp);

            foreach (Contention c in addOn)
            {
                contentions.Add(c);
            }

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();
        }
    }
}