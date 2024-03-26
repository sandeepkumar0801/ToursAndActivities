using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Response
{
    public class TicketDetails
    {
        [JsonProperty(PropertyName = "ticket_name")]
        public string TicketName { get; set; }

        [JsonProperty(PropertyName = "ticket_type")]
        public string TicketType { get; set; }

        [JsonProperty(PropertyName = "ticket_code")]
        public string TicketCode { get; set; }
    }
}