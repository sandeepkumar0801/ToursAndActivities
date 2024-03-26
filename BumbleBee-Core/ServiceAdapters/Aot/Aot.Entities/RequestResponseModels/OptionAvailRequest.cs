using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "OptionAvailRequest")]
    public class OptionAvailRequest
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

        [XmlElement(ElementName = "DateFrom")]
        public string DateFrom { get; set; }

        [XmlElement(ElementName = "DateTo")]
        public string DateTo { get; set; }

        [XmlElement(ElementName = "IncludeRates")]
        public string IncludeRates { get; set; }
    }
}