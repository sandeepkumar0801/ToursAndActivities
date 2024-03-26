using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class CacheMappingTime : CustomTimerTriggerBase
    {
        public CacheMappingTime() : base("CacheMappingTime")
        {
        }
    }
}