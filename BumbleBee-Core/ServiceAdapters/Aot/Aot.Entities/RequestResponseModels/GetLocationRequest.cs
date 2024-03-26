using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "GetLocationsRequest")]
    public class GetLocationsRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "LocationType")]
        public string LocationType { get; set; }

        [XmlElement(ElementName = "LocationCode")]
        public string LocationCode { get; set; }
    }
}