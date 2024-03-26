using Microsoft.Azure.WebJobs.Extensions.Timers;
using System;
using Util;

namespace CacheLoader.Webjob.CustomTimerClasses.BaseTimerTriggerClasses
{
    public class CustomWeeklyTimerTriggerBase : WeeklySchedule
    {
        protected CustomWeeklyTimerTriggerBase(string key)
        {
            TimeSpan timeSpan;
            var value = ConfigurationManagerHelper.GetValuefromAppSettings(key);
            var day = value.Split(',')[0];
            var time = value.Split(',')[1];
            switch (day)
            {
                //Calling the Add(day, time) method of WeeklySchedule class to add the weekday and time on that weekday to run the webjob.
                case "Mon":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Monday, timeSpan);
                    break;

                case "Tue":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Tuesday, timeSpan);
                    break;

                case "Wed":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Wednesday, timeSpan);
                    break;

                case "Thu":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Thursday, timeSpan);
                    break;

                case "Fri":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Friday, timeSpan);
                    break;

                case "Sat":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Saturday, timeSpan);
                    break;

                case "Sun":
                    timeSpan = DateTime.Parse(time).TimeOfDay;
                    Add(DayOfWeek.Sunday, timeSpan);
                    break;
            }
        }
    }
}