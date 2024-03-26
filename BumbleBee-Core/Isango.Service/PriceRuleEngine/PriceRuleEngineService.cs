using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Isango.Service.PriceRuleEngine
{
    public class PriceRuleEngineService : IPriceRuleEngineService
    {
        private readonly IPriceRuleEnginePersistence _priceRuleEnginePersistence;
        private readonly IPricingRulesCacheManager _pricingRulesCacheManager;
        private readonly ILogger _log;

        public PriceRuleEngineService(IPriceRuleEnginePersistence priceRuleEnginePersistence, IPricingRulesCacheManager pricingRulesCacheManager, IMasterService masterService, ILogger log)
        {
            _priceRuleEnginePersistence = priceRuleEnginePersistence;
            _pricingRulesCacheManager = pricingRulesCacheManager;
            _log = log;
        }

        /// <summary>
        /// Fetches the Product Sale Rules
        /// </summary>
        /// <returns></returns>
        public async Task<ProductSaleRule> GetProductSaleRule()
        {
            try
            {
                var productSaleRulesByActivity = _pricingRulesCacheManager.GetProductSaleRuleByActivity();
                var productSaleRulesByOption = _pricingRulesCacheManager.GetProductSaleRuleByOption();
                var productSaleRulesByAffiliate = _pricingRulesCacheManager.GetProductSaleRuleByAffiliate();
                var productSaleRulesByCountry = _pricingRulesCacheManager.GetProductSaleRuleByCountry();

                var allRulesAvailable = (productSaleRulesByActivity?.Count() > 0) &&
                                        (productSaleRulesByOption?.Count() > 0);
                //&&
                //(productSaleRulesByAffiliate?.Count() > 0) &&
                //(productSaleRulesByCountry?.Count() > 0);

                //Ignoring above 2 rules as this can be empty.
                //Discussed with HP

                if (allRulesAvailable)
                {
                    var productSaleRule = new ProductSaleRule()
                    {
                        ProductSaleRulesByActivity = productSaleRulesByActivity,
                        ProductSaleRulesByOption = productSaleRulesByOption,
                        ProductSaleRulesByAffiliate = productSaleRulesByAffiliate,
                        ProductSaleRulesByCountry = productSaleRulesByCountry
                    };
                    return await Task.FromResult(productSaleRule);
                }

                return await Task.FromResult(_priceRuleEnginePersistence.GetProductSaleRule());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEngineService",
                    MethodName = "GetProductSaleRule"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Fetches the Product Cost Rules
        /// </summary>
        /// <returns></returns>
        public async Task<ProductSaleRule> GetProductCostSaleRule()
        {
            try
            {
                var productCostSaleRulesByActivity = _pricingRulesCacheManager.GetProductCostSaleRuleByActivity();
                var productCostSaleRulesByOption = _pricingRulesCacheManager.GetProductCostSaleRuleByOption();
                var productSaleRulesByAffiliate = _pricingRulesCacheManager.GetProductCostSaleRuleByAffiliate();
                var productSaleRulesByCountry = _pricingRulesCacheManager.GetProductCostSaleRuleByCountry();

                var allRulesAvailable = (productCostSaleRulesByActivity?.Count() > 0) &&
                                        (productCostSaleRulesByOption?.Count() > 0);
                //&&
                //(productSaleRulesByAffiliate?.Count() > 0) &&
                //(productSaleRulesByCountry?.Count() > 0);

                //Ignoring above 2 rules as this can be empty.
                //Discussed with HP

                if (allRulesAvailable)
                {
                    var productSaleRule = new ProductSaleRule()
                    {
                        ProductSaleRulesByActivity = productCostSaleRulesByActivity,
                        ProductSaleRulesByOption = productCostSaleRulesByOption,
                        ProductSaleRulesByAffiliate = productSaleRulesByAffiliate,
                        ProductSaleRulesByCountry = productSaleRulesByCountry
                    };
                    return await Task.FromResult(productSaleRule);
                }

                return await Task.FromResult(_priceRuleEnginePersistence.GetProductCostSaleRule());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEngineService",
                    MethodName = "GetProductSaleRule"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Fetches the B2B Sale Rules
        /// </summary>
        /// <returns></returns>
        public async Task<List<B2BSaleRule>> GetB2BSaleRules()
        {
            try
            {
                var b2BSaleRules = _pricingRulesCacheManager.GetB2BSaleRules();
                //?? _priceRuleEnginePersistence.GetB2BSaleRules();
                //Commenting Persistence Call, as Cache would always have latest data and data can be empty. hence no need to call Persistence.
                //Discussed with HP
                return await Task.FromResult(b2BSaleRules);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEngineService",
                    MethodName = "GetB2BSaleRules"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the B2B Net Rate Rules
        /// </summary>
        /// <returns></returns>
        public async Task<List<B2BNetRateRule>> GetB2BNetRateRules()
        {
            try
            {
                var b2BNetRateRules = _pricingRulesCacheManager.GetB2BNetRules() ?? _priceRuleEnginePersistence.GetB2BNetRateRules();
                return await Task.FromResult(b2BNetRateRules);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEngineService",
                    MethodName = "GetB2BNetRateRules"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the supplier Sale Rules
        /// </summary>
        /// <returns></returns>
        public async Task<SupplierSaleRule> GetSupplierSaleRule()
        {
            try
            {
                var supplierSaleRulesByActivity = _pricingRulesCacheManager.GetSupplierSaleRuleByActivity();
                var supplierSaleRulesByOption = _pricingRulesCacheManager.GetSupplierSaleRuleByOption();

                var supplierSaleRule = new SupplierSaleRule
                {
                    SupplierSaleRulesByActivity = supplierSaleRulesByActivity,
                    SupplierSaleRulesByOption = supplierSaleRulesByOption
                };
                return await Task.FromResult(supplierSaleRule);
                //return await Task.FromResult(_priceRuleEnginePersistence.GetSupplierSaleRule());
                //Commenting Persistence Call, as Cache would always have latest data and data can be empty. hence no need to call Persistence.
                //Discussed with HP
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEngineService",
                    MethodName = "GetSupplierSaleRule"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public DateTime? GetExpirationTime()
        {
            try
            {
                var result = _pricingRulesCacheManager.GetExpirationTime();
                if (result == null || result == default(DateTime))
                {
                    result = DateTime.UtcNow.AddHours(1);
                }
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEngineService",
                    MethodName = "GetExpirationTime"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}