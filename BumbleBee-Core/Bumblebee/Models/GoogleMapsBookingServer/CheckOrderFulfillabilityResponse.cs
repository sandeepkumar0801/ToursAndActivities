using Isango.Entities.GoogleMaps.BookingServer;
using Newtonsoft.Json;

namespace WebAPI.Models.GoogleMapsBookingServer
{
    public class CheckOrderFulfillabilityResponse
    {
        [JsonProperty("fulfillability")]
        public OrderFulfillability OrderFulfillability { get; set; }
        [JsonProperty("fees_and_taxes")]
        public Price FeesAndTaxes { get; set; }
        [JsonProperty("fees")]
        public Fees Fees { get; set; }
        [JsonProperty("cart_expiration_sec ")]
        public int CartExpirationSec { get; set; }
    }
}