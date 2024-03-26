using DataDumping.WebJob.Contracts;
using DataDumping.WebJob.CustomTimerClasses;
using Isango.Entities;
using Isango.Entities.ElasticData;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Util;


namespace DataDumping.WebJob
{
    public class Functions
    {
        private readonly IServiceAvailabilityService _serviceAvailabilityService;
        private readonly IDataDumpingHelper _dataDumpingHelper;
        private readonly ILogger _log;
        private readonly IStorageOperationService _storageOperationService;
        private readonly IGoogleMapsService _googleMapsService;
        private readonly IMasterPersistence _masterPersistence;

        public const string ElasticSearchAPIEndPoint = "ElasticSearchAPIEndPoint";
        public const string ElasticDestinationEndPoint = "ElasticDestinationEndPoint";
        public const string ElasticProductsEndPoint = "ElasticProductsEndPoint";
        public const string ElasticAttractionsEndPoint = "ElasticAttractionsEndPoint";
        public const string ElasticAffiliateEndPoint = "ElasticAffiliatesEndPoint";
        public const string ErrorMail = "ErrorMail";
        private readonly IMailerService _mailerService;

        public Functions(IServiceAvailabilityService serviceAvailabilityService, IDataDumpingHelper dataDumpingHelper, ILogger log,
            IStorageOperationService storageOperationService, IGoogleMapsService googleMapsService,
             IMasterPersistence masterPersistence, IMailerService mailerService = null)
        {
            _serviceAvailabilityService = serviceAvailabilityService;
            _dataDumpingHelper = dataDumpingHelper;
            _log = log;
            _storageOperationService = storageOperationService;
            _googleMapsService = googleMapsService;
            _masterPersistence = masterPersistence;
            _mailerService = mailerService;

        }

        public Functions()
        {
            //Startup.Application_Start();
            //ContainerBuilder builder = Startup._builder;
            //builder.RegisterType<Functions>();
            //builder.RegisterType<DataDumpingHelper>().As<IDataDumpingHelper>().InstancePerLifetimeScope();
            //using (ILifetimeScope context = builder.Build().BeginLifetimeScope())
            //{
            //    _dataDumpingHelper = context.Resolve<IDataDumpingHelper>();
            //    _serviceAvailabilityService = context.Resolve<IServiceAvailabilityService>();
            //    _log = context.Resolve<ILogger>();
            //    _storageOperationService = context.Resolve<IStorageOperationService>();
            //    _googleMapsService = context.Resolve<IGoogleMapsService>();
            //    _masterPersistence = context.Resolve<IMasterPersistence>();
            //    _mailerService = context.Resolve<IMailerService>();
            //}
        }

        public void DeleteExistingAvailabilityDetails([TimerTrigger(typeof(DeleteExistingAvailabilityTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|DeleteExistingAvailabilityDetails|Starting deleting existing availabilities from database");
            var watch = Stopwatch.StartNew();
            _serviceAvailabilityService.DeleteExistingAvailabilityDetails();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|DeleteExistingAvailabilityDetails|Deleted existing availabilities from database in {watch.Elapsed}");
        }

        public void LoadMoulinRougeServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadMoulinRougeServiceAvailabilities|Starting loading availabilities for Moulin Rouge");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadMoulinRougeData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadMoulinRougeServiceAvailabilities|Loaded availabilities for Moulin Rouge in {watch.Elapsed}");
        }

        public void LoadGrayLineIceLandServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadGrayLineIceLandServiceAvailabilities|Starting loading availabilities for GrayLine IceLand");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadGrayLineIceLandData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadGrayLineIceLandServiceAvailabilities|Loaded availabilities for GrayLine IceLand in {watch.Elapsed}");
        }

        public void LoadAotServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadAotServiceAvailabilities|Starting loading availabilities for AOT");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadAotData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadAotServiceAvailabilities|Loaded availabilities for AOT in {watch.Elapsed}");
        }

        public void LoadPrioServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadPrioServiceAvailabilities|Starting loading availabilities for Prio");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadPrioData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadPrioServiceAvailabilities|Loaded availabilities for Prio in {watch.Elapsed}");
        }

        /// <summary>
        /// Dumping price and availability for GoldenTours API Product.
        /// </summary>
        /// <param name="timer"></param>
        public void LoadGoldenToursServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadGoldenToursServiceAvailabilities|Starting loading availabilities for GoldenTours");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadGoldenToursData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadGoldenToursServiceAvailabilities|Loaded availabilities for GoldenTours in {watch.Elapsed}");
        }

        /// <summary>
        /// Dumping price and availability for TourCMS API Product.
        /// </summary>
        /// <param name="timer"></param>
        public void LoadTourCMSServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|TourCMSServiceAvailabilities|Starting loading availabilities for TourCMS");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadTourCMSData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|TourCMSServiceAvailabilities|Loaded availabilities for TourCMS in {watch.Elapsed}");
        }

        public void LoadIsangoServiceAvailabilities([QueueTrigger("loadisangoserviceavailabilities")] string message, TextWriter log)
        {
            _log.Info("DataDumping.WebJob.Function|LoadIsangoServiceAvailabilities|Starting loading availabilities for Isango");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadIsangoData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadIsangoServiceAvailabilities|Loaded availabilities for Isango in {watch.Elapsed}");
        }

        public void LoadAPIImagesToCloudinary([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadAPIImagesToCloudinary|Starting loading Images in cloudinary for Isango");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadAPIImages();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadAPIImagesToCloudinary|Loaded Images in cloudinary in {watch.Elapsed}");
        }

        public void InitiateLoadIsangoServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _storageOperationService.AddMessageToQueue("loadisangoserviceavailabilities", "1");
        }

        public void LoadCancellationPolicies([TimerTrigger(typeof(CancellationPoliciesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadCancellationPolicies|Starting loading cancellation policies");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadCancellationPolicies();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadCancellationPolicies|Loaded cancellation policies in {watch.Elapsed}");
        }

        /// <summary>
        /// Dumping price and availability for Redeam API Product.
        /// </summary>
        /// <param name="timer"></param>
        public void LoadRedeamServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadRedeamServiceAvailabilities|Starting loading availabilities for Redeam");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadRedeamData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadRedeamServiceAvailabilities|Loaded availabilities for Redeam in {watch.Elapsed}");
        }

        /// <summary>
        /// Load hotelbeds price and availability
        /// </summary>
        /// <param name="timer"></param>
        //public void LoadHBApitudeServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        //{
        //    _log.Info("DataDumping.WebJob.Function|LoadHBApitudeServiceAvailabilities|Starting loading availabilities for HBApitude");
        //    var watch = Stopwatch.StartNew();
        //    _serviceAvailabilityService.SaveApiTudeAvailabilities();
        //    _serviceAvailabilityService.SyncPriceAvailabilities();
        //    watch.Stop();
        //    _log.Info($"DataDumping.WebJob.Function|LoadHBApitudeServiceAvailabilities|Loaded availabilities for HBApitude in {watch.Elapsed}");
        //}

        //New Function
        public void LoadHBApitudeServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadHBApitudeServiceAvailabilities|Starting loading availabilities for HBApitude");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadHBApitudeData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadHBApitudeServiceAvailabilities|Loaded availabilities for HBApitude in {watch.Elapsed}");
        }

        public void LoadRezdyServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadRezdyServiceAvailabilities|Starting loading availabilities for Rezdy");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadRezdyData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadRezdyServiceAvailabilities|Loaded availabilities for Rezdy in {watch.Elapsed}");
        }

        public void LoadGlobalTixServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadGlobalTixServiceAvailabilities|Starting loading availabilities for GlobalTix");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadGlobalTixData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadGlobalTixServiceAvailabilities|Loaded availabilities for GlobalTix in {watch.Elapsed}");
        }

        public void LoadVentrataAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadVentrataAvailabilities|Starting loading availabilities for Ventrata");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadVentrataData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadVentrataAvailabilities|Loaded availabilities for Ventrata in {watch.Elapsed}");
        }

        public void LoadNewCitySightSeeingServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadNewCitySightSeeingServiceAvailabilities|Starting loading availabilities for NewCitySightSeeing");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadNewCitySightSeeingData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadNewCitySightSeeingServiceAvailabilities|Loaded availabilities for NewCitySightSeeing in {watch.Elapsed}");
        }

        public void LoadPrioHubServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadPrioHubServiceAvailabilities|Starting loading availabilities for NewPrio");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadPrioHubData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadPrioHubServiceAvailabilities|Loaded availabilities for NewPrio in {watch.Elapsed}");
        }

        public void LoadRaynaServiceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        {
            _log.Info("DataDumping.WebJob.Function|LoadRaynaServiceAvailabilities|Starting loading availabilities for Rayna");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadRaynaData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadRaynaServiceAvailabilities|Loaded availabilities for Rayna in {watch.Elapsed}");
        }

        //public void SyncPriceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        //{
        //    _log.Info("DataDumping.WebJob.Function|SyncPriceAvailabilities|Starting syncing availabilities");
        //    var watch = Stopwatch.StartNew();
        //    _serviceAvailabilityService.SyncPriceAvailabilities();
        //    watch.Stop();
        //    _log.Info($"DataDumping.WebJob.Function|SyncPriceAvailabilities|Synced all the availabilities in {watch.Elapsed}");
        //}
        //public void SyncPriceAvailabilities([TimerTrigger(typeof(ServiceAvailabilitiesTime))] TimerInfo timer)
        //{
        //    _log.Info("DataDumping.WebJob.Function|SyncPriceAvailabilities|Starting syncing availabilities");
        //    var watch = Stopwatch.StartNew();
        //    _serviceAvailabilityService.SyncPriceAvailabilities();
        //    watch.Stop();
        //    _log.Info($"DataDumping.WebJob.Function|SyncPriceAvailabilities|Synced all the availabilities in {watch.Elapsed}");
        //}

        /// <summary>
        /// To Initiate the Google Feeds
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateGoogleFeeds([TimerTrigger(typeof(InitiateGoogleFeeds))] TimerInfo timer)
        {
            _storageOperationService.AddMessageToQueue("merchantfeedqueue", "1");
        }

        /// <summary>
        /// To Initiate the Age Dumping
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateAgeDumping([TimerTrigger(typeof(AgeDumpingTime))] TimerInfo timer)
        {
            //var AgeDumpingAPIs = ConfigurationManagerHelper.GetValuefromAppSettings("AgeDumpingAPIs").Split(',');
            List<string> AgeDumpingAPIs = _dataDumpingHelper.GetAgeDumpingAPIs();
            if (AgeDumpingAPIs != null && AgeDumpingAPIs.Count > 0)
            {
                foreach (var API in AgeDumpingAPIs)
                {
                    _storageOperationService.AddMessageToQueue("datadumping-queue", API.Trim());
                }
            }
        }

        /// <summary>
        /// To Processs the Inventory RTU
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateInventoryRealTimeUpdate([TimerTrigger(typeof(InitiateInventoryRealTimeUpdate))] TimerInfo timer)
        {
            _googleMapsService.InventoryRealTimeUpdate();
        }

        /// <summary>
        /// Initiates the Order Notification RTU
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateOrderNotificationRealTimeUpdate([TimerTrigger(typeof(InitiateOrderNotificationRealTimeUpdate))] TimerInfo timer)
        {
            _googleMapsService.OrderNotificationRealTimeUpdate();
        }

        #region Console App Functions

        /// <summary>
        /// Dumping price and availability for Tiqets API Product.s
        /// </summary>
        /// <param name="timer"></param>
        public void LoadTiqetsServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadTiqetsServiceAvailabilities|Starting loading availabilities for Tiqets");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadTiqetsData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadTiqetsServiceAvailabilities|Loaded availabilities for Tiqets in {watch.Elapsed}");
        }

        public void LoadGrayLineIceLandServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadGrayLineIceLandServiceAvailabilities|Starting loading availabilities for GrayLine IceLand");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadGrayLineIceLandData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadGrayLineIceLandServiceAvailabilities|Loaded availabilities for GrayLine IceLand in {watch.Elapsed}");
        }

        //Note: Not dumping Bokun data as its request is invalid which we are getting from database
        public void LoadBokunServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadBokunServiceAvailabilities|Starting loading availabilities for Bokun");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadBokunData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadBokunServiceAvailabilities|Loaded availabilities for Bokun in {watch.Elapsed}");
        }

        public void DeleteExistingAvailabilityDetails()
        {
            _log.Info("DataDumping.WebJob.Function|DeleteExistingAvailabilityDetails|Starting deleting existing availabilities from database");
            var watch = Stopwatch.StartNew();
            _serviceAvailabilityService.DeleteExistingAvailabilityDetails();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|DeleteExistingAvailabilityDetails|Deleted existing availabilities from database in {watch.Elapsed}");
        }

        public void LoadAotServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadAotServiceAvailabilities|Starting loading availabilities for AOT");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadAotData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadAotServiceAvailabilities|Loaded availabilities for AOT in {watch.Elapsed}");
        }

        public void LoadFareHarborServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadFareHarborServiceAvailabilities|Starting loading availabilities for Fare Harbor");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadFareHarborData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadFareHarborServiceAvailabilities|Loaded availabilities for Fare Harbor in {watch.Elapsed}");
        }

        public void LoadPrioServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadPrioServiceAvailabilities|Starting loading availabilities for Prio");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadPrioData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadPrioServiceAvailabilities|Loaded availabilities for Prio in {watch.Elapsed}");
        }

        public void LoadMoulinRougeServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadMoulinRougeServiceAvailabilities|Starting loading availabilities for Moulin Rouge");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadMoulinRougeData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadMoulinRougeServiceAvailabilities|Loaded availabilities for Moulin Rouge in {watch.Elapsed}");
        }

        public void LoadGoldenToursServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadGoldenToursServiceAvailabilities|Starting loading availabilities for GoldenTours");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadGoldenToursData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadGoldenToursServiceAvailabilities|Loaded availabilities for GoldenTours in {watch.Elapsed}");
        }

        public void LoadTourCMSServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|TourCMSServiceAvailabilities|Starting loading availabilities for TourCMS");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadTourCMSData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|TourCMSServiceAvailabilities|Loaded availabilities for TourCMS in {watch.Elapsed}");
        }

        public void LoadIsangoServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadIsangoServiceAvailabilities|Starting loading availabilities for Isango");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadIsangoData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadIsangoServiceAvailabilities|Loaded availabilities for Isango in {watch.Elapsed}");
        }

        public void LoadAPIImagesToCloudinary()
        {
            _log.Info("DataDumping.WebJob.Function|LoadAPIImagesToCloudinary|Starting loading Images in cloudinary for Isango");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadAPIImages();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadAPIImagesToCloudinary|Loaded Images in cloudinary in {watch.Elapsed}");
        }

        public void InitiateLoadIsangoServiceAvailabilities()
        {
            _storageOperationService.AddMessageToQueue("loadisangoserviceavailabilities", "1");
        }

        public void LoadCancellationPolicies()
        {
            _log.Info("DataDumping.WebJob.Function|LoadCancellationPolicies|Starting loading cancellation policies");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadCancellationPolicies();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadCancellationPolicies|Loaded cancellation policies in {watch.Elapsed}");
        }

        /// <summary>
        /// Dumping price and availability for Redeam API Product.
        /// </summary>
        /// <param name="timer"></param>
        public void LoadRedeamServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadRedeamServiceAvailabilities|Starting loading availabilities for Redeam");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadRedeamData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadRedeamServiceAvailabilities|Loaded availabilities for Redeam in {watch.Elapsed}");
        }

        //New Function
        public void LoadHBApitudeServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadHBApitudeServiceAvailabilities|Starting loading availabilities for HBApitude");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadHBApitudeData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadHBApitudeServiceAvailabilities|Loaded availabilities for HBApitude in {watch.Elapsed}");
        }

        //public void LoadRezdyServiceAvailabilities()
        //{
        //    _log.Info("DataDumping.WebJob.Function|LoadRezdyServiceAvailabilities|Starting loading availabilities for Rezdy");
        //    var watch = Stopwatch.StartNew();
        //    _dataDumpingHelper.LoadRezdyData();
        //    watch.Stop();
        //    _log.Info($"DataDumping.WebJob.Function|LoadRezdyServiceAvailabilities|Loaded availabilities for Rezdy in {watch.Elapsed}");
        //}

        public void LoadGlobalTixServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadGlobalTixServiceAvailabilities|Starting loading availabilities for GlobalTix");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadGlobalTixData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadGlobalTixServiceAvailabilities|Loaded availabilities for GlobalTix in {watch.Elapsed}");
        }

        public void LoadVentrataAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadVentrataAvailabilities|Starting loading availabilities for Ventrata");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadVentrataData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadVentrataAvailabilities|Loaded availabilities for Ventrata in {watch.Elapsed}");
        }

        public void LoadNewCitySightSeeingServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadNewCitySightSeeingServiceAvailabilities|Starting loading availabilities for NewCitySightSeeing");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadNewCitySightSeeingData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadNewCitySightSeeingServiceAvailabilities|Loaded availabilities for NewCitySightSeeing in {watch.Elapsed}");
        }

        public void LoadRaynaServiceAvailabilities()
        {
            _log.Info("DataDumping.WebJob.Function|LoadRaynaServiceAvailabilities|Starting loading availabilities for Rayna");
            var watch = Stopwatch.StartNew();
            _dataDumpingHelper.LoadRaynaData();
            watch.Stop();
            _log.Info($"DataDumping.WebJob.Function|LoadRaynaServiceAvailabilities|Loaded availabilities for Rayna in {watch.Elapsed}");
        }

        /// <summary>
        /// To Initiate the Google Feeds
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateGoogleFeeds()
        {
            _storageOperationService.AddMessageToQueue("merchantfeedqueue", "1");
        }

        /// <summary>
        /// To Initiate the Age Dumping
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateAgeDumping()
        {
            //var AgeDumpingAPIs = ConfigurationManagerHelper.GetValuefromAppSettings("AgeDumpingAPIs").Split(',');
            List<string> AgeDumpingAPIs = _dataDumpingHelper.GetAgeDumpingAPIs();
            if (AgeDumpingAPIs != null && AgeDumpingAPIs.Count > 0)
            {
                foreach (var API in AgeDumpingAPIs)
                {
                    _storageOperationService.AddMessageToQueue("datadumping-queue", API.Trim());
                }
            }
        }

        /// <summary>
        /// To Processs the Inventory RTU
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateInventoryRealTimeUpdate()
        {
            _googleMapsService.InventoryRealTimeUpdate();
        }

        /// <summary>
        /// Initiates the Order Notification RTU
        /// </summary>
        /// <param name="timer"></param>
        public void InitiateOrderNotificationRealTimeUpdate()
        {
            _googleMapsService.OrderNotificationRealTimeUpdate();
        }

        public void SendAbandonCartEmails()
        {
            try
            {
                _log.Info("DataDumping.WebJob.Function|SendAbandonCartEmails|Starting SendAbandonCartEmails");
                var watch = Stopwatch.StartNew();
                var serviceURL = ConfigurationManagerHelper.GetValuefromAppSettings("SendAbandonCartEmails");
                using (var client = new WebClient())
                {
                    // Request configuration
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Accept", "application/json");
                    var response = client.DownloadString(serviceURL);
                }
                watch.Stop();
                _log.Info($"DataDumping.WebJob.Function|SendAbandonCartEmails|Completed SendAbandonCartEmails in {watch.Elapsed}");
            }
            catch (System.Exception ex)
            {
            }
        }

        public void ElasticProductDataSave()
        {
            var productList = new List<ProductDatum>();
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var apiBaseURL = ConfigurationManagerHelper.GetValuefromAppSettings(ElasticSearchAPIEndPoint);
                var apiProductURl = ConfigurationManagerHelper.GetValuefromAppSettings(ElasticProductsEndPoint);
                var finalURL = apiProductURl+"?include=regionid,RegionName,servicename,Offer_Percent,url,boosterCount,bookingCount,location,servicename_regionname,isactive,clickCount,createdAt,updatedAt,image_linkprice,currency";
                //start - Data Get and Save
                try
                {
                    var getData = ElasticProductDataGetAndSave(apiBaseURL, finalURL);
                    if (getData != null)
                    {
                        productList.AddRange(getData);
                    }

                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumping.WebJob",
                        MethodName = "ElasticProductDataSave"
                    };
                    _log.Error(isangoErrorEntity, ex);
                    mailErrorLog.Add(Tuple.Create("DataDumping.WebJob|ElasticProductDataSave", "Error:" + ex));

                }

                if (productList != null && productList.Count() > 0)
                {
                    _masterPersistence.SaveElasticProducts(productList);
                }
                // end- Data Get and Save
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumping.WebJob",
                    MethodName = "ElasticProductDataSave"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("DataDumping.WebJob|ElasticProductDataSave", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }

        }
        private void SendMail(List<Tuple<string, string>> data)
        {
            if (Convert.ToString(ConfigurationManager.AppSettings["ErrorMail"]) == "1")
            {
                _mailerService?.SendErrorMail(data);
            }
        }
        private bool ElasticDataPost(string jsonData, string baseAddress, string api)
        {
            try
            {
                var client = new HttpClient();
                var httpTimeout = TimeSpan.FromMinutes(10);
                client.Timeout = httpTimeout;
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(contentType);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = client.PostAsync(api, contentData);
                response.Wait();
                if (response.Result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private List<ProductDatum> ElasticProductDataGetAndSave(string baseAddress, string apiURL)
        {
            try
            {
                var elasticProduct = new ElasticProduct();
                using (var client = new HttpClient())
                {
                    var httpTimeout = TimeSpan.FromMinutes(10);
                    client.Timeout = httpTimeout;
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Add(contentType);
                    var response = client.GetAsync(apiURL);
                    response.Wait();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var elasticProductStr = response?.Result?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                        elasticProduct = JsonConvert.DeserializeObject<ElasticProduct>(elasticProductStr);
                        if (elasticProduct?.Data?.Count() == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return elasticProduct?.Data;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion Console App Functions
    }
}