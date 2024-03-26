using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.Review;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Isango.Entities.Affiliate;

namespace Isango.Service.Contract
{
    public interface IActivityService
    {
        Task<Activity> LoadActivityAsync(int activityId, DateTime startDate, ClientInfo clientInfo, string B2BAffiliate = null);
        Task<Activity> CalculateActivityWithMinPricesAsync(Activity activity);

        Task<Activity> GetProductAvailabilityAsync(int activityId, ClientInfo clientInfo, Criteria criteria);

        Task<Activity> GetActivityDetailsAsync(int activityId, ClientInfo clientInfo, Criteria criteria);

        Task<ActivityDetailsWithCalendarResponse> GetActivityDetailsWithCalendar(ActivityDetailsWithCalendarRequest request, string B2BAffiliate = null);

        Task<List<CalendarAvailability>> GetCalendarAvailabilityAsync(int productId, string affiliateId);

        Task<Activity> GetBundleProductAvailabilityAsync(int bundleActivityId, ClientInfo clientInfo, Dictionary<int, Criteria> criteriaForActivity);

        Task<Activity> GetActivityById(int activityId, DateTime startDate, string languageCode);
        Task<Activity> GetActivityById_B2B(int activityId, DateTime startDate, string languageCode);


        List<RegionCategoryMapping> LoadRegionCategoryMapping();
        List<Activity> GetActivitiesWithLivePrice(ClientInfo clientInfo, Criteria criteria, List<Activity> activities);
        bool MatchAllAffiliateCriteria(AffiliateFilter affiliateFilter, Product product);

        Task<List<Activity>> GetCrossSellProducts(int? lineOfBusinessId, Affiliate affiliate, ClientInfo clientInfo, string regionId);

        Task<List<WidgetMappedData>> GetWidgetData();
    }
}