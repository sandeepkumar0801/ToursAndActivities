using Isango.Entities.GoogleMaps.BookingServer.Enums;
using Newtonsoft.Json;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class PaymentInformation
    {
        [JsonProperty("payment_transaction_id")]
        public string PaymentTransactionId { get; set; }

        [JsonProperty("payment_option_id")]
        public string PaymentOptionId { get; set; }

        [JsonProperty("user_payment_option_id")]
        public string UserPaymentOptionId { get; set; }

        [JsonProperty("fraud_signals")]
        public string FraudSignals { get; set; }

        [JsonProperty("pa_response")]
        public string PaResponse { get; set; }

        [JsonProperty("md_merchant_data")]
        public string MdMerchantData { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("tax_amount")]
        public Price TaxAmount { get; set; }

        [JsonProperty("fees")]
        public Price Fees { get; set; }

        [JsonProperty("fees_and_taxes")]
        public Price FeesAndTaxes { get; set; }

        [JsonProperty("payment_processed_by")]
        public PaymentProcessedBy PaymentProcessedBy { get; set; }

        [JsonProperty("prepayment_status")]
        public PrepaymentStatus PrepaymentStatus { get; set; }
    }
}