namespace Isango.Entities.PricingRules
{
    public class SupplierSaleRuleByOption
    {
        public int AppliedRuleId { get; set; }
        public int ServiceOptionInServiceId { get; set; }
        public int PriorityOrder { get; set; }
    }
}