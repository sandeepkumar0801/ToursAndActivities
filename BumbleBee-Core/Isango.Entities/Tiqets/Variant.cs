using Newtonsoft.Json;

namespace Isango.Entities.Tiqets
{
    public class Variant
    {
        [JsonProperty(PropertyName = "variant_id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}