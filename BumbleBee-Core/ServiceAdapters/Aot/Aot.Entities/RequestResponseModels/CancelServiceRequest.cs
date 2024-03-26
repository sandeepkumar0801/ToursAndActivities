using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "CancelServiceRequest")]
    public class CancelServiceRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentID { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "ServiceLineId")]
        public string ServiceLineId { get; set; }
    }
}