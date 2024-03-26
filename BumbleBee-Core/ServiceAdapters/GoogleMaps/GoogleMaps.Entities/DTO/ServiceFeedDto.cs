using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class ServiceFeedDto
    {
        [JsonProperty("metadata")]
        public ServiceMetadata Metadata { get; set; }

        [JsonProperty("service")]
        public List<Service> Service { get; set; }
    }

    public class ServiceMetadata
    {
        [JsonProperty("processing_instruction")]
        public string ProcessingInstruction { get; set; }

        [JsonProperty("shard_number")]
        public long ShardNumber { get; set; }

        [JsonProperty("total_shards")]
        public long TotalShards { get; set; }

        [JsonProperty("generation_timestamp")]
        public string GenerationTimestamp { get; set; }
    }

    public class Service
    {
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("localized_service_name")]
        public LocalizedDescription LocalizedServiceName { get; set; }

        [JsonProperty("localized_description")]
        public LocalizedDescription LocalizedDescription { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("ticket_type")]
        public List<TicketType> TicketType { get; set; }

        [JsonProperty("prepayment_type")]
        public string PrepaymentType { get; set; }

        [JsonProperty("integration_type")]
        public string IntegrationType { get; set; }

        [JsonProperty("related_media")]
        public List<RelatedMedia> RelatedMedia { get; set; }

        [JsonProperty("rating")]
        public Rating Rating { get; set; }

        [JsonProperty("rules")]
        public Rules Rules { get; set; }

        [JsonProperty("tours_and_activities_content")]
        public ToursAndActivitiesContent ToursAndActivitiesContent { get; set; }

        [JsonProperty("intake_form")]
        public IntakeForm IntakeForm { get; set; }

        [JsonProperty("location")]
        public List<Location> Location { get; set; }
    }

    public class LocalizedDescription
    {
        [JsonProperty("localized_value")]
        public List<LocalizedValue> LocalizedValue { get; set; }
    }

    public class IntakeFormLocalizedDescription
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("localized_value")]
        public List<LocalizedValue> LocalizedValue { get; set; }
    }

    public class Location
    {
        [JsonProperty("geo")]
        public GeoCordinate Geo { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }
    }

    public class GeoCordinate
    {
        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }

    public class LocalizedValue
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Rating
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("number_of_ratings")]
        public long NumberOfRatings { get; set; }
    }

    public class RelatedMedia
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Rules
    {
        [JsonProperty("cancellation_policy")]
        public CancellationPolicy CancellationPolicy { get; set; }

        [JsonProperty("admission_policy")]
        public string AdmissionPolicy { get; set; }
    }

    public class CancellationPolicy
    {
        [JsonProperty("refund_condition")]
        public List<RefundCondition> RefundCondition { get; set; }
    }

    public enum AdmissionPolicy
    {
        ADMISSION_POLICY_UNSPECIFIED,
        TIME_STRICT,
        TIME_FLEXIBLE,
        TIMED_ENTRY_WITH_FLEXIBLE_DURATION
    }

    public class RefundCondition
    {
        [JsonProperty("min_duration_before_start_time_sec")]
        public long MinDurationBeforeStartTimeSec { get; set; }

        [JsonProperty("refund_percent")]
        public long RefundPercent { get; set; }
    }

    public class TicketType
    {
        [JsonProperty("ticket_type_id")]
        public string TicketTypeId { get; set; }

        [JsonProperty("localized_short_description")]
        public LocalizedDescription LocalizedShortDescription { get; set; }

        [JsonProperty("localized_option_description")]
        public LocalizedDescription LocalizedOptionDescription { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("per_ticket_fee")]
        public PerTicketFee PerTicketFee { get; set; }

        [JsonProperty("per_ticket_intake_form")]
        public IntakeForm IntakeForm { get; set; }
    }

    public class PerTicketFee
    {
        [JsonProperty("taxes")]
        public Price Taxes { get; set; }
    }

    public class Price
    {
        [JsonProperty("price_micros")]
        public string PriceMicros { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }
    }

    public class ToursAndActivitiesContent
    {
        [JsonProperty("highlights")]
        public List<LocalizedDescription> Highlights { get; set; }

        [JsonProperty("inclusions")]
        public List<LocalizedDescription> Inclusions { get; set; }

        [JsonProperty("exclusions")]
        public List<LocalizedDescription> Exclusions { get; set; }

        [JsonProperty("must_know")]
        public List<LocalizedDescription> MustKnow { get; set; }
    }

    public class IntakeForm
    {
        [JsonProperty("field")]
        public List<Field> Field { get; set; }
    }

    public class Field
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("localized_label")]
        public IntakeFormLocalizedDescription Label { get; set; }

        [JsonProperty("value")]
        public List<string> Value { get; set; }

        [JsonProperty("is_required")]
        public bool IsRequired { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ticket_type_restrict")]
        public List<string> TicketIds { get; set; }
    }
}