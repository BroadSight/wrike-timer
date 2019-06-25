using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace wrike_timer.Api.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Task
    {
        public string Id { get; set; }

        public string Title { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusGroup Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Importance Importance { get; set; }

        public string CustomStatusId { get; set; }

        public string Permalink { get; set; }

        public List<KeyValuePair<string, string>> Metadata { get; set; }
    }

    public enum Importance
    {
        High,
        Normal,
        Low
    }
}
