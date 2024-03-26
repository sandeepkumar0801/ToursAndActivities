using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ListBookingsRequest")]
    public class ListBookingsRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "TravelDateFrom")]
        public string TravelDateFrom { get; set; }

        [XmlElement(ElementName = "TravelDateTo")]
        public string TravelDateTo { get; set; }

        [XmlElement(ElementName = "EnteredDateFrom")]
        public string EnteredDateFrom { get; set; }

        [XmlElement(ElementName = "EnteredDateTo")]
        public string EnteredDateTo { get; set; }
    }
}