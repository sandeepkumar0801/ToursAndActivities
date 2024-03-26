using Newtonsoft.Json;
using System;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels
{
    public class CancelReleaseRequest
    {
        [JsonProperty(PropertyName = "referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }
    }
}



