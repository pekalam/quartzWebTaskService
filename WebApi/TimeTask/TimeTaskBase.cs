using System;
using WebApi.Models;

namespace WebApi.TimeTask
{
    public abstract class TimeTaskBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime StartDate { get; set; }
        public Uri Endpoint { get; set; }
        public string Type { get; set; }
        public abstract Type JobType { get; protected set; }
        public virtual InfoResponse GetInfoResponse() => new InfoResponse(Id);
        public abstract ICallData GetCallData();
        public string TaskId => $"{this.GetType().Name}-{Id.ToString()}";
    }
}