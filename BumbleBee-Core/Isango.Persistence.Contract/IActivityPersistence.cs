using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.Review;
using Isango.Entities.Rezdy;
using System;
using System.Collections.Generic;
using System.Data;

namespace Isango.Persistence.Contract
{
    public interface IActivityPersistence
    {
        SearchResult SearchActivities(SearchCriteria searchCriteria, ClientInfo clientInfo);

        List<RegionCategoryMapping> LoadRegionCategoryMapping();

        RegionMetaData LoadRegionMetaData(int regionId, int catId, string languageCode);

        List<RegionActivityMapping> GetFullTextSearchActivitiyIdMapping(string regionIds, string keywordPhrase, ClientInfo clientInfo);

        List<string> GetRegionIdsFromAttractionId(string affiliateId, int attractionId);

        Int32[] GetLiveActivityIds(string languageCode);

        List<Activity> LoadLiveHbActivities(int activityId, string languageCode);

        List<Activity> GetActivitiesByActivityIds(string activityIds, string languageCode);

        List<AutoSuggest> GetAutoSuggestData(string affiliateId);

        int GetActivityId(int productId);

        List<ProductOption> GetPriceAndAvailability(Activity activity, ClientInfo clientInfo, bool isB2BNetPriceApplied);

        List<ProductOption> GetAllOptionsAvailability(Activity activity, DateTime startDate, DateTime endDate);

        List<ProductOptionAvailabilty> GetAllOptionsAvailabilities(Activity activity, DateTime startDate, DateTime endDate);

        int GetActivityType(int serviceId);

        List<AttractionActivityMapping> CategoryServiceMapping();

        Dictionary<string, int> LoadMaxPaxDetails(int id);

        List<ActivityChangeTracker> GetModifiedServices();

        int RemoveUpdatedServices(string servicedIds);

        void InsertOptionAvailability();

        DataTable GetOptionAvailability(string regionIds = "", string activityIds = "");

        List<CalendarAvailability> GetCalendarAvailability(string activityIds = "");

        List<OptionDetail> GetPaxPrices(PaxPriceRequest paxPriceRequest);

        List<Entities.Booking.PassengerInfo> GetPassengerInfoDetails(string activityIds = "");
        List<RezdyPaxMapping> GetRezdyPaxMappings();
        bool GetCalendarFlag();

        List<WidgetMappedData> GetRegionMappedDataForWidget();
        void InsertApiErrorLog(int activityId, string message, string MethodName);


    }
}