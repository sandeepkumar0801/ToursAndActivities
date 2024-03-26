using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability
{
    public class AvailabilityResponse
    {
        [JsonProperty("toDate")]
        public DateTimeOffset ToDate { get; set; }
        [JsonProperty("fromDate")]
        public DateTimeOffset FromDate { get; set; }
        [JsonProperty("days")]
        public List<Day> Days { get; set; }
    }

    public class Day
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }
        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }
        [JsonProperty("availabilities")]
        public List<AvailabilityData> Availabilities { get; set; }
    }

    public class AvailabilityData
    {
        [JsonProperty("rate")]
        public string Rate { get; set; }
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }
        [JsonProperty("variantCode")]
        public string VariantCode { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("availability")]
        public int Availability { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("price")]
        public float Price { get; set; }
        [JsonProperty("discountedPrice")]
        public float DiscountedPrice { get; set; }
        [JsonProperty("costPrice")]
        public float CostPrice { get; set; }
    }
}