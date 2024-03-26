using Newtonsoft.Json;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.Booking
{
    public class BookingRequest
    {
        [JsonProperty("customer")]
        public BookingRequestCustomer Customer { get; set; }

        [JsonProperty("fields")]
        public BookingRequestField[] Fields { get; set; }

        [JsonProperty("items")]
        public BookingRequestItem[] Items { get; set; }

        [JsonProperty("supplierId")]
        public int SupplierId { get; set; }

        [JsonProperty("payments")]
        public BookingRequestPayment[] Payments { get; set; }

        [JsonProperty("sendNotifications")]
        public bool SendNotifications { get; set; }

        [JsonProperty("resellerComments")]
        public string ResellerComments { get; set; }

        [JsonProperty("resellerReference")]
        public string ResellerReference { get; set; }

        [JsonProperty("totalAmount")]
        public float TotalAmount { get; set; }
    }

    public class BookingRequestCreditCard
    {
        [JsonProperty("cardName")]
        public string CardName { get; set; }

        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty("cardSecurityNumber")]
        public string CardSecurityNumber { get; set; }

        [JsonProperty("cardToken")]
        public string CardToken { get; set; }

        [JsonProperty("cardType")]
        public string CardType { get; set; }

        [JsonProperty("expiryMonth")]
        public string ExpiryMonth { get; set; }

        [JsonProperty("expiryYear")]
        public string ExpiryYear { get; set; }
    }

    public class BookingRequestCustomer
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        
    }

    public class BookingRequestField
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class BookingRequestItem
    {
        [JsonProperty("startTimeLocal")]
        public string StartTimeLocal { get; set; }

        [JsonProperty("participants")]
        public BookingRequestParticipant[] Participants { get; set; }

        [JsonProperty("pickupLocation")]
        public BookingRequestPickupLocation PickupLocation { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("quantities")]
        public BookingRequestQuantity[] Quantities { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }

        [JsonProperty("subtotal")]
        public float SubTotal { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }
        
    }

    public class BookingRequestExtra
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extraPriceType")]
        public string ExtraPriceType { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    public class BookingRequestImage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("itemUrl")]
        public string ItemUrl { get; set; }

        [JsonProperty("largeSizeUrl")]
        public string LargeSizeUrl { get; set; }

        [JsonProperty("mediumSizeUrl")]
        public string MediumSizeUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }

    public class BookingRequestParticipant
    {
        [JsonProperty("fields")]
        public BookingRequestField[] Fields { get; set; }
    }

    public class BookingRequestQuantity
    {
        [JsonProperty("optionLabel")]
        public string OptionLabel { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("optionPrice")]
        public float OptionPrice { get; set; }
    }

    public class BookingRequestPayment
    {
        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class BookingRequestPickupLocation
    {
        [JsonProperty("additionalInstructions")]
        public string AdditionalInstructions { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("minutesPrior")]
        public int MinutesPrior { get; set; }

        [JsonProperty("pickupInstructions")]
        public string PickupInstructions { get; set; }

        [JsonProperty("pickupTime")]
        public string PickupTime { get; set; }
    }
}