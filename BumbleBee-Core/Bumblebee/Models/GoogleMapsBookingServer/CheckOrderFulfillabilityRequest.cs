using System.Collections.Generic;
using Isango.Entities.GoogleMaps.BookingServer;
using Newtonsoft.Json;

namespace WebAPI.Models.GoogleMapsBookingServer
{
    public class CheckOrderFulfillabilityRequest
    {
        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("item")]
        public List<LineItem> LineItems  { get; set; }

        [JsonProperty("cart_id ")]
        public string CartId { get; set; }
    }
}