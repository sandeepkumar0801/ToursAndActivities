using Newtonsoft.Json;
using System;

namespace Isango.Entities.RiskifiedPayment
{
    // An object describing the failed result of an authorization attempt by a payment gateway.
    public class AuthorizationError
    {
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}