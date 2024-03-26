using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingRequest
{
    [XmlRoot(ElementName = "booking")]
    public class DeleteBookingRequest
    {
        [XmlElement(ElementName = "booking_id")]
        public int BookingId { get; set; }

        
    }
}