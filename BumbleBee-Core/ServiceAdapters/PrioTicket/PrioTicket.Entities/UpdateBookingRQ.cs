using Newtonsoft.Json;
using ServiceAdapters.PrioTicket.PrioTicket.Entities.Request;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class UpdateBookingRq : EntityBase
    {
        [JsonProperty(PropertyName = "request_type")]
        public string RequestType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "distributor_id")]
        public string DistributorId { get; set; }

        [JsonProperty(PropertyName = "booking_reference")]
        public string BookingReference { get; set; }

        [JsonProperty(PropertyName = "booking_name")]
        public string BookingName { get; set; }

        [JsonProperty(PropertyName = "booking_email")]
        public string BookingEmail { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public Contact Contact { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string[] Notes { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }
    }

    public class ContactUpdateBooking
    {
        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }

        [JsonProperty(PropertyName = "phonenumber")]
        public string Phonenumber { get; set; }
    }

    public class AddressUpdateBooking
    {
        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
    }
}