using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class ImageList : EntityBase
    {
        [XmlElement("ImageList")]
        public List<Image> Images { get; set; }
    }
}