using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class CancelCaptureTransactionRequest
    {
        [JsonIgnore]
        [JsonProperty(PropertyName = "captureId")]
        public string CaptureId { get; set; }

        [JsonProperty(PropertyName = "duplicate_check")]
        public string DuplicateCheck { get; set; }
        
        [JsonProperty(PropertyName = "cancel_capture_reference")]
        public string CancelCaptureReference { get; set; }
    }
}