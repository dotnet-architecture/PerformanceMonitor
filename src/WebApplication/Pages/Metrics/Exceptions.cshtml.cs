using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;

namespace WebApplication.Pages.Metrics
{
    public class ExceptionsModel : PageModel
    {
        public List<Exceptions> exceptions { get; set; } = new List<Exceptions>();

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Exceptions> addOn = await FetchDataService.getUpdatedData<Exceptions>(oldStamp, newStamp);

            foreach (Exceptions e in addOn)
            {
                exceptions.Add(e);
            }

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();
        }
    }
}