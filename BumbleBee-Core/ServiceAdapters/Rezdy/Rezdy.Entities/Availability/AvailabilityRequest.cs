using System;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.Availability
{
    public class AvailabilityRequest
    {
        public string ProductCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}