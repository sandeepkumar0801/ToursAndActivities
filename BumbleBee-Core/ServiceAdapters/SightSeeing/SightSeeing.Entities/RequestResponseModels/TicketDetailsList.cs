using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class TicketDetailsList : TicketBase
    {
        [JsonProperty("obj")]
        public List<TicketDetail> TicketDetail { get; set; }
    }
}
