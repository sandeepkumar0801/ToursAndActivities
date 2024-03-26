using Isango.Entities;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IFareHarborUserKeysCacheManager
    {
        bool SetAllFareHarborUserKeysToCache(CacheKey<FareHarborUserKey> cacheResult);

        List<FareHarborUserKey> GetFareHarborUserKeys();
    }
}