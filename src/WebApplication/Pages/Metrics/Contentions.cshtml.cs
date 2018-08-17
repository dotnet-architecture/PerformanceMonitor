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
        // Contains contentions that have both an start and stop, and a calculated duration
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

            // Geting new data 
            List<Contention> addOn = await FetchDataService.getData<Contention>(oldStamp, newStamp);

            foreach (Contention c in addOn)
            {
                // When there is start event, create a new Client_Contention object and add it to contentionTracker
                if (c.type.Equals("Start"))
                {
                    Client_Contention clientC = new Client_Contention(c);
                    contentionTracker[c.id] = clientC;
                    contentions.Add(clientC);
                }
                // When there is a stop event, remove associated contention (by looking at id) from contentionTracker,
                // update the Client_Contention to include stop event and update contentions list
                else if (c.type.Equals("Stop"))
                {
                    Client_Contention clientC = contentionTracker[c.id];
                    contentions.Remove(clientC);
                    clientC.updateEndTimestamp(c.timestamp);
                    contentions.Add(clientC);
                }
            }

            contentions.OrderBy(c => c.StartTimestamp).ToList(); // Updating contentions so that is sorted by time
            contentions.Reverse(); // Updating contentions so that the most current contentions are shown first

            totalContentions = contentions.Count;
            updateAvg();
        }

        // Updates avgerage duration of contention
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

            if (totalEndedReq == 0)
            {
                return 0;
            }
            else
            {
                avgDuration = totalDuration / totalEndedReq;
                return avgDuration;
            }
        }
    }
}