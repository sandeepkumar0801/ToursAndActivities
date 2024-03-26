using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class BookingRES
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "result")]
        public Result Result { get; set; }
    }

    public class Result
    {
        [JsonProperty(PropertyName = "details")]
        public List<Detail> Details { get; set; }
        [JsonProperty(PropertyName = "referenceNo")]
        public string ReferenceNo { get; set; }
    }

    public class Detail
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "bookingId")]
        public int BookingId { get; set; }
        [JsonProperty(PropertyName = "downloadRequired")]
        public bool DownloadRequired { get; set; }
        [JsonProperty(PropertyName = "serviceUniqueId")]
        public string ServiceUniqueId { get; set; }
        [JsonProperty(PropertyName = "servicetype")]
        public string Servicetype { get; set; }
        [JsonProperty(PropertyName = "confirmationNo")]
        public string ConfirmationNo { get; set; }
    }
}
