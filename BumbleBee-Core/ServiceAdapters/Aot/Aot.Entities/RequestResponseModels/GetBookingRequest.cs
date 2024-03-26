using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "GetBookingRequest")]
    public class GetBookingRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }
    }
}