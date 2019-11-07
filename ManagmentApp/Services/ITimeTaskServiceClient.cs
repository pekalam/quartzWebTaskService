using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestEase;
using TimeTaskService.HTTP;
using WebApi.Models;

namespace ManagmentApp.Services
{
    public interface ITimeTaskServiceClient
    {
        [Header("X-API-Key")]
        string ApiKey { get; set; }

        [Get("stats/groups")]
        Task<IEnumerable<TaskGroupStats>> GetGroupsStats();

        [Post("task/set")]
        Task<InfoResponse> ScheduleEchoTask([Body] EchoTimeTaskDTO echoTimeTaskDto);

        [Get("stats/tasks")]
        Task<IEnumerable<TaskStats>> GetTasksStats();

        [Get("stats/tasks/{groupName}/{taskId}")]
        Task<JObject> GetSingleTaskStats([Path] string groupName, [Path] string taskId);
    }
}
