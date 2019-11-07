using System;
using WebApi.QuartzClock.Jobs;

namespace WebApi.TimeTask.Echo
{
    public class EchoTimeTask : TimeTaskBase
    {
        public object Values { get; set; }
        public override Type JobType { get; protected set; } = typeof(EchoJob);

        public override ICallData GetCallData()
        {
            return new EchoTimeTaskCallData(Id, Values);
        }
    }
}