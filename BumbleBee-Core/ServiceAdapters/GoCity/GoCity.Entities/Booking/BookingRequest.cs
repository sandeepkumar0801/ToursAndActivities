using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GoCity.GoCity.Entities.Booking
{

    public class BookingRequest
    {
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
        [JsonProperty("cartItems")]
        public List<Cartitem> CartItems { get; set; }
        [JsonProperty("details")]
        public Details Details { get; set; }
    }

    public class Customer
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class Details
    {
        [JsonProperty("travelDate")]
        public string TravelDate { get; set; }
        [JsonProperty("deliveryMethod")]
        public string DeliveryMethod { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("externalOrderNumber")]
        public string ExternalOrderNumber { get; set; }
    }

    public class Cartitem
    {
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }
        [JsonProperty("skuOptions")]
        public Skuoptions SkuOptions { get; set; }
    }

    public class Skuoptions
    {
        [JsonProperty("adult")]
        public int Adult { get; set; }
        [JsonProperty("child")]
        public int Child { get; set; }
    }
}