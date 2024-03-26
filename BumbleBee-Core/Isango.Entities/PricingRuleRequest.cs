using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class PricingRuleRequest
    {
        public List<ProductOption> ProductOptions { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public Criteria Criteria { get; set; }
        public PriceTypeId PriceTypeId { get; set; }
        public APIType ApiType { get; set; }
        public bool IsQrScanDiscountApplicable { get; set; }
    }
}