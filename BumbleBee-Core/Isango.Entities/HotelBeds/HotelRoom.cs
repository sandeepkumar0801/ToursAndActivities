using System;

namespace Isango.Entities.HotelBeds
{
    public class HotelRoom
    {
        public string SHRUI { get; set; }
        public int AvailableCount { get; set; }
        public string Status { get; set; } // ?? ENUM
        public string BoardCode { get; set; }
        public string BoardShortName { get; set; }
        public string BoardName { get; set; } // ?? ENUM
        public string RoomTypeCode { get; set; }
        public string Characteristic { get; set; }
        public string RoomType { get; set; }
        public decimal CancellationAmount { get; set; }
        public DateTime CancellationFromdate { get; set; }
        public DateTime CancellationToDate { get; set; }
    }
}