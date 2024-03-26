using ApplicationCacheManager.Contract;
using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace ApplicationCacheManager
{
    public class B2BNetRateRuleCache : IB2BNetRateRuleCache
    {
        private readonly ILogger _log;
        private readonly IPriceRuleEnginePersistence _priceRuleEnginePersistence;

        public B2BNetRateRuleCache(
            ILogger log, 
            IPriceRuleEnginePersistence priceRuleEnginePersistence
            )
        {
           
            _log = log;
            _priceRuleEnginePersistence = priceRuleEnginePersistence;
        }

        public B2BNetRateRule GetB2BNetRateRule(string affiliateId)
        {
            var key = $"GetB2BNetRateRule_{affiliateId}";
            var result = default(B2BNetRateRule);

            try
            {
                if (!CacheHelper.Exists(key) || !CacheHelper.Get<B2BNetRateRule>(key, out result))
                {
                    var b2bNetRateRules = GetB2BNetRateRules();
                    result = b2bNetRateRules.FirstOrDefault(x =>
                                        x.AffiliateId?.ToLower() == affiliateId?.ToLower() &&
                                        x.BookingFromDate.ToUniversalTime() <= DateTime.UtcNow &&
                                        x.BookingToDate.ToUniversalTime() >= DateTime.UtcNow);

                    if (result != null)
                    {
                        
                            try
                            {
                                CacheHelper.Set<B2BNetRateRule>(key, result);
                            }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                                 _log.Error(new IsangoErrorEntity
                                 {
                                     ClassName = nameof(B2BNetRateRuleCache),
                                     MethodName = nameof(GetB2BNetRateRule),
                                     Params = "Unable to save data B2BNetRateRuleCache using GetB2BNetRateRules"
                                 }, ex)
                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                     _log.Error(new IsangoErrorEntity
                     {
                         ClassName = nameof(B2BNetRateRuleCache),
                         MethodName = nameof(GetB2BNetRateRules)
                      }, ex)
                     );
            }
            return result;
        }

        /// <summary>
        /// Load all rules from db into application cache which will refresh in 2 hours.
        /// </summary>
        /// <returns></returns>
        private List<B2BNetRateRule> GetB2BNetRateRules()
        {
            var key = $"GetB2BNetRateRule_All_AffiliateIds";
            var result = default(List<B2BNetRateRule>);

            try
            {
                if (!CacheHelper.Exists(key) || !CacheHelper.Get<List<B2BNetRateRule>>(key, out result))
                {
                    result = _priceRuleEnginePersistence.GetB2BNetRateRules();

                    if (result != null)
                    {
                        
                            try
                            {
                                CacheHelper.Set<List<B2BNetRateRule>>(key, result, 120);
                            }

                        catch (Exception ex)
                        {
                            Task.Run(() =>
                                 _log.Error(new IsangoErrorEntity
                                 {
                                     ClassName = nameof(B2BNetRateRuleCache),
                                     MethodName = nameof(GetB2BNetRateRule),
                                     Params = "Unable to save data B2BNetRateRuleCache using GetB2BNetRateRules"
                                 }, ex)
                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                     _log.Error(new IsangoErrorEntity
                     {
                         ClassName = nameof(B2BNetRateRuleCache),
                         MethodName = nameof(GetB2BNetRateRules)
                     }, ex)
                     );
            }
            return result;
        }
    }
}
