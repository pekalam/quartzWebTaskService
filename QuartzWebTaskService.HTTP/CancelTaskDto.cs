using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TimeTaskService.HTTP
{
    public class CancelTaskDto
    {
        [JsonRequired]
        public Guid Id { get; set; }
        [JsonRequired]
        public string Type { get; set; }
    }
}
