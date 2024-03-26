using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "OptionInfo")]
    public class OptionInfo
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Comment")]
        public string Comment { get; set; }

        [XmlElement(ElementName = "BeddingConfiguration")]
        public string BeddingConfiguration { get; set; }

        [XmlElement(ElementName = "InfantAgeFrom")]
        public string InfantAgeFrom { get; set; }

        [XmlElement(ElementName = "InfantAgeTo")]
        public string InfantAgeTo { get; set; }

        [XmlElement(ElementName = "ChildAgeFrom")]
        public string ChildAgeFrom { get; set; }

        [XmlElement(ElementName = "ChildAgeTo")]
        public string ChildAgeTo { get; set; }

        [XmlElement(ElementName = "AdultAgeFrom")]
        public string AdultAgeFrom { get; set; }

        [XmlElement(ElementName = "AdultAgeTo")]
        public string AdultAgeTo { get; set; }

        [XmlElement(ElementName = "MaxAdults")]
        public string MaxAdults { get; set; }

        [XmlElement(ElementName = "MaxPax")]
        public string MaxPax { get; set; }

        [XmlElement(ElementName = "Periods")]
        public string Periods { get; set; }

        [XmlElement(ElementName = "SType")]
        public string SType { get; set; }

        [XmlElement(ElementName = "MPFCU")]
        public string Mpfcu { get; set; }

        [XmlElement(ElementName = "SCU")]
        public string Scu { get; set; }

        [XmlElement(ElementName = "MinSCU")]
        public string MinScu { get; set; }

        [XmlElement(ElementName = "MaxSCU")]
        public string MaxScu { get; set; }

        [XmlElement(ElementName = "OptExtras")]
        public OptExtras OptExtras { get; set; }

        [XmlElement(ElementName = "Inclusions")]
        public string Inclusions { get; set; }

        [XmlElement(ElementName = "OptionImportantInfo")]
        public string OptionImportantInfo { get; set; }
    }
}