using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{

    public class ReservationRQ
    {
        [JsonProperty(PropertyName = "alternateEmail")]
        public string AlternateEmail { get; set; }
        [JsonProperty(PropertyName = "creditCardCurrencyId")]
        public object CreditCardCurrencyId { get; set; }
        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "groupName")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "groupBooking")]
        public bool GroupBooking { get; set; }
        [JsonProperty(PropertyName = "groupNoOfMember")]
        public int GroupNoOfMember { get; set; }
        [JsonProperty(PropertyName = "mobileNumber")]
        public string MobileNumber { get; set; }
        [JsonProperty(PropertyName = "mobilePrefix")]
        public string MobilePrefix { get; set; }
        [JsonProperty(PropertyName = "otherInfo")]
        public Otherinfo OtherInfo { get; set; }
        [JsonProperty(PropertyName = "passportNumber")]
        public string PassportNumber { get; set; }
        [JsonProperty(PropertyName = "paymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonProperty(PropertyName = "ticketTypes")]
        public List<TickettypeReservation> TicketTypes { get; set; }
        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }
    }

    public class Otherinfo
    {
        [JsonProperty(PropertyName = "partnerReference")]
        public string PartnerReference { get; set; }
    }

    public class TickettypeReservation
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
        [JsonProperty(PropertyName = "sellingPrice")]
        public object SellingPrice { get; set; }
        [JsonProperty(PropertyName = "visitDate")]
        public object VisitDate { get; set; }
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }
        [JsonProperty(PropertyName = "questionList")]
        public List<Questionlist> QuestionList { get; set; }
        [JsonProperty(PropertyName = "event_id")]
        public object Event_id { get; set; }
        [JsonProperty(PropertyName = "packageItems")]
        public List<object> PackageItems { get; set; }
        [JsonProperty(PropertyName = "visitDateSettings")]
        public List<object> VisitDateSettings { get; set; }
    }
    public class Questionlist
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
    }
}
