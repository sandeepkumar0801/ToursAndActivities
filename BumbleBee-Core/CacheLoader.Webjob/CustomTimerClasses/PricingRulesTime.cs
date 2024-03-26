using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class PricingRulesTime : CustomDailyTimerTriggerBase
    {
        public PricingRulesTime() : base("PricingRulesTime")
        {
        }
    }
}