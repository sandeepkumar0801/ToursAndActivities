using System;
using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.SubmitCheckout
{
    public class SubmitCheckoutRs
    {
        public Booking Booking { get; set; }
        public string BookingHash { get; set; }
        public TravelDocuments TravelDocuments { get; set; }
        public string message { get; set; }
        public Fields fields { get; set; }
    }

    public class Booking
    {
        public long? CreationDate { get; set; }
        public long? BookingId { get; set; }
        public string Language { get; set; }
        public string ConfirmationCode { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public long? TotalPrice { get; set; }
        public long? TotalPaid { get; set; }
        public long? TotalDue { get; set; }
        public long? TotalPriceConverted { get; set; }
        public Customer Customer { get; set; }
        public BookingInvoice Invoice { get; set; }
        public List<CustomerPayment> CustomerPayments { get; set; }
        public string PaymentType { get; set; }
        public Seller Seller { get; set; }
        public BookingChannel BookingChannel { get; set; }
        public List<object> AccommodationBookings { get; set; }
        public List<object> CarRentalBookings { get; set; }
        public List<ActivityBooking> ActivityBookings { get; set; }
        public List<object> RouteBookings { get; set; }
        public List<object> BookingFields { get; set; }
    }

    public class ActivityBooking
    {
        public long? BookingId { get; set; }
        public long? ParentBookingId { get; set; }
        public string ConfirmationCode { get; set; }
        public string ProductConfirmationCode { get; set; }
        public BokunBarcode Barcode { get; set; }
        public bool HasTicket { get; set; }
        public bool BoxBooking { get; set; }
        public long? StartDateTime { get; set; }
        public string Status { get; set; }
        public bool IncludedOnCustomerInvoice { get; set; }
        public string Title { get; set; }
        public long? TotalPrice { get; set; }
        public long? PriceWithDiscount { get; set; }
        public long? DiscountPercentage { get; set; }
        public long? DiscountAmount { get; set; }
        public string ProductCategory { get; set; }
        public string PaidType { get; set; }
        public Product Product { get; set; }
        public long? ProductId { get; set; }
        public List<object> LinksToExternalProducts { get; set; }
        public List<object> Answers { get; set; }
        public InvoiceElement Invoice { get; set; }
        public List<object> Notes { get; set; }
        public List<object> SupplierContractFlags { get; set; }
        public List<object> SellerContractFlags { get; set; }
        public ActivityBookingCancellationPolicy CancellationPolicy { get; set; }
        public List<string> BookingRoles { get; set; }
        public long? Date { get; set; }
        public string StartTime { get; set; }
        public long? StartTimeId { get; set; }
        public long? RateId { get; set; }
        public string RateTitle { get; set; }
        public Activity Activity { get; set; }
        public bool Flexible { get; set; }
        public bool Customized { get; set; }
        public bool TicketPerPerson { get; set; }
        public List<PricingCategoryBooking> PricingCategoryBookings { get; set; }
        public List<object> Extras { get; set; }
        public List<object> BookingFields { get; set; }
        public bool Pickup { get; set; }
        public bool Dropoff { get; set; }
        public long? TotalParticipants { get; set; }
        public List<PricingCategory> BookedPricingCategories { get; set; }
        public QuantityByPricingCategory QuantityByPricingCategory { get; set; }
        public long? SavedAmount { get; set; }
    }

    public class Activity
    {
        public long? Id { get; set; }
        public string ExternalId { get; set; }
        public object ProductGroupId { get; set; }
        public string ProductCategory { get; set; }
        public bool Box { get; set; }
        public bool InventoryLocal { get; set; }
        public bool InventorySupportsPricing { get; set; }
        public bool InventorySupportsAvailability { get; set; }
        public string LastPublished { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Excerpt { get; set; }
        public object CancellationPolicy { get; set; }
        public string BarcodeType { get; set; }
        public string TimeZone { get; set; }
        public List<object> MainContactFields { get; set; }
        public List<object> RequiredCustomerFields { get; set; }
        public List<object> Keywords { get; set; }
        public List<object> Flags { get; set; }
        public object Slug { get; set; }
        public string BaseLanguage { get; set; }
        public List<string> Languages { get; set; }
        public List<string> PaymentCurrencies { get; set; }
        public List<object> CustomFields { get; set; }
        public List<object> TagGroups { get; set; }
        public List<object> Categories { get; set; }
        public object KeyPhoto { get; set; }
        public List<object> Photos { get; set; }
        public List<object> Videos { get; set; }
        public Vendor Vendor { get; set; }
        public object BoxedVendor { get; set; }
        public bool StoredExternally { get; set; }
        public object PluginId { get; set; }
        public object ReviewRating { get; set; }
        public object ReviewCount { get; set; }
        public string ActivityType { get; set; }
        public string BookingType { get; set; }
        public string ScheduleType { get; set; }
        public string CapacityType { get; set; }
        public string PassExpiryType { get; set; }
        public string MeetingType { get; set; }
        public long? PassCapacity { get; set; }
        public long? PassesAvailable { get; set; }
        public bool DressCode { get; set; }
        public bool PassportRequired { get; set; }
        public List<object> SupportedAccessibilityTypes { get; set; }
        public List<StartPoint> StartPoints { get; set; }
        public string Included { get; set; }
        public string Requirements { get; set; }
        public string Attention { get; set; }
        public LocationCode LocationCode { get; set; }
        public long? BookingCutoffMinutes { get; set; }
        public long? BookingCutoffHours { get; set; }
        public long? BookingCutoffDays { get; set; }
        public long? BookingCutoffWeeks { get; set; }
        public long? RequestDeadlineMinutes { get; set; }
        public long? RequestDeadlineHours { get; set; }
        public long? RequestDeadlineDays { get; set; }
        public long? RequestDeadlineWeeks { get; set; }
        public object BoxedActivityId { get; set; }
        public bool ComboActivity { get; set; }
        public bool DayBasedAvailability { get; set; }
        public bool SelectFromDayOptions { get; set; }
        public List<object> DayOptions { get; set; }
        public List<object> ActivityCategories { get; set; }
        public List<object> ActivityAttributes { get; set; }
        public List<object> GuidanceTypes { get; set; }
        public long? DefaultRateId { get; set; }
        public List<Rate> Rates { get; set; }
        public bool TicketPerPerson { get; set; }
        public string DurationType { get; set; }
        public long? Duration { get; set; }
        public long? DurationMinutes { get; set; }
        public long? DurationHours { get; set; }
        public long? DurationDays { get; set; }
        public long? DurationWeeks { get; set; }
        public string DurationText { get; set; }
        public long? MinAge { get; set; }
        public object NextDefaultPrice { get; set; }
        public object NextDefaultPriceMoney { get; set; }
        public bool PickupService { get; set; }
        public bool PickupAllotment { get; set; }
        public bool UseComponentPickupAllotments { get; set; }
        public List<object> PickupFlags { get; set; }
        public bool CustomPickupAllowed { get; set; }
        public long? PickupMinutesBefore { get; set; }
        public string NoPickupMsg { get; set; }
        public bool ShowGlobalPickupMsg { get; set; }
        public bool ShowNoPickupMsg { get; set; }
        public bool DropoffService { get; set; }
        public List<object> DropoffFlags { get; set; }
        public bool CustomDropoffAllowed { get; set; }
        public string DifficultyLevel { get; set; }
        public List<PricingCategory> PricingCategories { get; set; }
        public List<object> AgendaItems { get; set; }
        public List<StartTime> StartTimes { get; set; }
        public List<object> BookableExtras { get; set; }
        public object Route { get; set; }
        public bool HasOpeningHours { get; set; }
        public object DefaultOpeningHours { get; set; }
        public List<object> SeasonalOpeningHours { get; set; }
        public DisplaySettings DisplaySettings { get; set; }
        public long? RequestDeadline { get; set; }
        public long? ActualId { get; set; }
        public long? BookingCutoff { get; set; }
        public Vendor ActualVendor { get; set; }
    }

    public class Vendor
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string CurrencyCode { get; set; }
        public bool ShowInvoiceIdOnTicket { get; set; }
        public bool ShowAgentDetailsOnTicket { get; set; }
        public bool ShowPaymentsOnInvoice { get; set; }
    }

    public class DisplaySettings
    {
        public bool ShowPickupPlaces { get; set; }
        public bool ShowRouteMap { get; set; }
        public bool SelectRateBasedOnStartTime { get; set; }
        public List<object> CustomFields { get; set; }
    }

    public class LocationCode
    {
        public long? Id { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string NameWoDiacritics { get; set; }
        public string Subdivision { get; set; }
        public string Status { get; set; }
        public string Function { get; set; }
        public long? Date { get; set; }
    }

    public class PricingCategory
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string TicketCategory { get; set; }
        public long? Occupancy { get; set; }
        public bool AgeQualified { get; set; }
        public object MinAge { get; set; }
        public object MaxAge { get; set; }
        public bool Dependent { get; set; }
        public object MasterCategoryId { get; set; }
        public long? MaxPerMaster { get; set; }
        public bool SumDependentCategories { get; set; }
        public long? MaxDependentSum { get; set; }
        public bool InternalUseOnly { get; set; }
        public List<object> Flags { get; set; }
        public bool DefaultCategory { get; set; }
        public string FullTitle { get; set; }
    }

    public class Rate
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public object Description { get; set; }
        public long? Index { get; set; }
        public string RateCode { get; set; }
        public bool PricedPerPerson { get; set; }
        public long? MinPerBooking { get; set; }
        public object MaxPerBooking { get; set; }
        public RateCancellationPolicy CancellationPolicy { get; set; }
        public object FixedPassExpiryDate { get; set; }
        public object PassValidForDays { get; set; }
        public string PickupSelectionType { get; set; }
        public string PickupPricingType { get; set; }
        public bool PickupPricedPerPerson { get; set; }
        public string DropoffSelectionType { get; set; }
        public string DropoffPricingType { get; set; }
        public bool DropoffPricedPerPerson { get; set; }
        public List<object> ExtraConfigs { get; set; }
        public List<object> StartTimeIds { get; set; }
        public List<object> PricingCategoryIds { get; set; }
    }

    public class RateCancellationPolicy
    {
        public long? Id { get; set; }
        public List<long?> Created { get; set; }
        public string Title { get; set; }
        public object Tax { get; set; }
        public List<PurplePenaltyRule> PenaltyRules { get; set; }
    }

    public class PurplePenaltyRule
    {
        public long? Id { get; set; }
        public List<long?> Created { get; set; }
        public long? CutoffHours { get; set; }
        public long? Percentage { get; set; }
    }

    public class StartPoint
    {
        public long? Id { get; set; }
        public List<long?> Created { get; set; }
        public object Type { get; set; }
        public string Title { get; set; }
        public object Code { get; set; }
        public Address Address { get; set; }
        public object PickupTicketDescription { get; set; }
        public object DropoffTicketDescription { get; set; }
        public List<object> Labels { get; set; }
    }

    public class Address
    {
        public long? Id { get; set; }
        public List<long?> Created { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public object AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public long? MapZoomLevel { get; set; }
        public GeoPoint GeoPoint { get; set; }
        public UnLocode UnLocode { get; set; }
    }

    public class GeoPoint
    {
        public double? Latitude { get; set; }
        public double? longitude { get; set; }
    }

    public class UnLocode
    {
        public string Country { get; set; }
        public string City { get; set; }
    }

    public class StartTime
    {
        public long? Id { get; set; }
        public long? Hour { get; set; }
        public long? Minute { get; set; }
        public bool OverrideTimeWhenPickup { get; set; }
        public long? PickupHour { get; set; }
        public long? PickupMinute { get; set; }
        public string DurationType { get; set; }
        public long? Duration { get; set; }
        public long? DurationMinutes { get; set; }
        public long? DurationHours { get; set; }
        public long? DurationDays { get; set; }
        public long? DurationWeeks { get; set; }
        public List<object> Flags { get; set; }
    }

    public class BokunBarcode
    {
        public string Value { get; set; }
        public string BarcodeType { get; set; }
    }

    public class ActivityBookingCancellationPolicy
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public List<FluffyPenaltyRule> PenaltyRules { get; set; }
        public object Tax { get; set; }
    }

    public class FluffyPenaltyRule
    {
        public long? Id { get; set; }
        public long? CutoffHours { get; set; }
        public long? Charge { get; set; }
        public string ChargeType { get; set; }
    }

    public class InvoiceElement
    {
        public long? Id { get; set; }
        public string Currency { get; set; }
        public List<object> IncludedAppliedTaxes { get; set; }
        public List<object> ExcludedAppliedTaxes { get; set; }
        public long? ProductBookingId { get; set; }
        public string Title { get; set; }
        public Issuer Product { get; set; }
        public string ProductCategory { get; set; }
        public string ProductConfirmationCode { get; set; }
        public string Dates { get; set; }
        public List<object> CustomLineItems { get; set; }
        public List<LineItem> LineItems { get; set; }
        public AsMoney TotalDiscountedAsMoney { get; set; }
        public AsMoney TotalAsMoney { get; set; }
        public bool Free { get; set; }
        public string TotalAsText { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public AsMoney TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public bool ExcludedTaxes { get; set; }
        public AsMoney TotalExcludedTaxAsMoney { get; set; }
        public string TotalExcludedTaxAsText { get; set; }
        public bool IncludedTaxes { get; set; }
        public AsMoney TotalIncludedTaxAsMoney { get; set; }
        public string TotalIncludedTaxAsText { get; set; }
        public AsMoney TotalTaxAsMoney { get; set; }
        public string TotalTaxAsText { get; set; }
        public AsMoney TotalDiscountAsMoney { get; set; }
    }

    public class LineItem
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Currency { get; set; }
        public long? UnitPriceDate { get; set; }
        public long? Quantity { get; set; }
        public long? UnitPrice { get; set; }
        public long? CalculatedDiscount { get; set; }
        public long? CustomDiscount { get; set; }
        public long? CalculatedDiscountAmount { get; set; }
        public long? TaxAmount { get; set; }
        public List<object> TaxesAsMoney { get; set; }
        public string ItemBookingId { get; set; }
        public long? CostItemId { get; set; }
        public string CostItemTitle { get; set; }
        public string CostGroupTitle { get; set; }
        public bool SupportsDiscount { get; set; }
        public long? Total { get; set; }
        public AsMoney TotalDiscountedAsMoney { get; set; }
        public AsMoney TotalAsMoney { get; set; }
        public string TotalAsText { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public AsMoney TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public AsMoney TaxAsMoney { get; set; }
        public long? TotalDue { get; set; }
        public AsMoney DiscountedUnitPriceAsMoney { get; set; }
        public long? Discount { get; set; }
        public string TaxAsText { get; set; }
        public long? TotalDiscounted { get; set; }
        public long? DiscountedUnitPrice { get; set; }
        public string DiscountedUnitPriceAsText { get; set; }
        public AsMoney UnitPriceAsMoney { get; set; }
        public string UnitPriceAsText { get; set; }
        public AsMoney DiscountAmountAsMoney { get; set; }
        public string DiscountAmountAsText { get; set; }
        public AsMoney CalculatedDiscountAmountAsMoney { get; set; }
    }

    public class AsMoney
    {
        public long? Amount { get; set; }
        public string Currency { get; set; }
    }

    public class Issuer
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public List<object> Flags { get; set; }
        public string ExternalId { get; set; }
    }

    public class PricingCategoryBooking
    {
        public long? Id { get; set; }
        public long? PricingCategoryId { get; set; }
        public bool LeadPassenger { get; set; }
        public long? Age { get; set; }
        public BokunBarcode Barcode { get; set; }
        public Customer PassengerInfo { get; set; }
        public string BookedTitle { get; set; }
        public long? Quantity { get; set; }
        public List<object> Extras { get; set; }
        public List<object> Answers { get; set; }
        public List<object> Flags { get; set; }
    }

    public class Customer
    {
        public bool ContactDetailsHidden { get; set; }
        public object ContactDetailsHiddenUntil { get; set; }
        public long? Id { get; set; }
        public object Created { get; set; }
        public Guid Uuid { get; set; }
        public string Email { get; set; }
        public object Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public object PersonalIdNumber { get; set; }
        public object Language { get; set; }
        public object Nationality { get; set; }
        public object Sex { get; set; }
        public object DateOfBirth { get; set; }
        public object PhoneNumber { get; set; }
        public object PhoneNumberCountryCode { get; set; }
        public object Address { get; set; }
        public object PostCode { get; set; }
        public object State { get; set; }
        public object Place { get; set; }
        public object Country { get; set; }
        public object Organization { get; set; }
        public object PassportId { get; set; }
        public object PassportExpDay { get; set; }
        public object PassportExpMonth { get; set; }
        public object PassportExpYear { get; set; }
        public object Credentials { get; set; }
    }

    public class Product
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public List<object> Flags { get; set; }
        public Vendor Vendor { get; set; }
        public string ExternalId { get; set; }
        public string ProductCategory { get; set; }
        public ActivityBookingCancellationPolicy CancellationPolicy { get; set; }
    }

    public class QuantityByPricingCategory
    {
        public long? The1691 { get; set; }
    }

    public class BookingChannel
    {
        public long? Id { get; set; }
        public Guid Uuid { get; set; }
        public string Title { get; set; }
        public bool Backend { get; set; }
        public bool OverrideVoucherHeader { get; set; }
        public List<object> Flags { get; set; }
    }

    public class CustomerPayment
    {
        public long? Id { get; set; }
        public long? Amount { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        public long? TransactionDate { get; set; }
        public long? ActiveCustomerInvoiceId { get; set; }
        public string PaymentType { get; set; }
        public bool IsRefundable { get; set; }
        public long? TotalRefundedAmount { get; set; }
        public long? RemainingRefundableAmount { get; set; }
        public bool Refundable { get; set; }
        public AsMoney AmountAsMoney { get; set; }
        public string AmountAsText { get; set; }
        public string RefundedAmountAsText { get; set; }
    }

    public class BookingInvoice
    {
        public long? Id { get; set; }
        public long? IssueDate { get; set; }
        public string Currency { get; set; }
        public List<object> IncludedAppliedTaxes { get; set; }
        public List<object> ExcludedAppliedTaxes { get; set; }
        public Issuer Issuer { get; set; }
        public IssuerCompany IssuerCompany { get; set; }
        public Customer Recipient { get; set; }
        public List<object> CustomLineItems { get; set; }
        public List<InvoiceElement> ProductInvoices { get; set; }
        public List<object> Payments { get; set; }
        public string IssuerName { get; set; }
        public List<object> LodgingTaxes { get; set; }
        public string InvoiceNumber { get; set; }
        public string RecipientName { get; set; }
        public AsMoney TotalDiscountedAsMoney { get; set; }
        public AsMoney TotalAsMoney { get; set; }
        public bool Free { get; set; }
        public string TotalAsText { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public AsMoney TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public bool ExcludedTaxes { get; set; }
        public AsMoney TotalExcludedTaxAsMoney { get; set; }
        public string TotalExcludedTaxAsText { get; set; }
        public bool IncludedTaxes { get; set; }
        public AsMoney TotalIncludedTaxAsMoney { get; set; }
        public string TotalIncludedTaxAsText { get; set; }
        public AsMoney TotalTaxAsMoney { get; set; }
        public string TotalTaxAsText { get; set; }
        public AsMoney TotalDiscountAsMoney { get; set; }
    }

    public class IssuerCompany
    {
    }

    public class Seller
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CurrencyCode { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string LogoStyle { get; set; }
        public bool ShowInvoiceIdOnTicket { get; set; }
        public bool ShowAgentDetailsOnTicket { get; set; }
        public bool ShowPaymentsOnInvoice { get; set; }
    }

    public class TravelDocuments
    {
        public string Invoice { get; set; }
        public List<ActivityTicket> ActivityTickets { get; set; }
        public List<object> AccommodationTickets { get; set; }
        public List<object> RentalTickets { get; set; }
        public List<object> TransportTickets { get; set; }
    }

    public class ActivityTicket
    {
        public long? BookingId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductConfirmationCode { get; set; }
        public string Ticket { get; set; }
    }

    public enum Currency { Usd };

    public class Fields
    {
        public Error[] errors { get; set; }
    }

    public class Error
    {
        public string reason { get; set; }
        public string path { get; set; }
        public string questionId { get; set; }
    }
}