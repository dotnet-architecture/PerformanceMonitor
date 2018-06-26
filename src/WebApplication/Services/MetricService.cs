using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using PerfMonitor;
using WebApplication.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication.Services
{
    public class MetricService : IMetricService
    {
        private CPU_Usage cpu = new CPU_Usage();

        private Mem_Usage mem = new Mem_Usage();
        public void updateUsingHttpResponse(HttpResponseMessage response)
        {

            var result = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine("string_response: " + result);

            // Desearilizes response JSON file 
            var deserial_obj = JsonConvert.DeserializeObject<CPU_Usage>(result, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            Console.WriteLine("deserialized JSON object has " + deserial_obj);

            // Updates cpu and mem
            cpu = deserial_obj;
            //mem = deserial_obj.Memory;

            if (cpu != null)
            {
                Console.WriteLine("testing CPU output: " + cpu.ToString());
            } else
            {
                Console.WriteLine("failure CPU output is null");
            }
        }
        public async Task<CPU_Usage> getCPUUsage()
        {
            return cpu;
        }
        public async Task<Mem_Usage> getMemUsage()
        {
            return mem;
        }
    }

    public class MetricWrapper
    {
        public List<Metric> metric { get; set; }
    }
    public class Metric
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