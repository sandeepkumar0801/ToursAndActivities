using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class Products
    {
        [JsonProperty(PropertyName = "product")]
        public List<ProductBooking> Product { get; set; }
    }
}