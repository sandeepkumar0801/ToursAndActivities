using Newtonsoft.Json;
using System;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class AvailableDays
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "days")]
        public DateTime[] Days { get; set; }
    }
}