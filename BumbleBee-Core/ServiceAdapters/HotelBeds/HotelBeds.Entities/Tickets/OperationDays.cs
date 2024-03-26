using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class OperationDays : EntityBase
    {
        public List<Day> DayList { get; set; }
    }
}