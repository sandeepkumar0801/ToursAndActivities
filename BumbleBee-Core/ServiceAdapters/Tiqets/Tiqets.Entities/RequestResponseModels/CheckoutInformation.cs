using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class CheckoutInformation
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "additional_information")]
        public AdditionalInformation AdditionalInformation { get; set; }

        [JsonProperty(PropertyName = "has_timeslots")]
        public bool HasTimeSlots { get; set; }

        [JsonProperty(PropertyName = "has_dynamic_pricing")]
        public bool HasDynamicPricing { get; set; }
    }

    public class AdditionalInformation
    {
        [JsonProperty(PropertyName = "must_know")]
        public string MustKnow { get; set; }

        [JsonProperty(PropertyName = "good_to_know")]
        public string GoodToKnow { get; set; }

        [JsonProperty(PropertyName = "pre_purchase")]
        public string PrePurchase { get; set; }

        [JsonProperty(PropertyName = "usage")]
        public string Usage { get; set; }

        [JsonProperty(PropertyName = "excluded")]
        public string Excluded { get; set; }

        [JsonProperty(PropertyName = "included")]
        public string Included { get; set; }

        [JsonProperty(PropertyName = "post_purchase")]
        public string PostPurchase { get; set; }
    }
}