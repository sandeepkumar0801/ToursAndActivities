using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class Availability
    {
        [JsonProperty("Start_at")]
        public string StartAt { get; set; }

        [JsonProperty("Customer_Type_Rates")]
        public List<CustomerTypeRate> CustomerTypeRates { get; set; }

        [JsonProperty("Minimum_party_size")]
        public int MinimumPartySize { get; set; }

        [JsonProperty("Custom_field_instances")]
        public List<CustomFieldInstance> CustomFieldInstances { get; set; }

        public Item Item { get; set; }

        [JsonProperty("Maximum_party_size")]
        public object MaximumPartySize { get; set; }

        [JsonProperty("capacity")]
        public int? Capacity { get; set; }

        [JsonProperty("End_at")]
        public string EndAt { get; set; }

        public int Pk { get; set; }
    }
}