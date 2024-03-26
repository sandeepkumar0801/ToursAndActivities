using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class AvailablityRq : EntityBase
    {
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public AvailablityData Data { get; set; }
    }

    public class AvailablityData
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }

        [JsonProperty(PropertyName = "ticket_id")]
        public string TicketId { get; set; }

        [JsonProperty(PropertyName = "from_date")]
        public string FromDate { get; set; }

        [JsonProperty(PropertyName = "to_date")]
        public string ToDate { get; set; }
    }
}