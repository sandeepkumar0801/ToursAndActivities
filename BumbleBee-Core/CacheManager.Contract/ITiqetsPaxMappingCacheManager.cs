using Isango.Entities.Tiqets;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface ITiqetsPaxMappingCacheManager
    {
        bool SetPaxMappingToCache(CacheKey<TiqetsPaxMapping> cacheResult);

        List<TiqetsPaxMapping> GetPaxMappings();
    }
}