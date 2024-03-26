using ApplicationCacheManager.Contract;
using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Service.Contract;
using Logger.Contract;

namespace Isango.Service
{
    public class ApplicationService : IApplicationService

    {
        #region Variables
        private readonly IB2BNetRateRuleCache _b2BNetRateCacheManager;
        private readonly ILogger _log;
        #endregion Variables

        #region Constructor

        public ApplicationService(IB2BNetRateRuleCache b2BNetRateCacheManager, ILogger log)
        {

            _b2BNetRateCacheManager = b2BNetRateCacheManager;
            _log = log;
           
        }

        #endregion Constructor

        public async Task<B2BNetRateRule> GetB2BNetRateRuleAsync(string affiliateId)
        {
            try
            {
                var b2BNetRateRule = _b2BNetRateCacheManager.GetB2BNetRateRule(affiliateId);

                return await Task.FromResult(b2BNetRateRule);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApplicationService",
                    MethodName = "GetB2BNetRateRuleAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}