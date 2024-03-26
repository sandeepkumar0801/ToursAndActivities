using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class CancelReservationRq : EntityBase
    {
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public CancelReservationRqData Data { get; set; }
    }

    public class CancelReservationRqData
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }

        [JsonProperty(PropertyName = "reservation_reference")]
        public string ReservationReference { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }
    }
}