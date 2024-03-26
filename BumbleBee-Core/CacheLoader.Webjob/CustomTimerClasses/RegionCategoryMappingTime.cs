using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class RegionCategoryMappingTime : CustomDailyTimerTriggerBase
    {
        public RegionCategoryMappingTime() : base("RegionCategoryMapping")
        {
        }
    }
}