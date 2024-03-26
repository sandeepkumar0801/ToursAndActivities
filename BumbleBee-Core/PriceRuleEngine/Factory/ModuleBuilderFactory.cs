using Isango.Service.Contract;
using PriceRuleEngine.ModuleBuilders;
using System;
using System.Collections.Concurrent;

namespace PriceRuleEngine.Factory
{
    /// <summary>
    /// This class will return the object of the ModuleBuilder
    /// </summary>
    public class ModuleBuilderFactory
    {
        private ConcurrentDictionary<string, BaseModuleBuilder> _moduleBuilderDictionary;
        private readonly IPriceRuleEngineService _priceRuleEngineService;

        public ModuleBuilderFactory(IPriceRuleEngineService priceRuleEngineService)
        {
            _priceRuleEngineService = priceRuleEngineService;
            _moduleBuilderDictionary = new ConcurrentDictionary<string, BaseModuleBuilder>();
        }
      
        /// <summary>
        /// This method returns the object of the ModuleBuilder for the given category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public BaseModuleBuilder GetModuleBuilder(string category)
        {
            var isServiceInstanceExist = _moduleBuilderDictionary.TryGetValue(category, out var instance);
            if (isServiceInstanceExist) return instance;

            var configReader = new ConfigReader();
            var moduleBuilder = configReader.GetModuleBuilder(category);
            if (string.IsNullOrWhiteSpace(moduleBuilder)) return null;

            var type = Type.GetType(moduleBuilder);
            object[] args = { _priceRuleEngineService };

            if (type == null)
                return null;
            instance = (BaseModuleBuilder) Activator.CreateInstance(type, args);
            if (!_moduleBuilderDictionary.ContainsKey(category))
                _moduleBuilderDictionary.TryAdd(category, instance);

            return instance;
        }
    }
}