using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "OptionStayPricingRequest")]
    public class OptionStayPricingRequest
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

        [XmlElement(ElementName = "RoomConfigs")]
        public RoomConfigs RoomConfigs { get; set; }

        [XmlElement(ElementName = "ReturnOptionInfo")]
        public string ReturnOptionInfo { get; set; }

        [XmlElement(ElementName = "ReturnExtraRate")]
        public string ReturnExtraRate { get; set; }

        [XmlElement(ElementName = "MachineCancelPolicies")]
        public int MachineCancelPolicies { get; set; }

        [XmlElement(ElementName = "SCUqty")]
        public string ScUqty { get; set; }

        [XmlElement(ElementName = "ReturnCancelPolicy")]
        public string ReturnCancelPolicy { get; set; }
    }
}