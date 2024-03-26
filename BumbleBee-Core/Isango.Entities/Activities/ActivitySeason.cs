using System;
using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    public class ActivitySeason
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Id { get; set; }
        public List<ActivityPolicy> ActivityPolicies { get; set; }
    }
}