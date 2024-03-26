using Isango.Entities.GoogleMaps.BookingServer.Enums;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class OrderFailure
    {
        public Cause Cause { get; set; }
        public OrderFulfillability OrderFulfillability { get; set; }
        public string Description { get; set; }
        public PaymentFailureInformation PaymentFailureInformation { get; set; }
        public CreditCardType CreditCardType { get; set; }
    }
}