using Isango.Entities.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCacheManager.Contract
{
    public interface IB2BNetRateRuleCache
    {
        B2BNetRateRule GetB2BNetRateRule(string affiliateId);
    }
}
