using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public class RiskifiedPartialRefundRequest
    {
        [JsonProperty(PropertyName = "order")]
        public PartialRefundRequestObject Order { get; set; }
    }

    public class PartialRefundRequestObject
    {
        /// <summary>
        /// The unique identifier for the order
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// A list of partial refunds for the order.
        /// </summary>
        [JsonProperty(PropertyName = "refunds")]
        public List<RefundDetail> Refunds { get; set; }
    }

    public class RefundDetail
    {
        /// <summary>
        /// Unique identifier for this refund
        /// </summary>
        [JsonProperty(PropertyName = "refund_id")]
        public string RefundId { get; set; }

        /// <summary>
        /// A unique identifier of the item in the refund
        /// </summary>
        [JsonProperty(PropertyName = "sku")]
        public int ServiceId { get; set; }

        /// <summary>
        /// When the refund was issued to the customer.
        /// </summary>
        [JsonProperty(PropertyName = "refunded_at")]
        public DateTime RefundedAt { get; set; }

        /// <summary>
        /// Total amount of refund, specified as a positive number
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public float Amount { get; set; }

        /// <summary>
        /// The 3-letter code (ISO 4217) for the currency used for the payment.
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Text note detailing the reason this refund was issued
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
    }
}