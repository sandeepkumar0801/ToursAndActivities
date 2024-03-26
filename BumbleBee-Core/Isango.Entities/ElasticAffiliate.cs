using Newtonsoft.Json;

namespace Isango.Entities
{
    public class ElasticAffiliate
    {
        [JsonProperty(PropertyName = "Affiliatekey")]
        public int AffiliateKey { get; set; }
    }
}