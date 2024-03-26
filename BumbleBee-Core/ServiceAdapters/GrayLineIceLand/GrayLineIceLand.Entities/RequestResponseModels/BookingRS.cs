using Isango.Entities.GrayLineIceLand;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels
{
    public class BookingRS : EntityBase
    {
        public bool IsDeleted { get; set; }
        public bool IsHiddenPrices { get; set; }
        public int BookingNumber { get; set; }
        public int DocumentTypeId { get; set; }
        public int ConfirmationDetailLevelId { get; set; }
        public string BookingGuid { get; set; }
        public int AgentProfileId { get; set; }
        public string AgentReference { get; set; }
        public string ExternalIdentity { get; set; }
        public string ExternalSubIdentity { get; set; }
        public DateTime Created { get; set; }
        public int ProfileId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public DateTime PickupTime { get; set; }
        public string CurrencyCode { get; set; }
        public Orderdetail[] OrderDetails { get; set; }
        public object[] Notes { get; set; }
        public Payment[] Payments { get; set; }
        public int PriceType { get; set; }
        public bool BookingPricesHaveChanged { get; set; }
        public int ErrorCode { get; set; }

        public List<GrayLineIceLandSelectedProduct> SelectedProduct { get; set; }
    }

    public class Orderdetail
    {
        public int OrderDetailId { get; set; }
        public string OrderDetailDescription { get; set; }
        public string ProductNumber { get; set; }
        public string TourNumber { get; set; }
        public string TourDescription { get; set; }
        public int TourId { get; set; }
        public int TourDepartureId { get; set; }
        public int SystemProviderId { get; set; }
        public DateTime Departure { get; set; }
        public DateTime PickupTime { get; set; }
        public int PickupMinutes { get; set; }
        public decimal Quantity { get; set; }
        public int AgeGroup { get; set; }
        public string AgeGroupDescription { get; set; }
        public string PickupLocation { get; set; }
        public string DropOffLocation { get; set; }
        public string PickupNote { get; set; }
        public string DropOffNote { get; set; }
        public string PickupFlightNumber { get; set; }
        public string Language { get; set; }
        public string PickupPlaceText { get; set; }
        public int PickupLocationId { get; set; }
        public int DropOffLocationId { get; set; }
        public int PaxDetailTypeId { get; set; }
        public int CheckInStatus { get; set; }
        public int PickupStatus { get; set; }
        public int DropOffStatus { get; set; }
        public int AccountingStatus { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public float CommissionPercent { get; set; }
        public float VatPercent { get; set; }
        public string ExternalReference { get; set; }
        public object[] Vouchers { get; set; }
        public bool PaymentRequired { get; set; }
        public string PhotoListViewUrl { get; set; }
        public string PhotoDetailViewUrl { get; set; }
        public string ComboTourDescription { get; set; }
        public bool IsDeleted { get; set; }
    }
}