using Newtonsoft.Json;

namespace Isango.Entities.RiskifiedPayment
{
    public class RiskifiedCheckoutDeniedResponse
    {
        [JsonProperty(PropertyName = "checkout")]
        public OrderResponse Checkout;
    }
}