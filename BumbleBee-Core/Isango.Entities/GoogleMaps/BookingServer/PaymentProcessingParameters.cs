using Isango.Entities.GoogleMaps.BookingServer.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class PaymentProcessingParameters
    {
        [JsonProperty("processor")]
        public PaymentProcessor Processor { get; set; }

        [JsonProperty("payment_method_token")]
        public string PaymentMethodToken { get; set; }

        [JsonProperty("unparsed_payment_method_token")]
        public string UnparsedPaymentMethodToken { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("payment_processor")]
        public string PaymentProcessor { get; set; }

        [JsonProperty("tokenization_config")]
        public TokenizationConfig TokenizationConfig { get; set; }
    }

    public class TokenizationConfig
    {
        [JsonProperty("tokenization_parameter")]
        public Dictionary<string, string> TokenizationParameters { get; set; }

        [JsonProperty("billing_information_format")]
        public BillingInformationFormat BillingInformationFormat { get; set; }

        [JsonProperty("merchant_of_record_name")]
        public string MerchantOfRecordName { get; set; }

        [JsonProperty("PaymentCountryCode")]
        public string PaymentCountryCode { get; set; }

        [JsonProperty("card_network_parameters ")]
        public List<CardNetworkParameter> CardNetworkParameter { get; set; }

        [JsonProperty("allowed_auth_methods")]
        private List<AuthMethod> AllowedAuthMethods { get; set; }
    }

    public enum BillingInformationFormat
    {
        BILLING_INFORMATION_FORMAT_UNSPECIFIED,
        MIN,
        FULL
    }

    public enum AuthMethod
    {
        AUTH_METHOD_UNSPECIFIED,
        PAN_ONLY,
        CRYPTOGRAM_3DS
    }

    public enum CreditCardType
    {
        // Unused.
        CREDIT_CARD_TYPE_UNSPECIFIED,

        // A Visa credit card.
        VISA,

        // A Mastercard credit card.
        MASTERCARD,

        // An American Express credit card.
        AMERICAN_EXPRESS,

        // A Discover credit card.
        DISCOVER,

        // A JCB credit card.
        JCB
    }

    public class CreditCardRestrictions
    {
        public List<CreditCardType> CreditCardTypes { get; set; }
    }

    public class CardNetworkParameter
    {
        [JsonProperty("card_network")]
        public CreditCardType CardNetwork { get; set; }

        [JsonProperty("acquirer_bin")]
        public string AcquirerBin { get; set; }

        [JsonProperty("acquirer_merchant_id")]
        public string AcquirerMerchantId { get; set; }
    }
}