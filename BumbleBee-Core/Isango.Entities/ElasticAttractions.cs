using Newtonsoft.Json;

namespace Isango.Entities
{
    public class ElasticAttractions
    {
        [JsonProperty(PropertyName = "regionId")]
        public int RegionId { get; set; }

        [JsonProperty(PropertyName = "regionName")]
        public string RegionName { get; set; }

        [JsonProperty(PropertyName = "serviceId")]
        public int Categoryid { get; set; }

        [JsonProperty(PropertyName = "serviceName")]
        public string CategoryName { get; set; }

        [JsonProperty(PropertyName = "affiliateKey")]
        public int AffiliateKey { get; set; }

        [JsonProperty(PropertyName = "languageCode")]
        public string LanguageCode { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }

        [JsonProperty(PropertyName = "boosterCount")]
        public int BoostingCount { get; set; }

        [JsonProperty(PropertyName = "bookingCount")]
        public int BookingCount { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "serviceType")]
        public string AType { get; set; }
    }
}