using Newtonsoft.Json;

namespace Isango.Entities.Tiqets
{
    public class CreateOrderResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "order_reference_id")]
        public string OrderReferenceId { get; set; }

        [JsonProperty(PropertyName = "payment_confirmation_token")]
        public string PaymentConfirmationToken { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}





