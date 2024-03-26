using System.Collections;
using System.Collections.Generic;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Response
{

    public class ProductRes
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
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
        public List<OptionFP> options { get; set; }
        public string title { get; set; }
        public string country { get; set; }
        public string location { get; set; }
        public string subtitle { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
        public string[] highlights { get; set; }
        public List<string> inclusions { get; set; }
        public List<string> exclusions { get; set; }
        public string bookingTerms { get; set; }
        public string redemptionInstructions { get; set; }
        public string cancellationPolicy { get; set; }
        public DestinationFP destination { get; set; }
        public CategoryFP[] categories { get; set; }
        public List<FaqFP> faqs { get; set; }
        public string coverImageUrl { get; set; }
        public string bannerImageUrl { get; set; }
        public string videoUrl { get; set; }
        public ImagesForProduct[] galleryImages { get; set; }
        public ImagesForProduct[] bannerImages { get; set; }
        public string defaultCurrency { get; set; }
        public string[] availableCurrencies { get; set; }
        public string offerCode { get; set; }
        public string offerTitle { get; set; }
        public string SupplierId { get; set; }

        public bool isPackage { get; set; }

        /*Package Fields Start*/
        public string[] settlementMethods { get; set; }
        /*Package Fields End*/

    }


    public class ImagesForProduct
    {
        public string url { get; set; }
        public string title { get; set; }
        public string caption { get; set; }
    }

    public class DestinationFP
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public ContactFP contact { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public Category1FP[] categories { get; set; }
    }

    public class ContactFP
    {
        public string website { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string address { get; set; }
    }

    public class CategoryFP
    {
        public string id { get; set; }
        public string title { get; set; }
        public string shortDescription { get; set; }
        public string coverImageUrl { get; set; }
        public string bannerImageUrl { get; set; }
        public string[] productIds { get; set; }
    }

    public class OptionFP
    {
        public string id { get; set; }
        public bool _default { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
        public string cancellationCutoff { get; set; }
        public int cancellationCutoffAmount { get; set; }
        public string cancellationCutoffUnit { get; set; }
        public RestrictionsFP restrictions { get; set; }
        public List<UnitForproduct> units { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string language { get; set; }
        public string shortDescription { get; set; }
        public string duration { get; set; }
        public string durationAmount { get; set; }
        public string durationUnit { get; set; }
        public object itinerary { get; set; }
        public bool pickupAvailable { get; set; }
        public bool pickupRequired { get; set; }
        public object[] pickupPoints { get; set; }


        //Package Start
        public Packageinclude[] packageIncludes { get; set; }
        public string[] availabilityLocalStartTimes { get; set; }
        public object[] requiredContactFields { get; set; }
        //Package End
    }

    public class RestrictionsFP
    {
        public int minUnits { get; set; }
        public object maxUnits { get; set; }

        //Package Start
        public int minPaxCount { get; set; }
        public object maxPaxCount { get; set; }
        //Package End
    }

    public class UnitForproduct
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Restrictions1FP restrictions { get; set; }
        public string title { get; set; }
        public string titlePlural { get; set; }
        public string subtitle { get; set; }
        public PricingfromFP[] pricingFrom { get; set; }

        //Package Start
        public object[] requiredContactFields { get; set; }
        //Package End

    }

    public class Restrictions1FP
    {
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public bool idRequired { get; set; }
        public int minQuantity { get; set; }
        public int maxQuantity { get; set; }
        public int paxCount { get; set; }
        public List<string> accompaniedBy { get; set; }
    }

    public class PricingfromFP
    {
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
        public string currency { get; set; }
        public int currencyPrecision { get; set; }
        public IncludedtaxFP[] includedTaxes { get; set; }
    }

    public class IncludedtaxFP
    {
        public string name { get; set; }
        public int original { get; set; }
        public int retail { get; set; }
        public int net { get; set; }
    }

    public class Category1FP
    {
        public string id { get; set; }
        public string title { get; set; }
        public string shortDescription { get; set; }
        public string coverImageUrl { get; set; }
        public string bannerImageUrl { get; set; }
        public string[] productIds { get; set; }
    }

    public class FaqFP
    {
        public string question { get; set; }
        public string answer { get; set; }
    }


    public class Packageinclude
    {
        public string title { get; set; }
        public int count { get; set; }
        public Include[] includes { get; set; }
    }

    public class Include
    {
        public string id { get; set; }
        public bool required { get; set; }
        public int limit { get; set; }
        public string productId { get; set; }
        public ProductRes product { get; set; }
        public string optionId { get; set; }
        public OptionFP option { get; set; }
    }

}







