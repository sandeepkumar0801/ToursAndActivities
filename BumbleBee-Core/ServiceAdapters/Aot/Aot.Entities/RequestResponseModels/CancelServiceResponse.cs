using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "CancelServiceResponse")]
    public class CancelServiceResponse
    {
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
    }
}