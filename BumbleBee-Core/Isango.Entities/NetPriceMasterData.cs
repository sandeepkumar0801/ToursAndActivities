using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class NetPriceMasterData
    {
        public string AffiliateId { get; set; }
        public int ProductId { get; set; }
        public decimal CommisionPercentage { get; set; }
        public decimal NetPrice { get; set; }
        public string CurrencyCode { get; set; }
        public decimal MaxSellPrice { get; set; }
        public decimal CostPrice { get; set; }
        public APIType ApiType { get; set; }
    }
}