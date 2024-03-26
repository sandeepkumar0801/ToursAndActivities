using System.Data.Common;
using System.Xml;

namespace Isango.Entities.GrayLineIceLand
{
    public class GraylineIceLandBookingCriteria
    {
        public Booking.Booking Booking { get; set; }
        public DbTransaction Transaction { get; set; }
        public Payment.Payment PreauthPayment { get; set; }
        public Payment.Payment PurchasePayment { get; set; }
        public DbConnection Connection { get; set; }
        public XmlDocument BookingXml { get; set; }
        public string Authentication { get; set; }
    }
}