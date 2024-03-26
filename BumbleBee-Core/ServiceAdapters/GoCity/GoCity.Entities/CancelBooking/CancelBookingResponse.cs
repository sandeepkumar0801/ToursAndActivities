using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GoCity.GoCity.Entities.CancelBooking
{

    public class CancelBookingResponse
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("orderCancelResponseList")]
        public List<Ordercancelresponselist> OrderCancelResponseList { get; set; }
        [JsonProperty("successStatus")]
        public bool SuccessStatus { get; set; }
    }

    public class Ordercancelresponselist
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }
        [JsonProperty("successStatus")]
        public bool SuccessStatus { get; set; }
        [JsonProperty("skuCodeList")]
        public List<Skucodelist> SkuCodeList { get; set; }
    }

    public class Skucodelist
    {
        [JsonProperty("skuCode")]
        public string SkuCode { get; set; }
        [JsonProperty("expDate")]
        public long ExpDate { get; set; }
        [JsonProperty("createdDate")]
        public long CreatedDate { get; set; }
        [JsonProperty("confirmationCode")]
        public string ConfirmationCode { get; set; }
    }
}