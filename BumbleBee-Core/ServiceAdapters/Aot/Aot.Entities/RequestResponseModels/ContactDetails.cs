using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ContactDetails")]
    public class ContactDetails
    {
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Forename")]
        public string Forename { get; set; }

        [XmlElement(ElementName = "Surname")]
        public string Surname { get; set; }

        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "Phone")]
        public string Phone { get; set; }

        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "City")]
        public string City { get; set; }

        [XmlElement(ElementName = "State")]
        public string State { get; set; }

        [XmlElement(ElementName = "Postcode")]
        public string Postcode { get; set; }

        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }
    }
}