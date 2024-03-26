using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public class Rating
    {
        [JsonProperty(PropertyName = "average")]
        public string Average { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }
}