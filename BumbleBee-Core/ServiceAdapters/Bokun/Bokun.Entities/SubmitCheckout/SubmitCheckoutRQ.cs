using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.SubmitCheckout
{
    public class SubmitCheckoutRq
    {
        [JsonProperty(PropertyName = "checkoutOption")]
        public long? CheckoutOption { get; set; } = 3; // Enum value for "CUSTOMER_FULL_PAYMENT"

        [JsonProperty(PropertyName = "paymentMethod")]
        public long? PaymentMethod { get; set; } = 1; // Enum value for "VOUCHER"

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; } = "DIRECT_REQUEST"; // default value for Isango booking

        [JsonProperty(PropertyName = "directBooking")]
        public DirectBooking DirectBooking { get; set; }

        [JsonProperty(PropertyName = "sendNotificationToMainContact")]
        public bool SendNotificationToMainContact { get; set; }

        [JsonProperty(PropertyName = "showPricesInNotification")]
        public bool ShowPricesInNotification { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public long? Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "uti")]
        public string Uti { get; set; }

        [JsonProperty(PropertyName = "successUrl")]
        public string SuccessUrl { get; set; }

        [JsonProperty(PropertyName = "errorUrl")]
        public string ErrorUrl { get; set; }

        [JsonProperty(PropertyName = "cancelUrl")]
        public string CancelUrl { get; set; }

        [JsonProperty(PropertyName = "acceptDccQuote")]
        public bool AcceptDccQuote { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "countryIsoCode")]
        public string CountryIsoCode { get; set; }
    }

    public class DirectBooking
    {
        [JsonProperty(PropertyName = "mainContactDetails")]
        public List<Answer> MainContactDetails { get; set; }

        [JsonProperty(PropertyName = "activityBookings")]
        public List<ActivityBookingDto> ActivityBookings { get; set; }

        [JsonProperty(PropertyName = "sendCustomerNotification")]
        public bool SendCustomerNotification { get; set; }

        [JsonProperty(PropertyName = "discountPercentage")]
        public long? DiscountPercentage { get; set; }

        [JsonProperty(PropertyName = "externalBookingReference")]
        public string ExternalBookingReference { get; set; }

        [JsonProperty(PropertyName = "labels")]
        public List<object> Labels { get; set; }
    }

    public class ActivityBookingDto
    {
        [JsonProperty(PropertyName = "activityId")]
        public long? ActivityId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "pickUp")]
        public bool Pickup { get; set; }

        [JsonProperty(PropertyName = "dropOff")]
        public bool Dropoff { get; set; }

        [JsonProperty(PropertyName = "checkedIn")]
        public bool CheckedIn { get; set; }

        [JsonProperty(PropertyName = "startTimeId")]
        public long? StartTimeId { get; set; }

        [JsonProperty(PropertyName = "answers")]
        public List<Answer> Answers { get; set; }

        [JsonProperty(PropertyName = "passengers")]
        public List<Passenger> Passengers { get; set; }

        [JsonProperty(PropertyName = "pickupPlaceId")]
        public int? PickupPlaceId { get; set; }

        [JsonProperty(PropertyName = "pickupDescription")]
        public string PickupDescription { get; set; }

        [JsonProperty(PropertyName = "rateId")]
        public long? RateId { get; set; }

        [JsonProperty(PropertyName = "dropoffPlaceId")]
        public int? DropoffPlaceId { get; set; }

        [JsonProperty(PropertyName = "dropoffDescription")]
        public string DropoffDescription { get; set; }
    }

    public class Passenger
    {
        [JsonProperty(PropertyName = "pricingCategoryId")]
        public long? PricingCategoryId { get; set; }

        [JsonProperty(PropertyName = "passengerDetails")]
        public List<Answer> PassengerDetails { get; set; }

        [JsonProperty(PropertyName = "passengerAnswers")]
        public List<Answer> PassengerAnswers { get; set; }
    }

    public class Answer
    {
        [JsonProperty(PropertyName = "questionId")]
        public string QuestionId { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<string> Values { get; set; }
    }
}