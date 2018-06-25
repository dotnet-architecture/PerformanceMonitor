using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using PerfMonitor;
using WebApplication.Interfaces;
using Newtonsoft.Json; 

namespace WebApplication.Services
{
    public class MetricService : IMetricService
    {
        CPU_Usage cpu = new CPU_Usage();

        Mem_Usage mem = new Mem_Usage();

        public void updateUsingHttpResponse(HttpResponseMessage response)
        {
            String string_response = response.Content.ReadAsAsync<String>().Result;

            // Desearilizes response JSON file 
            User objects = JsonConvert.DeserializeObject(string_response);
            
            // Desearilizes response JSON file 
            // updates CPU_Usage
            // updates Mem_Usage

        }
        public async Task<CPU_Usage> getCPUUsage()
        {
            // await Http call
            return cpu;
        }
        public async Task<Mem_Usage> getMemUsage()
        {
            // await Http call
            return mem;
        }
    }

    public class User
    {
        [JsonProperty("CPU")]
        public CPU_Usage CPU { get; set; }

        [JsonProperty("Memory")]
        public Mem_Usage Memory { get; set; }

        /*
        [JsonProperty("Http")]
        public string Http { get; set; }

        [JsonProperty("Exceptions")]
        public string Exceptions { get; set; }
        */

    }
}