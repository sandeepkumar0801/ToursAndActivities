namespace Isango.Entities
{
    public class Margin
    {
        public decimal Value { get; set; }
        public bool IsPercentage { get; set; } = true;
        public string CurrencyCode { get; set; }
    }
}