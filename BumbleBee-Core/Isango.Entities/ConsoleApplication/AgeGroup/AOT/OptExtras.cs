using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities.ConsoleApplication.AgeGroup.AOT
{
    [XmlRoot(ElementName = "OptExtra")]
    public class OptExtra
    {
        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "IsPricePerPerson")]
        public string IsPricePerPerson { get; set; }
    }

    [XmlRoot(ElementName = "OptExtras")]
    public class OptExtras
    {
        [XmlElement(ElementName = "OptExtra")]
        public List<OptExtra> OptExtra { get; set; }
    }
}