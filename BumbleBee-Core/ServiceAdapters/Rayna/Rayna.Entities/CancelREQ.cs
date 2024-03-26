using Newtonsoft.Json;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class CancelREQ
    {
        [JsonProperty(PropertyName = "bookingId")]
        public int BookingId { get; set; }
        [JsonProperty(PropertyName = "referenceNo")]
        public string ReferenceNo { get; set; }
        [JsonProperty(PropertyName = "cancellationReason")]
        public string CancellationReason { get; set; }
    }
}
