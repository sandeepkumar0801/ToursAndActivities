using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.AvailabilityListResponse
{
    public class AvailabilityListResponse
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
        [JsonProperty("total_items")]
        public int TotalItems { get; set; }
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("availability_id")]
        public string AvailabilityId { get; set; }
        [JsonProperty("availability_capacity_id")]
        public string AvailabilityCapacityId { get; set; }
        [JsonProperty("availability_active")]
        public bool AvailabilityActive { get; set; }
        [JsonProperty("availability_product_id")]
        public string AvailabilityProductId { get; set; }
        [JsonProperty("availability_admission_type")]
        public string AvailabilityAdmissionType { get; set; }
        [JsonProperty("availability_from_date_time")]
        public string AvailabilityFromDateTime { get; set; }
        [JsonProperty("availability_to_date_time")]
        public string AvailabilityToDateTime { get; set; }
        [JsonProperty("availability_spots")]
        public AvailabilitySpots AvailabilitySpots { get; set; }
        [JsonProperty("availability_created")]
        public DateTime AvailabilityCreated { get; set; }
        [JsonProperty("availability_modified")]
        public DateTime AvailabilityModified { get; set; }

        [JsonProperty("availability_pricing")]
        public List<AvailabilityPricing> AvailabilityPricing { get; set; }
    }

    public class AvailabilitySpots
    {
        [JsonProperty("availability_spots_total")]
        public int AvailabilitySpotsTotal { get; set; }
        [JsonProperty("availability_spots_reserved")]
        public int AvailabilitySpotsReserved { get; set; }
        [JsonProperty("availability_spots_booked")]
        public int AvailabilitySpotsBooked { get; set; }
        [JsonProperty("availability_spots_open")]
        public int AvailabilitySpotsOpen { get; set; }

        
    }

    public class AvailabilityPricing
    {
        [JsonProperty("availability_pricing_variation_amount")]
        public decimal AvailabilityPricingVariationAmount { get; set; }
        [JsonProperty("availability_pricing_variation_price_type")]
        public string AvailabilityPricingVariationPriceType { get; set; }
        [JsonProperty("availability_pricing_variation_product_type_id")]
        public int AvailabilityPricingVariationProductTypeId { get; set; }
        [JsonProperty("availability_pricing_variation_type")]
        public string AvailabilityPricingVariationType { get; set; }
        [JsonProperty("availability_pricing_variation_product_type_discount_included")]
        public bool AvailabilityPricingVariationProductTypeDiscountIncluded { get; set; }
        [JsonProperty("availability_pricing_variation_commission_included")]
        public bool AvailabilityPricingVariationCommissionIncluded { get; set; }

        [JsonProperty("availability_pricing_variation_percentage")]
        public string AvailabilityPricingVariationPercentage { get; set; }
    }
}
