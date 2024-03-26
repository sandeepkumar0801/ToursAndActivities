using Isango.Entities.Enums;
using Newtonsoft.Json;

namespace Isango.Entities
{
    public class AgeGroup
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public int ApiType { get; set; }
        public int AgeGroupId { get; set; }
        public string Description { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public PassengerType PassengerType { get; set; }
    }
}