using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class CancelReservationResult
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class CancelReservationResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "cancelReservationResult")]
        public CancelReservationResult CancelReservationResult { get; set; }
    }
}