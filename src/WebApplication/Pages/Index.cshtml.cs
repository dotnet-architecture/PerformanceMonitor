﻿using DataTransfer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Services;
using WebApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using System.Net;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        public List<Session> sessions = new List<Session>();
        public Session selectedSession { get; set; } = new Session();
        public static Session userSession { get; set; }

        public String message = "Please enter the name of the application and process you would like to examine.";

        [Required]
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "app")]
        public String app { get; set; } = "";
        
        [Required]
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "pro")]
        public String pro { get; set; } = "";

        public async Task OnGet()
        {
            sessions = await FetchDataService.getSessionData();
        }

        public async Task OnPostAsync(String app, String pro)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:54022/");
            String htmlAddress = "SESSIONBYAPPANDPRO?app=" +
                Uri.EscapeDataString(app) +
                "&pro=" +
                Uri.EscapeDataString(pro);
            HttpResponseMessage response = await client.GetAsync(htmlAddress);

            var result = response.Content.ReadAsStringAsync().Result;

            selectedSession = JsonConvert.DeserializeObject<Session>(result);

            if (selectedSession == null || !selectedSession.application.Equals(app) || !selectedSession.process.Equals(pro))
            {
                message = "Error. Please try re-entering the application and process name. " +
                    "Make sure it is one of the sessions listed above.";
            } else
            {
                message = "Showing information of " + selectedSession.application + " application and " + selectedSession.process + " process.";
                userSession = selectedSession;
            }

            await OnGet();
        }  
    }
}
