using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class TicketTypeGroup : IdentifierWithName
    {
        [JsonProperty(PropertyName = "applyCapacity")]
        public bool? ApplyCapacity { get; set; }
		[JsonProperty(PropertyName = "description")]
		public string Desc { get; set; }
		[JsonProperty(PropertyName = "products")]
        public List<Identifier> Products { get; set; }
        [JsonProperty(PropertyName = "series")]
        public List<Identifier> Series { get; set; }
    }
}
