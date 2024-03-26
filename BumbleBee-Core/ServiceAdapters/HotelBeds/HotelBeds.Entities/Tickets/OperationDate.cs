using System;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class OperationDate : EntityBase
    {
        public DateTime Date { get; set; }
        public int MinimumDuration { get; set; }
        public int MaximumDuration { get; set; }
    }
}