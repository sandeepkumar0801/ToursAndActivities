using CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace CacheLoader.Webjob.CustomTimerClasses
{
    public class LoadPickupDetailsTime : CustomWeeklyTimerTriggerBase
    {
        public LoadPickupDetailsTime() : base("LoadPickupDetailsTime")
        {
        }
    }
}