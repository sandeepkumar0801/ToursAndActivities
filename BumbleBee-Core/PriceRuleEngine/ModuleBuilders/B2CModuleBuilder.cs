using Isango.Service.Contract;
using PriceRuleEngine.Constants;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PriceRuleEngine.ModuleBuilders
{
    /// <summary>
    /// This class will fetch the registered modules of B2C category
    /// </summary>
    public class B2CModuleBuilder : BaseModuleBuilder
    {
        private ConcurrentDictionary<string, List<BaseModule>> _moduleDictionary;

        public B2CModuleBuilder(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
            _moduleDictionary = new ConcurrentDictionary<string, List<BaseModule>>();
        }

        /// <summary>
        /// Fetch the registered modules for the B2C category
        /// </summary>
        /// <returns></returns>
        public override List<BaseModule> GetModules()
        {
            var isBaseModulesExist = _moduleDictionary.TryGetValue(Constant.B2CCategory, out var baseModules);
            if (isBaseModulesExist) return baseModules;

            //TODO: This object will be created using AutoFac IOC container
            var configReader = new ConfigReader();
            var modulesNames = configReader.GetModuleNames(Constant.B2CCategory);
            var modules = GetModules(modulesNames);
            if (!_moduleDictionary.ContainsKey(Constant.B2CCategory))
                _moduleDictionary.TryAdd(Constant.B2CCategory, modules);
            return modules;
        }
    }
}