using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.RedeamV12.RedeamV12.Entities.CreateBooking
{

    public class CreateBookingResponse
    {
        [JsonProperty("booking")]
        public BookingData Booking { get; set; }
        [JsonProperty("meta")]
        public MetaData Meta { get; set; }


        [JsonProperty("error")]
        public Error Error { get; set; }
       
    }
    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class BookingData
    {
        [JsonProperty("cancelable")]
        public bool Cancelable { get; set; }
        [JsonProperty("customer")]
        public CustomerData Customer { get; set; }
        [JsonProperty("ext")]
        public Ext Ext { get; set; }
        [JsonProperty("holdId")]
        public string HoldId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("items")]
        public List<ItemData> Items { get; set; }
        [JsonProperty("resellerBookingRef")]
        public string ResellerBookingRef { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("tickets")]
        public List<Ticket> Tickets { get; set; }
        [JsonProperty("timeline")]
        public List<Timeline> Timeline { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class CustomerData
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

    public class Ext
    {
        [JsonProperty("backend")]
        public string Backend { get; set; }
        [JsonProperty("resellerref")]
        public string Resellerref { get; set; }
    }

    public class ItemData
    {
        [JsonProperty("availabilityId")]
        public string AvailabilityId { get; set; }
        [JsonProperty("ext")]
        public Ext1 Ext { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("priceId")]
        public string PriceId { get; set; }
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("rate")]
        public Rate Rate { get; set; }
        [JsonProperty("rateId")]
        public string RateId { get; set; }
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }
        [JsonProperty("traveler")]
        public TravelerData Traveler { get; set; }
    }

    public class Ext1
    {
        [JsonProperty("backend")]
        public string Backend { get; set; }
    }

    public class Rate
    {
        [JsonProperty("cancelable")]
        public bool Cancelable { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("cutoff")]
        public int Cutoff { get; set; }
        [JsonProperty("ext")]
        public Ext2 Ext { get; set; }
        [JsonProperty("holdable")]
        public bool Holdable { get; set; }
        [JsonProperty("holdablePeriod")]
        public int HoldablePeriod { get; set; }
        [JsonProperty("hours")]
        public List<Hour> Hours { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("maxTravelers")]
        public int MaxTravelers { get; set; }
        [JsonProperty("minTravelers")]
        public int MinTravelers { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("prices")]
        public List<Price> Prices { get; set; }
        [JsonProperty("pricingType")]
        public string PricingType { get; set; }
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("refundable")]
        public bool Refundable { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class Ext2
    {
    }

    public class Hour
    {
        [JsonProperty("dates")]
        public List<object> Dates { get; set; }
        [JsonProperty("daysOfWeek")]
        public List<int> DaysOfWeek { get; set; }
        [JsonProperty("times")]
        public List<Time> Times { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("valid")]
        public Valid Valid { get; set; }
    }

    public class Valid
    {
        [JsonProperty("from")]
        public DateTime From { get; set; }
        [JsonProperty("until")]
        public DateTime Until { get; set; }
    }

    public class Time
    {
        [JsonProperty("close")]
        public string Close { get; set; }
        [JsonProperty("open")]
        public string Open { get; set; }
    }

    public class Price
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("includedTaxes")]
        public List<Includedtax> IncludedTaxes { get; set; }
        [JsonProperty("labels")]
        public List<object> Labels { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("net")]
        public Net Net { get; set; }
        [JsonProperty("retail")]
        public Retail Retail { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("travelerType")]
        public Travelertype TravelerType { get; set; }
        [JsonProperty("original")]
        public Original Original { get; set; }
    }

    public class Net
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Retail
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Travelertype
    {
        [JsonProperty("ageBand")]
        public string AgeBand { get; set; }
        [JsonProperty("maxAge")]
        public int MaxAge { get; set; }
        [JsonProperty("minAge")]
        public int MinAge { get; set; }
        [JsonProperty("modifier")]
        public string Modifier { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Original
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Includedtax
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("net")]
        public int Net { get; set; }
        [JsonProperty("retail")]
        public int Retail { get; set; }
    }

    public class TravelerData
    {
        [JsonProperty("age")]
        public int Age { get; set; }
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

    public class Ticket
    {
        [JsonProperty("barcode")]
        public Barcode Barcode { get; set; }
        [JsonProperty("bookingItemIds")]
        public List<string> BookingItemIds { get; set; }
        [JsonProperty("guests")]
        public List<Guest> Guests { get; set; }
        [JsonProperty("leadTraveler")]
        public Leadtraveler LeadTraveler { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Barcode
    {
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Leadtraveler
    {
        [JsonProperty("age")]
        public int Age { get; set; }
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

    public class Guest
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("typeCode")]
        public string TypeCode { get; set; }
        [JsonProperty("typeModifier")]
        public string TypeModifier { get; set; }
        [JsonProperty("typeName")]
        public string TypeName { get; set; }
    }

    public class Timeline
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("typeOf")]
        public string TypeOf { get; set; }
    }

    public class MetaData
    {
        [JsonProperty("reqId")]
        public string ReqId { get; set; }
    }

}