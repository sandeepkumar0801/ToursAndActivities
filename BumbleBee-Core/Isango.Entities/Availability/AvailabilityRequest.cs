using Isango.Entities.Activities;
using System;

namespace Isango.Entities.Availability
{
    public class AvailabilityRequest
    {
        public Activity Activity { get; set; }
        public string ActivityType { get; set; }
        public DateTime Date { get; set; }
        public string ApiType { get; set; }
        public string RegionId { get; set; }
        public string CategoryId { get; set; }
    }
}