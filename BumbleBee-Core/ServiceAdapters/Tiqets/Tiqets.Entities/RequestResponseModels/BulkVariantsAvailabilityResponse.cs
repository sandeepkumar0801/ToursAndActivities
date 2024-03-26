using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class BulkVariantsAvailabilityResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool? Success { get; set; }

        [JsonProperty(PropertyName = "capped_at")]
        public string CappedAt { get; set; }

        [JsonProperty(PropertyName = "sales_enabled")]
        public bool? SalesEnabled { get; set; }

        [JsonProperty(PropertyName = "dynamic_pricing")]
        public bool? DynamicPricing { get; set; }

        [JsonProperty(PropertyName = "dates")]
        public List<Dates> Dates { get; set; }

        [JsonProperty(PropertyName = "max_tickets_per_order")]
        public int? MaxTicketsPerOrder { get; set; }

        [JsonProperty(PropertyName = "variants")]
        public List<VariantsDescription> Variants { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public List<Groups> Groups { get; set; }
    }

    public class Dates
    {
        [JsonProperty(PropertyName = "availability")]
        public int? Availability { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "timeslots")]
        public List<TimeSlots> TimeSlots { get; set; }
    }

    public class TimeSlots
    {
        [JsonProperty(PropertyName = "availability")]
        public int? Availability { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "variants")]
        public List<Variants> Variants { get; set; }
    }

    public class Variants
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "max_tickets")]
        public int MaxTickets { get; set; }

        [JsonProperty(PropertyName = "price_mediation")]
        public Isango.Entities.Tiqets.PriceComponent PriceMediation { get; set; }

        [JsonProperty(PropertyName = "pricing_restriction")]
        public int? PricingRestriction { get; set; }
    }

    public class PriceMediation
    {
        [JsonProperty(PropertyName = "sale_ticket_value_incl_vat")]
        public decimal? SaleTicketValueIncVat { get; set; }

        [JsonProperty(PropertyName = "booking_fee_incl_vat")]
        public decimal? BookingFeeIncVat { get; set; }

        [JsonProperty(PropertyName = "total_retail_price_incl_vat")]
        public decimal? TotalRetailPriceIncVat { get; set; }

        [JsonProperty(PropertyName = "distributor_commission_excl_vat")]
        public decimal? DistributorCommissionExclVat { get; set; }
    }

    public class VariantsDescription
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "dynamic_variant_pricing")]
        public bool? DynamicVariantPricing { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public string[] Languages { get; set; }

        [JsonProperty(PropertyName = "requires_visitors_details")]
        public List<string> RequiresVisitorsDetails { get; set; }

        [JsonProperty(PropertyName = "valid_with_variant_ids")]
        public int[] ValidWithVariantIds { get; set; }

        [JsonProperty(PropertyName = "variant_type")]
        public string VariantType { get; set; }

        [JsonProperty(PropertyName = "group_ids")]
        public int?[] GroupIds { get; set; }

        [JsonProperty(PropertyName = "cancellation")]
        public Cancellation Cancellation { get; set; }
    }

    public class Cancellation
    {
        [JsonProperty(PropertyName = "window")]
        public int Window { get; set; }

        //[JsonProperty(PropertyName = "is_included")]
        //public bool? IsIncluded { get; set; }

        [JsonProperty(PropertyName = "policy")]
        public string Policy { get; set; }
    }

    public class Groups
    {
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}