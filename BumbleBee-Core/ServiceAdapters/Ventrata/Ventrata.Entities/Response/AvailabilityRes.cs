using Newtonsoft.Json;
using System;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Response
{
    public class AvailabilityRes : EntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("localDateTimeStart")]
        public string LocalDateTimeStart { get; set; }
        [JsonProperty("localDateTimeEnd")]
        public string LocalDateTimeEnd { get; set; }
        [JsonProperty("allDay")]
        public bool AllDay { get; set; }
        [JsonProperty("available")]
        public bool Available { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("vacancies")]
        public int Vacancies { get; set; }
        [JsonProperty("capacity")]
        public string Capacity { get; set; }
        [JsonProperty("maxUnits")]
        public string MaxUnits { get; set; }
        [JsonProperty("openingHours")]
        public Openinghour[] OpeningHours { get; set; }
        [JsonProperty("meetingPoint")]
        public string MeetingPoint { get; set; }
        [JsonProperty("meetingPointCoordinates")]
        public object MeetingPointCoordinates { get; set; }
        [JsonProperty("meetingLocalDateTime")]
        public string MeetingLocalDateTime { get; set; }
        [JsonProperty("unitPricing")]
        public Unitpricing[] UnitPricing { get; set; }
        [JsonProperty("pricing")]
        public Pricing Pricing { get; set; }
        [JsonProperty("pickupAvailable")]
        public bool PickupAvailable { get; set; }
        [JsonProperty("pickupRequired")]
        public bool PickupRequired { get; set; }
        [JsonProperty("pickupPoints")]
        public Pickuppoint[] PickupPoints { get; set; }
        [JsonProperty("offerCode")]
        public object OfferCode { get; set; }
        [JsonProperty("offerTitle")]
        public object OfferTitle { get; set; }
    }

    public class Pricing
    {
        [JsonProperty("original")]
        public int Original { get; set; }
        [JsonProperty("retail")]
        public int Retail { get; set; }
        [JsonProperty("net")]
        public int Net { get; set; }
        [JsonProperty("currencyPrecision")]
        public int CurrencyPrecision { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("includedTaxes")]
        public object[] IncludedTaxes { get; set; }
    }

    public class Openinghour
    {
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class Unitpricing
    {
        [JsonProperty("original")]
        public int Original { get; set; }
        [JsonProperty("retail")]
        public int Retail { get; set; }
        [JsonProperty("net")]
        public int Net { get; set; }
        [JsonProperty("currencyPrecision")]
        public int CurrencyPrecision { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("includedTaxes")]
        public object[] IncludedTaxes { get; set; }
        [JsonProperty("unitId")]
        public string UnitId { get; set; }
    }

    public class Pickuppoint
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("directions")]
        public string Directions { get; set; }
        [JsonProperty("latitude")]
        public float? Latitude { get; set; }
        [JsonProperty("longitude")]
        public float? Longitude { get; set; }
        [JsonProperty("localDateTime")]
        public string LocalDateTime { get; set; }
        [JsonProperty("googlePlaceId")]
        public string GooglePlaceId { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("locality")]
        public string Locality { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
    }


}
