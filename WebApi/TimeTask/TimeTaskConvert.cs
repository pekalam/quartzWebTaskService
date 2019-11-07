using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeTaskService.HTTP;
using WebApi.TimeTask.Echo;

namespace WebApi.TimeTask
{
    public static class EchoTimeTaskAssembler
    {
        public static EchoTimeTask FromDTO(object dto)
        {
            var echoTimeTaskDto = (EchoTimeTaskDTO) dto;
            return new EchoTimeTask()
            {
                Type = echoTimeTaskDto.Type, Endpoint = echoTimeTaskDto.Endpoint, StartDate = echoTimeTaskDto.StartDate,
                Values = echoTimeTaskDto.Values
            };
        }
    }

    public static class TimeTaskAssembler
    {
        public static TimeTaskBase FromDTO(object dto, Type taskType)
        {
            if (taskType == typeof(EchoTimeTask))
            {
                return EchoTimeTaskAssembler.FromDTO(dto);
            }

            throw new ArgumentException($"Invalid argument taskType: {taskType}");
        }
    }

    public static class TimeTaskTypeMap
    {
        public static readonly Dictionary<string, ValueTuple<Type, Type>> Map =
            new Dictionary<string, ValueTuple<Type, Type>>()
            {
                {"echo", (typeof(EchoTimeTaskDTO), typeof(EchoTimeTask))}
            };
    };


    public class TimeTaskConvert
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TimeTaskConvert(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private void ValidateEndpoint(Uri endpoint)
        {
            var myHost = _httpContextAccessor.HttpContext.Request.Host;
            if (endpoint.Host == myHost.Host && endpoint.Port == myHost.Port)
            {
                throw new ArgumentException($"Cannot set {endpoint.ToString()}");
            }
        }

        public JObject ToJson<T>(T taskBase) where T : TimeTaskBase
        {
            var json =  JObject.FromObject(taskBase);
            json.Remove("JobType");
            return json;
        }

        public TimeTaskBase FromJson(JObject json)
        {
            string type;
            if (json.ContainsKey("Type"))
            {
                type = json.GetValue("Type").ToObject<string>();
            }
            else if (json.ContainsKey("type"))
            {
                type = json.GetValue("type").ToObject<string>();
            }
            else
            {
                throw new ArgumentException("Argument does not contain type property");
            }

            ValueTuple<Type, Type> timeTaskType;
            if (TimeTaskTypeMap.Map.TryGetValue(type, out timeTaskType))
            {
                var timeTaskDto = JsonConvert.DeserializeObject(json.ToString(), timeTaskType.Item1);
                var timeTask = TimeTaskAssembler.FromDTO(timeTaskDto, timeTaskType.Item2);
                ValidateEndpoint(timeTask.Endpoint);
                return timeTask;
            }
            else
            {
                throw new ArgumentException($"Invalid type: {type}");
            }
        }
    }
}