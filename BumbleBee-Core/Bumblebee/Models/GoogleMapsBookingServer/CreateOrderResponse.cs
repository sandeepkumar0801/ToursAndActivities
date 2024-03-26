using Isango.Entities.GoogleMaps.BookingServer;

namespace WebAPI.Models.GoogleMapsBookingServer
{
    public class CreateOrderResponse
    {
        public Order Order { get; set; }
        public OrderFailure OrderFailure { get; set; }
    }
}