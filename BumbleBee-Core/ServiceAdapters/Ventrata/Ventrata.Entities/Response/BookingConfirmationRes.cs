using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Response
{
    public class BookingConfirmationRes
    {
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "testMode")]
        public bool TestMode { get; set; }

        [JsonProperty(PropertyName = "resellerReference")]
        public string ResellerReference { get; set; }

        [JsonProperty(PropertyName = "supplierReference")]
        public string SupplierReference { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "optionId")]
        public string OptionId { get; set; }

        [JsonProperty(PropertyName = "cancellable")]
        public bool Cancellable { get; set; }

        [JsonProperty(PropertyName = "cancellation")]
        public object Cancellation
        {
            get
            {
                return this?.BookedProduct?.ApiCancellationPolicy ?? string.Empty;
            }
        }

        [JsonProperty(PropertyName = "freesale")]
        public bool Freesale { get; set; }

        [JsonProperty(PropertyName = "availabilityId")]
        public DateTime AvailabilityId { get; set; }

        [JsonProperty(PropertyName = "availability")]
        public AvailabilityInBookingConfirmationRes Availability { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public ContactInBookingConfirmationRes Contact { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        [JsonProperty(PropertyName = "deliveryMethods")]
        public string[] DeliveryMethods { get; set; }

        [JsonProperty(PropertyName = "voucher")]
        public VoucherInBookingConfirmationRes Voucher { get; set; }

        [JsonProperty(PropertyName = "unitItems")]
        public UnititemInBookingConfirmationResponse[] UnitItems { get; set; }

        [JsonProperty(PropertyName = "checkedIn")]
        public bool CheckedIn { get; set; }

        [JsonProperty(PropertyName = "checkinAvailable")]
        public bool CheckinAvailable { get; set; }

        [JsonProperty(PropertyName = "checkinUrl")]
        public object CheckinUrl { get; set; }

        [JsonProperty(PropertyName = "meetingPoint")]
        public string MeetingPoint { get; set; }

        [JsonProperty(PropertyName = "meetingPointCoordinates")]
        public object MeetingPointCoordinates { get; set; }

        [JsonProperty(PropertyName = "meetingLocalDateTime")]
        public DateTime? MeetingLocalDateTime { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }

        [JsonProperty(PropertyName = "durationAmount")]
        public string DurationAmount { get; set; }

        [JsonProperty(PropertyName = "durationUnit")]
        public string DurationUnit { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "primary")]
        public bool Primary { get; set; }

        //[JsonProperty(PropertyName = "uuid")]
        //public Pricing pricing { get; set; }

        [JsonProperty(PropertyName = "pickupRequested")]
        public bool PickupRequested { get; set; }

        [JsonProperty(PropertyName = "pickupPoint")]
        public object PickupPoint { get; set; }

        [JsonProperty(PropertyName = "adjustments")]
        public object[] Adjustments { get; set; }

        [JsonProperty(PropertyName = "offerCode")]
        public string offerCode { get; set; }

        [JsonProperty(PropertyName = "offerTitle")]
        public string OfferTitle { get; set; }

        [JsonProperty(PropertyName = "offerComparisons")]
        public object[] OfferComparisons { get; set; }

        [JsonProperty(PropertyName = "offerIsCombination")]
        public bool OfferIsCombination { get; set; }

        [JsonProperty(PropertyName = "product")]
        public VentrataBookedProduct BookedProduct { get; set; }

        [JsonProperty(PropertyName = "isPackage")]
        public bool? IsPackage { get; set; }
        

    }

    public class VentrataBookedOptions
    {
        public string cancellationCutoff { get; set; }
        public int cancellationCutoffAmount { get; set; }
        public string cancellationCutoffUnit { get; set; }
    }

    public class VentrataBookedProduct
    {
        [JsonProperty(PropertyName = "redemptionInstructions")]
        public string RedemptionInstructions { get; set; }

        [JsonProperty(PropertyName = "cancellationPolicy")]
        public string ApiCancellationPolicy { get; set; }

        [JsonProperty(PropertyName = "options")]
        public List<VentrataBookedOptions> BookedOptions { get; set; }
    }

    public class AvailabilityInBookingConfirmationRes
    {
        [JsonProperty(PropertyName = "id")]
        public DateTime Id { get; set; }

        [JsonProperty(PropertyName = "localDateTimeStart")]
        public DateTime LocalDateTimeStart { get; set; }

        [JsonProperty(PropertyName = "localDateTimeEnd")]
        public DateTime LocalDateTimeEnd { get; set; }

        [JsonProperty(PropertyName = "allDay")]
        public bool AllDay { get; set; }

        [JsonProperty(PropertyName = "openingHours")]
        public OpeninghourInBookingConfirmationRes[] OpeningHours { get; set; }
    }

    public class OpeninghourInBookingConfirmationRes
    {
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
    }

    public class ContactInBookingConfirmationRes
    {
        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "locales")]
        public string[] Locales { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public object Notes { get; set; }
    }

    public class VoucherInBookingConfirmationRes
    {
        [JsonProperty(PropertyName = "redemptionMethod")]
        public string RedemptionMethod { get; set; }

        [JsonProperty(PropertyName = "utcRedeemedAt")]
        public object UtcRedeemedAt { get; set; }

        [JsonProperty(PropertyName = "deliveryOptions")]
        public DeliveryoptionAtVoucherLevel[] DeliveryOptions { get; set; }
    }

    public class DeliveryoptionAtVoucherLevel
    {
        [JsonProperty(PropertyName = "deliveryFormat")]
        public string DeliveryFormat { get; set; }

        [JsonProperty(PropertyName = "deliveryValue")]
        public string DeliveryValue { get; set; }
    }

    public class UnititemInBookingConfirmationResponse
    {
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "resellerReference")]
        public object ResellerReference { get; set; }

        [JsonProperty(PropertyName = "supplierReference")]
        public string SupplierReference { get; set; }

        [JsonProperty(PropertyName = "unitId")]
        public string UnitId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "utcRedeemedAt")]
        public object UtcRedeemedAt { get; set; }

        [JsonProperty(PropertyName = "ticket")]
        public TicketAtPaxLevel Ticket { get; set; }

        [JsonProperty(PropertyName = "unit")]
        public Unit Unit { get; set; }
    }

    public class TicketAtPaxLevel
    {
        [JsonProperty(PropertyName = "redemptionMethod")]
        public string RedemptionMethod { get; set; }

        [JsonProperty(PropertyName = "utcRedeemedAt")]
        public object UtcRedeemedAt { get; set; }

        [JsonProperty(PropertyName = "deliveryOptions")]
        public DeliveryoptionAtPaxLevel[] DeliveryOptions { get; set; }
    }

    public class DeliveryoptionAtPaxLevel
    {
        [JsonProperty(PropertyName = "deliveryFormat")]
        public string DeliveryFormat { get; set; }

        [JsonProperty(PropertyName = "deliveryValue")]
        public string DeliveryValue { get; set; }
    }
}