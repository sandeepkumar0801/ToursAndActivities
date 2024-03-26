using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.ReservationRQ
{
    public class ReservationRq : EntityBase
    {
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public ReservationRqData Data { get; set; }
    }
}