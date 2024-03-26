using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class ProductResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "product")]
        public Product Product { get; set; }
    }
}