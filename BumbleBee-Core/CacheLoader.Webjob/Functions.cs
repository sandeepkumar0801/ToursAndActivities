using CacheLoader.Webjob.CustomTimerClasses;
using Isango.Service.Contract;
using Logger.Contract;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;

namespace CacheLoader.Webjob
{
    public class Functions
    {
        public readonly ICacheLoaderService _cacheLoaderService;
        public readonly ISynchronizerService _synchronizerService;
        public readonly ILogger _log;
        private readonly TelemetryClient _telemetryClient;
        public Functions(ICacheLoaderService cacheLoaderService, ISynchronizerService synchronizerService, ILogger log, TelemetryClient telemetryClient)
        {
            _cacheLoaderService = cacheLoaderService;
            _synchronizerService = synchronizerService;
            _log = log;
            _telemetryClient = telemetryClient;

        }

        //public void LoadCacheMapping([TimerTrigger(typeof(CacheMappingTime))] TimerInfo timer)
        //{
        //    _log.Info("Starting loading cache mapping list");
        //    var watch = Stopwatch.StartNew();
        //    //_cacheLoaderService.LoadCacheMappingAsync().Wait();
        //    watch.Stop();
        //    _log.Info($"Loaded cache mapping list in {watch.Elapsed}");
        //}
        [FunctionName("RegionCategoryMappingTime")]
        public void RegionCategoryMapping([TimerTrigger(typeof(RegionCategoryMappingTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|RegionCategoryMapping|Starting loading region category mapping list");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.RegionCategoryMappingAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|RegionCategoryMapping|Loaded region category mapping list in {watch.Elapsed}");
            _telemetryClient.TrackEvent("RegionCategoryMappingTime", new Dictionary<string, string>
              {
                  { "EventName", "TimerTrigger" }
              });

        }

        public void RegionDestinationMapping([TimerTrigger(typeof(RegionDestinationMappingTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|RegionDestinationMapping|Loading Region Destination Mapping list");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.RegionDestinationMappingAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|RegionDestinationMapping|Loaded Localized merchandising list in {watch.Elapsed}");
        }

        //public void LoadNetPriceMasterData([TimerTrigger(typeof(NetPriceMasterDataTime))] TimerInfo timer)
        //{
        //    _log.Info("Loading Net Price master data");
        //    var watch = Stopwatch.StartNew();
        //   _cacheLoaderService.LoadNetPriceMasterDataAsync().Wait();
        //    watch.Stop();
        //    _log.Info($"Loaded Net Price master data in {watch.Elapsed}");
        //}


        public void LoadRegionCategoryMappingProducts([TimerTrigger(typeof(RegionCategoryMappingProductsTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadRegionCategoryMappingProducts|Loading region category mapping products list");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadRegionCategoryMappingProductsAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadRegionCategoryMappingProducts|Loaded region category mapping products list in {watch.Elapsed}");
        }


        public void InsertOptionAvailability([TimerTrigger(typeof(InsertOptionAvailabilityTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|InsertOptionAvailability|inserting option availability");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.InsertOptionAvailability().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|InsertOptionAvailability|inserted option availability in {watch.Elapsed}");
        }


        public void LoadAvailabilityCache([TimerTrigger(typeof(AvailabilityCacheTime))]TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadAvailabilityCache|Loading availability cache");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadAvailabilityCacheAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadAvailabilityCache|Loaded availability cache in {watch.Elapsed}");
        }


        public void LoadCurrencyExchangeRates([TimerTrigger(typeof(CurrencyExchangeRatesTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadCurrencyExchangeRates|Loading currency exchange rate list");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadCurrencyExchangeRatesAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadCurrencyExchangeRates|Loaded currency exchange rate list in {watch.Elapsed}");
        }

        //public void LoadActivitiesCollection([TimerTrigger(typeof(ActivitiesCollectionTime))] TimerInfo timer)
        //{
        //    _log.Info("CacheLoader.Webjob|LoadActivitiesCollection|Loading activity collection");
        //    var watch = Stopwatch.StartNew();
        //    _cacheLoaderService.LoadActivitiesCollectionAsync().Wait();
        //    watch.Stop();
        //    _log.Info($"CacheLoader.Webjob|LoadActivitiesCollection|Loaded activity collection in {watch.Elapsed}");
        //}

        //public void LoadRegion([TimerTrigger(typeof(RegionTime))] TimerInfo timer)
        //{
        //    _log.Info("CacheLoader.Webjob|LoadRegion|Loading regions");
        //    var watch = Stopwatch.StartNew();
        //   _cacheLoaderService.SetBlogDataAsync().Wait();
        //    watch.Stop();
        //    _log.Info($"CacheLoader.Webjob|LoadRegion|Loaded regions in {watch.Elapsed}");
        //}

        public void LoadAffiliateFilter([TimerTrigger(typeof(AffiliateFilterTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadAffiliateFilter|Loading affiliate filter");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.SetAffiliateFilterAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadAffiliateFilter|Loaded affiliate filter in {watch.Elapsed}");
        }
        [FunctionName("GetCustomerPrototypeByActivity")]

        public void GetCustomerPrototypeByActivity([TimerTrigger(typeof(GetCustomerPrototypeByActivityTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|GetCustomerPrototypeByActivity|Loading customer prototype by activity");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.GetCustomerPrototypeByActivityAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|GetCustomerPrototypeByActivity|Loaded customer prototype by activity in {watch.Elapsed}");
        }


        public void Synchronizer([TimerTrigger(typeof(SynchronizerTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|Synchronizer|Starting web job to poll database");
            _synchronizerService.PollDatabaseForChangesAsync().Wait();
            _log.Info("CacheLoader.Webjob|Synchronizer|web job completed");
        }

        public void LoadAffiliateDataByDomain([TimerTrigger(typeof(LoadAffiliateDataByDomainTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadAffiliateDataByDomain|Loading Affiliate Data By Domain");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadAffiliateDataByDomainAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadAffiliateDataByDomain|Loaded Affiliate Data By Domain in {watch.Elapsed}");
        }

        public void PricingRules([TimerTrigger(typeof(PricingRulesTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|PricingRules|Loading Pricing Rules");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadPricingRulesAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|PricingRules|Loaded Pricing Rules in {watch.Elapsed}");
        }

        public void LoadCalendarAvailability([TimerTrigger(typeof(CalendarAvailabilityTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadCalendarAvailability|Loading Calendar Availability");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadCalendarAvailability().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadCalendarAvailability|Loaded Calendar Availability in {watch.Elapsed}");
        }

        public void LoadPickupLocationsData([TimerTrigger(typeof(LoadPickupDetailsTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadPickupLocationsData|Loading Pickup Locations Data for GLI");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadPickupLocationsDataAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadPickupLocationsData|Loaded pickup details in {watch.Elapsed}");
        }

        public void LoadMappedLanguageData([TimerTrigger(typeof(MappedLanguageTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadMappedLanguageData|Loading Mapped Language Data");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadMappedLanguageAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadMappedLanguageData|Loaded Mapped Language in {watch.Elapsed}");
        }

        public void LoadFareHarborCustomerPrototypeData([TimerTrigger(typeof(FareHarborCustomerPrototypeTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadFareHarborCustomerPrototypeData|Loading Fareharbor customer Prototype Data");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.GetCustomerPrototypeByActivityAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadFareHarborCustomerPrototypeData|Loaded Fareharbor customer Prototype in {watch.Elapsed}");
        }

        public void LoadFareHarborUserKeysData([TimerTrigger(typeof(FareHarborUserKeysTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadFareHarborUserKeysData|Loading Fareharbor user keys Data");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.GetAllFareHarborUserKeysAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadFareHarborUserKeysData|Loaded Fareharbor user keys in {watch.Elapsed}");
        }

        public void LoadElasticDestination([TimerTrigger(typeof(ElasticSearchTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadElasticDestination|Loading Elastic Destination");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadElasticSearchDestinationsAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticDestination|Loaded Elastic Destination in {watch.Elapsed}");
        }

        public void LoadElasticProducts([TimerTrigger(typeof(ElasticSearchTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadElasticProducts|Loading Elastic Products");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadElasticSearchProductsAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticProducts|Loaded Elastic Products in {watch.Elapsed}");
        }

        public void LoadElasticAttractions([TimerTrigger(typeof(ElasticSearchTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadElasticAttractions|Loading Elastic Attractions");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadElasticSearchAttractionsAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticAttractions|Loaded Elastic Attractions in {watch.Elapsed}");
        }

        public void LoadElasticAffiliate([TimerTrigger(typeof(ElasticSearchTime))] TimerInfo timer)
        {
            _log.Info("CacheLoader.Webjob|LoadElasticAffiliate|Loading Elastic Affiliate");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadElasticAffiliateAsync().Wait();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticAffiliate|Loaded Elastic Affiliate in {watch.Elapsed}");
        }
    }
}