using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TimeTaskService.HTTP;
using WebApi.TimeTask;

namespace WebApi.Controllers
{
    [Route("stats")]
    public class TimeTaskStatsController : Controller
    {
        private readonly TaskStatsService _taskStatsService;

        public TimeTaskStatsController(TaskStatsService taskStatsService)
        {
            _taskStatsService = taskStatsService;
        }

        [HttpGet("groups"), Authorize(Roles = "ManagmentApp")]
        public IEnumerable<TaskGroupStats> AllGroupsStats()
        {
            return _taskStatsService.GetAllGroupsStats();
        }

        [HttpGet("groups/{groupName}"), Authorize(Roles = "ManagmentApp")]
        public IEnumerable<TaskStats> JobsStatsInGroup(string groupName)
        {
            return _taskStatsService.GetJobStatsInGroup(groupName);
        }

        [HttpGet("tasks/{groupName}/{taskId}"), Authorize(Roles = "ManagmentApp")]
        public JObject TaskStats(string groupName, string taskId)
        {
            return _taskStatsService.GetTaskStats(taskId, groupName);
        }

        [HttpGet("tasks"), Authorize(Roles = "ManagmentApp")]
        public IEnumerable<TaskStats> AllJobsStats()
        {
            return _taskStatsService.GetAllJobsStats();
        }
    }
}
