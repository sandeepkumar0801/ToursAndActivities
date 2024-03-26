using Microsoft.Azure.WebJobs.Extensions.Timers;
using Util;
using NCrontab;

namespace DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses
{
    public class CustomCronTimerTriggerBase : CronSchedule
    {
        public CustomCronTimerTriggerBase(string cronTabExpression) : base(ParseCronExpression(cronTabExpression))
        {
        }

        private static CrontabSchedule ParseCronExpression(string cronExpression)
        {
            var schedule = CrontabSchedule.Parse(cronExpression);
            return schedule;
        }
    }
}
