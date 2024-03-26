using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.CommitBooking
{
    [XmlRoot(ElementName = "booking")]
    public class CommitBookingRequest
    {
        [XmlElement(ElementName = "booking_id")]
        public int BookingId { get; set; }
    }
}