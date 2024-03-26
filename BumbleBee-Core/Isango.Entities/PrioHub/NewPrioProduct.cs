using System;
namespace Isango.Entities.PrioHub
{
    public class NewPrioProduct
    {
        public int ProductId { get; set; }
        public string ProductInternalReference { get; set; }
        public string ProductSupplierId { get; set; }
        public string ProductDistributorId { get; set; }
        public string ProductDistributorName { get; set; }
        public string ProductResellerId { get; set; }
        public string ProductResellerName { get; set; }

        public string Productsourcename { get; set; }
        public string ProductFromPrice { get; set; }
        public string ProductStartDate { get; set; }

        public string ProductEndDate { get; set; }
        public string ProductBookingStartDate { get; set; }
        public int ProductDuration { get; set; }
        public bool ProductThirdParty { get; set; }

        public bool ProductSeasonalPricing { get; set; }
        public bool ProductQuantityPricing { get; set; }

        public bool ProductDailyPricing { get; set; }

        public bool ProductDynamicPricing { get; set; }
        public bool ProductrelationDetailsVisible { get; set; }
        public bool ProductTimePickerVisible { get; set; }
        public bool ProductCluster { get; set; }
        public bool ProductCombi { get; set; }
        public bool ProductAddon { get; set; }
        public bool ProductAvailability { get; set; }
        public string ProductCapacityId { get; set; }
        public bool ProductCapacity { get; set; }
        public bool ProductTravelDateRequired { get; set; }
        public bool productCancellationAllowed { get; set; }
        public bool ProductoverBookingAllowed { get; set; }
        public string ProductCapacityType { get; set; }
        public string ProductAdmissionType { get; set; }
        public string ProductStatus { get; set; }
        public string productCatalogueStatus { get; set; }
        public string ProductPickupPoint { get; set; }
        public string ProductTitle { get; set; }
        public string ProductShortDescription { get; set; }
        public string ProductLongDescription { get; set; }
        public string ProductAdditionalInformation { get; set; }
        public string ProductdurationText { get; set; }
        public string ProductSupplierName { get; set; }
        public string ProductEntryNotes { get; set; }
        public string ProductcodeFormat { get; set; }
        public string ProductCodeSource { get; set; }
        public bool ProductGroupCode { get; set; }
        public bool ProductCombiCode { get; set; }
        public string ProductVoucherSettings { get; set; }
        public string ProductPaymentCurrency { get; set; }
        public string ProductViewType { get; set; }
        public string Route { get; set; }

        public int MinQuantity { get; set; }

        public int MaxQuantity { get; set; }
    }
}