using Isango.Service.Contract;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger.Contract;
using Hangfire;
using System.Diagnostics;

namespace CacheLoader.HangFire
{
    public class Functions
    {
        public readonly ICacheLoaderService _cacheLoaderService;
        public readonly ISynchronizerService _synchronizerService;
        public readonly ILogger _log;
        public Functions(ICacheLoaderService cacheLoaderService, ISynchronizerService synchronizerService, ILogger log)
        {
            _cacheLoaderService = cacheLoaderService;
            _synchronizerService = synchronizerService;
            _log = log;

        }
        [JobDisplayName("CacherLoader.RegionDestinationMapping")]
        public void RegionDestinationMapping()
        {
            try {
                _log.Info("CacheLoader.Webjob|RegionDestinationMapping|Loading Region Destination Mapping list");
                var watch = Stopwatch.StartNew();

                // Enqueue the background job
                 _cacheLoaderService.RegionDestinationMappingAsync()?.GetAwaiter().GetResult();

                watch.Stop();
                _log.Info($"CacheLoader.Webjob|RegionDestinationMapping|Enqueued region destination mapping list load job in {watch.Elapsed}");
            }
            catch (Exception ex) 
            {
                
                    // Log the exception
                    _log.Error("An error occurred in RegionDestinationMapping job", ex);
                }
            }
        [JobDisplayName("CacherLoader.LoadElasticDestination")]
        public void LoadElasticDestination()
        {
            _log.Info("CacheLoader.Webjob|LoadElasticDestination|Loading Elastic Destination");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadElasticSearchDestinationsAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticDestination|Loaded Elastic Destination in {watch.Elapsed}");
        }
        [JobDisplayName("CacherLoader.RegionCategoryMapping")]
        public void RegionCategoryMapping()
        {
            _log.Info("CacheLoader.Webjob|RegionCategoryMapping|Starting loading region category mapping list");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.RegionCategoryMappingAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|RegionCategoryMapping|Loaded region category mapping list in {watch.Elapsed}");
        }
        [JobDisplayName("CacherLoader.LoadRegionCategoryMappingProducts")]
        public void LoadRegionCategoryMappingProducts()
        {
            _log.Info("CacheLoader.Webjob|LoadRegionCategoryMappingProducts|Loading region category mapping products list");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadRegionCategoryMappingProductsAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadRegionCategoryMappingProducts|Loaded region category mapping products list in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.InsertOptionAvailability")]
        public void InsertOptionAvailability()
        {
            _log.Info("CacheLoader.Webjob|InsertOptionAvailability|inserting option availability");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.InsertOptionAvailability()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|InsertOptionAvailability|inserted option availability in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadAvailabilityCache")]
        public void LoadAvailabilityCache()
        {
            _log.Info("CacheLoader.Webjob|LoadAvailabilityCache|Loading availability cache");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadAvailabilityCacheAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadAvailabilityCache|Loaded availability cache in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadCurrencyExchangeRates")]
        public void LoadCurrencyExchangeRates()
        {
            _log.Info("CacheLoader.Webjob|LoadCurrencyExchangeRates|Loading currency exchange rate list");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadCurrencyExchangeRatesAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadCurrencyExchangeRates|Loaded currency exchange rate list in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadAffiliateFilter")]
        public void LoadAffiliateFilter()
        {
            _log.Info("CacheLoader.Webjob|LoadAffiliateFilter|Loading affiliate filter");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.SetAffiliateFilterAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadAffiliateFilter|Loaded affiliate filter in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.GetCustomerPrototypeByActivity")]
        public void GetCustomerPrototypeByActivity()
        {
            _log.Info("CacheLoader.Webjob|GetCustomerPrototypeByActivity|Loading customer prototype by activity");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.GetCustomerPrototypeByActivityAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|GetCustomerPrototypeByActivity|Loaded customer prototype by activity in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.Synchronizer")]
        public void Synchronizer()
        {
            _log.Info("CacheLoader.Webjob|Synchronizer|Starting web job to poll database");
             _synchronizerService.PollDatabaseForChangesAsync()?.GetAwaiter().GetResult();
            _log.Info("CacheLoader.Webjob|Synchronizer|web job completed");
        }

        [JobDisplayName("CacherLoader.LoadAffiliateDataByDomain")]
        public void LoadAffiliateDataByDomain()
        {
            _log.Info("CacheLoader.Webjob|LoadAffiliateDataByDomain|Loading Affiliate Data By Domain");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadAffiliateDataByDomainAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadAffiliateDataByDomain|Loaded Affiliate Data By Domain in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.PricingRules")]
        public void PricingRules()
        {
            _log.Info("CacheLoader.Webjob|PricingRules|Loading Pricing Rules");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadPricingRulesAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|PricingRules|Loaded Pricing Rules in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadCalendarAvailability")]
        public void LoadCalendarAvailability()
        {
            _log.Info("CacheLoader.Webjob|LoadCalendarAvailability|Loading Calendar Availability");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadCalendarAvailability()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadCalendarAvailability|Loaded Calendar Availability in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadPickupLocationsData")]
        public void LoadPickupLocationsData()
        {
            _log.Info("CacheLoader.Webjob|LoadPickupLocationsData|Loading Pickup Locations Data for GLI");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadPickupLocationsDataAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadPickupLocationsData|Loaded pickup details in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadMappedLanguageData")]
        public void LoadMappedLanguageData()
        {
            _log.Info("CacheLoader.Webjob|LoadMappedLanguageData|Loading Mapped Language Data");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadMappedLanguageAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadMappedLanguageData|Loaded Mapped Language in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadFareHarborCustomerPrototypeData")]
        public void LoadFareHarborCustomerPrototypeData()
        {
            _log.Info("CacheLoader.Webjob|LoadFareHarborCustomerPrototypeData|Loading Fareharbor customer Prototype Data");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.GetCustomerPrototypeByActivityAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadFareHarborCustomerPrototypeData|Loaded Fareharbor customer Prototype in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadFareHarborUserKeysData")]
        public void LoadFareHarborUserKeysData()
        {
            _log.Info("CacheLoader.Webjob|LoadFareHarborUserKeysData|Loading Fareharbor user keys Data");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.GetAllFareHarborUserKeysAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadFareHarborUserKeysData|Loaded Fareharbor user keys in {watch.Elapsed}");
        }


        [JobDisplayName("CacherLoader.LoadElasticProducts")]
        public void LoadElasticProducts()
        {
            _log.Info("CacheLoader.Webjob|LoadElasticProducts|Loading Elastic Products");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadElasticSearchProductsAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticProducts|Loaded Elastic Products in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadElasticAttractions")]
        public void LoadElasticAttractions()
        {
            _log.Info("CacheLoader.Webjob|LoadElasticAttractions|Loading Elastic Attractions");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadElasticSearchAttractionsAsync()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticAttractions|Loaded Elastic Attractions in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.LoadElasticAffiliate")]
        public void LoadElasticAffiliate()
        {
            _log.Info("CacheLoader.Webjob|LoadElasticAffiliate|Loading Elastic Affiliate");
            var watch = Stopwatch.StartNew();
             _cacheLoaderService.LoadElasticAffiliateAsync()?.GetAwaiter().GetResult();

            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticAffiliate|Loaded Elastic Affiliate in {watch.Elapsed}");
        }

        [JobDisplayName("CacherLoader.InitaliseMongoDb")]
        public void InitaliseMongoDb()
        {
            _log.Info("CacheLoader.Webjob|InitaliseMongoDb|InitaliseMongoDb");
            var watch = Stopwatch.StartNew();

            _cacheLoaderService.RegionDestinationMappingAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadElasticSearchDestinationsAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.RegionCategoryMappingAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadRegionCategoryMappingProductsAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.InsertOptionAvailability()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadAvailabilityCacheAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadCurrencyExchangeRatesAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.SetAffiliateFilterAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.GetCustomerPrototypeByActivityAsync()?.GetAwaiter().GetResult();
            _synchronizerService.PollDatabaseForChangesAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadAffiliateDataByDomainAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadPricingRulesAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadCalendarAvailability()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadPickupLocationsDataAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadMappedLanguageAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.GetCustomerPrototypeByActivityAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.GetAllFareHarborUserKeysAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadElasticSearchProductsAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadElasticSearchAttractionsAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadElasticAffiliateAsync()?.GetAwaiter().GetResult();
            _cacheLoaderService.LoadAotAgeGroupByActivityAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadFareHarborAgeGroupByActivityAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadGliAgeGroupByActivityAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadPrioAgeGroupByActivityAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadTiqetsPaxMappingsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadGoldenToursPaxMappingsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadRezdyPaxMappingsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadRezdyLabelDetailsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadTourCMSPaxMappingsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadVentrataPaxMappingsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadHBAuthorizationDataAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadGlobalTixV3PaxMappingsAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|InitaliseMongoDb|InitaliseMongoDb in {watch.Elapsed}");

        }

        [JobDisplayName("CacherLoader.ClearMongoSessions")]
        public void ClearMongoSessions()
        {
            _log.Info("CacheLoader.Webjob|ClearMongoSessions|Clearing Mongo User Sessions");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.ClearMongoSessions()?.GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|ClearMongoSessions|Cleared Mongo User Sessions in {watch.Elapsed}");
        }
    }
}

