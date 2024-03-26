using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class AvailabilityCacheTime : CustomDailyTimerTriggerBase
    {
        public AvailabilityCacheTime() : base("AvailabilityCacheTime")
        {
        }
    }
}