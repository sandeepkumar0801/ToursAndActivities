using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Request
{
    public class PrioCreateBookingData
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }

        [JsonProperty(PropertyName = "booking_type")]
        public BookingType BookingType { get; set; }

        [JsonProperty(PropertyName = "booking_name")]
        public string BookingName { get; set; }

        [JsonProperty(PropertyName = "booking_email")]
        public string BookingEmail { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public Contact Contact { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string[] Notes { get; set; }

        [JsonProperty(PropertyName = "product_language")]
        public string ProductLanguage { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }
    }
}