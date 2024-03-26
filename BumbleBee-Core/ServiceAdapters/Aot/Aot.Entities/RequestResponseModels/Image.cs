using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "ServerPath")]
        public string ServerPath { get; set; }
    }
}