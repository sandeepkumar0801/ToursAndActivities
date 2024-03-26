using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public class GeoLocation
    {
        [JsonProperty(PropertyName = "lat")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public float Longitude { get; set; }
    }
}