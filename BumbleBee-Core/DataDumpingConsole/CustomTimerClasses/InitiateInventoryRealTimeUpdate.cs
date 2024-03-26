using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public sealed class InitiateInventoryRealTimeUpdate : CustomDailyTimerTriggerBase
    {
        public InitiateInventoryRealTimeUpdate() : base("InitiateInventoryRealTimeUpdate") { }
    }
}