using Newtonsoft.Json;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class ActivityInfoRS
    {
        [JsonProperty(PropertyName="data")]
        public ActivityInfo Data { get; set; }
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
    }
}