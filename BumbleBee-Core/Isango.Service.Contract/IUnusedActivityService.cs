using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.HotelBeds;

namespace Isango.Service.Contract
{
    public interface IUnusedActivityService
    {
        Task<List<Activity>> GetActivitiesAsync(string activityIds, ClientInfo clientInfo, DateTime startDate);

        Task<int[]> GetLiveActivityIdsAsync(string languageCode);

        Task<List<Activity>> LoadLiveHbActivitiesAsync(int activityId, ClientInfo clientInfo);

        Task<List<AutoSuggest>> GetAutoSuggestByAffiliateIdAsync(string affiliateId);

        Task<int> GetActivityIdAsync(int productId);

        Task<List<AutoSuggest>> GetAutoSuggestDataAsync(ClientInfo clientInfo);

        Task<List<ProductOption>> GetPriceAndAvailabilityAsync(Activity activity, ClientInfo clientInfo);

        Task<SearchStack> GetOfferDataAsync(SearchCriteria searchCriteria, ClientInfo clientInfo);

        Task<int> GetActivityTypeAsync(int serviceId);

        Task<Dictionary<string, int>> LoadMaxPaxDetailsAsync(int activityId);

        Task<Activity> GetActivityDetailAsync(HotelbedCriteriaApitude activityRq, string token);

        Task<List<Activity>> GetSimilarProductsAsync(SearchCriteria criteria, ClientInfo clientInfo);
    }
}