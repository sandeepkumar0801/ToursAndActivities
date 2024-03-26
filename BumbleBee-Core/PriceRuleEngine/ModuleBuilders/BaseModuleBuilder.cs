using Isango.Service.Contract;
using System;
using System.Collections.Generic;

namespace PriceRuleEngine.ModuleBuilders
{
    public abstract class BaseModuleBuilder
    {
        protected readonly IPriceRuleEngineService _priceRuleEngineService;

        protected BaseModuleBuilder(IPriceRuleEngineService priceRuleEngineService)
        {
            _priceRuleEngineService = priceRuleEngineService;
        }

        public abstract List<BaseModule> GetModules();

        /// <summary>
        /// Returns the registered modules for the given category
        /// </summary>
        /// <param name="moduleNames"></param>
        /// <returns></returns>
        protected List<BaseModule> GetModules(List<string> moduleNames)
        {
            var modules = new List<BaseModule>();
            if (moduleNames == null || moduleNames.Count == 0) return modules;

            foreach (var moduleName in moduleNames)
            {
                var type = Type.GetType(moduleName);
                Object[] args = { _priceRuleEngineService };
                // ReSharper disable once AssignNullToNotNullAttribute
                var instance = Activator.CreateInstance(type, args);

                var baseModule = (BaseModule)instance;
                modules.Add(baseModule);
            }
            return modules;
        }
    }
}