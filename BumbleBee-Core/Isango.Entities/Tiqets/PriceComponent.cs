using Newtonsoft.Json;

namespace Isango.Entities.Tiqets
{
    public class PriceComponent
    {
        [JsonProperty(PropertyName = "sale_ticket_value_incl_vat")]
        public decimal SaleTicketValueIncVat { get; set; }

        [JsonProperty(PropertyName = "booking_fee_incl_vat")]
        public decimal BookingFeeIncVat { get; set; }

        [JsonProperty(PropertyName = "total_retail_price_incl_vat")]
        public decimal TotalRetailPriceIncVat { get; set; }

        [JsonProperty(PropertyName = "distributor_commission_excl_vat")]
        public decimal DistributorCommissionExclVat { get; set; }
    }
}