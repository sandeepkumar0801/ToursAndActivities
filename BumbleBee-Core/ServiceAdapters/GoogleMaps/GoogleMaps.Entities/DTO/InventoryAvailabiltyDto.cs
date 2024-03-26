using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class InventoryAvailabiltyDto
    {
        [JsonProperty("extendedServiceAvailability")]
        public List<ExtendedServiceAvailability> ExtendedServiceAvailability { get; set; }
    }

    public class ExtendedServiceAvailability
    {
        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }
        [JsonProperty("serviceId")]
        public string ServiceId { get; set; }
        [JsonProperty("startTimeRestrict")]
        public string StartTimeRestrict { get; set; }
        [JsonProperty("endTimeRestrict")]
        public string EndTimeRestrict { get; set; }
        [JsonProperty("durationRestrict")]
        public string DurationRestrict { get; set; }
        [JsonProperty("resourcesRestrict")]
        public Resources ResourcesRestrict { get; set; }
        [JsonProperty("availability")]
        public List<InventoryAvailability> Availability { get; set; }

    }

    public class Resources
    {
        [JsonProperty("staffId")]
        public string StaffId { get; set; }
        [JsonProperty("staffName")]
        public string StaffName { get; set; }
        [JsonProperty("roomId")]
        public string RoomId { get; set; }
        [JsonProperty("roomName")]
        public string RoomName { get; set; }
        [JsonProperty("partySize")]
        public string PartySize { get; set; }

    }
    public class InventoryAvailability
    {
        [JsonProperty("ticket_type_id")]
        public List<string> TicketTypeId { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("spots_total")]
        public string SpotsTotal { get; set; }

        [JsonProperty("spots_open")]
        public string SpotsOpen { get; set; }
        [JsonProperty("confirmation_mode")]
        public string ConfirmationMode { get; set; }
    }

    public class Recurrence
    {
        [JsonProperty("repeatUntil")]
        public string RepeatUntil { get; set; }
        [JsonProperty("repeatEvery")]
        public string RepeatEvery { get; set; }

    }

    public class TimeRange
    {
        [JsonProperty("startTime")]
        public string StartTime { get; set; }
        [JsonProperty("endTime")]
        public string EndTime { get; set; }
    }

    public class ScheduleException
    {
        [JsonProperty("timeRange")]
        public TimeRange TimeRange { get; set; }
    }
}