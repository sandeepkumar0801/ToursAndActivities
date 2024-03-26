using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse
{

    public class ProductDetailResponse
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
        [JsonProperty("product")]
        public Product Product { get; set; }
    }

    public class Product
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("product_external_id")]
        public string ProductExternalId { get; set; }
        [JsonProperty("product_supplier_id")]
        public string ProductSupplierId { get; set; }
        [JsonProperty("product_distributor_id")]
        public string ProductDistributorid { get; set; }
        [JsonProperty("product_distributor_name")]
        public string ProductDistributorName { get; set; }
        [JsonProperty("product_reseller_id")]
        public string ProductReselleId { get; set; }
        [JsonProperty("product_reseller_name")]
        public string ProductResellerName { get; set; }
        [JsonProperty("product_source_id")]
        public string ProductSourceId { get; set; }
        [JsonProperty("product_source_name")]
        public string ProductSourceName { get; set; }
        [JsonProperty("product_default_language")]
        public string ProductDefaultLanguage { get; set; }
        [JsonProperty("product_slug")]
        public string ProductSlug { get; set; }
        [JsonProperty("product_from_price")]
        public string ProductFromPrice { get; set; }
        [JsonProperty("product_start_date")]
        public DateTime ProductStartDate { get; set; }
        [JsonProperty("product_end_date")]
        public DateTime ProductEndDate { get; set; }
        [JsonProperty("product_booking_start_date")]
        public DateTime ProductBookingStartDate { get; set; }

        [JsonProperty("product_booking_quantity_min")]
        public int? ProductBookingQuantityMin { get; set; }
        [JsonProperty("product_booking_quantity_max")]
        public int? ProductBookingQuantityMax { get; set; }


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
        public bool ProductRelationDetailsVisible { get; set; }
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
        public bool ProductAvailabilityAssigned { get; set; }
        [JsonProperty("product_past_date_booking_allowed")]
        public bool ProductPastDateBookingAllowed { get; set; }
        [JsonProperty("product_capacity_id")]
        public string ProductCapacityId { get; set; }
        [JsonProperty("product_capacity")]
        public bool ProductCapacity { get; set; }
        [JsonProperty("product_traveldate_required")]
        public bool ProductTravelDateRequired { get; set; }
        [JsonProperty("product_cancellation_allowed")]
        public bool ProductCancellationAllowed { get; set; }
        [JsonProperty("product_overbooking_allowed")]
        public bool ProductOverBookingAllowed { get; set; }
        [JsonProperty("product_capacity_type")]
        public string ProductCapacityType { get; set; }
        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }
        [JsonProperty("product_status")]
        public string ProductStatus { get; set; }
        [JsonProperty("product_catalogue_status")]
        public string ProductCatalogueStatus { get; set; }
        [JsonProperty("product_pickup_point")]
        public string ProductPickupPoint { get; set; }

        [JsonProperty("product_pickup_point_details")]
        public List<ProductPickupPointDetails> ProductPickupPointDetails { get; set; }

        [JsonProperty("product_content")]
        public ProductContent ProductContent { get; set; }
        [JsonProperty("product_redemption_rules")]
        public ProductRedemptionRules ProductRedemptionRules { get; set; }
        [JsonProperty("product_code_settings")]
        public ProductCodeSettings ProductCodeSettings { get; set; }
        [JsonProperty("product_payment_detail")]
        public ProductPaymentDetail ProductPaymentDetail { get; set; }
        [JsonProperty("product_type_seasons")]
        public List<ProductTypeSeasons> ProductTypeSeasons { get; set; }
        [JsonProperty("product_cancellation_policies")]
        public List<ProductCancellationPolicies> ProductCancellationPolicies { get; set; }
        [JsonProperty("product_opening_times")]
        public List<ProductOpeningTimes> ProductOpeningTimes { get; set; }

        [JsonProperty("product_locations")]
        public List<ProductLocations> ProductLocations { get; set; }

        [JsonProperty("product_combi_details")]
        public List<ProductCombiDetails> ProductCombiDetails { get; set; }

        [JsonProperty("product_cluster_details")]
        public List<ProductClusterDetails> ProductClusterDetails { get; set; }

        [JsonProperty("product_categories")]
        public List<string> ProductCategories { get; set; }
        [JsonProperty("product_content_languages")]
        public List<string> ProductContentLanguages { get; set; }
        [JsonProperty("product_guide_languages")]
        public List<ProductGuideLanguages> ProductGuideLanguages { get; set; }
        [JsonProperty("product_custom_fields")]
        public List<ProductCustomFields> ProductCustomFields { get; set; }
        [JsonProperty("product_created")]
        public DateTime ProductCreated { get; set; }
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
        public string ProductViewType { get; set; }
    }

    public class ProductClusterDetails
    {
        [JsonProperty("product_parent_id")]
        public int ProductParentId { get; set; }

        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("product_supplier_id")]
        public int ProductSupplierId { get; set; }

        [JsonProperty("product_supplier_name")]
        public string ProductSupplierName { get; set; }

        [JsonProperty("product_title")]
        public string ProductTitle { get; set; }

        [JsonProperty("product_from_price")]
        public decimal ProductFromPrice { get; set; }

        [JsonProperty("product_currency_code")]
        public string ProductCurrencyCode { get; set; }

        [JsonProperty("product_start_date")]
        public DateTime ProductStartDate { get; set; }

        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }

        [JsonProperty("product_timepicker_visible")]
        public bool ProductTimepickerVisible { get; set; }
    }

    public class ProductContent
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
        public string ProductDurationText { get; set; }
        [JsonProperty("product_supplier_name")]
        public string ProductSupplierName { get; set; }
        [JsonProperty("product_entry_notes")]
        public string ProductEntryNotes { get; set; }
        [JsonProperty("product_favorite")]
        public bool ProductFavorite { get; set; }
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
        public string ProductCodeFormat { get; set; }
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
        public ProductPaymentCurrency ProductPaymentCurrency { get; set; }
    }

    public class ProductPaymentCurrency
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }
    }

    public class ProductTypeSeasons
    {
        [JsonProperty("product_type_season_start_date")]
        public DateTime ProductTypeSeasonStartDate { get; set; }
        [JsonProperty("product_type_season_end_date")]
        public DateTime ProductTypeSeasonEndDate { get; set; }
        [JsonProperty("product_type_season_details")]
        public List<ProductTypeSeasonDetails> ProductTypeSeasonDetails { get; set; }

        [JsonProperty(PropertyName = "StartDateAsDate")]
        public DateTime StartDateAsDate { get; set; }
        [JsonProperty(PropertyName = "EndDateAsDate")]
        public DateTime EndDateAsDate { get; set; }
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
        public string ProductTypePriceTaxId { get; set; }
        [JsonProperty("product_type_pricing")]
        public ProductTypePricing ProductTypePricing { get; set; }


        [JsonProperty("product_type_quantity_variations")]
        public List<ProductTypeQuantityVariations> ProductTypeQuantityVariations { get; set; }

        [JsonProperty("product_type_quantity_min")]
        public int ProductTypeQuantityMin { get; set; }

        [JsonProperty("product_type_quantity_max")]
        public int ProductTypeQuantityMax { get; set; }

        [JsonProperty("product_type_fees")]
        public List<ProductTypeFees> ProductTypeFees { get; set; }

        [JsonProperty(PropertyName = "StartDateAsDate")]
        public DateTime StartDateAsDate { get; set; }
        [JsonProperty(PropertyName = "EndDateAsDate")]
        public DateTime EndDateAsDate { get; set; }
    }

    public class ProductTypeQuantityVariations
    {
        [JsonProperty("product_type_quantity_variation_min")]
        public int ProductTypeQuantityVariationMin { get; set; }

        [JsonProperty("product_type_quantity_variation_max")]
        public int ProductTypeQuantityVariationMax { get; set; }

        [JsonProperty("product_type_quantity_variation_amount")]
        public decimal ProductTypeQuantityVariationAmount { get; set; }
    }

    public class ProductTypePricing
    {
        [JsonProperty("product_type_list_price")]
        public string ProductTypelistPrice { get; set; }
        [JsonProperty("product_type_display_price")]
        public bool ProductTypeDisplayPrice { get; set; }
        [JsonProperty("product_type_sales_price")]
        public string ProductTypeSalesPrice { get; set; }
        [JsonProperty("product_type_resale_price")]
        public string ProductTypeResalePrice { get; set; }
        [JsonProperty("product_type_supplier_price")]
        public string ProductTypeSupplierPrice { get; set; }
    }

    public class ProductTypeFees
    {
        [JsonProperty("fee_type")]
        public string FeeType { get; set; }
        [JsonProperty("fee_amount")]
        public string FeeAmount { get; set; }
    }

    public class ProductCancellationPolicies
    {
        [JsonProperty("cancellation_type")]
        public string CancellationType { get; set; }

        [JsonProperty("cancellation_description")]
        public string CancellationDescription { get; set; }

        [JsonProperty("cancellation_fee_threshold")]
        public int CancellationFeeThreshold { get; set; }
    }

    public class ProductOpeningTimes
    {
        [JsonProperty("opening_time_valid_from")]
        public string OpeningTimeValidFrom { get; set; }
        [JsonProperty("opening_time_valid_till")]
        public string OpeningTimeValidTill { get; set; }
        [JsonProperty("opening_time_details")]
        public List<OpeningTimeDetails> OpeningTimeDetails { get; set; }
    }

    public class OpeningTimeDetails
    {
        [JsonProperty("opening_time_day")]
        public string OpeningTimeDay { get; set; }
        [JsonProperty("opening_time_start")]
        public string OpeningTimeStart { get; set; }
        [JsonProperty("opening_time_end")]
        public string OpeningTimEnd { get; set; }
    }

    public class ProductLocations
    {
        [JsonProperty("location_id")]
        public string Locationid { get; set; }
        [JsonProperty("location_name")]
        public string LocationName { get; set; }
        [JsonProperty("location_type")]
        public string LocationType { get; set; }
        [JsonProperty("location_address")]
        public LocationAddress LocationAddress { get; set; }
        [JsonProperty("location_pickup_point")]
        public bool LocationPickupPoint { get; set; }
    }

    public class LocationAddress
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }

    public class ProductGuideLanguages
    {
        [JsonProperty("language_codes")]
        public List<string> LanguageCodes { get; set; }
        [JsonProperty("language_type")]
        public string LanguageType { get; set; }
    }

    public class ProductCustomFields
    {
        [JsonProperty("custom_field_name")]
        public string CustomFieldName { get; set; }
        [JsonProperty("custom_field_value")]
        public string CustomFieldValue { get; set; }
    }

    public class ProductPickupPointDetails
    {
        [JsonProperty("pickup_point_id")]
        public string PickupPointId { get; set; }

        [JsonProperty("pickup_point_name")]
        public string PickupPointName { get; set; }

        [JsonProperty("pickup_point_description")]
        public string PickupPointDescription { get; set; }

        [JsonProperty("pickup_point_location")]
        public string PickupPointLocation { get; set; }

        [JsonProperty("pickup_point_times")]
        public List<string> PickupPointTimes { get; set; }

        [JsonProperty("pickup_point_availability_dependency")]
        public bool PickupPointAvailabilityDependency { get; set; }
    }

    public class ProductCombiDetails
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
        public string ProductFromPrice { get; set; }
        [JsonProperty("product_currency_code")]
        public string ProductCurrencyCode { get; set; }
        [JsonProperty("product_start_date")]
        public DateTime ProductStartDate { get; set; }
        [JsonProperty("product_booking_window_product_id")]
        public int ProductBookingWindowProductId { get; set; }
        [JsonProperty("product_booking_window_start_time")]
        public int ProductBookingWindowStartTime { get; set; }
        [JsonProperty("product_booking_window_end_time")]
        public int ProductBookingWindowEndTime { get; set; }
        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }
        [JsonProperty("product_timepicker_visible")]
        public bool ProductTimepickerVisible { get; set; }
        [JsonProperty("product_images")]
        public List<Product_Images> ProductImages { get; set; }
    }
    public class Product_Images
    {
        [JsonProperty("image_type")]
        public string ImageType { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}

