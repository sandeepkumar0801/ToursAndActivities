using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.GoogleMaps.BookingServer
{
    public class OrderFulfillability
    {
        [JsonProperty("result")]
        public OrderFulfillabilityResult OrderFulfillabilityResult { get; set; }

        [JsonProperty("item_fulfillability")]
        public List<LineItemFulfillability> ItemFulfillability { get; set; }

        [JsonProperty("unfulfillable_reason")]
        public string UnfulfillableReason { get; set; }

        public string TokenId { get; set; }
    }

    public class Fees
    {
        [JsonProperty("per_ticket_fee")]
        public List<SpecificPerTicketFee> PerTicketFees { get; set; }

        [JsonProperty("per_order_fee")]
        public List<SpecificPerOrderFee> PerOrderFees { get; set; }
    }

    public class SpecificPerTicketFee
    {
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }

        [JsonProperty("fee_name")]
        public string FeeName { get; set; }

        [JsonProperty("fee_amount")]
        public Price Fees { get; set; }
    }

    public class SpecificPerOrderFee
    {
        [JsonProperty("fee_name")]
        public string FeeName { get; set; }

        [JsonProperty("fee_amount")]
        public Price Fees { get; set; }
    }

    public enum OrderFulfillabilityResult
    {
        // Default value: Don't use.
        ORDER_FULFILLABILITY_RESULT_UNSPECIFIED = 0,

        // The order can be fulfilled.
        CAN_FULFILL = 1,

        // The order cannot be fulfilled due to one or more unfulfillable line
        // item(s).
        UNFULFILLABLE_LINE_ITEM = 2,

        // The combination of the line items requested cannot be fulfilled.
        UNFULFILLABLE_SERVICE_COMBINATION = 3,

        // The order cannot be fulfilled due to reasons that do not fall into the
        // categories above.
        ORDER_UNFULFILLABLE_OTHER_REASON = 4
    }

    public class LineItemFulfillability
    {
        public string AvailabilityReferenceId { get; set; }

        [JsonProperty("item")]
        public LineItem LineItem { get; set; }

        [JsonProperty("result")]
        public ItemFulfillabilityResult FulfillabilityResult { get; set; }

        [JsonProperty("unfulfillable_reason")]
        public string UnfulfillableReason { get; set; }

        [JsonProperty("availability ")]
        public UpdatedAvailability Availability { get; set; }

        //[JsonProperty("ticket_type")]
        //public List<TicketType> TicketType { get; set; }
        [JsonProperty("violated_ticket_constraint")]
        public List<ViolatedTicketConstraint> ViolatedTicketConstraints { get; set; }
    }

    // The result of a line item fulfillability check.
    public enum ItemFulfillabilityResult
    {
        // Default value: Don't use.
        ITEM_FULFILLABILITY_RESULT_UNSPECIFIED = 0,

        // This line item can be fulfilled.
        CAN_FULFILL = 1,

        // No adequate availability for the slot requested.
        SLOT_UNAVAILABLE = 2,

        // Child tickets cannot be booked without an adult ticket.
        CHILD_TICKETS_WITHOUT_ADULT = 6,

        // The combination of ticket types requested cannot be fulfilled.
        UNFULFILLABLE_TICKET_COMBINATION = 3,

        // The total price of this line item is not correct.
        INCORRECT_PRICE = 4,

        // The line item cannot be fulfilled since a ticket constraint specified by
        // the partner has been violated.
        TICKET_CONSTRAINT_VIOLATED = 7,

        // The line item cannot be fulfilled for reasons that do not fall into
        // the categories above.
        ITEM_UNFULFILLABLE_OTHER_REASON = 5
    }

    public class UpdatedAvailability
    {
        [JsonProperty("spots_open")]
        public int SpotsOpen { get; set; }
    }

    public class ViolatedTicketConstraint
    {
        [JsonProperty("min_ticket_count")]
        public string MinTicketCount { get; set; }

        [JsonProperty("max_ticket_count")]
        public string MaxTicketCount { get; set; }

        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
    }
}