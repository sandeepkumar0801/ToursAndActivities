using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.CancelBookingResponse
{
    [XmlRoot(ElementName = "response")]
    public class CancelBookingResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }
    }
}