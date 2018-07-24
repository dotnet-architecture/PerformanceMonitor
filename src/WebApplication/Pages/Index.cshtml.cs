using DataTransfer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Services;
using WebApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        public class InputModel
        {

        }
        public List<Session> sess = new List<Session>();
        public static Session selected { get; set; } = new Session();

        [BindProperty]
        public List<Session> sessionsReturned { get; set; } = new List<Session>();

        [Required]
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "Application")]
        public String application { get; set; } = "";
        
        [Required]
        [BindProperty]
        [DataType(DataType.Text)]
        [Display(Name = "Process")]
        public String process { get; set; } = "";

        public async void OnGet()
        {
            IMetricService<Session> _metricService = new MetricService<Session>();

            //call api to get a list of all application and processes
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54022/");
            HttpResponseMessage response = await client.GetAsync("RETURNALL");

            _metricService.updateUsingHttpResponse(response);

            sess = await _metricService.getServiceUsage();
        }

        public void OnPost()
        {
            var app = Request.Form["emailaddress"];
            var pro = Request.Form["process"];
        }

        // Returns true if an appropriate application and process are inputed
        // Returns false if no application and process string found OR they are not in the database
        public Boolean getSession()
        {
            if (process.Equals("") || application.Equals(""))
            {
                return false;
            }

            for (int i = 0; i < sess.Count; i++)
            {
                String sessApp = sess[i].application;
                String sessProcess = sess[i].process; 

                if (sessApp.Equals(application) && sessProcess.Equals(process))
                {
                    selected = sess[i];
                    return true;
                }
            }

            return false;
        }
    }
}
