using System;

namespace TimeTaskService.HTTP
{
    public class TaskStats
    {
        public Guid Id { get; set; }
        public string TaskId { get; set; }
        public DateTime StartDate { get; set; }
        public Uri Endpoint { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
    }
}