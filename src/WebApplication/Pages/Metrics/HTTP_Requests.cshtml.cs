using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;

namespace WebApplication.Pages.Metrics
{
    public class HTTP_RequestsModel : PageModel
    {
        public List<Http_Request> http { get; set; } = new List<Http_Request>();

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Http_Request> addOn = await FetchDataService.getUpdatedData<Http_Request>(oldStamp, newStamp);

            foreach (Http_Request h in addOn)
            {
                http.Add(h);
            }

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();
        }
    }
}