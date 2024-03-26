using Newtonsoft.Json;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.Booking
{
    public class BookingResponse
    {
        [JsonProperty("booking")]
        public Booking Booking { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }
    }

    public class Booking
    {
        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("coupon")]
        public string Coupon { get; set; }

        [JsonProperty("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonProperty("creditCard")]
        public CreditCard CreditCard { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("dateConfirmed")]
        public string DateConfirmed { get; set; }

        [JsonProperty("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("datePaid")]
        public string DatePaid { get; set; }

        [JsonProperty("dateReconciled")]
        public string DateReconciled { get; set; }

        [JsonProperty("dateUpdated")]
        public string DateUpdated { get; set; }

        [JsonProperty("fields")]
        public Field[] Fields { get; set; }

        [JsonProperty("internalNotes")]
        public string InternalNotes { get; set; }

        [JsonProperty("items")]
        public BookingResponseItem[] Items { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("paymentOption")]
        public string PaymentOption { get; set; }

        [JsonProperty("payments")]
        public Payment[] Payments { get; set; }

        [JsonProperty("resellerAlias")]
        public string ResellerAlias { get; set; }

        [JsonProperty("resellerComments")]
        public string ResellerComments { get; set; }

        [JsonProperty("resellerId")]
        public string ResellerId { get; set; }

        [JsonProperty("resellerName")]
        public string ResellerName { get; set; }

        [JsonProperty("resellerReference")]
        public string ResellerReference { get; set; }

        [JsonProperty("resellerSource")]
        public string ResellerSource { get; set; }

        [JsonProperty("resellerUser")]
        public CreatedBy ResellerUser { get; set; }

        [JsonProperty("sendNotifications")]
        public string SendNotifications { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("sourceChannel")]
        public string SourceChannel { get; set; }

        [JsonProperty("sourceReferrer")]
        public string SourceReferrer { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("supplierAlias")]
        public string SupplierAlias { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("supplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("surcharge")]
        public string Surcharge { get; set; }

        [JsonProperty("totalAmount")]
        public string TotalAmount { get; set; }

        [JsonProperty("totalCurrency")]
        public string TotalCurrency { get; set; }

        [JsonProperty("totalDue")]
        public string TotalDue { get; set; }

        [JsonProperty("totalPaid")]
        public string TotalPaid { get; set; }

        [JsonProperty("vouchers")]
        public string[] Vouchers { get; set; }
    }

    public class CreatedBy
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }

    public class CreditCard
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

    public class Customer
    {
        [JsonProperty("aboutUs")]
        public string AboutUs { get; set; }

        [JsonProperty("addressLine")]
        public string AddressLine { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("dob")]
        public string Dob { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("marketing")]
        public string Marketing { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("newsletter")]
        public string Newsletter { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonProperty("skype")]
        public string Skype { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class Field
    {
        [JsonProperty("fieldType")]
        public string FieldType { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("listOptions")]
        public string ListOptions { get; set; }

        [JsonProperty("requiredPerBooking")]
        public string RequiredPerBooking { get; set; }

        [JsonProperty("requiredPerParticipant")]
        public string RequiredPerParticipant { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("visiblePerBooking")]
        public string VisiblePerBooking { get; set; }

        [JsonProperty("visiblePerParticipant")]
        public string VisiblePerParticipant { get; set; }
    }

    public class BookingResponseItem
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("endTimeLocal")]
        public string EndTimeLocal { get; set; }

        [JsonProperty("extras")]
        public Extra[] Extras { get; set; }

        [JsonProperty("participants")]
        public Participant[] Participants { get; set; }

        [JsonProperty("pickupLocation")]
        public PickupLocation PickupLocation { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("quantities")]
        public Quantity[] Quantities { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("startTimeLocal")]
        public string StartTimeLocal { get; set; }

        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("totalItemTax")]
        public string TotalItemTax { get; set; }

        [JsonProperty("totalQuantity")]
        public string TotalQuantity { get; set; }

        [JsonProperty("transferFrom")]
        public string TransferFrom { get; set; }

        [JsonProperty("transferReturn")]
        public string TransferReturn { get; set; }

        [JsonProperty("transferTo")]
        public string TransferTo { get; set; }

        [JsonProperty("vouchers")]
        public Voucher[] Vouchers { get; set; }
    }

    public class Extra
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extraPriceType")]
        public string ExtraPriceType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }
    }

    public class Image
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("itemUrl")]
        public string ItemUrl { get; set; }

        [JsonProperty("largeSizeUrl")]
        public string LargeSizeUrl { get; set; }

        [JsonProperty("mediumSizeUrl")]
        public string MediumSizeUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }

    public class Participant
    {
        [JsonProperty("fields")]
        public Field[] Fields { get; set; }
    }

    public class PickupLocation
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
        public string MinutesPrior { get; set; }

        [JsonProperty("pickupInstructions")]
        public string PickupInstructions { get; set; }

        [JsonProperty("pickupTime")]
        public string PickupTime { get; set; }
    }

    public class Quantity
    {
        [JsonProperty("optionId")]
        public string OptionId { get; set; }

        [JsonProperty("optionLabel")]
        public string OptionLabel { get; set; }

        [JsonProperty("optionPrice")]
        public string OptionPrice { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Voucher
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("expiryDate")]
        public string ExpiryDate { get; set; }

        [JsonProperty("internalNotes")]
        public string InternalNotes { get; set; }

        [JsonProperty("internalReference")]
        public string InternalReference { get; set; }

        [JsonProperty("issueDate")]
        public string IssueDate { get; set; }

        [JsonProperty("sourceOrder")]
        public string SourceOrder { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("valueType")]
        public string ValueType { get; set; }
    }

    public class Payment
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

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

    public class RequestStatus
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("warning")]
        public Warning Warning { get; set; }
    }

    public class Error
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class Warning
    {
        [JsonProperty("warningMessage")]
        public string WarningMessage { get; set; }
    }
}