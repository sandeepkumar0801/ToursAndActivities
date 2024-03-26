using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class IssueRequest
    {
        public List<TicketList> TicketList { get; set; }

        [JsonProperty("API-KEY")]
        public string ApiKey { get; set; }
    }
}