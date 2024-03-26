using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.Redeam.Redeam.Entities.GetRates
{
    public class GetRatesResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("rates")]
        public List<Rate> Rates { get; set; }
    }

    public class Meta
    {
        [JsonProperty("reqId")]
        public Guid ReqId { get; set; }
    }

    public class Rate
    {
        [JsonProperty("cancelable")]
        public bool Cancelable { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("cutoff")]
        public int Cutoff { get; set; }

        [JsonProperty("holdable")]
        public bool Holdable { get; set; }

        [JsonProperty("holdablePeriod")]
        public int HoldablePeriod { get; set; }

        [JsonProperty("hours")]
        public List<Hour> Hours { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("maxTravelers")]
        public int MaxTravelers { get; set; }

        [JsonProperty("minTravelers")]
        public int MinTravelers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }

        [JsonProperty("prices")]
        public List<Price> Prices { get; set; }

        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        [JsonProperty("refundable")]
        public bool Refundable { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("valid")]
        public Valid Valid { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class Hour
    {
        [JsonProperty("dates")]
        public List<DateTimeOffset> Dates { get; set; }

        [JsonProperty("daysOfWeek")]
        public List<long> DaysOfWeek { get; set; }

        [JsonProperty("times")]
        public List<Time> Times { get; set; }

        [JsonProperty("valid")]
        public Valid Valid { get; set; }
    }

    public class Time
    {
        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("open")]
        public string Open { get; set; }
    }

    public class Valid
    {
        [JsonProperty("from")]
        public DateTimeOffset From { get; set; }

        [JsonProperty("until")]
        public DateTimeOffset Until { get; set; }
    }

    public class Price
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("net")]
        public Net Net { get; set; }

        [JsonProperty("retail")]
        public Net Retail { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("travelerType")]
        public TravelerType TravelerType { get; set; }
    }

    public class Net
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class TravelerType
    {
        [JsonProperty("ageBand")]
        public string AgeBand { get; set; }

        [JsonProperty("maxAge")]
        public int MaxAge { get; set; }

        [JsonProperty("minAge")]
        public int MinAge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}