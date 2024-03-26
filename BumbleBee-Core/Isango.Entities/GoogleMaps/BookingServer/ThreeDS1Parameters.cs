using Newtonsoft.Json;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class ThreeDs1Parameters
    {
        [JsonProperty("acs_url")]
        public string AcsUrl { get; set; }

        [JsonProperty("pa_req")]
        public string PaReq { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("md_merchant_data")]
        public string MdMerchantData { get; set; }
    }
}