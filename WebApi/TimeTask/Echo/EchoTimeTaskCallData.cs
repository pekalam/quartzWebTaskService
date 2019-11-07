using System;

namespace WebApi.TimeTask.Echo
{
    public class EchoTimeTaskCallData : ICallData
    {
        public Guid Id { get; }
        public object Values { get; }

        public EchoTimeTaskCallData(Guid id, object values)
        {
            Id = id;
            Values = values;
        }
    }
}