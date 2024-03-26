using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class GeneratePaymentResponse
    {
        [JsonProperty(PropertyName = "amount")]
        public GeneratePaymentAmountRS GeneratePaymentAmount { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "expiresAt")]
        public string ExpiresAt { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "merchantAccount")]
        public string MerchantAccount { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "shopperLocale")]
        public string ShopperLocale { get; set; }

        [JsonProperty(PropertyName = "shopperReference")]
        public string ShopperReference { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class GeneratePaymentAmountRS
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }
}

