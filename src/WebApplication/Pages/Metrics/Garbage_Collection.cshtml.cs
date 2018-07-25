using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using WebApplication.ClientSideData;
using System.Linq;

namespace WebApplication.Pages.Metrics
{
    public class Garbage_CollectionModel : PageModel
    {
        public List<Client_GC> gc { get; set; } = new List<Client_GC>();

        // Contains gc events that do not yet have a endStamp
        public Dictionary<int, Client_GC> gcTracker = new Dictionary<int, Client_GC>();
        public int totalGC = 0;

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<DataTransfer.GC> addOn = await FetchDataService.getUpdatedData<DataTransfer.GC>(oldStamp, newStamp);

            foreach (DataTransfer.GC g in addOn)
            {
                if (g.type.Equals("Start"))
                {
                    Client_GC clientG = new Client_GC(g);
                    gcTracker[g.id] = clientG;
                    gc.Add(clientG);
                }
                else if (g.type.Equals("Stop"))
                {
                    Client_GC clientG = gcTracker[g.id];
                    gc.Remove(clientG);
                    clientG.updateEndTimestamp(g.timestamp);
                    gc.Add(clientG);
                }
            }

            gc.OrderBy(c => c.StartTimestamp).ToList(); // updating http so that is sorted by time
            gc.Reverse(); // updating http so that the most current http requests are shown first


            totalGC = gc.Count;

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();
        }
    }
}