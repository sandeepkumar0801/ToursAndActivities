using Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;
using System;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels
{
    public class ToursAvailabilityRS : EntityBase
    {
        public Tourdeparture[] TourDepartures { get; set; }
        public int[] PaymentTypesAllowed { get; set; }
        public int ErrorCode { get; set; }
    }

    public class Tourdeparture
    {
        public object[] SlaveTours { get; set; }
        public string TourNumber { get; set; }
        public int BookingTemplateId { get; set; }
        public int TourId { get; set; }
        public int DimensionId { get; set; }
        public int ClosingTimeMinutes { get; set; }
        public string TourDescription { get; set; }
        public string DepartureText { get; set; }
        public DateTime Departure { get; set; }
        public int DurationMinutes { get; set; }
        public int PaxDetailTypeId { get; set; }
        public int TourDepartureId { get; set; }
        public int SystemProviderId { get; set; }
        public bool Available { get; set; }
        public int SeatsLeft { get; set; }
        public Price[] Prices { get; set; }
        public string ExternalOrderId { get; set; }
        public string ExternalBarcode { get; set; }
        public object[] Vouchers { get; set; }
        public string PhotoListViewUrl { get; set; }
        public string PhotoDetailViewUrl { get; set; }
        public Pickuplocation[] PickUpLocations { get; set; }
        public object[] DropOffLocations { get; set; }
        public int PickupDropOffId { get; set; }
        public bool PickupTime { get; set; }
        public bool DropOffTime { get; set; }
        public bool PickupFlightInfo { get; set; }
        public bool DropOffFlightInfo { get; set; }
        public Agegroup[] AgeGroups { get; set; }
        public Tourproduct[] TourProducts { get; set; }
        public Guidelanguage[] GuideLanguages { get; set; }
        public bool Ticked { get; set; }
        public bool HasInventory { get; set; }
        public bool AllHaveSameDate { get; set; }
        public bool IsStructure { get; set; }
        public bool IsHasMultiplePaxPriceRanges { get; set; }
        public DateTime FromTourDate { get; set; }
        public DateTime UntilTourDate { get; set; }
        public int MinimumPaxPerOrder { get; set; }
        public int MaximumPaxPerOrder { get; set; }
        public bool IsFlexibleDeparture { get; set; }
    }

    public class Price
    {
        public int AgeGroup { get; set; }
        public string AgeGroupDescription { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public decimal PricePerPAX { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal DropOffPrice { get; set; }
        public decimal ExternalExtraCharge { get; set; }
        public int FareRuleBucketId { get; set; }
        public bool isSurchargesIncluded { get; set; }
        public int AgeDiscountSchemaId { get; set; }
        public decimal CommissionPercent { get; set; }
    }

    public class Agegroup
    {
        public int AgeGroup { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public float Discount { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }

    public class Tourproduct
    {
        public int ProductId { get; set; }
        public int DimensionId { get; set; }
        public string ProductNumber { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public decimal Price { get; set; }
        public bool Mandatory { get; set; }
        public bool MustBeOnePerPaxIfSelected { get; set; }
        public bool AnyQuantityAllowed { get; set; }
        public bool MandatoryForMinimumPaxPerOrder { get; set; }
        public int AgeGroup { get; set; }
        public int Quantity { get; set; }
        public int MaxPax { get; set; }
        public int ProductTypeId { get; set; }
    }

    public class Guidelanguage
    {
        public int LanguageCode { get; set; }
        public int LanguageId { get; set; }
        public string Language { get; set; }
        public bool Ticked { get; set; }
    }
}