using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{

    public class TicketListRq : EntityBase
    {
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }
        [JsonProperty(PropertyName = "data")]
        public DataListRequest Data { get; set; }
    }

    public class DataListRequest
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }
    }
}