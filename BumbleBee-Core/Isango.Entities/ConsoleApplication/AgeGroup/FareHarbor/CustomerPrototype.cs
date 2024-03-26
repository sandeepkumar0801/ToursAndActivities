using Newtonsoft.Json;

namespace Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor
{
    public class CustomerPrototype
    {
        public int Pk { get; set; }

        [JsonProperty("Display_name")]
        public string DisplayName { get; set; }

        public int Total { get; set; }
    }
}