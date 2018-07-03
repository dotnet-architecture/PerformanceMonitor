using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfMonitor;
using WebApplication.Interfaces;
using WebApplication.Services;

namespace WebApplication.Pages.Metrics
{
    public class HTTP_RequestsModel : PageModel
    {
        private readonly IMetricService<Http_Request> _httpMetricService = new MetricService<Http_Request>();
        public List<Http_Request> http { get; set; } = new List<Http_Request>();

        // Counter that detects when 5 seconds pass so HTTP get requests are sent every 5 seconds
        // Will decide later on oldStamp, automatically set to a month previous to current time (gets data for a month range)
        private DateTime oldStamp = DateTime.Today.AddMonths(-1).ToUniversalTime();
        private DateTime newStamp = DateTime.Now.ToUniversalTime();
        public async Task OnGet()
        {
            newStamp = DateTime.Now.ToUniversalTime();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/");

            // Converting DateTime to string that is accepted by HTTP requests
            String httpGetRequestEnd = FetchDataService.convertDateTime(oldStamp) + "&end="
                + FetchDataService.convertDateTime(newStamp);

            HttpResponseMessage httpResponse = await client.GetAsync("api/v1/HTTP??STUFF/Daterange?start=" + httpGetRequestEnd);
            _httpMetricService.updateUsingHttpResponse(httpResponse);

            if (httpResponse.IsSuccessStatusCode)
            {
                // Updates CPU_Usage list and totalCPU to calculate new average
                List<Http_Request> addOn = await _httpMetricService.getServiceUsage();
                foreach (Http_Request h in addOn)
                {
                    http.Add(h);
                }
            }
        }
    }
}