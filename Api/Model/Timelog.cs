using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace wrike_timer.Api.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Timelog
    {
        public string Id { get; set; }

        public string TaskId { get; set; }

        public string UserId { get; set; }

        public string CategoryId { get; set; }

        public float Hours { get; set; }

        public DateTime TrackedDate { get; set; }

        public string Comment { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TimelogCategory
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public bool Hidden { get; set; }
    }
}
