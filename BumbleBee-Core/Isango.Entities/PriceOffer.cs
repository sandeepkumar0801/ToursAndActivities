namespace Isango.Entities
{
    public class PriceOffer
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string RuleName { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal CostAmount { get; set; }

    }
}