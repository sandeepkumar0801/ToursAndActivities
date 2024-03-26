using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class BookingRequestObject
    {
        [JsonProperty(PropertyName = "bookingRequest")]
        public BookingRequest BookingRequest { get; set; }
    }

    public class BookingRequest
    {
        [JsonProperty(PropertyName = "reservationReference")]
        public string ReservationReference { get; set; }

        /// <summary>
        /// This field is configurable.
        /// When true - in booking response, API returns QR code for each passenger
        /// When False -  in booking response, API returns QR code for booking.
        /// </summary>
        [JsonProperty(PropertyName = "ticketPerPassenger")]
        public bool TicketPerPassenger { get; set; }

        [JsonProperty(PropertyName = "products")]
        public Products Products { get; set; }
    }
}