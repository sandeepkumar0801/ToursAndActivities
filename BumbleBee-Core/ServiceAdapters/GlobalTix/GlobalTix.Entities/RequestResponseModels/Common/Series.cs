using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class Series : IdentifierWithName
    {
        [JsonProperty(PropertyName = "start")]
        public DateTime StartDateTime { get; set; }
        [JsonProperty(PropertyName = "end")]
        public DateTime EndDateTime { get; set; }
        [JsonProperty(PropertyName = "daysOfWeek")]
        public List<int> DaysOfWeek { get; set; }
        [JsonProperty(PropertyName = "events")]
        public List<Event> Events { get; set; }
    }
}
