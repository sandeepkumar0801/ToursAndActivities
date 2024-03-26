using Isango.Entities;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IFareHarborCustomerPrototypesCacheManager
    {
        bool SetCustomerPrototypeByActivityToCache(CacheKey<CustomerPrototype> cacheResult);

        List<CustomerPrototype> GetCustomerPrototypeList();
    }
}