using Newtonsoft.Json;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class UnderNameBase
    {
        [JsonProperty(PropertyName = "firstname")]
        public string Firstname { get; set; }

        [JsonProperty(PropertyName = "lastname")]
        public string Lastname { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "mobile")]
        public string Mobile { get; set; }
    }
}