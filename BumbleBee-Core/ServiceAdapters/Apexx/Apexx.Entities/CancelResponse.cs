using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class CancelResponse
    {
        [JsonProperty(PropertyName = "_id")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public long Amount { get; set; }

        [JsonProperty(PropertyName = "dynamic_descriptor")]
        public string DynamicDescriptor { get; set; }

        [JsonProperty(PropertyName = "merchant_reference")]
        public string MerchantReference { get; set; }

        [JsonProperty(PropertyName = "authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "last_status_update")]
        public string LastStatusUpdate { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}