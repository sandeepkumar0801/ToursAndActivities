using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.GetBookingDates
{
    [XmlRoot(ElementName = "query")]
    public class Query
    {
        [XmlElement(ElementName = "productid")]
        public string Productid { get; set; }
    }

    [XmlRoot(ElementName = "schedule")]
    public class Schedule
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "availability")]
    public class Availability
    {
        [XmlElement(ElementName = "schedule")]
        public Schedule Schedule { get; set; }

        [XmlElement(ElementName = "availablepax")]
        public string Availablepax { get; set; }
    }

    [XmlRoot(ElementName = "values")]
    public class Values
    {
        [XmlElement(ElementName = "day")]
        public string Day { get; set; }

        [XmlElement(ElementName = "availability")]
        public List<Availability> Availability { get; set; }
    }

    [XmlRoot(ElementName = "days")]
    public class Days
    {
        [XmlElement(ElementName = "values")]
        public List<Values> Values { get; set; }
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
    public class GetBookingDatesResponse
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