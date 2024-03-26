using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Ventrata
{
    public class VentrataRedemption
    {
        [JsonProperty(PropertyName = "webhook")]

        public VentrataWebhook Webhook { get; set; }

        [JsonProperty(PropertyName = "booking")]

        public VentrataRedemptionBooking Booking { get; set; }
    }

    public class VentrataWebhook
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "retryOnError")]
        public bool RetryOnError { get; set; }

        [JsonProperty(PropertyName = "useContactLanguage")]
        public bool UseContactLanguage { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "capabilities")]
        public List<string> Capabilities { get; set; }
    }

    public class VentrataRedemptionBooking
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

        [JsonProperty(PropertyName = "utcExpiresAt")]
        public string UtcExpiresAt { get; set; }

        [JsonProperty(PropertyName = "utcConfirmedAt")]
        public string UtcConfirmedAt { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "optionId")]
        public string OptionId { get; set; }

        [JsonProperty(PropertyName = "cancellable")]
        public bool Cancellable { get; set; }

        [JsonProperty(PropertyName = "Cancellation")]
        public object Cancellation { get; set; }

        [JsonProperty(PropertyName = "freesale")]
        public bool Freesale { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        [JsonProperty(PropertyName = "availability")]
        public VentrataRedemptionAvailability Availability { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public VentrataRedemptionContact Contact { get; set; }

        [JsonProperty(PropertyName = "deliveryMethods")]
        public List<string> DeliveryMethods { get; set; }

        [JsonProperty(PropertyName = "voucher")]
        public VentrataRedemptionVoucher Voucher { get; set; }

        [JsonProperty(PropertyName = "unitItems")]
        public List<VentrataRedemptionUnitItem> UnitItems { get; set; }
    }

    public class VentrataRedemptionAvailability
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "localDateTimeStart")]
        public string LocalDateTimeStart { get; set; }

        [JsonProperty(PropertyName = "localDateTimeEnd")]
        public string LocalDateTimeEnd { get; set; }

        [JsonProperty(PropertyName = "allDay")]
        public bool AllDay { get; set; }

        [JsonProperty(PropertyName = "openingHours")]
        public List<object> OpeningHours { get; set; }
    }

    public class VentrataRedemptionContact
    {
        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "locales")]
        public List<object> Locales { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
    }

    public class VentrataRedemptionVoucher
    {
        [JsonProperty(PropertyName = "redemptionMethod")]
        public string RedemptionMethod { get; set; }

        [JsonProperty(PropertyName = "utcRedeemedAt")]
        public object UtcRedeemedAt { get; set; }

        [JsonProperty(PropertyName = "deliveryOptions")]

        public List<object> DeliveryOptions { get; set; }
    }

    public class VentrataRedemptionUnitItem
    {
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "resellerReference")]
        public string ResellerReference { get; set; }

        [JsonProperty(PropertyName = "supplierReference")]
        public string SupplierReference { get; set; }

        [JsonProperty(PropertyName = "unitId")]
        public string UnitId { get; set; }

        [JsonProperty(PropertyName = "ticket")]
        public VentrataRedemptionTicket Ticket { get; set; }
    }

    public class VentrataRedemptionTicket
    {
        [JsonProperty(PropertyName = "redemptionMethod")]

        public string RedemptionMethod { get; set; }
        [JsonProperty(PropertyName = "utcRedeemedAt")]

        public object UtcRedeemedAt { get; set; }
        [JsonProperty(PropertyName = "deliveryOptions")]

        public List<object> DeliveryOptions { get; set; }
    }

}
