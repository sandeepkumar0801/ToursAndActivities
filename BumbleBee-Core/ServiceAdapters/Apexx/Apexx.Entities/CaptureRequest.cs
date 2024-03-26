using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class CaptureRequest
    {
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "capture_reference")]
        public string CaptureReference { get; set; }

        [JsonProperty(PropertyName = "_id")]
        [JsonIgnore]
        public string TransactionId { get; set; }
    }
}