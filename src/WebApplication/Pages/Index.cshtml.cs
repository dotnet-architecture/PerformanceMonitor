using DataTransfer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        // List of all active sessions that users can view data of
        private List<Session> sessions = new List<Session>();
        
        // Sorting sessions by application name to present in the menu seen in the homepage
        public Dictionary<String, List<String>> sessionsByApp = new Dictionary<String, List<String>>();
        
        // Deserialized versison of user's input, may not be a valid session
        private Session selectedSession { get; set; } = new Session();

        // Valid session that the user has selected, available to the rest of the web application to fetch data accordingly
        public static Session userSession { get; set; }

        // User feedback that lets user's know if their input is valid and how to proceed
        public String indexMessage = "Please select the name of the application and process you would like to examine.";

        public async Task OnGet() // Method that gets called as page is loaded and refreshed
        {
            sessions = await FetchDataService.getSessionData();
            sortSessionsByApp();
        }

        // Sorts sessions by application and updates sessionsByApp
        public void sortSessionsByApp()
        {
            foreach (Session s in sessions)
            {
                if (sessionsByApp.ContainsKey(s.application))
                {
                    List<String> sess = sessionsByApp.GetValueOrDefault(s.application);
                    sess.Add(s.process);
                    sessionsByApp[s.application] = sess; 
                } else
                {
                    List<String> newSessList = new List<String>();
                    newSessList.Add(s.process);
                    sessionsByApp.Add(s.application, newSessList); 
                }
            }
        }
        
        // Triggered when user clicks "Examine" button
        // Parameters: app - what user clicked in the first menu, pro - retrieved from second portion of menu
        public async Task OnPostAsync(String app, String pro)
        {
            // Sending http request to api to get session that matches application and process defined by user
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:54022/");
            String htmlAddress = "SESSIONBYAPPANDPRO?app=" +
                Uri.EscapeDataString(app) + // Application name user defined
                "&pro=" +
                Uri.EscapeDataString(pro); // Process name user defined
            HttpResponseMessage response = await client.GetAsync(htmlAddress);

            var result = response.Content.ReadAsStringAsync().Result;

            selectedSession = JsonConvert.DeserializeObject<Session>(result);

            // Checking session user requested is valid, should not be hit unless user selected only application
            if (selectedSession == null || !selectedSession.application.Equals(app) || !selectedSession.process.Equals(pro))
            {
                indexMessage = "Error. Please try re-entering the application and process name."; 
            } else
            {
                userSession = selectedSession; // Copying over selectedSession to public userSession
                // Positive user feedback
                indexMessage = "Showing information of " + userSession.application + " application and " + userSession.process + " process.";
            }
        }  
    }
}
