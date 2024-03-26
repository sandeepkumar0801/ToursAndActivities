using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class Item
    {
        [JsonProperty("Image_cdn_url")]
        public string ImageCdnUrl { get; set; }

        public string Name { get; set; }

        [JsonProperty("Cancellation_policy_safe_html")]
        public string CancellationPolicySafeHtml { get; set; }

        public string Headline { get; set; }

        [JsonProperty("Cancellation_policy")]
        public string CancellationPolicy { get; set; }

        [JsonProperty("Is_pickup_ever_available")]
        public bool IsPickupEverAvailable { get; set; }

        [JsonProperty("Description_safe_html")]
        public string DescriptionSafeHtml { get; set; }

        public string Location { get; set; }

        [JsonProperty("Customer_prototypes")]
        public List<CustomerPrototype> CustomerPrototypes { get; set; }

        public List<object> Images { get; set; }
        public int Pk { get; set; }

        [JsonProperty("Tax_percentage")]
        public double TaxPercentage { get; set; }

        public string Description { get; set; }
    }
}