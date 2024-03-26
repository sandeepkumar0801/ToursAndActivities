using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingRequest
{
    public class CreateBookingRequest
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
    public class Data
    {
        [JsonProperty("order")]
        public Order Order { get; set; }
    }
    public class Order
    {
        [JsonProperty("order_distributor_id")]
        public string OrderDistributorId { get; set; }
        [JsonProperty("order_external_reference")]
        public string OrderExternalReference { get; set; }
        [JsonProperty("order_language")]
        public string OrderLanguage { get; set; }
        [JsonProperty("order_settlement_type")]
        public string OrderSettlementType { get; set; }
        [JsonProperty("order_options")]
        public OrderOptions OrderOptions { get; set; }
        [JsonProperty("order_contacts")]
        public List<OrderContacts> OrderContacts { get; set; }
        [JsonProperty("order_bookings")]
        public List<OrderBookings> OrderBookings { get; set; }
        [JsonProperty("order_custom_fields")]
        public List<OrderCustomFields> OrderCustomFields { get; set; }
    }

    public class OrderOptions
    {
        [JsonProperty("email_options")]
        public EmailOptions EmailOptions { get; set; }
    }

    public class EmailOptions
    {
        [JsonProperty("email_types")]
        public EmailTypes EmailTypes { get; set; }
        [JsonProperty("email_recipients")]
        public List<EmailRecipients> EmailRecipients { get; set; }
    }

    public class EmailTypes
    {
        [JsonProperty("send_tickets")]
        public bool SendTickets { get; set; }
        [JsonProperty("send_receipt")]
        public bool SendReceipt { get; set; }
    }

    public class EmailRecipients
    {
        [JsonProperty("recipients_name")]
        public string RecipientsName { get; set; }
        [JsonProperty("recipients_address")]
        public string RecipientsAddress { get; set; }
    }

    public class OrderContacts
    {
        [JsonProperty("contact_name_first")]
        public string ContactNameFirst { get; set; }
        [JsonProperty("contact_name_last")]
        public string ContactNameLast { get; set; }
        [JsonProperty("contact_email")]
        public string ContactEmail { get; set; }
    }
    public class OrderBookings
    {
        [JsonProperty("reservation_reference")]
        public string ReservationReference { get; set; }
        [JsonProperty("booking_external_reference")]
        public string BookingExternalReference { get; set; }
        [JsonProperty("booking_option_type")]
        public string BookingOptionType { get; set; }
    }

    public class OrderCustomFields
    {
        [JsonProperty("custom_field_name")]
        public string CustomFieldName { get; set; }
        [JsonProperty("custom_field_value")]
        public string CustomFieldValue { get; set; }
    }
}