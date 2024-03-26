using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.GetVoucherRes
{
    public class GetVoucherRes
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}