using Newtonsoft.Json;

namespace Isango.Entities
{
    public class ElasticDestinations
    {
        [JsonProperty(PropertyName = "regionid")]
        public int RegionId { get; set; }

        [JsonProperty(PropertyName = "RegionName")]
        public string landingkeyword { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string landingURL { get; set; }

        [JsonProperty(PropertyName = "affiliatekey")]
        public string AffiliateKey { get; set; }

        [JsonProperty(PropertyName = "languagecode")]
        public string LanguageCode { get; set; }

        [JsonProperty(PropertyName = "bookingCount")]
        public int BookingCount { get; set; }

        [JsonProperty(PropertyName = "boosterCount")]
        public int BoosterCount { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "isactive")]
        public bool IsActive { get; set; }
    }
}