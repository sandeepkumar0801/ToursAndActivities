using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "OptionGeneralInfoRequest")]
    public class OptionGeneralInfoRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "Opts")]
        public Opts Opts { get; set; }

        [XmlElement(ElementName = "LocationType")]
        public string LocationType { get; set; }

        [XmlElement(ElementName = "LocationCode")]
        public string LocationCode { get; set; }

        [XmlElement(ElementName = "Amenities")]
        public string Amenities { get; set; }

        [XmlElement(ElementName = "MoreInformationURL")]
        public string MoreInformationUrl { get; set; }

        [XmlElement(ElementName = "StarRating")]
        public string StarRating { get; set; }

        [XmlElement(ElementName = "ReturnTourInfo")]
        public string ReturnTourInfo { get; set; }
    }
}