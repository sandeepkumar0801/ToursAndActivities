using Newtonsoft.Json;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    
    public class AvailabilityRES
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public ResultAvailability ResultAvailability { get; set; }
    }

    public class ResultAvailability
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "productType")]
        public int ProductType { get; set; }
    }

}
