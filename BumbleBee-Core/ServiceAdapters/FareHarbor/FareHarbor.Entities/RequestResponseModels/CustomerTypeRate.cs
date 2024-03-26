using Newtonsoft.Json;
using System;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CustomerTypeRate
    {
        [JsonProperty("Customer_Prototype")]
        public CustomerPrototype CustomerPrototype { get; set; }

        [JsonProperty("capacity")]
        public int? Capacity { get; set; }

        [JsonProperty("minimum_party_size")]
        public int? MinimumPartySize { get; set; }

        [JsonProperty("Customer_Type")]
        public CustomerType CustomerType { get; set; }

        [JsonProperty("maximum_party_size")]
        public int? MaximumPartySize { get; set; }

        public Int64 Pk { get; set; }
        public int Total { get; set; }
    }
}