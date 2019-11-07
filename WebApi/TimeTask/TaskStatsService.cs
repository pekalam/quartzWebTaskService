using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TimeTaskService.HTTP;
using WebApi.Interfaces;

namespace WebApi.TimeTask
{
    public class TaskStatsService
    {
        private readonly IClockServiceStats _clockServiceStats;
        private readonly TimeTaskConvert _timeTaskConvert;

        public TaskStatsService(IClockServiceStats clockServiceStats, TimeTaskConvert timeTaskConvert)
        {
            _clockServiceStats = clockServiceStats;
            _timeTaskConvert = timeTaskConvert;
        }

        public IEnumerable<TaskStats> GetJobStatsInGroup(string groupName)
        {
            var groupJobsStats = new List<TaskStats>();
            var jobNames = _clockServiceStats.GetJobNamesForGroup(groupName).Result;
            foreach (var job in jobNames)
            {
                var stats = new TaskStats();
                var taskBase = _clockServiceStats.GetTaskBase(job, groupName).Result;
                stats.TaskId = taskBase.TaskId;
                stats.Endpoint = taskBase.Endpoint;
                stats.StartDate = taskBase.StartDate;
                stats.Id = taskBase.Id;
                stats.Type = taskBase.Type;
                stats.Group = groupName;
                groupJobsStats.Add(stats);
            }
            return groupJobsStats;
        }

        public IEnumerable<TaskGroupStats> GetAllGroupsStats()
        {
            var jobGroupStats = new List<TaskGroupStats>();

            var groupNames = _clockServiceStats.GetJobGroupNames().Result;
            foreach (var name in groupNames)
            {
                var stats = new TaskGroupStats();
                stats.GroupName = name;
                stats.TaskCount = _clockServiceStats.GetJobNamesForGroup(name).Result.Count;
                jobGroupStats.Add(stats);
            }

            return jobGroupStats;
        }

        public JObject GetTaskStats(string jobId, string groupName)
        {
            var taskBase = _clockServiceStats.GetTaskBase(jobId, groupName).Result;
            return _timeTaskConvert.ToJson(taskBase);
        }

        public IEnumerable<TaskStats> GetAllJobsStats()
        {
            var groupJobsStats = new List<TaskStats>();

            var groupNames = _clockServiceStats.GetJobGroupNames().Result;
            foreach (var name in groupNames)
            {
                groupJobsStats.AddRange(GetJobStatsInGroup(name));
            }

            return groupJobsStats;
        }
    }
}