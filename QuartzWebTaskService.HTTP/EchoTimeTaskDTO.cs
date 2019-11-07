using Newtonsoft.Json.Linq;

namespace TimeTaskService.HTTP
{


    public class EchoTimeTaskDTO : TimeTaskBaseDTO
    {
        public JObject Values { get; set; }
    }
}