using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public class DeleteExistingAvailabilityTime : CustomDailyTimerTriggerBase
    {
        public DeleteExistingAvailabilityTime() : base("DeleteExistingAvailabilityTime") { }
    }
}