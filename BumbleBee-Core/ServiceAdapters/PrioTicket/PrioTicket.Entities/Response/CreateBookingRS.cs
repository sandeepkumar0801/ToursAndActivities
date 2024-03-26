using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Response
{
    public class CreateBookingRs : EntityBase
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }
}