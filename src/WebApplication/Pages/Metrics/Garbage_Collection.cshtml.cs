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
        public List<DataTransfer.GC> gc { get; set; } = new List<DataTransfer.GC>();

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
                gc.Add(g);
            }

            totalGC = gc.Count;
        }
    }
}