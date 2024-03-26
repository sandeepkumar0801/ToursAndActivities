using Isango.Entities.PricingRules;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;

namespace PriceRuleEngine.Modules
{
    public sealed class ModuleDataSingleton
    {
        public ModuleDataSingleton()
        {
        }

        public ModuleData ModuleData { get; set; }

        public DateTime ExpirationTime { get; private set; }

        public bool LoadRulesIfExpired => CheckExpirationTime();

        public IPriceRuleEngineService PriceRuleEngineService { get; set; }

        public static ModuleDataSingleton Instance => Nested.Instance;

        private ModuleData GetModuleData()
        {
            if (PriceRuleEngineService == null)
                return null;
            var moduleData = new ModuleData
            {
                B2BSaleRules = PriceRuleEngineService?.GetB2BSaleRules().GetAwaiter().GetResult(),
                ProductSaleRules = PriceRuleEngineService?.GetProductSaleRule().GetAwaiter().GetResult(),
                SupplierSaleRules = PriceRuleEngineService?.GetSupplierSaleRule().GetAwaiter().GetResult(),
                B2BNetRateRules = PriceRuleEngineService?.GetB2BNetRateRules().GetAwaiter().GetResult(),
                ProductCostSaleRules = PriceRuleEngineService?.GetProductCostSaleRule().GetAwaiter().GetResult()
            };
            if (PriceRuleEngineService != null)
                ExpirationTime = GetExpirationTime();
            return moduleData;
        }

        private bool CheckExpirationTime()
        {
            var expirationTime = GetExpirationTime();
            if (expirationTime > ExpirationTime || ModuleData == null)
                ModuleData = GetModuleData();
            return true;
        }

        private DateTime GetExpirationTime()
        {
            var result = PriceRuleEngineService?.GetExpirationTime();
            return result ?? DateTime.MinValue;
        }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly ModuleDataSingleton Instance = new ModuleDataSingleton();
        }
    }

    public class ModuleData
    {
        public List<B2BSaleRule> B2BSaleRules { get; set; }
        public ProductSaleRule ProductSaleRules { get; set; }
        public SupplierSaleRule SupplierSaleRules { get; set; }
        public List<B2BNetRateRule> B2BNetRateRules { get; set; }
        public ProductSaleRule ProductCostSaleRules { get; set; }
    }
}