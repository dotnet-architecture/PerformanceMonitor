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

        // List of exceptions data
        public List<Exceptions> exceptions { get; set; } = new List<Exceptions>();
        public int totalExceptions = 0;

        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        public DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        public DateTime newStamp; 

        // Dictionary that keeps track of the frequency of every exception type seen
        public Dictionary<string, int> exceptionTracker = new Dictionary<string, int>();
        // Sorted list of exceptions by frequency
        public List<KeyValuePair<string, int>> exceptionSorted = new List<KeyValuePair<string, int>>();

        //[Required]
        //[BindProperty]
        //[Display(Name = "userReqNum")]
        //public int userReqNum { get; set; }

        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();
            List<Exceptions> addOn = await FetchDataService.getData<Exceptions>(oldStamp, newStamp); // Get data

            foreach (Exceptions e in addOn)
            {
                exceptions.Add(e);
                string typeOfException = e.type; 

                // If exceptionTracker contains the type of exception, update the value 
                if (exceptionTracker.ContainsKey(typeOfException))
                {
                    exceptionTracker[typeOfException] = exceptionTracker[typeOfException] + 1; 
                }
                // If exceptionTracker does not contain that type of exception, create a new key value pair
                else
                {
                    exceptionTracker.Add(typeOfException, 1); 
                }
            }

            // In order to sort the dictionary, first need to put into list
            exceptionSorted = exceptionTracker.ToList();
            // Ordering list by frequency of exception
            exceptionSorted.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            // Reverse list so most frequent exceptions are at the front of the list
            exceptionSorted.Reverse();

            totalExceptions = exceptions.Count; // Update totalExceptions
        }
    }
}