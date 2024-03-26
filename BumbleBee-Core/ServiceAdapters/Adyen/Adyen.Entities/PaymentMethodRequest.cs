using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{

    public class PaymentMethodRequest
    {
        [JsonProperty(PropertyName = "merchantAccount")]
        public string MerchantAccount { get; set; }
        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty(PropertyName = "shopperLocale")]
        public string ShopperLocale { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public PaymentMethodAmount Amount { get; set; }

        [JsonProperty(PropertyName = "blockedPaymentMethods")]
        public string[] BlockedPaymentMethods { get; set; }
    }

    public class PaymentMethodAmount
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }
    }

}


