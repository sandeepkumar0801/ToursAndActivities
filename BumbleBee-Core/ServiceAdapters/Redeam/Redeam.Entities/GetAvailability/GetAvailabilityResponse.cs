using Newtonsoft.Json;

using System;

namespace ServiceAdapters.Redeam.Redeam.Entities.GetAvailability
{
    public class AvailabilityResponse
    {
        [JsonProperty("availability")]
        public Availability Availability { get; set; }

        [JsonProperty("bookable")]
        public bool Bookable { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Availability
    {
        [JsonProperty("capacity")]
        public long Capacity { get; set; }

        [JsonProperty("end")]
        public DateTimeOffset End { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("start")]
        public DateTimeOffset Start { get; set; }
    }

    public class Meta
    {
        [JsonProperty("reqId")]
        public Guid ReqId { get; set; }
    }
}