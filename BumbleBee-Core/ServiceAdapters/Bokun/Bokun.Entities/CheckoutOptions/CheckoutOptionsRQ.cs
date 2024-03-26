using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.CheckoutOptions
{
    public class CheckoutOptionsRq
    {
        [JsonProperty("mainContactDetails")]
        public List<Answer> MainContactDetails { get; set; }

        [JsonProperty("activityBookings")]
        public List<Activitybooking> ActivityBookings { get; set; }

        [JsonProperty("sendCustomerNotification")]
        public bool SendCustomerNotification { get; set; }

        [JsonProperty("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonProperty("externalBookingReference")]
        public string ExternalBookingReference { get; set; }

        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("currencyISOCode")]
        public string CurrencyISOCode { get; set; }
    }

    public class Activitybooking
    {
        [JsonProperty("activityId")]
        public int ActivityId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("pickup")]
        public bool Pickup { get; set; }

        [JsonProperty("dropoff")]
        public bool Dropoff { get; set; }

        [JsonProperty("checkedIn")]
        public bool CheckedIn { get; set; }

        [JsonProperty("customized")]
        public bool Customized { get; set; }

        [JsonProperty("passengers")]
        public List<PassengerDto> Passengers { get; set; }

        [JsonProperty("pickupPlaceId")]
        public int? PickupPlaceId { get; set; }

        [JsonProperty("dropoffPlaceId")]
        public int? DropOffPlaceId { get; set; }

        [JsonProperty("pickupDescription")]
        public string PickupDescription { get; set; }

        [JsonProperty("dropoffDescription")]
        public string DropoffDescription { get; set; }

        [JsonProperty("rateId")]
        public int? RateId { get; set; }

        [JsonProperty("startTimeId")]
        public int? StartTimeId { get; set; }
    }

    public class PassengerDto
    {
        [JsonProperty("pricingCategoryId")]
        public int PricingCategoryId { get; set; }

        [JsonProperty("passengerDetails")]
        public List<Answer> PassengerDetails { get; set; }

        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }
    }
}