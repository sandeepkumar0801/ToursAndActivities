using Isango.Entities.Activities;
using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.Availability
{
    public class AvailabilityResponse
    {
        public Activity Activity { get; set; }
        public List<ActivityLite> SimilarProducts { get; set; }
        public ActivityType ActivityType { get; set; }
        public APIType APIType { get; set; }
    }
}