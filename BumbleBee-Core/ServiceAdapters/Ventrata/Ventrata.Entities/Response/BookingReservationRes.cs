using System;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Response
{

    public class BookingReservationRes
    {
        public string uuid { get; set; }
        public bool testMode { get; set; }
        public string resellerReference { get; set; }
        public string supplierReference { get; set; }
        public string status { get; set; }
        public DateTime utcCreatedAt { get; set; }
        public DateTime utcUpdatedAt { get; set; }
        public DateTime utcExpiresAt { get; set; }
        public object utcRedeemedAt { get; set; }
        public object utcConfirmedAt { get; set; }
        public string productId { get; set; }
        public Product product { get; set; }
        public string optionId { get; set; }
        public Option1 option { get; set; }
        public bool cancellable { get; set; }
        public object cancellation { get; set; }
        public bool freesale { get; set; }
        public DateTime availabilityId { get; set; }
        public Availability availability { get; set; }
        public Contact1 contact { get; set; }
        public string notes { get; set; }
        public string[] deliveryMethods { get; set; }
        public Voucher voucher { get; set; }
        public Unititem[] unitItems { get; set; }
        public bool checkedIn { get; set; }
        public bool checkinAvailable { get; set; }
        public object checkinUrl { get; set; }
        public string meetingPoint { get; set; }
        public object meetingPointCoordinates { get; set; }
        public DateTime meetingLocalDateTime { get; set; }
        public string duration { get; set; }
        public string durationAmount { get; set; }
        public string durationUnit { get; set; }
        public string orderId { get; set; }
        public bool primary { get; set; }
        public PricingRes pricing { get; set; }
        public bool pickupRequested { get; set; }
        public object pickupPoint { get; set; }
        public object[] adjustments { get; set; }
        public object offerCode { get; set; }
        public object offerTitle { get; set; }
        public Offercomparison[] offerComparisons { get; set; }
        public bool offerIsCombination { get; set; }
        public Cardpayment cardPayment { get; set; }

        public bool? isPackage { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public object reference { get; set; }
        public string locale { get; set; }
        public string timeZone { get; set; }
        public bool allowFreesale { get; set; }
        public bool instantConfirmation { get; set; }
        public bool instantDelivery { get; set; }
        public bool availabilityRequired { get; set; }
        public string availabilityType { get; set; }
        public string[] deliveryFormats { get; set; }
        public string[] deliveryMethods { get; set; }
        public string redemptionMethod { get; set; }
        public Capability[] capabilities { get; set; }
        public Option[] options { get; set; }
        public string title { get; set; }
        public string country { get; set; }
        public string location { get; set; }
        public object subtitle { get; set; }
        public object shortDescription { get; set; }
        public string description { get; set; }
        public string[] highlights { get; set; }
        public string[] inclusions { get; set; }
        public string[] exclusions { get; set; }
        public string bookingTerms { get; set; }
        public string redemptionInstructions { get; set; }
        public object cancellationPolicy { get; set; }
        public Destination destination { get; set; }
        public Category1[] categories { get; set; }
        public Faq[] faqs { get; set; }
        public string coverImageUrl { get; set; }
        public object bannerImageUrl { get; set; }
        public object videoUrl { get; set; }
        public object[] galleryImages { get; set; }
        public object[] bannerImages { get; set; }
        public string defaultCurrency { get; set; }
        public string[] availableCurrencies { get; set; }
        public object offerCode { get; set; }
        public object offerTitle { get; set; }
    }

    public class Destination
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public Contact contact { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public Category[] categories { get; set; }
    }

    public class Contact
    {
        public object website { get; set; }
        public string email { get; set; }
        public object telephone { get; set; }
        public object address { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string title { get; set; }
        public object shortDescription { get; set; }
        public object coverImageUrl { get; set; }
        public object bannerImageUrl { get; set; }
        public string[] productIds { get; set; }
    }

    public class Capability
    {
        public string id { get; set; }
        public int revision { get; set; }
        public bool required { get; set; }
        public string[] dependencies { get; set; }
        public string docs { get; set; }
        public bool _default { get; set; }
    }

    public class Option
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string internalName { get; set; }
        public object reference { get; set; }
        public string cancellationCutoff { get; set; }
        public int cancellationCutoffAmount { get; set; }
        public string cancellationCutoffUnit { get; set; }
        public Restrictions restrictions { get; set; }
        public Unit[] units { get; set; }
        public string title { get; set; }
        public object subtitle { get; set; }
        public string language { get; set; }
        public object shortDescription { get; set; }
        public string duration { get; set; }
        public string durationAmount { get; set; }
        public string durationUnit { get; set; }
        public object itinerary { get; set; }
        public bool pickupAvailable { get; set; }
        public bool pickupRequired { get; set; }
        public object[] pickupPoints { get; set; }
    }

    public class Restrictions
    {
        public int minUnits { get; set; }
        public object maxUnits { get; set; }
    }

    public class Unit
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Restrictions1 restrictions { get; set; }
        public string title { get; set; }
        public string titlePlural { get; set; }
        public string subtitle { get; set; }
        public Pricingfrom[] pricingFrom { get; set; }
    }

    public class Restrictions1
    {
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public bool idRequired { get; set; }
        public object minQuantity { get; set; }
        public object maxQuantity { get; set; }
        public object[] accompaniedBy { get; set; }
    }

    public class Pricingfrom
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public Includedtax[] includedTaxes { get; set; }
    }

    public class Includedtax
    {
        public string name { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Category1
    {
        public string id { get; set; }
        public string title { get; set; }
        public object shortDescription { get; set; }
        public object coverImageUrl { get; set; }
        public object bannerImageUrl { get; set; }
        public string[] productIds { get; set; }
    }

    public class Faq
    {
        public string question { get; set; }
        public string answer { get; set; }
    }

    public class Option1
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string internalName { get; set; }
        public object reference { get; set; }
        public string cancellationCutoff { get; set; }
        public int cancellationCutoffAmount { get; set; }
        public string cancellationCutoffUnit { get; set; }
        public Restrictions2 restrictions { get; set; }
        public Unit1[] units { get; set; }
        public string title { get; set; }
        public object subtitle { get; set; }
        public string language { get; set; }
        public object shortDescription { get; set; }
        public string duration { get; set; }
        public string durationAmount { get; set; }
        public string durationUnit { get; set; }
        public object itinerary { get; set; }
        public bool pickupAvailable { get; set; }
        public bool pickupRequired { get; set; }
        public object[] pickupPoints { get; set; }
    }

    public class Restrictions2
    {
        public int minUnits { get; set; }
        public object maxUnits { get; set; }
    }

    public class Unit1
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Restrictions3 restrictions { get; set; }
        public string title { get; set; }
        public string titlePlural { get; set; }
        public string subtitle { get; set; }
        public Pricingfrom1[] pricingFrom { get; set; }
    }

    public class Restrictions3
    {
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public bool idRequired { get; set; }
        public object minQuantity { get; set; }
        public object maxQuantity { get; set; }
        public object[] accompaniedBy { get; set; }
    }

    public class Pricingfrom1
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public Includedtax1[] includedTaxes { get; set; }
    }

    public class Includedtax1
    {
        public string name { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Availability
    {
        public DateTime id { get; set; }
        public DateTime localDateTimeStart { get; set; }
        public DateTime localDateTimeEnd { get; set; }
        public bool allDay { get; set; }
        public OpeninghourForResponse[] openingHours { get; set; }
    }

    public class OpeninghourForResponse
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Contact1
    {
        public object fullName { get; set; }
        public object firstName { get; set; }
        public object lastName { get; set; }
        public object emailAddress { get; set; }
        public object phoneNumber { get; set; }
        public object[] locales { get; set; }
        public object country { get; set; }
        public object notes { get; set; }
    }

    public class Voucher
    {
        public string redemptionMethod { get; set; }
        public object utcRedeemedAt { get; set; }
        public object[] deliveryOptions { get; set; }
    }

    public class PricingRes
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public Includedtax2[] includedTaxes { get; set; }
    }

    public class Includedtax2
    {
        public string name { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Cardpayment
    {
        public string gateway { get; set; }
        public string merchantName { get; set; }
        public Worldpay worldpay { get; set; }
    }

    public class Worldpay
    {
        public string cseKey { get; set; }
        public string merchantCode { get; set; }
        public string googlePayMerchantId { get; set; }
    }

    public class Unititem
    {
        public string uuid { get; set; }
        public object resellerReference { get; set; }
        public string supplierReference { get; set; }
        public string unitId { get; set; }
        public Unit2 unit { get; set; }
        public string status { get; set; }
        public object utcRedeemedAt { get; set; }
        public Ticket ticket { get; set; }
    }

    public class Unit2
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Restrictions4 restrictions { get; set; }
        public string title { get; set; }
        public string titlePlural { get; set; }
        public string subtitle { get; set; }
        public Pricingfrom2[] pricingFrom { get; set; }
    }

    public class Restrictions4
    {
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public bool idRequired { get; set; }
        public object minQuantity { get; set; }
        public object maxQuantity { get; set; }
        public object[] accompaniedBy { get; set; }
    }

    public class Pricingfrom2
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public Includedtax3[] includedTaxes { get; set; }
    }

    public class Includedtax3
    {
        public string name { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Ticket
    {
        public string redemptionMethod { get; set; }
        public object utcRedeemedAt { get; set; }
        public object[] deliveryOptions { get; set; }
    }

    public class Offercomparison
    {
        public string productId { get; set; }
        public string optionId { get; set; }
        public Pricing1 pricing { get; set; }
        public Product1 product { get; set; }
    }

    public class Pricing1
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public Includedtax4[] includedTaxes { get; set; }
    }

    public class Includedtax4
    {
        public string name { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Product1
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public object reference { get; set; }
        public string locale { get; set; }
        public string timeZone { get; set; }
        public bool allowFreesale { get; set; }
        public bool instantConfirmation { get; set; }
        public bool instantDelivery { get; set; }
        public bool availabilityRequired { get; set; }
        public string availabilityType { get; set; }
        public string[] deliveryFormats { get; set; }
        public string[] deliveryMethods { get; set; }
        public string redemptionMethod { get; set; }
        public Capability1[] capabilities { get; set; }
        public Option2[] options { get; set; }
        public string title { get; set; }
        public string country { get; set; }
        public string location { get; set; }
        public object subtitle { get; set; }
        public object shortDescription { get; set; }
        public string description { get; set; }
        public string[] highlights { get; set; }
        public string[] inclusions { get; set; }
        public string[] exclusions { get; set; }
        public string bookingTerms { get; set; }
        public string redemptionInstructions { get; set; }
        public object cancellationPolicy { get; set; }
        public Destination1 destination { get; set; }
        public Category3[] categories { get; set; }
        public Faq1[] faqs { get; set; }
        public string coverImageUrl { get; set; }
        public object bannerImageUrl { get; set; }
        public object videoUrl { get; set; }
        public object[] galleryImages { get; set; }
        public object[] bannerImages { get; set; }
        public string defaultCurrency { get; set; }
        public string[] availableCurrencies { get; set; }
        public object offerCode { get; set; }
        public object offerTitle { get; set; }
    }

    public class Destination1
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public Contact2 contact { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public Category2[] categories { get; set; }
    }

    public class Contact2
    {
        public object website { get; set; }
        public string email { get; set; }
        public object telephone { get; set; }
        public object address { get; set; }
    }

    public class Category2
    {
        public string id { get; set; }
        public string title { get; set; }
        public object shortDescription { get; set; }
        public object coverImageUrl { get; set; }
        public object bannerImageUrl { get; set; }
        public string[] productIds { get; set; }
    }

    public class Capability1
    {
        public string id { get; set; }
        public int revision { get; set; }
        public bool required { get; set; }
        public string[] dependencies { get; set; }
        public string docs { get; set; }
        public bool _default { get; set; }
    }

    public class Option2
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string internalName { get; set; }
        public object reference { get; set; }
        public string cancellationCutoff { get; set; }
        public int cancellationCutoffAmount { get; set; }
        public string cancellationCutoffUnit { get; set; }
        public Restrictions5 restrictions { get; set; }
        public Unit3[] units { get; set; }
        public string title { get; set; }
        public object subtitle { get; set; }
        public string language { get; set; }
        public object shortDescription { get; set; }
        public string duration { get; set; }
        public string durationAmount { get; set; }
        public string durationUnit { get; set; }
        public object itinerary { get; set; }
        public bool pickupAvailable { get; set; }
        public bool pickupRequired { get; set; }
        public object[] pickupPoints { get; set; }
    }

    public class Restrictions5
    {
        public int minUnits { get; set; }
        public object maxUnits { get; set; }
    }

    public class Unit3
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Restrictions6 restrictions { get; set; }
        public string title { get; set; }
        public string titlePlural { get; set; }
        public string subtitle { get; set; }
        public Pricingfrom3[] pricingFrom { get; set; }
    }

    public class Restrictions6
    {
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public bool idRequired { get; set; }
        public object minQuantity { get; set; }
        public object maxQuantity { get; set; }
        public object[] accompaniedBy { get; set; }
    }

    public class Pricingfrom3
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public Includedtax5[] includedTaxes { get; set; }
    }

    public class Includedtax5
    {
        public string name { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Category3
    {
        public string id { get; set; }
        public string title { get; set; }
        public object shortDescription { get; set; }
        public object coverImageUrl { get; set; }
        public object bannerImageUrl { get; set; }
        public string[] productIds { get; set; }
    }

    public class Faq1
    {
        public string question { get; set; }
        public string answer { get; set; }
    }

}
