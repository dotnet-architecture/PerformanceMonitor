using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransfer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.ClientSideData;
using System.Linq;

namespace WebApplication.Pages.Metrics
{
    public class ContentionsModel : PageModel
    {
        public List<Client_Contention> contentions { get; set; } = new List<Client_Contention>();
        public double avgDuration = 0;

        // Contains contentions that do not yet have a endStamp
        public Dictionary<Guid, Client_Contention> contentionTracker = new Dictionary<Guid, Client_Contention>();
        public int totalContentions = 0;

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Contention> addOn = await FetchDataService.getUpdatedData<Contention>(oldStamp, newStamp);

            foreach (Contention c in addOn)
            {
                if (c.type.Equals("Start"))
                {
                    Client_Contention clientC = new Client_Contention(c);
                    contentionTracker[c.id] = clientC;
                    contentions.Add(clientC);
                }
                else if (c.type.Equals("Stop"))
                {
                    Client_Contention clientC = contentionTracker[c.id];
                    contentions.Remove(clientC);
                    clientC.updateEndTimestamp(c.timestamp);
                    contentions.Add(clientC);
                }
            }

            contentions.OrderBy(c => c.StartTimestamp).ToList(); // updating http so that is sorted by time
            contentions.Reverse(); // updating http so that the most current http requests are shown first


            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();

            totalContentions = contentions.Count;
            updateAvg();
        }
        public double updateAvg()
        {
            double totalDuration = 0;
            double totalEndedReq = 0;
            foreach (Client_Contention c in contentions)
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
}