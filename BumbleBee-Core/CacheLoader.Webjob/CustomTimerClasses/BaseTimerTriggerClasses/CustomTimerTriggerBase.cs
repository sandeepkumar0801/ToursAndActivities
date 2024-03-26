using Microsoft.Azure;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Renci.SshNet.Security;
using System;
using Util;

namespace CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses
{
    public class CustomTimerTriggerBase : TimerSchedule
    {
        private readonly TimeSpan _timer;

        protected CustomTimerTriggerBase(string triggerConfigKey)
        {
            try
            {
                var value = ConfigurationManagerHelper.GetValuefromAppSettings(triggerConfigKey);
                if (!string.IsNullOrEmpty(value))
                {
                    _timer = TimeSpan.Parse(value);
                }
                else
                {
                    // Default timespan of 1 day
                    _timer = TimeSpan.FromDays(1);
                }
            }
            catch (Exception ex)
            {
                // Default timespan of 1 day
                _timer = TimeSpan.FromDays(1);
                throw;
            }
        }

        public override DateTime GetNextOccurrence(DateTime now)
        {
            return now.Add(_timer);
        }

        public override bool AdjustForDST => true; // Adjust for Daylight Saving Time
    }
}
