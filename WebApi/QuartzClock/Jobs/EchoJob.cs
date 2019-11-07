using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Quartz.Util;
using WebApi.Models;
using WebApi.Auth;
using WebApi.TimeTask;

namespace WebApi.QuartzClock.Jobs
{
    public class EchoJob : IJob
    {
        private readonly ILogger<EchoJob> _logger;
        private readonly ApiKeysStrings _apiKeys;

        public EchoJob(ILogger<EchoJob> logger, ApiKeysStrings apiKeys)
        {
            _logger = logger;
            _apiKeys = apiKeys;
        }

        private TimeTaskBase GetTaskBase(IJobExecutionContext context)
        {
            var timeTaskJson = context.JobDetail.JobDataMap.GetString("TimeTask");
            try
            {
                var task = JsonConvert.DeserializeObject<TimeTaskBase>(timeTaskJson,
                    new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.All});
                return task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var timeTask = GetTaskBase(context);

            _logger.LogInformation($"Executing EchoJob id:{timeTask.TaskId} startDate: {timeTask.StartDate.ToLocalTime()}");
            var callData = timeTask.GetCallData();
            var endpointUri = timeTask.Endpoint;
            var executor = new TimeTaskExecutor(endpointUri, callData, _logger, _apiKeys);
            var finished = await executor.Exec();
            if (!finished)
            {
                _logger.LogInformation($"EchoJob FAILED id:{timeTask.TaskId} startDate: {timeTask.StartDate.ToLocalTime()}");
            }
        }
    }
}