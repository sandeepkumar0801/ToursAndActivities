using Isango.Entities.PricingRules;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IPriceRuleEnginePersistence
    {
        ProductSaleRule GetProductSaleRule();

        List<B2BSaleRule> GetB2BSaleRules();

        List<B2BNetRateRule> GetB2BNetRateRules();

        SupplierSaleRule GetSupplierSaleRule();

        ProductSaleRule GetProductCostSaleRule();
    }
}