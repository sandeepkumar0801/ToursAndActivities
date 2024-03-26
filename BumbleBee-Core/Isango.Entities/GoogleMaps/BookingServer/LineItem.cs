using Isango.Entities.GoogleMaps.BookingServer.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class LineItem
    {
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("start_sec")]
        public long StartSec { get; set; }

        [JsonProperty("duration_sec")]
        public long DurationSec { get; set; }

        [JsonProperty("tickets")]
        public List<OrderedTicket> Tickets { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("status")]
        public Enums.BookingStatus Status { get; set; }

        [JsonProperty("intake_form_answers")]
        public IntakeFormAnswers IntakeFormAnswers { get; set; }

        [JsonProperty("warning_reason")]
        public WarningReason WarningReason { get; set; }
    }

    public class OrderedTicket
    {
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class IntakeFormAnswers
    {
        [JsonProperty("answer")]
        public List<IntakeFormFieldAnswer> IntakeFormFieldAnswers { get; set; }
    }

    public class IntakeFormFieldAnswer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("response")]
        public List<string> Response { get; set; }
    }
}