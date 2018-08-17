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
        // Contains http requests that have both an start and stop, and a calculated duration
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
            List<Http_Request> addOn = await FetchDataService.getData<Http_Request>(oldStamp, newStamp); // Get data

            foreach (Http_Request h in addOn)
            {
                // When there is start event, create a new Client_Http_Request object and add it to httpTracker
                if (h.type.Equals("Start"))
                {
                    Client_Http_Request clientH = new Client_Http_Request(h);
                    httpTracker[h.activityID] = clientH;
                    http.Add(clientH);
                }
                // When there is a stop event, remove associated http request (by looking at id) from httpTracker,
                // update the Client_Http_Request to include stop event and update http list
                else if (h.type.Equals("Stop"))
                {
                    if (httpTracker.ContainsKey(h.activityID))
                    {
                        Client_Http_Request clientH = httpTracker[h.activityID];
                        http.Remove(clientH);
                        clientH.updateEndTimestamp(h.timestamp);
                        http.Add(clientH);
                    }
                }
            }

            http.OrderBy(h => h.StartTimestamp).ToList(); // Updating http so that is sorted by time
            http.Reverse(); // Updating http so that the most current http requests are shown first

            totalHttpRequest = http.Count;
            updateAvg(); 
        }

        // Updates avgerage duration of http request
        public double updateAvg() 
        {
            double totalDuration = 0;
            foreach (Client_Http_Request c in http)
            {
                if (c.EndTimestamp != null)
                {
                    totalDuration += c.Duration;
                }
            }

            if (totalHttpRequest == 0)
            {
                return 0;
            }
            else
            {
                avgDuration = totalDuration / totalHttpRequest;
                return avgDuration;
            }
        }
    }
}