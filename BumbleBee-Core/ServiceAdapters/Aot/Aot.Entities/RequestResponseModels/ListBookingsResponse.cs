using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "BookingHeader")]
    public class BookingHeader
    {
        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Consult")]
        public string Consult { get; set; }

        [XmlElement(ElementName = "AgentRef")]
        public string AgentRef { get; set; }

        [XmlElement(ElementName = "TravelDate")]
        public string TravelDate { get; set; }

        [XmlElement(ElementName = "EnteredDate")]
        public string EnteredDate { get; set; }

        [XmlElement(ElementName = "BookingStatus")]
        public string BookingStatus { get; set; }

        [XmlElement(ElementName = "TotalPrice")]
        public string TotalPrice { get; set; }
    }

    [XmlRoot(ElementName = "BookingHeaders")]
    public class BookingHeaders
    {
        [XmlElement(ElementName = "BookingHeader")]
        public List<BookingHeader> BookingHeader { get; set; }
    }

    [XmlRoot(ElementName = "ListBookingsResponse")]
    public class ListBookingsResponse
    {
        [XmlElement(ElementName = "BookingHeaders")]
        public BookingHeaders BookingHeaders { get; set; }
    }
}