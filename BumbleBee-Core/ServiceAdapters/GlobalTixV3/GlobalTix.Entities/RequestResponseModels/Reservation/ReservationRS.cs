using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{

    public class ReservationRS
    {
        [JsonProperty(PropertyName = "data")]
        public DataReservation Data { get; set; }
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public object Size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }

    public class DataReservation
    {
        [JsonProperty(PropertyName = "referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty(PropertyName = "bookingTime")]
        public DateTime BookingTime { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public float Amount { get; set; }
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }


    public class Error
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "errorDetails")]
        public object ErrorDetails { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

}
