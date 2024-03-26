using Newtonsoft.Json;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CustomFieldInstance
    {
        public int Pk { get; set; }

        [JsonProperty("Custom_field")]
        public CustomField CustomField { get; set; }
    }
}