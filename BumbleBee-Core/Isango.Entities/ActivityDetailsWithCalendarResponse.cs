using Isango.Entities.Activities;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class ActivityDetailsWithCalendarResponse
    {
        public Activity Activity { get; set; }
        public List<CalendarAvailability> CalendarAvailabilityList { get; set; }
    }
}