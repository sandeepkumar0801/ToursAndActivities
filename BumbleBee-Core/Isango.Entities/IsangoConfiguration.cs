using Newtonsoft.Json;
using System;

namespace Isango.Entities
{
    public class IsangoConfiguration
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}