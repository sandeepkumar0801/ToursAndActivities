using Microsoft.Azure.WebJobs.Extensions.Timers;
using Util;

namespace CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses
{
    public class CustomDailyTimerTriggerBase : DailySchedule
    {
        public CustomDailyTimerTriggerBase(string triggerConfigKey) : base(GetValuesFromConfig(triggerConfigKey))
        {
        }

        private static string[] GetValuesFromConfig(string key)
        {
            var value = ConfigurationManagerHelper.GetValuefromAppSettings(key);
            return value.Split(',');
        }
    }
}