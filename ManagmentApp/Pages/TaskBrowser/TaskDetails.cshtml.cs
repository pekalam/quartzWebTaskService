using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagmentApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace ManagmentApp.Pages.TaskBrowser
{
    public class TaskDetailsModel : PageModel
    {
        private readonly ITimeTaskServiceClient _taskServiceClient;

        public string TaskDetails { get; private set; }
        public string TaskId { get; private set; }

        public TaskDetailsModel(ITimeTaskServiceClient taskServiceClient)
        {
            _taskServiceClient = taskServiceClient;
        }

        public async Task OnGet(string groupName, string taskId)
        {
            var json = await _taskServiceClient.GetSingleTaskStats(groupName, taskId);
            TaskId = json.GetValue("TaskId").ToString();
            TaskDetails = json.ToString().Replace(",", @"\r\n");
        }
    }
}