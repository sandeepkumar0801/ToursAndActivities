using Hangfire;
using Isango.Service.Contract;
using Logger.Contract;
using System.Diagnostics;

namespace DataDumpingQueue.Hangfire
{
    public class DataDumpingQueue_Function
    {
        public readonly IAgeGroupService _ageGroupService;
        public readonly ILogger _log;
        public readonly ICacheLoaderService _cacheLoaderService;
        public readonly IGoogleMapsService _googleMapsService;
        public readonly IStorageOperationService _storageOperationService;

        public DataDumpingQueue_Function(IAgeGroupService ageGroupService, ICacheLoaderService cacheLoaderService,
            ILogger log
            , IGoogleMapsService googleMapsService
            , IStorageOperationService storageOperationService)
        {
            _ageGroupService = ageGroupService;
            _cacheLoaderService = cacheLoaderService;
            _log = log;
            _googleMapsService = googleMapsService;
            _storageOperationService = storageOperationService;
        }
        [JobDisplayName("InitiateGoogleFeeds")]
        public void ProcessMerchantFeed()
        {
            _log.Info("DataDumpingQueue.WebJob.Function|ProcessMerchantFeed|Starting processing merchant feed");
            var watch = Stopwatch.StartNew();
            _googleMapsService.LoadMerchantFeeds();
            watch.Stop();
            ProcessServiceAvailabilityFeed();
            _log.Info($"DataDumpingQueue.WebJob.Function|ProcessMerchantFeed| Processed merchant feed in {watch.Elapsed}");
        }

        
        public void ProcessServiceAvailabilityFeed()
        {
            _log.Info("DataDumpingQueue.WebJob.Function|ProcessServiceAvailabilityFeed|Starting processing service feed");
            var watch = Stopwatch.StartNew();
            _googleMapsService.LoadServiceAvailabilityFeeds();
            watch.Stop();
            _log.Info($"DataDumpingQueue.WebJob.Function|ProcessServiceAvailabilityFeed| Processed service feed in {watch.Elapsed}");
        }

        [JobDisplayName("AOT")]
        public void LoadAgeGroupForAot()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForAot|Starting loading Age group for AOT");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_AOT";
            _ageGroupService.SaveAOTAgeGroups(token);
            _cacheLoaderService.LoadAotAgeGroupByActivityAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForAot|Loaded Age group list for AOT in {watch.Elapsed}");
        }

        [JobDisplayName("Fareharbor")]
        public void LoadAgeGroupForFareHarbor()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForFareHarbor|Starting loading Age group for fare harbor");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_FareHarbor";
            _ageGroupService.SaveFareHarborCompanies(token);
            _ageGroupService.SaveFareHarborCustomerProtoTypes(token);
            _ageGroupService.SyncFareHarborData();
            _cacheLoaderService.LoadFareHarborAgeGroupByActivityAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForFareHarbor|Loaded Age group list for fare harbor in {watch.Elapsed}");
        }

        [JobDisplayName("GrayLineIceLand")]
        public void LoadAgeGroupForGrayLineIceLand()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForGrayLineIceLand|Starting loading Age group for GrayLine IceLand");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_GrayLineIceLand";
            _ageGroupService.SaveGrayLineIceLandAgeGroups(token);
            _ageGroupService.SaveGrayLineIceLandPickupLocations(token);
            _ageGroupService.SyncGrayLineIceLandData();
            _cacheLoaderService.LoadGliAgeGroupByActivityAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForGrayLineIceLand|Loaded Age group list for GrayLine IceLand in {watch.Elapsed}");
        }

        //public void LoadAgeGroupForVentrata()
        //{
        //    _log.Info("DataDumpingQueue.Function|LoadAgeGroupForVentrata|Starting loading Age group for Ventrata");
        //    var watch = Stopwatch.StartNew();
        //    var token = "AgeGroup_Ventrata";
        //    _ageGroupService.SaveVentrataProductDetails(token);
        //    _cacheLoaderService.LoadVentrataPaxMappingsAsync().GetAwaiter().GetResult();
        //    watch.Stop();
        //    _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForVentrata|Loaded Age group list for Ventrata in {watch.Elapsed}");
        //}

        [JobDisplayName("Prio")]

        public void LoadAgeGroupForPrio()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForPrio|Starting loading Age group for Prio");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Prio";
            _ageGroupService.SavePrioTicketDetails(token);
            _cacheLoaderService.LoadPrioAgeGroupByActivityAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForPrio|Loaded Age group list for Prio in {watch.Elapsed}");
        }

        [JobDisplayName("HBAuthorization")]

        public void LoadHBAuthorizationData()
        {
            _log.Info("DataDumpingQueue.Function|LoadHBAuthorizationData|Loading HBAuthorization Data");
            var watch = Stopwatch.StartNew();
            _cacheLoaderService.LoadHBAuthorizationDataAsync().Wait();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadHBAuthorizationData|Loaded HBAuthorization in {watch.Elapsed}");
        }

        public void LoadTiqetsVariants()
        {
            _log.Info("DataDumpingQueue.Function|LoadTiqetsVariants|Starting loading Age group for Tiqets");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Tiqets";
            _ageGroupService.SaveTiqetsVariants(token);
            _cacheLoaderService.LoadTiqetsPaxMappingsAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadTiqetsVariants|Loaded Age group list for Tiqets in {watch.Elapsed}");
        }
        [JobDisplayName("TourCMS")]

        public void LoadTourCMS()
        {
            _log.Info("DataDumpingQueue.Function|LoadTourCMS|Starting data for LoadTourCMS");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_TourCMS";
            _ageGroupService.SaveTourCMSChannelData(token);
            _ageGroupService.SaveTourCMSTourData(token);
            _cacheLoaderService.LoadTourCMSPaxMappingsAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadTourCMS|Loaded data for TourCMS in {watch.Elapsed}");
        }

        [JobDisplayName("GoldenTours")]

        public void LoadAgeGroupForGoldenTours()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForGoldenTours|Starting loading Age group for GoldenTours");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_GoldenTours";
            _ageGroupService.SaveGoldenToursAgeGroups(token);
            _cacheLoaderService.LoadGoldenToursPaxMappingsAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForGoldenTours|Loaded Age group list for GoldenTours in {watch.Elapsed}");
        }

        [JobDisplayName("Bokun")]
        public void LoadAgeGroupForBokun()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForBokun|Starting loading Age group for Bokun");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Bokun";
            _ageGroupService.SaveBokunAgeGroups(token);

            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForBokun|Loaded Age group list for Bokun in {watch.Elapsed}");
        }

        /// <summary>
        /// Load ApiTude
        /// </summary>
        [JobDisplayName("ApiTude")]
        public void LoadContentForApiTude()
        {
            _log.Info("DataDumpingQueue.Function|LoadApiTude|Starting loading for ApiTude Content");
            var watch = Stopwatch.StartNew();
            var token = "Content_ApiTude";
            _ageGroupService.SaveAPITudeContentData(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadApiTude|Loaded list for ApiTude Content in {watch.Elapsed}");
        }

        public void LoadRedeamData()
        {
            _log.Info("DataDumpingQueue.Function|LoadRedeamData|Starting loading Redeam Supplier, Products, Rates and AgeGroup data");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Redeam";
            _ageGroupService.SaveRedeamData(token);
            // cache call
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadRedeamData|Loaded Redeam Supplier, Products, Rates and AgeGroup data in {watch.Elapsed}");
        }

        public void LoadRedeamV12Data()
        {
            _log.Info("DataDumpingQueue.Function|LoadRedeamV12Data|Starting loading Redeam Supplier, Products, Rates and AgeGroup data");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_RedeamV12";
            _ageGroupService.SaveRedeamV12Data(token);
            // cache call
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadRedeamV12Data|Loaded Redeam Supplier, Products, Rates and AgeGroup data in {watch.Elapsed}");
        }

        /// <summary>
        /// Load AgeGroup For Rezdy
        /// </summary>
        [JobDisplayName("AgeGroup_Rezdy")]
        public void LoadAgeGroupForRezdy()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForRezdy|Starting loading Age group for Rezdy");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Rezdy";
            _cacheLoaderService.LoadRezdyPaxMappingsAsync().GetAwaiter().GetResult();
            _cacheLoaderService.LoadRezdyLabelDetailsAsync().GetAwaiter().GetResult();
            _ageGroupService.SaveRezdyDataInDB(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForRezdy|Loaded Age group list for Rezdy in {watch.Elapsed}");
        }

        /// <summary>
        /// Load AgeGroup For Rezdy
        /// </summary>
        [JobDisplayName("GlobalTix")]
        public void LoadAgeGroupForGlobalTix()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForGlobalTix|Starting loading Age group for GlobalTix");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_GlobalTix";
            _ageGroupService.SaveGlobalTixCountryCityList(token);
            _ageGroupService.SaveGlobalTixActivities(token);
            _ageGroupService.SaveGlobalTixPackages(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForGlobalTix|Loaded Age group list for GlobalTix in {watch.Elapsed}");
        }
        [JobDisplayName("GlobalTixV3")]
        public void LoadAgeGroupForGlobalTixV3()
        {

            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForGlobalTixV3|Starting loading Age group for GlobalTixV3");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_GlobalTixV3";
            _ageGroupService.SaveGlobalTixCountryCityListV3(token);
            _ageGroupService.SaveGlobalTixActivitiesV3Async(token);
            _ageGroupService.SaveGlobalTixCategoriesV3(token);
            _ageGroupService.SaveGlobalTixV3PackageOptions(token);
            _cacheLoaderService.LoadGlobalTixV3PaxMappingsAsync().GetAwaiter().GetResult();
            
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForGlobalTixV3|Loaded Age group list for GlobalTixV3 in {watch.Elapsed}");
        }

        [JobDisplayName("NewCitySightseeing")]
        public void LoadNewCitySightSeeingData()
        {
            _log.Info("DataDumpingQueue.Function|LoadNewCitySightSeeingData|Starting data for NewCitySightSeeingData");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_NewCitySightSeeing";
            _ageGroupService.SaveNewCitySightSeeingProductList(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadNewCitySightSeeingData|Loaded data for NewCitySightSeeingData in {watch.Elapsed}");
        }

        [JobDisplayName("GoCity")]
        public void LoadGoCityData()
        {
            _log.Info("DataDumpingQueue.Function|LoadGoCityData|Starting data for LoadGoCityData");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_GoCity";
            _ageGroupService.SaveGoCityProductList(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadGoCityData|Loaded data for LoadGoCityData in {watch.Elapsed}");
        }
        [JobDisplayName("PrioHub")]
        public void LoadPrioHub()
        {
            _log.Info("DataDumpingQueue.Function|LoadPrioHub|Starting data for LoadPrioHub");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_NewPrio";
            _ageGroupService.SavePrioHubProductData(token);

            _log.Info($"DataDumpingQueue.Function|LoadPrioHub|Loaded data for LoadPrioHub in {watch.Elapsed}");
        }
        [JobDisplayName("Rayana")]
        public void LoadRaynaData()
        {
            _log.Info("DataDumpingQueue.Function|LoadRaynaData|Starting data for LoadRaynaData");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Rayna";
            _ageGroupService.SaveRaynaProductList(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadRaynaData|Loaded data for LoadRaynaData in {watch.Elapsed}");
        }

        [JobDisplayName("AgeGroup")]
        public void LoadAgeGroupsToCache()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupsToCache|Starting loading AgeGroups");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroups_Cache";
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
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupsToCache|Loaded AgeGroups in {watch.Elapsed}");
        }
        [JobDisplayName("CssExternal")]
        public void LoadCssExternalProductData()
        {
            _log.Info("DataDumpingQueue.Function|LoadCssExternalProductData|Starting data for LoadCssExternalProductData");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_CssExternal";
            _ageGroupService.SaveExternalProducts(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadCssExternalProductData|Loaded data for LoadCssExternalProductData in {watch.Elapsed}");
        }
        [JobDisplayName("RedemptionCss")]
        public void LoadTourCmsRedemptionData()
        {
            _log.Info("DataDumpingQueue.Function|LoadTourCmsRedemptionData|Starting data for LoadTourCmsRedemptionData");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_LoadTourCmsRedemptionData";
            _ageGroupService.SaveCssRedemptionBooking(token);
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadTourCmsRedemptionData|Loaded data for LoadTourCmsRedemptionData in {watch.Elapsed}");
        }
        public void LoadAgeGroupForVentrata()
        {
            _log.Info("DataDumpingQueue.Function|LoadAgeGroupForVentrata|Starting loading Age group for Ventrata");
            var watch = Stopwatch.StartNew();
            var token = "AgeGroup_Ventrata";
            _ageGroupService.SaveVentrataProductDetails(token);
            _cacheLoaderService.LoadVentrataPaxMappingsAsync().GetAwaiter().GetResult();
            watch.Stop();
            _log.Info($"DataDumpingQueue.Function|LoadAgeGroupForVentrata|Loaded Age group list for Ventrata in {watch.Elapsed}");
        }

       

    }
}