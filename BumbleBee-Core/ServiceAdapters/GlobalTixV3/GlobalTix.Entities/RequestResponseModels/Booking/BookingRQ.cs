using Newtonsoft.Json;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class BookingRQ
    {
        [JsonProperty(PropertyName = "referenceNumber")]
        public string ReferenceNumber { get; set; }
        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }

    }
}
