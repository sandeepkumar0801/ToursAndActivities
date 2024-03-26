using System;
using System.Collections.Generic;


namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class AvailabilityReturnData
    {
        public List<Dictionary<DateTime,AvailabilityTourOptionRS>> AvailabilityTourOptionRS { get; set; }

        public List<Tuple<DateTime, int, int, int,AvailabilityTimeSlotRS>> AvailabilityTimeSlotRS { get; set; }

        public List<Tuple<DateTime,int,int,int,AvailabilityRES>> AvailabilityRES { get; set; }

    }
}
