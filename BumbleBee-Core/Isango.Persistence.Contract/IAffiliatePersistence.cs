using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IAffiliatePersistence
    {
        Affiliate GetAffiliateInfo(string domain, string alias, string widgetDate);

        List<AffiliateFilter> GetAffiliateFilter();

        List<Partner> GetWidgetPartners();

        Affiliate GetAffiliateInformation(string affiliateId, string languageCode);

        List<AffiliateReleaseTag> GetAffiliateReleaseTags();

        string UpdateAffiliateReleaseTags(string affiliateId, string releaseTag, bool isForAll = false);

        List<Affiliate> GetWLAffiliateInfo();

        string GetAffiliateIdByUserId(string userId);

        List<string> GetModifiedAffiliates();

        List<AffiliateWiseServiceMinPrice> GetAffiliateWiseServiceMinPrice();
    }
}