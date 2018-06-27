using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using PerfMonitor;

namespace WebApplication.Interfaces
{
    public interface IMemoryService
    {
        void updateUsingHttpResponse(HttpResponseMessage response);
        Task<Mem_Usage> getMemoryUsage();
    }
}