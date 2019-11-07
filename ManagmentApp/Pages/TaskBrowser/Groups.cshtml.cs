using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagmentApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTaskService.HTTP;

namespace ManagmentApp.Pages.TaskBrowser.Groups
{
    public class GroupsModel : PageModel
    {
        private readonly ITimeTaskServiceClient _taskServiceClient;

        public GroupsModel(ITimeTaskServiceClient taskServiceClient)
        {
            _taskServiceClient = taskServiceClient;
        }

        public IEnumerable<TaskGroupStats> GroupStats { get; private set; } = new List<TaskGroupStats>();

        public async Task OnGet()
        {
            GroupStats = await _taskServiceClient.GetGroupsStats();
        }
    }
}