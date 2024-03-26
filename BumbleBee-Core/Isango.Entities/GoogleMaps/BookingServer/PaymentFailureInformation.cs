using Newtonsoft.Json;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class PaymentFailureInformation
    {
        [JsonProperty("threeds1_parameters")]
        public ThreeDs1Parameters ThreeDs1Parameters { get; set; }
    }
}