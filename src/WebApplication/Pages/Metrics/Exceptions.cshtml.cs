using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataTransfer;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Pages.Metrics
{
    public class ExceptionsModel : PageModel
    {
        public List<Exceptions> exceptions { get; set; } = new List<Exceptions>();
        public int totalExceptions = 0;

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();

        public Dictionary<string, int> exceptionTracker = new Dictionary<string, int>();
        public List<KeyValuePair<string, int>> exceptionSorted = new List<KeyValuePair<string, int>>();

        public String message = "Enter the number of top Exceptions you would like to see.";

        [Required]
        [BindProperty]
        [Display(Name = "userReqNum")]
        public int userReqNum { get; set; }

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Exceptions> addOn = await FetchDataService.getUpdatedData<Exceptions>(oldStamp, newStamp);

            foreach (Exceptions e in addOn)
            {
                exceptions.Add(e);
                string typeOfException = e.type; 

                if (exceptionTracker.ContainsKey(typeOfException))
                {
                    exceptionTracker[typeOfException] = exceptionTracker[typeOfException] + 1; 
                } else
                {
                    exceptionTracker.Add(typeOfException, 1); 
                }
            }

            // Reset timers
            this.oldStamp = newStamp;
            this.newStamp = DateTime.Now.ToUniversalTime();

            exceptionSorted = exceptionTracker.ToList();
            exceptionSorted.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            test = exceptionSorted.Count.ToString();
            totalExceptions = exceptions.Count;
        }
        public async Task OnPostAsync(String app, String pro)
        {
            if (userReqNum <= exceptionSorted.Count)
            {
                message = "Currently showing top " + userReqNum + " exceptions.";
            } else
            {
                message = "Do not have " + userReqNum + " exceptions. Showing all exceptions seen so far.";
            }
            await OnGet();
        }
    }
}