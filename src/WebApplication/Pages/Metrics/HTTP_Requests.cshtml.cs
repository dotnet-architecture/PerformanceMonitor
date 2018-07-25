using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using WebApplication.ClientSideData;
using System.Linq;

namespace WebApplication.Pages.Metrics
{
    public class HTTP_RequestsModel : PageModel
    {
        public List<Detailed_Http_Req> http { get; set; } = new List<Detailed_Http_Req>();
        public Dictionary<Guid, Detailed_Http_Req> httpTracker = new Dictionary<Guid, Detailed_Http_Req>(); 
        public int totalHttpRequest = 0; 

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Http_Request> addOn = await FetchDataService.getUpdatedData<Http_Request>(oldStamp, newStamp);

            foreach (Http_Request h in addOn)
            {
                if (h.type.Equals("Start"))
                {
                    Detailed_Http_Req clientH = new Detailed_Http_Req(h);
                    httpTracker[h.id] = clientH;
                    http.Add(clientH);
                } else if (h.type.Equals("Stop"))
                {
                    Detailed_Http_Req clientH = httpTracker[h.id];
                    http.Remove(clientH);
                    clientH.updateEndTimestamp(h.timestamp);
                    http.Add(clientH);
                }
            }

            http.OrderBy(h => h.startTimestamp).ToList(); // updating http so that is sorted by time
            http.Reverse(); // updating http so that the most current http requests are shown first

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();

            totalHttpRequest = http.Count;
        }
    }
}