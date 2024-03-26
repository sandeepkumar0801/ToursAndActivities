using DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses;

namespace DataDumping.WebJob.CustomTimerClasses
{
    public sealed class CancellationPoliciesTime : CustomDailyTimerTriggerBase
    {
        public CancellationPoliciesTime() : base("CancellationPoliciesTime") { }
    }
}
