using Isango.Entities.GoogleMaps.BookingServer;
using Newtonsoft.Json;

namespace WebAPI.Models.GoogleMapsBookingServer
{
    public class CreateOrderRequest
    {
        [JsonProperty("order")]
        public Order Order { get; set; }
        [JsonProperty("payment_processing_parameters")]
        public PaymentProcessingParameters PaymentProcessingParameters { get; set; }
        [JsonProperty("idempotency_token")]
        public string IdempotencyToken { get; set; }
    }
}