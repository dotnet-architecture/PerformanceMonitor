using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using WebApplication.Interfaces;
using Newtonsoft.Json;
using System;

namespace WebApplication.Services
{
    public class MetricService<T> : IMetricService<T>
    {
        private List<T> data = new List<T>();

        // Updates data
        public void updateUsingHttpResponse(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;

            data = JsonConvert.DeserializeObject<List<T>>(result);
        }

        public async Task<List<T>> getServiceUsage()
        {
            return data;
        }
    }
}