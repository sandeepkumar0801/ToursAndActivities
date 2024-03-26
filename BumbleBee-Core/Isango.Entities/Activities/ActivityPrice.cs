namespace Isango.Entities.Activities
{
    public class ActivityPrice
    {
        public string Id { get; set; }
        public decimal BasePrice { get; set; }
        public string BaseCurrencyCode { get; set; }
        public decimal Price { get; set; } // Sell Price
        public string CurrencyIsoCode { get; set; }

        /// <summary>
        /// Provides information for the customer type allowed or not viz. Child allowed or not
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Provides information for the child price is the % of the Adult Price
        /// </summary>
        public bool IsPercent { get; set; }
    }
}