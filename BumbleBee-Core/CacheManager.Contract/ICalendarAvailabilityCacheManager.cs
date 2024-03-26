using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface ICalendarAvailabilityCacheManager
    {
        bool LoadCalendarAvailability();

        List<CalendarAvailability> GetCalendarAvailability(int productId, string affiliateId);

        bool InsertCalendarDocuments(CalendarAvailability calendarAvailability);

        List<CalendarAvailability> GetOldCalendarAvailability(string timestamp);

        bool DeleteOldCalendarActivityFromCache(List<CalendarAvailability> calendarAvailabilities);

        bool DeleteManyCalendarDocuments();

        bool InsertManyCalendarDocuments(List<CalendarAvailability> calendarAvailability);
    }
}