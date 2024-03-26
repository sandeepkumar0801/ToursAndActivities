using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using Isango.Persistence.Contract;
using Isango.Service.Canocalization;
using Isango.Service.ConsoleApplication.CriteriaHandlers;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Bokun;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Util;
using Activity = Isango.Entities.Activities.Activity;
using Constant = Isango.Service.Constants.Constant;
using Criteria = Isango.Entities.ConsoleApplication.ServiceAvailability.Criteria;
using PassengerType = Isango.Entities.Enums.PassengerType;

namespace Isango.Service.ConsoleApplication
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ServiceAvailabilityService : IServiceAvailabilityService
    {
        private readonly ILogger _log;
        private readonly IServiceAvailabilityPersistence _serviceAvailabilityPersistence;
        private readonly IMasterPersistence _masterPersistence;
        private readonly IGrayLineIceLandCriteriaService _grayLineIceLandCriteriaService;
        private readonly IMoulinRougeCriteriaService _moulinRougeCriteriaService;
        private readonly IPrioCriteriaService _prioCriteriaService;
        private readonly IFareHarborCriteriaService _fareHarborCriteriaService;
        private readonly IBokunCriteriaService _bokunCriteriaService;
        private readonly IAotCriteriaService _aotCriteriaService;
        private readonly IVentrataCriteriaService _ventrataCriteriaService;
        private readonly ITiqetsCriteriaService _tiqetsCriteriaService;
        private readonly IGoldenToursCriteriaService _goldenToursCriteriaService;
        private readonly IApiTudeCriteriaService _apiTudeCriteriaService;
        private readonly IApiTudePersistence _apiTudePersistence;
        private readonly IRedeamCriteriaService _redeamCriteriaService;
        private readonly IGoogleMapsDataDumpingService _googleMapsDataDumpingService;
        private readonly ICartService _cartService;
        private readonly IBokunAdapter _bokunAdapter;
        private readonly IRezdyCriteriaService _rezdyCriteriaService;
        private readonly IGlobalTixCriteriaService _globalTixCriteriaService;
        private readonly ITourCMSCriteriaService _tourCMSCriteriaService;
        private readonly INewCitySightSeeingCriteriaService _newCitySightSeeingCriteriaService;
        private readonly IPrioHubCriteriaService _prioHubCriteriaService;
        private readonly IRaynaCriteriaService _raynaCriteriaService;
        private readonly ICanocalizationService _canocalizationService;
        public ServiceAvailabilityService(IServiceAvailabilityPersistence serviceAvailabilityPersistence,
            IMasterPersistence masterPersistence
            , IGrayLineIceLandCriteriaService grayLineIceLandCriteriaService
            , IMoulinRougeCriteriaService moulinRougeCriteriaService
            , IPrioCriteriaService prioCriteriaService
            , IFareHarborCriteriaService fareHarborCriteriaService
            , IBokunCriteriaService bokunCriteriaService
            , ILogger logger
            , IAotCriteriaService aotCriteriaService
            , ITiqetsCriteriaService tiqetsCriteriaService
            , IGoldenToursCriteriaService goldenToursCriteriaService
            , IApiTudeCriteriaService apiTudeCriteriaService
            , IApiTudePersistence apiTudePersistence
            , IRedeamCriteriaService redeamCriteriaService
            , IGoogleMapsDataDumpingService googleMapsDataDumpingService
            , ICartService cartService
            , IBokunAdapter bokunAdapter
            , IRezdyCriteriaService rezdyCriteriaService
            , IGlobalTixCriteriaService globalTixCriteriaService
            , IVentrataCriteriaService ventrataCriteriaService
            , ITourCMSCriteriaService tourCMSCriteriaService
            , INewCitySightSeeingCriteriaService newCitySightSeeingCriteriaService
            , IPrioHubCriteriaService prioHubCriteriaService
            , IRaynaCriteriaService raynaCriteriaService
            , ICanocalizationService canocalizationService
            )
        {
            _serviceAvailabilityPersistence = serviceAvailabilityPersistence;
            _masterPersistence = masterPersistence;
            _grayLineIceLandCriteriaService = grayLineIceLandCriteriaService;
            _moulinRougeCriteriaService = moulinRougeCriteriaService;
            _prioCriteriaService = prioCriteriaService;
            _bokunCriteriaService = bokunCriteriaService;
            _log = logger;
            _fareHarborCriteriaService = fareHarborCriteriaService;
            _aotCriteriaService = aotCriteriaService;
            _tiqetsCriteriaService = tiqetsCriteriaService;
            _goldenToursCriteriaService = goldenToursCriteriaService;
            _apiTudeCriteriaService = apiTudeCriteriaService;
            _apiTudePersistence = apiTudePersistence;
            _redeamCriteriaService = redeamCriteriaService;
            _googleMapsDataDumpingService = googleMapsDataDumpingService;
            _cartService = cartService;
            _bokunAdapter = bokunAdapter;
            _rezdyCriteriaService = rezdyCriteriaService;
            _globalTixCriteriaService = globalTixCriteriaService;
            _ventrataCriteriaService = ventrataCriteriaService;
            _tourCMSCriteriaService = tourCMSCriteriaService;
            _newCitySightSeeingCriteriaService = newCitySightSeeingCriteriaService;
            _prioHubCriteriaService = prioHubCriteriaService;
            _raynaCriteriaService = raynaCriteriaService;
            _canocalizationService = canocalizationService;
        }

        /// <summary>
        /// Save GLI Availabilities
        /// </summary>
        /// <returns></returns>
        public void SaveServiceAvailabilitiesInDatabase(List<TempHBServiceDetail> serviceDetails)
        {
            try
            {
                //Save details in the database
                _serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "SaveServiceAvailabilitiesInDatabase"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveServiceAvailabilitiesInDatabaseForTiqets(List<TempHBServiceDetail> serviceDetails)
        {
            try
            {
                //Save details in the database
                _serviceAvailabilityPersistence.SaveServiceAvailabilitiesForTiqets(serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "SaveServiceAvailabilitiesInDatabaseForTiqets"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveQuestionsInDatabase(List<ExtraDetailQuestions> Details, int ApiType)
        {
            try
            {
                _serviceAvailabilityPersistence.SaveQuestionsInDB(Details, ApiType);
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                    _log.Error(new Isango.Entities.IsangoErrorEntity
                    {
                        ClassName = "ServiceAvailabilityService",
                        MethodName = "SaveQuestionsInDatabase",
                    }, ex)
                );
            }
        }

        /// <summary>
        /// Save GLI Availabilities
        /// </summary>
        /// <returns></returns>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetGrayLineIceLandAvailabilities()
        {
            try
            {
                //Get GLI products
                var grayLineIceLandProducts = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Graylineiceland)).ToList();

                //Get GLI service details
                var availabilities = GetGrayLineIceLandAvailabilities(grayLineIceLandProducts);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _grayLineIceLandCriteriaService.GetServiceDetails(availabilities, grayLineIceLandProducts);

                //Dictionary<List<Isango.Entities.GoogleMaps.Question>, int> Questions = new Dictionary<List<Isango.Entities.GoogleMaps.Question>, int>();

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetGrayLineIceLandAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save MoulineRouge Availabilities
        /// </summary>
        /// <returns></returns>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetMoulinRougeAvailabilities()
        {
            try
            {
                //Get MR products
                var moulinRougeProducts = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Moulinrouge)).ToList();

                //Get MR service details
                var availabilities = GetMoulinRougeAvailabilities(moulinRougeProducts);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _moulinRougeCriteriaService.GetServiceDetails(availabilities, moulinRougeProducts);

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetMoulinRougeAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Prio Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetPrioAvailabilities()
        {
            try
            {
                //Get Prio products
                var prioProducts = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Prio)
                //&& (//  x.IsangoHotelBedsActivityId == 23350 ||
                //        x.IsangoHotelBedsActivityId == 5106
                //    )
                ).ToList();

                //Get Prio service details
                var availabilities = GetPrioAvailabilities(prioProducts);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _prioCriteriaService.GetServiceDetails(availabilities, prioProducts);

                //Uncomment to save in db from this service.
                //_serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails.ToList());

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetPrioAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Save Prio Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetPrioHubAvailabilities()
        {
            try
            {
                //Get New Prio products
                var prioHubProducts = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.PrioHub)).ToList();
                prioHubProducts = prioHubProducts?.Where(x => x.HotelBedsActivityCode != "").ToList();
                //prioHubProducts = prioHubProducts.Take(5).ToList();
                //prioHubProducts = prioHubProducts.Where(x => x.IsangoHotelBedsActivityId == 36904).ToList();
                //Get New Prio service details
                var availabilities = GetPrioHubAvailabilities(prioHubProducts);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _prioHubCriteriaService.GetServiceDetails(availabilities, prioHubProducts);
                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetNewPrioAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Save Fare harbor Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetFareHarborAvailabilities()
        {
            try
            {
                //Get Fare harbor products
                var fhbProducts = _masterPersistence.LoadLiveHBOptions()
                    .Where(x => x.ApiType.Equals(APIType.Fareharbor))
                    //.Take(10)
                    .ToList();

                var watch = System.Diagnostics.Stopwatch.StartNew();
                var startTime = DateTime.Now;

                //Get FHB service details
                var availabilities = GetFareHarborAvailabilities(fhbProducts);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _fareHarborCriteriaService.GetServiceDetails(availabilities, fhbProducts);
                watch.Stop();
                var tokenWithStartAndEndTime = $"FareHarbor StartTime {startTime.ToString("yyyy-MMM-dd HH:mm:ss")}, finishTime {DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")}, for {fhbProducts.Count} products";

                _log.WriteTimer($"FareHarborDumping", tokenWithStartAndEndTime, "FareHarbor", watch.Elapsed.ToString());

                //Uncomment for testing using test case in GetBokunAvailabilitiesTest
                //_serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails.ToList());

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetFareHarborAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Bokun Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetBokunAvailabilities()
        {
            try
            {
                //Get Bokun products
                var bokunProducts = _masterPersistence.LoadLiveHBOptions().Where(x => x.ApiType.Equals(APIType.Bokun)).ToList();

                //Uncomment for testing using test case in GetBokunAvailabilitiesTest
                //bokunProducts = bokunProducts.Where(x => x.IsangoHotelBedsActivityId == 5890 || x.IsangoHotelBedsActivityId == 3733).ToList();

                //Get Bokun service details
                var availabilities = GetBokunAvailabilities(bokunProducts);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _bokunCriteriaService.GetServiceDetails(availabilities, bokunProducts);

                //Uncomment for testing using test case in GetBokunAvailabilitiesTest
                //_serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails.ToList());

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetBokunAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Fare harbor Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetAotAvailabilities()
        {
            try
            {
                var watch2 = Stopwatch.StartNew();
                //Get Aot products
                var aotProducts = _masterPersistence.LoadLiveHBOptions().Where(x => x.ApiType.Equals(APIType.Aot)).ToList();

                watch2.Stop();
                _log.WriteTimer("GetAotAvailabilities", $"DataDumping_Product_Count:{aotProducts.Count}", APIType.Aot.ToString(), watch2.Elapsed.ToString());

                //Get AOT service details
                var watch3 = Stopwatch.StartNew();
                var availabilities = GetAotAvailabilities(aotProducts);
                watch3.Stop();
                _log.WriteTimer("GetAotAvailabilities", $"DataDumping_Availabilities_Count:{availabilities?.Count ?? 0}", APIType.Aot.ToString(), watch3.Elapsed.ToString());
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _aotCriteriaService.GetServiceDetails(availabilities, aotProducts);

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetAotAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Tiqets Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetTiqetsAvailabilities()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var startTime = DateTime.Now;
            var productCount = 0;
            try
            {


                //Get Tiqets products
                var products = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Tiqets))?.ToList();
                productCount = products.Count;
                var limitedProducts = products.ChunkBy(15);

                var availabilities = new List<Activity>();

                foreach (var chunkedProducts in limitedProducts)
                {
                    var chunkedavailabilities = GetTiqetsAvailabilities(chunkedProducts);
                    if (chunkedavailabilities != null && chunkedavailabilities?.Count > 0)
                        availabilities.AddRange(chunkedavailabilities);
                }
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get service details
                var serviceDetails = _tiqetsCriteriaService.GetServiceDetails(availabilities, products);

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetTiqetsAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            finally
            {
                var tokenWithStartAndEndTime = $"Tiqets StartTime {startTime.ToString("yyyy-MMM-dd HH:mm:ss")}, finishTime {DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")}, for {productCount} products";
                _log.WriteTimer($"tiqet-dumping", tokenWithStartAndEndTime, "FareHarbor", watch.Elapsed.ToString());
            }
        }

        /// <summary>
        /// Save GoldenTours Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetGoldenToursAvailabilities()
        {
            try
            {
                //Get GoldenTours products
                var products = _masterPersistence.LoadLiveHBOptions()
                    .Where(x => x.ApiType.Equals(APIType.Goldentours)
                    //&& x.IsangoHotelBedsActivityId == 21925
                    ).Distinct()
                    .ToList();

                /*
                var p = products.FirstOrDefault(x => x.IsangoHotelBedsActivityId == 24235);
                var p1 = products.FirstOrDefault(x => x.IsangoHotelBedsActivityId != 24235);
                products.Clear();
                products.Add(p);
                products.Add(p1);
                */

                //Get GoldenTours service details
                var availabilities = GetGoldenToursAvailabilities(products);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get Service Details
                var serviceDetails = _goldenToursCriteriaService.GetServiceDetails(availabilities, products);

                //_serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails);

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetGoldenToursAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        /// <summary>
        /// Save GoldenTours Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetTourCMSAvailabilities()
        {
            try
            {

                //Get GoldenTours products
                var products = _masterPersistence.LoadLiveHBOptions()
                    .Where(x => x.ApiType.Equals(APIType.TourCMS)
                    ).Distinct()
                    .ToList();


                //products = products.Where(x=>x.HotelBedsActivityCode=="27").ToList();

                //isangoOptionId_tourCMSId_(ChannelId_AccountId)_IsangoId

                //Get TourCMS service details
                var availabilities = GetTourCMSAvailabilities(products);
                if (availabilities == null || availabilities.Count == 0) return null;

                //Get Service Details
                var serviceDetails = _tourCMSCriteriaService.GetServiceDetails(availabilities, products);

                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetTourCMSAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetIsangoAvailabilities()
        {
            try
            {
                var serviceAvailabilityFeeds = GetServiceAvailabilityFeeds();
                var result = PrepareAvailabilitiesData(serviceAvailabilityFeeds);
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "GetIsangoAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Redeam Availabilities
        /// </summary>
        public void SaveRedeamAvailabilities()
        {
            try
            {
                //Get Redeam service details
                var serviceDetails = GetRedeamServiceDetails();

                //Save details in the database
                _serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails.Item2);
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|SaveRedeamAvailabilities", ex);
                throw;
            }
        }

        /// <summary>
        /// Delete existing availability data from database
        /// </summary>
        public void DeleteExistingAvailabilityDetails()
        {
            try
            {
                _serviceAvailabilityPersistence.DeleteExistingHBServiceDetails();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "DeleteExistingAvailabilityDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Sync availability data
        /// </summary>
        public void SyncPriceAvailabilities()
        {
            try
            {
                _serviceAvailabilityPersistence.SyncAPIPriceAvailabilities();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "SyncPriceAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Save ApiTude Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> SaveApiTudeAvailabilities(List<IsangoHBProductMapping> products)
        {
            try
            {
                ////Get ApiTude products
                //var products = _masterPersistence.LoadLiveHBOptions()?
                //               .Where(x => x.ApiType.Equals(APIType.Hotelbeds)
                //                            && !string.IsNullOrWhiteSpace(x.SupplierCode)
                //                     )
                //               .Distinct().OrderBy(x => x.FactSheetId)
                //               .ToList();
                /*
                var input = new int[] { 4098 };// 3626, 7663, 25288, 26228, 26594, 31447, 31347, 31737, 6591 };
                var pQuery = from p in products
                             from i in input
                             where p.IsangoHotelBedsActivityId == i
                             select p;
                //products = products.Take(1000).ToList();
                //products.AddRange(pQuery.ToList());
                products = pQuery.ToList();
                //*/

                //Get ApiTude service details
                var serviceDetails = GetApiTudeServiceDetails(products);

                return serviceDetails;
                //Save details in the database
                //_serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityService",
                    MethodName = "SaveApiTudeAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<GoogleCancellationPolicy> GetCancellationPolicies()
        {
            var token = "CancellationPolicies";
            var products = _masterPersistence.LoadLiveHBOptions();
            if (products == null)
                return null;
            var cancellationPolicies = new List<GoogleCancellationPolicy>();
            var bokunProducts = products.Where(x => x.ApiType.Equals(APIType.Bokun)).Distinct().ToList();
            var bokunCancellationPolicies = _bokunCriteriaService.GetBokunCancellationPolicies(bokunProducts, $"{token}_Bokun");
            cancellationPolicies.AddRange(bokunCancellationPolicies);

            var aotProducts = products.Where(x => x.ApiType.Equals(APIType.Aot)).Distinct().ToList();
            var aotCancellationPolicies = _aotCriteriaService.GetCancellationPolicies(aotProducts, $"{token}_AOT");
            cancellationPolicies.AddRange(aotCancellationPolicies);

            var serviceCancellationPolicies = GetServiceCancellationPolicy($"{token}_Isango");
            cancellationPolicies.AddRange(serviceCancellationPolicies);

            return cancellationPolicies;
        }

        /// <summary>
        /// Save GlobalTix Availabilities
        /// </summary>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> SaveGlobalTixAvailabilities()
        {
            try
            {
                //Get GlobalTix products
                var globalTixProducts = _masterPersistence.LoadLiveHBOptions().Where(x => x.ApiType.Equals(APIType.GlobalTix)).ToList();

                //Get GlobalTix service details
                var serviceDetails = GetGlobalTixServiceDetails(globalTixProducts);

                return serviceDetails;
                //SaveServiceAvailabilitiesInDatabase(serviceDetails);
                ////Save details in the database
                //_serviceAvailabilityPersistence.SaveServiceAvailabilitiesGTix(serviceDetails);
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|SaveGlobalTixAvailabilities", ex);
                throw;
            }
        }

        /// <summary>
        /// Save Ventrata Availabilities
        /// </summary>
        /// 
        public Tuple<List<Activity>, List<TempHBServiceDetail>> SaveGlobalTixV3Availabilities()
        {
            try
            {
                //Get GlobalTix products
                var globalTixProducts = _masterPersistence.LoadLiveHBOptions().Where(x => x.ApiType.Equals(APIType.GlobalTixV3)).ToList();
                //globalTixProducts = globalTixProducts.Where(x => x.IsangoHotelBedsActivityId == 29843).ToList();
                //Get GlobalTix service details
                var serviceDetails = GetGlobalTixV3ServiceDetails(globalTixProducts);

                return serviceDetails;
                //SaveServiceAvailabilitiesInDatabase(serviceDetails);
                ////Save details in the database
                //_serviceAvailabilityPersistence.SaveServiceAvailabilitiesGTix(serviceDetails);
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|SaveGlobalTixAvailabilities", ex);
                throw;
            }
        }
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetVentrataAvailabilities()
        {
            try
            {
                //Get Ventrata products
                var VentrataProducts = _masterPersistence.LoadLiveHBOptions().Where(x => x.ApiType.Equals(APIType.Ventrata)).ToList();
                var supplierDetails = GetVentrataData();
                //Get Ventrata service details
                var serviceDetails = GetVentrataServiceDetails(VentrataProducts, supplierDetails);

                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|GetVentrataAvailabilities", ex);
                throw;
            }
        }
        public List<Entities.Ventrata.SupplierDetails> GetVentrataData()
        {
            try
            {
                var memCache = MemoryCache.Default;
                var key = "getventratasupplierdata";
                var res = memCache.Get(key);
                if (res != null)
                {
                    return (List<Entities.Ventrata.SupplierDetails>)res;
                }
                else
                {
                    var VentrataData = _masterPersistence.GetVentrataSupplierDetails();
                    memCache.Add(key, VentrataData, DateTimeOffset.UtcNow.AddMinutes(5));
                    return VentrataData;
                }
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|GetVentrataData", ex);
                throw;
            }
        }




        #region "Private Methods"

        /// <summary>
        /// GetVentrataServiceDetails
        /// </summary>
        /// <param name="ventrataProducts"></param>
        /// <returns></returns>
        private Tuple<List<Activity>, List<TempHBServiceDetail>> GetVentrataServiceDetails(List<IsangoHBProductMapping> ventrataProducts,
            List<Entities.Ventrata.SupplierDetails> supplierDetails)
        {
            if (ventrataProducts?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();
            try
            {
                //Create criteria
                var criteria = CreateCriteria(ventrataProducts, "Ventrata");

                //Get GlobalTix availabilities
                var availabilities = _ventrataCriteriaService.GetAvailability(criteria, supplierDetails);

                if (availabilities?.Count > 0)
                {
                    //Get service details
                    serviceDetails = _ventrataCriteriaService.GetServiceDetails(availabilities, ventrataProducts);
                }
                return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get GLI availabilities
        /// </summary>
        /// <param name="grayLineIceLandProducts"></param>
        /// <returns></returns>
        private List<Activity> GetGrayLineIceLandAvailabilities(List<IsangoHBProductMapping> grayLineIceLandProducts)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (grayLineIceLandProducts?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            //Create criteria
            var criteria = CreateCriteria(grayLineIceLandProducts, "GrayLineIceLand");

            //Get GLI availabilities
            var availabilities = _grayLineIceLandCriteriaService.GetAvailability(criteria);

            return availabilities;
        }

        /// <summary>
        /// Get MR availabilities
        /// </summary>
        /// <param name="moulinRougeProducts"></param>
        /// <returns></returns>
        private List<Activity> GetMoulinRougeAvailabilities(List<IsangoHBProductMapping> moulinRougeProducts)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (moulinRougeProducts?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
            //Create criteria
            var criteria = CreateCriteria(moulinRougeProducts, "MoulinRouge");

            //Get MR availabilities
            var availabilities = _moulinRougeCriteriaService.GetAvailability(criteria);
            return availabilities;
        }

        /// <summary>
        /// Get Prio availabilities
        /// </summary>
        /// <param name="prioProducts"></param>
        /// <returns></returns>
        private List<Activity> GetPrioAvailabilities(List<IsangoHBProductMapping> prioProducts)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (prioProducts?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            //Create criteria
            var criteria = CreateCriteria(prioProducts, "Prio");

            //Get Prio availabilities
            var availabilities = _prioCriteriaService.GetAvailability(criteria);
            return availabilities;
        }

        /// <summary>
        /// Get Fareharbor availabilities
        /// </summary>
        /// <param name="fhbProducts"></param>
        /// <returns></returns>
        private List<Activity> GetFareHarborAvailabilities(List<IsangoHBProductMapping> fhbProducts)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (!(fhbProducts?.Count > 0)) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            //Create criteria
            var criteria = CreateCriteria(fhbProducts, "fareharbor");

            //Get Fareharbor availabilities
            var availabilities = _fareHarborCriteriaService.GetAvailability(criteria);
            return availabilities;
        }

        /// <summary>
        /// Get Bokun availabilities
        /// </summary>
        /// <param name="bokunProducts"></param>
        /// <returns></returns>
        private List<Activity> GetBokunAvailabilities(List<IsangoHBProductMapping> bokunProducts)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (bokunProducts?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            //Create criteria
            var criteria = CreateCriteria(bokunProducts, "Bokun");
            var priceCategoryIdMapping = _masterPersistence.GetBokunPriceCategoryByActivity();

            //Get Bokun availabilities
            var availabilities = _bokunCriteriaService.GetAvailability(criteria, priceCategoryIdMapping);

            return availabilities;
        }

        /// <summary>
        /// Get AOT availabilities
        /// </summary>
        /// <param name="aotProducts"></param>
        /// <returns></returns>
        private List<Activity> GetAotAvailabilities(List<IsangoHBProductMapping> aotProducts)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (aotProducts?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
            //Create criteria
            var criteria = CreateCriteria(aotProducts, $"AOT");

            //Get Fare harbor availabilities
            var availabilities = _aotCriteriaService.GetAvailability(criteria);
            return availabilities;
        }

        /// <summary>
        /// Get Tiqets availabilities
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Activity> GetTiqetsAvailabilities(List<IsangoHBProductMapping> products)
        {
            if (!(products?.Count > 0)) return null;

            //Create criteria
            var criteria = CreateTiqetsCriteria(products, $"Tiqets");

            //Get Tiqets availabilities
            var availabilities = _tiqetsCriteriaService.GetAvailability(criteria);
            return availabilities;
        }

        /// <summary>
        /// Get GoldenTours availabilities
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Activity> GetGoldenToursAvailabilities(List<IsangoHBProductMapping> products)
        {
            if (products?.Count <= 0) return null;

            var criteria = CreateCriteria(products, "GoldenTours");
            var availabilities = _goldenToursCriteriaService.GetAvailability(criteria);

            return availabilities;
        }

        /// <summary>
        /// Get ApiTude service details
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private Tuple<List<Activity>, List<TempHBServiceDetail>> GetApiTudeServiceDetails(List<IsangoHBProductMapping> products)
        {
            if (products?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            var criteria = CreateCriteria(products, "ApiTude");
            var availabilities = _apiTudeCriteriaService.GetAvailability(criteria);

            //1. Save AgeGroup
            try
            {
                Task.Run(() =>
                    _apiTudePersistence.SaveApiTudeAgeGroups(availabilities?.Item2)
                );
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|GetApiTudeServiceDetails", ex);
                throw;
            }

            //2. Save availabilities
            if (availabilities?.Item1?.Count > 0)
                serviceDetails = _apiTudeCriteriaService.GetServiceDetails(availabilities.Item1, products);

            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities?.Item1, serviceDetails);
        }

        /// <summary>
        /// Get Redeam service details
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetRedeamServiceDetails()
        {
            //Get Redeam products
            var products = _masterPersistence.LoadLiveHBOptions()
                .Where(x => x.ApiType.Equals(APIType.Redeam)
                        && !string.IsNullOrWhiteSpace(x.PrefixServiceCode)
                       //&& x.IsangoHotelBedsActivityId == 5148//--27143
                       )
                .Distinct()
                .ToList();

            if (products?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            var criteria = CreateCriteria(products, "Redeam");
            var availabilities = _redeamCriteriaService.GetAvailability(criteria);
            if (availabilities?.Count > 0)
                serviceDetails = _redeamCriteriaService.GetServiceDetails(availabilities, products);

            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }

        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetRedeamV12ServiceDetails()
        {
            //Get Redeam products
            var products = _masterPersistence.LoadLiveHBOptions()
                .Where(x => x.ApiType.Equals(APIType.RedeamV12)
                        && !string.IsNullOrWhiteSpace(x.PrefixServiceCode)
                       //&& x.IsangoHotelBedsActivityId == 24155//--27143
                       )
                .Distinct()
                .ToList();

            //var products = new List<IsangoHBProductMapping>();
            //HotelBedsActivityCode should be in  'ProductId#RateId' format
            //SupplierCode is supplierid
            //PrefixServiceCode is Type
            //1.)PASS TYPE
            //var dataIsangoHBProductMapping = new IsangoHBProductMapping
            //{
            //    ApiType = APIType.RedeamV12,
            //    HotelBedsActivityCode = "12d95589-c6bf-4fa6-ab5b-43556c0cbd74#8682fa44-8912-4def-9342-53f56ab355d5",
            //    SupplierCode = "d88ccd8e-2435-403f-99f8-80238cb602c9",
            //    PrefixServiceCode = "PASS",
            //    IsangoHotelBedsActivityId = 27143,
            //    ServiceOptionInServiceid = 123,
            //    FactSheetId = 0,
            //    IsIsangoMarginApplicable = false,
            //    IsMarginPercent = false,
            //    MarginAmount = 0,
            //    MinAdultCount = 1,
            //    PriceTypeId = 0,
            //    CountryId = 0,
            //    Credentials = string.Empty,
            //    DestinationCode = string.Empty,
            //    Language = "en",
            //    IsangoRegionId = 0,
            //    CurrencyISOCode = "USD",
            //};

            //HotelBedsActivityCode should be in  'ProductId#RateId' format
            //SupplierCode is supplierid
            //PrefixServiceCode is Type
            //var dataIsangoHBProductMapping = new IsangoHBProductMapping
            //{
            //    ApiType = APIType.RedeamV12,
            //    HotelBedsActivityCode = "02f0c6cb-77ae-4fcc-8f4d-99bc0c3bee18#0666f27f-2f16-4eba-91b7-28f08ce095d2",
            //    SupplierCode = "fc49b925-6942-4df8-954b-ed7df10adf7e",
            //    PrefixServiceCode = "RESERVED",
            //    IsangoHotelBedsActivityId = 27141,
            //    ServiceOptionInServiceid = 1234,
            //    FactSheetId = 0,
            //    IsIsangoMarginApplicable = false,
            //    IsMarginPercent = false,
            //    MarginAmount = 0,
            //    MinAdultCount = 1,
            //    PriceTypeId = 0,
            //    CountryId = 0,
            //    Credentials = string.Empty,
            //    DestinationCode = string.Empty,
            //    Language = "en",
            //    IsangoRegionId = 0,
            //    CurrencyISOCode = "USD",
            //};
            //products.Add(dataIsangoHBProductMapping);

            if (products?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            var criteria = CreateCriteria(products, "RedeamV12");
            criteria.ApiType = APIType.RedeamV12;
            var availabilities = _canocalizationService.GetAvailability(criteria);
            if (availabilities?.Count > 0)
                serviceDetails = _canocalizationService.GetServiceDetails(availabilities, products, PriceDataType.CostAndSell, APIType.RedeamV12);

            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }

        /// <summary>
        /// Get TourCMS availabilities
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Activity> GetTourCMSAvailabilities(List<IsangoHBProductMapping> products)
        {
            if (products?.Count <= 0) return null;

            var criteria = CreateCriteria(products, "TourCMS");
            var availabilities = _tourCMSCriteriaService.GetAvailability(criteria);
            return availabilities;
        }

        /// <summary>
        /// Create criteria
        /// </summary>
        /// <param name="products"></param>
        /// <param name="apiName"> api name for which dumping is going to run</param>
        /// <returns></returns>
        private Criteria CreateCriteria(List<IsangoHBProductMapping> products, string token)
        {
            token = $"{token?.ToLower()}-dumping";
            var days_loop = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days4Loop));
            var days = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days2FetchForHeavyData));
            var months = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Months2FetchForHeavyData));

            int Monthsfromdays = (int)Math.Ceiling((double)days / 30);
            if (Monthsfromdays == 0)
            {
                days_loop = 30;
                days = 30;
                months = 1;
                Monthsfromdays = (int)Math.Ceiling((double)days / 30);
            }

            //Set criteria
            var criteria = new Criteria
            {
                MappedProducts = products,
                Days2Fetch = days_loop,
                Months2Fetch = Monthsfromdays,
                Counter = (int)Math.Ceiling((double)(months * 30) / days_loop),
                SameDay = false,
                Token = token,
                Language = "en"
            };

            return criteria;
        }
        /// <summary>
        /// Create criteria
        /// </summary>
        /// <param name="products"></param>
        /// <param name="apiName"> api name for which dumping is going to run</param>
        /// <returns></returns>
        private Criteria CreateCriteriaLightData(List<IsangoHBProductMapping> products, string token)
        {
            token = $"{token?.ToLower()}-dumping";
            var days_loop = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days4LoopForLightData));
            var days = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days2FetchForLightData));
            var months = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Months2FetchForLightData));

            int Monthsfromdays = (int)Math.Ceiling((double)days / 30);
            if (Monthsfromdays == 0)
            {
                days_loop = 30;
                days = 30;
                months = 1;
                Monthsfromdays = (int)Math.Ceiling((double)days / 30);
            }

            //Set criteria
            var criteria = new Criteria
            {
                MappedProducts = products,
                Days2Fetch = days_loop,
                Months2Fetch = Monthsfromdays,
                Counter = (int)Math.Ceiling((double)(months * 30) / days_loop),
                SameDay = false,
                Token = token,
                Language = "en"
            };

            return criteria;
        }

        private Criteria CreateTiqetsCriteria(List<IsangoHBProductMapping> products, string token)
        {
            token = $"{token?.ToLower()}-dumping";
            var days_loop = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days4LoopTiqets));
            var days = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days2FetchForTiqetsHeavyData));
            var months = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Months2FetchForHeavyData));
            var counter = (int)Math.Ceiling((double)days / days_loop);
            //Set criteria
            var criteria = new Criteria
            {
                MappedProducts = products,
                Days2Fetch = days_loop > 0 ? days_loop : 30,
                Months2Fetch = months,
                Counter = counter > 0 ? counter : 30,
                SameDay = false,
                Token = token,
            };

            return criteria;
        }

        /// <summary>
        /// Fetch the Isango Service Availabilities from the database
        /// </summary>
        /// <returns></returns>
        private List<ServiceAvailabilityFeed> GetServiceAvailabilityFeeds()
        {
            var result = _serviceAvailabilityPersistence.GetIsangoServiceAvailabilities();
            return result;
        }

        /// <summary>
        /// Map the activities and service details from the service availabilties feeds data
        /// </summary>
        /// <param name="serviceAvailabilityFeeds"></param>
        /// <returns></returns>
        private Tuple<List<Activity>, List<TempHBServiceDetail>> PrepareAvailabilitiesData(List<ServiceAvailabilityFeed> serviceAvailabilityFeeds)
        {
            var activities = new List<Activity>();
            var serviceDetails = new List<TempHBServiceDetail>();

            var groupedServiceAvailabilityFeeds = serviceAvailabilityFeeds.GroupBy(x => x.ActivityId);
            var defaultCapacity = ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity");
            var capacity = Convert.ToInt32(defaultCapacity);

            foreach (var serviceFeedsByActivityId in groupedServiceAvailabilityFeeds)
            {
                var serviceFeeds = serviceFeedsByActivityId.ToList();
                var activity = new Activity
                {
                    Id = serviceFeedsByActivityId.Key.ToString(),
                    ProductOptions = MapProductOptions(serviceFeeds)
                };
                foreach (var serviceFeed in serviceFeeds)
                {
                    var serviceDetail = new TempHBServiceDetail
                    {
                        ProductCode = serviceFeed.ProductCode,
                        Modality = serviceFeed.Modality,
                        AvailableOn = serviceFeed.AvailableOn,
                        Price = serviceFeed.Price,
                        Currency = serviceFeed.Currency,
                        ProductClass = serviceFeed.ProductClass,
                        FactSheetID = serviceFeed.Factsheetid,
                        TicketOfficePrice = serviceFeed.TicketOfficePrice,
                        MinAdult = serviceFeed.MinAdult,
                        ActivityId = serviceFeed.ActivityId,
                        CommissionPercent = serviceFeed.CommissionPercent,
                        SellPrice = serviceFeed.SellPrice,
                        Status = serviceFeed.Status,
                        ServiceOptionID = serviceFeed.Serviceoptionid,
                        StartTime = serviceFeed.StartTime,
                        Variant = serviceFeed.Variant,
                        PassengerTypeId = serviceFeed.PassengerTypeId,
                        UnitType = ((UnitType)serviceFeed.UnitType).ToString(),
                        Capacity = capacity
                    };
                    serviceDetails.Add(serviceDetail);
                }
                activities.Add(activity);
            }

            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(activities, serviceDetails);
        }

        private List<ProductOption> MapProductOptions(List<ServiceAvailabilityFeed> serviceAvailabilityFeeds)
        {
            var productOptions = new List<ProductOption>();
            var groupedServiceFeeds = serviceAvailabilityFeeds.GroupBy(x => x.Serviceoptionid).ToList();
            foreach (var serviceFeedsByOption in groupedServiceFeeds)
            {
                var serviceFeeds = serviceFeedsByOption.ToList();
                var productOption = new ProductOption
                {
                    Id = serviceFeedsByOption.Key,
                    //Margin =
                };
                foreach (var serviceFeed in serviceFeeds)
                {
                    var result = MapDatePriceAndAvailabilities(serviceFeeds);
                    var costPriceAndAvailabilities = result.Item1;
                    var basePriceAndAvailabilities = result.Item2;

                    productOption.BasePrice = new Price
                    {
                        Amount = basePriceAndAvailabilities.Select(x => x.Value.TotalPrice).FirstOrDefault(),
                        Currency = new Currency
                        {
                            IsoCode = serviceFeed.Currency
                        },
                        DatePriceAndAvailabilty = basePriceAndAvailabilities
                    };
                    productOption.CostPrice = new Price
                    {
                        Amount = costPriceAndAvailabilities.Select(x => x.Value.TotalPrice).FirstOrDefault(),
                        Currency = new Currency
                        {
                            IsoCode = serviceFeed.Currency
                        },
                        DatePriceAndAvailabilty = costPriceAndAvailabilities
                    };
                    productOption.GateBasePrice = productOption.BasePrice.DeepCopy();
                }
                productOptions.Add(productOption);
            }
            return productOptions;
        }

        private Tuple<Dictionary<DateTime, PriceAndAvailability>, Dictionary<DateTime, PriceAndAvailability>> MapDatePriceAndAvailabilities(List<ServiceAvailabilityFeed> serviceAvailabilityFeeds)
        {
            var costPriceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();
            var basePriceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();
            var groupedServiceFeeds = serviceAvailabilityFeeds.GroupBy(x => x.AvailableOn);
            foreach (var serviceFeedsByDate in groupedServiceFeeds)
            {
                var serviceFeeds = serviceFeedsByDate.ToList();
                var basePriceUnits = new List<PricingUnit>();
                var costPriceUnits = new List<PricingUnit>();
                foreach (var serviceFeed in serviceFeeds)
                {
                    // Get PriceUnit by PassengerType
                    var passengerType = (PassengerType)serviceFeed.PassengerTypeId;
                    var priceUnit = PricingUnitFactory.GetPricingUnit(passengerType);

                    // Adding priceUnit to costPriceUnit and updating cost price
                    var costPriceUnit = priceUnit;
                    costPriceUnit.Price = serviceFeed.Price;

                    // Adding priceUnit to basePriceUnit and updating base price
                    var basePriceUnit = priceUnit;
                    basePriceUnit.Price = serviceFeed.Price;

                    costPriceUnits.Add(costPriceUnit);
                    basePriceUnits.Add(basePriceUnit);
                }
                var costPriceAndAvailability = new DefaultPriceAndAvailability
                {
                    PricingUnits = costPriceUnits,
                    TotalPrice = costPriceUnits.Sum(x => x.Price),
                    //TODO: Set Availability Status from pricing units
                    // AvailabilityStatus =
                };
                var basePriceAndAvailability = new DefaultPriceAndAvailability
                {
                    PricingUnits = basePriceUnits,
                    TotalPrice = basePriceUnits.Sum(x => x.Price),
                    //TODO: Set Availability Status from pricing units
                    // AvailabilityStatus =
                };
                costPriceAndAvailabilities.Add(serviceFeedsByDate.Key, costPriceAndAvailability);
                basePriceAndAvailabilities.Add(serviceFeedsByDate.Key, basePriceAndAvailability);
            }
            return new Tuple<Dictionary<DateTime, PriceAndAvailability>, Dictionary<DateTime, PriceAndAvailability>>(costPriceAndAvailabilities, basePriceAndAvailabilities);
        }

        /// <summary>
        /// Save Service Level Cancellation Policy
        /// </summary>
        /// <param name="token"></param>
        private List<GoogleCancellationPolicy> GetServiceCancellationPolicy(string token)
        {
            var serviceCancellationPolicies = _masterPersistence.GetServiceCancellationPolicies();
            var cancellationPoliciesForGoogle = new List<GoogleCancellationPolicy>();
            var cancellationPoliciesByServiceId = serviceCancellationPolicies.GroupBy(x => x.ServiceId);
            foreach (var cancellationPolicies in cancellationPoliciesByServiceId)
            {
                var cancellationPrices = new List<GoogleCancellationPrice>();
                foreach (var cancellationPolicy in cancellationPolicies)
                {
                    var cancellationPrice = new GoogleCancellationPrice
                    {
                        CutoffHours = (cancellationPolicy.CutOffDays * 24).ToString(),
                        CancellationCharge = cancellationPolicy.CancellationAmount,
                        IsPercentage = !cancellationPolicy.IsFixed
                    };
                    cancellationPrices.Add(cancellationPrice);
                }
                var cancellationPolicyForGoogle = new GoogleCancellationPolicy
                {
                    ActivityId = cancellationPolicies.Key,
                    CancellationPrices = cancellationPrices
                };
                cancellationPoliciesForGoogle.Add(cancellationPolicyForGoogle);
            }
            return cancellationPoliciesForGoogle;
        }

        /// <summary>
        /// Save Rezdy Availabilities
        /// </summary>
        public void SaveRezdyAvailabilities()
        {
            try
            {
                //Get Rezdy service details
                var serviceDetails = GetRezdyServiceDetails();

                //Save details in the database
                _serviceAvailabilityPersistence.SaveServiceAvailabilities(serviceDetails.Item2);
            }
            catch (Exception ex)
            {
                _log.Error("ServiceAvailabilityService|SaveRezdyAvailabilities", ex);
                throw;
            }
        }

        /// <summary>
        /// Get Rezdy Service Details
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetRezdyServiceDetails()
        {
            //Get Rezdy products
            var products = _masterPersistence.LoadLiveHBOptions()
                .Where(x => x.ApiType.Equals(APIType.Rezdy)).Distinct().ToList();
            //products = products.Where(x => x.IsangoHotelBedsActivityId == 39615).ToList();
            if (products?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            var criteria = CreateCriteriaLightData(products, "Rezdy");
            var availabilities = _rezdyCriteriaService.GetAvailability(criteria);
            if (availabilities != null && availabilities?.Count > 0)
            {
                foreach (var itemAvailabilities in availabilities)
                {
                    //remove "seatsAvailable": 0,
                    //seatsAvailable: This is the number of seats available for that specific session.
                    itemAvailabilities?.ProductOptions?.RemoveAll(y => ((ActivityOption)y).SeatsAvailable <= 0);
                }
                serviceDetails = _rezdyCriteriaService.GetServiceDetails(availabilities, products);
            }
            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }

        /// <summary>
        /// Get GlobalTix service details
        /// </summary>
        /// <param name="globalTixProducts"></param>
        /// <returns></returns>
        private Tuple<List<Activity>, List<TempHBServiceDetail>> GetGlobalTixServiceDetails(List<IsangoHBProductMapping> globalTixProducts)
        {
            if (globalTixProducts?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            //Create criteria
            var criteria = CreateCriteria(globalTixProducts, "GlobalTix");

            //Get GlobalTix availabilities
            var availabilities = _globalTixCriteriaService.GetAvailability(criteria);
            if (availabilities?.Count > 0)
            {
                //Get service details
                serviceDetails = _globalTixCriteriaService.GetServiceDetails(availabilities, globalTixProducts);
            }

            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }
        private Tuple<List<Activity>, List<TempHBServiceDetail>> GetGlobalTixV3ServiceDetails(List<IsangoHBProductMapping> globalTixProducts)
        {
            if (globalTixProducts?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            //Create criteria
            var criteria = CreateCriteria(globalTixProducts, "GlobalTixV3");
            criteria.ApiType = APIType.GlobalTixV3;
            //Get GlobalTix availabilities
            var availabilities = _canocalizationService.GetAvailability(criteria);
            if (availabilities?.Count > 0)
            {
                //Get service details
                serviceDetails = _canocalizationService.GetServiceDetails(availabilities, globalTixProducts, PriceDataType.CostAndSell, APIType.GlobalTixV3);
            }

            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }
        /// <summary>
        /// Get Rezdy Service Details
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetNewCitySightSeeingServiceDetails()
        {
            //Get NewCitySightSeeing products
            var products = _masterPersistence.LoadLiveHBOptions()
                .Where(x => x.ApiType.Equals(APIType.NewCitySightSeeing)).Distinct().ToList();

            //products = products.Where(x => x.IsangoHotelBedsActivityId == 26815 || x.IsangoHotelBedsActivityId== 26816).ToList();

            if (products?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            var criteria = CreateCriteria(products, "NewCitySightSeeing");
            var availabilities = _newCitySightSeeingCriteriaService.GetAvailability(criteria);
            if (availabilities?.Count > 0)
                serviceDetails = _newCitySightSeeingCriteriaService.GetServiceDetails(availabilities, products);
            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }

        private List<Activity> GetPrioHubAvailabilities(List<IsangoHBProductMapping> prioProducts)
        {
            if (prioProducts?.Count <= 0) return null;
            //Create criteria
            var criteria = CreateCriteria(prioProducts, "PrioHub");

            //Get PrioHub availabilities
            var availabilities = _prioHubCriteriaService.GetAvailability(criteria);
            return availabilities;
        }
        public Tuple<List<Activity>, List<TempHBServiceDetail>> GetRaynaServiceDetails()
        {
            //Get Rayna products
            var products = _masterPersistence.LoadLiveHBOptions()
                .Where(x => x.ApiType.Equals(APIType.Rayna)).Distinct().ToList();

            //products = products.ToList().Skip(1).Take(1).ToList();
            //foreach (var test in products)
            //{
            //    test.HotelBedsActivityCode = "65";
            //    test.PrefixServiceCode = "261";
            //}

            //var idList = new List<int> { 38516,29595  };
            //var idList = new List<int> { 29595 };
            //products = products.Where(t => idList.Contains(t.IsangoHotelBedsActivityId)).ToList();

            if (products?.Count <= 0) return null;

            var serviceDetails = new List<TempHBServiceDetail>();

            var criteria = CreateCriteriaLightData(products, "Rayna");
            var availabilities = _raynaCriteriaService.GetAvailability(criteria);
            if (availabilities?.Count > 0)
                serviceDetails = _raynaCriteriaService.GetServiceDetails(availabilities, products);
            return new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilities, serviceDetails);
        }
        //private void SaveAvailabilitiesData(Tuple<List<Activity>, List<TempHBServiceDetail>> availabilityData, APIType apiType)
        //{
        //    if (availabilityData == null)
        //        return;
        //    var activities = availabilityData.Item1;
        //    var serviceDetails = availabilityData.Item2;
        //    var token = $"DataDumping";
        //    var watch = Stopwatch.StartNew();

        //    if (apiType != APIType.Undefined)
        //    {
        //        // Save the ServiceDetails in the DB
        //        _serviceAvailabilityService.SaveServiceAvailabilitiesInDatabase(serviceDetails);
        //        watch.Stop();
        //        _log.WriteTimer($"SaveServiceAvailabilitiesInDatabase_serviceDetails:{serviceDetails.Count}", token, apiType.ToString(), watch.Elapsed.ToString());

        //        watch.Restart();
        //        // Sync the Price and Availabilities databases
        //        _serviceAvailabilityService.SyncPriceAvailabilities();
        //        watch.Stop();
        //        _log.WriteTimer("SyncPriceAvailabilities", token, apiType.ToString(), watch.Elapsed.ToString());
        //    }

        //    watch.Restart();
        //    // Apply PRE on the activities
        //    var processedActivities = ApplyPriceRuleEngine(activities);
        //    watch.Stop();
        //    _log.WriteTimer($"ApplyPriceRuleEngine_activities:{activities.Count}_processedActivities:{processedActivities.Count}", token, apiType.ToString(), watch.Elapsed.ToString());

        //    watch.Restart();
        //    var allProductOptions = new List<ProductOption>();
        //    foreach (var processedActivity in processedActivities)
        //    {
        //        if (processedActivity?.ProductOptions != null)
        //        {
        //            allProductOptions.AddRange(processedActivity.ProductOptions);
        //        }
        //    }
        //    //Save Price and Availabilities in the storage
        //    var storageServiceDetailCount = _googleMapsDataDumpingService.DumpPriceAndAvailabilities(serviceDetails, allProductOptions, apiType);
        //    watch.Stop();
        //    _log.WriteTimer($"GoogleDumpPriceAndAvailabilities_storageServiceDetailCount:{storageServiceDetailCount}", token, apiType.ToString(), watch.Elapsed.ToString());

        //    watch.Restart();
        //    //Save Extra Details in the storage
        //    _googleMapsDataDumpingService.DumpExtraDetail(processedActivities, apiType);
        //    watch.Stop();
        //    _log.WriteTimer("GoogleDumpExtraDetail", token, apiType.ToString(), watch.Elapsed.ToString());
        //}

        public List<string> GetAgeDumpingAPIs()
        {
            var ageDumpingAPIs = _masterPersistence.LoadAgeDumpngAPIs();

            return ageDumpingAPIs;
        }

        #endregion "Private Methods"
    }
}