using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IHbProductMappingCacheManager
    {
        string LoadCacheMapping(List<IsangoHBProductMapping> isangoHBProductMappingList);
    }
}