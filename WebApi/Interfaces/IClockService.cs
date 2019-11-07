using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using WebApi.Models;
using WebApi.TimeTask;

namespace WebApi.Interfaces
{
    public interface IClockService : IClockServiceStats
    {
        Task<InfoResponse> ScheduleTimeTask(Type jobType, Type taskClassType, TimeTaskBase timeTask);
        Task<bool> CancelTimeTask(Type taskClassType, string taskGroup, Guid taskId);
    }
}