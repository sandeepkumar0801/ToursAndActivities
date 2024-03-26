using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IAttractionActivityMappingCacheManager
    {
        string LoadCachedAttractionActivity(List<AttractionActivityMapping> attractionActivityList);

        List<AttractionActivityMapping> GetAttractionActivityList(int attractionId);

        string InsertDocuments(List<AttractionActivityMapping> attractionActivityList);
    }
}