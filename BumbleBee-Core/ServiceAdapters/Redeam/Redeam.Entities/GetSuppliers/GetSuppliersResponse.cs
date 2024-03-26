using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.Redeam.Redeam.Entities.GetSuppliers
{
    public class GetSuppliersResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("suppliers")]
        public List<Supplier> Suppliers { get; set; }
    }

    public class Meta
    {
        [JsonProperty("reqId")]
        public Guid ReqId { get; set; }
    }

    public class Supplier
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("ext")]
        public Ext Ext { get; set; }

        [JsonProperty("hours")]
        public List<Hour> Hours { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("mainLocation")]
        public Location MainLocation { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("otherLocations")]
        public List<Location> OtherLocations { get; set; }

        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }

        [JsonProperty("travelerTypes")]
        public List<TravelerType> TravelerTypes { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class Ext
    {
        [JsonProperty("string")]
        public string String { get; set; }
    }

    public class Hour
    {
        [JsonProperty("dates")]
        public List<DateTimeOffset> Dates { get; set; }

        [JsonProperty("daysOfWeek")]
        public List<long> DaysOfWeek { get; set; }

        [JsonProperty("times")]
        public List<Time> Times { get; set; }

        [JsonProperty("valid")]
        public Valid Valid { get; set; }
    }

    public class Time
    {
        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("open")]
        public string Open { get; set; }
    }

    public class Valid
    {
        [JsonProperty("from")]
        public DateTimeOffset From { get; set; }

        [JsonProperty("until")]
        public DateTimeOffset Until { get; set; }
    }

    public class Location
    {
        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("longLat")]
        public LongLat LongLat { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("utcOffset")]
        public string UtcOffset { get; set; }
    }

    public class Address
    {
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }
    }

    public class LongLat
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }

    public class TravelerType
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
}