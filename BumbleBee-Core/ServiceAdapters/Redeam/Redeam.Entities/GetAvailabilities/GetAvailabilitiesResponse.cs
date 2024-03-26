using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.Redeam.Redeam.Entities.GetAvailabilities
{
    public class AvailabilitiesResponse
    {
        [JsonProperty("availabilities")]
        public Availabilities Availabilities { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Availabilities
    {
        [JsonProperty("byProduct")]
        public ByProduct ByProduct { get; set; }

        [JsonProperty("byRate")]
        public ByRate ByRate { get; set; }

        [JsonProperty("expires")]
        public DateTime Expires { get; set; }

        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        [JsonProperty("retrieved")]
        public DateTime Retrieved { get; set; }
    }

    public class ByProduct
    {
        [JsonProperty("availability")]
        public List<Availability> Availability { get; set; }
    }

    public class Availability
    {
        [JsonProperty("capacity")]
        public long Capacity { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }
    }

    public class ByRate
    {
        [JsonProperty("string")]
        [JsonExtensionData]
        public Dictionary<string, JToken> String { get; set; }
    }

    public class Meta
    {
        [JsonProperty("reqId")]
        public Guid ReqId { get; set; }
    }
}