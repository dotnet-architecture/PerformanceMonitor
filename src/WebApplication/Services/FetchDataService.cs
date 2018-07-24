using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Services;
using WebApplication.Interfaces;
using System.Net.Http;
using WebApplication.Pages;
using DataTransfer; 

namespace WebApplication
{
    public class FetchDataService
    {
        public static async Task<List<T>> getUpdatedData<T>(DateTime oldStamp, DateTime newStamp)
        {
            IMetricService<T> _metricService = new MetricService<T>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54022/");

            String dateRange = convertDateTime(oldStamp) + "&end=" + convertDateTime(newStamp);
            String sessionId = "&id=" + IndexModel.userSession.Id.ToString(); 

            String type = "";

            if (typeof(T).ToString().Equals("DataTransfer.CPU_Usage"))
            {
                type = "CPU";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Mem_Usage"))
            {
                type = "Memory";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Http_Request"))
            {
                type = "HTTP";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Exceptions"))
            {
                type = "Exceptions";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Contention"))
            {
                type = "Contention";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.GC"))
            {
                type = "GC"; 
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Jit"))
            {
                type = "Jit";
            }
            else
            {
                type = "error"; //should never hit this 
            }

            HttpResponseMessage response = await client.GetAsync("api/v1/" + 
                type + 
                "/Daterange?start=" + 
                dateRange +
                sessionId);
            _metricService.updateUsingHttpResponse(response);

            List<T> addOn = new List<T>();

            if (response.IsSuccessStatusCode)
            {
                addOn = await _metricService.getServiceUsage();
            }

            return addOn;
        }

        public static async Task<List<Session>> getSessionData()
        {
            IMetricService<Session> _metricService = new MetricService<Session>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54022/");
            HttpResponseMessage response = await client.GetAsync("RETURNALL");
            _metricService.updateUsingHttpResponse(response);

            List<Session> sessionData = new List<Session>();

            if (response.IsSuccessStatusCode)
            {
                sessionData = await _metricService.getServiceUsage();
            }

            return sessionData; 
        }

        // Converting DateTIme to a string that Http request will accept
        public static String convertDateTime(DateTime d)
        {
            return d.Year.ToString("D4") + "-" + 
                d.Month.ToString("D2") + "-" + 
                d.Day.ToString("D2") + "T" + 
                d.Hour.ToString("D2") + "%3A" +
                d.Minute.ToString("D2") + "%3A" + 
                d.Second.ToString("D2") + "." +
                d.Millisecond.ToString("D3");
        }
    }
}
