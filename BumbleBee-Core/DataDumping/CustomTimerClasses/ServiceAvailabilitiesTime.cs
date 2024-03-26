using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public sealed class ServiceAvailabilitiesTime : CustomDailyTimerTriggerBase
    {
        public ServiceAvailabilitiesTime() : base("ServiceAvailabilitiesTime") { }
    }
}