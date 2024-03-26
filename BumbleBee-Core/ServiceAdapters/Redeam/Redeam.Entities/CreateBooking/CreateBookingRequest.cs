using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.Redeam.Redeam.Entities.CreateBooking
{
    public class CreateBookingRequest
    {
        [JsonProperty("booking")]
        public Booking Booking { get; set; }
    }

    public class Booking
    {
        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("holdId")]
        public string HoldId { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("resellerBookingRef")]
        public string ResellerBookingRef { get; set; }
    }

    public class Customer
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public class Item
    {
        [JsonProperty("priceId")]
        public string PriceId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("rateId")]
        public string RateId { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("traveler")]
        public Traveler Traveler { get; set; }
    }

    public class Traveler
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("isLead")]
        public bool IsLead { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Meta
    {
        [JsonProperty("reqId")]
        public string ReqId { get; set; }
    }
}