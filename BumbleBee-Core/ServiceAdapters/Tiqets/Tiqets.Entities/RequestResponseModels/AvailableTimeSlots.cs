using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class AvailableTimeSlots
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "timeslots")]
        public List<TimeSlot> TimeSlots { get; set; }
    }

    public class TimeSlot
    {
        [JsonProperty(PropertyName = "timeslot")]
        public string Slot { get; set; }

        [JsonProperty(PropertyName = "is_available")]
        public bool IsAvailable { get; set; }
    }
}