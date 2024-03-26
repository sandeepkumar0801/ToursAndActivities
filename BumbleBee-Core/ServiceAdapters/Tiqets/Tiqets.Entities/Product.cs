using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public class Product
    {
        [JsonProperty(PropertyName = "geolocation")]
        public GeoLocation GeoLocation { get; set; } //Latitude and Longitude

        [JsonProperty(PropertyName = "languages")]
        public string[] Languages { get; set; }

        [JsonProperty(PropertyName = "product_slug")]
        public string ProductSlug { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "city_id")]
        public string CityId { get; set; }

        [JsonProperty(PropertyName = "tag_ids")]
        public string[] TagIds { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "country_id")]
        public string CountryId { get; set; }

        [JsonProperty(PropertyName = "whats_included")]
        public string Included { get; set; }

        [JsonProperty(PropertyName = "venue")]
        public Venue Venue { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "product_url")]
        public string ProductUrl { get; set; }

        [JsonProperty(PropertyName = "country_name")]
        public string CountryName { get; set; }

        [JsonProperty(PropertyName = "ratings")]
        public Rating Rating { get; set; }

        [JsonProperty(PropertyName = "whats_excluded")]
        public string Excluded { get; set; }

        [JsonProperty(PropertyName = "tagline")]
        public string TagLine { get; set; }

        [JsonProperty(PropertyName = "city_name")]
        public string CityName { get; set; }

        [JsonProperty(PropertyName = "sale_status_expected_reopen")]
        public string SaleStatusExpectedReopen { get; set; }

        [JsonProperty(PropertyName = "sale_status_reason")]
        public string SaleStatusReason { get; set; }

        [JsonProperty(PropertyName = "sale_status")]
        public string SaleStatus { get; set; }

        [JsonProperty(PropertyName = "product_checkout_url")]
        public string ProductCheckoutUrl { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "starting_point")]
        public StartingPoint StartingPoint { get; set; }

        [JsonProperty(PropertyName = "skip_line")]
        public bool SkipLine { get; set; }

        [JsonProperty(PropertyName = "smartphone_ticket")]
        public bool SmartPhoneTicket { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }

        [JsonProperty(PropertyName = "instant_ticket_delivery")]
        public bool Instant_Ticket_Delivery { get; set; }

		//[JsonProperty(PropertyName = "is_package")]
		//public bool Is_Package { get; set; }

		[JsonProperty(PropertyName = "package_products")]
		public List<int> package_products { get; set; }

		[JsonProperty(PropertyName = "images")]
        public List<Image> Images { get; set; }

		[JsonProperty(PropertyName = "is_package")]
		public bool is_package { get; set; }

		[JsonProperty(PropertyName = "display_price")]
		public double display_price { get; set; }

		[JsonProperty(PropertyName = "live_guide_languages")]
		public string live_guide_languages { get; set; }

		[JsonProperty(PropertyName = "audio_guide_languages")]
		public string audio_guide_languages { get; set; }

		[JsonProperty(PropertyName = "starting_time")]
		public string starting_time { get; set; }

		[JsonProperty(PropertyName = "wheelchair_access")]
		public bool wheelchair_access { get; set; }

		[JsonProperty(PropertyName = "supplier")]
		public Supplier supplier { get; set; }

        [JsonProperty(PropertyName = "marketing_restrictions")]
        public List<string> marketing_restrictions { get; set; }
    }

    public class Image {
        [JsonProperty(PropertyName = "large")]
        public string Large { get; set; }
    }

    public class StartingPoint
    {
        [JsonProperty(PropertyName = "lat")]
        public string Lat { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public string Lng { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "city_id")]
        public string CityId { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
    }

	public class Supplier
	{
		[JsonProperty(PropertyName = "name")]
		public string name { get; set; }

		[JsonProperty(PropertyName = "city")]
		public string city { get; set; }

	}
}