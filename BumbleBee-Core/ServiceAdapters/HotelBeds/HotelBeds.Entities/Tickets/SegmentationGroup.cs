using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class SegmentationGroup : EntityBase
    {
        public string Name { get; set; }

        [XmlElement("Segment")]
        public List<Segment> Segment { get; set; }
    }
}