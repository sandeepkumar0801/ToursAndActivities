using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface ISynchronizerCacheManager
    {
        bool DeleteActivityFromCache(int activityId, List<Language> languages);

        List<int> RemoveFromCache(int[] activityIds, List<Language> languages);

        bool CreateCollectionIfNotExist(string languageCode = "en");
    }
}