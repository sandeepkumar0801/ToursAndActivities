using Newtonsoft.Json;
using System;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels
{

    public class CancelReleaseResponse
    {
        [JsonProperty(PropertyName = "data")]
        public DataCancel DataCancel { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public object Size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }

    public class DataCancel
    {
        [JsonProperty(PropertyName = "referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty(PropertyName = "bookingTime")]
        public DateTime BookingTime { get; set; }
        [JsonProperty(PropertyName = "releaseTime")]
        public DateTime ReleaseTime { get; set; }
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}



