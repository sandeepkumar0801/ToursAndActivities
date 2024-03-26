using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace ServiceAdapters.RedeamV12.RedeamV12.Entities.GetProducts
{
    public class GetProductsResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
        [JsonProperty("products")]
        public List<Product> Products { get; set; }
    }

    public class Meta
    {
        [JsonProperty("reqId")]
        public string ReqId { get; set; }
    }

    public class Product
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("contacts")]
        public List<Contact> Contacts { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("extensions")]
        public Extensions Extensions { get; set; }
        [JsonProperty("hours")]
        public object[] Hours { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("otherLocations")]
        public List<OtherLocations> OtherLocations { get; set; }
        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
    }
    public class OtherLocations
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("utcOffset")]
        public string UtcOffset { get; set; }
    }
    public class Contact
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("primary")]
        public bool Primary { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
    public class Extensions
    {
        [JsonProperty("passhubvendorId")]
        public string PasshubvendorId { get; set; }
        [JsonProperty("passhubvendorName")]
        public string PasshubvendorName { get; set; }
    }

    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("notes")]
        public string Notes { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
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

        [JsonProperty("googleplaceId")]
        public string GooglePlaceId { get; set; }
    }


}