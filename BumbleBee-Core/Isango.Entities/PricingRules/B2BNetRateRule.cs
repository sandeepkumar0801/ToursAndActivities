using System;

namespace Isango.Entities.PricingRules
{
    public class B2BNetRateRule
    {
        public string AffiliateId { get; set; }
        public decimal NetRatePercent { get; set; }
        public int NetPriceType { get; set; }
        public DateTime BookingFromDate { get; set; }
        public DateTime BookingToDate { get; set; }
    }
}