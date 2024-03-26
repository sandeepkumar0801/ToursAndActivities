using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class CancelBookingRq : EntityBase
    {
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public DataCancelBooking Data { get; set; }
    }

    public class DataCancelBooking
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }

        [JsonProperty(PropertyName = "booking_reference")]
        public string BookingReference { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }
    }
}