using Isango.Entities;
using Isango.Entities.Wrapper;

namespace CacheManager.Contract
{
    public interface IFareHarborCustomerTypesCacheManager
    {
        bool SetFareHarborAgeGroupsByActivityToCache(CacheKey<AgeGroup> cacheResult);
    }
}