using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class ReservationRequestObject
    {
        [JsonProperty(PropertyName = "reservationRequest")]
        public ReservationRequest ReservationRequest { get; set; }
    }

    public class ReservationRequest
    {
        [JsonProperty(PropertyName = "products")]
        public Products Products { get; set; }
    }
}