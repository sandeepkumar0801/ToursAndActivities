using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class CreateCardTransactionRequest
    {
        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }

        [JsonProperty(PropertyName = "capture_now")]
        public string CaptureNow { get; set; }

        [JsonProperty(PropertyName = "merchant_reference")]
        public string MerchantReference { get; set; }

        [JsonProperty(PropertyName = "card")]
        public Card Card { get; set; }

        [JsonProperty(PropertyName = "recurring_type")]
        public string RecurringType { get; set; }

        [JsonProperty(PropertyName = "three_ds")]
        public ThreeDS ThreeDs { get; set; }

        [JsonProperty(PropertyName = "organisation")]
        public string Organisation { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }
}