namespace Isango.Entities
{
    public class Currency
    {
        public string IsoCode { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public bool IsPostFix { get; set; }

        public string ShortSymbol { get; set; }

        public int CurrencyID { get; set; }

        public Currency()
        {
        }

        public Currency(Currency currency)
        {
            IsoCode = currency.IsoCode;
            Name = currency.Name;
            Symbol = currency.Symbol;
            IsPostFix = currency.IsPostFix;
            ShortSymbol = currency.ShortSymbol;
        }
    }
}