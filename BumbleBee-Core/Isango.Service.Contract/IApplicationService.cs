using Isango.Entities;
using Isango.Entities.PricingRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Isango.Service.Contract
{
    public interface IApplicationService
    {

        Task<B2BNetRateRule> GetB2BNetRateRuleAsync(string affiliateId);
    }
}