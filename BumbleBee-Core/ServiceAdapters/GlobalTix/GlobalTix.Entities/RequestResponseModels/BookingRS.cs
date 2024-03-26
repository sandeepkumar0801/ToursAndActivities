using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class BookingRS
    {
        [JsonProperty(PropertyName = "data")]
        public BookResponseData BookingData { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
    }

    public class BookResponseData : Identifier
    {
        [JsonProperty(PropertyName = "time")]
        public DateTime BookTime { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string CurrencyCode { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }
        [JsonProperty(PropertyName = "reference_number")]
        public string BookReferenceNumber { get; set; }

        [JsonProperty(PropertyName = "tickets")]
        public List<BookTicket> Tickets { get; set; }
        [JsonProperty(PropertyName = "eTicketUrl")]
        public string ETicketUrl { get; set; }
    }

    public class BookTicket : IdentifierWithName
    {
        [JsonProperty (PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "redeemed")]
        public bool Redeemed { get; set; }
        [JsonProperty(PropertyName = "termsAndConditions")]
        public string TermsAndConditions { get; set; }
        [JsonProperty(PropertyName = "reseller")]
        public int Reseller { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "isOpenDated")]
        public bool? IsOpenDated { get; set; }
        [JsonProperty(PropertyName = "variation")]
        public EnumValue Variation { get; set; }
        [JsonProperty(PropertyName = "attraction")]
        public OptionalIdentifier Attraction { get; set; }
        [JsonProperty(PropertyName = "qrcode")]
        public string QRCode { get; set; }
        [JsonProperty(PropertyName = "status")]
        public EnumValue Status { get; set; }
		[JsonProperty(PropertyName = "ticketFormat")]
		public string TicketFormat { get; set; }
    }

	public class OptionalIdentifier
	{
		[JsonProperty(PropertyName = "id")]
		public int? Id { get; set; }
	}
}
