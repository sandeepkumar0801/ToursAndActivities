using Newtonsoft.Json;

namespace Isango.Entities
{
    public class ActivityWithApiType
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public int ApiType { get; set; }
    }
}