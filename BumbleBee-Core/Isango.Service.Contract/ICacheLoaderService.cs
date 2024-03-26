using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Availability;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface ICacheLoaderService
    {
        Task<bool> LoadGliAgeGroupByActivityAsync();

        Task<bool> LoadPrioAgeGroupByActivityAsync();

        Task<bool> LoadAotAgeGroupByActivityAsync();

        Task<bool> LoadCacheMappingAsync();

        Task<bool> RegionCategoryMappingAsync();

        Task<bool> RegionDestinationMappingAsync();

        Task<bool> LoadMappedLanguageAsync();

        Task<bool> LoadNetPriceMasterDataAsync();

        Task<bool> LoadRegionCategoryMappingProductsAsync();

        Task<bool> LoadAvailabilityCacheAsync();

        Task<bool> LoadCurrencyExchangeRatesAsync();

        Task<bool> LoadActivitiesCollectionAsync();

        Task<bool> SetRegionAsync();

        Task<bool> SetAffiliateFiltersAsync();

        Task<bool> GetCustomerPrototypeByActivityAsync();

        Task<bool> GetAllFareHarborUserKeysAsync();

        Task<bool> SetUrlPageIdMappingMappingAsync();

        Task<bool> LoadPricingRulesAsync();

        Task<bool> LoadAffiliateDataByDomainAsync();

        Task<List<Activity>> GetAllActivities();

        Task<List<Availability>> GetPriceAndAvailability();

        Task<Activity> GetSingleActivity(string activityId);

        Task<bool> LoadFareHarborAgeGroupByActivityAsync();

        Task<string> LoadCalendarAvailability();

        Task<bool> LoadHBAuthorizationDataAsync();

        Task<bool> SetFilteredTicketAsync();

        Task<string> LoadPickupLocationsDataAsync();

        Task InsertOptionAvailability();

        Task<bool> SetAffiliateFilterAsync();

        Task<bool> LoadSelectedActivitiesAsync(string activityIds);

        Task<bool> LoadTiqetsPaxMappingsAsync();

        Task<bool> LoadGoldenToursPaxMappingsAsync();

        Task<bool> LoadRezdyLabelDetailsAsync();
        Task<bool> LoadRezdyPaxMappingsAsync();

        Task<bool> LoadTourCMSPaxMappingsAsync();

        Task<bool> LoadElasticSearchDestinationsAsync();

        Task<bool> LoadElasticSearchProductsAsync();

        Task<bool> LoadElasticSearchAttractionsAsync();

        Task<bool> LoadElasticAffiliateAsync();

        void ClearActivityWebsiteCache(List<ActivityChangeTracker> changedActivities);
        Task<bool> LoadVentrataPaxMappingsAsync();

        Task<bool> LoadGlobalTixV3PaxMappingsAsync();

        Task<bool> ClearMongoSessions();
     }
}