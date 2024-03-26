using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class RefundRequest
    {
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "_id")]
        [JsonIgnore]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "capture_id")]
       // [JsonIgnore]
        public string CaptureId { get; set; }
    }
}