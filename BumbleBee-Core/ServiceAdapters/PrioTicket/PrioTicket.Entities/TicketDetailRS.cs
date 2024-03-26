using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using IsangoEntities = Isango.Entities;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class TicketDetailRs : EntityBase
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public CreateBookingData Data { get; set; }
    }

    public class CreateBookingData
    {
        [JsonProperty(PropertyName = "ticket_id")]
        public int TicketId { get; set; }

        [JsonProperty(PropertyName = "Ticket_title")]
        public string TicketTitle { get; set; }

        [JsonProperty(PropertyName = "short_description")]
        public string ShortDescription { get; set; }

        [JsonProperty(PropertyName = "long_description")]
        public string LongDescription { get; set; }

        [JsonProperty(PropertyName = "highlights")]
        public string[] Highlights { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }

        [JsonProperty(PropertyName = "combi_ticket")]
        public string CombiTicket { get; set; }

        [JsonProperty(PropertyName = "ticket_entry_notes")]
        public string TicketEntryNotes { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public object[] Tags { get; set; }

        [JsonProperty(PropertyName = "included")]
        public object[] Included { get; set; }

        [JsonProperty(PropertyName = "company_opening_times")]
        public CompanyOpeningTimes[] CompanyOpeningTimes { get; set; }

        [JsonProperty(PropertyName = "book_size_min")]
        public string BookSizeMin { get; set; }

        [JsonProperty(PropertyName = "book_size_max")]
        public string BookSizeMax { get; set; }

        [JsonProperty(PropertyName = "supplier_url")]
        public string SupplierUrl { get; set; }

        [JsonProperty(PropertyName = "ticket_class")]
        public int TicketClass { get; set; }

        [JsonProperty(PropertyName = "start_date")]
        public string StartDate { get; set; }

        [JsonProperty(PropertyName = "end_date")]
        public string EndDate { get; set; }

        [JsonProperty(PropertyName = "booking_start_date")]
        public string BookingStartDate { get; set; }

        [JsonProperty(PropertyName = "images")]
        public string[] Images { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "product_language")]
        public string ProductLanguage { get; set; }

        [JsonProperty(PropertyName = "pickup_points")]
        public string PickupPoints { get; set; }

        [JsonProperty(PropertyName = "pickup_point_details")]
        public IsangoEntities.PickupPointDetails[] PickupPointDetails { get; set; }

        [JsonProperty(PropertyName = "ticket_type_details")]
        public TicketTypeDetails[] TicketTypeDetails { get; set; }

        [JsonProperty(PropertyName = "routes")]
        public Route[] Route { get; set; }
    }

    public class CompanyOpeningTimes
    {
        [JsonProperty(PropertyName = "day")]
        public string Day { get; set; }

        [JsonProperty(PropertyName = "start_from")]
        public string StartFrom { get; set; }

        [JsonProperty(PropertyName = "end_to")]
        public string EndTo { get; set; }
    }

    public class TicketTypeDetails
    {
        [JsonProperty(PropertyName = "start_date")]
        public string StartDate { get; set; }

        [JsonProperty(PropertyName = "end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// Updated value of StartDate without offset in date format "yyyy-MM-dd"
        /// </summary>

        [JsonProperty(PropertyName = "StartDateAsDate")]
        public DateTime StartDateAsDate { get; set; }

        /// <summary>
        /// Updated value of EndDate without offset in date format "yyyy-MM-dd"
        /// </summary>
        [JsonProperty(PropertyName = "EndDateAsDate")]
        public DateTime EndDateAsDate { get; set; }

        [JsonProperty(PropertyName = "ticket_type")]
        public string TicketType { get; set; }

        [JsonProperty(PropertyName = "age_from")]
        public int AgeFrom { get; set; }

        [JsonProperty(PropertyName = "age_to")]
        public int AgeTo { get; set; }

        [JsonProperty(PropertyName = "unit_price")]
        public string UnitPrice { get; set; }

        [JsonProperty(PropertyName = "unit_list_price")]
        public string UnitListPrice { get; set; }

        [JsonProperty(PropertyName = "unit_discount")]
        public string UnitDiscount { get; set; }

        [JsonProperty(PropertyName = "unit_gross_price")]
        public string UnitGrossPrice { get; set; }
    }

    public class RouteLocation
    {
        [JsonProperty(PropertyName = "route_location_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "route_location_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "route_location_description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "route_location_latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "route_location_longitude")]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "route_location_stopover")]
        public bool StopOver { get; set; }
    }

    public class Route
    {
        [JsonProperty(PropertyName = "route_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "route_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "route_color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "route_duration")]
        public string Duration { get; set; }

        [JsonProperty(PropertyName = "route_type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "route_start_time")]
        public string StartTime { get; set; }

        [JsonProperty(PropertyName = "route_end_time")]
        public string EndTime { get; set; }

        [JsonProperty(PropertyName = "route_frequency")]
        public string Frequency { get; set; }

        [JsonProperty(PropertyName = "route_audio_languages")]
        public List<string> AudioLanguages { get; set; }

        [JsonProperty(PropertyName = "route_live_languages")]
        public List<object> LiveLanguages { get; set; }

        [JsonProperty(PropertyName = "route_locations")]
        public List<RouteLocation> Locations { get; set; }
    }
}