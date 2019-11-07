using System;
using Newtonsoft.Json;

namespace TimeTaskService.HTTP
{


    public class TimeTaskBaseDTO
    {
        [JsonRequired] public DateTime StartDate { get; set; }
        [JsonRequired] public Uri Endpoint { get; set; }
        [JsonRequired] public string Type { get; set; }
    }
}