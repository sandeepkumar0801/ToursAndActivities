using Newtonsoft.Json;

namespace Isango.Entities
{
    public class ElasticProducts
    {
        [JsonProperty(PropertyName = "regionid")]
        public int RegionId { get; set; }

        [JsonProperty(PropertyName = "RegionName")]
        public string RegionName { get; set; }

        [JsonProperty(PropertyName = "serviceid")]
        public int ServiceId { get; set; }

        [JsonProperty(PropertyName = "servicename")]
        public string ServiceName { get; set; }

        [JsonProperty(PropertyName = "Offer_Percent")]
        public decimal Offer_Percent { get; set; }

        [JsonProperty(PropertyName = "affiliatekey")]
        public string AffiliateKey { get; set; }

        [JsonProperty(PropertyName = "languagecode")]
        public string LanguageCode { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }

        [JsonProperty(PropertyName = "boosterCount")]
        public int BoostingCount { get; set; }

        [JsonProperty(PropertyName = "bookingCount")]
        public int BookingCount { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "servicename_regionname")]
        public string ServiceNameRegionName { get; set; }

        [JsonProperty(PropertyName = "isactive")]
        public bool IsActive { get; set; }


        [JsonProperty(PropertyName = "image_link")]
        public string ImageURL { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Serviceprice { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currencycode { get; set; }
    }
}