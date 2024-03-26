using ServiceAdapters.HotelBeds.Constants;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    [XmlRootAttribute(ElementName = "TicketAvailRS", Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages", IsNullable = false)]
    public class TicketAvailRs : EntityBase
    {
        ///// <remarks/>
        //public AuditData AuditData
        //{
        //    get;
        //    set;
        //}
        [XmlIgnore]
        public object InputCriteria { get; set; }

        /// <remarks/>
        public PaginationData PaginationData { get; set; }

        /// <remarks/>
        [XmlElementAttribute("ServiceTicket")]
        public List<ServiceTicket> ServiceTicket { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("totalItems")]
        public string TotalItems { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("echoToken")]
        public string EchoToken { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class PaginationData
    {
        /// <remarks/>
        [XmlAttributeAttribute("currentPage")]
        public string CurrentPage
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlAttributeAttribute("totalPages")]
        public string TotalPages { get; set; }
    }

    /// <remarks/>

    [XmlType(TypeName = "ServiceTicket", Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ServiceTicket : EntityBase
    {
        /// <remarks/>
        public DateFrom DateFrom { get; set; }

        /// <remarks/>
        public DateTo DateTo { get; set; }

        /// <remarks/>
        public ServiceCurrency Currency { get; set; }

        // [XmlElement(ElementName="TicketInfo", Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages/")]

        public TktInfo TicketInfo { get; set; }

        /// <remarks/>
        [XmlElementAttribute("AvailableModality")]
        public List<TktAvailableModality> AvailableModality { get; set; }

        /// <remarks/>
        public TktPaxes Paxes { get; set; }

        public TktFactSheet ContentFactSheet { get; set; }

        /// <remarks/>
        public object BarcodeImageList { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("order")]
        public string Order { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("availToken")]
        public string AvailToken { get; set; }

        public decimal CheapestPriceAdult { get; set; }

        public decimal CheapestPriceChild { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class DateFrom
    {
        /// <remarks/>
        [XmlAttributeAttribute("date")]
        public string Date { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class DateTo
    {
        /// <remarks/>
        [XmlAttributeAttribute("date")]
        public string Date { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ServiceCurrency
    {
        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    // [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    //[XmlType(TypeName = "ProductTicket", Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages/")]
    // [XmlInclude(typeof(ProductTicket))]
    public class TktInfo
    {
        /// <remarks/>
        public string Code { get; set; }

        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public DescriptionList DescriptionList { get; set; }

        /// <remarks/>
        public string CompanyCode { get; set; }

        /// <remarks/>
        public string TicketClass { get; set; }

        /// <remarks/>
        public TktInfoDestination Destination { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class DescriptionList
    {
        /// <remarks/>
        public TktDescription Description { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktDescription
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("languageCode")]
        public string LanguageCode { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktInfoDestination
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktAvailableModality
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public string SupplierOption { get; set; }

        /// <remarks/>
        public ModalityContract Contract { get; set; }

        /// <remarks/>
        [XmlArrayItemAttribute("Price", IsNullable = false)]
        public List<ModalityPrice> PriceList { get; set; }

        /// <remarks/>
        public ModalityType Type { get; set; }

        /// <remarks/>
        public ModalityMode Mode { get; set; }

        [XmlElement("OperationDateList")]
        public OperationDateList OperationDateList { get; set; }

        /// <remarks/>
        public ModalityChildAge ChildAge { get; set; }

        /// <remarks/>
        [XmlArrayItemAttribute("PriceRange", IsNullable = false)]
        public List<ModalityPriceRange> PriceRangeList { get; set; }

        /// <remarks/>
        public string TicketGeneration { get; set; }

        /// <remarks/>
        [XmlElement(Constant.CancellationPolicyList)]
        public List<TktCancellationPolicyList> CancellationPolicyList { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityContract
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public IncomingOffice IncomingOffice { get; set; }

        public string Classification { get; set; }

        [XmlElement("Comments")]
        public List<Comment> Comments { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class IncomingOffice
    {
        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityPrice
    {
        /// <remarks/>
        public string Amount { get; set; }

        /// <remarks/>
        public string Description { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityType
    {
        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityMode
    {
        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class OperationDateList
    {
        /// <remarks/>
        [XmlElement("OperationDate")]
        public List<ModalityOperationDate> OperationDate { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityOperationDate
    {
        /// <remarks/>
        [XmlAttributeAttribute("date")]
        public string Date { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("minimumDuration")]
        public string MinimumDuration { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("maximumDuration")]
        public string MaximumDuration { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityChildAge
    {
        /// <remarks/>
        [XmlAttributeAttribute("ageFrom")]
        public string AgeFrom { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ageTo")]
        public string AgeTo { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ModalityPriceRange
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ageFrom")]
        public string AgeFrom { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ageTo")]
        public string AgeTo { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("unitPrice")]
        public string UnitPrice { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktCancellationPolicyList
    {
        /// <remarks/>
        public TktCancellationPrice Price { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktCancellationPrice
    {
        /// <remarks/>
        public string Amount { get; set; }

        /// <remarks/>
        public TktCancellationPriceDateTimeFrom DateTimeFrom { get; set; }

        /// <remarks/>
        public string Percentage { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktCancellationPriceDateTimeFrom
    {
        /// <remarks/>
        [XmlAttributeAttribute("date")]
        public string Date { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("time")]
        public string Time { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktPaxes
    {
        /// <remarks/>
        public string AdultCount { get; set; }

        /// <remarks/>
        public string ChildCount { get; set; }

        /// <remarks/>
        [XmlArrayItemAttribute("Customer", IsNullable = false)]
        public List<TktCustomer> GuestList { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class TktCustomer
    {
        /// <remarks/>
        public string Age { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }
    }

    /// <remarks/>
    [XmlType(TypeName = "ProductTicket")]
    public class TktFactSheet
    {
        /// <remarks/>
        public string Code { get; set; }

        [XmlAttributeAttribute("name")]
        public string Name { get; set; }

        public List<Description> DescriptionList { get; set; }

        [XmlElement("ImageList")]
        public ImageList ImageList { get; set; }

        public Destination Destination { get; set; }
        public TicketPosition TicketPosition { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }

        [XmlElement("Segmentation")]
        public Segmentation Segmentation { get; set; }

        public string ShortDescription { get; set; }
        public List<Day> OperationDays { get; set; }

        [XmlElement("TicketFeature")]
        public List<TicketFeature> TicketFeatureList { get; set; }
    }
}