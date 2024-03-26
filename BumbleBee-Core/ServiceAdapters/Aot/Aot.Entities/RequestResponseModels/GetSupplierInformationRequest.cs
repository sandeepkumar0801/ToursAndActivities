using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "SupplierCodes")]
    public class SupplierCodes
    {
        [XmlElement(ElementName = "SupplierCode")]
        public string SupplierCode { get; set; }
    }

    [XmlRoot(ElementName = "SupplierInfoRequest")]
    public class SupplierInfoRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentID { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "ServiceType")]
        public string ServiceType { get; set; }

        [XmlElement(ElementName = "SupplierCodes")]
        public SupplierCodes SupplierCodes { get; set; }

        [XmlElement(ElementName = "LocationType")]
        public string LocationType { get; set; }

        [XmlElement(ElementName = "LocationCode")]
        public string LocationCode { get; set; }
    }
}