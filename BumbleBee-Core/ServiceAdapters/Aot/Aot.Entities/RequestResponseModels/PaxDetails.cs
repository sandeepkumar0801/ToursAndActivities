using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "PaxDetails")]
    public class PaxDetails
    {
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Forename")]
        public string Forename { get; set; }

        [XmlElement(ElementName = "Surname")]
        public string Surname { get; set; }

        [XmlElement(ElementName = "PaxType")]
        public string PaxType { get; set; }

        [XmlElement(ElementName = "DateOfBirth")]
        public string DateOfBirth { get; set; }

        [XmlElement(ElementName = "Age")]
        public int Age { get; set; }
    }
}