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
    public class CPUService : ICPUService
    {
        private CPU_Usage cpu = new CPU_Usage();
        public void updateUsingHttpResponse(HttpResponseMessage response)
        {

            var result = response.Content.ReadAsStringAsync().Result;

            // Desearilizes response JSON file 
            var deserial_obj = JsonConvert.DeserializeObject<CPU_Usage>(result, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            // Updates cpu
            cpu = deserial_obj;
        }
        public async Task<CPU_Usage> getCPUUsage()
        {
            return cpu;
        }
    }
}