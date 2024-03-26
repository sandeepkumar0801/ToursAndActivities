using Newtonsoft.Json;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class TicketList
    {
        public string TicketId { get; set; }
        public string TicketType { get; set; }
        public int Qty { get; set; }

        [JsonProperty("departure_date")]
        public string DepartureDate { get; set; }
    }
}