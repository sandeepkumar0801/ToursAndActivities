using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class EnrollmentCheckRequest
    {
        //[JsonProperty(PropertyName = "account")]
        //public string Account { get; set; }

        [JsonProperty(PropertyName = "organisation")]
        public string Organisation { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "capture_now")]
        public bool CaptureNow { get; set; }

        [JsonProperty(PropertyName = "dynamic_descriptor")]
        public string DynamicDescriptor { get; set; }

        [JsonProperty(PropertyName = "merchant_reference")]
        public string MerchantReference { get; set; }

        [JsonProperty(PropertyName = "card")]
        public Card Card { get; set; }

        [JsonProperty(PropertyName = "customer_ip")]
        public string CustomerIp { get; set; }

        [JsonProperty(PropertyName = "user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty(PropertyName = "three_ds")]
        public ThreeDS ThreeDs { get; set; }

        [JsonProperty(PropertyName = "billing_address")]
        public BillingAddress BillingAddress { get; set; }

        [JsonProperty(PropertyName = "recurring_type")]
        public string RecurringType { get; set; }//conditional serialization!
        [JsonIgnore]
        //this property allows control of serialization at runtime
        public bool IsOnRequestProduct { get; set; }
        //only serialize RecurringType if IsOnRequestProduct == true
        public bool ShouldSerializeRecurringType()
        {
            return (this.IsOnRequestProduct);
        }

       

    }

    public class ThreeDS
    {
        [JsonProperty(PropertyName = "three_ds_required")]
        public string ThreeDSRequired { get; set; }
    }

    public class Card
    {
        [JsonProperty(PropertyName = "card_number")]
        public string CardNumber { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string SecurityCode { get; set; }

        [JsonProperty(PropertyName = "expiry_month")]
        public string ExpiryMonth { get; set; }

        [JsonProperty(PropertyName = "expiry_year")]
        public string ExpiryYear { get; set; }

        [JsonProperty(PropertyName = "create_token")]
        public string CreateToken { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

    }
}