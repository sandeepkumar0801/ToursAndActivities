using System;

namespace Isango.Entities.ConsoleApplication.ServiceAvailability
{
    public class ErrorLogger
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Destination { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string FactSheetIds { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}