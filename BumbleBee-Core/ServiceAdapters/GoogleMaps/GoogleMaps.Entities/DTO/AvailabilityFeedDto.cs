using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class AvailabilityFeedDto
    {
        [JsonProperty("metadata")]
        public AvailabilityMetadata Metadata { get; set; }

        [JsonProperty("service_availability")]
        public List<ServiceAvailability> ServiceAvailability { get; set; }
    }

    public class AvailabilityMetadata
    {
        [JsonProperty("processing_instruction")]
        public string ProcessingInstruction { get; set; }

        [JsonProperty("shard_number")]
        public string ShardNumber { get; set; }

        [JsonProperty("total_shards")]
        public int TotalShards { get; set; }

        [JsonProperty("generation_timestamp")]
        public string GenerationTimestamp { get; set; }
    }

    public class ServiceAvailability
    {
        [JsonProperty("availability")]
        public List<Availability> Availability { get; set; }
    }

    public class Availability
    {
        [JsonProperty("ticket_type_id")]
        public List<string> TicketTypeId { get; set; }

        [JsonProperty("start_sec")]
        public string StartSec { get; set; }

        [JsonProperty("duration_sec")]
        public string DurationSec { get; set; }

        [JsonProperty("spots_total")]
        public string SpotsTotal { get; set; }

        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("spots_open")]
        public string SpotsOpen { get; set; }

        [JsonProperty("confirmation_mode", NullValueHandling = NullValueHandling.Ignore)]
        public string ConfirmationMode { get; set; }
    }
}