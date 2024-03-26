using System;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class AuditData : EntityBase
    {
        public int ProcessTime { get; set; }
        public DateTime Timestamp { get; set; }
        public string RequestHost { get; set; }
        public string ServerName { get; set; }
        public string ServerId { get; set; }
        public string SchemaRelease { get; set; }
        public string HydraCoreRelease { get; set; }
        public string HydraEnumerationsRelease { get; set; }
        public string MerlinRelease { get; set; }
    }
}