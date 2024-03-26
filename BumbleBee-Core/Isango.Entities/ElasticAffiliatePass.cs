using Newtonsoft.Json;

namespace Isango.Entities
{
    public class ElasticAffiliatePass
    {
        [JsonProperty(PropertyName = "default")]
        public int Default { get; set; }

        [JsonProperty(PropertyName = "all")]
        public int[] All { get; set; }
    }
}