using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.Tiqets
{
    public class ProductVariant
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "max_tickets")]
        public int MaxTickets { get; set; }

        [JsonProperty(PropertyName = "valid_with_variant_ids")]
        public int[] ValidWithVariantIds { get; set; }

        [JsonProperty(PropertyName = "price_components_eur")]
        public PriceComponent PriceComponentsEur { get; set; }

        [JsonProperty(PropertyName = "cancellation" )]
        public Cancellation Cancellation { get; set; }

        [JsonProperty(PropertyName = "requires_visitors_details")]
        public List<string> RequiresVisitorsDetails { get; set; }

        public Dictionary<int,List<string>> RequiresVisitorsDetailsWithVariant { get; set; }
    }

    public class Cancellation
    {
        [JsonProperty(PropertyName = "window")]
        public int Window { get; set; }

        [JsonProperty(PropertyName = "policy")]
        public string Policy { get; set; }
    }    
}