using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.TimeTask;

namespace WebApi.Interfaces
{
    public interface IClockServiceStats
    {
        Task<IReadOnlyCollection<string>> GetJobGroupNames();
        Task<IReadOnlyCollection<string>> GetJobNamesForGroup(string groupName);
        Task<TimeTaskBase> GetTaskBase(string jobName, string groupName);
    }
}