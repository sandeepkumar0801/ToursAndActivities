using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
    public class AuthRS
    {
        [JsonProperty(PropertyName="success")]
        public bool IsSuccess { get; set; }

        [JsonProperty(PropertyName = "data")]
        public AuthRSData Data { get; set; }
    }

    public class AuthRSData
    {
        [JsonProperty(PropertyName = "roles")]
        public List<string> Roles { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}