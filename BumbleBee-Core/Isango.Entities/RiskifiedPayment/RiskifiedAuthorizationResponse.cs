using Newtonsoft.Json;

namespace Isango.Entities.RiskifiedPayment
{
    public class RiskifiedAuthorizationResponse
    {
        [JsonProperty(PropertyName = "order")]
        public OrderResponse Order;
    }

    public class OrderResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "old_status")]
        public string OldStatus { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
    }
}