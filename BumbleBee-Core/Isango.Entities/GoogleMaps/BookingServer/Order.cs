using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class Order
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("user_information")]
        public UserInformation UserInformation { get; set; }

        [JsonProperty("payment_information")]
        public PaymentInformation PaymentInformation { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("item")]
        public List<LineItem> Items { get; set; }
    }
}