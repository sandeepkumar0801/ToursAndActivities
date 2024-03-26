using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels
{

    public class BookingDetailsResponse
    {
        
        [JsonProperty(PropertyName = "data")]
        public DataBookingDetail dataBookingDetail { get; set; }
        [JsonProperty(PropertyName = "error")]
        public ErrorData error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public object size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool success { get; set; }
    }

    public class DataBookingDetail
    {
        [JsonProperty(PropertyName = "referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty(PropertyName = "bookingTime")]
        public DateTime BookingTime { get; set; }
       
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "tickets")]
        public List<TicketBookingDetail> TicketsBookingDetail { get; set; }
        [JsonProperty(PropertyName = "isTicketsReady")]
        public bool IsTicketsReady { get; set; }
        [JsonProperty(PropertyName = "eTicketUrl")]
        public string ETicketUrl { get; set; }
        [JsonProperty(PropertyName = "confirmTime")]
        public DateTime ConfirmTime { get; set; }
    }

    public class TicketBookingDetail
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
       
        [JsonProperty(PropertyName = "qrcode")]
        public string Qrcode { get; set; }
       
        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; }
        [JsonProperty(PropertyName = "ticketFormat")]
        public string TicketFormat { get; set; }
       
     }

    public class Status
    {
        [JsonProperty(PropertyName = "enumType")]
        public string EnumType { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
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



