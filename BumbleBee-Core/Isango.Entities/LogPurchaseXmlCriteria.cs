using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class LogPurchaseXmlCriteria
    {
        public string RequestXml { get; set; }
        public string ResponseXml { get; set; }
        public string Status { get; set; }
        public int BookingId { get; set; }
        public int SupplierId { get; set; }
        public string ApiRefNumber { get; set; }
        public string Bookingtype { get; set; } = "booking";
        public APIType APIType { get; set; } = APIType.Undefined;
        public string BookingReferenceNumber { get; set; }
    }
}