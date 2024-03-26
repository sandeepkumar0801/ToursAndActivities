using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class BookingRQ
    {
        //[JsonProperty(PropertyName = "data")]
        //public BookData BookingData { get; set; }

        [JsonProperty(PropertyName = "ticketTypes")]
        public List<BookTicketType> TicketTypes { get; set; }

        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "alternateEmail")]
        public string AlternateEmail { get; set; }
        [JsonProperty(PropertyName = "mobilePrefix")]
        public string MobilePrefix { get; set; }
        [JsonProperty(PropertyName = "mobileNumber")]
        public int MobileNumber { get; set; }

        [JsonProperty(PropertyName = "groupBooking")]
        public bool? GroupBooking { get; set; }
        [JsonProperty(PropertyName = "groupName")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "groupNoOfMember")]
        public int? GroupNumberOfMember { get; set; }
        [JsonProperty(PropertyName = "isSingleCodeForGroup")]
        public bool? IsSingleCodeForGroup { get; set; }

        [JsonProperty(PropertyName = "paymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonProperty(PropertyName = "creditCardCurrencyId")]
        public int? CreditCardCurrencyId { get; set; }

        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }

    }

    public class BookData
    {
        [JsonProperty(PropertyName = "ticketTypes")]
        public List<BookTicketType> TicketTypes { get; set; }

        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "alternateEmail")]
        public string AlternateEmail { get; set; }
        [JsonProperty(PropertyName = "mobilePrefix")]
        public string MobilePrefix { get; set; }
        [JsonProperty(PropertyName = "mobileNumber")]
        public int MobileNumber { get; set; }

        [JsonProperty(PropertyName = "groupBooking")]
        public bool? GroupBooking { get; set; }
        [JsonProperty(PropertyName = "groupName")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "groupNoOfMember")]
        public int? GroupNumberOfMember { get; set; }
        [JsonProperty(PropertyName = "isSingleCodeForGroup")]
        public bool? IsSingleCodeForGroup { get; set; }

        [JsonProperty(PropertyName = "paymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonProperty(PropertyName = "creditCardCurrencyId")]
        public int? CreditCardCurrencyId { get; set; }

        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }

    }

    public class BookTicketType : Identifier
    {
        [JsonProperty (PropertyName = "index")]
        public int Index { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
        [JsonProperty(PropertyName = "visitDate")]
        public string VisitDate { get; set; }
        [JsonProperty(PropertyName = "visitDateSettings")]
        public List<VisitDateSetting> VisitDateSettings { get; set; }
        [JsonProperty(PropertyName ="fromResellerId")]
        public int? FromResellerId { get; set; }
        [JsonProperty(PropertyName = "event_id")]
        public int? EventId { get; set; }
        [JsonProperty(PropertyName = "questionList")]
        public List<BookQuestion> Questions { get; set; }
        [JsonProperty(PropertyName = "packageItems")]
        public List<BookPackageItem> PackageItems { get; set; }
    }

    public class BookOtherInfo
    {
        [JsonProperty(PropertyName = "partnerReference")]
        public string PartnerReference { get; set; }
    }

    public class BookPackageItem : Identifier
    {
        [JsonProperty(PropertyName = "event_id")]
        public int? EventId { get; set; }
    }

    public class BookQuestion : Identifier
    {
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
    }

    public class VisitDateSetting
    {
        [JsonProperty(PropertyName = "productId")]
        public int ProductId { get; set; }
        [JsonProperty(PropertyName = "value")]
        public DateTime Value { get; set; }
    }
}
