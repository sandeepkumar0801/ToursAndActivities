using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CreateBooking
    {
        public string Rebooking { get; set; }

        [JsonProperty("contact")]
        public Contact Contact { get; set; }

        [JsonProperty("customers")]
        public List<Customer> Customers { get; set; }

        [JsonProperty("custom_field_values")]
        public List<CustomFieldValue> CustomFieldValues { get; set; }

        public string ShortName { get; set; }

        public string AvailabilityId { get; set; }

        public string Uuid { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("lodging")]
        public int Lodging { get; set; }

        [JsonProperty("voucher_number")]
        public string VoucherNumber { get; set; }

        public string UserKey { get; set; }
    }
}