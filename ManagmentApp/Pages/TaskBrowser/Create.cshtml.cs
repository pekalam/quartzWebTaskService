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
    public class CreateModel : PageModel
    {
        private readonly ITimeTaskServiceClient _taskServiceClient;

        [TempData]
        public string Notification { get; set; }

        public CreateModel(ITimeTaskServiceClient taskServiceClient)
        {
            _taskServiceClient = taskServiceClient;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(EchoTimeTaskDTO taskDto)
        {
            try
            {
                var response = await _taskServiceClient.ScheduleEchoTask(taskDto);
                Notification = $"Created task with ID: {response.Id.ToString()}";
                return RedirectToPage("Tasks");
            }
            catch (Exception)
            {
                Notification = "Cannot create a task";
                return Page();
            }
        }
    }
}