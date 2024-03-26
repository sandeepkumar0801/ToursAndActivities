using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Request
{
    public class ExtraOptions
    {
        [JsonProperty(PropertyName = "extra_option_id")]
        public string ExtraOptionId { get; set; }

        [JsonProperty(PropertyName = "options")]
        public Option[] Options { get; set; }
    }
}