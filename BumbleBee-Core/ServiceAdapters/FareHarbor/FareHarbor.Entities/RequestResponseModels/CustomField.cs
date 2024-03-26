using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CustomField
    {
        [JsonProperty("Is_required")]
        public bool IsRequired { get; set; }

        public string Description { get; set; }

        [JsonProperty("Booking_notes_safe_html")]
        public string BookingNotesSafeHtml { get; set; }

        public string Type { get; set; }

        [JsonProperty("Is_taxable")]
        public bool IsTaxable { get; set; }

        [JsonProperty("Modifier_kind")]
        public string ModifierKind { get; set; }

        [JsonProperty("Description_safe_html")]
        public string DescriptionSafeHtml { get; set; }

        [JsonProperty("Booking_notes")]
        public string BookingNotes { get; set; }

        public int Offset { get; set; }
        public int Pk { get; set; }
        public double Percentage { get; set; }

        [JsonProperty("Modifier_type")]
        public string ModifierType { get; set; }

        [JsonProperty("Extended_options")]
        public List<ExtendedOption> ExtendedOptions { get; set; }

        public bool IsAlwaysPerCustomer { get; set; }
        public string Name { get; set; }
    }
}