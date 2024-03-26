using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class BulkAvailabilityResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "day_availabilities")]
        public Dictionary<string, TimeSlotAvailability> DayAvailability { get; set; }
    }

    public class TimeSlotAvailability
    {
        [JsonProperty(PropertyName = "timeslot_availabilities")]
        public Dictionary<string, VariantAvailability> TimeSlotAvailabilities { get; set; }

        [JsonProperty(PropertyName = "max_per_order")]
        public string MaxPerOrder { get; set; }
    }

    public class VariantAvailability
    {
        [JsonProperty(PropertyName = "variant_availabilities")]
        public Dictionary<string, MaxPerOrder> VariantAvailabilities { get; set; }

        [JsonProperty(PropertyName = "max_per_order")]
        public string MaxPerOrder { get; set; }
    }

    public class MaxPerOrder
    {
        [JsonProperty(PropertyName = "max_per_order")]
        public string Value { get; set; }
    }
}