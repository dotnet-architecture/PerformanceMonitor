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
    public class CPUService<CPU_Usage>
    {
        private List<CPU_Usage> data = new List<CPU_Usage>();
        public void updateUsingHttpResponse(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;

            // Desearilizes response JSON file 
            var deserial_obj = JsonConvert.DeserializeObject<List<CPU_Usage>>(result);

            /*
            JsonConvert.DeserializeObject<List<CPU_Usage>>(result, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
            */

            // Updates data
            data = deserial_obj;
        }
        public async Task<List<CPU_Usage>> getServiceUsageCPU()
        {
            return data;
        }
    }
}