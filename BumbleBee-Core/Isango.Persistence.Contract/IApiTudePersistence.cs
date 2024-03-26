using Isango.Entities.HotelBeds;
using ServiceAdapters.HB.HB.Entities.ContentMulti;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IApiTudePersistence
    {
        void SaveApiTudeAgeGroups(List<ApiTudeAgeGroup> ageGroups);
        
        void SaveApiTudeContent(List<ActivitiesContent> activitiesContent);

        void SaveApiTudeContentCalendar(List<ServiceAdapters.HB.HB.Entities.Calendar.Activity> content);
    }
}