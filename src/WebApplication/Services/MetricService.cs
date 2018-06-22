using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using PerfMonitor;
using WebApplication.Interfaces;

namespace WebApplication.Services
{
    public class MetricService : IMetricService
    {

        CPU_Usage cpu = new CPU_Usage();
        Mem_Usage mem = new Mem_Usage();
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

        public void updateUsingHttpResponse(HttpResponseMessage response)
        {
            // Desearilizes response JSON file 
            // updates CPU_Usage
            // updates Mem_Usage
        }
    }
}
