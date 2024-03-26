using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public class CustomerDetails
    {
        [JsonProperty(PropertyName = "firstname")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastname")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string PhoneNumber { get; set; }
    }
}