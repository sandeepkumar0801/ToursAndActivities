using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Entities.Booking
{
    [XmlRoot(ElementName = "customer")]
    public class Customer
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "firstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "lastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "city")]
        public string City { get; set; }

        [XmlElement(ElementName = "county")]
        public string County { get; set; }

        [XmlElement(ElementName = "postCode")]
        public string PostCode { get; set; }

        [XmlElement(ElementName = "countryCode")]
        public string CountryCode { get; set; }

        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }

        [XmlElement(ElementName = "mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "flagnewsLetter")]
        public string FlagnewsLetter { get; set; }

        [XmlElement(ElementName = "sendEmail")]
        public string SendEmail { get; set; }
    }

    [XmlRoot(ElementName = "unit")]
    public class Unit
    {
        [XmlElement(ElementName = "unitId")]
        public string UnitId { get; set; }

        [XmlElement(ElementName = "paxCount")]
        public string PaxCount { get; set; }
    }

    [XmlRoot(ElementName = "paxInfo")]
    public class PaxInfo
    {
        [XmlElement(ElementName = "unit")]
        public List<Unit> Unit { get; set; }
    }

    [XmlRoot(ElementName = "product")]
    public class Product
    {
        [XmlElement(ElementName = "travelDate")]
        public string TravelDate { get; set; }

        [XmlElement(ElementName = "productId")]
        public string ProductId { get; set; }

        [XmlElement(ElementName = "scheduleId")]
        public string ScheduleId { get; set; }

        [XmlElement(ElementName = "pickuptimeId")]
        public string PickuptimeId { get; set; }

        [XmlElement(ElementName = "paxtoken")]
        public string Paxtoken { get; set; }

        [XmlElement(ElementName = "referenceNumber")]
        public string ReferenceNumber { get; set; }

        [XmlElement(ElementName = "otherRequirement")]
        public string OtherRequirement { get; set; }

        [XmlElement(ElementName = "promotionalCode")]
        public string PromotionalCode { get; set; }

        [XmlElement(ElementName = "paxInfo")]
        public PaxInfo PaxInfo { get; set; }

        [XmlElement(ElementName = "nights")]
        public string Nights { get; set; }

        [XmlElement(ElementName = "droppointId")]
        public string DroppointId { get; set; }

        [XmlElement(ElementName = "transferInfo")]
        public TransferInfo TransferInfo { get; set; }
    }

    [XmlRoot(ElementName = "transferInfo")]
    public class TransferInfo
    {
        [XmlElement(ElementName = "airlineName")]
        public string AirlineName { get; set; }

        [XmlElement(ElementName = "flightNumber")]
        public string FlightNumber { get; set; }

        [XmlElement(ElementName = "transferTime")]
        public string TransferTime { get; set; }

        [XmlElement(ElementName = "hotelName")]
        public string HotelName { get; set; }

        [XmlElement(ElementName = "hotelAddress")]
        public string HotelAddress { get; set; }

        [XmlElement(ElementName = "postCode")]
        public string PostCode { get; set; }

        [XmlElement(ElementName = "mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "origin")]
        public string Origin { get; set; }

        [XmlElement(ElementName = "destination")]
        public string Destination { get; set; }

        [XmlElement(ElementName = "greetingName")]
        public string GreetingName { get; set; }

        [XmlElement(ElementName = "returnDate")]
        public string ReturnDate { get; set; }

        [XmlElement(ElementName = "returnTime")]
        public string ReturnTime { get; set; }

        [XmlElement(ElementName = "returnFlight")]
        public string ReturnFlight { get; set; }
    }

    [XmlRoot(ElementName = "productInfo")]
    public class ProductInfo
    {
        [XmlElement(ElementName = "product")]
        public List<Product> Product { get; set; }
    }

    [XmlRoot(ElementName = "cardPayment")]
    public class CardPayment
    {
        [XmlElement(ElementName = "nameOnCard")]
        public string NameOnCard { get; set; }

        [XmlElement(ElementName = "cardNumber")]
        public string CardNumber { get; set; }

        [XmlElement(ElementName = "cardType")]
        public string CardType { get; set; }

        [XmlElement(ElementName = "cardCode")]
        public string CardCode { get; set; }

        [XmlElement(ElementName = "expiryDate")]
        public string ExpiryDate { get; set; }

        [XmlElement(ElementName = "securityCode")]
        public string SecurityCode { get; set; }

        [XmlElement(ElementName = "issueNumber")]
        public string IssueNumber { get; set; }

        [XmlElement(ElementName = "cardVerificationNumber")]
        public string CardVerificationNumber { get; set; }
    }

    [XmlRoot(ElementName = "Booking")]
    public class BookingRequest
    {
        [XmlElement(ElementName = "agentId")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "key")]
        public string Key { get; set; }

        [XmlElement(ElementName = "customer")]
        public Customer Customer { get; set; }

        [XmlElement(ElementName = "productInfo")]
        public ProductInfo ProductInfo { get; set; }

        [XmlElement(ElementName = "currencyCode")]
        public string CurrencyCode { get; set; }

        [XmlElement(ElementName = "paymentMode")]
        public string PaymentMode { get; set; }

        [XmlElement(ElementName = "charge")]
        public string Charge { get; set; }

        [XmlElement(ElementName = "cardPayment")]
        public CardPayment CardPayment { get; set; }

        [XmlElement(ElementName = "flagPriceDisplay")]
        public string FlagPriceDisplay { get; set; }
    }
}