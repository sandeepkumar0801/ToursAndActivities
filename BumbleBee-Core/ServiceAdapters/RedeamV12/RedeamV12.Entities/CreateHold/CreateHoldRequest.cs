using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.RedeamV12.RedeamV12.Entities.CreateHold
{
    public class CreateHoldRequest
    {
        [JsonProperty("hold")]
        public Hold Hold { get; set; }
    }

    public class Hold
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("at")]
        public string At { get; set; }

        [JsonProperty("priceId")]
        public string PriceId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("rateId")]
        public Guid RateId { get; set; }

        [JsonProperty("supplierId")]
        public Guid SupplierId { get; set; }

        [JsonProperty("travelerType")]
        public string TravelerType { get; set; }

        [JsonProperty("availabilityId")]
        public string AvailabilityId { get; set; }
    }
}