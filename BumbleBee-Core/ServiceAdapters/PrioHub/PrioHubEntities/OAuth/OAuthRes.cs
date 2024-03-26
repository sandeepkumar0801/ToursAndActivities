using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.OAuthRes
{
    public class OAuthRes
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
    }
}