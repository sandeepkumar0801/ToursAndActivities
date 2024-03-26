using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GoCity.GoCity.Entities.Booking
{

    public class BookingResponse
    {
        [JsonProperty("orderDetails")]
        public Orderdetails OrderDetails { get; set; }
        [JsonProperty("passDetails")]
        public Passdetails PassDetails { get; set; }
        [JsonProperty("successStatus")]
        public bool SuccessStatus { get; set; }
    }

    public class Orderdetails
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("travelDate")]
        public string TravelDate { get; set; }
        [JsonProperty("purchaseDate")]
        public long PurchaseDate { get; set; }
        [JsonProperty("deliveryMethod")]
        public string DeliveryMethod { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Passdetails
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }
        [JsonProperty("passList")]
        public List<Passlist> PassList { get; set; }
        [JsonProperty("getPassUrl")]
        public string GetPassUrl { get; set; }
        [JsonProperty("printPassesUrl")]
        public string PrintPassesUrl { get; set; }
        [JsonProperty("mobilePassesUrl")]
        public string MobilePassesUrl { get; set; }
    }

    public class Passlist
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