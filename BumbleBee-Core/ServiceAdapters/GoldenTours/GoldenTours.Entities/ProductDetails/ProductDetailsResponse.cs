using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.ProductDetails
{
    [XmlRoot(ElementName = "query")]
    public class Query
    {
        [XmlElement(ElementName = "productid")]
        public string Productid { get; set; }
    }

    [XmlRoot(ElementName = "description")]
    public class Description
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "content")]
        public string Content { get; set; }
    }

    [XmlRoot(ElementName = "descriptions")]
    public class Descriptions
    {
        [XmlElement(ElementName = "description")]
        public List<Description> Description { get; set; }
    }

    [XmlRoot(ElementName = "time")]
    public class Time
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "pickup_date")]
    public class Pickup_date
    {
        [XmlElement(ElementName = "pickupstart_date")]
        public string Pickupstart_date { get; set; }

        [XmlElement(ElementName = "pickupend_date")]
        public string Pickupend_date { get; set; }
    }

    [XmlRoot(ElementName = "pickup_dates")]
    public class Pickup_dates
    {
        [XmlElement(ElementName = "pickup_date")]
        public List<Pickup_date> Pickup_date { get; set; }
    }

    [XmlRoot(ElementName = "pickup")]
    public class Pickup
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "time")]
        public Time Time { get; set; }

        [XmlElement(ElementName = "pickup_dates")]
        public Pickup_dates Pickup_dates { get; set; }

        [XmlElement(ElementName = "address")]
        public string Address { get; set; }

        [XmlElement(ElementName = "postcode")]
        public string Postcode { get; set; }

        [XmlElement(ElementName = "pickupdescription")]
        public string Pickupdescription { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "scheduleid")]
        public string ScheduleId { get; set; }
    }

    [XmlRoot(ElementName = "pickups")]
    public class Pickups
    {
        [XmlElement(ElementName = "pickup")]
        public List<Pickup> Pickup { get; set; }
    }

    [XmlRoot(ElementName = "droppoint_date")]
    public class Droppoint_date
    {
        [XmlElement(ElementName = "droppointstart_date")]
        public string Droppointstart_date { get; set; }

        [XmlElement(ElementName = "droppointend_date")]
        public string Droppointend_date { get; set; }
    }

    [XmlRoot(ElementName = "droppoint_dates")]
    public class Droppoint_dates
    {
        [XmlElement(ElementName = "droppoint_date")]
        public List<Droppoint_date> Droppoint_date { get; set; }
    }

    [XmlRoot(ElementName = "droppoint")]
    public class Droppoint
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "time")]
        public Time Time { get; set; }

        [XmlElement(ElementName = "droppoint_dates")]
        public Droppoint_dates Droppoint_dates { get; set; }

        [XmlElement(ElementName = "address")]
        public string Address { get; set; }

        [XmlElement(ElementName = "postcode")]
        public string Postcode { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "droppoints")]
    public class Droppoints
    {
        [XmlElement(ElementName = "droppoint")]
        public List<Droppoint> Droppoint { get; set; }
    }

    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "images")]
    public class Images
    {
        [XmlElement(ElementName = "image")]
        public List<Image> Image { get; set; }
    }

    [XmlRoot(ElementName = "unit")]
    public class Unit
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "discount")]
        public string Discount { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "price")]
        public string Price { get; set; }

        [XmlElement(ElementName = "notes")]
        public string Notes { get; set; }
    }

    [XmlRoot(ElementName = "offer_unit")]
    public class Offer_unit
    {
        [XmlElement(ElementName = "unit")]
        public List<Unit> Unit { get; set; }
    }

    [XmlRoot(ElementName = "offer")]
    public class Offer
    {
        [XmlElement(ElementName = "discount_start_date")]
        public string Discount_start_date { get; set; }

        [XmlElement(ElementName = "discount_end_date")]
        public string Discount_end_date { get; set; }

        [XmlElement(ElementName = "discount_on")]
        public string Discount_on { get; set; }

        [XmlElement(ElementName = "discount_apply_book_before_days")]
        public string Discount_apply_book_before_days { get; set; }

        [XmlElement(ElementName = "discount_book_before_days")]
        public string Discount_book_before_days { get; set; }

        [XmlElement(ElementName = "discount_type")]
        public string Discount_type { get; set; }

        [XmlElement(ElementName = "discount")]
        public string Discount { get; set; }

        [XmlElement(ElementName = "offer_unit")]
        public Offer_unit Offer_unit { get; set; }
    }

    [XmlRoot(ElementName = "special_offer")]
    public class Special_offer
    {
        [XmlElement(ElementName = "offer")]
        public List<Offer> Offer { get; set; }
    }

    [XmlRoot(ElementName = "schedule")]
    public class Schedule
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "priceunits")]
    public class Priceunits
    {
        [XmlElement(ElementName = "unit")]
        public List<Unit> Unit { get; set; }
    }

    [XmlRoot(ElementName = "period")]
    public class Period
    {
        [XmlElement(ElementName = "start_date")]
        public string Start_date { get; set; }

        [XmlElement(ElementName = "end_date")]
        public string End_date { get; set; }

        [XmlElement(ElementName = "days")]
        public string Days { get; set; }

        [XmlElement(ElementName = "minimum_pax")]
        public string Minimum_pax { get; set; }

        [XmlElement(ElementName = "maximum_pax")]
        public string Maximum_pax { get; set; }

        [XmlElement(ElementName = "schedule")]
        public Schedule Schedule { get; set; }

        [XmlElement(ElementName = "priceunits")]
        public Priceunits Priceunits { get; set; }
    }

    [XmlRoot(ElementName = "priceperiods")]
    public class Priceperiods
    {
        [XmlElement(ElementName = "period")]
        public List<Period> Period { get; set; }
    }

    [XmlRoot(ElementName = "transferoption")]
    public class Transferoption
    {
        [XmlAttribute(AttributeName = "required")]
        public string Required { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "transferoptions")]
    public class Transferoptions
    {
        [XmlElement(ElementName = "transferoption")]
        public List<Transferoption> Transferoption { get; set; }
    }

    [XmlRoot(ElementName = "product")]
    public class Product
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "short_description")]
        public string Short_description { get; set; }

        [XmlElement(ElementName = "descriptions")]
        public Descriptions Descriptions { get; set; }

        [XmlElement(ElementName = "city")]
        public string City { get; set; }

        [XmlElement(ElementName = "producttype")]
        public string Producttype { get; set; }

        [XmlElement(ElementName = "transfertype")]
        public string Transfertype { get; set; }

        [XmlElement(ElementName = "sharedtransfer")]
        public string Sharedtransfer { get; set; }

        [XmlElement(ElementName = "shuttle")]
        public string Shuttle { get; set; }

        [XmlElement(ElementName = "priceunittype")]
        public string Priceunittype { get; set; }

        [XmlElement(ElementName = "timeduration")]
        public string Timeduration { get; set; }

        [XmlElement(ElementName = "duration")]
        public string Duration { get; set; }

        [XmlElement(ElementName = "book_before")]
        public string Book_before { get; set; }

        [XmlElement(ElementName = "book_before_type")]
        public string Book_before_type { get; set; }

        [XmlElement(ElementName = "direction")]
        public string Direction { get; set; }

        [XmlElement(ElementName = "ref")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "pickups")]
        public Pickups Pickups { get; set; }

        [XmlElement(ElementName = "droppoints")]
        public Droppoints Droppoints { get; set; }

        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }

        [XmlElement(ElementName = "thumbnail")]
        public string Thumbnail { get; set; }

        [XmlElement(ElementName = "special_offer")]
        public Special_offer Special_offer { get; set; }

        [XmlElement(ElementName = "priceperiods")]
        public Priceperiods Priceperiods { get; set; }

        [XmlElement(ElementName = "transferoptions")]
        public Transferoptions Transferoptions { get; set; }

        [XmlElement(ElementName = "content")]
        public string Content { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class ProductDetailsResponse
    {
        [XmlElement(ElementName = "query")]
        public Query Query { get; set; }

        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }

        [XmlElement(ElementName = "result")]
        public string Result { get; set; }

        [XmlElement(ElementName = "product")]
        public Product Product { get; set; }
    }
}