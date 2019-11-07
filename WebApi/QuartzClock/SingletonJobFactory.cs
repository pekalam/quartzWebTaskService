using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace WebApi.QuartzClock
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = (IJob)_serviceProvider.GetRequiredService(bundle.JobDetail.JobType);
            return job;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
