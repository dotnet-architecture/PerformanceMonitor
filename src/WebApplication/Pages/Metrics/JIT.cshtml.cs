using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;

namespace WebApplication.Pages.Metrics
{
    public class JITModel : PageModel
    {
        public List<Jit> jit { get; set; } = new List<Jit>();

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Jit> addOn = await FetchDataService.getUpdatedData<Jit>(oldStamp, newStamp);

            foreach (Jit j in addOn)
            {
                jit.Add(j);
            }
        }
    }
}