using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities.ConsoleApplication.AgeGroup.AOT
{
    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "ServerPath")]
        public string ServerPath { get; set; }
    }

    [XmlRoot(ElementName = "images")]
    public class Images
    {
        [XmlElement(ElementName = "image")]
        public List<Image> Image { get; set; }
    }
}