using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.SignalR;
<<<<<<< HEAD
using Microsoft.Owin;
using Owin;
=======
//using Microsoft.Owin;
//using Owin;
>>>>>>> bb69733ebca8fee66d098f2e4339df1f7d1cae0b
using WebApplication.Services;
using WebApplication.Interfaces;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.SpaServices;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

<<<<<<< HEAD
=======
        /*
>>>>>>> bb69733ebca8fee66d098f2e4339df1f7d1cae0b
        public static void COnfigureSignalR(IAppBuilder app)
        {
            //app.MapSignalR();
        }
<<<<<<< HEAD
=======
        */
>>>>>>> bb69733ebca8fee66d098f2e4339df1f7d1cae0b

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //Services.AddTransient<IMetricService, MetricService>(); 

            app.UseMvc();
        }
    }
}
