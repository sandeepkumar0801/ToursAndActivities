using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class CancelBooking
    {
        [JsonProperty(PropertyName = "bookingReference")]
        public string BookingReference { get; set; }
    }

    public class CancelBookingRequest
    {
        [JsonProperty(PropertyName = "cancelBookingRequest")]
        public CancelBooking CancelBookingReq { get; set; }
    }
}