using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class Event : Identifier
    {
        [JsonProperty(PropertyName = "total")]
        public int TotalCapacity { get; set; }
        [JsonProperty(PropertyName = "available")]
        public int AvailableCapacity { get; set; }
        [JsonProperty(PropertyName = "time")]
        public DateTime EventDateTime { get; set; }
    }
}
