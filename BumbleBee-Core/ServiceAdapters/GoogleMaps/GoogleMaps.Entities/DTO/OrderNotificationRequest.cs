using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class OrderNotificationRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("clientInformation")]
        public ClientInformation ClientInformation { get; set; }
        [JsonProperty("paymentInformation")]
        public PaymentInformation PaymentInformation { get; set; }
        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }
        [JsonProperty("item")]
        public List<Item> Item { get; set; }
    }

    public class Item
    {
        [JsonProperty("serviceId")]
        public string ServiceId { get; set; }
        [JsonProperty("startSec")]
        public string StartSec { get; set; }
        [JsonProperty("durationSec")]
        public string DurationSec { get; set; }
        [JsonProperty("tickets")]
        public List<Ticket> Tickets { get; set; }
        [JsonProperty("price")]
        public OrderPrice Price { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class OrderPrice
    {
        [JsonProperty("priceMicros")]
        public string PriceMicros { get; set; }
        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }
        [JsonProperty("pricingOptionTag")]
        public string PricingOptionTag { get; set; }
    }

    public class Ticket
    {
        [JsonProperty("ticketId")]
        public string TicketId { get; set; }
        [JsonProperty("count")]
        public string Count { get; set; }
    }
    
    public class ClientInformation
    {
        [JsonProperty("givenName")]
        public string GivenName { get; set; }
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }
        [JsonProperty("address")]
        public UserAddress Address { get; set; }
        [JsonProperty("telephone")]
        public string Telephone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class UserAddress
    {
        [JsonProperty("addressCountry")]
        public string AddressCountry { get; set; }
        [JsonProperty("addressLocality")]
        public string AddressLocality { get; set; }
        [JsonProperty("addressRegion")]
        public string AddressRegion { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }
    }

    public class PaymentInformation
    {
    }
}
