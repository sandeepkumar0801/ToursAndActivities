using Newtonsoft.Json;

namespace Isango.Entities
{
    public class PickupLocation
    {
        [JsonProperty(PropertyName = "Id")]
        public int LocationId { get; set; }

        public string Description { get; set; }
        public string Name { get; set; }
        public int ActivityId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}