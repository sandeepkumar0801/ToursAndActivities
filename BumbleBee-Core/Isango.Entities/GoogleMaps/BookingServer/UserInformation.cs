using Newtonsoft.Json;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class UserInformation
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("language_code")]
        public string LanguageCode { get; set; }

        [JsonProperty("address")]
        public PostalAddress Address { get; set; }
    }
}