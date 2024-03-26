using Newtonsoft.Json;
using System;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class BookingRS
    {
        [JsonProperty(PropertyName = "data")]
        public DataBooking DataBooking { get; set; }
        [JsonProperty(PropertyName = "error")]
        public ErrorData Error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public object Size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }

    public class DataBooking
    {
        [JsonProperty(PropertyName = "referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty(PropertyName = "bookingTime")]
        public DateTime BookingTime { get; set; }
        [JsonProperty(PropertyName = "confirmTime")]
        public DateTime ConfirmTime { get; set; }
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class ErrorData
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "errorDetails")]
        public object ErrorDetails { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
