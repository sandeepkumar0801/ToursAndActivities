using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class ElasticSearchTime : CustomDailyTimerTriggerBase
    {
        public ElasticSearchTime() : base("ElasticSearchTime")
        {
        }
    }
}