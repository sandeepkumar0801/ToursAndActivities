using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class Item : ItemBase
    {
        [JsonProperty(PropertyName = "ticketType")]
        public string TicketType { get; set; }

        [JsonProperty(PropertyName = "ticketBarCode")]
        public string TicketBarCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public object Message { get; set; }

        [JsonProperty(PropertyName = "image")]
        public object Image { get; set; }
    }

    public class ProductBookingResponse
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "dateOfTravel")]
        public string DateOfTravel { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<Item> Items { get; set; }
    }

    public class ProductsBookingResponse
    {
        [JsonProperty(PropertyName = "product")]
        public List<ProductBookingResponse> Product { get; set; }
    }

    public class BookingResult
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "bookingReference")]
        public string BookingReference { get; set; }

        [JsonProperty(PropertyName = "shortReference")]
        public string ShortReference { get; set; }

        [JsonProperty(PropertyName = "dateOfBooking")]
        public string DateOfBooking { get; set; }

        [JsonProperty(PropertyName = "underName")]
        public UnderNameBase UnderName { get; set; }

        [JsonProperty(PropertyName = "products")]
        public ProductsBookingResponse Products { get; set; }
    }

    public class BookingResponseObject
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "bookingResult")]
        public BookingResult BookingResult { get; set; }
    }
}