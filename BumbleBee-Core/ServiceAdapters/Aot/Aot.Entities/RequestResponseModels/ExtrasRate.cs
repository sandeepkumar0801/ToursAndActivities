using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ExtrasRate")]
    public class ExtrasRate
    {
        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "AdultRate")]
        public string AdultRate { get; set; }

        [XmlElement(ElementName = "ChildRate")]
        public string ChildRate { get; set; }

        [XmlElement(ElementName = "ExRate")]
        public string ExRate { get; set; }
    }
}