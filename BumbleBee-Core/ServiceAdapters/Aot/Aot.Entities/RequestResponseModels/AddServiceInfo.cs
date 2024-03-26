using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "AddServiceInfo")]
    public class AddServiceInfo
    {
        [XmlElement(ElementName = "Opt")]
        public string Opt { get; set; }

        [XmlElement(ElementName = "DateFrom")]
        public string DateFrom { get; set; }

        [XmlElement(ElementName = "RoomConfigs")]
        public RoomConfigs RoomConfigs { get; set; }

        [XmlElement(ElementName = "SCUqty")]
        public string ScUqty { get; set; }

        [XmlElement(ElementName = "Comments")]
        public string Comments { get; set; }

        [XmlElement(ElementName = "ExtraQuantities")]
        public ExtraQuantities ExtraQuantities { get; set; }

        [XmlElement(ElementName = "puTime")]
        public string PuTime { get; set; }

        [XmlElement(ElementName = "puRemark")]
        public string PuRemark { get; set; }

        [XmlElement(ElementName = "doTime")]
        public string DoTime { get; set; }

        [XmlElement(ElementName = "doRemark")]
        public string DoRemark { get; set; }
    }
}