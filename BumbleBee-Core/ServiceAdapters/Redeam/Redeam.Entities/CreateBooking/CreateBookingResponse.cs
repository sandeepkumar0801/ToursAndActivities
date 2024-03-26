using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.Redeam.Redeam.Entities.CreateBooking
{
    public class CreateBookingResponse
    {
        [JsonProperty("booking")]
        public ResponseBooking Booking { get; set; }
    }

    public class ResponseBooking
    {
        [JsonProperty("customer")]
        public ResponseCustomer Customer { get; set; }

        [JsonProperty("ext")]
        public BookingExt Ext { get; set; }

        [JsonProperty("holdId")]
        public string HoldId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("items")]
        public List<ResponseItem> Items { get; set; }

        [JsonProperty("resellerBookingRef")]
        public string ResellerBookingRef { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tickets")]
        public List<ResponseTicket> Tickets { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }
    }

    public class ResponseCustomer
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public class BookingExt
    {
        [JsonProperty("reseller.ref")]
        public string ResellerRef { get; set; }

        [JsonProperty("passhub.orderRecordId")]
        public string OrderRecordId { get; set; }
    }

    public class ResponseItem
    {
        [JsonProperty("availabilityId")]
        public string AvailabilityId { get; set; }

        [JsonProperty("ext")]
        public object Ext { get; set; }

        [JsonProperty("priceId")]
        public string PriceId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("rate")]
        public ResponseRate Rate { get; set; }

        [JsonProperty("rateId")]
        public string RateId { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("traveler")]
        public ResponseTraveler Traveler { get; set; }
    }

    public class ItemExt
    {
    }

    public class ResponseRate
    {
        [JsonProperty("cancelable")]
        public bool Cancelable { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("cutoff")]
        public long Cutoff { get; set; }

        [JsonProperty("holdable")]
        public bool Holdable { get; set; }

        [JsonProperty("holdablePeriod")]
        public long HoldablePeriod { get; set; }

        [JsonProperty("hours")]
        public List<ResponseHour> Hours { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("maxTravelers")]
        public long MaxTravelers { get; set; }

        [JsonProperty("minTravelers")]
        public long MinTravelers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }

        [JsonProperty("prices")]
        public List<ResponsePrice> Prices { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("refundable")]
        public bool Refundable { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }
    }

    public class ResponseHour
    {
        [JsonProperty("dates")]
        public List<DateTimeOffset> Dates { get; set; }

        [JsonProperty("daysOfWeek")]
        public List<object> DaysOfWeek { get; set; }

        [JsonProperty("times")]
        public List<ResponseTime> Times { get; set; }
    }

    public class ResponseTime
    {
        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("open")]
        public string Open { get; set; }
    }

    public class ResponsePrice
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("net")]
        public ResponseNet Net { get; set; }

        [JsonProperty("retail")]
        public ResponseNet Retail { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("travelerType")]
        public ResponseTravelerType TravelerType { get; set; }
    }

    public class ResponseNet
    {
        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class ResponseTravelerType
    {
        [JsonProperty("ageBand")]
        public string AgeBand { get; set; }

        [JsonProperty("maxAge")]
        public long MaxAge { get; set; }

        [JsonProperty("minAge")]
        public long MinAge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ResponseTraveler
    {
        [JsonProperty("age")]
        public long Age { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("isLead")]
        public bool IsLead { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ResponseTicket
    {
        [JsonProperty("barcode")]
        public ResponseBarcode Barcode { get; set; }

        [JsonProperty("guests")]
        public List<ResponseGuest> Guests { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }
    }

    public class ResponseBarcode
    {
        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class ResponseGuest
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("typeName")]
        public string TypeName { get; set; }
    }
}