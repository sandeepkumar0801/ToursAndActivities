using Newtonsoft.Json;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class PostalAddress
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }
    }
}