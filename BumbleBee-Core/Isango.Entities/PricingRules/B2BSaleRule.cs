using System;

namespace Isango.Entities.PricingRules
{
    public class B2BSaleRule
    {
        public string AffiliateId { get; set; }
        public string SaleDescription { get; set; }
        public decimal B2BSaleOfferPercent { get; set; }
        public decimal MinMarginCapPercent { get; set; }
        public DateTime BookingFromDate { get; set; }
        public DateTime BookingToDate { get; set; }
    }
}