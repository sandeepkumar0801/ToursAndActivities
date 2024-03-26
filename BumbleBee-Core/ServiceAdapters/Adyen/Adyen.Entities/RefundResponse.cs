using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class RefundResponse
    {
        [JsonProperty(PropertyName = "pspReference")]
        public string PspReference { get; set; }

        [JsonProperty(PropertyName = "response")]
        public string Response { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errorType")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "refusalReason")]
        public string RefusalReason { get; set; }

        [JsonProperty(PropertyName = "refusalReasonCode")]
        public string RefusalReasonCode { get; set; }
    }
}