using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class MappedLanguageTime : CustomDailyTimerTriggerBase
    {
        public MappedLanguageTime() : base("MappedLanguageTime")
        {
        }
    }
}