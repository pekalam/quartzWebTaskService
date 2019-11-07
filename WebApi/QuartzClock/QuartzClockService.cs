using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using Quartz.Spi;
using Quartz.Util;
using WebApi.Models;
using WebApi.QuartzClock.Jobs;
using WebApi.Interfaces;
using WebApi.QuartzClock.Triggers;
using WebApi.TimeTask;

namespace WebApi.QuartzClock
{
    public class QuartzLogger : ILogProvider
    {
        private ILogger _logger;

        public QuartzLogger(ILogger logger)
        {
            _logger = logger;
        }

        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) => {
                if (func != null)
                {
                    _logger.LogInformation(func.Invoke());
                }

                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            return null;
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            return null;

        }
    }

    public class QuartzClockService : IClockService, IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private IScheduler _scheduler;
        private TimeTaskTriggerFactory _taskTriggerFactory;
        private readonly ILogger<QuartzClockService> _logger;

        public QuartzClockService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            TimeTaskTriggerFactory taskTriggerFactory,
            ILogger<QuartzClockService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _taskTriggerFactory = taskTriggerFactory;
            _logger = logger;
            LogProvider.SetCurrentLogProvider(new QuartzLogger(logger));
        }

        private IJobDetail CreateJob(Type jobType, Type timeTaskType, TimeTaskBase timeTask)
        {
            return JobBuilder
                .Create(jobType)
                .WithIdentity($"{timeTaskType.Name}-{timeTask.Id.ToString()}", timeTask.Type)
                .UsingJobData("TimeTask", JsonConvert.SerializeObject(timeTask,
                    new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }))
                .Build();
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _jobFactory;
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }

        public async Task<InfoResponse> ScheduleTimeTask(Type jobType, Type taskClassType, TimeTaskBase timeTask)
        {
            var job = CreateJob(jobType, taskClassType, timeTask);
            var trigger = _taskTriggerFactory.CreateTrigger(timeTask);

            var offset = await _scheduler.ScheduleJob(job, trigger);

            _logger.LogInformation($"{taskClassType} scheduled on {timeTask.StartDate} offset: {offset}");
            return timeTask.GetInfoResponse();
        }

        public async Task<bool> CancelTimeTask(Type taskClassType, string taskGroup, Guid taskId)
        {
            return await _scheduler.DeleteJob(JobKey.Create($"{taskClassType.Name}-{taskId.ToString()}", taskGroup));
        }

        public async Task<IReadOnlyCollection<string>> GetJobNamesForGroup(string groupName)
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(groupName));
            return jobKeys.Select(k => k.Name).ToList();
        }

        public Task<IReadOnlyCollection<string>> GetJobGroupNames()
        {
            return _scheduler.GetJobGroupNames();
        }

        public async Task<TimeTaskBase> GetTaskBase(string jobName, string groupName)
        {
            var jobDetail = await _scheduler.GetJobDetail(JobKey.Create(jobName, groupName));
            
            var taskBase = 
            JsonConvert.DeserializeObject<TimeTaskBase>(jobDetail.JobDataMap.GetString("TimeTask"),
                new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.All});
            return taskBase;
        }
    }
}