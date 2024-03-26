using Newtonsoft.Json;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CustomerPrototype
    {
        public int Pk { get; set; }

        [JsonProperty("Display_name")]
        public string DisplayName { get; set; }

        public decimal Total { get; set; }

        [JsonProperty("total_including_tax")]
        public decimal TotalIncludingTax { get; set; }
    }
}