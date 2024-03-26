using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.RouteResponse
{
    public class RouteResponse
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
        public List<ItemRoute> Items { get; set; }
    }

    public class ItemRoute
    {
        [JsonProperty("route_id")]
        public string RouteId { get; set; }
        [JsonProperty("route_active")]
        public bool RouteActive { get; set; }
        [JsonProperty("route_name")]
        public string RouteName { get; set; }
        [JsonProperty("route_color")]
        public string RouteColor { get; set; }
        [JsonProperty("route_duration")]
        public int RouteDuration { get; set; }
        [JsonProperty("route_type")]
        public string RouteType { get; set; }
        [JsonProperty("route_start_time")]
        public string RouteStartTime { get; set; }
        [JsonProperty("route_end_time")]
        public string RouteEndTime { get; set; }
        [JsonProperty("route_frequency")]
        public int RouteFrequency { get; set; }
        [JsonProperty("route_audio_languages")]
        public List<string> RouteAudioLanguages { get; set; }
        [JsonProperty("route_live_languages")]
        public List<string> RouteLiveLanguages { get; set; }
        [JsonProperty("route_products")]
        public List<string> RouteProducts { get; set; }
        [JsonProperty("route_locations")]
        public List<Route_Locations> RouteLocations { get; set; }
    }

    public class Route_Locations
    {
        [JsonProperty("route_location_id")]
        public string RouteLocationId { get; set; }
        [JsonProperty("route_location_active")]
        public bool RouteLocationActive { get; set; }
        [JsonProperty("route_location_name")]
        public string RouteLocationName { get; set; }
        [JsonProperty("route_location_stopover")]
        public bool RouteLocationStopOver { get; set; }
    }
}