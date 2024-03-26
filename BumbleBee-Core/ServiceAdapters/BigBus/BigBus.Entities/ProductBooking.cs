using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class ProductBooking
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "dateOfTravel")]
        public DateTime? DateOfTravel { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<ItemBase> Items { get; set; }
    }
}