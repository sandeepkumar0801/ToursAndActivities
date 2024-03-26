using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public sealed class InitiateGoogleFeeds : CustomDailyTimerTriggerBase
    {
        public InitiateGoogleFeeds() : base("InitiateGoogleFeeds") { }
    }
}