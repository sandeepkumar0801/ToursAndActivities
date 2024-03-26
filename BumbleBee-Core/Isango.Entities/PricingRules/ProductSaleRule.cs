using System.Collections.Generic;

namespace Isango.Entities.PricingRules
{
    public class ProductSaleRule
    {
        public List<ProductSaleRuleByActivity> ProductSaleRulesByActivity { get; set; }
        public List<ProductSaleRuleByOption> ProductSaleRulesByOption { get; set; }
        public List<ProductSaleRuleByAffiliate> ProductSaleRulesByAffiliate { get; set; }
        public List<ProductSaleRuleByCountry> ProductSaleRulesByCountry { get; set; }
    }
}