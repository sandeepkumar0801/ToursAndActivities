using Isango.Entities;
using Isango.Entities.Wrapper;

namespace CacheManager.Contract
{
    public interface ILandingCacheManager
    {
        CacheKey<LocalizedMerchandising> GetLoadLocalizedMerchandising(string key);

        bool SetLoadLocalizedMerchandising(CacheKey<LocalizedMerchandising> localizedMerchandising);
    }
}