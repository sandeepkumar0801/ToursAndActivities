using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IActivityCacheManager
    {
        SearchResult SearchActivities(SearchCriteria searchCriteria, ClientInfo clientInfo);

        Activity GetActivity(string activityId, string languageCode = "en");

        bool DeleteAndCreateCollection(string languageCode = "en");

        List<RegionCategoryMapping> GetRegioncategoryMapping();

        CacheKey<RegionCategoryMapping> GetRegioncategoryMappingV2(string key);

        bool InsertActivity(Activity activity, string languageCode = "en");

        List<Activity> GetActivityWithProductOption(string languageCode = "en");

        bool UpdateActivity(Activity activity, string languageCode = "en");

        List<Activity> GetAllActivities(string languageCode = "en");

        List<Activity> GetAllActivitiesWithLineOfBusiness(int? lineOfBusinessId, string languageCode = "en", string regionid = "7243");

        List<ActivityWithApiType> GetActivityWithApiType(string apiTypeId = "", string languageCode = "en");
        bool InsertManyActivity(List<Activity> activity, string languageCode = "en");
    }
}