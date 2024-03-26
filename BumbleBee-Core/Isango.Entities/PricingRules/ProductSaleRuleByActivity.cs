using System;

namespace Isango.Entities.PricingRules
{
    public class ProductSaleRuleByActivity
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
        public bool SupplementRuleArriveOnSunday { get; set; }
        public bool SupplementRuleArriveOnMonday { get; set; }
        public bool SupplementRuleArriveOnTuesday { get; set; }
        public bool SupplementRuleArriveOnWednesday { get; set; }
        public bool SupplementRuleArriveOnThursday { get; set; }
        public bool SupplementRuleArriveOnFriday { get; set; }
        public bool SupplementRuleArriveOnSaturday { get; set; }
        public bool APPLIEDTOBUYRATES { get; set; }
        public bool SUPPLEMENTRULEWEEKDAYONARRIVAL { get; set; }

    }
}