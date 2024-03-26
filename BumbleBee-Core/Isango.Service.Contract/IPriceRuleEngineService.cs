using Isango.Entities.PricingRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IPriceRuleEngineService
    {
        Task<ProductSaleRule> GetProductSaleRule();

        Task<List<B2BSaleRule>> GetB2BSaleRules();

        Task<List<B2BNetRateRule>> GetB2BNetRateRules();

        Task<SupplierSaleRule> GetSupplierSaleRule();

        DateTime? GetExpirationTime();

        Task<ProductSaleRule> GetProductCostSaleRule();
    }
}