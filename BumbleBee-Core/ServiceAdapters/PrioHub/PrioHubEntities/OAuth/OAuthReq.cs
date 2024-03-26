using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.OAuthReq
{
    public class OAuthReq
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}