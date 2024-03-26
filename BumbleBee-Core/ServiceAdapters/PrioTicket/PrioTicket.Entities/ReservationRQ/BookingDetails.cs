using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.ReservationRQ
{
    public class BookingDetails
    {
        [JsonProperty(PropertyName = "ticket_type")]
        public string TicketType { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "extra_options")]
        public ExtraOptions[] ExtraOptions { get; set; }
    }
}