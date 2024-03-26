using Newtonsoft.Json;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CustomFieldValue
    {
        [JsonProperty("custom_field")]
        public int CustomField { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}