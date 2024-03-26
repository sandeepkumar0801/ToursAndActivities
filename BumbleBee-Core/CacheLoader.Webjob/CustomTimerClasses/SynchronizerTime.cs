using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public sealed class SynchronizerTime : CustomTimerTriggerBase
    {
        public SynchronizerTime() : base("SynchronizerTime")
        {
        }
    }
}