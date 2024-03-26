using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "RoomConfig")]
    public class RoomConfig
    {
        [XmlElement(ElementName = "Adults")]
        public string Adults { get; set; }

        [XmlElement(ElementName = "Children")]
        public string Children { get; set; }

        [XmlElement(ElementName = "Infants")]
        public string Infants { get; set; }

        [XmlElement(ElementName = "RoomType")]
        public string RoomType { get; set; }

        [XmlElement(ElementName = "PaxList")]
        public PaxList PaxList { get; set; }
    }

    [XmlRoot(ElementName = "RoomConfigs")]
    public class RoomConfigs
    {
        [XmlElement(ElementName = "RoomConfig")]
        public List<RoomConfig> RoomConfig { get; set; }
    }
}