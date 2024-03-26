using Isango.Entities.Tiqets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class CreateOrderRequest
    {
        [JsonProperty(PropertyName = "product_id")]
        public int ProductId { get; set; }

        [JsonProperty(PropertyName = "day")]
        public string Day { get; set; }

        [JsonProperty(PropertyName = "timeslot")]
        public string TimeSlot { get; set; }

        [JsonProperty(PropertyName = "variants")]
        public List<Variant> Variants { get; set; }

        [JsonProperty(PropertyName = "customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty(PropertyName = "external_reference")]
        public string external_reference { get; set; }

        [JsonProperty(PropertyName = "visitors_details")]
        public List<VisitorsDetails> VisitorsDetails { get; set; }
    }

    public class VisitorsDetails
    {
        [JsonProperty(PropertyName = "variant_id")]
        public int VariantId { get; set; }
        [JsonProperty(PropertyName = "visitors_data")]
        public List<VisitorsData> VisitorsData { get; set; }
    }

    public class VisitorsData
    {
        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }
        [JsonProperty(PropertyName = "passport_ids")]
        public string PassportNumber { get; set; }
        [JsonProperty(PropertyName = "nationality")]
        public string PassportNationality { get; set; }
    }
}