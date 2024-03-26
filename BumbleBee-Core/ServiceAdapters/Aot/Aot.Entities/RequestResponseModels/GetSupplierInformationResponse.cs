using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "AccommodationInfo")]
    public class AccommodationInfo
    {
        [XmlElement(ElementName = "AdditionalInfo")]
        public string AdditionalInfo { get; set; }

        [XmlElement(ElementName = "BarInfo")]
        public string BarInfo { get; set; }

        [XmlElement(ElementName = "ChainCode")]
        public string ChainCode { get; set; }

        [XmlElement(ElementName = "CheckIn")]
        public string CheckIn { get; set; }

        [XmlElement(ElementName = "CheckInOutInfo")]
        public string CheckInOutInfo { get; set; }

        [XmlElement(ElementName = "Checkout")]
        public string Checkout { get; set; }

        [XmlElement(ElementName = "ChildrensFacilities")]
        public string ChildrensFacilities { get; set; }

        [XmlElement(ElementName = "CreditCardsAccepted")]
        public string CreditCardsAccepted { get; set; }

        [XmlElement(ElementName = "DaySpaFacilities")]
        public string DaySpaFacilities { get; set; }

        [XmlElement(ElementName = "DisabledFacilities")]
        public string DisabledFacilities { get; set; }

        [XmlElement(ElementName = "DistanceToAirport")]
        public string DistanceToAirport { get; set; }

        [XmlElement(ElementName = "DistanceToBeach")]
        public string DistanceToBeach { get; set; }

        [XmlElement(ElementName = "DistanceToPublicTransport")]
        public string DistanceToPublicTransport { get; set; }

        [XmlElement(ElementName = "DrivingInfo")]
        public string DrivingInfo { get; set; }

        [XmlElement(ElementName = "NumFloors")]
        public string NumFloors { get; set; }

        [XmlElement(ElementName = "NumRooms")]
        public string NumRooms { get; set; }

        [XmlElement(ElementName = "ReceptionHours")]
        public string ReceptionHours { get; set; }

        [XmlElement(ElementName = "RestaurantInfo")]
        public string RestaurantInfo { get; set; }

        [XmlElement(ElementName = "TransferInfo")]
        public string TransferInfo { get; set; }
    }

    [XmlRoot(ElementName = "Supplier")]
    public class Supplier
    {
        [XmlElement(ElementName = "SupplierCode")]
        public string SupplierCode { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "LocationID")]
        public string LocationID { get; set; }

        [XmlElement(ElementName = "LocationName")]
        public string LocationName { get; set; }

        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "MinChildAge")]
        public string MinChildAge { get; set; }

        [XmlElement(ElementName = "MaxChildAge")]
        public string MaxChildAge { get; set; }

        [XmlElement(ElementName = "ChildPolicyDescription")]
        public string ChildPolicyDescription { get; set; }

        [XmlElement(ElementName = "CancellationPolicy")]
        public string CancellationPolicy { get; set; }

        [XmlElement(ElementName = "StarRating")]
        public string StarRating { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }

        [XmlElement(ElementName = "AccommodationInfo")]
        public AccommodationInfo AccommodationInfo { get; set; }

        [XmlElement(ElementName = "SupplierImportantInfo")]
        public string SupplierImportantInfo { get; set; }
    }

    [XmlRoot(ElementName = "SupplierInfoResponse")]
    public class SupplierInfoResponse
    {
        [XmlElement(ElementName = "Supplier")]
        public List<Supplier> Supplier { get; set; }
    }
}