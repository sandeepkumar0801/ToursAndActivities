using Newtonsoft.Json;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities
{
    public class RedemptionRequest
    {
        [JsonProperty("referenceId")]
        public string referenceId { get; set; }
        [JsonProperty("otaReferenceId")]
        public string otaReferenceId { get; set; }
        public string datetime { get; set; }

    }
}
