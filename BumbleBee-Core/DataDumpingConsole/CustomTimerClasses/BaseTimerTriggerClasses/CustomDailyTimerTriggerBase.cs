using Microsoft.Azure.WebJobs.Extensions.Timers;
using Util;

namespace DataDumping.WebJob.CustomTimerClasses.BaseTimerTriggerClasses
{
    public class CustomDailyTimerTriggerBase : DailySchedule
    {
        protected CustomDailyTimerTriggerBase(string triggerConfigKey) : base(GetValuesFromConfig(triggerConfigKey))
        {

        }

        private static string[] GetValuesFromConfig(string key)
        {
            var value = ConfigurationManagerHelper.GetValuefromAppSettings(key);
            return value.Split(',');
        }
    }
}