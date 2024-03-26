namespace Isango.Entities
{
    public class CurrencyExchangeRates
    {
        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}