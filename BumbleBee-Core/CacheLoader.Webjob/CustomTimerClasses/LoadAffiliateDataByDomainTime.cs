using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class LoadAffiliateDataByDomainTime : CustomDailyTimerTriggerBase
    {
        public LoadAffiliateDataByDomainTime() : base("LoadAffiliateDataByDomainTime")
        {
        }
    }
}