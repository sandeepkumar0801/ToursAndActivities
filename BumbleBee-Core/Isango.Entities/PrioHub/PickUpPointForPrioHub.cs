using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class PickUpPointForPrioHub
    {
            public string PickupPointId { get; set; }
            public string PickupPointName { get; set; }
            public string PickupPointDescription { get; set; }
            public string PickupPointLocation { get; set; }
            public List<string> PickupPointTimes { get; set; }
            public bool PickupPointAvailabilityDependency { get; set; }
    }

    public class ExtraDetailsForPrioHub
    {
        public Dictionary<int, string> PickupDetails { get; set; }
    }
}