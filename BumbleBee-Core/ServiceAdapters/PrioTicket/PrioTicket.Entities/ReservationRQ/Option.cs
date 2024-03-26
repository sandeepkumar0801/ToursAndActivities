using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.ReservationRQ
{
    public class Option
    {
        [JsonProperty(PropertyName = "option_id")]
        public string OptionId { get; set; }

        [JsonProperty(PropertyName = "count")]
        public string Count { get; set; }
    }
}