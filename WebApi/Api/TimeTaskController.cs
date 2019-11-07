using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Auth;
using WebApi.Models;
using TimeTaskService.HTTP;
using WebApi.TimeTask;

namespace WebApi.Controllers
{
    [Route("task")]
    public class TimeTaskController : Controller
    {
        private readonly TaskSchedulingService _taskSchedulingService;

        public TimeTaskController(TaskSchedulingService taskSchedulingService)
        {
            _taskSchedulingService = taskSchedulingService;
        }

        [HttpPost("set"), Authorize(Roles = "Client")]
        public async Task<InfoResponse> Set([FromBody] JObject timeTask)
        {
            var infoResponse = await _taskSchedulingService.ScheduleTimeTask(timeTask);
            StatusCode((int) HttpStatusCode.OK);
            return infoResponse;
        }

        [HttpPost("cancel"), Authorize(Roles = "Client")]
        public async Task Cancel([FromQuery] CancelTaskDto cancelTaskDto)
        {
            var canceled = await _taskSchedulingService.CancelTimeTask(cancelTaskDto.Type, cancelTaskDto.Id);
            if (canceled)
            {
                StatusCode((int) HttpStatusCode.OK);
            }
            else
            {
                StatusCode((int) HttpStatusCode.BadRequest);
            }
        }


        [HttpGet("test"), Authorize(AuthenticationSchemes = "X-API-Key", Roles = "Client")]
        public string Test()
        {
            return "test";
        }

        [HttpGet("test2"), Authorize(AuthenticationSchemes = "X-API-Key", Roles = "Client,ManagmentApp")]
        public string Test2()
        {
            return "test2";
        }
    }
}
