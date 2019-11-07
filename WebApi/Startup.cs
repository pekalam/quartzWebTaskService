using System;
using System.Collections.Specialized;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RabbitMQ.Client;
using WebApi.Auth;
using WebApi.Interfaces;
using WebApi.QuartzClock;
using WebApi.QuartzClock.Jobs;
using WebApi.QuartzClock.Triggers;
using WebApi.TimeTask;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace WebApi
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return Quartz.SystemTime.UtcNow.Invoke().DateTime;
        }
    }

    public class Startup
    {
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected virtual void ConfigureClockService(IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
            services.AddSingleton<TimeTaskConvert>();
            services.AddSingleton<TimeTaskTriggerFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>(provider =>
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                return new StdSchedulerFactory(props);
            });
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<IClockService, QuartzClockService>();
            services.AddSingleton<IClockServiceStats>(provider => provider.GetRequiredService<IClockService>());
            services.AddSingleton(s => (IHostedService) s.GetRequiredService<IClockService>());
            services.AddSingleton<TaskSchedulingService>();
            services.AddSingleton<TaskStatsService>();
            services.AddTransient<EchoJob>();
        }

        protected virtual void ConfigureApiKeyAuthentication(IServiceCollection services)
        {
            var apiKeys = new ApiKeysStrings();

            apiKeys.ClientKey = Environment.GetEnvironmentVariable(nameof(apiKeys.ClientKey));
            apiKeys.ManagmentAppKey = Environment.GetEnvironmentVariable(nameof(apiKeys.ManagmentAppKey));
            services.AddSingleton(apiKeys);
            services.AddSingleton<ApiKeyService>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.Scheme;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.Scheme;
                })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.Scheme,
                    null);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHttpContextAccessor();


            ConfigureClockService(services);
            ConfigureApiKeyAuthentication(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<ILogger<Startup>>().LogError(serviceProvider.GetService<ApiKeysStrings>().ClientKey);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}");
            });
        }
    }
}