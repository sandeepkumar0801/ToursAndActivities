using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public sealed class AgeDumpingTime : CustomDailyTimerTriggerBase
    {
        public AgeDumpingTime() : base("AgeDumpingTime") { }
    }
}
