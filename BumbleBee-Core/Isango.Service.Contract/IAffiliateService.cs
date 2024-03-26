using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IAffiliateService
    {
        Task<Affiliate> GetAffiliateInfoAsync(string domain, string alias, string affiliateId);

        Task<AffiliateFilter> GetAffiliateFilterByIdAsync(string affiliateId);

        Task<List<AffiliateFilter>> GetAffiliateFiltersAsync();

        Task<Affiliate> GetAffiliateInformationAsync(string affiliateId);

        Task<Affiliate> GetAffiliateInformationV2Async(string affiliateId);

        List<Partner> GetWidgetPartnersAsync();

        Task<List<AffiliateReleaseTag>> GetAffiliateReleaseTagsAsync();

        Task<string> UpdateAffiliateReleaseTagsAsync(string affiliateId, string releaseTag, bool isForAll = false);

        Task<List<Affiliate>> GetWLAffiliateInfoAsync();

        Task<string> GetAffiliateIdByUserIdAsync(string userId);

        Task<List<AffiliateWiseServiceMinPrice>> GetAffiliateInformationAsync();
    }
}