using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using WebApplication.Pages;
using DataTransfer;
using Newtonsoft.Json;

namespace WebApplication
{
    public class FetchDataService
    {
        // Generic method that takes timestamps and makes a call to the API based off of the class T
        public static async Task<List<T>> getData<T>(DateTime oldStamp, DateTime newStamp)
        {
            // Creating HttpClient that will call the web api
            HttpClient client = new HttpClient();
            // Constructing url that will be called, the domain is hardcoded for now, will be more variable in the future
            client.BaseAddress = new Uri(WebApplication.Startup.apiDomain);

            // Constructing string that will pass timestamps to web api controllers
            String dateRange = convertDateTime(oldStamp) + "&end=" + convertDateTime(newStamp);
            // Passing session information to the web api controllers
            String sessionId = "&id=" + IndexModel.userSession.Id.ToString(); 

            // Specifying which controller to call upon based off the object of T
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
                type = "Exception";
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
                type = "error"; // Should never hit this because T can only take on values defined above
            }

            // Stringing all components of http request together and actually calling web api
            HttpResponseMessage response = await client.GetAsync("api/v1/" + 
                type + 
                "/Daterange?start=" + 
                dateRange +
                sessionId);

            List<T> data = new List<T>();
            if (response.IsSuccessStatusCode) // If the response is successfull, update data
            {
                var result = response.Content.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject<List<T>>(result);
            }

            return data; 
        }

        // getUpdatedData method gets information for metrics based off of a daterange whereas the session 
        // RETURNALL controller doesn't require a daterange, so getSessionData is separated from getUpdatedData
        public static async Task<List<Session>> getSessionData()
        {
            // Creating HttpClient that will call the web api
            HttpClient client = new HttpClient();
            // Constructing url that will be called, the domain is hardcoded for now, will be more variable in the future
            client.BaseAddress = new Uri(WebApplication.Startup.apiDomain);
            HttpResponseMessage response = await client.GetAsync("RETURNALL");

            List<Session> sessionData = new List<Session>();
            if (response.IsSuccessStatusCode) // If the response is successfull, update sessionData
            {
                var result = response.Content.ReadAsStringAsync().Result;
                sessionData = JsonConvert.DeserializeObject<List<Session>>(result);
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
