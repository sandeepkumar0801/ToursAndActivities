using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public sealed class InitiateOrderNotificationRealTimeUpdate : CustomCronTimerTriggerBase
    {
        public InitiateOrderNotificationRealTimeUpdate() : base("InitiateOrderNotificationRealTimeUpdate") { }
    }
}
