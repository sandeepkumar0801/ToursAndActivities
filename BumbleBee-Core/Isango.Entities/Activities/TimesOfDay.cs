using System;

namespace Isango.Entities.Activities
{
    public class TimesOfDay
    {
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        public string DurationDay { get; set; }
        public string TotalDuration { get; set; }
        public DateTime AppliedFromDate { get; set; }
        public DateTime AppliedToDate { get; set; }
    }
}