using System.Collections.Generic;
using Newtonsoft.Json;

namespace Isango.Entities.Cancellation
{
    public class Cancellation
    {
        public string BookingRefNo { get; set; }
        public string CancelledByUserId { get; set; }
        public int CancelledByUser { get; set; }
        public string TokenId { get; set; }
        public int? TrackerStatusId { get; set; }
        public CancellationParameters CancellationParameters { get; set; }
    }

    public class CancellationParameters
    {
        [JsonProperty("bookedoptionid")]
        public int BookedOptionId { get; set; }

        [JsonProperty("currencycode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("suppliercurrencycode")]
        public string SupplierCurrencyCode { get; set; }

        [JsonProperty("userrefundamount")]
        public decimal UserRefundAmount { get; set; }

        //[JsonProperty("supplierrefundamount")]
        //public decimal SupplierRefundAmount { get; set; }

        [JsonProperty("guwid")]
        public string Guwid { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("alternativetours")]
        public string AlternativeTours { get; set; }

        [JsonProperty("suppliernotes")]
        public string SupplierNotes { get; set; }

        [JsonProperty("customernotes")]
        public string CustomerNotes { get; set; }

        [JsonProperty("alternativedates")]
        public List<string> AlternativeDates { get; set; }
    }
}