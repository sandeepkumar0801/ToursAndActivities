using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.GetProductDates
{
    [XmlRoot(ElementName = "query")]
    public class Query
    {
        [XmlElement(ElementName = "productid")]
        public string Productid { get; set; }
    }

    [XmlRoot(ElementName = "days")]
    public class Days
    {
        [XmlElement(ElementName = "day")]
        public List<string> Day { get; set; }
    }

    [XmlRoot(ElementName = "monthyear")]
    public class Monthyear
    {
        [XmlElement(ElementName = "days")]
        public Days Days { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "dates")]
    public class Dates
    {
        [XmlElement(ElementName = "monthyear")]
        public List<Monthyear> Monthyear { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class GetProductDatesResponse
    {
        [XmlElement(ElementName = "query")]
        public Query Query { get; set; }

        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }

        [XmlElement(ElementName = "result")]
        public string Result { get; set; }

        [XmlElement(ElementName = "dates")]
        public Dates Dates { get; set; }
    }
}