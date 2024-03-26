using Isango.Entities.Activities;
using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Entities
{
    public class SelectedProduct : ErrorList
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HotelPickUpLocation { get; set; }

        public string HotelDropoffLocation { get; set; }

        public bool OnSale { get; set; }

        public ProductType ProductType { get; set; }

        private OptionBookingStatus _status;

        public OptionBookingStatus Status
        {
            get
            {
                if (ProductOptions == null || ProductOptions?.Count == 0)
                {
                    _status = OptionBookingStatus.Failed;
                }
                if (ProductOptions?.All(x => x.BookingStatus == OptionBookingStatus.Failed) == true)
                {
                    _status = OptionBookingStatus.Failed;
                }
                else if (ProductOptions?.All(x => x.BookingStatus == OptionBookingStatus.Cancelled) == true)
                {
                    _status = OptionBookingStatus.Cancelled;
                }
                else if (ProductOptions?.All(x => x.BookingStatus == OptionBookingStatus.Requested) == true)
                {
                    _status = OptionBookingStatus.Requested;
                }
                else
                {
                    _status = OptionBookingStatus.Confirmed;
                }
                return _status;
            }
        }

        public string DurationString { get; set; }

        public List<double> Duration { get; set; }

        /// <summary>
        /// Time in integer
        /// </summary>
        /// <example>For 11:30, output will be 1130, if the time is 1 25, it will be 125</example>
        public List<int> Time { get; set; }

        public string ScheduleReturnDetails { get; set; }

        public Supplier Supplier { get; set; }

        public string TsProductName { get; set; }

        public string TsProductCode { get; set; }

        public double SupplierPrice { get; set; }

        public string SupplierCurrency { get; set; }

        public double SellPrice { get; set; }

        public decimal MultisaveDiscountedPrice { get; set; }

        public string StartTime { get; set; }

        public bool IsServiceLevelPickUp { get; set; }

        public bool IsPickupFilled { get; set; }

        /// <summary>
        /// Cancellation Policy Text
        /// </summary>
        public string CancellationPolicy { get; set; }
        public int? BUNDLESERVICEID { get; set; }

        /// <summary>
        /// Api Cancellation Policy Processed
        /// </summary>
        public string SupplierCancellationPolicy { get; set; }

        private bool _isReceipt;

        public bool GetIsReceipt()
        {
            return _isReceipt;
        }

        public void SetIsReceipt(bool value)
        {
            _isReceipt = value;
        }

        public string AvailabilityInformation { get; set; }

        public string ThumbNailImage { get; set; }

        public string ActivityCode { get; set; }

        public List<ProductOption> ProductOptions { get; set; }

        public string ProductName { set; get; }

        public int ProductId { set; get; }

        public decimal Price { set; get; }

        public ActivityType ActivityType { set; get; }

        public bool IsPaxDetailRequired { set; get; }

        public List<PassengerDetail> PassengerDetails { get; set; }

        public decimal DiscountedPrice { set; get; }

        public string SpecialRequest { set; get; }

        public string RegionId { get; set; }

        public string ProductSeoUrl { set; get; }

        public List<int> AttractionIds { get; set; }

        public string ImagePath { get; set; }

        public int ImageId { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public bool IsPackage { get; set; }

        public string PriceWithSymbol { get; set; }

        public bool IsSmartPhoneVoucher { get; set; }

        public string NodeAttractionId { get; set; }

        public string NodeAttractionName { get; set; }

        public List<ActivityCategoryType> CategoryTypes { get; set; }

        public APIType APIType { get; set; }

        public int ParentBundleId { get; set; }

        public string ParentBundleName { get; set; }

        public Supplier ActivityOperator { get; set; }

        public string ScheduleLocation { get; set; }

        public PickUpDropOffOptionType PickUpOption { get; set; }

        public PickUpDropOffOptionType DropOffOption { get; set; }

        public Boolean IsReceipt { get; set; }

        public List<ActivityItinerary> Itineraries { get; set; }

        public List<AppliedDiscountCoupon> AppliedDiscountCoupons { set; get; }

        public string SupplierConfirmationData { get; set; }

        public string CartReferenceId { get; set; }

        public DateTime Expiry { get; set; }

        public string AvailabilityReferenceId { get; set; }
        public int BundleOptionId { get; set; }
        public string UnitType { get; set; }
        public List<Region.Region> Regions { get; set; }

        public List<AgeGroupDescription> AgeGroupDescription { get; set; }
        public List<PaxWisePdfEntity> PaxWisePdfDetails { get; set; }
        public bool IsShowSupplierVoucher { get; set; }
        public string LinkType { get; set; }
        public string LinkValue { get; set; }
        public string Variant { get; set; }
        public string CountryCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Category { get; set; }
        public int? LineOfBusiness { get; set; }

        public string InvoicingCompany { get; set; }

        public bool AdyenStringentAccount { get; set; }

        public List<Barcode> Barcodes { get; set; }
    }

    public class Barcode
    {
        public string PricingCategoryId { get; set; }
        public string PassengerType { get; set; }
        public string BarcodeType { get; set; }
        public string BarCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    //Redeam API class

}