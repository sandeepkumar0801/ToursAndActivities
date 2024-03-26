using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class TicketTypesRS
    {
        [JsonProperty(PropertyName = "data")]
        public List<TicketType> Data { get; set; }
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
    }

}
