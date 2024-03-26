using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class ThreeDVerifyResponse
    {
        [JsonProperty(PropertyName = "resultCode")]
        public string resultCode { get; set; }

        [JsonProperty(PropertyName = "pspReference")]
        public string pspReference { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errorType")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "merchantReference")]
        public string MerchantReference { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public Amount Amount { get; set; }

        [JsonProperty(PropertyName = "action")]
        public Action Action { get; set; }

        /*
        {"pspReference":"1729296504463178","refusalReason":"Issuer Suspected Fraud","resultCode":"Refused","refusalReasonCode":"31","amount":{"currency":"USD","value":4900},"merchantReference":"ISAM9H14L"}
        */

        [JsonProperty(PropertyName = "refusalReason")]
        public string RefusalReason { get; set; }

        [JsonProperty(PropertyName = "refusalReasonCode")]
        public string RefusalReasonCode { get; set; }
    }
}