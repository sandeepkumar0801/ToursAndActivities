using Isango.Entities;
using Isango.Service.Contract;
using PriceRuleEngine.Modules;
using System.Collections.Generic;

namespace PriceRuleEngine
{
    public abstract class BaseModule
    {
        protected readonly IPriceRuleEngineService _priceRuleEngineService;

        protected BaseModule(IPriceRuleEngineService priceRuleEngineService)
        {
            _priceRuleEngineService = priceRuleEngineService;
            ModuleDataSingleton.Instance.PriceRuleEngineService = priceRuleEngineService;
        }

        public abstract List<ProductOption> Process(PricingRuleRequest request);
    }
}