using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace wrike_timer.Api.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Workflow
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<Status> CustomStatuses { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Status
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusGroup Group { get; set; }
    }

    public enum StatusGroup
    {
        Active,
        Completed,
        Deferred,
        Cancelled
    }
}
