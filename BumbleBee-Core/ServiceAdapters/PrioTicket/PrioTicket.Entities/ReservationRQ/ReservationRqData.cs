using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.ReservationRQ
{
    public class ReservationRqData
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }

        [JsonProperty(PropertyName = "ticket_id")]
        public string TicketId { get; set; }

        [JsonProperty(PropertyName = "pickup_point_id")]
        public string PickupPointId { get; set; }

        [JsonProperty(PropertyName = "from_date_time")]
        public string FromDateTime { get; set; }

        [JsonProperty(PropertyName = "to_date_time")]
        public string ToDateTime { get; set; }

        [JsonProperty(PropertyName = "booking_details")]
        public BookingDetails[] BookingDetails { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }
    }
}