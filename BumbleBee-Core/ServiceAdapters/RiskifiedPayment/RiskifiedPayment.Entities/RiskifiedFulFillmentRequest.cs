using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public class RiskifiedFulFillRequest
    {
        [JsonProperty(PropertyName = "order")]
        public RiskifiedFulFillRequestObject Order { get; set; }
    }

    public class RiskifiedFulFillRequestObject
    {
        /// <summary>
        /// The unique identifier for the order
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// A list of fulfillment attempts for the order.
        /// </summary>
        [JsonProperty(PropertyName = "fulfillments")]
        public List<FulfillmentDetails> Fulfillments { get; set; }
    }

    public class FulfillmentDetails
    {
        /// <summary>
        /// Unique identifier of this fulfillment attempt.
        /// </summary>
        [JsonProperty(PropertyName = "fulfillment_id")]
        public string FulfillmentId { get; set; }

        /// <summary>
        /// When the order was fulfilled.
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The fulfillment status, valid values are:
        /// success The fulfillment was successful.
        /// cancelled The fulfillment was cancelled.
        /// error There was an error with the fulfillment request.
        /// failure The fulfillment request failed.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}