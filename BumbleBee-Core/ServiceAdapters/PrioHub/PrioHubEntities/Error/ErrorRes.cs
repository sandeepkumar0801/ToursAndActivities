using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.ErrorRes
{
    public class ErrorRes
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
        [JsonProperty("error_uri")]
        public string ErrorUri { get; set; }
        [JsonProperty("error_reference")]
        public string ErrorReference { get; set; }
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }
    }
}