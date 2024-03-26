using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    public class OptionDetail
    {
        public int ServiceOptionId { get; set; }
        public string CurrencyIsoCode { get; set; }
        public int PriceType { get; set; }
        public int UnitType { get; set; }
        public int MaxCapacity { get; set; }
        public int MaxQuantity { get; set; }
        public List<PaxPrice> PaxPrices { get; set; }
    }
}