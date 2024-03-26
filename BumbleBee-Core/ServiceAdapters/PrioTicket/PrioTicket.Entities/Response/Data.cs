using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Response
{
    public class Data
    {
        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }

        [JsonProperty(PropertyName = "booking_reference")]
        public string BookingReference { get; set; }

        [JsonProperty(PropertyName = "booking_status")]
        public string BookingStatus { get; set; }

        [JsonProperty(PropertyName = "booking_details")]
        public BookingDetails[] BookingDetails { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_message")]
        public string ErrorMessage { get; set; }
    }
}