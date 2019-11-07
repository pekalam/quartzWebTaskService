using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Models;
using WebApi.Interfaces;

namespace WebApi.TimeTask
{
    public class TaskSchedulingService
    {
        private TimeTaskConvert _timeTaskConvert;
        private IClockService _clockService;
        private ILogger<TaskSchedulingService> _logger;

        public TaskSchedulingService(TimeTaskConvert timeTaskConvert, IClockService clockService, ILogger<TaskSchedulingService> logger)
        {
            _timeTaskConvert = timeTaskConvert;
            _clockService = clockService;
            _logger = logger;
        }

        private async Task<InfoResponse> TryScheduleTimeTask(JObject json)
        {
            try
            {
                var task = _timeTaskConvert.FromJson(json);
                var response = await _clockService.ScheduleTimeTask(task.JobType, task.GetType(), task);
                return response;
            }
            catch (ArgumentException e)
            {
                _logger.LogDebug($"{e.Message}");
                throw new WebApiException(HttpStatusCode.BadRequest, "", e);
            }
            catch (JsonSerializationException e)
            {
                _logger.LogDebug($"{e.Message}");
                throw new WebApiException(HttpStatusCode.BadRequest, "", e);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogDebug($"{e.Message}");
                throw new WebApiException(HttpStatusCode.InternalServerError, "", e);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                throw new WebApiException(HttpStatusCode.InternalServerError, "", e);
            }
        }

        public async Task<InfoResponse> ScheduleTimeTask(JObject json)
        {
            var response = await TryScheduleTimeTask(json);
            return response;
        }


        private async Task<bool> TryCancelTimeTask(Type taskType, string taskGroup, Guid taskId)
        {
            try
            {
                var canceled = await _clockService.CancelTimeTask(taskType , taskGroup, taskId);
                return canceled;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                throw new WebApiException(HttpStatusCode.InternalServerError, "", e);
            }
        }

        public async Task<bool> CancelTimeTask(string strTaskType, Guid taskId)
        {
            ValueTuple<Type,Type> dtoTaskTypeTuple = TimeTaskTypeMap.Map.GetValueOrDefault(strTaskType);
            if (dtoTaskTypeTuple.Item1 == null || dtoTaskTypeTuple.Item2 == null)
            {
                throw new WebApiException(HttpStatusCode.BadRequest, $"Invalid task type {strTaskType}");
            }

            return await TryCancelTimeTask(dtoTaskTypeTuple.Item2, strTaskType, taskId);
        }
    }
}
