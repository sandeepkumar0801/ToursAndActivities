using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.Redeam.Redeam.Entities.CreateHold
{
    public class CreateHoldResponse
    {
        [JsonProperty("hold")]
        public ResponseHold Hold { get; set; }
    }

    public class ResponseHold
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("items")]
        public List<ResponseItem> Items { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class ResponseItem
    {
        [JsonProperty("at")]
        public DateTimeOffset At { get; set; }

        [JsonProperty("availabilityId")]
        public string AvailabilityId { get; set; }

        [JsonProperty("ext")]
        public ResponseExt Ext { get; set; }

        [JsonProperty("priceId")]
        public Guid PriceId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("rate")]
        public Rate Rate { get; set; }

        [JsonProperty("rateId")]
        public Guid RateId { get; set; }

        [JsonProperty("supplierId")]
        public Guid SupplierId { get; set; }

        [JsonProperty("travelerType")]
        public string TravelerType { get; set; }
    }

    public class ResponseExt
    {
    }

    public class Rate
    {
        [JsonProperty("cancelable")]
        public bool Cancelable { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("cutoff")]
        public long Cutoff { get; set; }

        [JsonProperty("holdable")]
        public bool Holdable { get; set; }

        [JsonProperty("holdablePeriod")]
        public long HoldablePeriod { get; set; }

        [JsonProperty("hours")]
        public List<Hour> Hours { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("maxTravelers")]
        public long MaxTravelers { get; set; }

        [JsonProperty("minTravelers")]
        public long MinTravelers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("partnerId")]
        public Guid PartnerId { get; set; }

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

        [JsonProperty("version")]
        public long Version { get; set; }
    }

    public class Hour
    {
        [JsonProperty("dates")]
        public List<DateTimeOffset> Dates { get; set; }

        [JsonProperty("daysOfWeek")]
        public List<object> DaysOfWeek { get; set; }

        [JsonProperty("times")]
        public List<Time> Times { get; set; }
    }

    public class Time
    {
        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("open")]
        public string Open { get; set; }
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
        public long Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class TravelerType
    {
        [JsonProperty("ageBand")]
        public string AgeBand { get; set; }

        [JsonProperty("maxAge")]
        public long MaxAge { get; set; }

        [JsonProperty("minAge")]
        public long MinAge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}