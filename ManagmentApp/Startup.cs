using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagmentApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestEase;

namespace ManagmentApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.None;
            });

            services.Configure<TimeTaskServiceSettings>(settings =>
            {
                settings.Address = Environment.GetEnvironmentVariable("ServiceAddress") != null ?
                   Environment.GetEnvironmentVariable("ServiceAddress") : "http://localhost:5001";
                settings.ApiKey = Environment.GetEnvironmentVariable("ApiKey") != null
                    ? Environment.GetEnvironmentVariable("ApiKey")
                    : "testm";
            });

            services.AddSingleton<ITimeTaskServiceClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptionsMonitor<TimeTaskServiceSettings>>().CurrentValue;
                var client = RestClient.For<ITimeTaskServiceClient>(settings.Address);
                client.ApiKey = settings.ApiKey;
                return client;
            });

            services.AddMvc().AddRazorPagesOptions(options =>
            {
 
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
