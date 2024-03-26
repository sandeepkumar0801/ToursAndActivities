using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class PricingModule
    {
        public PriceModule Name { get; set; }
        public BusinessCategory BusinessCategory { get; set; }
        public List<PricingRule> PricingRules { get; set; }
    }
}