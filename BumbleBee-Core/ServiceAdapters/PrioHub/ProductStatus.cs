namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductStatus
    {
        ProductThirdParty = 0,//product_third_party
        ProductSeasonalPricing = 1,//product_seasonal_pricing
        ProductQuantityPricing = 2,//product_quantity_pricing
        ProductDailyPricing = 3,//product_daily_pricing
        ProductDynamicPricing = 4,//product_dynamic_pricing
        
        ProductCluster = 5,//product_cluster
        ProductCombi = 6,//product_combi
        ProductAddon = 7,//product_addon
        ProductRelationDetailsVisible = 8,//product_relation_details_visible
        ProductTimepickerVisible = 9,//product_timepicker_visible

    }
}