using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Response
{
    public class BookingDetails
    {
        [JsonProperty(PropertyName = "venue_name")]
        public string VenueName { get; set; }

        [JsonProperty(PropertyName = "code_type")]
        public string CodeType { get; set; }

        [JsonProperty(PropertyName = "group_code")]
        public string GroupCode { get; set; }

        [JsonProperty(PropertyName = "ticket_details")]
        public TicketDetails[] TicketDetails { get; set; }
    }
}