using System;

namespace Isango.Entities.Booking
{
    public class WireCardXmlCriteria
    {
        public string JobId { get; set; }
        public int TransId { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string RequestType { get; set; }
        public string TransGuWId { get; set; }
    }
}