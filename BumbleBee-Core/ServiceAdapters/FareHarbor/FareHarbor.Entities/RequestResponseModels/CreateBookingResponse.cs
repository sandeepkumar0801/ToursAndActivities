using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class CreateBookingResponse
    {
        [JsonProperty("voucher_number")]
        public string VoucherNumber { get; set; }

        [JsonProperty("display_id")]
        public string DisplayId { get; set; }

        [JsonProperty("rebooked_to")]
        public object RebookedTo { get; set; }

        [JsonProperty("note_safe_html")]
        public string NoteSafeHtml { get; set; }

        public List<CustomerBookingResponse> Customers { get; set; }
        public object Agent { get; set; }

        [JsonProperty("affiliate_company")]
        public AffiliateCompany AffiliateCompany { get; set; }

        public Availability Availability { get; set; }
        public string Uuid { get; set; }

        [JsonProperty("receipt_subtotal")]
        public string ReceiptSubtotal { get; set; }

        [JsonProperty("confirmation_url")]
        public string ConfirmationUrl { get; set; }

        public string Note { get; set; }

        [JsonProperty("pickup")]
        public BookedPickup Pickup { get; set; }
        public string Pk { get; set; }
        public Company Company { get; set; }
        public Arrival Arrival { get; set; }

        [JsonProperty("receipt_total")]
        public string ReceiptTotal { get; set; }

        [JsonProperty("effective_cancellation_policy")]
        public EffectiveCancellationPolicy EffectiveCancellationPolicy { get; set; }

        [JsonProperty("amount_paid")]
        public string AmountPaid { get; set; }

        public object Desk { get; set; }

        [JsonProperty("is_eligible_for_cancellation")]
        public bool IsEligibleForCancellation { get; set; }

        [JsonProperty("receipt_taxes")]
        public string ReceiptTaxes { get; set; }

        public Contact Contact { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("invoice_price")]
        public string InvoicePrice { get; set; }

        [JsonProperty("rebooked_from")]
        public object RebookedFrom { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }
    }

    public class BookedPickup
    {
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("description_safe_html")]
        public string DescriptionSafeHTML { get; set; }
        [JsonProperty("map_url")]
        public string MapUrl { get; set; }
        [JsonProperty("display_text")]
        public string DisplayText { get; set; }
    }
}