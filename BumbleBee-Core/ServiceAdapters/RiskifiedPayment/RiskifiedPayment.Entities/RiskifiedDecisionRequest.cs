using Newtonsoft.Json;
using System;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public class RiskifiedDecisionRequest
    {
        [JsonProperty(PropertyName = "order")]
        public DecisionRequestObject Order { get; set; }
    }

    public class DecisionRequestObject
    {
        /// <summary>
        /// The unique identifier for the order
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The decision details for the order.
        /// </summary>
        [JsonProperty(PropertyName = "decision")]
        public DecisionDetails Decision { get; set; }
    }

    public class DecisionDetails
    {
        /// <summary>
        /// The unique identifier for the order
        /// </summary>
        [JsonProperty(PropertyName = "external_status")]
        public string ExternalStatus { get; set; }

        /// <summary>
        /// The decision details for the order.
        /// </summary>
        [JsonProperty(PropertyName = "decided_at")]
        public DateTime DecidedAt { get; set; }
    }
}