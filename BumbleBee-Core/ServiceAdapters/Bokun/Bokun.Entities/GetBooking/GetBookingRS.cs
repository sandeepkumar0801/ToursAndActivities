using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.GetBooking
{
    public class GetBookingRs
    {
        public List<Accommodationbooking> AccommodationBookings { get; set; }
        public List<Activitybooking> ActivityBookings { get; set; }
        public Affiliate Affiliate { get; set; }
        public Agent Agent { get; set; }
        public Bookingchannel BookingChannel { get; set; }
        public List<Bookingfield> BookingFields { get; set; }
        public List<Carrentalbooking> CarRentalBookings { get; set; }
        public string ConfirmationCode { get; set; }
        public string CreationDate { get; set; }
        public string Currency { get; set; }
        public Customer Customer { get; set; }
        public List<Customerpayment> CustomerPayments { get; set; }
        public string ExternalBookingReference { get; set; }
        public Extranetuser ExtranetUser { get; set; }
        public Harbour Harbour { get; set; }
        public int? BookingId { get; set; }
        public Invoice Invoice { get; set; }
        public string Language { get; set; }
        public string PaymentType { get; set; }
        public string QrCodeUrl { get; set; }
        public List<Routebooking> RouteBookings { get; set; }
        public Vendor Seller { get; set; }
        public string Status { get; set; }
        public Systemconfig SystemConfig { get; set; }
        public decimal? TotalDue { get; set; }
        public decimal? TotalPaId { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPriceConverted { get; set; }
        public Vessel Vessel { get; set; }
    }

    public class Affiliate
    {
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string IdNumber { get; set; }
        public string Title { get; set; }
        public string TrackingCode { get; set; }
    }

    public class Agent
    {
        public int? Id { get; set; }
        public string IdNumber { get; set; }
        public List<Linkedexternalcustomer> LinkedExternalCustomers { get; set; }
        public string ReferenceCode { get; set; }
        public string Title { get; set; }
    }

    public class Linkedexternalcustomer
    {
        public string ExternalCustomerId { get; set; }
        public string ExternalCustomerTitle { get; set; }
        public string ExternalDepartmentId { get; set; }
        public List<string> Flags { get; set; }
        public int? SystemConfigId { get; set; }
        public string SystemType { get; set; }
    }

    public class Bookingchannel
    {
        public bool? Backend { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public bool? OverrideVoucherHeader { get; set; }
        public string Title { get; set; }
        public string UuId { get; set; }
        public string VoucherEmailAddress { get; set; }
        public Voucherlogo VoucherLogo { get; set; }
        public string VoucherLogoStyle { get; set; }
        public string VoucherName { get; set; }
        public string VoucherPhoneNumber { get; set; }
        public string VoucherWebsite { get; set; }
    }

    public class Voucherlogo
    {
        public string AlternateText { get; set; }
        public List<Derived> Derived { get; set; }
        public string Description { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string OriginalUrl { get; set; }
    }

    public class Derived
    {
        public string CleanUrl { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Customer
    {
        public string Address { get; set; }
        public bool? ContactDetailsHidden { get; set; }
        public string ContactDetailsHiddenUntil { get; set; }
        public string Country { get; set; }
        public string Created { get; set; }
        public Credentials Credentials { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int? Id { get; set; }
        public string Language { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string Organization { get; set; }
        public string PassportExpMonth { get; set; }
        public string PassportExpYear { get; set; }
        public string PassportId { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string Place { get; set; }
        public string PostCode { get; set; }
        public string Sex { get; set; }
        public string State { get; set; }
        public string UuId { get; set; }
    }

    public class Credentials
    {
        public string Username { get; set; }
    }

    public class Extranetuser
    {
        public Agent Agent { get; set; }
        public string CreationDate { get; set; }
        public string FirstName { get; set; }
        public string FullNameShort { get; set; }
        public int? Id { get; set; }
        public string LastLoginDate { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string TermsOfServiceAgreedDate { get; set; }
        public string Username { get; set; }
        public List<string> VendorRoles { get; set; }
    }

    public class Harbour
    {
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Pickupplace PickupPlace { get; set; }
        public int? ProductListId { get; set; }
        public string Title { get; set; }
    }

    public class Pickupplace
    {
        public bool? AskForRoomNumber { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Location Location { get; set; }
        public string Title { get; set; }
    }

    public class Location
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string PostCode { get; set; }
        public int? ZoomLevel { get; set; }
    }

    public class Invoice
    {
        public string Currency { get; set; }
        public List<Customlineitem> CustomLineItems { get; set; }
        public List<Appliedtax> ExcludedAppliedTaxes { get; set; }
        public bool? ExcludedTaxes { get; set; }
        public bool? Free { get; set; }
        public int? Id { get; set; }
        public List<Appliedtax> IncludedAppliedTaxes { get; set; }
        public bool? IncludedTaxes { get; set; }
        public string InvoiceNumber { get; set; }
        public string IssueDate { get; set; }
        public Issuer Issuer { get; set; }
        public string IssuerName { get; set; }
        public List<Appliedtax> LodgingTaxes { get; set; }
        public Money PaidAmountAsMoney { get; set; }
        public string PaidAmountAsText { get; set; }
        public List<Productinvoice> ProductInvoices { get; set; }
        public Recipient Recipient { get; set; }
        public string RecipientName { get; set; }
        public Money RemainingAmountAsMoney { get; set; }
        public string RemainingAmountAsText { get; set; }
        public Money TotalAsMoney { get; set; }
        public string TotalAsText { get; set; }
        public Money TotalDiscountAsMoney { get; set; }
        public Money TotalDiscountedAsMoney { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public Money TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public Money TotalExcludedTaxAsMoney { get; set; }
        public string TotalExcludedTaxAsText { get; set; }
        public Money TotalIncludedTaxAsMoney { get; set; }
        public string TotalIncludedTaxAsText { get; set; }
        public Money TotalTaxAsMoney { get; set; }
        public string TotalTaxAsText { get; set; }
    }

    public class Issuer
    {
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string Title { get; set; }
    }

    public class Money
    {
        public decimal? Amount { get; set; }
        public decimal? AmountMajor { get; set; }
        public int? AmountMajorInt { get; set; }
        public int? AmountMajorLong { get; set; }
        public decimal? AmountMinor { get; set; }
        public int? AmountMinorInt { get; set; }
        public int? AmountMinorLong { get; set; }
        public Currencyunit CurrencyUnit { get; set; }
        public int? MinorPart { get; set; }
        public bool? Negative { get; set; }
        public bool? NegativeOrZero { get; set; }
        public bool? Positive { get; set; }
        public bool? PositiveOrZero { get; set; }
        public int? Scale { get; set; }
        public bool? Zero { get; set; }
    }

    public class Currencyunit
    {
        public string Code { get; set; }
        public List<string> CountryCodes { get; set; }
        public string CurrencyCode { get; set; }
        public int? DecimalPlaces { get; set; }
        public int? DefaultFractionDigits { get; set; }
        public string Numeric3Code { get; set; }
        public int? NumericCode { get; set; }
        public bool? PseudoCurrency { get; set; }
        public string Symbol { get; set; }
    }

    public class Recipient
    {
        public string Address { get; set; }
        public bool? ContactDetailsHidden { get; set; }
        public string ContactDetailsHiddenUntil { get; set; }
        public string Country { get; set; }
        public string Created { get; set; }
        public Credentials Credentials { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int? Id { get; set; }
        public string Language { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string Organization { get; set; }
        public string PassportExpMonth { get; set; }
        public string PassportExpYear { get; set; }
        public string PassportId { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string Place { get; set; }
        public string PostCode { get; set; }
        public string Sex { get; set; }
        public string State { get; set; }
        public string UuId { get; set; }
    }

    public class Customlineitem
    {
        public decimal? CalculatedDiscount { get; set; }
        public string Currency { get; set; }
        public decimal? CustomDiscount { get; set; }
        public decimal? Discount { get; set; }
        public int? Id { get; set; }
        public string LineItemType { get; set; }
        public int? Quantity { get; set; }
        public Tax Tax { get; set; }
        public decimal? TaxAmount { get; set; }
        public Money TaxAsMoney { get; set; }
        public string TaxAsText { get; set; }
        public string Title { get; set; }
        public decimal? Total { get; set; }
        public Money TotalAsMoney { get; set; }
        public string TotalAsText { get; set; }
        public decimal? TotalDiscounted { get; set; }
        public Money TotalDiscountedAsMoney { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public decimal? TotalDue { get; set; }
        public Money TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public decimal? UnitPrice { get; set; }
        public Money UnitPriceAsMoney { get; set; }
        public string UnitPriceAsText { get; set; }
        public string UnitPriceDate { get; set; }
    }

    public class Tax
    {
        public int? Id { get; set; }
        public bool? Included { get; set; }
        public int? Percentage { get; set; }
        public string Title { get; set; }
    }

    public class Appliedtax
    {
        public string Currency { get; set; }
        public decimal? Tax { get; set; }
        public Money TaxAsMoney { get; set; }
        public string TaxAsText { get; set; }
        public string Title { get; set; }
    }

    public class Productinvoice
    {
        public string Currency { get; set; }
        public List<Customlineitem> CustomLineItems { get; set; }
        public string Dates { get; set; }
        public List<Appliedtax> ExcludedAppliedTaxes { get; set; }
        public bool? ExcludedTaxes { get; set; }
        public bool? Free { get; set; }
        public int? Id { get; set; }
        public List<Appliedtax> IncludedAppliedTaxes { get; set; }
        public bool? IncludedTaxes { get; set; }
        public string IssueDate { get; set; }
        public List<Lineitem> LineItems { get; set; }
        public Appliedtax LodgingTax { get; set; }
        public Product Product { get; set; }
        public int? ProductBookingId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductConfirmationCode { get; set; }
        public Money TotalAsMoney { get; set; }
        public string TotalAsText { get; set; }
        public Money TotalDiscountAsMoney { get; set; }
        public Money TotalDiscountedAsMoney { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public Money TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public Money TotalExcludedTaxAsMoney { get; set; }
        public string TotalExcludedTaxAsText { get; set; }
        public Money TotalIncludedTaxAsMoney { get; set; }
        public string TotalIncludedTaxAsText { get; set; }
        public Money TotalTaxAsMoney { get; set; }
        public string TotalTaxAsText { get; set; }
    }

    public class Product
    {
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string Title { get; set; }
    }

    public class Lineitem
    {
        public decimal? CalculatedDiscount { get; set; }
        public string Currency { get; set; }
        public decimal? CustomDiscount { get; set; }
        public decimal? Discount { get; set; }
        public int? Id { get; set; }
        public string ItemBookingId { get; set; }
        public int? Quantity { get; set; }
        public Tax Tax { get; set; }
        public decimal? TaxAmount { get; set; }
        public Money TaxAsMoney { get; set; }
        public string TaxAsText { get; set; }
        public string Title { get; set; }
        public decimal? Total { get; set; }
        public Money TotalAsMoney { get; set; }
        public string TotalAsText { get; set; }
        public decimal? TotalDiscounted { get; set; }
        public Money TotalDiscountedAsMoney { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public decimal? TotalDue { get; set; }
        public Money TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public decimal? UnitPrice { get; set; }
        public Money UnitPriceAsMoney { get; set; }
        public string UnitPriceAsText { get; set; }
        public string UnitPriceDate { get; set; }
    }

    public class Logo
    {
        public string AlternateText { get; set; }
        public List<Derived> Derived { get; set; }
        public string Description { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string OriginalUrl { get; set; }
    }

    public class Systemconfig
    {
        public int? Id { get; set; }
        public string SystemType { get; set; }
        public string Title { get; set; }
    }

    public class Vessel
    {
        public Affiliate Affiliate { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public List<Stopover> Stopovers { get; set; }
        public string Title { get; set; }
    }

    public class Stopover
    {
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public int? HarbourId { get; set; }
        public int? Id { get; set; }
    }

    public class Accommodationbooking
    {
        public Accommodation Accommodation { get; set; }
        public decimal? AffiliateCommission { get; set; }
        public Agent Agent { get; set; }
        public decimal? AgentCommission { get; set; }
        public Allotment Allotment { get; set; }
        public List<Answer> Answers { get; set; }
        public Barcode Barcode { get; set; }
        public bool? BoxBooking { get; set; }
        public Boxproduct BoxProduct { get; set; }
        public string CancelNote { get; set; }
        public string CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public string ConfirmationCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string EndDate { get; set; }
        public List<Linkstoexternalproduct> LinksToExternalProducts { get; set; }
        public string FirstStartDate { get; set; }
        public List<string> SupplierContractFlags { get; set; }
        public List<string> SellerContractFlags { get; set; }
        public int? Id { get; set; }
        public bool? IncludedOnCustomerInvoice { get; set; }
        public Invoice Invoice { get; set; }
        public string LastEndDate { get; set; }
        public Appliedtax LodgingTax { get; set; }
        public List<Note> Notes { get; set; }
        public string PaIdType { get; set; }
        public GetBookingRs ParentBooking { get; set; }
        public int? ParentBookingId { get; set; }
        public decimal? PriceWithDiscount { get; set; }
        public Product Product { get; set; }
        public string ProductConfirmationCode { get; set; }
        public List<Room> Rooms { get; set; }
        public decimal? SavedAmount { get; set; }
        public Vendor Seller { get; set; }
        public decimal? SellerCommission { get; set; }
        public string SortDate { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
        public Supplier Supplier { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class Accommodation
    {
        public Vendor ActualVendor { get; set; }
        public int? ActualId { get; set; }
        public string BaseLanguage { get; set; }
        public List<Bookableextra> BookableExtras { get; set; }
        public bool? Box { get; set; }
        public int? BoxedAccommodationId { get; set; }
        public Vendor BoxedVendor { get; set; }
        public List<Category> Categories { get; set; }
        public int? CheckInHour { get; set; }
        public int? CheckInMinute { get; set; }
        public int? CheckOutHour { get; set; }
        public int? CheckOutMinute { get; set; }
        public List<string> PaymentCurrencies { get; set; }
        public List<Customfield> CustomFields { get; set; }
        public string Description { get; set; }
        public string Excerpt { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Photo KeyPhoto { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Languages { get; set; }
        public string LastPublished { get; set; }
        public Location Location { get; set; }
        public List<Photo> Photos { get; set; }
        public string ProductCategory { get; set; }
        public int? ProductGroupId { get; set; }
        public int? Rating { get; set; }
        public List<Roomtype> RoomTypes { get; set; }
        public string Slug { get; set; }
        public List<Taggroup> TagGroups { get; set; }
        public string Title { get; set; }
        public Vendor Vendor { get; set; }
        public List<Video> Videos { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
        public bool? StoredExternally { get; set; }
    }

    public class Vendor
    {
        public string CurrencyCode { get; set; }
        public string EmailAddress { get; set; }
        public int? Id { get; set; }
        public string InvoiceIdNumber { get; set; }
        public Logo Logo { get; set; }
        public string LogoStyle { get; set; }
        public string PhoneNumber { get; set; }
        public bool? ShowAgentDetailsOnTicket { get; set; }
        public bool? ShowInvoiceIdOnTicket { get; set; }
        public bool? ShowPaymentsOnInvoice { get; set; }
        public string Title { get; set; }
        public string Website { get; set; }
    }

    public class Cancellationpolicy
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public List<Penaltyrule> PenaltyRules { get; set; }
        public Tax Tax { get; set; }
    }

    public class Penaltyrule
    {
        public int? Id { get; set; }
        public int? CutoffHours { get; set; }
        public int? Charge { get; set; }
        public string ChargeType { get; set; }
    }

    public class Bookableextra
    {
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public bool? Free { get; set; }
        public int? Id { get; set; }
        public bool? Included { get; set; }
        public bool? IncreasesCapacity { get; set; }
        public string Information { get; set; }
        public int? MaxPerBooking { get; set; }
        public decimal? Price { get; set; }
        public string PricingType { get; set; }
        public string PricingTypeLabel { get; set; }
        public List<Question> Questions { get; set; }
        public string Title { get; set; }
    }

    public class Question
    {
        public bool? Active { get; set; }
        public bool? AnswerRequired { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string Label { get; set; }
        public string Options { get; set; }
        public string Type { get; set; }
    }

    public class Category
    {
        public bool? AllowsSelectingMultipleChildren { get; set; }
        public List<Category> Categories { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string Title { get; set; }
    }

    public class Customfield
    {
        public List<string> Flags { get; set; }
    }

    public class Photo
    {
        public string AlternateText { get; set; }
        public List<Derived> Derived { get; set; }
        public string Description { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string OriginalUrl { get; set; }
    }

    public class Roomtype
    {
        public int? AccommodationId { get; set; }
        public string AccommodationTitle { get; set; }
        public int? BunkBedCount { get; set; }
        public int? Capacity { get; set; }
        public List<Category> Categories { get; set; }
        public string Code { get; set; }
        public int? DefaultRateId { get; set; }
        public string Description { get; set; }
        public int? DoubleBedCount { get; set; }
        public string Excerpt { get; set; }
        public string ExternalId { get; set; }
        public int? ExtraLargeDoubleBedCount { get; set; }
        public List<Extra> Extras { get; set; }
        public List<string> Flags { get; set; }
        public int? FutonMatCount { get; set; }
        public int? Id { get; set; }
        public bool? InternalUseOnly { get; set; }
        public Photo KeyPhoto { get; set; }
        public int? LargeDoubleBedCount { get; set; }
        public List<Photo> Photos { get; set; }
        public int? RoomCount { get; set; }
        public List<Roomrate> RoomRates { get; set; }
        public bool? Shared { get; set; }
        public int? SingleBedCount { get; set; }
        public int? SofaBedCount { get; set; }
        public List<Tag> Tags { get; set; }
        public string Title { get; set; }
        public int? VendorId { get; set; }
    }

    public class Extra
    {
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public bool? Free { get; set; }
        public int? Id { get; set; }
        public bool? Included { get; set; }
        public bool? IncreasesCapacity { get; set; }
        public string Information { get; set; }
        public int? MaxPerBooking { get; set; }
        public decimal? Price { get; set; }
        public string PricingType { get; set; }
        public string PricingTypeLabel { get; set; }
        public List<Question> Questions { get; set; }
        public string Title { get; set; }
    }

    public class Roomrate
    {
        public int? Id { get; set; }
        public int? MaxNightsStay { get; set; }
        public int? MaxOccupants { get; set; }
        public int? MinNightsStay { get; set; }
        public bool? StayRestrictions { get; set; }
        public bool? StayRestrictionsAllMonths { get; set; }
        public bool? StayRestrictionsAllWeekdays { get; set; }
        public List<int> StayRestrictionsMonths { get; set; }
        public List<int> StayRestrictionsWeekdays { get; set; }
        public string Title { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
    }

    public class Tag
    {
        public bool? Exclusive { get; set; }
        public string FacetName { get; set; }
        public List<string> Flags { get; set; }
        public int? GroupId { get; set; }
        public int? Id { get; set; }
        public int? OwnerId { get; set; }
        public int? ParentTagId { get; set; }
        public string Title { get; set; }
    }

    public class Taggroup
    {
        public bool? Exclusive { get; set; }
        public string ExternalId { get; set; }
        public string FacetName { get; set; }
        public List<string> Flags { get; set; }
        public bool? Group { get; set; }
        public int? Id { get; set; }
        public int? OwnerId { get; set; }
        public string ProductCategory { get; set; }
        public bool? SharedWithSuppliers { get; set; }
        public string SubCategory { get; set; }
        public List<Tag> Tags { get; set; }
        public string Title { get; set; }
    }

    public class Video
    {
        public string CleanPreviewUrl { get; set; }
        public string CleanThumbnailUrl { get; set; }
        public string Html { get; set; }
        public int? Id { get; set; }
        public string PreviewUrl { get; set; }
        public string ProviderName { get; set; }
        public string SourceUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class Allotment
    {
        public int? Id { get; set; }
        public string Title { get; set; }
    }

    public class Barcode
    {
        public string Value { get; set; }
        public string OfflineCode { get; set; }
        public string BarcodeType { get; set; }
    }

    public class Boxproduct
    {
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Photo KeyPhoto { get; set; }
        public decimal? Price { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public Vendor Vendor { get; set; }
    }

    public class Supplier
    {
        public string CurrencyCode { get; set; }
        public string EmailAddress { get; set; }
        public int? Id { get; set; }
        public string InvoiceIdNumber { get; set; }
        public List<Linkedexternalcustomer> LinkedExternalCustomers { get; set; }
        public Logo Logo { get; set; }
        public string LogoStyle { get; set; }
        public string PhoneNumber { get; set; }
        public bool? ShowAgentDetailsOnTicket { get; set; }
        public bool? ShowInvoiceIdOnTicket { get; set; }
        public bool? ShowPaymentsOnInvoice { get; set; }
        public string Title { get; set; }
        public string Website { get; set; }
    }

    public class Answer
    {
        public string Answers { get; set; }
        public string Group { get; set; }
        public int? Id { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
    }

    public class Linkstoexternalproduct
    {
        public string ExternalProductId { get; set; }
        public string ExternalProductTitle { get; set; }
        public List<string> Flags { get; set; }
        public int? SystemConfigId { get; set; }
    }

    public class Note
    {
        public string Author { get; set; }
        public string Body { get; set; }
        public string Created { get; set; }
        public int? OwnerId { get; set; }
        public string Recipient { get; set; }
        public bool? SentAsEmail { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public bool? VoucherAttached { get; set; }
        public bool? VoucherPricesShown { get; set; }
    }

    public class Room
    {
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public string EndDate { get; set; }
        public List<Extra> Extras { get; set; }
        public List<Bookingfield> BookingFields { get; set; }
        public List<Guest> Guests { get; set; }
        public int? Id { get; set; }
        public int? Infants { get; set; }
        public int? NightCount { get; set; }
        public Roomrate RoomRate { get; set; }
        public Roomtype RoomType { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
    }

    public class BookingAnswerGroup
    {
        public List<Answer> Answers { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public List<Qanda> QandA { get; set; }
        public List<Questionsandanswer> QuestionsAndAnswers { get; set; }
    }

    public class Qanda
    {
        public Answer Answer { get; set; }
        public string AnswerAsString { get; set; }
        public Question Question { get; set; }
        public string QuestionAsString { get; set; }
    }

    public class Questionsandanswer
    {
        public Answer Answer { get; set; }
        public string AnswerAsString { get; set; }
        public Question Question { get; set; }
        public string QuestionAsString { get; set; }
    }

    public class Bookingfield
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Guest
    {
        public string Address { get; set; }
        public bool? ContactDetailsHidden { get; set; }
        public string ContactDetailsHiddenUntil { get; set; }
        public string Country { get; set; }
        public string Created { get; set; }
        public Credentials Credentials { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int? Id { get; set; }
        public string Language { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string Organization { get; set; }
        public string PassportExpMonth { get; set; }
        public string PassportExpYear { get; set; }
        public string PassportId { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string Place { get; set; }
        public string PostCode { get; set; }
        public string Sex { get; set; }
        public string State { get; set; }
        public string UuId { get; set; }
    }

    public class Activitybooking
    {
        public Activity Activity { get; set; }
        public decimal? AffiliateCommission { get; set; }
        public Agent Agent { get; set; }
        public decimal? AgentCommission { get; set; }
        public List<Answer> Answers { get; set; }
        public Barcode Barcode { get; set; }
        public string BookedDropoffPricingType { get; set; }
        public string BookedPickupPricingType { get; set; }
        public List<Pricingcategory> BookedPricingCategories { get; set; }
        public bool? BoxBooking { get; set; }
        public Boxproduct BoxProduct { get; set; }
        public string CancelNote { get; set; }
        public string CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public List<Combochildbooking> ComboChildBookings { get; set; }
        public Comboparentbooking ComboParentBooking { get; set; }
        public string ConfirmationCode { get; set; }
        public bool? Customized { get; set; }
        public string Date { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? Dropoff { get; set; }
        public Dropoffplace DropoffPlace { get; set; }
        public string DropoffPlaceDescription { get; set; }
        public List<Linkstoexternalproduct> LinksToExternalProducts { get; set; }
        public List<Extra> Extras { get; set; }
        public List<Bookingfield> BookingFields { get; set; }
        public List<string> SupplierContractFlags { get; set; }
        public List<string> SellerContractFlags { get; set; }
        public bool? Flexible { get; set; }
        public int? Id { get; set; }
        public bool? IncludedOnCustomerInvoice { get; set; }
        public Invoice Invoice { get; set; }
        public List<Note> Notes { get; set; }
        public string PaidType { get; set; }
        public GetBookingRs ParentBooking { get; set; }
        public int? ParentBookingId { get; set; }
        public bool? Pickup { get; set; }
        public Pickupplace PickupPlace { get; set; }
        public string PickupPlaceDescription { get; set; }
        public string PickupPlaceRoomNumber { get; set; }
        public string PickupTime { get; set; }
        public decimal? PriceWithDiscount { get; set; }
        public List<Pricingcategorybooking> PricingCategoryBookings { get; set; }
        public Product Product { get; set; }
        public string ProductConfirmationCode { get; set; }
        public Quantitybypricingcategory QuantityByPricingCategory { get; set; }
        public decimal? SavedAmount { get; set; }
        public string SelectedFlexDayOption { get; set; }
        public Vendor Seller { get; set; }
        public decimal? SellerCommission { get; set; }
        public string StartTime { get; set; }
        public int? StartTimeId { get; set; }
        public string Status { get; set; }
        public Supplier Supplier { get; set; }
        public int? TotalParticipants { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class Activity
    {
        public string ActivityType { get; set; }
        public string BookingType { get; set; }
        public string ScheduleType { get; set; }
        public string CapacityType { get; set; }
        public string PassExpiryType { get; set; }
        public string MeetingType { get; set; }
        public int? PassCapacity { get; set; }
        public int? PassAvailable { get; set; }
        public bool? DressCode { get; set; }
        public bool? PassportRequired { get; set; }
        public bool? TicketPerPerson { get; set; }
        public List<string> SupportedAccessibilityTypes { get; set; }
        public List<Startpoint> StartPoints { get; set; }
        public List<Guidancetype> GuidanceTypes { get; set; }
        public List<string> ActivityCategories { get; set; }
        public List<string> ActivityAttributes { get; set; }
        public Vendor ActualVendor { get; set; }
        public int? ActualId { get; set; }
        public List<Agendaitem> AgendaItems { get; set; }
        public string Attention { get; set; }
        public string BaseLanguage { get; set; }
        public List<Bookableextra> BookableExtras { get; set; }
        public int? BookingCutoffDays { get; set; }
        public int? BookingCutoffHours { get; set; }
        public int? BookingCutoffMinutes { get; set; }
        public int? BookingCutoffWeeks { get; set; }
        public bool? Box { get; set; }
        public int? BoxedActivityId { get; set; }
        public Vendor BoxedVendor { get; set; }
        public List<Category> Categories { get; set; }
        public bool? ComboActivity { get; set; }
        public List<string> PaymentCurrencies { get; set; }
        public bool? CustomDropoffAllowed { get; set; }
        public List<Customfield> CustomFields { get; set; }
        public bool? CustomPickupAllowed { get; set; }
        public bool? DayBasedAvailability { get; set; }
        public List<string> DayOptions { get; set; }
        public string Description { get; set; }
        public string DifficultyLevel { get; set; }
        public List<string> DropoffFlags { get; set; }
        public string DropoffPricingType { get; set; }
        public bool? DropoffService { get; set; }
        public int? Duration { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }
        public string DurationType { get; set; }
        public int? DurationWeeks { get; set; }
        public string Excerpt { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string Included { get; set; }
        public Photo KeyPhoto { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Languages { get; set; }
        public string LastPublished { get; set; }
        public Locationcode LocationCode { get; set; }
        public int? MinAge { get; set; }
        public decimal? NextDefaultPrice { get; set; }
        public string NoPickupMsg { get; set; }
        public List<Photo> Photos { get; set; }
        public bool? PickupAllotment { get; set; }
        public List<string> PickupFlags { get; set; }
        public int? PickupMinutesBefore { get; set; }
        public string PickupPricingType { get; set; }
        public bool? PickupService { get; set; }
        public List<Pricingcategory> PricingCategories { get; set; }
        public string ProductCategory { get; set; }
        public int? ProductGroupId { get; set; }
        public string Requirements { get; set; }
        public Route Route { get; set; }
        public bool? SeasonAllYear { get; set; }
        public int? SeasonEndDate { get; set; }
        public int? SeasonEndMonth { get; set; }
        public int? SeasonStartDate { get; set; }
        public int? SeasonStartMonth { get; set; }
        public bool? SelectFromDayOptions { get; set; }
        public bool? ShowGlobalPickupMsg { get; set; }
        public bool? ShowNoPickupMsg { get; set; }
        public string Slug { get; set; }
        public List<Starttime> StartTimes { get; set; }
        public List<Taggroup> TagGroups { get; set; }
        public string Title { get; set; }
        public bool? UseComponentPickupAllotments { get; set; }
        public Vendor Vendor { get; set; }
        public List<Video> Videos { get; set; }
        public List<Weekday> Weekdays { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
        public bool? StoredExternally { get; set; }
        public int? DefaultRateId { get; set; }
        public List<Rate> Rates { get; set; }
        public bool? HasOpeningHours { get; set; }
        public Defaultopeninghours DefaultOpeningHours { get; set; }
        public List<Seasonalopeninghour> SeasonalOpeningHours { get; set; }
    }

    public class Locationcode
    {
        public string Coordinates { get; set; }
        public string Country { get; set; }
        public int? Date { get; set; }
        public string Function { get; set; }
        public string Iata { get; set; }
        public int? Id { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string NameWoDiacritics { get; set; }
        public string RecentChange { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string Subdivision { get; set; }
    }

    public class Route
    {
        public Waypoint Center { get; set; }
        public Waypoint End { get; set; }
        public int? MapZoomLevel { get; set; }
        public bool? SameStartEnd { get; set; }
        public Waypoint Start { get; set; }
        public List<Waypoint> Waypoints { get; set; }
    }

    public class Waypoint
    {
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }

    public class Defaultopeninghours
    {
        public int? Id { get; set; }
        public OpeningHoursWeekday Monday { get; set; }
        public OpeningHoursWeekday Tuesday { get; set; }
        public OpeningHoursWeekday Wednesday { get; set; }
        public OpeningHoursWeekday Thursday { get; set; }
        public OpeningHoursWeekday Friday { get; set; }
        public OpeningHoursWeekday Saturday { get; set; }
        public OpeningHoursWeekday Sunday { get; set; }
    }

    public class OpeningHoursWeekday
    {
        public int? Id { get; set; }
        public bool? Open24Hours { get; set; }
        public List<Timeinterval> TimeIntervals { get; set; }
    }

    public class Timeinterval
    {
        public List<int> OpenFrom { get; set; }
        public int? OpenForHours { get; set; }
        public int? OpenForMinutes { get; set; }
        public Frequency Frequency { get; set; }
    }

    public class Frequency
    {
        public int? Minutes { get; set; }
        public int? Hours { get; set; }
        public int? Days { get; set; }
        public int? Weeks { get; set; }
    }

    public class Startpoint
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public List<string> Labels { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public int? Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public int? MapZoomLevel { get; set; }
        public Geopoint GeoPoint { get; set; }
        public Unlocode UnLocode { get; set; }
    }

    public class Geopoint
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class Unlocode
    {
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class Guidancetype
    {
        public int? Id { get; set; }
        public string GuidanceType { get; set; }
        public List<string> Languages { get; set; }
    }

    public class Agendaitem
    {
        public string Body { get; set; }
        public string Excerpt { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public int? Index { get; set; }
        public Photo KeyPhoto { get; set; }
        public List<Photo> Photos { get; set; }
        public Place Place { get; set; }
        public bool? PossibleStartPoint { get; set; }
        public string Title { get; set; }
    }

    public class Place
    {
        public int? Id { get; set; }
        public Location Location { get; set; }
        public string Title { get; set; }
    }

    public class Pricingcategory
    {
        public bool? AgeQualified { get; set; }
        public bool? DefaultCategory { get; set; }
        public bool? Dependent { get; set; }
        public List<string> Flags { get; set; }
        public string FullTitle { get; set; }
        public int? Id { get; set; }
        public bool? InternalUseOnly { get; set; }
        public int? MasterCategoryId { get; set; }
        public int? MaxAge { get; set; }
        public int? MaxDependentSum { get; set; }
        public int? MaxPerMaster { get; set; }
        public int? MinAge { get; set; }
        public bool? SumDependentCategories { get; set; }
        public string Title { get; set; }
    }

    public class Starttime
    {
        public int? Duration { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }
        public string DurationType { get; set; }
        public int? DurationWeeks { get; set; }
        public List<string> Flags { get; set; }
        public int? Hour { get; set; }
        public int? Id { get; set; }
        public int? Minute { get; set; }
        public bool? OverrideTimeWhenPickup { get; set; }
        public int? PickupHour { get; set; }
        public int? PickupMinute { get; set; }
    }

    public class Weekday
    {
        public int? Index { get; set; }
        public string Name { get; set; }
    }

    public class Rate
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Index { get; set; }
        public string RateCode { get; set; }
        public bool? PricedPerPerson { get; set; }
        public int? MinPerBooking { get; set; }
        public int? MaxPerBooking { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
        public string PickupSelectionType { get; set; }
        public string PickupPricingType { get; set; }
        public bool? PickupPricedPerPerson { get; set; }
        public string DropoffSelectionType { get; set; }
        public string DropoffPricingType { get; set; }
        public bool? DropoffPricedPerPerson { get; set; }
        public List<int> StartTimeIds { get; set; }
        public List<int> PricingCategoryIds { get; set; }
        public List<Extraconfig> ExtraConfigs { get; set; }
    }

    public class Extraconfig
    {
        public int? Id { get; set; }
        public int? ActivityExtraId { get; set; }
        public string SelectionType { get; set; }
        public string PricingType { get; set; }
    }

    public class Seasonalopeninghour
    {
        public int? Id { get; set; }
        public OpeningHoursWeekday Monday { get; set; }
        public OpeningHoursWeekday Tuesday { get; set; }
        public OpeningHoursWeekday Wednesday { get; set; }
        public OpeningHoursWeekday Thursday { get; set; }
        public OpeningHoursWeekday Friday { get; set; }
        public OpeningHoursWeekday Saturday { get; set; }
        public OpeningHoursWeekday Sunday { get; set; }
        public int? StartMonth { get; set; }
        public int? StartDay { get; set; }
        public int? EndMonth { get; set; }
        public int? EndDay { get; set; }
    }

    public class Comboparentbooking
    {
        public int? Id { get; set; }
        public Product Product { get; set; }
        public string ProductConfirmationCode { get; set; }
    }

    public class Dropoffplace
    {
        public bool? AskForRoomNumber { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Location Location { get; set; }
        public string Title { get; set; }
    }

    public class Quantitybypricingcategory
    {
    }

    public class Combochildbooking
    {
        public int? Id { get; set; }
        public Product Product { get; set; }
        public string ProductConfirmationCode { get; set; }
    }

    public class Pricingcategorybooking
    {
        public List<Answer> Answers { get; set; }
        public Barcode Barcode { get; set; }
        public int? BookedDropoffPrice { get; set; }
        public int? BookedPickupPrice { get; set; }
        public int? BookedPrice { get; set; }
        public string BookedTitle { get; set; }
        public List<Extra> Extras { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Pricingcategory PricingCategory { get; set; }
        public int? PricingCategoryId { get; set; }
    }

    public class Carrentalbooking
    {
        public decimal? AffiliateCommission { get; set; }
        public Agent Agent { get; set; }
        public decimal? AgentCommission { get; set; }
        public List<Answer> Answers { get; set; }
        public Barcode Barcode { get; set; }
        public bool? BoxBooking { get; set; }
        public Boxproduct BoxProduct { get; set; }
        public string CancelNote { get; set; }
        public string CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public List<Carbooking> CarBookings { get; set; }
        public Carrental CarRental { get; set; }
        public string ConfirmationCode { get; set; }
        public int? DayCount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public Dropofflocation DropoffLocation { get; set; }
        public string EndDate { get; set; }
        public List<Linkstoexternalproduct> LinksToExternalProducts { get; set; }
        public List<string> SupplierContractFlags { get; set; }
        public List<string> SellerContractFlags { get; set; }
        public int? Id { get; set; }
        public bool? IncludedOnCustomerInvoice { get; set; }
        public Invoice Invoice { get; set; }
        public List<Note> Notes { get; set; }
        public string PaidType { get; set; }
        public GetBookingRs ParentBooking { get; set; }
        public int? ParentBookingId { get; set; }
        public Pickuplocation PickupLocation { get; set; }
        public decimal? PriceWithDiscount { get; set; }
        public Product Product { get; set; }
        public string ProductConfirmationCode { get; set; }
        public decimal? SavedAmount { get; set; }
        public Vendor Seller { get; set; }
        public decimal? SellerCommission { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
        public Supplier Supplier { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class Carrental
    {
        public Vendor ActualVendor { get; set; }
        public string BaseLanguage { get; set; }
        public List<Bookableextra> BookableExtras { get; set; }
        public bool? Box { get; set; }
        public Vendor BoxedVendor { get; set; }
        public List<Cartype> CarTypes { get; set; }
        public List<Category> Categories { get; set; }
        public List<string> PaymentCurrencies { get; set; }
        public List<Customfield> CustomFields { get; set; }
        public int? DefaultDropoffHour { get; set; }
        public int? DefaultDropoffLocationId { get; set; }
        public int? DefaultDropoffMinute { get; set; }
        public int? DefaultPickupHour { get; set; }
        public int? DefaultPickupLocationId { get; set; }
        public int? DefaultPickupMinute { get; set; }
        public string Description { get; set; }
        public List<Dropofflocation> DropoffLocations { get; set; }
        public string Excerpt { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Photo KeyPhoto { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Languages { get; set; }
        public string LastPublished { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Pickuplocation> PickupLocations { get; set; }
        public string PricingMode { get; set; }
        public string ProductCategory { get; set; }
        public int? ProductGroupId { get; set; }
        public string Slug { get; set; }
        public List<Taggroup> TagGroups { get; set; }
        public string Title { get; set; }
        public Vendor Vendor { get; set; }
        public List<Video> Videos { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
        public bool? StoredExternally { get; set; }
    }

    public class Cartype
    {
        public string AcrissCategory { get; set; }
        public string AcrissCode { get; set; }
        public string AcrissType { get; set; }
        public bool? AirConditioning { get; set; }
        public int? AvgRentalPricePerDay { get; set; }
        public string BaseLanguage { get; set; }
        public int? BookingCutoff { get; set; }
        public Carrental CarRental { get; set; }
        public List<Category> Categories { get; set; }
        public int? Co2Emission { get; set; }
        public List<string> PaymentCurrencies { get; set; }
        public int? DefaultDropoffHour { get; set; }
        public int? DefaultDropoffLocationId { get; set; }
        public int? DefaultDropoffMinute { get; set; }
        public int? DefaultPickupHour { get; set; }
        public int? DefaultPickupLocationId { get; set; }
        public int? DefaultPickupMinute { get; set; }
        public int? DeliveryFee { get; set; }
        public string Description { get; set; }
        public int? DoorCount { get; set; }
        public string DriveType { get; set; }
        public List<Dropofflocation> DropoffLocations { get; set; }
        public string ExampleCarModel { get; set; }
        public string Excerpt { get; set; }
        public string ExternalId { get; set; }
        public List<Extra> Extras { get; set; }
        public List<string> Flags { get; set; }
        public int? FuelEconomy { get; set; }
        public string FuelType { get; set; }
        public int? Id { get; set; }
        public int? IncludedExtrasPrice { get; set; }
        public Photo KeyPhoto { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Languages { get; set; }
        public int? LuggageCapacity { get; set; }
        public int? MaxBookableCount { get; set; }
        public int? MaxRentalHours { get; set; }
        public int? MinDriverAge { get; set; }
        public int? MinRentalHours { get; set; }
        public int? PassengerCapacity { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Pickuplocation> PickupLocations { get; set; }
        public int? ProductGroupId { get; set; }
        public int? RentalPrice { get; set; }
        public bool? RentalRestrictions { get; set; }
        public string Summary { get; set; }
        public List<Taggroup> TagGroups { get; set; }
        public string Title { get; set; }
        public int? TotalPrice { get; set; }
        public string TransmissionType { get; set; }
        public Vendor Vendor { get; set; }
    }

    public class Dropofflocation
    {
        public bool? AllDay { get; set; }
        public int? ClosingHour { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Location Location { get; set; }
        public int? OpeningHour { get; set; }
        public int? PriceForDropoff { get; set; }
        public int? PriceForPickup { get; set; }
        public int? PriceForPickupAndDropoff { get; set; }
        public string Title { get; set; }
    }

    public class Pickuplocation
    {
        public bool? AllDay { get; set; }
        public int? ClosingHour { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Location Location { get; set; }
        public int? OpeningHour { get; set; }
        public int? PriceForDropoff { get; set; }
        public int? PriceForPickup { get; set; }
        public int? PriceForPickupAndDropoff { get; set; }
        public string Title { get; set; }
    }

    public class Carbooking
    {
        public List<Answer> Answers { get; set; }
        public Cartype CarType { get; set; }
        public int? DayCount { get; set; }
        public List<Extra> Extras { get; set; }
        public List<Bookingfield> BookingFields { get; set; }
        public int? Id { get; set; }
        public int? UnitCount { get; set; }
    }

    public class Customerpayment
    {
        public int? ActiveCustomerInvoiceId { get; set; }
        public decimal? Amount { get; set; }
        public Money AmountAsMoney { get; set; }
        public string AmountAsText { get; set; }
        public string AuthorizationCode { get; set; }
        public string CardNumber { get; set; }
        public string Comment { get; set; }
        public string Currency { get; set; }
        public int? Id { get; set; }
        public int? PaymentContractId { get; set; }
        public int? PaymentId { get; set; }
        public string PaymentProviderType { get; set; }
        public string PaymentReferenceId { get; set; }
        public string PaymentType { get; set; }
        public string TransactionDate { get; set; }
    }

    public class Routebooking
    {
        public decimal? AffiliateCommission { get; set; }
        public Agent Agent { get; set; }
        public decimal? AgentCommission { get; set; }
        public List<Answer> Answers { get; set; }
        public Barcode Barcode { get; set; }
        public bool? BoxBooking { get; set; }
        public Boxproduct BoxProduct { get; set; }
        public string CancelNote { get; set; }
        public string CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public string ConfirmationCode { get; set; }
        public string Date { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public List<Linkstoexternalproduct> LinksToExternalProducts { get; set; }
        public List<string> SupplierContractFlags { get; set; }
        public List<string> SellerContractFlags { get; set; }
        public bool? Flexible { get; set; }
        public int? Id { get; set; }
        public bool? IncludedOnCustomerInvoice { get; set; }
        public Invoice Invoice { get; set; }
        public List<Legbooking> LegBookings { get; set; }
        public List<Note> Notes { get; set; }
        public string PaidType { get; set; }
        public GetBookingRs ParentBooking { get; set; }
        public int? ParentBookingId { get; set; }
        public List<Passengerbooking> PassengerBookings { get; set; }
        public List<Passengerspecification> PassengerSpecifications { get; set; }
        public decimal? PriceWithDiscount { get; set; }
        public Product Product { get; set; }
        public string ProductConfirmationCode { get; set; }
        public Routebooking ReturnBooking { get; set; }
        public Route Route { get; set; }
        public decimal? SavedAmount { get; set; }
        public Vendor Seller { get; set; }
        public decimal? SellerCommission { get; set; }
        public string Status { get; set; }
        public Supplier Supplier { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class Availabilityexpression
    {
        public List<Capacity> Capacities { get; set; }
        public string Comment { get; set; }
        public string Cron { get; set; }
        public bool? CronConditionActive { get; set; }
        public bool? DateRangeConditionActive { get; set; }
        public string EndDate { get; set; }
        public List<string> Flags { get; set; }
        public bool? FlagConditionActive { get; set; }
        public int? Id { get; set; }
        public bool? RecurringDateRange { get; set; }
        public int? RecurringEndDay { get; set; }
        public int? RecurringEndMonth { get; set; }
        public int? RecurringStartDay { get; set; }
        public int? RecurringStartMonth { get; set; }
        public string StartDate { get; set; }
    }

    public class Capacity
    {
        public string AvailabilityItemId { get; set; }
        public int? Capacitys { get; set; }
        public string CapacityType { get; set; }
        public int? Id { get; set; }
    }

    public class Closure
    {
        public List<Closeditem> ClosedItems { get; set; }
        public string Comment { get; set; }
        public string Cron { get; set; }
        public bool? CronConditionActive { get; set; }
        public bool? DateRangeConditionActive { get; set; }
        public string EndDate { get; set; }
        public List<string> Flags { get; set; }
        public bool? FlagConditionActive { get; set; }
        public int? Id { get; set; }
        public bool? RecurringDateRange { get; set; }
        public int? RecurringEndDay { get; set; }
        public int? RecurringEndMonth { get; set; }
        public int? RecurringStartDay { get; set; }
        public int? RecurringStartMonth { get; set; }
        public string StartDate { get; set; }
    }

    public class Closeditem
    {
        public int? Id { get; set; }
        public string Item { get; set; }
    }

    public class Leg
    {
        public int? Distance { get; set; }
        public string DistanceUnit { get; set; }
        public string ExternalId { get; set; }
        public List<string> Flags { get; set; }
        public bool? FlexibleSchedule { get; set; }
        public RouteStation From { get; set; }
        public int? Id { get; set; }
        public List<Schedule> Schedules { get; set; }
        public RouteStation To { get; set; }
    }

    public class Region
    {
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public string Title { get; set; }
    }

    public class RouteStation
    {
        public bool? AirportStation { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public Location Location { get; set; }
        public List<string> PickupFlags { get; set; }
        public int? PickupPlaceId { get; set; }
        public bool? PickupStation { get; set; }
        public List<Region> Regions { get; set; }
        public string ShortOrLongTitle { get; set; }
        public string ShortTitle { get; set; }
        public string Title { get; set; }
    }

    public class Schedule
    {
        public int? ArrivalHour { get; set; }
        public int? ArrivalMinute { get; set; }
        public string ArrivalTimeStr { get; set; }
        public int? DepartureHour { get; set; }
        public int? DepartureMinute { get; set; }
        public string DepartureTimeStr { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public bool? Peak { get; set; }
    }

    public class Legbooking
    {
        public string Date { get; set; }
        public Departure Departure { get; set; }
        public int? DropoffPlaceId { get; set; }
        public string DropoffPlaceTitle { get; set; }
        public int? Id { get; set; }
        public Leg Leg { get; set; }
        public bool? Peak { get; set; }
        public int? PickupPlaceId { get; set; }
        public string PickupPlaceTitle { get; set; }
    }

    public class Departure
    {
        public int? ArrivalHour { get; set; }
        public int? ArrivalMinute { get; set; }
        public string ArrivalTimeStr { get; set; }
        public int? DepartureHour { get; set; }
        public int? DepartureMinute { get; set; }
        public string DepartureTimeStr { get; set; }
        public List<string> Flags { get; set; }
        public int? Id { get; set; }
        public bool? Peak { get; set; }
    }

    public class Passengerbooking
    {
        public int? Id { get; set; }
        public Pricingcategory PricingCategory { get; set; }
        public Barcode Barcode { get; set; }
    }

    public class Passengerspecification
    {
        public int? PricePerPassenger { get; set; }
        public Pricingcategory PricingCategory { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}