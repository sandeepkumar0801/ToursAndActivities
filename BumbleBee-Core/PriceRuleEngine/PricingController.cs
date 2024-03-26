using Isango.Entities;
using Logger.Contract;
using PriceRuleEngine.Constants;
using PriceRuleEngine.Factory;
using PriceRuleEngine.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace PriceRuleEngine
{
    /// <summary>
    /// Fetch the modules applicable for a category and traverse the given activity through those modules
    /// </summary>
    public class PricingController
    {
        private readonly ILogger _log;
        private readonly ModuleBuilderFactory _moduleBuilderFactory;
        private readonly string _saveInStorageLogPriceOffers;

        public PricingController(ILogger log, ModuleBuilderFactory moduleBuilderFactory)
        {
            _log = log;
            _moduleBuilderFactory = moduleBuilderFactory;
            _saveInStorageLogPriceOffers = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.LogPriceOffers);
        }

        public List<ProductOption> Process(PricingRuleRequest request)
        {
            try
            {
                //As we are getting bool property in ClientInfo to determine whether affiliate is B2B or B2C, we've created enum internally and passing it to factory.
                //This condition can be removed in future if any property of enum type is introduced for category in ClientInfo
                var category = request.ClientInfo.IsB2BAffiliate ? Constant.B2BCategory : Constant.B2CCategory;

                var moduleBuilder = _moduleBuilderFactory.GetModuleBuilder(category);
                if (moduleBuilder == null) return request.ProductOptions;

                var modules = moduleBuilder.GetModules();

                // Check and load rules in memory if Expired
                var unused = ModuleDataSingleton.Instance.LoadRulesIfExpired;
                foreach (var module in modules)
               {
                    try
                    {
                        request.ProductOptions = module.Process(request);
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() =>
                        _log.Error(new IsangoErrorEntity
                        {
                            ClassName = "PricingController",
                            MethodName = "Process",
                            Params = $"{module}|{SerializeDeSerializeHelper.Serialize(request)}",
                            Token = request.ClientInfo.ApiToken
                        }, ex)
                        );
                        continue;
                    }
                }
                if (_saveInStorageLogPriceOffers.Equals(Constant.LogPriceOffersValue))
                {
                    Task.Run(() => LogPriceOffers(request.ProductOptions));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                _log.Error(new IsangoErrorEntity
                {
                    ClassName = "PricingController",
                    MethodName = "Process",
                    Params = $"{SerializeDeSerializeHelper.Serialize(request)}",
                    Token = request.ClientInfo.ApiToken
                }, ex)
                );
                throw;
            }
            return request.ProductOptions;
        }

        #region Private Methods

        private void LogPriceOffers(List<ProductOption> options)
        {
            if (options?.Any() == true)
            {
                foreach (var option in options)
                {
                    if (option?.PriceOffers != null)
                    {
                        _log.Info($"PricingController|Process|{option.Id},{SerializeDeSerializeHelper.Serialize(option.PriceOffers)}");
                    }
                }
            }
        }

        #endregion Private Methods
    }
}