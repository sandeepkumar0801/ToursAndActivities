using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class ReservationRs : EntityBase
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public ReservationRsData Data { get; set; }
    }

    public class ReservationRsData
    {
        [JsonProperty(PropertyName = "reservation_reference")]
        public string ReservationReference { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }

        [JsonProperty(PropertyName = "booking_status")]
        public string BookingStatus { get; set; }
    }
}