using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ExtraQuantityItem")]
    public class ExtraQuantityItem
    {
        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "ExtraQuantity")]
        public string ExtraQuantity { get; set; }
    }
}