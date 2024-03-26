using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.PriceRuleEngine
{
    public class PriceRuleEnginePersistence : PersistenceBase, IPriceRuleEnginePersistence
    {
        private readonly ILogger _log;
        public PriceRuleEnginePersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Fetches the Product Sale Rules from the database
        /// </summary>
        public ProductSaleRule GetProductSaleRule()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetProductSaleRules))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        return activityData.GetProductSaleRule(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEnginePersistence",
                    MethodName = "GetProductSaleRule",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the B2B Sale Rules from the database
        /// </summary>
        /// <returns></returns>
        public List<B2BSaleRule> GetB2BSaleRules()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetB2BSaleRules))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        return activityData.GetB2BSaleRules(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEnginePersistence",
                    MethodName = "GetB2BSaleRules",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the B2B Net Rate Rules from the database
        /// </summary>
        /// <returns></returns>
        public List<B2BNetRateRule> GetB2BNetRateRules()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetB2BNetRateRules))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        return activityData.GetB2BNetRateRules(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEnginePersistence",
                    MethodName = "GetB2BNetRateRules",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the Supplier Sale Rules from the database
        /// </summary>
        /// <returns></returns>
        public SupplierSaleRule GetSupplierSaleRule()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetSupplierSaleRules))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        return activityData.GetSupplierSaleRule(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEnginePersistence",
                    MethodName = "GetSupplierSaleRule",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public ProductSaleRule GetProductCostSaleRule()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetProductCostSaleRules))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        return activityData.GetProductSaleRule(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PriceRuleEnginePersistence",
                    MethodName = "GetProductCostSaleRule",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}