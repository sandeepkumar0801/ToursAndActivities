using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class RegionDestinationMappingTime : CustomWeeklyTimerTriggerBase
    {
        public RegionDestinationMappingTime() : base("RegionDestinationMappingTime")
        {
        }
    }
}