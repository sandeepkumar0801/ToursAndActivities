using System;

namespace Isango.Entities.PricingRules
{
    public class SupplierSaleRuleByActivity
    {
        public int AppliedRuleId { get; set; }
        public string RuleName { get; set; }
        public DateTime TravelFromDate { get; set; }
        public DateTime TravelToDate { get; set; }
        public DateTime BookingFromDate { get; set; }
        public DateTime BookingToDate { get; set; }
        public int ServiceId { get; set; }
        public decimal SaleRuleOfferPercent { get; set; }
        public bool ShowSale { get; set; }
    }
}