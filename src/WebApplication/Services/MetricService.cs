﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using WebApplication.Interfaces;
using Newtonsoft.Json;

namespace WebApplication.Services
{
    public class MetricService<T> : IMetricService<T>
    {
        private List<T> data = new List<T>();

        // Updates data
        public void updateUsingHttpResponse(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;

            var deserial_obj = JsonConvert.DeserializeObject<List<T>>(result);

            data = deserial_obj;
        }
        public async Task<List<T>> getServiceUsage()
        {
            return data;
        }
    }
}