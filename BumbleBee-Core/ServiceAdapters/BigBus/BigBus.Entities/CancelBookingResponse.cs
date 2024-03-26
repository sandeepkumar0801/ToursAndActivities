using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class CancelBookingResult
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class CancelBookingResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "cancelBookingResult")]
        public CancelBookingResult CancelBookingResult { get; set; }
    }
}