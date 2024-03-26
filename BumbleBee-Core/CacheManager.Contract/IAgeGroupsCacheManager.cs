using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IAgeGroupsCacheManager
    {
        string LoadAgeGroupByActivity(List<AgeGroup> ageGroupByActivityList);

        string LoadFhAgeGroupByActivity(List<FareHarborAgeGroup> ageGroupByActivityList);

        List<AgeGroup> GetAgeGroup(int apiType, int activityID);

        List<FareHarborAgeGroup> GetFareHarborAgeGroup(int apiType, int activityID);
    }
}