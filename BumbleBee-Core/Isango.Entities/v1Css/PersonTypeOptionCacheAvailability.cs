using System;
using System.Collections.Generic;

namespace Isango.Entities.v1Css
{
    public class AvailablePersonTypes
    {
        public List<PersonTypeOptionDateRanges> AvailablePassengerTypes { get; set; }
        public List<PersonTypeOptionCacheAvailability> AvailableDates { get; set; }
    }

    public class PersonTypeOptionCacheAvailability
    {
        public int ServiceId { get; set; }
        public int ServiceOptionId { get; set; }
        public int Capacity { get; set; }
        public DateTime? AvailableOn { get; set; }
    }

    public class PersonTypeOptionDateRanges
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PassengerTypeID { get; set; }
        public string PassengerTypename { get; set; }
        public string MeasurementDesc { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
      
    }
}