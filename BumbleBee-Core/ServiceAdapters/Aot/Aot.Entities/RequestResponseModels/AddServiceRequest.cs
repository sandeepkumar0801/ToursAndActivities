using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "AddServiceRequest")]
    public class AddServiceRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "AddServiceInfo")]
        public AddServiceInfo AddServiceInfo { get; set; }
    }
}