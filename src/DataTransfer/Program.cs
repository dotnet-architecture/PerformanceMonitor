using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DataTransfer;

namespace DataTransfer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // uncomment below three lines to perform monitoring
            Monitor monitor = new Monitor();
            monitor.EnableHttp();
            monitor.DisableCPU();
            monitor.DisableMem();
            monitor.Record();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
