using Isango.Entities.Enums;
using Isango.Entities.Tiqets;
using Newtonsoft.Json;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class GetTicketResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "order_reference_id")]
        public string OrderReferenceId { get; set; }

        [JsonProperty(PropertyName = "how_to_use")]
        public string HowToUse { get; set; }

        [JsonProperty(PropertyName = "post_purchase")]
        public string PostPurchase { get; set; }

        [JsonProperty(PropertyName = "order_status")]
        public TiqetsOrderStatus TiqetsOrderStatus { get; set; }

        [JsonProperty(PropertyName = "tickets_pdf_url")]
        public string TicketPdfUrl { get; set; }

        [JsonProperty(PropertyName = "price_components_eur")]
        public PriceComponent PriceComponentEur { get; set; }
    }
}