using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.PickupPoints
{
    [XmlRoot(ElementName = "query")]
    public class Query
    {
        [XmlElement(ElementName = "productid")]
        public string Productid { get; set; }
    }

    [XmlRoot(ElementName = "time")]
    public class Time
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "pickup")]
    public class Pickup
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "time")]
        public Time Time { get; set; }

        [XmlElement(ElementName = "address")]
        public string Address { get; set; }

        [XmlElement(ElementName = "postcode")]
        public string Postcode { get; set; }

        [XmlElement(ElementName = "pickupdescription")]
        public string Pickupdescription { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "pickupname")]
        public string Pickupname { get; set; }

        [XmlElement(ElementName = "pickuptimes")]
        public Pickuptimes Pickuptimes { get; set; }

        [XmlAttribute(AttributeName = "scheduleid")]
        public string ScheduleId { get; set; }
    }

    [XmlRoot(ElementName = "pickups")]
    public class Pickups
    {
        [XmlElement(ElementName = "pickup")]
        public List<Pickup> Pickup { get; set; }
    }

    [XmlRoot(ElementName = "period")]
    public class Period
    {
        [XmlElement(ElementName = "startdate")]
        public string Startdate { get; set; }

        [XmlElement(ElementName = "enddate")]
        public string Enddate { get; set; }
    }

    [XmlRoot(ElementName = "pickuptimeperiods")]
    public class Pickuptimeperiods
    {
        [XmlElement(ElementName = "period")]
        public List<Period> Period { get; set; }
    }

    [XmlRoot(ElementName = "pickuptime")]
    public class Pickuptime
    {
        [XmlElement(ElementName = "pickuptimeid")]
        public string Pickuptimeid { get; set; }

        [XmlElement(ElementName = "pickuptimename")]
        public string Pickuptimename { get; set; }

        [XmlElement(ElementName = "pickuptimeperiods")]
        public Pickuptimeperiods Pickuptimeperiods { get; set; }
    }

    [XmlRoot(ElementName = "pickuptimes")]
    public class Pickuptimes
    {
        [XmlElement(ElementName = "pickuptime")]
        public List<Pickuptime> Pickuptime { get; set; }
    }

    [XmlRoot(ElementName = "blockpickups")]
    public class Blockpickups
    {
        [XmlElement(ElementName = "pickup")]
        public List<Pickup> Pickup { get; set; }
    }

    [XmlRoot(ElementName = "productpickups")]
    public class Productpickups
    {
        [XmlElement(ElementName = "pickups")]
        public Pickups Pickups { get; set; }

        [XmlElement(ElementName = "blockpickups")]
        public Blockpickups Blockpickups { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class PickupPointsResponse
    {
        [XmlElement(ElementName = "query")]
        public Query Query { get; set; }

        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }

        [XmlElement(ElementName = "result")]
        public string Result { get; set; }

        [XmlElement(ElementName = "productpickups")]
        public Productpickups Productpickups { get; set; }
    }
}