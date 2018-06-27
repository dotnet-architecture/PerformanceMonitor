using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace WebApplication.Interfaces
{
    public interface IMetricService<T>
    {
        void updateUsingHttpResponse(HttpResponseMessage response);
        Task<List<T>> getServiceUsage();
    }
}