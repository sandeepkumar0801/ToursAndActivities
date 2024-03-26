using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Request
{
    public class BookingType
    {
        [JsonProperty(PropertyName = "ticket_id")]
        public string TicketId { get; set; }

        [JsonProperty(PropertyName = "booking_details")]
        public BookingDetails[] BookingDetails { get; set; }

        [JsonProperty(PropertyName = "reservation_reference")]
        public string ReservationReference { get; set; }
    }
}