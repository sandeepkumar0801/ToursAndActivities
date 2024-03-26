using System;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class CancellationPolicy : EntityBase
    {
        public decimal Amount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int FromTime { get; set; }
        public float Percentage { get; set; }
    }
}