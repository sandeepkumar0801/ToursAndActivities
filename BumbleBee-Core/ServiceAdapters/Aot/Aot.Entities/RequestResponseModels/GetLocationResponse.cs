using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "Location")]
    public class Location
    {
        [XmlElement(ElementName = "LocationType")]
        public string LocationType { get; set; }

        [XmlElement(ElementName = "LocationCode")]
        public string LocationCode { get; set; }

        [XmlElement(ElementName = "CountryCode")]
        public string CountryCode { get; set; }

        [XmlElement(ElementName = "StateCode")]
        public string StateCode { get; set; }

        [XmlElement(ElementName = "LocationName")]
        public string LocationName { get; set; }
    }

    [XmlRoot(ElementName = "GetLocationsResponse")]
    public class GetLocationsResponse
    {
        [XmlElement(ElementName = "Location")]
        public List<Location> Location { get; set; }
    }
}