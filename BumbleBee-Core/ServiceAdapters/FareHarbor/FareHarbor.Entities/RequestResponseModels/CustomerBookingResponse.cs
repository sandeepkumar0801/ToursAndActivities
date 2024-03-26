using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CustomerBookingResponse
    {
        public int Pk { get; set; }

        [JsonProperty("custom_field_values")]
        public List<object> CustomFieldValues { get; set; }

        [JsonProperty("customer_type_rate")]
        public CustomerTypeRate CustomerTypeRate { get; set; }
    }
}