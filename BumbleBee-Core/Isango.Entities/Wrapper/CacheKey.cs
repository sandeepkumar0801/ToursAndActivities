using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.Wrapper
{
    public class CacheKey<T>
    {
        public string CacheKeyName { get; set; }
        public List<T> CacheValue { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}