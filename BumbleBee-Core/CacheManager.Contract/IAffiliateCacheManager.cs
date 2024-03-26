using Isango.Entities.Affiliate;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IAffiliateCacheManager
    {
        Affiliate GetAffiliateInfo(string affiliateCacheKey);

        bool SetAffiliateInfoToCache(Affiliate affiliateCacheResult);

        CacheKey<AffiliateFilter> GetAffiliateFilter(string affiliateCacheKey);

        bool SetAffiliateFilterToCache(CacheKey<AffiliateFilter> affiliateCacheResult);

        bool DeleteAndCreateCollection();

        bool SetAffiliateToCache(List<AffiliateFilter> affiliateCacheResult);

        AffiliateFilter GetAffiliateFilterCache(string affiliateId);

        bool DeleteAndCreateAffiliateFilterCollection();

        Affiliate GetAffiliateInfoV2(string affiliateCacheKey);
    }
}