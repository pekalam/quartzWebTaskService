using System;
using Newtonsoft.Json;
using Quartz;
using WebApi.Models;
using Remotion.Linq.Clauses;
using WebApi.TimeTask;

namespace WebApi.QuartzClock.Triggers
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }

    public class TimeTaskTriggerFactory
    {
        private IDateTimeProvider _dateTimeProvider;

        public TimeTaskTriggerFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        private TimeSpan GetTimeSpan(TimeTaskBase timeTask)
        {
            var currentDateTime = _dateTimeProvider.GetCurrentDateTime();
            if (timeTask.StartDate.CompareTo(currentDateTime) == -1)
            {
                throw new ArgumentException("Invalid task StartDate");
            }
            var timeSpan = timeTask.StartDate - currentDateTime;
            return timeSpan;
        }

        public ITrigger CreateTrigger<T>(T timeTask) where T : TimeTaskBase
        {
            var timeSpan = GetTimeSpan(timeTask);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"{nameof(T)}-{timeTask.Id.ToString()}", timeTask.Type)
                .StartAt(new DateTimeOffset(timeTask.StartDate))
                .Build();


            return trigger;
        }
    }
}