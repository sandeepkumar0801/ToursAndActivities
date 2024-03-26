using Newtonsoft.Json;
using System.Collections.Generic;


namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class AvailabilityTimeSlotRS
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public List<ResultAvailabilityTimeSlot> ResultAvailabilityTimeSlot { get; set; }
    }

    public class ResultAvailabilityTimeSlot
    {
        [JsonProperty(PropertyName = "tourOptionId")]
        public int TourOptionId { get; set; }
        [JsonProperty(PropertyName = "timeSlotId")]
        public string TimeSlotId { get; set; }
        [JsonProperty(PropertyName = "timeSlot")]
        public string TimeSlot { get; set; }

        [JsonProperty(PropertyName = "available")]
        public int Available { get; set; }

        [JsonProperty(PropertyName = "adultPrice")]
        public decimal AdultPrice { get; set; }

        [JsonProperty(PropertyName = "childPrice")]
        public decimal ChildPrice { get; set; }

        [JsonProperty(PropertyName = "infantPrice")]
        public decimal InfantPrice { get; set; }

        [JsonProperty(PropertyName = "isDynamicPrice")]
        public bool IsDynamicPrice { get; set; }
        //for Isango Filter Purpose, otherwise not filter correctly
        public int TransferId { get; set; }
    }


}
