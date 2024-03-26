using Isango.Service.Contract;
using PriceRuleEngine.Constants;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PriceRuleEngine.ModuleBuilders
{
    /// <summary>
    /// This class will fetch the registered modules of B2B category
    /// </summary>
    public class B2BModuleBuilder : BaseModuleBuilder
    {
        private ConcurrentDictionary<string, List<BaseModule>> _moduleDictionary;

        public B2BModuleBuilder(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
            _moduleDictionary = new ConcurrentDictionary<string, List<BaseModule>>();
        }

        /// <summary>
        /// Fetch the registered modules for the B2B category
        /// </summary>
        /// <returns></returns>
        public override List<BaseModule> GetModules()
        {
            var isBaseModulesExist = _moduleDictionary.TryGetValue(Constant.B2BCategory, out var baseModules);
            if (isBaseModulesExist) return baseModules;

            //TODO: This object will be created using AutoFac IOC container
            var configReader = new ConfigReader();
            var modulesNames = configReader.GetModuleNames(Constant.B2BCategory);
            var modules = GetModules(modulesNames);
            if (!_moduleDictionary.ContainsKey(Constant.B2BCategory))
                _moduleDictionary.TryAdd(Constant.B2BCategory, modules);
            return modules;
        }
    }
}