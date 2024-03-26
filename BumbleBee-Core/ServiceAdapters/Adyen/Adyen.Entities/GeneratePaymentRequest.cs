using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class GeneratePaymentRequest
    {
        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public GeneratePaymentAmountRQ GeneratePaymentAmount { get; set; }

        [JsonProperty(PropertyName = "shopperReference")]
        public string ShopperReference { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "merchantAccount")]
        public string MerchantAccount { get; set; }

        [JsonProperty(PropertyName = "shopperLocale")]
        public string ShopperLocale { get; set; }

    }

    public class GeneratePaymentAmountRQ
    {
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }
}

