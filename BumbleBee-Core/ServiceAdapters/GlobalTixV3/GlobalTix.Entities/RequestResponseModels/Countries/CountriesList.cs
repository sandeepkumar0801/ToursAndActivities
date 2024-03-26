using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.Countries
{
    public class CountriesList
    {
        
        
            [JsonProperty(PropertyName = "data")]
            public Datum[] Data { get; set; }

            [JsonProperty(PropertyName = "error")]
            public object Error { get; set; }

            [JsonProperty(PropertyName = "size")]
            public object Size { get; set; }

            [JsonProperty(PropertyName = "success")]
            public bool Success { get; set; }
        

        public class Datum
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("cities")]
            public City[] Cities { get; set; }

            [JsonProperty("currency")]
            public Currency Currency { get; set; }

            [JsonProperty("isCurrencyExchange")]
            public bool IsCurrencyExchange { get; set; }

            [JsonProperty("isDistributionTable")]
            public bool IsDistributionTable { get; set; }

            [JsonProperty("isListing")]
            public bool IsListing { get; set; }

            [JsonProperty("isBilling")]
            public bool IsBilling { get; set; }

            [JsonProperty("lastUpdated")]
            public DateTime LastUpdated { get; set; }

            [JsonProperty("lastUpdatedBy")]
            public string LastUpdatedBy { get; set; }

            [JsonProperty("mobilePrefix")]
            public string MobilePrefix { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Currency
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("markup")]
            public float Markup { get; set; }

            [JsonProperty("roundingUp")]
            public float RoundingUp { get; set; }

            [JsonProperty("creditCardFee")]
            public float CreditCardFee { get; set; }
        }

        public class City
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("countryId")]
            public int CountryId { get; set; }

            [JsonProperty("timezoneOffset")]
            public int? TimezoneOffset { get; set; }
        }

    }
}
