using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagmentApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTaskService.HTTP;

namespace ManagmentApp.Pages.TaskBrowser
{
    public class TasksModel : PageModel
    {
        private readonly ITimeTaskServiceClient _taskServiceClient;

        public IEnumerable<TaskStats> TasksStats { get; set; } = new List<TaskStats>();

        public TasksModel(ITimeTaskServiceClient taskServiceClient)
        {
            _taskServiceClient = taskServiceClient;
        }

        public async Task OnGet()
        {
            TasksStats = await _taskServiceClient.GetTasksStats();
        }
    }
}