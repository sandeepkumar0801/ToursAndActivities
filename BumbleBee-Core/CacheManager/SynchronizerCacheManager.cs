using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Activities;
using System.Collections.Generic;
using Util;

namespace CacheManager
{
    public class SynchronizerCacheManager : ISynchronizerCacheManager
    {
        private readonly CollectionDataFactory<Activity> _collectionData;

        public SynchronizerCacheManager(CollectionDataFactory<Activity> collectionData)
        {
            _collectionData = collectionData;
        }

        public bool DeleteActivityFromCache(int activityId, List<Language> languages)
        {
            var alldeleted = true;
            foreach (var language in languages)
            {
                var collectionName = language.Code.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{language.Code}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
                var result = _collectionData.GetCollectionDataHelper().DeleteDocument(collectionName, activityId.ToString(), activityId);
                result.Wait();
                alldeleted = result.Result;
            }
            return alldeleted;
        }

        public List<int> RemoveFromCache(int[] activityIds, List<Language> languages)
        {
            var actIds = new List<int>();
            foreach (var activityId in activityIds)
            {
                var isDelete = DeleteActivityFromCache(activityId, languages);
                if (isDelete)
                {
                    actIds.Add(activityId);
                }
            }
            return actIds;
        }

        public bool CreateCollectionIfNotExist(string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            return _collectionData.GetCollectionDataHelper().CheckIfCollectionExist(collectionName).Result || _collectionData
                       .GetCollectionDataHelper().CreateCollection(collectionName, Constant.PartitionKeyActivityCollection).Result;
        }
    }
}