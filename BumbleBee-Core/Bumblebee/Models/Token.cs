using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Bumblebee.Models
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("token_type")]
        public string token_type { get; set; }
        [JsonProperty("expires_in")]
        public int expires_in { get; set; }
        [JsonProperty("userName")]
        public string userName { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }
        [JsonProperty(".issued")]
        public string Issued { get; set; }
        [JsonProperty(".expires")]
        public string Expires { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonProperty("error")]
        public string? error { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonProperty("error_description")]
        public string? error_description { get; set; }
    }

    public class TokenAuthorization
    {
        [JsonProperty("error")]
        public string error { get; set; }

        [JsonProperty("error_description")]
        public string error_description { get; set; }
    }
}
