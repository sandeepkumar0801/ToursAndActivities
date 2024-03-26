using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using IsangoEntities = Isango.Entities;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{

    public class TicketListRs : EntityBase
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public TicketListData Data { get; set; }
    }

    public class TicketListData
    {
        [JsonProperty(PropertyName = "tickets")]
        public List<Ticket> Tickets { get; set; }
    }

    public class Ticket
    {
        [JsonProperty(PropertyName = "ticket_id")]
        public Int32 TicketId { get; set; }
        [JsonProperty(PropertyName = "ticket_title")]
        public string TicketTitle { get; set; }
        [JsonProperty(PropertyName = "venue_id")]
        public string VenueId { get; set; }
        [JsonProperty(PropertyName = "venue_name")]
        public string VenueName { get; set; }
        [JsonProperty(PropertyName = "txt_language")]
        public string Language { get; set; }
        [JsonProperty(PropertyName = "start_date")]
        public string StartDate { get; set; }
        [JsonProperty(PropertyName = "end_date")]
        public string EndDate { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "locations")]
        public List<Location> Locations { get; set; }
        
    }

    public class Location
    {
        [JsonProperty(PropertyName = "location_name")]
        public string LocationName { get; set; }
        [JsonProperty(PropertyName = "country_name")]
        public string CountryName { get; set; }
    }
}