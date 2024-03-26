using Newtonsoft.Json;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class TicketBase
    {
        [JsonProperty("status_code")]
        public string StatusCode { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        public object CbObject { get; set; }
    }
}