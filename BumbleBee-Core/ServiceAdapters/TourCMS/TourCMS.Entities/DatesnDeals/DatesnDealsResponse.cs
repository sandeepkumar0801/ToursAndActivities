using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.DatesnDealsResponse
{
    [XmlRoot(ElementName = "response")]
    public class DatesnDealsResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }

        [XmlElement(ElementName = "total_date_count")]
        public string TotalDateCount { get; set; }

        [XmlElement(ElementName = "channel_id")]
        public string ChannelId { get; set; }

        [XmlElement(ElementName = "account_id")]
        public string AccountId { get; set; }

        [XmlElement(ElementName = "tour_id")]
        public string TourId { get; set; }

        [XmlElement(ElementName = "dates_and_prices")]
        public DatesAndPrices DatesAndPrices { get; set; }
    }

    public class DatesAndPrices
    {
        [XmlElement(ElementName = "date")]
        public List<DateList> DateList { get; set; }
    }

    public class DateList
    {
        [XmlElement(ElementName = "start_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "date_code")]
        public string DateCode { get; set; }

        [XmlElement(ElementName = "note")]
        public string Note { get; set; }

        [XmlElement(ElementName = "sale_currency")]
        public string SaleCurrency { get; set; }

        [XmlElement(ElementName = "min_booking_size")]
        public string MinBookingSize { get; set; }

        [XmlElement(ElementName = "spaces_remaining")]
        public string SpacesRemaining { get; set; }

        [XmlElement(ElementName = "special_offer_type")]
        public string SpecialOfferType { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "book_url")]
        public string BookUrl { get; set; }

        [XmlElement(ElementName = "start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "price_1")]
        public string Price1 { get; set; }

        [XmlElement(ElementName = "price_2")]
        public string Price2 { get; set; }

        [XmlElement(ElementName = "price_1_display")]
        public string Price1Display { get; set; }

        [XmlElement(ElementName = "price_2_display")]
        public string Price2Display { get; set; }

        [XmlElement(ElementName = "supplier_note")]
        public string SupplierNote { get; set; }
    }
}