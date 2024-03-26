using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.Booking
{
    [XmlRoot(ElementName = "customerInformation")]
    public class CustomerInformation
    {
        [XmlElement(ElementName = "firstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "lastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "Phone")]
        public string Phone { get; set; }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
    }

    [XmlRoot(ElementName = "ticketcodeinfo")]
    public class Ticketcodeinfo
    {
        [XmlElement(ElementName = "priceunitname")]
        public string Priceunitname { get; set; }

        [XmlElement(ElementName = "code")]
        public string Code { get; set; }

        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "ticketcode")]
    public class Ticketcode
    {
        [XmlElement(ElementName = "activitycode")]
        public string Activitycode { get; set; }

        [XmlElement(ElementName = "flagpriceunit")]
        public string Flagpriceunit { get; set; }

        [XmlElement(ElementName = "ticketcodeinfo")]
        public Ticketcodeinfo Ticketcodeinfo { get; set; }
    }

    [XmlRoot(ElementName = "ticketcodes")]
    public class Ticketcodes
    {
        [XmlElement(ElementName = "ticketcode")]
        public List<Ticketcode> Ticketcode { get; set; }
    }

    [XmlRoot(ElementName = "schedule")]
    public class Schedule
    {
        [XmlElement(ElementName = "sheduleGroupName")]
        public string SheduleGroupName { get; set; }

        [XmlElement(ElementName = "scheduleName")]
        public string ScheduleName { get; set; }
    }

    [XmlRoot(ElementName = "product")]
    public class BookingResponseProduct
    {
        [XmlElement(ElementName = "ticketRefNo")]
        public string TicketRefNo { get; set; }

        [XmlElement(ElementName = "ticketUrl")]
        public string TicketUrl { get; set; }

        [XmlElement(ElementName = "ticketcodes")]
        public Ticketcodes Ticketcodes { get; set; }

        [XmlElement(ElementName = "productName")]
        public string ProductName { get; set; }

        [XmlElement(ElementName = "travelDate")]
        public string TravelDate { get; set; }

        [XmlElement(ElementName = "referenceNumber")]
        public string ReferenceNumber { get; set; }

        [XmlElement(ElementName = "schedule")]
        public Schedule Schedule { get; set; }

        [XmlElement(ElementName = "priceUnitDesc")]
        public string PriceUnitDesc { get; set; }

        [XmlElement(ElementName = "price")]
        public string Price { get; set; }
    }

    [XmlRoot(ElementName = "productInformation")]
    public class ProductInformation
    {
        [XmlElement(ElementName = "product")]
        public List<BookingResponseProduct> Products { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class BookingResponse
    {
        [XmlElement(ElementName = "customerInformation")]
        public CustomerInformation CustomerInformation { get; set; }

        [XmlElement(ElementName = "productInformation")]
        public ProductInformation ProductInformation { get; set; }
    }
}