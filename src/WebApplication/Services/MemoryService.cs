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
    public class MemoryService : IMemoryService
    {
        private Mem_Usage mem = new Mem_Usage();
        public void updateUsingHttpResponse(HttpResponseMessage response)
        {

            var result = response.Content.ReadAsStringAsync().Result;

            // Desearilizes response JSON file 
            var deserial_obj = JsonConvert.DeserializeObject<Mem_Usage>(result, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            // Updates cpu
            mem = deserial_obj;
        }
        public async Task<Mem_Usage> getMemoryUsage()
        {
            return mem;
        }
    }
}