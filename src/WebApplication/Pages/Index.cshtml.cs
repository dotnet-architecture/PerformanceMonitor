using DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Services;
using WebApplication.Interfaces; 

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        public List<Session> sess = new List<Session>();
        public Session selected;
        public String process { get; set; } = "";
        public String application { get; set; } = ""; 
        public async void OnGet()
        {
            IMetricService<Session> _metricService = new MetricService<Session>();

            //call api to get a list of all application and processes
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54022/");
            HttpResponseMessage response = await client.GetAsync("RETURNALL");

            _metricService.updateUsingHttpResponse(response);

            sess = await _metricService.getServiceUsage();
            getSession();
        }

        public void getSession()
        {
            if (process.Equals("") || application.Equals(""))
            {
                return;
            }

            for (int i = 0; i < sess.Count; i++)
            {
                String sessApp = sess[i].application;
                String sessProcess = sess[i].process; 

                if (sessApp.Equals(application) && sessProcess.Equals(process))
                {
                    selected = sess[i]; 
                }
            }
        }
    }
}
