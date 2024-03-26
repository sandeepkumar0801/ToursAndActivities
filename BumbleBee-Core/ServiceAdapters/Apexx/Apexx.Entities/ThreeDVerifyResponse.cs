using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class ThreeDVerifyResponse
    {
        [JsonProperty(PropertyName = "_id")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        [JsonProperty(PropertyName = "organisation")]
        public string Organisation { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }

        [JsonProperty(PropertyName = "authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonProperty(PropertyName = "capture_now")]
        public string CaptureNow { get; set; }

        [JsonProperty(PropertyName = "blocked ")]
        public bool Blocked { get; set; }

        [JsonProperty(PropertyName = "dynamic_descriptor")]
        public string DynamicDescriptor { get; set; }

        [JsonProperty(PropertyName = "merchant_reference")]
        public string MerchantReference { get; set; }

        [JsonProperty(PropertyName = "card")]
        public Card3DVerrify Card { get; set; }

        [JsonProperty(PropertyName = "customer_ip")]
        public string CustomerIp { get; set; }

        [JsonProperty(PropertyName = "pares")]
        public string Pares { get; set; }

        [JsonProperty(PropertyName = "user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "cvv_present")]
        public bool CvvPresent { get; set; }

        [JsonProperty(PropertyName = "last_status_update ")]
        public string LastStatusUpdate { get; set; }

        [JsonProperty(PropertyName = "payment_product")]
        public string PaymentProduct { get; set; }

        [JsonProperty(PropertyName = "card_brand")]
        public string CardBrand { get; set; }

        [JsonProperty(PropertyName = "issuer_name")]
        public string IssuerName { get; set; }

        [JsonProperty(PropertyName = "issuer_country")]
        public string IssuerCountry { get; set; }

        [JsonProperty(PropertyName = "organisation_psp_name")]
        public string OrganisationPspName { get; set; }

        [JsonProperty(PropertyName = "reason_code")]
        public string ReasonCode { get; set; }

        [JsonProperty(PropertyName = "reason_message")]
        public string ReasonMessage { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "shopper_interaction")]
        public string ShopperInteraction { get; set; }

        [JsonProperty(PropertyName = "cvv_result")]
        public string CvvResult { get; set; }

        [JsonProperty(PropertyName = "avs_result")]
        public string AVSResult { get; set; }

        [JsonProperty(PropertyName = "three_ds")]
        public ThreeDs3DVerrify ThreeDS { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public class Card3DVerrify
    {
        [JsonProperty(PropertyName = "card_number")]
        public string CardNumber { get; set; }

        [JsonProperty(PropertyName = "expiry_month")]
        public string ExpiryMonth { get; set; }

        [JsonProperty(PropertyName = "expiry_year")]
        public string ExpiryYear { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    public class ThreeDs3DVerrify
    {
        [JsonProperty(PropertyName = "three_ds_required")]
        public string ThreeDSRequired { get; set; }
    }
}