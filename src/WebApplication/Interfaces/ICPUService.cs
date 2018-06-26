using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using PerfMonitor;

namespace WebApplication.Interfaces
{
    public interface ICPUService
    {
        void updateUsingHttpResponse(HttpResponseMessage response);
        Task<CPU_Usage> getCPUUsage();
    }
}