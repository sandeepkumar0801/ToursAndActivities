using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public class Venue
    {
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}