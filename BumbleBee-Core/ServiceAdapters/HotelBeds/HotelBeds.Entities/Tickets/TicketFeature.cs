using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    [XmlType(TypeName = "ProductFeatureGroup")]
    public class TicketFeature : EntityBase
    {
        public int Code { get; set; }
        public string Name { get; set; }

        [XmlAttribute]
        public int Group { get; set; }

        [XmlElement("FeatureList")]
        public List<Feature> FeatureList { get; set; }
    }
}