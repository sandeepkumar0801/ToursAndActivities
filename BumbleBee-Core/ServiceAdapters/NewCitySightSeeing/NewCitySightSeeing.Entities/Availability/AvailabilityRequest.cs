using System;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability
{
    public class AvailabilityRequest
    {
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
    }
}