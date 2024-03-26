using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class CancelReservationRequestObject
    {
        [JsonProperty(PropertyName = "cancelReservationRequest")]
        public CancelReservationRequest CancelReservationRequest { get; set; }
    }

    public class CancelReservationRequest
    {
        [JsonProperty(PropertyName = "reservationReference")]
        public string ReservationReference { get; set; }
    }
}