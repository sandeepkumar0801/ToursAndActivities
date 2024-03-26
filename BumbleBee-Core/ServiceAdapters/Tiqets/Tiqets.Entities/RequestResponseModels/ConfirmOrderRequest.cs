using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class ConfirmOrderRequest
    {
        [JsonProperty(PropertyName = "payment_confirmation_token")]
        public string PaymentConfirmationToken { get; set; }
    }
}