using Newtonsoft.Json;
using System;

namespace Isango.Entities
{
    public class CalendarAvailability
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public int ActivityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RegionId { get; set; }
        public string Currency { get; set; }
        public string AffiliateId { get; set; }
        public decimal B2CBasePrice { get; set; }
        public decimal B2BBasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public long _ts { get; set; }
    }
}