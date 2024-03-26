using Newtonsoft.Json;
using System;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class Price
    {
        [JsonProperty("price_micros")]
        public Int64 PriceMicros { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("pricing_option_tag")]
        public string PricingOptionTag { get; set; }
    }
}