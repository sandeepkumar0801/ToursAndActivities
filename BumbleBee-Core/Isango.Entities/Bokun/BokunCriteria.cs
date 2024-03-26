using System.Collections.Generic;

namespace Isango.Entities.Bokun
{
    public class BokunCriteria : Criteria
    {
        public List<int> FactSheetIds { get; set; }
        public int ActivityId { get; set; }
        public string ActivityCode { get; set; }
        public decimal MarginPercentage { get; set; }
        public List<PriceCategory> PriceCategoryIdMapping { get; set; }
        public List<PriceCategory> AllPriceCategoryIdMapping { get; set; }

        public string CurrencyIsoCode { get; set; }
    }
}