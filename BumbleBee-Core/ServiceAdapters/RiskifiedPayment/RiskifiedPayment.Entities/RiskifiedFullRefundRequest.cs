using Newtonsoft.Json;
using System;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public class RiskifiedFullRefundRequest
    {
        [JsonProperty(PropertyName = "order")]
        public FullRefundRequest Order { get; set; }
    }

    public class FullRefundRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "cancel_reason")]
        public string CancelReason { get; set; }

        [JsonProperty(PropertyName = "cancelled_at")]
        public DateTime CancelledAt { get; set; }
    }
}