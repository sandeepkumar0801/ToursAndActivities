using System.Collections.Generic;

namespace Isango.Entities.GoldenTours
{
    public class GoldenToursCriteria : Criteria
    {
        public string SupplierOptionCode { get; set; }
        public List<string> SupplierOptionCodes { get; set; }
        public string CurrencyCode { get; set; } = "GBP";
        public string LanguageId { get; set; } = "1";
        public string Status { get; set; } = "OPEN";
        public List<PassengerMapping> PassengerMappings { get; set; }
    }
}