using Newtonsoft.Json;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public class RiskifiedCheckout
    {
        [JsonProperty(PropertyName = "checkout")]
        public Checkout CheckoutData { get; set; }
    }

    public class Checkout : OrderData
    {
        /// <summary>
        /// The unique identifier of the Checkout that created this order.
        /// </summary>
        [JsonProperty(PropertyName = "checkout_id")]
        public string CheckoutId { get; set; }
    }
}