using System.Collections.Generic;

namespace Isango.Entities.PricingRules
{
    public class SupplierSaleRule
    {
        public List<SupplierSaleRuleByActivity> SupplierSaleRulesByActivity { get; set; }
        public List<SupplierSaleRuleByOption> SupplierSaleRulesByOption { get; set; }
    }
}