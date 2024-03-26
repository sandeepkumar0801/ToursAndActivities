using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.ProductListResponse
{
    public class ProductListResponse
    {
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("current_item_count")]
        public int CurrentItemCount { get; set; }
        [JsonProperty("items_per_page")]
        public int ItemsPerPage { get; set; }
        [JsonProperty("start_index")]
        public int StartIndex { get; set; }
        [JsonProperty("total_items")]
        public int TotalItems { get; set; }
        [JsonProperty("page_index")]
        public int PageIndex { get; set; }
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("product_internal_reference")]
        public string ProductInternalReference { get; set; }
        [JsonProperty("product_supplier_id")]
        public string ProductSupplierId { get; set; }
        [JsonProperty("product_distributor_id")]
        public string ProductDistributorId { get; set; }
        [JsonProperty("product_distributor_name")]
        public string ProductDistributorName { get; set; }
        [JsonProperty("Product_reseller_id")]
        public string ProductResellerId { get; set; }
        [JsonProperty("product_reseller_name")]
        public string ProductResellerName { get; set; }
        [JsonProperty("product_source_name")]
        public string Productsourcename { get; set; }
        [JsonProperty("product_default_language")]
        public string ProductDefaultLanguage { get; set; }
        [JsonProperty("product_slug")]
        public string ProductSlug { get; set; }
        [JsonProperty("Productfromprice")]
        public string ProductFromPrice { get; set; }
        [JsonProperty("product_start_date")]
        public DateTime ProductStartDate { get; set; }
        [JsonProperty("product_end_date")]
        public DateTime ProductEndDate { get; set; }
        [JsonProperty("product_booking_start_date")]
        public DateTime ProductBookingStartDate { get; set; }
        [JsonProperty("product_duration")]
        public int ProductDuration { get; set; }
        [JsonProperty("product_show_capacity_count")]
        public int ProductShowCapacityCount { get; set; }
        [JsonProperty("product_disabled")]
        public bool ProductDisabled { get; set; }
        [JsonProperty("product_third_party")]
        public bool ProductThirdParty { get; set; }
        [JsonProperty("product_seasonal_pricing")]
        public bool ProductSeasonalPricing { get; set; }
        [JsonProperty("product_quantity_pricing")]
        public bool ProductQuantityPricing { get; set; }
        [JsonProperty("product_daily_pricing")]
        public bool ProductDailyPricing { get; set; }
        [JsonProperty("product_dynamic_pricing")]
        public bool ProductDynamicPricing { get; set; }
        [JsonProperty("product_relation_details_visible")]
        public bool ProductrelationDetailsVisible { get; set; }
        [JsonProperty("product_timepicker_visible")]
        public bool ProductTimePickerVisible { get; set; }
        [JsonProperty("product_cluster")]
        public bool ProductCluster { get; set; }
        [JsonProperty("product_combi")]
        public bool ProductCombi { get; set; }
        [JsonProperty("product_addon")]
        public bool ProductAddon { get; set; }
        [JsonProperty("product_availability")]
        public bool ProductAvailability { get; set; }
        [JsonProperty("product_availability_assigned")]
        public bool ProductavailabilityAssigned { get; set; }
        [JsonProperty("product_past_date_booking_allowed")]
        public bool ProductPastDateBookingAllowed { get; set; }
        [JsonProperty("product_capacity_id")]
        public string ProductCapacityId { get; set; }
        [JsonProperty("product_capacity")]
        public bool ProductCapacity { get; set; }
        [JsonProperty("product_traveldate_required")]
        public bool ProductTravelDateRequired { get; set; }
        [JsonProperty("product_cancellation_allowed")]
        public bool productCancellationAllowed { get; set; }
        [JsonProperty("product_overbooking_allowed")]
        public bool ProductoverBookingAllowed { get; set; }
        [JsonProperty("product_capacity_type")]
        public string ProductCapacityType { get; set; }
        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }
        [JsonProperty("product_status")]
        public string ProductStatus { get; set; }
        [JsonProperty("product_catalogue_status")]
        public string productCatalogueStatus { get; set; }
        [JsonProperty("product_pickup_point")]
        public string ProductPickupPoint { get; set; }
        [JsonProperty("product_content")]
        public Product_Content ProductContent { get; set; }
        [JsonProperty("product_redemption_rules")]
        public ProductRedemptionRules ProductRedemptionRules { get; set; }
        [JsonProperty("product_code_settings")]
        public ProductCodeSettings productCodeSettings { get; set; }
        [JsonProperty("product_payment_detail")]
        public ProductPaymentDetail ProductPaymentDetail { get; set; }
        [JsonProperty("product_type_seasons")]
        public List<ProductTypeSeasons> ProductTypeSeasons { get; set; }
        [JsonProperty("product_options")]
        public List<ProductOptions> ProductOptions { get; set; }
        [JsonProperty("product_cancellation_policies")]
        public List<Product_Cancellation_Policies> ProductCancellationPolicies { get; set; }
        [JsonProperty("product_noshow_policy")]
        public Product_Noshow_Policy ProductNoShowPolicy { get; set; }
        [JsonProperty("product_flags")]
        public List<Product_Flags> ProductFlags { get; set; }
        [JsonProperty("product_opening_times")]
        public List<ProductOpeningTimes> ProductOpeningTimes { get; set; }
        [JsonProperty("product_addon_details")]
        public List<ProductAddonDetails> ProductAddonDetails { get; set; }
        [JsonProperty("product_categories")]
        public List<string> ProductCategories { get; set; }
        [JsonProperty("product_destinations")]
        public List<string> ProductDestinations { get; set; }
        [JsonProperty("product_google_categories")]
        public string ProductGoogleCategories { get; set; }
        [JsonProperty("product_booking_url")]
        public string ProductBookingUrl { get; set; }
        [JsonProperty("product_landing_page_view_url")]
        public string ProductLandingPageViewUrl { get; set; }
        [JsonProperty("product_languages")]
        public List<string> ProductLanguages { get; set; }
        [JsonProperty("product_content_languages")]
        public List<string> ProductContentLanguages { get; set; }
        [JsonProperty("product_guide_languages")]
        public List<Product_Guide_Languages> ProductGuideLanguages { get; set; }
        [JsonProperty("product_notes")]
        public List<Product_Notes> ProductNotes { get; set; }
        [JsonProperty("product_custom_fields")]
        public List<Product_Custom_Fields> ProductCustomFields { get; set; }
        [JsonProperty("product_created")]
        public DateTime productCreated { get; set; }
        [JsonProperty("product_created_name")]
        public string ProductCreatedName { get; set; }
        [JsonProperty("product_created_email")]
        public string ProductCreatedEmail { get; set; }
        [JsonProperty("product_modified")]
        public DateTime ProductModified { get; set; }
        [JsonProperty("product_modified_name")]
        public string ProductModifiedName { get; set; }
        [JsonProperty("product_modified_email")]
        public string ProductModifiedEmail { get; set; }
        [JsonProperty("product_view_type")]
        public string Productviewtype { get; set; }


        [JsonProperty("product_routes")]
        public List<string> ProductRoute { get; set; }

        [JsonProperty("product_booking_quantity_min")]
        public int MinQuantity { get; set; }

        [JsonProperty("product_booking_quantity_max")]
        public int MaxQuantity { get; set; }

    }

    public class Product_Content
    {
        [JsonProperty("product_title")]
        public string ProductTitle { get; set; }
        [JsonProperty("product_short_description")]
        public string ProductShortDescription { get; set; }
        [JsonProperty("product_long_description")]
        public string ProductLongDescription { get; set; }
        [JsonProperty("product_additional_information")]
        public string ProductAdditionalInformation { get; set; }
        [JsonProperty("product_duration_text")]
        public string ProductdurationText { get; set; }
       
        [JsonProperty("product_supplier_name")]
        public string ProductSupplierName { get; set; }
        [JsonProperty("product_entry_notes")]
        public string ProductEntryNotes { get; set; }
        [JsonProperty("product_favorite")]
        public bool ProductFavorite { get; set; }
        [JsonProperty("product_highlights")]
        public List<Product_Highlights> ProductHighlights { get; set; }
        [JsonProperty("product_images")]
        public List<Product_Images> ProductImages { get; set; }
        [JsonProperty("product_includes")]
        public List<Product_Includes> ProductIncludes { get; set; }
    }

    public class Product_Highlights
    {
        [JsonProperty("highlight_description")]
        public string HighlightDescription { get; set; }
    }

    public class Product_Images
    {
        [JsonProperty("image_type")]
        public string ImageType { get; set; }
        [JsonProperty("image_url")]
        public string Imageurl { get; set; }
    }

    public class Product_Includes
    {
        [JsonProperty("include_description")]
        public string includedescription { get; set; }
    }

    public class ProductRedemptionRules
    {
        [JsonProperty("redemption_has_duration")]
        public bool RedemptionHasDuration { get; set; }
        [JsonProperty("redemption_count_type")]
        public string RedemptionCountType { get; set; }
        [JsonProperty("redemption_count_value")]
        public int RedemptionCountValue { get; set; }
    }

    public class ProductCodeSettings
    {
        [JsonProperty("product_code_format")]
        public string ProductcodeFormat { get; set; }
        [JsonProperty("product_code_source")]
        public string ProductCodeSource { get; set; }
        [JsonProperty("product_group_code")]
        public bool ProductGroupCode { get; set; }
        [JsonProperty("product_combi_code")]
        public bool ProductCombiCode { get; set; }
        [JsonProperty("product_voucher_settings")]
        public string ProductVoucherSettings { get; set; }
    }

    public class ProductPaymentDetail
    {
        [JsonProperty("product_payment_currency")]
        public Product_Payment_Currency ProductPaymentCurrency { get; set; }
    }

    public class Product_Payment_Currency
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }
    }

    public class Product_Noshow_Policy
    {
        [JsonProperty("fee_percentage")]
        public string FeePercentage { get; set; }
    }

    public class ProductTypeSeasons
    {
        [JsonProperty("product_type_season_start_date")]
        public DateTime ProductTypeSeasonStartdate { get; set; }
        [JsonProperty("product_type_season_end_date")]
        public DateTime ProductTypeSeasonEnddate { get; set; }
        [JsonProperty("product_type_season_details")]
        public List<ProductTypeSeasonDetails> ProductTypeSeasonDetails { get; set; }
    }

    public class ProductTypeSeasonDetails
    {
        [JsonProperty("product_type_id")]
        public string ProductTypeId { get; set; }
        [JsonProperty("product_type")]
        public string ProductType { get; set; }
        [JsonProperty("product_type_label")]
        public string ProductTypeLabel { get; set; }
        [JsonProperty("product_type_class")]
        public string ProductTypeClass { get; set; }
        [JsonProperty("product_type_age_from")]
        public int ProductTypeAgeFrom { get; set; }
        [JsonProperty("product_type_age_to")]
        public int ProductTypeAgeTo { get; set; }
        [JsonProperty("product_type_pax")]
        public int ProductTypePax { get; set; }
        [JsonProperty("product_type_capacity")]
        public int ProductTypeCapacity { get; set; }
        [JsonProperty("product_type_price_type")]
        public string ProductTypePriceType { get; set; }
        [JsonProperty("product_type_price_tax_id")]
        public string ProductTypePriceTaxid { get; set; }
        [JsonProperty("product_type_pricing")]
        public ProductTypePricing ProductTypePricing { get; set; }
        [JsonProperty("product_type_fees")]
        public List<ProductTypeFees> ProductTypeFees { get; set; }
        [JsonProperty("product_type_description")]
        public string ProductTypeDescription { get; set; }
    }

    public class ProductTypePricing
    {
        [JsonProperty("product_type_list_price")]
        public string ProductTypeListPrice { get; set; }
        [JsonProperty("product_type_display_price")]
        public bool ProductTypeDisplayPrice { get; set; }
        [JsonProperty("product_type_sales_price")]
        public string ProductTypeSalesPrice { get; set; }
        [JsonProperty("product_type_resale_price")]
        public string ProductTyperesalePrice { get; set; }
        [JsonProperty("product_type_supplier_price")]
        public string ProductTypeSupplierPrice { get; set; }
        [JsonProperty("product_type_discount")]
        public string ProductTypeDiscount { get; set; }

        
        
    }

    public class ProductTypeFees
    {
        [JsonProperty("fee_type")]
        public string FeeType { get; set; }
        [JsonProperty("fee_amount")]
        public string FeeAmount { get; set; }
        [JsonProperty("fee_percentage")]
        public string FeePercentage { get; set; }
    }

    public class ProductOptions
    {
        [JsonProperty("option_id")]
        public string OptionId { get; set; }
        [JsonProperty("option_name")]
        public string optionName { get; set; }
        [JsonProperty("option_description")]
        public string OptionDescription { get; set; }
        [JsonProperty("option_type")]
        public string OptionType { get; set; }
        [JsonProperty("option_selection_type")]
        public string OptionSelectiontype { get; set; }
        [JsonProperty("option_count_type")]
        public string OptionCounttype { get; set; }
        [JsonProperty("option_list_type")]
        public string OptionlistType { get; set; }
        [JsonProperty("option_price_type")]
        public string OptionpriceType { get; set; }
        [JsonProperty("option_mandatory")]
        public bool OptionMandatory { get; set; }
        [JsonProperty("option_values")]
        public List<OptionValues> OptionValues { get; set; }
    }

    public class OptionValues
    {
        [JsonProperty("value_id")]
        public string ValueiId { get; set; }
        [JsonProperty("value_name")]
        public string ValueName { get; set; }
        [JsonProperty("value_price")]
        public string ValuePrice { get; set; }
        [JsonProperty("value_price_tax_id")]
        public string ValuePriceTaxId { get; set; }
    }

    public class Product_Cancellation_Policies
    {
        [JsonProperty("cancellation_description")]
        public string CancellationDescription { get; set; }
        [JsonProperty("cancellation_type")]
        public string CancellationType { get; set; }
        [JsonProperty("cancellation_fee_threshold")]
        public int CancellationfeeThreshold { get; set; }
    }

    public class Product_Flags
    {
        [JsonProperty("flag_id")]
        public string FlagId { get; set; }
        [JsonProperty("flag_name")]
        public string FlagName { get; set; }
        [JsonProperty("flag_type")]
        public string FlagType { get; set; }
        [JsonProperty("flag_value_id")]
        public string FlagValueId { get; set; }
        [JsonProperty("flag_value")]
        public string FlagValue { get; set; }
    }

    public class ProductOpeningTimes
    {
        [JsonProperty("opening_time_valid_from")]
        public string OpeningtimeValidFrom { get; set; }
        [JsonProperty("opening_time_valid_till")]
        public string OpeningtimeValidTill { get; set; }
        [JsonProperty("opening_time_details")]
        public List<Opening_Time_Details> OpeningTimeDetails { get; set; }
    }

    public class Opening_Time_Details
    {
        [JsonProperty("opening_time_day")]
        public string OpeningTimeDay { get; set; }
        [JsonProperty("opening_time_start")]
        public string OpeningTimeStart { get; set; }
        [JsonProperty("opening_time_end")]
        public string OpeningTimeEnd { get; set; }
    }

    public class ProductAddonDetails
    {
        [JsonProperty("product_parent_id")]
        public string ProductParentId { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("product_supplier_id")]
        public string ProductSupplierId { get; set; }
        [JsonProperty("product_supplier_name")]
        public string ProductSupplierName { get; set; }
        [JsonProperty("product_title")]
        public string ProductTitle { get; set; }
        [JsonProperty("product_from_price")]
        public string ProductfromPrice { get; set; }
        [JsonProperty("product_currency_code")]
        public string ProductCurrencyCode { get; set; }
        [JsonProperty("product_start_date")]
        public DateTime ProductStartDate { get; set; }
        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }
        [JsonProperty("product_timepicker_visible")]
        public bool ProductTimePickerVisible { get; set; }
        [JsonProperty("product_images")]
        public List<Product_Images1> ProductImages { get; set; }
    }

    public class Product_Images1
    {
        [JsonProperty("image_type")]
        public string ImageType { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }

    public class Product_Guide_Languages
    {
        [JsonProperty("language_codes")]
        public List<string> LanguageCodes { get; set; }
        [JsonProperty("language_type")]
        public string LanguageType { get; set; }
    }

    public class Product_Notes
    {
        [JsonProperty("note_value")]
        public string NoteValue { get; set; }
        [JsonProperty("note_type")]
        public string NoteType { get; set; }
        [JsonProperty("note_date")]
        public string NoteDate { get; set; }
        [JsonProperty("note_creator")]
        public string NoteCreator { get; set; }
    }

    public class Product_Custom_Fields
    {
        [JsonProperty("custom_field_name")]
        public string CustomFieldName { get; set; }
        [JsonProperty("custom_field_value")]
        public string CustomFieldValue { get; set; }
    }
}