using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using Util;
using Constant = Isango.Service.Constants.Constant;

namespace Isango.Service
{
    public class AffiliateService : IAffiliateService
    {
        private readonly IAffiliatePersistence _affiliatePersistence;
        private readonly IAffiliateCacheManager _affiliateCache;
        private readonly ILogger _log;

        public AffiliateService(IAffiliatePersistence affiliatePersistence, IAffiliateCacheManager affiliateCache, ILogger log)
        {
            _affiliatePersistence = affiliatePersistence;
            _affiliateCache = affiliateCache;
            _log = log;
        }

        /// <summary>
        /// This method gives affiliate info.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="alias"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<Affiliate> GetAffiliateInfoAsync(string domain, string alias, string affiliateId)
        {
            try
            {
                var affiliateCacheKey = string.IsNullOrWhiteSpace(domain) ? alias : domain;
                if (string.IsNullOrWhiteSpace(affiliateCacheKey)) affiliateCacheKey = affiliateId;

                //Get from cosmos db
                var affiliateCacheResult = _affiliateCache.GetAffiliateInfo(affiliateCacheKey) ?? _affiliatePersistence.GetAffiliateInfo(domain, alias, affiliateId);

                return await Task.FromResult(affiliateCacheResult);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateInfo",
                    AffiliateId = affiliateId,
                    Params = $"{domain},{alias},{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        //public async Task<Affiliate> GetAffiliateInfoAsync(string affiliateId)
        //{
        // try
        //{
        //Get from cosmos db
        // var affiliateCacheResult = _affiliateCache.GetAffiliateInfo(affiliateId) ?? _affiliatePersistence.GetAffiliateInfo(domain, alias, affiliateId);

        // return await Task.FromResult(affiliateCacheResult);
        // }
        // catch (Exception ex)
        // {
        // var isangoErrorEntity = new IsangoErrorEntity
        // {
        //  ClassName = "AffiliateService",
        //  MethodName = "GetAffiliateInfo",
        //  AffiliateId = affiliateId,
        //  Params = $"{affiliateId}"
        // };
        //_log.Error(isangoErrorEntity, ex);
        //  throw;
        //}
        // }

        /// <summary>
        /// This method gives list of affiliate filters
        /// </summary>
        /// <returns></returns>
        public async Task<List<AffiliateFilter>> GetAffiliateFiltersAsync()
        {
            try
            {
                var affiliateCacheKey = Constant.AffiliateFilterCacheKey;
                //get data from cosmos DB.
                var affiliateFilterResult = _affiliateCache.GetAffiliateFilter(affiliateCacheKey);

                if (affiliateFilterResult == null || affiliateFilterResult.CacheValue?.Count == 0)
                {
                    var result = _affiliatePersistence.GetAffiliateFilter();

                    affiliateFilterResult = new CacheKey<AffiliateFilter>()
                    {
                        Id = Constant.AffiliateFilterCacheKey,
                        CacheValue = result
                    };

                    if (affiliateFilterResult.CacheValue != null)
                    {
                        _affiliateCache.SetAffiliateFilterToCache(affiliateFilterResult);
                    }
                }
                return await Task.FromResult(affiliateFilterResult.CacheValue);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateFilters"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method gives affiliate filter by using affiliate id
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<AffiliateFilter> GetAffiliateFilterByIdAsync(string affiliateId)
        {
            try
            {
                if (string.IsNullOrEmpty(affiliateId))
                    return null;
                var affiliateFilter = _affiliateCache.GetAffiliateFilterCache(affiliateId.ToLowerInvariant());
                var affiliate = affiliateFilter ??
                                _affiliatePersistence.GetAffiliateFilter().FirstOrDefault(aff => aff.AffiliateId == affiliateId);
                return await Task.FromResult(affiliate);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateFilterById",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
            }
        }

        /// <summary>
        /// Get affiliate info of widget partner
        /// </summary>
        /// <returns></returns>
        public List<Partner> GetWidgetPartnersAsync()
        {
            try
            {
                return _affiliatePersistence.GetWidgetPartners();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetWidgetPartners"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Affiliate Information
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<Affiliate> GetAffiliateInformationAsync(string affiliateId)
        {
            var result = default(Affiliate);
            try
            {
                var key = $"AffiliateInformation_{affiliateId}";
                if (!CacheHelper.Exists(key) || !CacheHelper.Get(key, out result))
                {
                    result = _affiliatePersistence.GetAffiliateInformation(affiliateId, "en");

                    if (result != null)
                    {
                        CacheHelper.Set(key, result);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateInformation",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                }, ex);
                throw;
            }
            return await Task.FromResult(result);
        }

        public async Task<Affiliate> GetAffiliateInformationV2Async(string affiliateId)
        {
            try
            {
                var affiliateData = _affiliateCache.GetAffiliateInfoV2(affiliateId.ToLowerInvariant());
                var affiliate = affiliateData ??
                                  _affiliatePersistence.GetAffiliateInformation(affiliateId, "en");
                return await Task.FromResult(affiliate);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateInformation",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Affiliate Release Tags
        /// </summary>
        /// <returns></returns>
        public async Task<List<AffiliateReleaseTag>> GetAffiliateReleaseTagsAsync()
        {
            try
            {
                return await Task.FromResult(_affiliatePersistence.GetAffiliateReleaseTags());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateReleaseTags"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Update Affiliate Release Tags
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="releaseTag"></param>
        /// <param name="isForAll"></param>
        /// <returns></returns>
        public async Task<string> UpdateAffiliateReleaseTagsAsync(string affiliateId, string releaseTag, bool isForAll = false)
        {
            try
            {
                return await Task.FromResult(_affiliatePersistence.UpdateAffiliateReleaseTags(affiliateId, releaseTag, isForAll));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "UpdateAffiliateReleaseTags",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId},{releaseTag},{isForAll}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get White Label Affiliates Information
        /// </summary>
        /// <returns></returns>
        public async Task<List<Affiliate>> GetWLAffiliateInfoAsync()
        {
            try
            {
                return await Task.FromResult(_affiliatePersistence.GetWLAffiliateInfo());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetWLAffiliateInfo"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Affiliate Id through User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetAffiliateIdByUserIdAsync(string userId)
        {
            try
            {
                return await Task.FromResult(_affiliatePersistence.GetAffiliateIdByUserId(userId));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateIDbyUserId"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Affiliate Information V2 Async
        /// </summary>
        /// <returns></returns>
        public async Task<List<AffiliateWiseServiceMinPrice>> GetAffiliateInformationAsync()
        {
            try
            {
                var affiliatePrice = _affiliatePersistence.GetAffiliateWiseServiceMinPrice();
                return await Task.FromResult(affiliatePrice);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliateService",
                    MethodName = "GetAffiliateInformation"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}