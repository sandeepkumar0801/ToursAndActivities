using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.CustomHelper
{
    public class CustomHelper
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("product_id")]
        public string product_id { get; set; }
        [JsonProperty("tps_id")]
        public string tps_id { get; set; }
    }

}