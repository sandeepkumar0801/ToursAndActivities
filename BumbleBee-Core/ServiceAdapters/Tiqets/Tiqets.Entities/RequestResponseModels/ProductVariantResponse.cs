using Isango.Entities.Tiqets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class ProductVariantResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "max_tickets_per_order")]
        public int MaxTicketsPerOrder { get; set; }

        [JsonProperty(PropertyName = "variants")]
        public List<ProductVariant> Variants { get; set; }
    }
}