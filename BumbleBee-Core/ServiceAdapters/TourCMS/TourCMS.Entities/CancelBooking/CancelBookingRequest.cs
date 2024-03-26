using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.CancelBookingRequest
{
    [XmlRoot(ElementName = "booking")]
    public class CancelBookingRequest
    {
        [XmlElement(ElementName = "booking_id")]
        public int BookingId { get; set; }

        [XmlElement(ElementName = "note")]
        public string Note { get; set; }
    }
}