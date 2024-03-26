namespace Isango.Entities
{
    public class PricingRule
    {
        public string Affiliate { get; set; }
        public string CustomerOrigin { get; set; }
        public string Currency { get; set; }
        public decimal MarginPercentage { get; set; }
    }
}