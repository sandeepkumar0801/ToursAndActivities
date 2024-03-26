using Newtonsoft.Json;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class ExtendedOption
    {
        public string Name { get; set; }

        [JsonProperty("Is_Taxable")]
        public bool IsTaxable { get; set; }

        [JsonProperty("Modifier_kind")]
        public string ModifierKind { get; set; }

        [JsonProperty("Description_safe_html")]
        public string DescriptionSafeHtml { get; set; }

        public int Offset { get; set; }
        public int Pk { get; set; }
        public double Percentage { get; set; }

        [JsonProperty("Modifier_type")]
        public string ModifierType { get; set; }

        [JsonProperty("Is_always_per_customer")]
        public bool IsAlwaysPerCustomer { get; set; }

        public string Description { get; set; }
    }
}