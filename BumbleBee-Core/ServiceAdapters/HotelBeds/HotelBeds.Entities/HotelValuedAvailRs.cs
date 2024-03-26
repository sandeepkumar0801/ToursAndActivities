using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    [XmlRootAttribute(IsNullable = false, Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages", ElementName = "HotelValuedAvailRS")]
    public class HotelValuedAvailRs : EntityBase
    {
        /// <remarks/>
        public PaginationData PaginationData { get; set; }

        /// <remarks/>
        [XmlElementAttribute("ServiceHotel")]
        public List<ServiceHotel> ServiceHotel { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("timeToExpiration")]
        public string TimeToExpiration { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("totalItems")]
        public string TotalItems { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("echoToken")]
        public string EchoToken { get; set; }
    }

    /// <remarks/>

    [XmlType(TypeName = "ServiceHotel", Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ServiceHotel
    {
        /// <remarks/>
        public string DirectPayment { get; set; }

        [XmlElement("ContractList")]
        public List<HotelContractList> ContractList
        {
            get;
            set;
        }

        /// <remarks/>
        public DateFrom DateFrom { get; set; }

        /// <remarks/>
        public DateTo DateTo { get; set; }

        /// <remarks/>
        public ServiceCurrency Currency { get; set; }

        public HotelPromotionList PromotionList
        {
            get;
            set;
        }

        /// <remarks/>
        public string PackageRate { get; set; }

        /// <remarks/>
        public ServiceHotelInfo HotelInfo { get; set; }

        /// <remarks/>
        [XmlElementAttribute("AvailableRoom")]
        public List<HotelAvailableRoom> AvailableRoom { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("availToken")]
        public string AvailToken { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelContractList
    {
        /// <remarks/>
        public HotelContract Contract { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelContract
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public IncomingOffice IncomingOffice { get; set; }

        /// <remarks/>
        public HotelContractClassification Classification { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelContractClassification
    {
        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(IsNullable = false)]
    public class HotelPromotionList
    {
        [XmlElement("Promotion")]
        public List<ServiceHotelPromotion> Promotions
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelPromotion
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public string ShortName { get; set; }

        /// <remarks/>
        public string Observations { get; set; }

        public string Remark { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }
    }

    /// <remarks/>
    [XmlType(TypeName = "ProductHotel", Namespace = "http://www.hotelbeds.com/schemas/2005/06/messages")]
    public class ServiceHotelInfo
    {
        /// <remarks/>
        public string Code { get; set; }

        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        [XmlArrayItemAttribute("Image", IsNullable = false)]
        public List<HotelInfoImage> ImageList { get; set; }

        /// <remarks/>
        public HotelInfoCategory Category { get; set; }

        /// <remarks/>
        public HotelInfoDestination Destination { get; set; }

        /// <remarks/>
        public HotelInfoChildAge ChildAge { get; set; }

        /// <remarks/>
        public HotelInfoPosition Position { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelInfoImage
    {
        /// <remarks/>
        public string Type { get; set; }

        /// <remarks/>
        public string Order { get; set; }

        /// <remarks/>
        public string VisualizationOrder { get; set; }

        /// <remarks/>
        public string Url { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelInfoCategory
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("shortname")]
        public string Shortname { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelInfoDestination
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public DestinationZoneList ZoneList { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class DestinationZoneList
    {
        /// <remarks/>
        public DestinationZone Zone { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class DestinationZone
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelInfoChildAge
    {
        /// <remarks/>
        [XmlAttributeAttribute("ageFrom")]
        public string AgeFrom { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ageTo")]
        public string AgeTo { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelInfoPosition
    {
        /// <remarks/>
        [XmlAttributeAttribute("latitude")]
        public string Latitude { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("longitude")]
        public string Longitude { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelAvailableRoom
    {
        /// <remarks/>
        public HotelAvailableRoomOccupancy HotelOccupancy { get; set; }

        /// <remarks/>
        public ServiceHotelRoom HotelRoom { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class HotelAvailableRoomOccupancy
    {
        /// <remarks/>
        public string RoomCount { get; set; }

        /// <remarks/>
        public ServiceHotelOccupancy Occupancy { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelOccupancy
    {
        /// <remarks/>
        public string AdultCount { get; set; }

        /// <remarks/>
        public string ChildCount { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(IsNullable = false)]
    public class DiscountList
    {
        [XmlElement("Discount")]
        public List<ServiceHotelDiscount> Discount
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelDiscount
    {
        public string Code
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Price
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelRoom
    {
        /// <remarks/>
        public ServiceHotelRoomBoard Board { get; set; }

        /// <remarks/>
        public ServiceHotelRoomType RoomType { get; set; }

        /// <remarks/>
        public ServiceHotelRoomPrice Price { get; set; }

        public DiscountList DiscountList { get; set; }

        /// <remarks/>
        public ServiceHotelRoomRate Rate { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("SHRUI")]
        public string Shrui { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("availCount")]
        public string AvailCount { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("onRequest")]
        public string OnRequest { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelRoomBoard
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("shortname")]
        public string Shortname { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelRoomType
    {
        /// <remarks/>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("characteristic")]
        public string Characteristic { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelRoomPrice
    {
        /// <remarks/>
        public string Amount { get; set; }

        /// <remarks/>
        public ServiceHotelRoomSellingPrice SellingPrice { get; set; }

        /// <remarks/>
        public string NetPrice { get; set; }

        /// <remarks/>
        public string Commission { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelRoomSellingPrice
    {
        /// <remarks/>
        [XmlAttributeAttribute("mandatory")]
        public string Mandatory { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public class ServiceHotelRoomRate
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public string Description { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("code")]
        public string Code { get; set; }
    }
}