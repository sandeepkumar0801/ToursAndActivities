using Newtonsoft.Json;
using System.Collections.Generic;
namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public class ProductFilter
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "api_version")]
        public Api_Version ApiVersion { get; set; }
        [JsonProperty(PropertyName = "products")]
        public List<ProductFilterData> Products { get; set; }
        [JsonProperty(PropertyName = "pagination")]
        public Pagination Pagination { get; set; }
    }

    public class Api_Version
    {
        [JsonProperty(PropertyName = "major")]
        public int Major { get; set; }
        [JsonProperty(PropertyName = "minor")]
        public int Minor { get; set; }
        [JsonProperty(PropertyName = "patch")]
        public string Patch { get; set; }
    }

    public class Pagination
    {
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
        [JsonProperty(PropertyName = "page_size")]
        public int PageSize { get; set; }
    }

    public class ProductFilterData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "city_name")]
        public string CityName { get; set; }
        [JsonProperty(PropertyName = "city_id")]
        public string CityId { get; set; }
        [JsonProperty(PropertyName = "country_name")]
        public string CountryName { get; set; }
        [JsonProperty(PropertyName = "country_id")]
        public string CountryId { get; set; }
        [JsonProperty(PropertyName = "tag_ids")]
        public List<string> TagIds { get; set; }
        [JsonProperty(PropertyName = "images")]
        public List<ImageFilterData> Images { get; set; }
        [JsonProperty(PropertyName = "product_url")]
        public string ProductUrl { get; set; }
        [JsonProperty(PropertyName = "product_checkout_url")]
        public string ProductCheckoutUrl { get; set; }
        [JsonProperty(PropertyName = "promo_label")]
        public string PromoLabel { get; set; }
        [JsonProperty(PropertyName = "tagline")]
        public string Tagline { get; set; }
        [JsonProperty(PropertyName = "is_package")]
        public bool IsPackage { get; set; }
        [JsonProperty(PropertyName = "geolocation")]
        public Geolocation Geolocation { get; set; }
        [JsonProperty(PropertyName = "distance")]
        public object Distance { get; set; }
        [JsonProperty(PropertyName = "venue")]
        public VenueFilterData Venue { get; set; }
        [JsonProperty(PropertyName = "ratings")]
        public Ratings Ratings { get; set; }
        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }
        [JsonProperty(PropertyName = "prediscount_price")]
        public object PrediscountPrice { get; set; }
        [JsonProperty(PropertyName = "product_slug")]
        public string ProductSlug { get; set; }
        [JsonProperty(PropertyName = "languages")]
        public List<string> Languages { get; set; }
        [JsonProperty(PropertyName = "sale_status")]
        public string SaleStatus { get; set; }
        [JsonProperty(PropertyName = "sale_status_reason")]
        public string SaleStatusReason { get; set; }
        [JsonProperty(PropertyName = "sale_status_expected_reopen")]
        public object SaleStatusExpectedReopen { get; set; }
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }
        [JsonProperty(PropertyName = "whats_included")]
        public string WhatsIncluded { get; set; }
        [JsonProperty(PropertyName = "whats_excluded")]
        public object WhatsExcluded { get; set; }
        [JsonProperty(PropertyName = "display_price")]
        public float DisplayPrice { get; set; }
        [JsonProperty(PropertyName = "display_booking_fee")]
        public object DisplayBookingFee { get; set; }
        [JsonProperty(PropertyName = "best_time_to_visit")]
        public List<string> BestTimeToVisit { get; set; }
        [JsonProperty(PropertyName = "live_guide_languages")]
        public object LiveGuideLanguages { get; set; }
        [JsonProperty(PropertyName = "audio_guide_languages")]
        public object AudioGuideLanguages { get; set; }
        [JsonProperty(PropertyName = "starting_time")]
        public object StartingTime { get; set; }
        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public object Duration { get; set; }
        [JsonProperty(PropertyName = "starting_point")]
        public Starting_Point StartingPoint { get; set; }
        [JsonProperty(PropertyName = "wheelchair_access")]
        public bool WheelchairAccess { get; set; }
        [JsonProperty(PropertyName = "skip_line")]
        public bool SkipLine { get; set; }
        [JsonProperty(PropertyName = "smartphone_ticket")]
        public bool SmartphoneTicket { get; set; }
        [JsonProperty(PropertyName = "advance_arrival_time")]
        public object AdvanceArrivalTime { get; set; }
        [JsonProperty(PropertyName = "last_admission_window")]
        public string LastAdmissionWindow { get; set; }
        [JsonProperty(PropertyName = "description")]
        public object Description { get; set; }
        [JsonProperty(PropertyName = "highlights")]
        public object Highlights { get; set; }
        [JsonProperty(PropertyName = "safety_measures")]
        public Safety_Measures SafetyMeasures { get; set; }
        [JsonProperty(PropertyName = "exhibitions")]
        public object Exhibitions { get; set; }
        [JsonProperty(PropertyName = "reviews")]
        public object Reviews { get; set; }
        [JsonProperty(PropertyName = "instant_ticket_delivery")]
        public bool InstantTicketDelivery { get; set; }
        [JsonProperty(PropertyName = "discount_percentage")]
        public float DiscountPercentage { get; set; }
        [JsonProperty(PropertyName = "promotion_highlight")]
        public object PromotionHighlight { get; set; }
        [JsonProperty(PropertyName = "in_packages")]
        public object InPackages { get; set; }
        [JsonProperty(PropertyName = "package_products")]
        public object PackageProducts { get; set; }
    }

    public class Geolocation
    {
        [JsonProperty(PropertyName = "lat")]
        public float Lat { get; set; }
        [JsonProperty(PropertyName = "lng")]
        public float Lng { get; set; }
    }

    public class VenueFilterData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
    }

    public class Ratings
    {
        [JsonProperty(PropertyName = "average")]
        public float Average { get; set; }
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }

    public class Starting_Point
    {
        [JsonProperty(PropertyName = "lng")]
        public float Lng { get; set; }
        [JsonProperty(PropertyName = "city_id")]
        public int CityId { get; set; }
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "label")]
        public object Label { get; set; }
        [JsonProperty(PropertyName = "lat")]
        public float Lat { get; set; }
    }

    public class Safety_Measures
    {
        [JsonProperty(PropertyName = "has_intensified_cleaning")]
        public bool HasIntensifiedCleaning { get; set; }
        [JsonProperty(PropertyName = "has_social_distancing")]
        public bool HasSocialDistancing { get; set; }
        [JsonProperty(PropertyName = "has_hand_sanitizer_available")]
        public bool HasHandSanitizerAvailable { get; set; }
        [JsonProperty(PropertyName = "has_reduced_capacity")]
        public bool HasReducedCapacity { get; set; }
        [JsonProperty(PropertyName = "waiting_time")]
        public object WaitingTime { get; set; }
        [JsonProperty(PropertyName = "online_ticketing_only")]
        public bool OnlineTicketingOnly { get; set; }
        [JsonProperty(PropertyName = "limited_duration")]
        public object LimitedDuration { get; set; }
        [JsonProperty(PropertyName = "gloves")]
        public object Gloves { get; set; }
        [JsonProperty(PropertyName = "cash_accepted")]
        public bool CashAccepted { get; set; }
        [JsonProperty(PropertyName = "reduced_capacity")]
        public object ReducedCapacity { get; set; }
        [JsonProperty(PropertyName = "face_mask")]
        public object FaceMask { get; set; }
        [JsonProperty(PropertyName = "has_temperature_checks")]
        public bool HasTemperatureChecks { get; set; }
    }

    public class ImageFilterData
    {
        [JsonProperty(PropertyName = "credits")]
        public object Credits { get; set; }
        [JsonProperty(PropertyName = "large")]
        public string Large { get; set; }
        [JsonProperty(PropertyName = "medium")]
        public string Medium { get; set; }
        [JsonProperty(PropertyName = "small")]
        public string Small { get; set; }
    }
}
