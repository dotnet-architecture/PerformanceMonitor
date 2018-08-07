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
        public List<Client_Http_Request> http { get; set; } = new List<Client_Http_Request>();

        // Contains http requests that do not yet have a endStamp
        public Dictionary<Guid, Client_Http_Request> httpTracker = new Dictionary<Guid, Client_Http_Request>(); 
        public int totalHttpRequest = 0;
        public double avgDuration = 0; 

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
                    Client_Http_Request clientH = new Client_Http_Request(h);
                    httpTracker[h.id] = clientH;
                    http.Add(clientH);
                } else if (h.type.Equals("Stop"))
                {
                    if (httpTracker.ContainsKey(h.id))
                    {
                        Client_Http_Request clientH = httpTracker[h.id];
                        http.Remove(clientH);
                        clientH.updateEndTimestamp(h.timestamp);
                        http.Add(clientH);
                    }
                }
            }

            http.OrderBy(h => h.StartTimestamp).ToList(); // updating http so that is sorted by time
            http.Reverse(); // updating http so that the most current http requests are shown first

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();

            totalHttpRequest = http.Count;
            updateAvg(); 
        }

        public double updateAvg() 
        {
            double totalDuration = 0;
            double totalEndedReq = 0;
            foreach (Client_Http_Request c in http)
            {
                if (c.EndTimestamp != null)
                {
                    totalDuration += c.Duration;
                    totalEndedReq++; 
                }
            }

            avgDuration = totalDuration / totalEndedReq;
            return avgDuration;
        }
    }
}