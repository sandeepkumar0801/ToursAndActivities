using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.Availability
{
    [XmlRoot(ElementName = "query")]
    public class Query
    {
        [XmlElement(ElementName = "productid")]
        public string Productid { get; set; }

        [XmlElement(ElementName = "day")]
        public string Day { get; set; }

        [XmlElement(ElementName = "month")]
        public string Month { get; set; }

        [XmlElement(ElementName = "year")]
        public string Year { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class AvailabilityResponse
    {
        [XmlElement(ElementName = "query")]
        public Query Query { get; set; }

        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }

        [XmlElement(ElementName = "result")]
        public string Result { get; set; }

        [XmlElement(ElementName = "availability")]
        public string Availability { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }
    }
}