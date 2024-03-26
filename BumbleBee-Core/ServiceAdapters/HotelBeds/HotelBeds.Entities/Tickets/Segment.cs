using System.Xml.Serialization;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class Segment : EntityBase
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int Order { get; set; }

        public string Name { get; set; }
    }
}