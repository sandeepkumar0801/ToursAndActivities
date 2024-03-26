using System;
using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.GetActivity
{
    public class GetActivityRs : EntityBase
    {
        public int? Id { get; set; }
        public string ExternalId { get; set; }
        public int? ProductGroupId { get; set; }
        public string ProductCategory { get; set; }
        public bool? Box { get; set; }
        public bool? InventoryLocal { get; set; }
        public bool? InventorySupportsPricing { get; set; }
        public bool? InventorySupportsAvailability { get; set; }
        public DateTime LastPublished { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Excerpt { get; set; }
        public Cancellationpolicy CancellationPolicy { get; set; }
        public string BarcodeType { get; set; }
        public string TimeZone { get; set; }
        public List<Maincontactfield> MainContactFields { get; set; }
        public List<string> RequiredCustomerFields { get; set; }
        public List<object> Keywords { get; set; }
        public List<object> Flags { get; set; }
        public object Slug { get; set; }
        public string BaseLanguage { get; set; }
        public List<string> Languages { get; set; }
        public List<object> PaymentCurrencies { get; set; }
        public List<object> CustomFields { get; set; }
        public List<Taggroup> TagGroups { get; set; }
        public List<Category> Categories { get; set; }
        public object KeyPhoto { get; set; }
        public List<object> Photos { get; set; }
        public List<object> Videos { get; set; }
        public Vendor Vendor { get; set; }
        public object BoxedVendor { get; set; }
        public bool? StoredExternally { get; set; }
        public object PluginId { get; set; }
        public decimal? ReviewRating { get; set; }
        public int? ReviewCount { get; set; }
        public string ActivityType { get; set; }
        public string BookingType { get; set; }
        public string ScheduleType { get; set; }
        public string CapacityType { get; set; }
        public object PassExpiryType { get; set; }
        public string MeetingType { get; set; }
        public int? PassCapacity { get; set; }
        public int? PassesAvailable { get; set; }
        public bool? DressCode { get; set; }
        public bool? PassportRequired { get; set; }
        public List<string> SupportedAccessibilityTypes { get; set; }
        public List<object> StartPoints { get; set; }
        public List<object> BookingQuestions { get; set; }
        public List<object> PassengerFields { get; set; }
        public string Included { get; set; }
        public string Excluded { get; set; }
        public string Requirements { get; set; }
        public string Attention { get; set; }
        public Locationcode LocationCode { get; set; }
        public int? BookingCutoffMinutes { get; set; }
        public int? BookingCutoffHours { get; set; }
        public int? BookingCutoffDays { get; set; }
        public int? BookingCutoffWeeks { get; set; }
        public int? RequestDeadlineMinutes { get; set; }
        public int? RequestDeadlineHours { get; set; }
        public int? RequestDeadlineDays { get; set; }
        public int? RequestDeadlineWeeks { get; set; }
        public int? BoxedActivityId { get; set; }
        public bool? ComboActivity { get; set; }
        public bool? DayBasedAvailability { get; set; }
        public bool? SelectFromDayOptions { get; set; }
        public List<object> DayOptions { get; set; }
        public List<string> ActivityCategories { get; set; }
        public List<string> ActivityAttributes { get; set; }
        public List<object> GuidanceTypes { get; set; }
        public int? DefaultRateId { get; set; }
        public List<Rate> Rates { get; set; }
        public bool? TicketPerPerson { get; set; }
        public string DurationType { get; set; }
        public int? Duration { get; set; }
        public int? DurationMinutes { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationWeeks { get; set; }
        public string DurationText { get; set; }
        public int? MinAge { get; set; }
        public decimal? NextDefaultPrice { get; set; }
        public Nextdefaultpricemoney NextDefaultPriceMoney { get; set; }
        public bool? PickupService { get; set; }
        public bool? PickupAllotment { get; set; }
        public bool? UseComponentPickupAllotments { get; set; }
        public List<object> PickupFlags { get; set; }
        public bool? CustomPickupAllowed { get; set; }
        public int? PickupMinutesBefore { get; set; }
        public string NoPickupMsg { get; set; }
        public bool? ShowGlobalPickupMsg { get; set; }
        public bool? ShowNoPickupMsg { get; set; }
        public bool? DropoffService { get; set; }
        public List<object> DropoffFlags { get; set; }
        public bool? CustomDropoffAllowed { get; set; }
        public string DifficultyLevel { get; set; }
        public List<Pricingcategory> PricingCategories { get; set; }
        public List<object> AgendaItems { get; set; }
        public List<Starttime> StartTimes { get; set; }
        public List<BookableExtras> BookableExtras { get; set; }
        public object Route { get; set; }
        public bool? HasOpeningHours { get; set; }
        public object DefaultOpeningHours { get; set; }
        public List<object> SeasonalOpeningHours { get; set; }
        public Displaysettings DisplaySettings { get; set; }
        public int? RequestDeadline { get; set; }
        public int? ActualId { get; set; }
        public int? BookingCutoff { get; set; }
        public Actualvendor ActualVendor { get; set; }
    }

    public class Cancellationpolicy
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public List<Penaltyrule> PenaltyRules { get; set; }
        public object Tax { get; set; }
        public string policyType { get; set; }
    }

    public class Penaltyrule
    {
        public int? Id { get; set; }
        public int? CutoffHours { get; set; }
        public decimal? Charge { get; set; }
        public string ChargeType { get; set; }
    }

    public class Vendor
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string CurrencyCode { get; set; }
        public bool? ShowInvoiceIdOnTicket { get; set; }
        public bool? ShowAgentDetailsOnTicket { get; set; }
        public bool? ShowPaymentsOnInvoice { get; set; }
    }

    public class Locationcode
    {
        public int? Id { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string NameWoDiacritics { get; set; }
        public string Subdivision { get; set; }
        public string Status { get; set; }
        public string Function { get; set; }
        public int? Date { get; set; }
        public string Coordinates { get; set; }
    }

    public class Nextdefaultpricemoney
    {
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }

    public class Displaysettings
    {
        public bool? ShowPickupPlaces { get; set; }
        public bool? ShowRouteMap { get; set; }
        public bool? SelectRateBasedOnStartTime { get; set; }
        public List<object> CustomFields { get; set; }
    }

    public class Actualvendor
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string CurrencyCode { get; set; }
        public bool? ShowInvoiceIdOnTicket { get; set; }
        public bool? ShowAgentDetailsOnTicket { get; set; }
        public bool? ShowPaymentsOnInvoice { get; set; }
    }

    public class Maincontactfield
    {
        public string Field { get; set; }
        public bool? Required { get; set; }
    }

    public class Taggroup
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public List<object> Flags { get; set; }
        public List<Tag> Tags { get; set; }
        public bool? Group { get; set; }
        public string FacetName { get; set; }
        public bool? Exclusive { get; set; }
        public bool? SharedWithSuppliers { get; set; }
        public string ProductCategory { get; set; }
        public string SubCategory { get; set; }
    }

    public class Tag
    {
        public int? Id { get; set; }
        public int? GroupId { get; set; }
        public string Title { get; set; }
        public bool? Exclusive { get; set; }
        public List<object> Flags { get; set; }
    }

    public class Category
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public bool? AllowsSelectingMultipleChildren { get; set; }
        public List<object> Flags { get; set; }
        public List<Category> Categories { get; set; }
    }

    public class Cancellationpolicy1
    {
        public int? Id { get; set; }
        public List<int> Created { get; set; }
        public string Title { get; set; }
        public object Tax { get; set; }
        public List<Penaltyrule> PenaltyRules { get; set; }
    }

    public class Pricingcategory
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string TicketCategory { get; set; }
        public int? Occupancy { get; set; }
        public bool? AgeQualified { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool? Dependent { get; set; }
        public int? MasterCategoryId { get; set; }
        public int? MaxPerMaster { get; set; }
        public bool? SumDependentCategories { get; set; }
        public int? MaxDependentSum { get; set; }
        public bool? InternalUseOnly { get; set; }
        public List<object> Flags { get; set; }
        public bool? DefaultCategory { get; set; }
        public string FullTitle { get; set; }
    }

    public class Starttime
    {
        public int? Id { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public bool? OverrideTimeWhenPickup { get; set; }
        public int? PickupHour { get; set; }
        public int? PickupMinute { get; set; }
        public string DurationType { get; set; }
        public int? Duration { get; set; }
        public int? DurationMinutes { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationWeeks { get; set; }
        public List<object> Flags { get; set; }
    }

    public class Rate
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int? index { get; set; }
        public object rateCode { get; set; }
        public bool? pricedPerPerson { get; set; }
        public int? minPerBooking { get; set; }
        public int? maxPerBooking { get; set; }
        public object fixedPassExpiryDate { get; set; }
        public int? passValidForDays { get; set; }
        public string pickupSelectionType { get; set; }
        public string pickupPricingType { get; set; }
        public bool? pickupPricedPerPerson { get; set; }
        public string dropoffSelectionType { get; set; }
        public string dropoffPricingType { get; set; }
        public bool? dropoffPricedPerPerson { get; set; }
        public List<ExtraConfigs> extraConfigs { get; set; }
        public int[] startTimeIds { get; set; }
        public bool? allStartTimes { get; set; }
        public bool? tieredPricingEnabled { get; set; }
        public object[] tiers { get; set; }
        public int[] pricingCategoryIds { get; set; }
        public bool? allPricingCategories { get; set; }
        public object[] details { get; set; }
    }

    public class ExtraConfigs
    {
        public string activityExtraId { get; set; }
        public string selectionType { get; set; }
        public bool? pricedPerPerson { get; set; }
    }

    public class BookableExtras
    {
        public string id { get; set; }
        public string externalId { get; set; }
        public string title { get; set; }
        public string information { get; set; }
        public bool? included { get; set; }
        public bool? free { get; set; }
        public string productGroupId { get; set; }
        public string pricingType { get; set; }
        public string pricingTypeLabel { get; set; }
        public string price { get; set; }
        public bool? increasesCapacity { get; set; }
        public string maxPerBooking { get; set; }
        public bool? limitByPax { get; set; }
        public List<object> flags { get; set; }
        public List<BookableExtraQuestions> questions { get; set; }
    }

    public class BookableExtraQuestions
    {
        public string id { get; set; }
        public bool? active { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public string options { get; set; }
        public bool? answerRequired { get; set; }
        public List<object> flags { get; set; }
    }
}