namespace TableStorageOperations.Models.AdditionalPropertiesModels.AppliedPriceOffers
{
    public class PriceOffer : CustomTableEntity
    {
        public int AppliedId { get; set; }
        public string ModuleName { get; set; }
        public string RuleName { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal CostAmount { get; set; }

    }
}