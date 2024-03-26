using Newtonsoft.Json;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class PackageInfoRS
    {
        [JsonProperty(PropertyName="data")]
        public PackageInfo Data { get; set; }
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
    }
}