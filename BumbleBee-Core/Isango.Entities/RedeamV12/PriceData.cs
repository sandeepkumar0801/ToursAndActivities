namespace Isango.Entities.RedeamV12
{
    public class PriceData
    {
        public string ProductId { get; set; }
        public string RateId { get; set; }
        public string PriceId { get; set; }
        public string PriceName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public bool Refundable { get; set; }
        public string Status { get; set; }
        public int NetAmount { get; set; }
        public string NetCurrency { get; set; }
        public int RetailAmount { get; set; }
        public string RetailCurrency { get; set; }
        public string Labels { get; set; }
        public int Version { get; set; }
    }
}