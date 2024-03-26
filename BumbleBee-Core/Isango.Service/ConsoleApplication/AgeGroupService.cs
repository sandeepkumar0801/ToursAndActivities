using Isango.Entities;
using Isango.Entities.Aot;
using Isango.Entities.Bokun;
using Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor;
using Isango.Entities.ConsoleApplication.RouteMap.Prio;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTix;
using Isango.Entities.GoldenTours;
using Isango.Entities.HotelBeds;
using Isango.Entities.Redeam;
using Isango.Entities.Rezdy;
using Isango.Entities.Tiqets;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Aot;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using ServiceAdapters.Bokun;
using ServiceAdapters.Bokun.Bokun.Entities.GetActivity;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.GlobalTix;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using ServiceAdapters.GoldenTours;
using ServiceAdapters.GrayLineIceLand;
using ServiceAdapters.HB;
using ServiceAdapters.HB.HB.Entities.Calendar;
using ServiceAdapters.HB.HB.Entities.ContentMulti;
using ServiceAdapters.PrioTicket;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using ServiceAdapters.Redeam;
using ServiceAdapters.Rezdy;
using ServiceAdapters.Tiqets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Constant = Isango.Service.Constants.Constant;

using FareHarbor = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor;

using GoldenToursAgeGroup = Isango.Entities.GoldenTours.AgeGroup;
using GrayLineIceLand = Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;
using MethodType = ServiceAdapters.GlobalTix.GlobalTix.Entities.MethodType;
using Prio = Isango.Entities.ConsoleApplication.AgeGroup.Prio;
using VentrataBaseEntities = ServiceAdapters.Ventrata;
using VentrataEntities = ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse;
using ServiceAdapters.TourCMS;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelResponse;
using ServiceAdapters.NewCitySightSeeing;
using ServiceAdapters.GoCity;
using ServiceAdapters.Tiqets.Tiqets.Entities;
using System.Runtime.Caching;
using ServiceAdapters.Rayna;
using ServiceAdapters.PrioHub;
using Isango.Entities.PrioHub;
using System.Collections;
using ServiceAdapters.PrioHub.PrioHub.Entities.RouteResponse;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using ServiceAdapters.RedeamV12;
using Isango.Service.Canocalization;
using ServiceAdapters.GlobalTixV3;
using Isango.Entities.GlobalTixV3;
using CountryCity = Isango.Entities.GlobalTixV3.CountryCity;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using Isango.Entities.TourCMSCriteria;
using Isango.Entities.TourCMS;
using Newtonsoft.Json;
using ServiceAdapters.TourCMS.TourCMS.Entities.Redemption;

namespace Isango.Service.ConsoleApplication
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class AgeGroupService : IAgeGroupService
    {
        private readonly ILogger _log;
        private readonly IAOTPersistence _aotPersistence;
        private readonly IGrayLineIceLandPersistence _grayLineIceLandPersistence;
        private readonly IFareHarborPersistence _fareHarborPersistence;
        private readonly IPrioPersistence _prioPersistence;
        private readonly ITiqetsPersistence _tiqetsPersistence;
        private readonly IGoldenToursPersistence _goldenToursPersistence;
        private readonly IVentrataPersistence _ventrataPersistence;
        private readonly IRedeamPersistence _redeamPersistence;
        private readonly IMasterPersistence _masterPersistence;
        private readonly IPrioTicketAdapter _prioTicketAdapter;
        private readonly IGrayLineIceLandAdapter _grayLineIceLandAdapter;
        private readonly IAotAdapter _aotAdapter;
        private readonly IFareHarborAdapter _fareHarborAdapter;
        private readonly ITiqetsAdapter _tiqetsAdapter;
        private readonly IGoldenToursAdapter _goldenToursAdapter;
        private readonly VentrataBaseEntities.IVentrataAdapter _ventrataAdapter;
        private readonly IHBAdapter _hbAdapter;
        private readonly IBokunAdapter _bokunAdapter;
        private readonly IApiTudePersistence _apiTudePersistence;
        private readonly IActivityTicketTypeCommandHandler _activityTicketTypeCommandHandler;
        private readonly int _contentRecords;
        private readonly int _calendarRecords;
        private readonly int _daystoFecth;
        private readonly IRedeamAdapter _redeamAdapter;
        private readonly IBokunPersistence _bokunPersistence;
        private readonly IRezdyAdapter _rezdyAdapter;
        private readonly IRezdyPersistence _rezdyPersistence;
        private readonly string _className;
        private readonly IGlobalTixPersistence _globalTixPersistence;
        private readonly IGlobalTixAdapter _globalTixAdapter;
        private readonly IGlobalTixAdapterV3 _globalTixAdapterV3;
        private static readonly int _maxParallelThreadCount;
        private readonly ITourCMSAdapter _tourCMSAdapter;
        private readonly ITourCMSPersistence _tourCMSPersistence;
        private readonly INewCitySightSeeingAdapter _newCitySightSeeingAdapter;
        private readonly INewCitySightSeeingPersistence _newCitySightSeeingPersistence;
        private readonly IGoCityAdapter _goCityAdapter;
        private readonly IGoCityPersistence _goCityPersistence;
        private readonly IPrioHubAdapter _prioHubAdapter;
        private readonly INewPrioPersistence _newPrioPersistence;
        private readonly IRaynaPersistence _raynaPersistence;
        private readonly ICitySightSeeingAdapter _citySightSeeingAdapter;
        private readonly IRedemptionService _redemptionService;


        private readonly IRaynaAdapter _raynaAdapter;
        private readonly IRedeamV12Adapter _redeamV12Adapter;


        private readonly ICanocalizationService _icanocalizationService;
        private readonly ICssBookingService _cssbookingservice;
        private readonly int _tietscheckindays;
        private readonly int _tiqetscheckoutdays;


        static AgeGroupService()
        {
            try
            {
                _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Constant.MaxParallelThreadCount);
            }
            catch (Exception)
            {
                _maxParallelThreadCount = 1;
            }
        }

        public AgeGroupService(IAOTPersistence aotPersistence
            , IGrayLineIceLandPersistence grayLineIceLandPersistence
            , IFareHarborPersistence fareHarborPersistence
            , IPrioPersistence prioPersistence
            , ITiqetsPersistence tiqetsPersistence
            , IMasterPersistence masterPersistence
            , IPrioTicketAdapter prioTicketAdapter
            , IGrayLineIceLandAdapter grayLineIceLandAdapter
            , IAotAdapter aotAdapter
            , IFareHarborAdapter fareHarborAdapter
            , ITiqetsAdapter tiqetsAdapter
            , IGoldenToursAdapter goldenToursAdapter
            , IBokunAdapter bokunAdapter
            , IGoldenToursPersistence goldenToursPersistence
            , IHBAdapter hbAdapter
            , IApiTudePersistence apiTudePersistence
            , IRedeamAdapter redeamAdapter
            , IRedeamPersistence redeamPersistence
            , ILogger logger
            , IBokunPersistence bokunPersistence
            , IRezdyAdapter rezdyAdapter
            , IRezdyPersistence rezdyPersistence
            , IGlobalTixAdapter globalTixAdapter
            , IGlobalTixAdapterV3 globalTixAdapterV3
            , IGlobalTixPersistence globalTixPersistence
            , IActivityTicketTypeCommandHandler activityTicketTypeCommandHandler
            , VentrataBaseEntities.IVentrataAdapter ventrataAdapter
            , IVentrataPersistence ventrataPersistence
            , ITourCMSAdapter tourCMSAdapter
            , ITourCMSPersistence tourCMSPersistence
            , INewCitySightSeeingAdapter newCitySightSeeingAdapter
            , INewCitySightSeeingPersistence newCitySightSeeingPersistence
            , IGoCityAdapter goCityAdapter
            , IGoCityPersistence goCityPersistence
            , IPrioHubAdapter prioHubAdapter
            , INewPrioPersistence newPrioPersistence
            , IRaynaAdapter raynaAdapter
            , IRaynaPersistence raynaPersistence
            , IRedeamV12Adapter redeamV12Adapter
            , ICanocalizationService icanocalizationService
            , ICitySightSeeingAdapter citySightSeeingAdapter
            , ICssBookingService cssbookingservice
            , IRedemptionService redemptionService
            )
        {
            _aotPersistence = aotPersistence;
            _grayLineIceLandPersistence = grayLineIceLandPersistence;
            _fareHarborPersistence = fareHarborPersistence;
            _prioPersistence = prioPersistence;
            _tiqetsPersistence = tiqetsPersistence;
            _goldenToursPersistence = goldenToursPersistence;
            _redeamPersistence = redeamPersistence;
            _masterPersistence = masterPersistence;
            _prioTicketAdapter = prioTicketAdapter;
            _grayLineIceLandAdapter = grayLineIceLandAdapter;
            _aotAdapter = aotAdapter;
            _fareHarborAdapter = fareHarborAdapter;
            _tiqetsAdapter = tiqetsAdapter;
            _goldenToursAdapter = goldenToursAdapter;
            _bokunAdapter = bokunAdapter;
            _hbAdapter = hbAdapter;
            _apiTudePersistence = apiTudePersistence;
            _redeamAdapter = redeamAdapter;
            _log = logger;
            _contentRecords = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.APiTudeContentRecordsAtTimeData));
            _calendarRecords = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.APiTudeCalendarRecordsAtTimeData));
            _daystoFecth = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days2FetchForApiTudeData));
            _bokunPersistence = bokunPersistence;
            _rezdyAdapter = rezdyAdapter;
            _rezdyPersistence = rezdyPersistence;
            _globalTixPersistence = globalTixPersistence;
            _globalTixAdapter = globalTixAdapter;
            _globalTixAdapterV3 = globalTixAdapterV3;
            _activityTicketTypeCommandHandler = activityTicketTypeCommandHandler;
            _ventrataAdapter = ventrataAdapter;
            _ventrataPersistence = ventrataPersistence;
            _tourCMSAdapter = tourCMSAdapter;
            _tourCMSPersistence = tourCMSPersistence;

            _newCitySightSeeingAdapter = newCitySightSeeingAdapter;
            _newCitySightSeeingPersistence = newCitySightSeeingPersistence;

            _goCityAdapter = goCityAdapter;
            _goCityPersistence = goCityPersistence;
            _prioHubAdapter = prioHubAdapter;
            _newPrioPersistence = newPrioPersistence;
            _raynaAdapter = raynaAdapter;
            _raynaPersistence = raynaPersistence;
            _citySightSeeingAdapter = citySightSeeingAdapter;
            _cssbookingservice = cssbookingservice;
            _redemptionService = redemptionService;
            _redeamV12Adapter = redeamV12Adapter;
            _icanocalizationService = icanocalizationService;
            _tietscheckindays = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCheckinAddDays));
            _tiqetscheckoutdays = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCheckOutAddDays));
        }

        /// <summary>
        /// Get and save all AOT Age Group Data in the database
        /// </summary>
        /// <param name="token"></param>
        public void SaveAOTAgeGroups(string token)
        {
            try
            {
                //Get ALL AOT Tours
                var aotProducts = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Aot)).Distinct().ToList();

                //Save all Activity Age Groups Mapping
                SaveAllOptionsGeneralInformation(aotProducts, token);

                //Sync data between databases
                _aotPersistence.SyncDataBetweenIsangoDataBases();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveAOTAgeGroupData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get and save all GLI age group data in the database
        /// </summary>
        /// <param name="token"></param>
        public async Task SaveGrayLineIceLandAgeGroups(string token)
        {
            try
            {
                var mappings = _masterPersistence.LoadLiveHBOptions();
                //Get ALL GrayLineIceLand Tours
                var grayLineProducts = mappings?.Where(x => x.ApiType.Equals(APIType.Graylineiceland)).Distinct().ToList();

                //Get Age Groups by tours
                var ageGroups = await _grayLineIceLandAdapter.GetAgeGroupsByToursAsync(grayLineProducts, token);

                //Save all Activity Age Groups Mapping
                SaveAllGrayLineIceLandAgeGroups(ageGroups);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveGrayLineIceLandAgeGroupData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Get and save all GLI pickup location data in the database
        /// </summary>
        /// <param name="token"></param>
        public async Task SaveGrayLineIceLandPickupLocations(string token)
        {
            try
            {
                var mappings = _masterPersistence.LoadLiveHBOptions();
                //Get ALL GrayLineIceLand Tours
                var grayLineProducts = mappings?.Where(x => x.ApiType.Equals(APIType.Graylineiceland)).Distinct().ToList();

                //Get all pickup locations
                var pickupLocations = await _grayLineIceLandAdapter.GetAllPickupLocationsAsync(grayLineProducts, token);

                //Save all Pickup Locations
                SaveAllGrayLineIceLandPickupLocations(pickupLocations);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveGrayLineIceLandPickupLocationData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Sync GLI data between databases
        /// </summary>
        public void SyncGrayLineIceLandData()
        {
            try
            {
                _grayLineIceLandPersistence.SyncDataBetweenDataBases();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SyncGrayLineIceLandData"
                };
                _log.Error(isangoErrorEntity, ex);
                // throw;
            }
        }

        /// <summary>
        /// Save FHB company data
        /// </summary>
        /// <param name="token"></param>
        public void SaveFareHarborCompanies(string token)
        {
            try
            {
                //Get all user keys
                var userKeys = _fareHarborPersistence.GetUserKeys();

                //Get all companies
                var companies = GetAllFareHarborCompanies(userKeys, token);

                //Get all company mappings
                var companyMappings = GetAllFareHarborCompanyMappings(companies, token);

                //Save all companies and company mappings
                _fareHarborPersistence.SaveAllCompanies(companies);
                _fareHarborPersistence.SaveAllCompanyMappings(companyMappings);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveFareHarborCompanies",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        public void SaveExternalProducts(string token)
        {
            try
            {
                //Get all External Mapping
                var products = _citySightSeeingAdapter.GetProductExtrnalMappings(token);

                _masterPersistence.SaveAllCssExternalProducts(products);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveCssExternalProduct",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }


        public CssBookingResponseResult CreateBooking(string token, CreateBookingRequest bookingRequest)
        {
            try
            {
                // Get all External Mapping
                var booking = _citySightSeeingAdapter.CssBookingResult(token, bookingRequest);
                var result = SerializeDeSerializeHelper.DeSerialize<CssBookingResponseResult>(booking);

                return result;

                // _masterPersistence.SaveAllCssExternalProducts(products);
            }
            catch (Exception ex)
            {
                // Log the error and create an IsangoErrorEntity
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveCssExternalProduct",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                return new CssBookingResponseResult(); // Example of returning a default response
                // throw; // Commented out, so the exception is not re-thrown
            }
        }

        public object CancelBooking(string token, CancellationRequest cancellationRequest)
        {
            try
            {
                // Get all External Mapping
                var cancelbooking = _citySightSeeingAdapter.CancellationResult(token, cancellationRequest);

                return cancelbooking;

                // _masterPersistence.SaveAllCssExternalProducts(products);
            }
            catch (Exception ex)
            {
                // Log the error and create an IsangoErrorEntity
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveCssExternalProduct",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                return null; // Return null instead of CssBookingResponseResult
            }
        }



        /// <summary>
        /// Save FHB customer prototypes
        /// </summary>
        /// <param name="token"></param>
        public void SaveFareHarborCustomerProtoTypes(string token)
        {
            try
            {
                var products = _fareHarborPersistence.LoadProducts();

                var customerPrototypes = GetAllCustomerPrototypesFromFareHarbor(products, token);

                _fareHarborPersistence.SaveAllCustomerProtoTypes(customerPrototypes);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveFareHarborCustomerProtoTypes",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Sync FHB data between databases
        /// </summary>
        public void SyncFareHarborData()
        {
            try
            {
                _fareHarborPersistence.SyncDataBetweenDataBases();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SyncFareHarborData"
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Save Prio Ticket Details
        /// </summary>
        /// <param name="token"></param>
        public void SavePrioTicketDetails(string token)
        {
            try
            {
                SavePrioTicketList(token);
                var mappings = _masterPersistence.LoadLiveHBOptions();
                //Get prio product Ids
                var prioTicketProducts = mappings?.Where(x => x.ApiType.Equals(APIType.Prio)).OrderBy(y => y.HotelBedsActivityCode).Distinct().ToList();

                if (prioTicketProducts?.Count > 0)
                {
                    var prioRouteMaps = new List<RouteMap>();
                    var ageGroupPrio = new List<Prio.AgeGroup>();
                    var prioProductDetails = new List<Prio.ProductDetails>();
                    foreach (var product in prioTicketProducts)
                    {
                        try
                        {
                            var ticketId = product.HotelBedsActivityCode;
                            //Get ticket details by ticket id
                            var ticketDetails = _prioTicketAdapter.GetPrioTicketDetails(ticketId, token);

                            if (ticketDetails != null)
                            {
                                var productDetails = ProcessPrioProductDetails(ticketDetails, ticketId);
                                var ageGroups = ProcessPrioDetails(ticketDetails, ticketId);
                                ageGroupPrio.AddRange(ageGroups);
                                prioRouteMaps.AddRange(ProcessPrioRouteMaps(ticketDetails));
                                prioProductDetails.Add(productDetails);
                            }
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                                _log.Error(new IsangoErrorEntity
                                {
                                    ClassName = _className,
                                    MethodName = "SavePrioTicketDetails",
                                    Token = token,
                                    Params = Util.SerializeDeSerializeHelper.Serialize(product)
                                }, ex)
                            );
                        }
                    }
                    //Save Age Groups in database
                    _prioPersistence.SaveAllAgeGroups(ageGroupPrio);
                    _prioPersistence.SavePrioRouteMaps(prioRouteMaps);
                    _prioPersistence.SavePrioProductDetails(prioProductDetails);
                    //Sync data in the database
                    _prioPersistence.SyncDataBetweenDataBases();
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SavePrioTicketDetails",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                // throw;
            }
        }

        /// <summary>
        /// Get and save all Tiqets Variants in the database
        /// </summary>
        /// <param name="token"></param>
        public void SaveTiqetsVariants(string token)
        {
            try
            {
                //Get all Tiqets product details
                //var products = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Tiqets)).Distinct().ToList();
                _tiqetsPersistence.SyncDataInVariantTemp();

                //Get All variants
                var productsWithVariantsAndDetails = GetAllTiqetsVariants(token);

                //Save All Variants
                _tiqetsPersistence.SaveAllVariants(productsWithVariantsAndDetails.Item2);
                _tiqetsPersistence.SaveAllDetails(productsWithVariantsAndDetails.Item1);
                _tiqetsPersistence.SaveMediaImages(productsWithVariantsAndDetails.Item3);
                _tiqetsPersistence.SaveTiqetsPackage(productsWithVariantsAndDetails.Item4);


            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveTiqetsVariants",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Get and save all Tiqets Variants in the database
        /// </summary>
        /// <param name="token"></param>
        public void SaveTourCMS(string token)
        {
            try
            {
                //Get all Tiqets product details
                var products = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.TourCMS)).Distinct().ToList();

                //Get All variants
                //var productsWithVariantsAndDetails = GetAllTiqetsVariants(products, token);

                //Save All Variants
                //_tiqetsPersistence.SaveAllVariants(productsWithVariantsAndDetails.Item2);
                //_tiqetsPersistence.SaveAllDetails(productsWithVariantsAndDetails.Item1);
                //_tiqetsPersistence.SaveMediaImages(productsWithVariantsAndDetails.Item3);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveTourCMS",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        /// <summary>
        /// Save Golden Tours Age Groups
        /// </summary>
        /// <param name="token"></param>
        public void SaveGoldenToursAgeGroups(string token)
        {
            try
            {
                // Fetch all GoldenTours products
                var products = _masterPersistence.LoadLiveHBOptions()?.Where(x => x.ApiType.Equals(APIType.Goldentours)).Distinct().ToList();

                // Get all supported passenger types from the supplier
                var result = GetGoldenToursAgeGroups(products, token);

                // Save the product details in the database
                _goldenToursPersistence.SaveGoldenToursProductDetails(result.ProductDetails);

                // Save the passenger types in the database
                _goldenToursPersistence.SaveGoldenToursAgeGroups(result.AgeGroups);

                // Save the price periods in the database
                _goldenToursPersistence.SaveGoldenToursPricePeriods(result.PricePeriods);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveGoldenToursAgeGroups",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        public void SaveVentrataProductDetails(string token)
        {
            try
            {
                // Fetch all Ventrata products
                var supplierDetails = GetVentrataData();
                //golden tour test condition
                //supplierDetails = supplierDetails.Where(x => x.SupplierBearerToken == "45ba71e5-9811-44cc-8628-92b5723fcc54").ToList();
                if (supplierDetails != null && supplierDetails.Count > 0)
                {
                    var ageGroup = GetVentrataAgeGroups(supplierDetails, token);

                    // Save the product details in the database
                    if (ageGroup?.ProductDetails != null && ageGroup?.ProductDetails.Count > 0)
                    {
                        _ventrataPersistence.SaveProductDetails(ageGroup.ProductDetails);
                    }
                    if (ageGroup?.Destinations != null && ageGroup?.Destinations.Count > 0)
                    {
                        _ventrataPersistence.SaveDestinationDetails(ageGroup.Destinations);
                    }
                    if (ageGroup?.Faqs != null && ageGroup?.Faqs.Count > 0)
                    {
                        _ventrataPersistence.SaveFaqs(ageGroup.Faqs);
                    }
                    if (ageGroup?.Options != null && ageGroup?.Options.Count > 0)
                    {
                        _ventrataPersistence.SaveOptionDetails(ageGroup.Options);
                    }
                    if (ageGroup?.UnitDetailsForoptions != null && ageGroup?.UnitDetailsForoptions.Count > 0)
                    {
                        _ventrataPersistence.SaveUnitsDetailsOfOption(ageGroup.UnitDetailsForoptions);
                    }
                    if (ageGroup?.PackageInclude != null && ageGroup?.PackageInclude.Count > 0)
                    {
                        _ventrataPersistence.SavePackagesInclude(ageGroup.PackageInclude);
                    }

                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveVentrataProductsList",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
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
                _log.Error("AgegroupService|GetVentrataData", ex);
                throw;
            }
        }
        public void SaveBokunAgeGroups(string token)
        {
            try
            {
                // Fetch all Bokun products
                var productMappings = _masterPersistence.LoadLiveHBOptions()?
                    .Where(x =>
                            x.ApiType.Equals(APIType.Bokun)
                    //&& (x.IsangoHotelBedsActivityId == 38817
                    //    || x.IsangoHotelBedsActivityId == 32622
                    //|| x.IsangoHotelBedsActivityId == 32623
                    //)
                    ).Distinct().ToList();

                // Get all Activity from the supplier call
                var result = GetBokunAgeGroups(productMappings, token);

                // Save the product details in the database
                _bokunPersistence.SaveBokunProductDetails(result.Products);

                // Save the CancellationPolicies in the database
                _bokunPersistence.SaveBokunCancellationPolicies(result.CancellationPolicies);

                // Save the Rates in the database
                _bokunPersistence.SaveBokunRates(result.Rates);

                if (result.BookableExtras?.Count > 0)
                {
                    // Save the Bookable Extras in the database
                    _bokunPersistence.SaveBookableExtras(result.BookableExtras);
                }

                // Call Sync Procedure Bokun API data Syncing in Isango
                _bokunPersistence.BokunSyncCall();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveBokunAgeGroups",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                // throw;
            }
        }

        /// <summary>
        /// Save APITude
        /// </summary>
        /// <param name="token"></param>
        public void SaveAPITudeContentData(string token)
        {
            try
            {
                // Fetch all APITude products
                var products = _masterPersistence.LoadLiveHBOptionsApiTudeContent()
                    ?.Where(x => x.ApiType.Equals(APIType.Hotelbeds)
                      && x.SupplierCode != null && x.SupplierCode != string.Empty)
                    //&& x.SupplierCode == "E-U10-LUXSOUTOUR"
                    //|| x.SupplierCode == "E-U10-EMPIREOBS")
                    //&& x.IsangoHotelBedsActivityId == 4402) //UnComment this line after Testing
                    //&& x.IsangoHotelBedsActivityId == 26311)  //UnComment this line after Testing
                    //.Distinct().ToList().OrderBy(x => x.IsangoHotelBedsActivityId).ToList();
                    .Distinct().ToList().OrderBy(x => x.FactSheetId).ToList();

                //.Take(100).ToList(); // UnComment this line after Testing

                // Get all apitude content data
                //Comment for now, We will Save content now from Calendar Call
                //GetAPITudeDataAndSave(products, token);
                GetAPITudeCalendarDataAndSave(products, token);
            }
            //catch (Exception ex)
            //{
            //    _log.Error("AgeGroupService|SaveAPITudeData", ex);
            //    throw;
            //}
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveAPITudeData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Redeam Supplier, Products, Rates, Price and AgeGroup Data
        /// </summary>
        /// <param name="token"></param>
        public void SaveRedeamData(string token)
        {
            try
            {
                // Fetch the suppliers data and save it in the database
                var supplierIds = SaveRedeamSupplierData(token);
                if (supplierIds?.Count == 0) return;

                // Fetch the products data for the given supplier Ids and save it in the database
                var products = SaveRedeamProductsData(supplierIds, token);
                if (products == null) return;

                // Fetch the rates, price and age group data and save it in the database
                SaveRedeamRatesData(products, token);
            }
            catch (Exception ex)
            {
                _log.Error("AgeGroupService|SaveRedeamData", ex);
                throw;
            }
        }
        /// <summary>
        /// Save Redeam Supplier, Products, Rates, Price and AgeGroup Data
        /// </summary>
        /// <param name="token"></param>
        public void SaveRedeamV12Data(string token)
        {
            try
            {
                // Fetch the suppliers data and save it in the database
                var supplierIds = SaveRedeamV12SupplierData(token);
                if (supplierIds?.Count == 0) return;

                // Fetch the products data for the given supplier Ids and save it in the database
                var products = SaveRedeamV12ProductsData(supplierIds, token);
                if (products == null) return;

                // Fetch the rates, price and age group data and save it in the database
                SaveRedeamV12RatesData(products, token);
            }
            catch (Exception ex)
            {
                _log.Error("AgeGroupService|SaveRedeamV12Data", ex);
                throw;
            }
        }

        public void SaveGlobalTixCountryCityList(string token)
        {
            List<Entities.GlobalTix.CountryCity> countryCityList = _globalTixAdapter.GetCountryCityList(token);
            _globalTixPersistence.SaveCountryCityList(countryCityList);
        }
        public void SaveGlobalTixCountryCityListV3(string token)
        {
            var countryCityList = _globalTixAdapterV3.GetCountryCityListV3(token);
            if (countryCityList != null && countryCityList.Count > 0)
            {
                _globalTixPersistence.SaveCountryCityListV3(countryCityList);
            }
        }

        public void SaveGlobalTixProductInfoListV3(string token)
        {
            int id = 2368;//Enter Here ID to get the Product Information
            List<Entities.GlobalTixV3.ProductInfoV3> ProductList = _globalTixAdapterV3.GetProductInfoV3(id, token, true);
            _globalTixPersistence.SaveProductInfoListV3(ProductList);
        }

        public void SaveGlobalTixCategoriesV3(string token)
        {
            var categoriesV3List = _globalTixAdapterV3.GetCategoriesListV3(token);
            if (categoriesV3List != null && categoriesV3List.Count > 0)
            {
                _globalTixPersistence.SaveSaveGlobalTixCategoriesListV3(categoriesV3List);
            }
        }

        public void SaveGlobalTixProductChanges(string token)
        { /*int countryid = 1;*/
            var countryCityList = _globalTixPersistence.GetCountryCityV3(10000);
            var uniqueCountryIds = countryCityList.Select(cc => cc.CountryId)?.Distinct()?.ToList();

            foreach (var countryid in uniqueCountryIds)
            {
                List<Entities.GlobalTixV3.ProductChangesV3> ProductChangesV3List = _globalTixAdapterV3.GetProductChangesV3(token, countryid.ToInt(), true);
                if (ProductChangesV3List.Count > 1)
                {
                    _globalTixPersistence.SaveGlobalTixProductChangesV3(ProductChangesV3List);
                }
            }
        }

        public void SaveGlobalTixV3PackageOptions(string token)
        {
            try
            {
                int id = 0;//This is set 0 to dump all PackageOptions first.
                var packageOptionsV3 = _globalTixAdapterV3.GetPackageOptionsV3(token, id);
                _globalTixPersistence.SaveGlobalTixPackageOptionsV3(packageOptionsV3.Item1);
                _globalTixPersistence.SaveGlobalTixPackageTypeIdV3(packageOptionsV3.Item2);
            }
            catch (Exception ex)
            {

            }
        }




        public void SaveGlobalTixActivities(string token)
        {
            List<GlobalTixActivity> activities = new List<GlobalTixActivity>();
            var countryCityList = _globalTixPersistence.GetCountryCityList();

            var countryCityList4 = countryCityList.Where(x => x.CountryId.Equals("4")).ToList();
            var countryCityListOther = countryCityList.Where(x => !x.CountryId.Equals("4")).ToList();

            Parallel.ForEach(countryCityList4, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, countryCity =>
            {
                var countryCityActivities = new List<GlobalTixActivity>();
                countryCityActivities = _globalTixAdapter.GetAllActivities(countryCity.CountryId, countryCity.CityId, token, false);
                if (countryCityActivities != null)
                {
                    activities.AddRange(countryCityActivities);
                }
            });
            Parallel.ForEach(countryCityListOther, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, countryCity =>
            {
                var countryCityActivities = new List<GlobalTixActivity>();
                countryCityActivities = _globalTixAdapter.GetAllActivities(countryCity.CountryId, countryCity.CityId, token, true);
                if (countryCityActivities != null)
                {
                    activities.AddRange(countryCityActivities);
                }
            });

            foreach (var activity in activities)
            {
                foreach (var tktType in activity.TicketTypes)
                {
                    ActivityTicketTypeInputContext inCtx = new ActivityTicketTypeInputContext
                    {
                        MethodType = MethodType.TicketTypeDetail,
                        TicketType = tktType.Id.ToString()
                    };
                    //To change ... Temp code condition
                    var ticketTypeRSRaw = _activityTicketTypeCommandHandler.Execute(inCtx, token);
                    if (ticketTypeRSRaw == null)
                    {
                        continue;
                    }
                    TicketTypeRS ticketTypeRS = SerializeDeSerializeHelper.DeSerialize<TicketTypeRS>(ticketTypeRSRaw.ToString());
                    if (ticketTypeRS == null || ticketTypeRS.IsSuccess == false)
                    {
                        continue;
                    }
                    var ticketTypeRSData = ticketTypeRS.Data;
                    tktType.ToAge = ticketTypeRSData.ToAge;
                    tktType.FromAge = ticketTypeRSData.FromAge;
                    tktType.Currency = ticketTypeRSData.Currency;
                    tktType.MinimumSellingPrice = ticketTypeRSData.MinimumSellingPrice;
                    tktType.OriginalPrice = ticketTypeRSData.OriginalPrice;
                    tktType.PayableAmount = ticketTypeRSData.PayableAmount;
                    tktType.CancellationNotesSetting = new CancellationNotesActLevel();
                    ticketTypeRSData.CancellationNotesSettings.ForEach(thisNote =>
                    {
                        if (thisNote.IsActive && !string.IsNullOrEmpty(thisNote.Value))
                        {
                            tktType.CancellationNotesSetting.Id = thisNote.Id;
                            tktType.CancellationNotesSetting.IsActive = thisNote.IsActive;
                            tktType.CancellationNotesSetting.Value = thisNote.Value;
                        }
                    });
                }
            }

            _globalTixPersistence.SaveAllActivities(activities);
        }


        public async Task SaveGlobalTixActivitiesV3Async(string token)
        {
            try
            {
                var countryCityList = _globalTixPersistence.GetCountryCityV3(10000);
                var gtProductOption = new List<Entities.GlobalTixV3.ProductOptionV3>();
                var productListThailandList = new List<ProductList>();
                var productListOtherList = new List<ProductList>();
                var productList = new List<ProductList>();
                //Get product List
                //Thai Products
                var countryCityList4 = countryCityList?.Where(x => x.CountryId.Equals("4"))?.ToList();
                //Non-Thai Products
                var countryCityListOther = countryCityList?.Where(x => !x.CountryId.Equals("4"))?.ToList();

                var uniqueCountryIds4 = countryCityList4.Select(cc => cc.CountryId)?.Distinct()?.ToList();
                var uniqueCountryIdsOther = countryCityListOther.Select(cc => cc.CountryId)?.Distinct()?.ToList();

                //Thai Products
                var thaiProduct = false;
                var nonthaiProduct = true;
                if (uniqueCountryIds4 != null && uniqueCountryIds4.Count > 0)
                {
                    Parallel.ForEach(uniqueCountryIds4, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, async (countryid) =>
                    {
                        try
                        {
                            var productListData = await _globalTixAdapterV3.GetAllActivitiesV3Async(countryid.ToInt(), token, thaiProduct);
                            if (productListData != null)
                            {
                                lock (productListThailandList) // Use a lock to ensure thread safety when adding to the list
                                {
                                    productListThailandList.AddRange(productListData);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_ProductList|{countryid.ToInt()}", ex);

                        }
                    });


                }
                // Remove duplicates from productList
                var NoDupsproductListThailand = productListThailandList.GroupBy(d => new
                {
                    d.Id,
                    d.Name,
                    d.Currency,
                    d.City,
                    d.OriginalPrice,
                    d.Country,
                    d.IsOpenDated,
                    d.IsOwnContracted,
                    d.IsFavorited,
                    d.IsBestSeller,
                    d.IsInstantConfirmation,
                    d.Category
                })
                                      .Select(d => d.First())
                                      .ToList();
                var productListThailand = productListThailandList.Where(x => x.Country.ToLower() == "thailand")?.ToList();
                Parallel.ForEach(productListThailand, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (productInfo) =>
                {
                    try
                    {
                        var ProductList = _globalTixAdapterV3.GetProductInfoV3(productInfo.Id, token, thaiProduct);
                        if (ProductList.Count >= 1)
                        {
                            _globalTixPersistence.SaveProductInfoListV3(ProductList);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_GetProductInfoV3|{productInfo.Id}", ex);

                    }
                });
                var allTicketTypes = new List<Tickettype>();
                if (NoDupsproductListThailand != null)
                {
                    Parallel.ForEach(NoDupsproductListThailand, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (productOption) =>
                    {
                        try
                        {
                            var productOptionData = _globalTixAdapterV3.GetProductOptionV3(token, productOption.Id, thaiProduct);

                            List<ProductOptionV3> productOptions = productOptionData.Item1; // Extract the first item from the tuple
                            List<Tickettype> ticketTypes = productOptionData.Item2; // Extract the second item from the tuple

                            lock (gtProductOption) // Lock to ensure thread-safe access to gtProductOption list
                            {
                                gtProductOption.AddRange(productOptions);
                            }

                            lock (allTicketTypes) // Lock to ensure thread-safe access to allTicketTypes list
                            {
                                allTicketTypes.AddRange(ticketTypes);
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (_log) // Lock for thread-safe logging
                            {
                                _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_SaveGlobalTixActivitiesV3Async|{productOption.Id}", ex);
                            }

                        }
                    });
                }


                //Non-Thai Products
                if (uniqueCountryIdsOther != null && uniqueCountryIdsOther.Count > 0)
                {
                    Parallel.ForEach(uniqueCountryIdsOther, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, async (countryid) =>
                    {
                        try
                        {
                            var productListData = await _globalTixAdapterV3.GetAllActivitiesV3Async(countryid.ToInt(), token, nonthaiProduct);
                            if (productListData != null)
                            {
                                lock (productListOtherList) // Use a lock to ensure thread safety when adding to the list
                                {
                                    productListOtherList.AddRange(productListData);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_ProductList|{countryid.ToInt()}", ex);

                        }
                    });
                }

                // Remove duplicates from productList
                var NoDupsproductListOther = productListOtherList.GroupBy(d => new
                {
                    d.Id,
                    d.Name,
                    d.Currency,
                    d.City,
                    d.OriginalPrice,
                    d.Country,
                    d.IsOpenDated,
                    d.IsOwnContracted,
                    d.IsFavorited,
                    d.IsBestSeller,
                    d.IsInstantConfirmation,
                    d.Category
                })
                                      .Select(d => d.First())
                                      .ToList();



                var productListOther = productListOtherList.Where(x => x.Country.ToLower() != "thailand")?.ToList();

                Parallel.ForEach(productListOther, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (productInfo) =>
                {
                    try
                    {
                        var ProductList = _globalTixAdapterV3.GetProductInfoV3(productInfo.Id, token, nonthaiProduct);
                        if (ProductList.Count >= 1)
                        {
                            _globalTixPersistence.SaveProductInfoListV3(ProductList);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_GetProductInfoV3|{productInfo.Id}", ex);

                    }
                });

                /*List<Tickettype>*/
                if (NoDupsproductListOther != null)
                {
                    Parallel.ForEach(NoDupsproductListOther, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (productOption) =>
                    {
                        try
                        {
                            var productOptionData = _globalTixAdapterV3.GetProductOptionV3(token, productOption.Id, nonthaiProduct);

                            List<ProductOptionV3> productOptions = productOptionData.Item1; // Extract the first item from the tuple
                            List<Tickettype> ticketTypes = productOptionData.Item2; // Extract the second item from the tuple

                            lock (gtProductOption) // Lock to ensure thread-safe access to gtProductOption list
                            {
                                gtProductOption.AddRange(productOptions);
                            }

                            lock (allTicketTypes) // Lock to ensure thread-safe access to allTicketTypes list
                            {
                                allTicketTypes.AddRange(ticketTypes);
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (_log) // Lock for thread-safe logging
                            {
                                _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_SaveGlobalTixActivitiesV3Async|{productOption.Id}", ex);
                            }

                        }
                    });
                }


                if (productListThailand != null && productListThailand.Count > 0)
                {
                    productList.AddRange(productListThailand);
                }

                if (productListOther != null && productListOther.Count > 0)
                {
                    productList.AddRange(productListOther);
                }

                _globalTixPersistence.SaveAllActivitiesV3(productList, gtProductOption, allTicketTypes);


                Parallel.ForEach(uniqueCountryIds4, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (countryId) =>
                {
                    try
                    {
                        var ProductChangesV3List = _globalTixAdapterV3.GetProductChangesV3(token, countryId.ToInt(), false);
                        if (ProductChangesV3List != null && ProductChangesV3List.Count > 0)
                        {
                            _globalTixPersistence.SaveGlobalTixProductChangesV3(ProductChangesV3List);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_GetProductChangesV3|{countryId.ToInt()}", ex);

                    }
                });


                Parallel.ForEach(uniqueCountryIdsOther, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (countryId) =>
                {
                    try
                    {
                        var ProductChangesV3List = _globalTixAdapterV3.GetProductChangesV3(token, countryId.ToInt(), true);
                        if (ProductChangesV3List != null && ProductChangesV3List.Count > 0)
                        {
                            _globalTixPersistence.SaveGlobalTixProductChangesV3(ProductChangesV3List);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_GetProductChangesV3|{countryId.ToInt()}", ex);

                    }
                });
            }
            catch (Exception ex)
            {
                _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_GetProductOptionV3", ex);
            }
        }

        public void SaveGlobalTixPackages(string token)
        {
            List<GlobalTixPackage> pkgs = _globalTixAdapter.GetAllPackages(token, true);
            if (pkgs != null && pkgs.Count > 0)
            {
                pkgs.AddRange(_globalTixAdapter.GetAllPackages(token, false));
            }
            else
            {
                pkgs = _globalTixAdapter.GetAllPackages(token, true);
            }
            _globalTixPersistence.SaveAllPackages(pkgs);
        }

        /// <summary>
        /// SaveRezdyDataInDB
        /// </summary>
        /// <param name="token"></param>
        public void SaveRezdyDataInDB(string token)
        {
            try
            {
                //fetch all the Rezdy supported suppliers
                var result = GetRezdyProducts(token);

                if (result != null)
                {
                    if (result?.RezdyProductDetails != null)
                    {
                        //Insert Rezdy products in the database
                        _rezdyPersistence.SaveRezdyProducts(result?.RezdyProductDetails);
                    }
                    if (result?.AgeGroupMappings != null)
                    {
                        //Insert Rezdy product's Age group in database
                        _rezdyPersistence.SaveRezdyAgeGroup(result?.AgeGroupMappings);
                    }
                    if (result?.BookingFields != null)
                    {
                        //Insert rezdy product's Booking fields in database
                        _rezdyPersistence.SaveBookingFields(result?.BookingFields);
                    }
                    if (result?.ListOfProductWiseExtraDetails != null)
                    {
                        //Insert rezdy product's Booking fields in database
                        _rezdyPersistence.SaveExtraDetailsFields(result?.ListOfProductWiseExtraDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("AgeGroupService|SaveRezdyAgeGroup", ex);
                throw;
            }
        }

        /// <summary>
        /// Save Prio Ticket Details
        /// </summary>
        /// <param name="token"></param>
        public void SavePrioTicketList(string token)
        {
            try
            {
                var ticketDetails = _prioTicketAdapter.GetPrioTicketList(token);
                _prioPersistence.SavePrioTicketList(ticketDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SavePrioTicketList",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                // throw;
            }
        }

        /// <summary>
        /// Get and save all Tiqets Variants in the database
        /// </summary>
        /// <param name="token"></param>
        public void SaveTourCMSChannelData(string token)
        {
            try
            {
                var tourCMSList = GetTourCMSChannelList(token, 0);
                var tourCMSMapData = new ChannelResponse();
                if (tourCMSList != null)
                {
                    tourCMSMapData = TourCMSMapData(tourCMSList);
                }
                //var tourCMSMapFinalData = new ChannelResponse();
                if (tourCMSMapData != null && tourCMSMapData.ResponseChannelList != null && tourCMSMapData.ResponseChannelList.Count > 0)
                {
                    foreach (var item in tourCMSMapData.ResponseChannelList)
                    {
                        var tourCMS = GetTourCMSChannelShow(token, Convert.ToInt32(item.ChannelId));
                        var filterMapdata = tourCMSMapData.ResponseChannelList.Where(x => x.ChannelId == item.ChannelId)?.FirstOrDefault();
                        if (tourCMS != null && filterMapdata != null)
                        {
                            TourCMSMapDetail(tourCMS, filterMapdata);
                        }
                    }
                    if (tourCMSMapData?.ResponseChannelList != null && tourCMSMapData?.ResponseChannelList.Count > 0)
                    {
                        _tourCMSPersistence.SaveChannelData(tourCMSMapData?.ResponseChannelList);
                    }
                }
                //Save ChannelList


            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveTourCMSData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        private ChannelResponse TourCMSMapData(ChannelListResponse channelListResponse)
        {
            var responseChannelData = new List<ResponseChannelData>();
            foreach (var item in channelListResponse.ResponseChannelList)
            {
                var responseChannelItem = new ResponseChannelData
                {
                    ChannelId = item.ChannelId,
                    Accountid = item.Accountid,
                    ChannelName = item.ChannelName,
                    TourCount = item.TourCount,
                    LogoUrl = item.LogoUrl,
                    ConnectionPermission = item.ConnectionPermission,
                    Lang = item.Lang,
                    SaleCurrency = item.SaleCurrency,
                    HomeUrl = item.HomeUrl,
                    HomeUrlTracked = item.HomeUrlTracked,
                    ShortDesc = item.ShortDesc
                };
                responseChannelData.Add(responseChannelItem);
            }
            var channelResponse = new ChannelResponse
            {
                Request = channelListResponse.Request,
                Error = channelListResponse.Error,
                ResponseChannelList = responseChannelData
            };
            return channelResponse;
        }


        private void TourCMSMapDetail(ChannelShowResponse channelShowResponse, ResponseChannelData finalData)
        {
            var item = channelShowResponse.ResponseChannelShow?.FirstOrDefault();

            finalData.UtcOffsetMins = item.UtcOffsetMins;
            finalData.BaseCurrency = item.BaseCurrency;
            finalData.ConnectionDate = item.ConnectionDate;
            finalData.CreditCardFeeSalePercentage = item.CreditCardFeeSalePercentage;
            finalData.PermOverrideSalePrice = item.PermOverrideSalePrice;
            finalData.PermWaiveccfee = item.PermWaiveccfee;
            finalData.CompanyName = item.CompanyName;
            finalData.BookingStyle = item.BookingStyle;
            finalData.LongDesc = item.LongDesc;
            finalData.CancelPolicy = item.CancelPolicy;
            finalData.Termsandconditions = item.Termsandconditions;
            finalData.EmailCustomer = item.EmailCustomer;
            finalData.Twitter = item.Twitter;
            finalData.TripAdvisor = item.TripAdvisor;
            finalData.Youtube = item.Youtube;
            finalData.Facebook = item.Facebook;
            finalData.Address1 = item.Address1;
            finalData.AddressCity = item.AddressCity;
            finalData.AddressState = item.AddressState;
            finalData.AddressCountry = item.AddressCountry;
            finalData.CommercialEmailPrivate = item.CommercialEmailPrivate;
            finalData.CommercialContactnamePrivate = item.CommercialContactnamePrivate;
            finalData.CommercialPitchPrivate = item.CommercialPitchPrivate;
            finalData.CommercialPplPrivate = item.CommercialPplPrivate;
            finalData.CommercialDirPrivate = item.CommercialDirPrivate;
            finalData.CommercialPpcPrivate = item.CommercialPpcPrivate;
            finalData.CommercialAffPrivate = item.CommercialAffPrivate;
            finalData.CommercialAgPrivate = item.CommercialAgPrivate;
            finalData.CommercialAnyPrivate = item.CommercialAnyPrivate;
            finalData.CommercialAvleadtimePrivate = item.CommercialAvleadtimePrivate;
            finalData.CommercialAvtransactionPrivate = item.CommercialAvtransactionPrivate;
            finalData.CommercialAvpeoplePrivate = item.CommercialAvpeoplePrivate;
            finalData.CommercialAvdurationPrivate = item.CommercialAvdurationPrivate;
            finalData.CommercialPercentOnlinePrivate = item.CommercialPercentOnlinePrivate;
            finalData.CommercialPercentConvertPrivate = item.CommercialPercentConvertPrivate;
            finalData.CommercialAvclick2bookP = item.CommercialAvclick2bookP;

            //payment
            finalData.GatewayId = item.PaymentGateway.GatewayId;
            finalData.Name = item.PaymentGateway.Name;
            finalData.GatewayType = item.PaymentGateway.GatewayType;
            finalData.TakeVisa = item.PaymentGateway.TakeVisa;
            finalData.TakeMastercard = item.PaymentGateway.TakeMastercard;
            finalData.TakeDiners = item.PaymentGateway.TakeDiners;
            finalData.TakeDiscover = item.PaymentGateway.TakeDiscover;
            finalData.TakeAmex = item.PaymentGateway.TakeAmex;
        }

        /// <summary>
        /// Get TourCMS ChannelList
        /// </summary>

        /// <param name="token"></param>
        /// <returns></returns>
        private ChannelListResponse GetTourCMSChannelList(string token, int channelId)
        {
            var channelList = new ChannelListResponse();
            try
            {
                channelList = _tourCMSAdapter.GetChannelData(token, channelId);

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "GetTourCMSChannelList",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);

            }
            return channelList;
        }

        /// <summary>
        /// Get TourCMS ChannelList
        /// </summary>

        /// <param name="token"></param>
        /// <returns></returns>
        private ChannelShowResponse GetTourCMSChannelShow(string token, int channelId)
        {
            var channelShow = new ChannelShowResponse();
            try
            {
                channelShow = _tourCMSAdapter.GetChannelShowData(token, channelId);

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "GetTourCMSChannelShow",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);

            }
            return channelShow;
        }

        /// <summary>
        /// Get and save all Tiqets Variants in the database
        /// </summary>
        /// <param name="token"></param>
        public void SaveTourCMSTourData(string token)
        {
            try
            {
                //Data from Database:
                var selectedProducts = _masterPersistence.LoadTourCMSSelectedChannel().Distinct().ToList();
                //Data from API
                var tourCMSList = GetTourCMSTourList(token, 0);
                var tourCMSMapData = new TourResponse();
                if (tourCMSList != null)
                {
                    //filter data according to database
                    tourCMSMapData = TourCMSTourMapData(tourCMSList, selectedProducts);
                }
                if (tourCMSMapData != null && tourCMSMapData.ResponseTourList != null && tourCMSMapData.ResponseTourList.Count > 0)
                {
                    var tourRateResponseList = new List<TourRateResponse>();


                    //foreach (var item in tourCMSMapData.ResponseTourList)
                    try
                    {
                        Parallel.ForEach(tourCMSMapData.ResponseTourList, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, item =>
                        {
                            //if (item.ChannelId == 3930.ToString())
                            //{
                            var tourCMS = GetTourCMSTourShow(token, Convert.ToInt32(item.ChannelId), Convert.ToInt32(item.TourId));
                            var filterMapdata = tourCMSMapData?.ResponseTourList?.Where(x => x.ChannelId == item.ChannelId && x.TourId == item.TourId)?.FirstOrDefault();
                            if (tourCMS != null)
                            {
                                // Perserve TourCMSTourRateMapDetail
                                var tourRateResponse = new List<TourRateResponse>();
                                tourRateResponse = TourCMSTourRateMapDetail(tourCMS);
                                tourRateResponseList.AddRange(tourRateResponse);
                            }
                            if (tourCMS != null && filterMapdata != null)
                            {
                                TourCMSTourMapDetail(tourCMS, filterMapdata);
                            }
                            //}
                        });
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = _className,
                            MethodName = "SaveTourCMSTourData",
                            Token = token
                        };
                        _log.Error(isangoErrorEntity, ex);
                        //throw;
                    }
                    //Save TourData
                    if (tourCMSMapData?.ResponseTourList != null && tourCMSMapData.ResponseTourList.Count > 0)
                    {
                        //tourCMSMapData.ResponseTourList=tourCMSMapData?.ResponseTourList.Where(x => x.ChannelId == 3930.ToString()).ToList();
                        _tourCMSPersistence.SaveTourData(tourCMSMapData?.ResponseTourList);
                    }
                    //Save TourRateData
                    if (tourRateResponseList != null && tourRateResponseList.Count > 0)
                    {
                        _tourCMSPersistence.SaveTourRateData(tourRateResponseList);
                    }
                }
                //Save ChannelList
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveTourCMSTourData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }


        /// <param name="token"></param>
        /// <returns></returns>
        private TourShowResponse GetTourCMSTourShow(string token, int channelId, int tourId)
        {
            var tourShow = new TourShowResponse();
            try
            {
                tourShow = _tourCMSAdapter.GetTourShowData(token, channelId, tourId);

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "GetTourCMSTourShow",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return tourShow;
        }

        /// <param name="token"></param>
        /// <returns></returns>
        private TourListResponse GetTourCMSTourList(string token, int channelId)
        {
            var tourList = new TourListResponse();
            try
            {
                tourList = _tourCMSAdapter.GetTourData(token, channelId);

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "GetTourCMSTourList",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);

            }
            return tourList;
        }
        private TourResponse TourCMSTourMapData(TourListResponse tourListResponse, List<TourCMSChannelList> tourCMSChannelList)
        {
            var responseData = new List<Tour>();
            foreach (var item in tourListResponse.ResponseTourList)
            {
                var responseChannelItem = new Tour
                {
                    ChannelId = item.ChannelId,
                    Accountid = item.Accountid,
                    TourId = item.TourId,
                    TourCode = item.TourCode,
                    HasSale = item.HasSale,
                    HasD = item.HasD,
                    HasF = item.HasF,
                    HasH = item.HasH,
                    DescriptionsLastUpdated = item.DescriptionsLastUpdated,
                    TourName = item.TourName,
                };
                responseData.Add(responseChannelItem);
            }
            responseData = responseData?.Where(x => tourCMSChannelList.Any(y => y.channelId == x.ChannelId && y.accountId == x.Accountid)).ToList();

            var channelResponse = new TourResponse
            {
                Request = tourListResponse.Request,
                Error = tourListResponse.Error,
                ResponseTourList = responseData
            };
            return channelResponse;
        }
        private void TourCMSTourMapDetail(TourShowResponse tourShowResponse, Tour finalData)
        {
            var item = tourShowResponse?.TourListShow?.FirstOrDefault();
            finalData.TourNameLong = item?.TourNameLong;
            finalData.TimeType = item?.TimeType;
            finalData.StartTimeZone = item?.StartTimeZone;
            finalData.EndTimeZone = item?.EndTimeZone;
            finalData.QuantityRule = item?.QuantityRule;
            finalData.FromPrice = item?.FromPrice;
            finalData.FromPriceDisplay = item?.FromPriceDisplay;
            finalData.FromPriceUnit = item?.FromPriceUnit;
            finalData.SaleCurrency = item?.SaleCurrency;
            finalData.CreatedDate = item?.CreatedDate;
            finalData.VolumePricing = item?.VolumePricing;
            finalData.MinBookingSize = item?.MinBookingSize;
            finalData.MaxBookingSize = item?.MaxBookingSize;
            finalData.NextBookableDate = item?.NextBookableDate;
            finalData.LastBookableDate = item?.LastBookableDate;
            finalData.HasSaleJan = item?.HasSaleJan;
            finalData.HasSaleFeb = item?.HasSaleFeb;
            finalData.HasSaleMar = item?.HasSaleMar;
            finalData.HasSaleApr = item?.HasSaleApr;
            finalData.HasSaleMay = item?.HasSaleMay;
            finalData.HasSaleJun = item?.HasSaleJun;
            finalData.HasSaleJul = item?.HasSaleJul;
            finalData.HasSaleAug = item?.HasSaleAug;
            finalData.HasSaleSep = item?.HasSaleSep;
            finalData.HasSaleOct = item?.HasSaleOct;
            finalData.HasSaleNov = item?.HasSaleNov;
            finalData.HasSaleDec = item?.HasSaleDec;
            finalData.FromPriceJan = item?.FromPriceJan;
            finalData.FromPriceJanDisplay = item?.FromPriceJanDisplay;
            finalData.FromPriceFeb = item?.FromPriceFeb;
            finalData.FromPriceFebDisplay = item?.FromPriceFebDisplay;
            finalData.FromPriceMar = item?.FromPriceMar;
            finalData.FromPriceMarDisplay = item?.FromPriceMarDisplay;
            finalData.FromPriceApr = item?.FromPriceApr;
            finalData.FromPriceAprDisplay = item?.FromPriceAprDisplay;
            finalData.FromPriceMay = item?.FromPriceMay;
            finalData.FromPriceMayDisplay = item?.FromPriceMayDisplay;
            finalData.FromPriceJun = item?.FromPriceJun;
            finalData.FromPriceJunDisplay = item?.FromPriceJunDisplay;
            finalData.FromPriceJul = item?.FromPriceJul;
            finalData.FromPriceJulDisplay = item?.FromPriceJulDisplay;
            finalData.FromPriceAug = item?.FromPriceAug;
            finalData.FromPriceAugDisplay = item?.FromPriceAugDisplay;
            finalData.FromPriceSep = item?.FromPriceSep;
            finalData.FromPriceSepDisplay = item?.FromPriceSepDisplay;
            finalData.FromPriceOct = item?.FromPriceOct;
            finalData.FromPriceOctDisplay = item?.FromPriceOctDisplay;
            finalData.FromPriceNov = item?.FromPriceNov;
            finalData.FromPriceNovDisplay = item?.FromPriceNovDisplay;
            finalData.FromPriceDec = item?.FromPriceDec;
            finalData.FromPriceDecDisplay = item?.FromPriceDecDisplay;
            finalData.Priority = item?.Priority;
            finalData.Country = item?.Country;
            finalData.LanguagesSpoken = item?.LanguagesSpoken;
            finalData.GeocodeStart = item?.GeocodeStart;
            finalData.GeocodeEnd = item?.GeocodeEnd;
            finalData.TourleaderType = item?.TourleaderType;
            finalData.Grade = item?.Grade;
            finalData.Accomrating = item?.Accomrating;
            finalData.Location = item?.Location;
            finalData.Summary = item?.Summary;
            finalData.Shortdesc = item?.Shortdesc;
            finalData.Exp = item?.Exp;
            finalData.DurationDesc = item?.DurationDesc;
            finalData.Duration = item?.Duration;
            finalData.Available = item?.Available;
            finalData.IncEx = item?.IncEx;
            finalData.Inc = item?.Inc;
            finalData.Ex = item?.Ex;
            finalData.TourUrl = item?.TourUrl;
            finalData.TourUrlTracked = item?.TourUrlTracked;
            finalData.BookUrl = item?.BookUrl;
            finalData.SuitableForSolo = item?.SuitableForSolo;
            finalData.SuitableForCouples = item?.SuitableForCouples;
            finalData.SuitableForChildren = item?.SuitableForChildren;
            finalData.SuitableForGroups = item?.SuitableForGroups;
            finalData.SuitableForStudents = item?.SuitableForStudents;
            finalData.SuitableForBusiness = item?.SuitableForBusiness;
            finalData.SuitableForWheelchairs = item?.SuitableForWheelchairs;
            finalData.PickupOnRequest = item?.PickupOnRequest;
            finalData.PickupOnRequestKey = item?.PickupOnRequestKey;
            finalData.DistributionIdentifier = item?.DistributionIdentifier;

            ////ResponseTourPickup
            var pickupItem = item?.PickupPoints?.FirstOrDefault();
            finalData.PickupName = pickupItem?.PickupName;
            finalData.Description = pickupItem?.Description;
            finalData.Address1 = pickupItem?.Address1;
            finalData.Address2 = pickupItem?.Address2;
            finalData.Postcode = pickupItem?.Postcode;
            finalData.Geocode = pickupItem?.Geocode;
            finalData.PickupId = pickupItem?.PickupId;
            ////ResponseTourPickup

            ////Reposonse Tour Images
            var itemImage = item?.Images;
            finalData.UrlThumbnail = itemImage?.UrlThumbnail;
            finalData.Url = itemImage?.Url;
            finalData.UrlLarge = itemImage?.UrlLarge;
            finalData.UrlXlarge = itemImage?.UrlXlarge;
            finalData.ImageDesc = itemImage?.ImageDesc;
            finalData.Thumbnail = itemImage?.Thumbnail;
            ////Response Tour Images

            ////ResponseTourNew_booking
            finalData.Datetype = item.NewBooking?.DateSelection?.Datetype;
            ////ResponseTourNew_booking

            ////ResponseTourNew_bookingRate
            var itemPeople = item?.NewBooking?.PeopleSelection.BookingRate.FirstOrDefault();
            finalData.Label1 = itemPeople?.Label1;
            finalData.Label2 = itemPeople?.Label2;
            finalData.Minimum = itemPeople?.Minimum;
            finalData.Maximum = itemPeople?.Maximum;
            finalData.RateId = itemPeople?.RateId;
            finalData.AgeCat = itemPeople?.AgeCat;
            finalData.AgerangeMin = itemPeople?.AgerangeMin;
            finalData.AgerangeMax = itemPeople?.AgerangeMax;
            finalData.BookingRateFromPrice = itemPeople?.FromPrice;
            finalData.BookingRateFromPriceDisplay = itemPeople?.FromPriceDisplay;
            ////ResponseTourNew_bookingRate

            ////ResponseTourItem
            var itemHealth = item?.HealthAndSafety?.FirstOrDefault();
            finalData.Name = itemHealth?.Name;
            finalData.DisplayName = itemHealth?.DisplayName;
            finalData.Value = itemHealth?.Value;
            ////ResponseTourItem

            ////ResponseTourProduct_type
            var productItem = item?.ProductType;
            finalData.Direction = productItem?.Direction;
            finalData.AirportCode = productItem?.AirportCode;
            finalData.ProductTypeValue = productItem?.Value;
            ////ResponseTourProduct_type

            ////ResponseTourCutoff
            finalData.Type = item?.CutOff?.Type;
            finalData.TourCutoffValue = item?.CutOff?.Value;
            //payment

        }

        private List<TourRateResponse> TourCMSTourRateMapDetail(TourShowResponse tourShowResponse)
        {
            var TourRateResponseList = new List<TourRateResponse>();
            var GetParentData = tourShowResponse?.TourListShow?.FirstOrDefault();
            var GetRate = tourShowResponse?.TourListShow?.FirstOrDefault()?.NewBooking?.PeopleSelection?.BookingRate;
            if (GetRate != null && GetRate.Count > 0)
            {
                foreach (var item in GetRate)
                {
                    var rate = new TourRateResponse
                    {
                        ChannelId = Convert.ToInt32(GetParentData.ChannelId),
                        AccountId = Convert.ToInt32(GetParentData.AccountId),
                        TourId = Convert.ToInt32(GetParentData.TourId),
                        label_1 = item.Label1,
                        label_2 = item.Label2,
                        minimum = item.Minimum,
                        maximum = item.Maximum,
                        rate_id = item.RateId,
                        agecat = item.AgeCat,
                        agerange_min = item.AgerangeMin,
                        agerange_max = item.AgerangeMax,
                        from_price = item.FromPrice,
                        from_price_display = item.FromPriceDisplay
                    };
                    TourRateResponseList.Add(rate);
                }
            }
            return TourRateResponseList;
        }
        public void SaveNewCitySightSeeingProductList(string token)
        {
            string request = "";
            string response = "";
            var productsList = _newCitySightSeeingAdapter.ProductsAsync(null,
                token, out request, out response);

            //Check all products that have variant null, then call availability API for that only
            var NullVariantProducts = productsList?.Where(x => x.Variants == null || x.Variants.Count == 0)?.ToList();
            if (NullVariantProducts != null && NullVariantProducts.Count > 0)
            {
                foreach (var nullvariant in NullVariantProducts)
                {
                    var dataPass = new Entities.NewCitySightSeeing.NewCitySightSeeingCriteria
                    {
                        SupplierOptionNewCitySeeing = nullvariant.Sku,
                        CheckinDate = System.DateTime.Now,
                        CheckoutDate = System.DateTime.Now.AddDays(30),
                        Days2Fetch = 30
                    };

                    var availabilityResponseData = _newCitySightSeeingAdapter.GetNullVariantData(dataPass,
                    token, out request, out response);
                    if (availabilityResponseData != null)
                    {
                        //Find Distinct variant code
                        var SourcesExist = availabilityResponseData?.Days?.SelectMany(p => p.Availabilities)?.Select(i => new { i.Source, i.ProductCode })?.Distinct()?.ToList();
                        nullvariant.Variants = new List<ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product.Variant>();
                        if (SourcesExist != null && SourcesExist.Count > 0)
                        {
                            foreach (var itemSource in SourcesExist)
                            {
                                var singleNullVariant = new ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product.Variant
                                {
                                    Code = itemSource.ProductCode, //productcode
                                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                                    Title = "", //make it blank because cant name multiple options name same
                                    VariantCode = itemSource.ProductCode,
                                    VariantName = ""//make it blank because cant name multiple options name same
                                };
                                nullvariant?.Variants?.Add(singleNullVariant);
                            }
                        }
                    }
                }
            }
            if (productsList != null)
            {
                _newCitySightSeeingPersistence.SaveNewCitySightSeeingProducts(productsList);
            }
        }
        public void SaveGoCityProductList(string token)
        {
            string request = "";
            string response = "";
            var productsList = _goCityAdapter.ProductsAsync(null,
                token, out request, out response);
            if (productsList != null)
            {
                _goCityPersistence.SaveGoCityProducts(productsList);
            }
        }

        public void SavePrioHubProductData(string token)
        {
            try
            {

                var productList = new List<ServiceAdapters.PrioHub.PrioHub.Entities.ProductListResponse.Item>();
                var routeList = new List<ItemRoute>();
                var listDistributorId = new List<int>
                {
                    42865,
                    2425,
                    49167
                };
                listDistributorId = listDistributorId?.Distinct()?.ToList();
                string request = string.Empty;
                string response = string.Empty;
                var newPrioCriteria = new PrioHubCriteria();

                foreach (var dist in listDistributorId)
                {
                    try
                    {
                        newPrioCriteria.DistributorId = Convert.ToInt32(dist);
                        //Save Products
                        productList.AddRange(_prioHubAdapter.ProductsAsync(newPrioCriteria, token, out request, out response));
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (productList != null && productList.Count > 0)
                {
                    _newPrioPersistence.SavePrioHubProducts(productList);
                }

                foreach (var dist in listDistributorId)
                {
                    try
                    {
                        newPrioCriteria.DistributorId = Convert.ToInt32(dist);
                        //Save Routes
                        routeList.AddRange(_prioHubAdapter.ProductRoutesAsync(newPrioCriteria, token, out request, out response));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (routeList != null && routeList.Count > 0)
                {
                    _newPrioPersistence.SavePrioHubProductsRoutes(routeList);
                }

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "SaveNewPrioProductData",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }


        public void SaveRaynaProductList(string token)
        {
            string request = "";
            string response = "";
            var countryCityList = new List<Isango.Entities.Rayna.CountryCity>();
            var resultTour = new List<ServiceAdapters.Rayna.Rayna.Entities.ResultTour>();
            var resultTourById = new List<ServiceAdapters.Rayna.Rayna.Entities.ResultTourStaticDataById>();
            var resultTouroption = new List<ServiceAdapters.Rayna.Rayna.Entities.Touroption>();
            var resultTransferTimeTourOption = new List<ServiceAdapters.Rayna.Rayna.Entities.TransferTimeTourOption>();

            //1.1 Country City Adapter
            var country = _raynaAdapter.CountryData(token, out request, out response);
            if (country != null)
            {
                if (country?.ResultCountry != null && country?.ResultCountry.Count > 0)
                {
                    // Loop of Country
                    foreach (var couty in country.ResultCountry)
                    {
                        var countryId = couty?.CountryId;
                        if (countryId > 0)
                        {
                            var cty = _raynaAdapter.CityData(countryId, token, out request, out response);
                            if (cty != null && cty.ResultCityByCountry != null && cty.ResultCityByCountry.Count > 0)
                            {
                                //Loop of Cities
                                foreach (var ct in cty.ResultCityByCountry)
                                {
                                    var countryData = new Isango.Entities.Rayna.CountryCity
                                    {
                                        CountryId = couty.CountryId,
                                        CountryName = couty.CountryName,
                                        CityId = ct.CityId,
                                        CityName = ct.CityName
                                    };
                                    if (countryData != null)
                                    {
                                        countryCityList.Add(countryData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //1.2Save Country City
            if (countryCityList != null && countryCityList.Count > 0)
            {
                _raynaPersistence.SaveCountryCity(countryCityList);
            }
            //2.1 TourList Adapter
            if (countryCityList != null && countryCityList.Count > 0)
            {
                foreach (var item in countryCityList)
                {
                    var getData = _raynaAdapter.TourStaticData(item.CountryId, item.CityId, token, out request, out response);
                    if (getData != null && getData?.ResultTour != null && getData?.ResultTour.Count > 0)
                    {
                        resultTour.AddRange(getData?.ResultTour);
                    }
                }
            }
            //2.2 Save TourList 
            if (resultTour != null && resultTour.Count > 0)
            {
                _raynaPersistence.SaveTourList(resultTour);
            }
            //3.1 TourListBy Id Adapter
            if (resultTour != null && resultTour.Count > 0)
            {
                foreach (var itemById in resultTour)
                {
                    var getData = _raynaAdapter.TourStaticDataById(itemById.CountryId, itemById.CityId, itemById.TourId, itemById.ContractId, DateTime.Now.ToString("MM/dd/yyyy"), token, out request, out response);
                    if (getData != null && getData.ResultTourStaticDataById != null && getData.ResultTourStaticDataById.Count > 0)
                    {
                        resultTourById.AddRange(getData?.ResultTourStaticDataById);
                    }
                }
            }
            //3.2: Save TourList By Id
            if (resultTourById != null && resultTourById.Count > 0)
            {
                _raynaPersistence.SaveTourListById(resultTourById);
            }
            //4.1 Tour Options
            if (resultTour != null && resultTour.Count > 0)
            {
                foreach (var item in resultTour)
                {
                    var getData = _raynaAdapter.TourOptions(item.TourId, item.ContractId, token, out request, out response);
                    if (getData != null && getData?.ResultTourOptions != null && getData?.ResultTourOptions?.Touroption != null && getData?.ResultTourOptions?.Touroption.Count > 0)
                    {
                        resultTouroption.AddRange(getData?.ResultTourOptions.Touroption);
                    }
                    if (getData != null && getData.ResultTourOptions != null && getData?.ResultTourOptions?.TransferTimeTourOption != null && getData?.ResultTourOptions?.TransferTimeTourOption.Count > 0)
                    {
                        resultTransferTimeTourOption.AddRange(getData?.ResultTourOptions?.TransferTimeTourOption);
                    }

                }
            }
            //4.2: Save Tour Options
            if (resultTouroption != null && resultTouroption.Count > 0)
            {
                _raynaPersistence.SaveTourOptions(resultTouroption);
            }
            //4.3  Transfer Time Tour Option
            if (resultTransferTimeTourOption != null && resultTransferTimeTourOption.Count > 0)
            {
                _raynaPersistence.SaveTourOptionTransferTime(resultTransferTimeTourOption);
            }

        }
        #region Private Methods

        /// <summary>
        /// Get AOT Age Group data from adapter and save it in the database
        /// </summary>
        /// <param name="aotProducts"></param>
        /// <param name="token"></param>
        private void SaveAllOptionsGeneralInformation(List<IsangoHBProductMapping> aotProducts, string token)
        {
            if (aotProducts?.Count > 0)
            {
                var countryTypeAustralia = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeAustralia));
                var countryTypeNewZealand = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeNewZealand));

                foreach (var product in aotProducts)
                {
                    try
                    {
                        var opts = new Opts { Opt = new List<string> { product.HotelBedsActivityCode } };
                        var optionGeneralInfoRequest = new OptionGeneralInfoRequest { Opts = opts };

                        //Find country from region Id
                        var countryType = (product.CountryId == countryTypeAustralia) ? CountryType.Australia : product.CountryId == countryTypeNewZealand ? CountryType.NewZealand : CountryType.Fiji;

                        //Set AgentId and Password through a country type
                        _aotAdapter.SetAgentIdPassword(countryType);

                        //Get product details from adapter
                        var optionGeneralInfoResponse = _aotAdapter.GetProductDetails(optionGeneralInfoRequest, token);

                        //Get cancellation policy of option
                        var cancellationPolicy = GetCancellationPolicy(opts, token);

                        //Save all activity age groups mapping
                        _aotPersistence.SaveAllActivityAgeGroupsMapping(optionGeneralInfoResponse, cancellationPolicy);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = _className,
                            MethodName = "SaveAllOptionsGeneralInformation",
                            Token = token
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Get cancellation policy of option
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetCancellationPolicy(Opts opts, string token)
        {
            var criteria = new AotCriteria
            {
                OptCode = opts.Opt,
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Adult, 1}
                },
                CheckinDate = DateTime.Now.AddDays(1),
                CheckoutDate = DateTime.Now.AddDays(2),
                CancellationPolicy = true
            };

            var response = _aotAdapter.GetDetailedPricingAvailability(criteria, token, false);

            var cancellationPolicy = string.Empty;
            if (response != null)
            {
                var result = response as Dictionary<DateTime, OptionStayPricingResponse>;
                var optCode = criteria.OptCode.FirstOrDefault();
                var optStayResult =
                    result?.FirstOrDefault().Value.OptStayResults?.FirstOrDefault(x => x.Opt.Equals(optCode));
                cancellationPolicy = optStayResult?.CancellationPolicy;
            }
            return cancellationPolicy;
        }

        /// <summary>
        /// Save GrayLineIceLand Age Group data in the database
        /// </summary>
        /// <param name="ageGroups"></param>
        private void SaveAllGrayLineIceLandAgeGroups(Dictionary<int, List<GrayLineIceLand.AgeGroup>> ageGroups)
        {
            if (ageGroups?.Count > 0)
            {
                var masterAgeGroupList = new List<GrayLineIceLand.AgeGroup>();
                var activityAgeGroupList = new List<GrayLineIceLand.ActivityAgeGroup>();

                //Get activity age groups and master age groups
                foreach (var ageGroupList in ageGroups)
                {
                    var ageGroupLists = ageGroupList.Value;
                    foreach (var activityAgeGroup in ageGroupLists)
                    {
                        activityAgeGroupList.Add(new GrayLineIceLand.ActivityAgeGroup
                        {
                            ActivityId = ageGroupList.Key,
                            AgeGroupId = activityAgeGroup.AgeGroupId
                        });

                        if (!string.IsNullOrWhiteSpace(activityAgeGroup.Description))
                        {
                            var description = activityAgeGroup.Description.Trim();
                            if (description.Split('(').Length > 1)
                            {
                                var splitDescription = description.Split('(')[1].Split(')')[0].Split('-');
                                activityAgeGroup.FromAge = Convert.ToInt32(splitDescription[0]);
                                activityAgeGroup.ToAge = Convert.ToInt32(splitDescription[1]);
                            }
                        }

                        masterAgeGroupList.Add(activityAgeGroup);
                    }
                }

                masterAgeGroupList = masterAgeGroupList.GroupBy(x => x.AgeGroupId).Select(x => x.First()).ToList();

                //Save all age groups
                _grayLineIceLandPersistence.SaveAllAgeGroups(masterAgeGroupList);

                //Save all activity age groups
                _grayLineIceLandPersistence.SaveAllActivityAgeGroupsMapping(activityAgeGroupList);
            }
        }

        /// <summary>
        /// Save GrayLineIceLand Pickup Location data in the database
        /// </summary>
        /// <param name="pickupLocationsByProducts"></param>
        private void SaveAllGrayLineIceLandPickupLocations(Dictionary<int, List<GrayLineIceLand.Pickuplocation>> pickupLocationsByProducts)
        {
            if (pickupLocationsByProducts?.Count > 0)
            {
                var masterPickupLocations = new List<GrayLineIceLand.Pickuplocation>();
                var activityPickupLocationsList = new List<GrayLineIceLand.ActivityPickupLocation>();
                foreach (var pickupLocations in pickupLocationsByProducts)
                {
                    var pickupLocationValues = pickupLocations.Value;
                    masterPickupLocations.AddRange(pickupLocationValues);
                    foreach (var activityPickupLocations in pickupLocationValues)
                    {
                        activityPickupLocationsList.Add(new GrayLineIceLand.ActivityPickupLocation()
                        {
                            ActivityId = pickupLocations.Key,
                            PickupLocationId = activityPickupLocations.Id,
                            PickupTime = activityPickupLocations.PickupTime,
                            TimePUMinutes = activityPickupLocations.TimePuMinutes
                        });
                    }
                }
                masterPickupLocations = masterPickupLocations.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                //Save all pickup locations
                _grayLineIceLandPersistence.SaveAllPickupLocations(masterPickupLocations);

                //Save all activity pickup locations
                _grayLineIceLandPersistence.SaveAllPickupLocationsMapping(activityPickupLocationsList);
            }
        }

        /// <summary>
        /// Get FHB companies from FHB adapter through UserKey
        /// </summary>
        /// <param name="fareHarborUserKeys"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<Entities.Supplier> GetAllFareHarborCompanies(List<FareHarborUserKey> fareHarborUserKeys, string token)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (fareHarborUserKeys == null || fareHarborUserKeys?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            var companies = new List<Entities.Supplier>();
            foreach (var userKey in fareHarborUserKeys)
            {
                try
                {
                    //Get all companies from FHB adapter
                    var company = _fareHarborAdapter.GetCompanies(userKey, token);

                    company.ForEach(x => x.Currency = userKey.Currency);
                    company.ForEach(x => x.UserKey = userKey.UserKey);

                    companies.AddRange(company);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = _className,
                        MethodName = "GetAllFareHarborCompanies",
                        Token = token
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return companies;
        }

        /// <summary>
        /// Get FHB company mappings
        /// </summary>
        /// <param name="suppliers"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<CompaniesMapping> GetAllFareHarborCompanyMappings(List<Entities.Supplier> suppliers, string token)
        {
            if (suppliers?.Count > 0)
            {
                var companyMappingList = new List<CompaniesMapping>();
                foreach (var supplier in suppliers)
                {
                    try
                    {
                        var items = _fareHarborAdapter.GetItems(supplier, token);
                        items.RemoveAll(item => item == null);
                        if (items.Count > 0)
                        {
                            companyMappingList.AddRange(items.Select(x => new CompaniesMapping(x, supplier.ShortName)));
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = _className,
                            MethodName = "GetAllFareHarborCompanyMappings",
                            Token = token
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }

                return companyMappingList;
            }
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        private Prio.ProductDetails ProcessPrioProductDetails(object result, string ticketId)
        {
            var prioDetails = new Prio.ProductDetails();
            var ticketDetailRs = (TicketDetailRs)result;
            var productDetails = ticketDetailRs?.Data;
            if (productDetails != null)
            {
                prioDetails.CombiTickets = Convert.ToBoolean(productDetails.CombiTicket);
                prioDetails.ProductId = Convert.ToInt32(ticketId);
                prioDetails.Duration = productDetails.Duration;
                prioDetails.Included = (productDetails.Included != null && productDetails.Included.Count() > 0) ? SerializeDeSerializeHelper.Serialize(productDetails.Included) : string.Empty;
                prioDetails.PickUpPoints = productDetails.PickupPoints;
            }

            return prioDetails;
        }

        /// <summary>
        /// Process Prio Details
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        private List<Prio.AgeGroup> ProcessPrioDetails(object result, string ticketId)
        {
            var prioDetails = new List<Prio.AgeGroup>();
            var ticketDetailRs = (TicketDetailRs)result;
            var dataInRes = ticketDetailRs?.Data;
            var ticketTypeDetails = ticketDetailRs?.Data?.TicketTypeDetails;
            if (ticketTypeDetails != null)
            {
                foreach (var details in ticketTypeDetails)
                {
                    var ageGroup = new Prio.AgeGroup
                    {
                        TicketId = Convert.ToInt32(ticketId),
                        Description = details.TicketType,
                        FromAge = details.AgeFrom,
                        ToAge = details.AgeTo,
                        Startdate = DateTime.Parse(details.StartDate),
                        Enddate = DateTime.Parse(details.EndDate),
                        CreatedDate = DateTime.Now,
                        LastModifiedDate = DateTime.Now,
                        MinCapacity = !string.IsNullOrEmpty(dataInRes.BookSizeMin) ? (Convert.ToInt32(dataInRes.BookSizeMin) == 0 ? 1 : Convert.ToInt32(dataInRes.BookSizeMin)) : 1,
                        MaxCapacity = !string.IsNullOrEmpty(dataInRes.BookSizeMax) ? (Convert.ToInt32(dataInRes.BookSizeMax) == 0 ? 99 : Convert.ToInt32(dataInRes.BookSizeMax)) : 99
                    };
                    prioDetails.Add(ageGroup);
                }
            }

            return prioDetails;
        }

        private List<RouteMap> ProcessPrioRouteMaps(object result)
        {
            var routemapDetails = new List<RouteMap>();
            try
            {
                var ticketDetailRs = (TicketDetailRs)result;
                var routeDetails = ticketDetailRs?.Data?.Route;
                if (routeDetails != null)
                {
                    foreach (var route in routeDetails)
                    {
                        var ticketTypeDetails = route.Locations;
                        if (ticketTypeDetails != null)
                        {
                            foreach (var details in ticketTypeDetails)
                            {
                                var routeMap = new RouteMap
                                {
                                    LocationId = details.Id,
                                    Name = details.Name,
                                    Description = details.Description,
                                    Latitude = details.Latitude,
                                    Longitude = details.Longitude,
                                    StopOver = details.StopOver
                                };
                                routemapDetails.Add(routeMap);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return routemapDetails;
        }

        /// <summary>
        /// Get all FHB customer prototypes
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<CustomerProtoTypeCustomerType> GetAllCustomerPrototypesFromFareHarbor(List<FareHarbor.Product> products, string token)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (products == null || products?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null



            var customerPrototypesServiceList = new List<CustomerProtoTypeCustomerType>();
            foreach (var item in products)
            {
                try
                {
                    if (item.ServiceId != 0)
                    {
                        var daystoFetch = item.CheckoutDate.Subtract(item.CheckinDate).Days;



                        for (int i = 0; i < (daystoFetch / 7); i++)
                        {
                            item.CheckinDate = DateTime.Now.AddDays(i * 7);
                            item.CheckoutDate = item.CheckinDate.AddDays(6);



                            #region Get ALL Availabilities containg prototypes and types



                            var items = _fareHarborAdapter.GetCustomerPrototypesByProductId(item, token);
                            if (items != null)
                            {
                                foreach (var availableItems in items?.Availabilities)
                                {



                                    //var availableItems = itemInner.(x => x.CustomerTypeRates != null && x.CustomerTypeRates.Count > 0 && x.CustomerTypeRates.Count(y => y.CustomerPrototype != null && y.CustomerType != null) > 0);



                                    #endregion Get ALL Availabilities containg prototypes and types



                                    #region Bind Customer Prototypes and customer types for products



                                    if (availableItems?.CustomerTypeRates != null && availableItems.CustomerTypeRates.Count > 0)
                                    {
                                        var allCustomerTypeRates = availableItems.CustomerTypeRates;
                                        var startAt = DateTimeOffset.Parse(availableItems.StartAt);
                                        var endAt = DateTimeOffset.Parse(availableItems.EndAt);
                                        var tourMinPartySize = availableItems.MinimumPartySize != 0 ? availableItems.MinimumPartySize : 1;
                                        int? tourMaxPartySize = Convert.ToInt32(availableItems.MaximumPartySize);
                                        if (tourMaxPartySize == 0)
                                            tourMaxPartySize = availableItems.Capacity != null && availableItems.Capacity != 0 ? availableItems.Capacity : 99;
                                        var customerProtoTypeCustomerTypes = allCustomerTypeRates.Select(x => new CustomerProtoTypeCustomerType
                                        {
                                            CustomerTypePk = x.CustomerType.Pk,
                                            Pk = x.CustomerPrototype.Pk,
                                            DisplayName = x.CustomerPrototype.DisplayName,
                                            EndAt = endAt.DateTime,
                                            StartAt = startAt.DateTime,
                                            Note = x.CustomerType.Note,
                                            Plural = x.CustomerType.Plural,
                                            Singular = x.CustomerType.Singular,
                                            Total = x.Total,
                                            TourPk = item.FactsheetId,
                                            ServiceId = item.ServiceId,
                                            MinPartySize = x.MinimumPartySize,
                                            MaxPartySize = x.MaximumPartySize,
                                            TourMinPartySize = tourMinPartySize,
                                            TourMaxPartySize = tourMaxPartySize
                                        }).ToList();



                                        customerPrototypesServiceList.AddRange(customerProtoTypeCustomerTypes);
                                    }
                                }
                            }
                            #endregion Bind Customer Prototypes and customer types for products
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = _className,
                        MethodName = "GetAllCustomerPrototypesFromFareHarbor",
                        Token = token
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }



            return customerPrototypesServiceList;
        }

        /// <summary>
        /// Get All Tiqets Variants
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Tuple<List<Isango.Entities.Tiqets.ProductDetails>,
            Dictionary<int, List<ProductVariant>>,
            List<Isango.Entities.Tiqets.ContentMedia>, List<Isango.Entities.Tiqets.PackageProducts>>
            GetAllTiqetsVariants(
            string token)
        {

            var products = new List<IsangoHBProductMapping>();

            var productVariants = new Dictionary<int, List<ProductVariant>>();
            var productDetails = new List<Isango.Entities.Tiqets.ProductDetails>();
            var contentMediaList = new List<Isango.Entities.Tiqets.ContentMedia>();
            var PackageProductList = new List<Isango.Entities.Tiqets.PackageProducts>();

            //GetAllProducts from API
            var pageNumber = 1;
            var firstSetOfProducts = _tiqetsAdapter.GetProductFilter(token, pageNumber);
            int loopData = firstSetOfProducts.Pagination.Total;
            int loopDataupto = (loopData / 100) + 1;

            var itemList = new List<ProductFilterData>();
            //foreach (var pageNumberPass in Enumerable.Range(1, loopDataupto))
            Parallel.ForEach(Enumerable.Range(1, loopDataupto), pageNumberPass =>
            {
                try
                {
                    var data = _tiqetsAdapter.GetProductFilter(token, pageNumberPass);
                    itemList.AddRange(data?.Products);
                }
                catch (Exception ex)
                {
                }
            });
            var ProductIdsDataOnly = itemList.Select(x => x.Id).ToList();
            //if want to check particular product 
            //var ProductIdsDataOnly = itemList.Where(x => x.Id == "1012536").Select(x => x.Id).ToList();


            int checkinDays = _tietscheckindays < 0 ? 1 : _tietscheckindays;
            int checkoutDays = _tiqetscheckoutdays <= 0 ? 8 : _tiqetscheckoutdays;
            foreach (var product in ProductIdsDataOnly)
            //Parallel.ForEach(ProductIdsDataOnly, product =>
            {
                try
                {
                    //var criteria = new TiqetsCriteria
                    //{
                    //    ProductId = Convert.ToInt32(product),
                    //    Language = Constant.DefaultLanguage,
                    //    CheckinDate = DateTime.Now,
                    //    CheckoutDate = DateTime.Now.AddMonths(3) //Note: Need to verify
                    //};
                    var criteria = new TiqetsCriteria
                    {
                        ProductId = Convert.ToInt32(product),
                        //ProductId= 976338,
                        Language = Constant.DefaultLanguage,
                        CheckinDate = DateTime.Now.AddDays(checkinDays),
                        CheckoutDate = DateTime.Now.AddDays(checkoutDays)
                    };

                    //Get Product Variants
                    var variants = _tiqetsAdapter.GetVariantsForDumpingApplication(criteria, token);

                    //Get product details
                    var productDetail = _tiqetsAdapter.GetProductDetailsByProductId(criteria.ProductId, criteria.Language, token);

                    if (variants != null && !productVariants.ContainsKey(Convert.ToInt32(product)))
                    {
                        productVariants.Add(Convert.ToInt32(product), variants);
                    }
                    if (productDetail != null)
                    {
                        var tiqetsProdDetails = new Isango.Entities.Tiqets.ProductDetails();
                        //tiqetsProdDetails.ServiceId = product.IsangoHotelBedsActivityId.ToString();
                        tiqetsProdDetails.IsSmartPhoneTicket = productDetail.SmartPhoneTicket;

                        tiqetsProdDetails.Duration = productDetail.Duration;
                        tiqetsProdDetails.Language = productDetail.Language;
                        tiqetsProdDetails.Title = productDetail.Title;
                        tiqetsProdDetails.CountryName = productDetail.CountryName;
                        tiqetsProdDetails.CityName = productDetail.CityName;
                        tiqetsProdDetails.Geolocation_Latitude = productDetail.GeoLocation.Latitude;
                        tiqetsProdDetails.Geolocation_Longitude = productDetail.GeoLocation.Longitude;
                        tiqetsProdDetails.Price = productDetail.Price;
                        tiqetsProdDetails.StartPointLatitude = productDetail.StartingPoint != null ? productDetail.StartingPoint.Lat : string.Empty;
                        tiqetsProdDetails.StartPointLongitude = productDetail.StartingPoint != null ? productDetail.StartingPoint.Lng : string.Empty;
                        tiqetsProdDetails.StartPointCity = productDetail.CityName;
                        tiqetsProdDetails.StartPointAddress = productDetail.StartingPoint != null ? productDetail.StartingPoint.Address : string.Empty;
                        tiqetsProdDetails.Summary = productDetail.Summary;
                        tiqetsProdDetails.Included = productDetail.Included;
                        tiqetsProdDetails.Excluded = productDetail.Excluded;
                        tiqetsProdDetails.Display_Price = productDetail.display_price;
                        tiqetsProdDetails.Live_guide_languages = productDetail.live_guide_languages;
                        tiqetsProdDetails.Audio_guide_languages = productDetail.audio_guide_languages;
                        tiqetsProdDetails.Starting_time = productDetail.starting_time;
                        tiqetsProdDetails.wheelchair_access = productDetail.wheelchair_access;
                        tiqetsProdDetails.Supplier_Name = productDetail.supplier.name;
                        tiqetsProdDetails.Supplier_City = productDetail.supplier.city;

                        tiqetsProdDetails.VenueId = productDetail.Venue != null ? productDetail.Venue.Id : string.Empty;
                        tiqetsProdDetails.VenueAddress = productDetail.Venue != null ? productDetail.Venue.Address : string.Empty;
                        tiqetsProdDetails.VenueName = productDetail.Venue != null ? productDetail.Venue.Name : string.Empty;

                        tiqetsProdDetails.IsInstantTicket = productDetail.Instant_Ticket_Delivery;
                        tiqetsProdDetails.ProductId = criteria.ProductId.ToString();
                        tiqetsProdDetails.sale_status_expected_reopen = productDetail.SaleStatusExpectedReopen;
                        tiqetsProdDetails.sale_status_reason = productDetail.SaleStatusReason;
                        tiqetsProdDetails.sale_status = productDetail.SaleStatus;
                        tiqetsProdDetails.IsSkipTheLine = productDetail.SkipLine;
                        tiqetsProdDetails.is_package = productDetail.is_package;
                        tiqetsProdDetails.Marketing_Restrictions = productDetail.marketing_restrictions != null ? string.Join(", ", productDetail.marketing_restrictions) : string.Empty;


                        if (productDetail.Images != null && productDetail.Images.Count > 0)
                        {
                            foreach (var imageInRes in productDetail.Images)
                            {
                                var contentMedia = new Isango.Entities.Tiqets.ContentMedia();
                                contentMedia.Url = imageInRes.Large;
                                contentMedia.Mediatype = Constant.TiqetsImageType;
                                contentMedia.Language = productDetail.Language;
                                contentMedia.Factsheetid = criteria.ProductId;
                                //contentMedia.IsangoProductId = product.IsangoHotelBedsActivityId;
                                contentMedia.Dpi = 0;
                                contentMedia.Duration = string.Empty;
                                contentMedia.Height = 0;
                                contentMedia.Width = 0;
                                contentMedia.VisualizationOrder = 1;
                                contentMedia.Image_order = 1;
                                contentMedia.SizeType = string.Empty;

                                contentMediaList.Add(contentMedia);
                            }
                        }
                        if (productDetail.is_package == true)
                        {
                            foreach (var pckage in productDetail.package_products)
                            {
                                var package = new Isango.Entities.Tiqets.PackageProducts();
                                package.Package_Title = productDetail.Title;
                                package.Product_ID = pckage;
                                package.Package_ID = productDetail.Id;

                                PackageProductList.Add(package);
                            }
                        }


                        var alreadyExistingProd = productDetails.Find(thisProd => thisProd.ProductId.Equals(tiqetsProdDetails.ProductId));
                        if (alreadyExistingProd == null)
                            productDetails.Add(tiqetsProdDetails);
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = _className,
                        MethodName = "GetAllTiqetsVariants",
                        Token = token
                    };
                    _log.Error(isangoErrorEntity, ex);
                    //throw;
                }
            }

            return new Tuple<List<ProductDetails>, Dictionary<int, List<ProductVariant>>, List<Isango.Entities.Tiqets.ContentMedia>, List<Isango.Entities.Tiqets.PackageProducts>>(productDetails, productVariants, contentMediaList, PackageProductList);
        }

        /// <summary>
        /// Get All Golden Tours Age Groups
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private AgeGroupWrapper GetGoldenToursAgeGroups(List<IsangoHBProductMapping> products, string token)
        {
            if (products == null) return null;

            var productDetails = new List<ProductDetail>();
            var ageGroups = new List<GoldenToursAgeGroup>();
            var pricePeriods = new List<Periods>();
            foreach (var product in products)
            {
                var criteria = new GoldenToursCriteria
                {
                    SupplierOptionCode = product.HotelBedsActivityCode,
                    CheckinDate = DateTime.Now,
                    CheckoutDate = DateTime.Now.AddMonths(3)
                };

                var result = _goldenToursAdapter.GetProductDetailsResponse(criteria, token);
                if (result?.ProductDetails != null)
                    productDetails.AddRange(result.ProductDetails);
                if (result?.AgeGroups != null)
                    ageGroups.AddRange(result.AgeGroups);
                if (result?.PricePeriods != null)
                    pricePeriods.AddRange(result.PricePeriods);
            }

            var ageGroupWrapper = new AgeGroupWrapper
            {
                ProductDetails = productDetails,
                AgeGroups = ageGroups,
                PricePeriods = pricePeriods
            };

            return ageGroupWrapper;
        }

        private Entities.Ventrata.AgeGroupWrapperForVentrata GetVentrataAgeGroups(List<Entities.Ventrata.SupplierDetails> supplierDetails, string token)
        {
            var ageGroups = new List<GoldenToursAgeGroup>();
            var listOfProducts = new List<VentrataEntities.Response.ProductRes>();

            foreach (var suppDetail in supplierDetails)
            {
                var productResList = _ventrataAdapter.GetAllProducts(suppDetail.SupplierBearerToken, token, suppDetail.BaseURL);

                var listObjFromProductRes = productResList as List<VentrataEntities.Response.ProductRes>;
                if (listObjFromProductRes?.Count > 0)
                {
                    listObjFromProductRes.ForEach(thisProd => thisProd.SupplierId = suppDetail.SupplierBearerToken);
                    listOfProducts.AddRange(listObjFromProductRes);
                }
            }

            var ageGroupWrapper = new Entities.Ventrata.AgeGroupWrapperForVentrata
            {
                ProductDetails = new List<Entities.Ventrata.ProductDetail>(),
                Destinations = new List<Entities.Ventrata.Destination>(),
                Faqs = new List<Entities.Ventrata.FAQ>(),
                Options = new List<Entities.Ventrata.Option>(),
                UnitDetailsForoptions = new List<Entities.Ventrata.UnitsForOption>(),
                PackageInclude = new List<Entities.Ventrata.PackageInclude>()
            };

            if (listOfProducts != null && listOfProducts.Count > 0)
            {
                foreach (var product in listOfProducts)
                {
                    //For Testing one Product
                    //if (product.id == "22869068-8902-4831-b1e5-90d0ab43c745")
                    //{
                    //Add Product Details
                    var productDetail = new Entities.Ventrata.ProductDetail();
                    productDetail = GetProductDetail(product);
                    //Add to Age Group Wrapper
                    ageGroupWrapper.ProductDetails.Add(productDetail);

                    //Add Destination Details
                    var destination = new Entities.Ventrata.Destination();
                    destination = GetDestinations(product);


                    //Add to Age Group Wrapper
                    ageGroupWrapper.Destinations.Add(destination);

                    //Add FAQs
                    var listOfFaqs = new List<Entities.Ventrata.FAQ>();
                    product.faqs?.ForEach(thisFaq =>
                    {
                        var faq = new Entities.Ventrata.FAQ();
                        faq.ProductId = product?.id;
                        faq.Question = thisFaq?.question;
                        faq.Answer = thisFaq?.answer;
                        if (!listOfFaqs.Contains(faq))
                        {
                            listOfFaqs.Add(faq);
                        }
                    });

                    //Add to Age Group Wrapper
                    ageGroupWrapper.Faqs.AddRange(listOfFaqs);

                    //Add Option Details
                    var listOfOptions = new List<Entities.Ventrata.Option>();
                    var listOfUnitDetails = new List<Entities.Ventrata.UnitsForOption>();

                    product.options?.ForEach(thisOpt =>
                    {
                        var option = new Entities.Ventrata.Option();
                        option = GetOptionsData(product, thisOpt);

                        thisOpt.units?.ForEach(thisUnit =>
                        {
                            //Add UnitsDetails for option
                            var unitForOption = new Entities.Ventrata.UnitsForOption();
                            unitForOption = GetUnitForOption(product, thisOpt, thisUnit);
                            if (!listOfUnitDetails.Contains(unitForOption))
                            {
                                listOfUnitDetails.Add(unitForOption);
                            }
                        });

                        if (!listOfOptions.Contains(option))
                        {
                            listOfOptions.Add(option);
                        }
                    });

                    //Add to Age Group Wrapper
                    ageGroupWrapper.Options.AddRange(listOfOptions);

                    //Add to Age Group Wrapper
                    ageGroupWrapper.UnitDetailsForoptions.AddRange(listOfUnitDetails);

                    //}
                }

                //packages per option
                foreach (var product in listOfProducts)
                {
                    //For Testing one Product
                    //if (product.id == "22869068-8902-4831-b1e5-90d0ab43c745")
                    //{
                    var productDetailPackages = new Entities.Ventrata.ProductDetail();
                    var listOfPackageIncludePackages = new List<Entities.Ventrata.PackageInclude>();
                    var listOfOptionsPackages = new List<Entities.Ventrata.Option>();
                    var listOfUnitDetailsPackages = new List<Entities.Ventrata.UnitsForOption>();
                    product.options?.ForEach(thisOpt =>
                    {
                        PackagesData(thisOpt, product, listOfPackageIncludePackages, ageGroupWrapper,
                            listOfOptionsPackages, listOfUnitDetailsPackages);
                    });
                    //Add to Age Group Wrapper
                    ageGroupWrapper.Options.AddRange(listOfOptionsPackages);

                    //Add to Age Group Wrapper
                    //ageGroupWrapper.UnitDetailsForoptions.AddRange(listOfUnitDetailsPackages);
                    ageGroupWrapper.PackageInclude.AddRange(listOfPackageIncludePackages);
                    //}
                }
            }

            return ageGroupWrapper;
        }

        private void PackagesData(OptionFP thisOpt, ProductRes product,
            List<Entities.Ventrata.PackageInclude> listOfPackageIncludePackages,
            Entities.Ventrata.AgeGroupWrapperForVentrata ageGroupWrapper,
            List<Entities.Ventrata.Option> listOfOptionsPackages,
            List<Entities.Ventrata.UnitsForOption> listOfUnitDetailsPackages
            )
        {
            //packages start
            if (thisOpt?.packageIncludes != null && thisOpt?.packageIncludes.Count() > 0)
            {
                foreach (var include in thisOpt?.packageIncludes)
                {
                    try
                    {
                        if (include?.includes != null && include?.includes.Count() > 0)
                        {

                            foreach (var getInclude in include?.includes)
                            {
                                var getProduct = getInclude?.product;

                                var packageIncludeDataGet = GetPackageInclude(getInclude, include,
                                product?.id, thisOpt?.id);
                                if (packageIncludeDataGet != null)
                                {
                                    if (!listOfPackageIncludePackages.Contains(packageIncludeDataGet))
                                    {
                                        listOfPackageIncludePackages.Add(packageIncludeDataGet);
                                    }
                                }

                                var productDetailPackage = GetProductDetail(getProduct);
                                if (productDetailPackage != null)
                                {
                                    if (!ageGroupWrapper.ProductDetails.Contains(productDetailPackage))
                                    {
                                        bool has = ageGroupWrapper.ProductDetails.Any(cus => cus.ProductId == productDetailPackage.ProductId);
                                        if (!has)
                                        {
                                            ageGroupWrapper.ProductDetails.Add(productDetailPackage);
                                        }
                                    }
                                }


                                var GetDestination = GetDestinations(getProduct);
                                if (GetDestination != null)
                                {
                                    if (!ageGroupWrapper.Destinations.Contains(GetDestination))
                                    {
                                        bool has = ageGroupWrapper.Destinations.
                                               Any(cus => cus.ProductId == GetDestination.ProductId &&
                                               cus.DestinationId == GetDestination.DestinationId);

                                        if (!has)
                                        {
                                            ageGroupWrapper.Destinations.Add(GetDestination);
                                        }
                                    }
                                }

                                var listOfFaqsPackage = new List<Entities.Ventrata.FAQ>();
                                getProduct.faqs?.ForEach(thisFaq =>
                                {
                                    var faq = new Entities.Ventrata.FAQ();
                                    faq.ProductId = product?.id;
                                    faq.Question = thisFaq?.question;
                                    faq.Answer = thisFaq?.answer;
                                    if (!listOfFaqsPackage.Contains(faq))
                                    {
                                        bool has = listOfFaqsPackage.
                                               Any(cus => cus.ProductId == faq.ProductId);

                                        if (!has)
                                        {
                                            listOfFaqsPackage.Add(faq);
                                        }
                                    }
                                });

                                //Add to Age Group Wrapper
                                ageGroupWrapper.Faqs.AddRange(listOfFaqsPackage);



                                //package options start
                                getProduct.options?.ForEach(thisOptPackage =>
                                {

                                    var optionPackage = GetOptionsData(getProduct, thisOptPackage);

                                    thisOptPackage.units?.ForEach(thisUnitPackage =>
                                    {
                                        //Add UnitsDetails for option
                                        var unitForOption = GetUnitForOption(product, thisOptPackage, thisUnitPackage);
                                        if (unitForOption != null)
                                        {
                                            if (!listOfUnitDetailsPackages.Contains(unitForOption))
                                            {
                                                bool has = listOfUnitDetailsPackages.
                                                Any(cus => cus.ProductId == unitForOption.ProductId
                                                && cus.OptionId == unitForOption.OptionId
                                                && cus.OptionId == unitForOption.UnitId);
                                                if (!has)
                                                {
                                                    //List Assign
                                                    listOfUnitDetailsPackages.Add(unitForOption);
                                                }
                                            }
                                        }
                                    });
                                    if (!listOfOptionsPackages.Contains(optionPackage))
                                    {

                                        bool has = listOfOptionsPackages.Any(cus => cus.ProductId == optionPackage.ProductId && cus.OptionId == optionPackage.OptionId);
                                        if (!has)
                                        {
                                            //List Assign
                                            listOfOptionsPackages.Add(optionPackage);
                                        }
                                    }
                                });

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            //packages end
        }

        private Entities.Ventrata.UnitsForOption GetUnitForOption(
            VentrataEntities.Response.ProductRes product, OptionFP thisOpt,
            UnitForproduct thisUnit)
        {
            var unitForOption = new Entities.Ventrata.UnitsForOption();
            unitForOption.ProductId = product.id;
            unitForOption.OptionId = thisOpt.id;
            unitForOption.UnitType = thisUnit.type;
            unitForOption.UnitId = thisUnit.id;
            unitForOption.Reference = thisUnit.reference;
            unitForOption.PaxCount = thisUnit.restrictions.paxCount;
            unitForOption.InternalName = thisUnit.internalName;
            unitForOption.MaxAge = thisUnit.restrictions.maxAge;
            unitForOption.MinAge = thisUnit.restrictions.minAge;
            unitForOption.MaxQuantity = (thisUnit.restrictions.maxQuantity != 0 ? thisUnit.restrictions.maxQuantity : 99);
            unitForOption.MinQuantity = (thisUnit.restrictions.minQuantity != 0 ? thisUnit.restrictions.minQuantity : 1);
            unitForOption.IdRequired = thisUnit.restrictions.idRequired;
            unitForOption.SubTitle = thisUnit.subtitle;

            for (var count = 0; count < thisUnit.restrictions?.accompaniedBy?.Count; count++)
            {
                if (count == thisUnit.restrictions.accompaniedBy.Count - 1)
                    unitForOption.AccompaniedBy = unitForOption.AccompaniedBy + thisUnit.restrictions.accompaniedBy[count];
                else
                    unitForOption.AccompaniedBy = unitForOption.AccompaniedBy + thisUnit.restrictions.accompaniedBy[count] + "###";
            }
            return unitForOption;
        }

        private Entities.Ventrata.Option GetOptionsData(VentrataEntities.Response.ProductRes product,
            OptionFP thisOpt)

        {
            var option = new Entities.Ventrata.Option();
            option.ProductId = product.id;
            option.OptionId = thisOpt.id;
            option.IsDefault = thisOpt._default;
            option.InternalName = thisOpt.internalName;
            option.CancellationCutOff = thisOpt.cancellationCutoff;
            option.CancellationCutOffAmount = thisOpt.cancellationCutoffAmount;
            option.CancellationCutOffUnit = thisOpt.cancellationCutoffUnit;

            return option;
        }

        private Entities.Ventrata.Destination GetDestinations(
            VentrataEntities.Response.ProductRes product)
        {

            var destination = new Entities.Ventrata.Destination();
            destination.ProductId = product.id;
            destination.DestinationId = product.destination?.id;
            destination.DestinationName = product.destination?.name;
            destination.Latitude = product.destination?.latitude?.ToString();
            destination.Longitude = product.destination?.longitude?.ToString();
            destination.Country = product.destination?.country;

            return destination;
        }

        private Entities.Ventrata.ProductDetail GetProductDetail(
                VentrataEntities.Response.ProductRes product)
        {
            //Add Product Details
            var productDetail = new Entities.Ventrata.ProductDetail();
            //productDetail.Inclusions = new List<string>();
            //productDetail.Exclusions = new List<string>();
            productDetail.ProductId = product.id;
            for (var count = 0; count < product?.inclusions?.Count; count++)
            {
                if (count == product.inclusions.Count - 1)
                    productDetail.Inclusions = productDetail.Inclusions + product.inclusions[count];
                else
                    productDetail.Inclusions = productDetail.Inclusions + product.inclusions[count] + "###";
            }
            for (var count = 0; count < product?.exclusions?.Count; count++)
            {
                if (count == product.exclusions.Count - 1)
                    productDetail.Exclusions = productDetail.Exclusions + product.exclusions[count];
                else
                    productDetail.Exclusions = productDetail.Exclusions + product.exclusions[count] + "###";
            }
            productDetail.CancellationPolicy = product.cancellationPolicy;
            productDetail.Supplierid = product.SupplierId;
            productDetail.ProductName = product.internalName;
            productDetail.isPackage = product.isPackage;

            return productDetail;
        }

        private Entities.Ventrata.PackageInclude GetPackageInclude(Include getInclude,
            Packageinclude include,
            string productParentID,
            string optionParentID)
        {
            var packageInclude = new Entities.Ventrata.PackageInclude
            {
                PackageIncludeCount = include.count,
                PackageIncludeTitle = include.title,

                limit = getInclude.limit,
                required = getInclude.required,

                PackageIncludeId = getInclude.id,
                PackageIncludeProductId = getInclude.productId,
                PackageIncludeOptionId = getInclude.optionId,

                ParentProductId = productParentID,
                ParentOptionId = optionParentID,
            };
            return packageInclude;
        }

        /// <summary>
        /// Get bokun age group, cancellation policy and rates from bokun activity response.
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private BokunAgeGroupWrapper GetBokunAgeGroups(List<IsangoHBProductMapping> products, string token)
        {
            if (products == null) return null;
            var results = new List<GetActivityRs>();

            var bokunProducts = new List<Entities.Bokun.Product>();
            var bokunCancellationPolicies = new List<CancellationPolicy>();
            var rates = new List<Isango.Entities.Bokun.Rate>();
            var bookableExtras = new List<Entities.Bokun.BookableExtras>();
            var bokunProductMappings = products.Where(x => x?.ApiType.Equals(APIType.Bokun) == true).ToList();

            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            var apiActivityIds = products.Where(y => !string.IsNullOrWhiteSpace(y.HotelBedsActivityCode)).Select(x => x.HotelBedsActivityCode)?.Distinct()?.ToList();

            foreach (var apiactivityid in apiActivityIds)
            {
                try
                {
                    var result = _bokunAdapter.GetActivity(apiactivityid, token);
                    if (result != null)
                        results.Add(result);
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                                _log.Error(new Isango.Entities.IsangoErrorEntity
                                {
                                    ClassName = _className,
                                    MethodName = "GetBokunAgeGroups-GetActivityDetail from API",
                                    Token = token,
                                    Params = Util.SerializeDeSerializeHelper.Serialize(apiactivityid)
                                }, ex)
                    );
                }
            }

            //Parallel.ForEach(apiActivityIds, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, apiActivityId =>
            //  {
            //      try
            //      {
            //          var result = _bokunAdapter.GetActivity(apiActivityId, token);
            //          if (result != null)
            //          {
            //              results.Add(result);
            //          }
            //      }
            //      catch (Exception ex)
            //      {
            //          Task.Run(() =>
            //                      _log.Error(new Isango.Entities.IsangoErrorEntity
            //                      {
            //                          ClassName = _className,
            //                          MethodName = "GetBokunAgeGroups-GetActivityDetail from API",
            //                          Token = token,
            //                          Params = Util.SerializeDeSerializeHelper.Serialize(apiActivityId)
            //                      }, ex)
            //          );
            //      }
            //  });

            //Parallel.ForEach(bokunProductMappins.ToList(), new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, product =>
            //{
            foreach (var product in bokunProductMappings.ToList())
            {
                var serviceId = product.IsangoHotelBedsActivityId;
                int.TryParse(product.HotelBedsActivityCode, out var apiActivityCode);
                var result = results.Where(x => x.Id == apiActivityCode)?.FirstOrDefault();

                try
                {
                    if (result != null)
                    {
                        var extraConfigs = new List<Entities.Bokun.ExtraConfigs>();
                        foreach (var priceCategory in result?.PricingCategories?.ToList())
                        {
                            try
                            {
                                var isYouthAsAdult = (priceCategory?.Title?.ToLower() == "young") && (priceCategory?.TicketCategory?.ToLower() == "adult");
                                var priceCategoryTitle = isYouthAsAdult ? "Youth" : priceCategory?.Title;
                                var ticketCategory = isYouthAsAdult ? "YOUTH" : priceCategory?.TicketCategory;

                                var bokunActivity = new Entities.Bokun.Product
                                {
                                    ProductId = Convert.ToInt32(result.Id),
                                    VendorId = result?.Vendor?.Id,
                                    VendorTitle = result?.Title,
                                    VendorCurrency = result?.Vendor?.CurrencyCode,
                                    PassporTrequired = result?.PassportRequired,
                                    Inclusions = result.Included,
                                    Exclusions = result?.Excluded,
                                    Attention = result?.Attention,
                                    PriceCategoryId = Convert.ToInt32(priceCategory?.Id),

                                    PriceCategoryTitle = priceCategoryTitle,
                                    PriceCategoryTicketCategory = ticketCategory,

                                    PriceCategoryOccupancy = priceCategory?.Occupancy,
                                    PriceCategoryAgeQualified = priceCategory?.AgeQualified,
                                    PriceCategoryMinAge = priceCategory?.MinAge,
                                    PriceCategoryMaxAge = priceCategory?.MaxAge,
                                    PriceCategoryDependent = priceCategory?.Dependent,
                                    PriceCategoryMasterCategoryId = priceCategory?.MasterCategoryId,
                                    PriceCategoryMaxPerMaster = priceCategory?.MaxPerMaster,
                                    PriceCategorySumDependentCategories = priceCategory?.SumDependentCategories,
                                    PriceCategoryMaxDependentSum = priceCategory?.MaxDependentSum,
                                    PriceCategoryInternalUseOnly = priceCategory?.InternalUseOnly,
                                    PriceCategoryDefaultCategory = priceCategory?.DefaultCategory,
                                    PriceCategoryFullTitle = priceCategory?.FullTitle,
                                    ActivityMinAge = result?.MinAge,
                                    IsangoServiceID = serviceId,
                                    BookingType = result?.BookingType
                                };

                                if (bokunProducts?.Any(x =>
                                     x.ProductId == bokunActivity.ProductId &&
                                     x.VendorId == bokunActivity.VendorId &&
                                     x.VendorTitle == bokunActivity.VendorTitle &&
                                     x.VendorCurrency == bokunActivity.VendorCurrency &&
                                     x.PassporTrequired == bokunActivity.PassporTrequired &&
                                     x.Inclusions == bokunActivity.Inclusions &&
                                     x.Exclusions == bokunActivity.Exclusions &&
                                     x.Attention == bokunActivity.Attention &&
                                     x.PriceCategoryId == bokunActivity.PriceCategoryId &&
                                     x.PriceCategoryTitle == bokunActivity.PriceCategoryTitle &&
                                     x.PriceCategoryTicketCategory == bokunActivity.PriceCategoryTicketCategory &&
                                     x.PriceCategoryOccupancy == bokunActivity.PriceCategoryOccupancy &&
                                     x.PriceCategoryAgeQualified == bokunActivity.PriceCategoryAgeQualified &&
                                     x.PriceCategoryMinAge == bokunActivity.PriceCategoryMinAge &&
                                     x.PriceCategoryMaxAge == bokunActivity.PriceCategoryMaxAge &&
                                     x.PriceCategoryDependent == bokunActivity.PriceCategoryDependent &&
                                     x.PriceCategoryMasterCategoryId == bokunActivity.PriceCategoryMasterCategoryId &&
                                     x.PriceCategoryMaxPerMaster == bokunActivity.PriceCategoryMaxPerMaster &&
                                     x.PriceCategorySumDependentCategories == bokunActivity.PriceCategorySumDependentCategories &&
                                     x.PriceCategoryMaxDependentSum == bokunActivity.PriceCategoryMaxDependentSum &&
                                     x.PriceCategoryInternalUseOnly == bokunActivity.PriceCategoryInternalUseOnly &&
                                     x.PriceCategoryDefaultCategory == bokunActivity.PriceCategoryDefaultCategory &&
                                     x.PriceCategoryFullTitle == bokunActivity.PriceCategoryFullTitle &&
                                     x.ActivityMinAge == bokunActivity.ActivityMinAge &&
                                     x.IsangoServiceID == bokunActivity.IsangoServiceID &&
                                     x.BookingType == bokunActivity.BookingType
                                    ) != true)
                                {
                                    bokunProducts.Add(bokunActivity);
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = _className,
                                    MethodName = string.Format("GetBokunAgeGroups ||  Error while PriceCategory :- {2}\n{0}\n{1}", ex.Message, ex.StackTrace, "Isango ServiceId : " + serviceId + ", Bokun ActivityId : " + apiActivityCode),
                                    Token = token
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }

                        if (result?.CancellationPolicy?.PenaltyRules?.Count > 0)
                        {
                            foreach (var cancellationPolicy in result.CancellationPolicy.PenaltyRules)
                            {
                                try
                                {
                                    var policy = new CancellationPolicy
                                    {
                                        APIServiceId = Convert.ToInt32(result.Id),
                                        CancellationPolicyId = Convert.ToInt32(result.CancellationPolicy.Id),
                                        Title = Convert.ToString(result.CancellationPolicy.Title),
                                        Charge = Convert.ToDecimal(cancellationPolicy.Charge),
                                        ChargeType = cancellationPolicy.ChargeType,
                                        CutOffHours = cancellationPolicy.CutoffHours,
                                        PenaltyRuleId = Convert.ToInt32(cancellationPolicy.Id),
                                        IsangoServiceID = serviceId
                                    };
                                    bokunCancellationPolicies.Add(policy);
                                }
                                catch (Exception ex)
                                {
                                    var isangoErrorEntity = new IsangoErrorEntity
                                    {
                                        ClassName = _className,
                                        MethodName = string.Format("GetBokunAgeGroups || Error while cancellation policy :- {2}\n{0}\n{1}", ex.Message, ex.StackTrace, "Isango ServiceId : " + serviceId + ", Bokun ActivityId : " + apiActivityCode),
                                        Token = token
                                    };
                                    _log.Error(isangoErrorEntity, ex);
                                }
                            }

                        }
                        if (result?.CancellationPolicy?.policyType?.ToUpper() == "NON_REFUNDABLE")
                        {
                            var policyW = new CancellationPolicy
                            {
                                APIServiceId = Convert.ToInt32(result.Id),
                                CancellationPolicyId = Convert.ToInt32(result.CancellationPolicy.Id),
                                Title = Convert.ToString(result.CancellationPolicy.Title),
                                Charge = 0,
                                ChargeType = "",
                                CutOffHours = 0,
                                PenaltyRuleId = 0,
                                IsangoServiceID = serviceId
                            };
                            bokunCancellationPolicies.Add(policyW);
                        }

                        foreach (var rate in result?.Rates?.ToList())
                        {
                            int.TryParse(product.HotelBedsActivityCode, out var supplierActivityId);
                            int.TryParse(product.PrefixServiceCode, out var mappedRateId);

                            try
                            {
                                var bokunActivityRate = new Entities.Bokun.Rate
                                {
                                    ServiceId = product.IsangoHotelBedsActivityId,
                                    ServiceOptionId = product.ServiceOptionInServiceid,
                                    SupplierActivityId = supplierActivityId,
                                    MappedRateId = mappedRateId > 0 && mappedRateId == Convert.ToInt32(rate.id) ? mappedRateId : 0,
                                    RateId = rate.id ?? 0,
                                    Title = rate.title,
                                    Description = rate.description,
                                    StartTimeIds = SerializeDeSerializeHelper.Serialize(rate.startTimeIds),
                                    PricingCategoryIds = SerializeDeSerializeHelper.Serialize(rate.pricingCategoryIds),
                                    MaxPerBooking = rate?.maxPerBooking > 0 ? Convert.ToInt32(rate?.maxPerBooking) : 99,
                                    MinPerBooking = rate.minPerBooking ?? 1,
                                    /*
                                    AllPricingCategories = rate.allPricingCategories ?? false,
                                    AllStartTimes = rate.allStartTimes ?? false,
                                    Details = SerializeDeSerializeHelper.Serialize(rate.details),
                                    DropoffPricedPerPerson = rate.dropoffPricedPerPerson ?? false,
                                    DropoffPricingType = rate.dropoffPricingType,
                                    DropoffSelectionType = rate.dropoffSelectionType,
                                    ExtraConfigs = SerializeDeSerializeHelper.Serialize(rate.extraConfigs),
                                    FixedPassExpiryDate = SerializeDeSerializeHelper.Serialize(rate.fixedPassExpiryDate),
                                    Index = rate.index ?? 0,
                                    PassValidForDays = rate.passValidForDays ?? 0,
                                    PickupPricedPerPerson = rate.pickupPricedPerPerson ?? false,
                                    PickupPricingType = rate.pickupPricingType,
                                    PickupSelectionType = rate.pickupSelectionType,
                                    PricedPerPerson = rate.pricedPerPerson ?? false,
                                    RateCode = SerializeDeSerializeHelper.Serialize(rate.rateCode),
                                    TieredPricingEnabled = rate.tieredPricingEnabled ?? false,
                                    Tiers = SerializeDeSerializeHelper.Serialize(rate.tiers),
                                    */
                                };

                                if (rate?.extraConfigs?.Count > 0)
                                {
                                    foreach (var item in rate?.extraConfigs)
                                    {
                                        var tempExtraConfig = new Entities.Bokun.ExtraConfigs()
                                        {
                                            ServiceID = product.IsangoHotelBedsActivityId,
                                            ServiceOptionID = product.ServiceOptionInServiceid,
                                            ActivityExtraId = item?.activityExtraId,
                                            IsPricedPerPerson = item?.pricedPerPerson,
                                            SelectionType = item?.selectionType
                                        };
                                        extraConfigs.Add(tempExtraConfig);
                                    }
                                }

                                if (rates?.Any(x =>
                                               x.RateId == bokunActivityRate.RateId
                                            && x.ServiceId == product.IsangoHotelBedsActivityId
                                            && x.ServiceOptionId == product.ServiceOptionInServiceid
                                            ) != true)
                                {
                                    if (mappedRateId > 0
                                                        //true only for mapped rates
                                                        ? ((bokunActivityRate.MappedRateId == mappedRateId) ? true : false)
                                                        //for all rate where mapping is not upto rate level
                                                        : true
                                        )
                                    {
                                        rates.Add(bokunActivityRate);
                                    }
                                    else
                                    //Activties where out of 4 , 3 rates are mapped but 1 is pending.
                                    if (rates?.Any(x => x.RateId == bokunActivityRate.RateId) != true
                                    && bokunProductMappings?.Any(x => x.PrefixServiceCode == bokunActivityRate.RateId.ToString()) != true
                                    )
                                    {
                                        bokunActivityRate.MappedRateId = 0;
                                        bokunActivityRate.ServiceOptionId = 0;
                                        rates.Add(bokunActivityRate);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = _className,
                                    MethodName = string.Format("GetBokunAgeGroups ||  Error while Rate :- {2}\n{0}\n{1}", ex.Message, ex.StackTrace, "Isango ServiceId : " + serviceId + ", Bokun ActivityId : " + apiActivityCode),
                                    Token = token
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }

                        if (result?.BookableExtras?.Count > 0)
                        {
                            try
                            {
                                foreach (var bookextra in result?.BookableExtras?.ToList())
                                {
                                    var bookableExtra = new Entities.Bokun.BookableExtras()
                                    {
                                        ServiceID = product?.IsangoHotelBedsActivityId,
                                        ServiceOptionID = product?.ServiceOptionInServiceid,
                                        SelectionType = extraConfigs?.Where(x => x.ServiceID == product.IsangoHotelBedsActivityId
                                                                      && x.ServiceOptionID == product.ServiceOptionInServiceid
                                                                      && x.ActivityExtraId == bookextra?.id)?.FirstOrDefault()?.SelectionType,
                                        IsPricedPerPerson = extraConfigs?.Where(x => x.ServiceID == product.IsangoHotelBedsActivityId
                                                                      && x.ServiceOptionID == product.ServiceOptionInServiceid
                                                                      && x.ActivityExtraId == bookextra?.id)?.FirstOrDefault()?.IsPricedPerPerson,
                                        PricingType = bookextra?.pricingType,
                                        Id = bookextra?.id,
                                        ExternalId = bookextra?.externalId,
                                        Flags = SerializeDeSerializeHelper.Serialize(bookextra?.flags),
                                        Free = bookextra?.free,
                                        Included = bookextra?.included,
                                        IncreasesCapacity = bookextra?.increasesCapacity,
                                        Information = bookextra?.information,
                                        LimitByPax = bookextra?.limitByPax,
                                        MaxPerBooking = bookextra?.maxPerBooking,
                                        Price = bookextra?.price,
                                        PricingTypeLabel = bookextra?.pricingTypeLabel,
                                        ProductGroupId = bookextra?.productGroupId,
                                        Title = bookextra?.title,
                                        Questions = new List<Entities.Bokun.BookableExtraQuestions>()
                                    };

                                    if (bookextra?.questions?.Count > 0)
                                    {
                                        foreach (var ques in bookextra?.questions?.ToList())
                                        {
                                            var tempQuestion = new Entities.Bokun.BookableExtraQuestions()
                                            {
                                                Active = ques?.active,
                                                AnswerRequired = ques?.answerRequired,
                                                Flags = SerializeDeSerializeHelper.Serialize(ques?.flags),
                                                Id = ques?.id,
                                                Label = ques?.label,
                                                Options = ques?.options,
                                                Type = ques?.type
                                            };
                                            bookableExtra.Questions.Add(tempQuestion);
                                        }
                                    }

                                    if (bookableExtras?.Any(x =>
                                               x.Id == bookableExtra?.Id
                                            && x.ServiceID == product.IsangoHotelBedsActivityId
                                            && x.ServiceOptionID == product.ServiceOptionInServiceid
                                            ) != true)
                                    {
                                        bookableExtras.Add(bookableExtra);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = _className,
                                    MethodName = string.Format("GetBokunAgeGroups ||  Error while BookableExtras :- {2}\n{0}\n{1}", ex.Message, ex.StackTrace, "Isango ServiceId : " + serviceId + ", Bokun ActivityId : " + apiActivityCode),
                                    Token = token
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                                _log.Error(new Isango.Entities.IsangoErrorEntity
                                {
                                    ClassName = _className,
                                    MethodName = "GetBokunAgeGroups",
                                    Token = token,
                                    Params = Util.SerializeDeSerializeHelper.Serialize(product)
                                }, ex)
                    );
                }
            }
            //);
            var ageGroupWrapper = new BokunAgeGroupWrapper
            {
                Products = bokunProducts,
                CancellationPolicies = bokunCancellationPolicies,
                Rates = rates.OrderBy(x => x.SupplierActivityId).ThenBy(y => y.ServiceOptionId).ThenBy(z => z.RateId).ToList(),
                BookableExtras = bookableExtras
            };

            return ageGroupWrapper;
        }

        /// <summary>
        /// Get All APItude Data
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async void GetAPITudeDataAndSave(List<IsangoHBProductMapping> products, string token)
        {
            var language = new List<string>() { "en", "es", "de", "fr" };
            var contentMultiRs = new List<ActivitiesContent>();
            foreach (var itemLang in language)
            {
                try
                {
                    var criteriaRecords = _contentRecords;
                    //Take n records at a Time in for loop

                    for (int itemCriteria = 0; itemCriteria < products.Count; itemCriteria = (itemCriteria + criteriaRecords))
                    {
                        if (products[itemCriteria] != null)
                        {
                            var criteriaRecordsAtTime = products.Skip(itemCriteria).Take(criteriaRecords).ToList();
                            if (criteriaRecordsAtTime != null && criteriaRecordsAtTime.ToList().Count > 0)
                            {
                                var codes = new List<Code>();
                                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                                Parallel.ForEach(criteriaRecordsAtTime, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, item =>
                                {
                                    var code = new Code
                                    {
                                        ActivityCode = item.SupplierCode
                                    };
                                    codes.Add(code);
                                });

                                var criteria = new ContentMultiRq
                                {
                                    Codes = codes,
                                    Language = itemLang
                                };

                                var result = await _hbAdapter.ContentMultiAsync(criteria, token);
                                if (result != null && result?.ActivitiesContent != null)
                                {
                                    result?.ActivitiesContent.ForEach(x => x.Language = itemLang);
                                }
                                else
                                {
                                    continue;
                                }
                                contentMultiRs.AddRange(result?.ActivitiesContent);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("AgeGroupService|GetAPITudeDataAndSave", ex);
                    continue;
                }
            }
            _apiTudePersistence.SaveApiTudeContent(contentMultiRs);
        }

        private void GetAPITudeCalendarDataAndSave(List<IsangoHBProductMapping> products, string token)
        {
            var language = new List<string>() { "en", "es", "de", "fr" };
            var activity = new List<ServiceAdapters.HB.HB.Entities.Calendar.Activity>();
            foreach (var itemLang in language)
            {
                try
                {
                    var criteriaRecords = _calendarRecords;
                    //GroupBy MinAdult
                    var groupByMinAdult = products.OrderBy(x => x.MinAdultCount).GroupBy(x => x.MinAdultCount);

                    foreach (var groupItem in groupByMinAdult)
                    {
                        try
                        {
                            var count = groupItem.ToList().Count;

                            //Take n records at a Time in for loop
                            for (int itemCriteria = 0; itemCriteria < count; itemCriteria = (itemCriteria + criteriaRecords))
                            {
                                try
                                {
                                    if (groupItem.ToList()[itemCriteria] != null)
                                    {
                                        //
                                        var criteriaRecordsAtTime = groupItem?.ToList().Skip(itemCriteria).Take(criteriaRecords);

                                        if (criteriaRecordsAtTime != null && criteriaRecordsAtTime.ToList().Count > 0)
                                        {
                                            var _token = token;
                                            //Create Criteria Request For Calendar
                                            var daystoFecth = _daystoFecth;

                                            decimal temploopcount = 1;

                                            if (daystoFecth > 30)
                                            {
                                                temploopcount = Math.Ceiling(Convert.ToDecimal(daystoFecth / 30));
                                            }

                                            var tempactivities = new CalendarRs();

                                            var LstcriteriaRecordsAtTime = criteriaRecordsAtTime?.ToList();

                                            for (int i = 0; i < temploopcount; i++)
                                            {
                                                try
                                                {
                                                    if (tempactivities?.Activities?.Count > 0)
                                                    {
                                                        foreach (var tempactivity in criteriaRecordsAtTime?.ToList())
                                                        {
                                                            try
                                                            {
                                                                var tempcheckavailable = tempactivities?.Activities?.Find(x => x.Code == tempactivity.SupplierCode);
                                                                if (tempcheckavailable != null)
                                                                {
                                                                    var tempremove = criteriaRecordsAtTime?.ToList().Find(x => x.SupplierCode == tempcheckavailable.Code);
                                                                    LstcriteriaRecordsAtTime.Remove(tempremove);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                _log.Error("AgeGroupService|GetAPITudeCalendarDataAndSave", ex);
                                                                //ignored
                                                            }
                                                        }
                                                    }

                                                    if (LstcriteriaRecordsAtTime?.Count > 0)
                                                    {
                                                        tempactivities = null;
                                                        //daystoFecth = 3; //Uncomment line after Testing
                                                        var calendarRq = GetCalendarCriteriaRequest(LstcriteriaRecordsAtTime?.ToList(), daystoFecth, itemLang, i);
                                                        //Call Calendar API and Get Response
                                                        var result = new CalendarRs();
                                                        var tupleResponse = _hbAdapter.CalendarAsync(calendarRq, _token.ToString()).GetAwaiter().GetResult();
                                                        result = tupleResponse.Item2;
                                                        tempactivities = result;
                                                        if (result == null || result.Activities == null) continue;
                                                        result.Activities.ForEach(isangoSerice =>
                                                        {
                                                            isangoSerice.IsangoServiceId = groupItem?.ToList()?.Where(m => m.SupplierCode == isangoSerice.Code).Select(x => x.IsangoHotelBedsActivityId).FirstOrDefault();
                                                        });
                                                        var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                                                        Parallel.ForEach(result?.Activities, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, itemActivity =>
                                                        {
                                                            if (itemActivity != null && itemActivity.Content != null)
                                                            {
                                                                itemActivity.Content.Language = itemLang;
                                                                itemActivity.Language = itemLang;
                                                                activity.Add(itemActivity);
                                                            }
                                                        });
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    _log.Error("AgeGroupService|GetAPITudeCalendarDataAndSave", ex);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Error("AgeGroupService|GetAPITudeCalendarDataAndSave", ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("AgeGroupService|GetAPITudeCalendarDataAndSave", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("AgeGroupService|GetAPITudeCalendarDataAndSave", ex);
                    continue;
                }
            }
            //_apiTudePersistence.SaveApiTudeContent(contentMultiRs);
            _apiTudePersistence.SaveApiTudeContentCalendar(activity);
        }

        /// <summary>
        /// /// Calendar Filter Request: Get Calendar Criteria Request
        /// </summary>
        /// <param name="itemCriteria"></param>
        /// <returns></returns>

        private HotelbedCriteriaApitudeFilter GetCalendarCriteriaRequest(List<IsangoHBProductMapping> itemCriteria, int daysToFetch, string language, int i)
        {
            try
            {
                var filters = new List<Filters>();
                var filter = new Filters();
                var searchFilterItems = new List<SearchFilterItems>();
                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(itemCriteria, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, item =>
                {
                    var searchItems = new SearchFilterItems
                    {
                        Type = "service",
                        Value = item.SupplierCode
                    };
                    searchFilterItems.Add(searchItems);
                });

                filter.SearchFilterItems = searchFilterItems;
                filters.Add(filter);

                var tempdaystofetch = (i + 1) * 30;

                var tempdifference = tempdaystofetch - 30;

                var activityRq = new HotelbedCriteriaApitudeFilter
                {
                    Filters = filters,
                    CheckinDate = Convert.ToDateTime(DateTime.Now.AddDays(tempdifference).ToString("yyyy-MM-dd")),
                    CheckoutDate = Convert.ToDateTime((DateTime.Now.AddDays(tempdaystofetch)).ToString("yyyy-MM-dd")),
                    Language = language?.ToLowerInvariant(),
                    Ages = new Dictionary<PassengerType, int>(),
                    //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                    NoOfPassengers = new Dictionary<PassengerType, int>()
                };

                activityRq.NoOfPassengers.Add(PassengerType.Adult, itemCriteria[0].MinAdultCount);
                activityRq.Ages.Add(PassengerType.Adult, 30);

                return activityRq;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "GetCalendarCriteriaRequest",
                    Token = "GetCalendarCriteriaRequest-Token"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all Redeam Suppliers
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<SupplierData> GetRedeamSuppliersData(string token)
        {
            var supplierDataList = _redeamAdapter.GetSuppliers(token).Result;
            return supplierDataList;
        }
        private List<Isango.Entities.RedeamV12.SupplierData> GetRedeamV12SuppliersData(string token)
        {
            var supplierDataList = _redeamV12Adapter.GetSuppliers(token).Result;
            return supplierDataList;
        }

        /// <summary>
        /// Get all Redeam Products
        /// </summary>
        /// <param name="token"></param>
        /// <param name="supplierIds"></param>
        /// <returns></returns>
        private List<ProductData> GetRedeamProductsData(List<string> supplierIds, string token)
        {
            var productDataList = new List<ProductData>();
            foreach (var supplierId in supplierIds)
            {
                var criteria = new RedeamCriteria
                {
                    SupplierId = supplierId
                };
                var productData = _redeamAdapter.GetProducts(criteria, token).Result;

                if (productData == null) continue;
                productDataList.AddRange(productData);
            }
            return productDataList;
        }
        private List<Isango.Entities.RedeamV12.ProductData> GetRedeamV12ProductsData(List<string> supplierIds, string token)
        {
            var productDataList = new List<Isango.Entities.RedeamV12.ProductData>();
            foreach (var supplierId in supplierIds)
            {
                var criteria = new Isango.Entities.CanocalizationCriteria
                {
                    SupplierId = supplierId
                };
                var productData = _redeamV12Adapter.GetProducts(criteria, token).Result;

                if (productData == null) continue;
                productDataList.AddRange(productData);
            }
            return productDataList;
        }

        /// <summary>
        /// Get all Redeam Rates, Prices and TravelerTypes data
        /// </summary>
        /// <param name="token"></param>
        /// <param name="productData"></param>
        /// <returns></returns>
        private RatesWrapper GetRedeamRatesWrapperData(List<ProductData> productData, string token)
        {
            var ratesDataList = new List<RateData>();
            var priceDataList = new List<PriceData>();
            var passengerTypeDataList = new List<PassengerTypeData>();

            var productsBySupplierId = productData.GroupBy(x => x.SupplierId);
            foreach (var value in productsBySupplierId)
            {
                var productDataList = value.ToList();
                foreach (var data in productDataList)
                {
                    var criteria = new RedeamCriteria
                    {
                        SupplierId = value.Key,
                        ProductId = data.ProductId
                    };
                    var ratesWrapper = _redeamAdapter.GetRatesWrapper(criteria, token).Result;
                    if (ratesWrapper == null) continue;

                    ratesDataList.AddRange(ratesWrapper.Rates);
                    priceDataList.AddRange(ratesWrapper.Prices);
                    passengerTypeDataList.AddRange(ratesWrapper.TravelerTypes);
                }
            }

            var result = new RatesWrapper
            {
                Prices = priceDataList,
                Rates = ratesDataList,
                TravelerTypes = passengerTypeDataList
            };
            return result;
        }

        private Isango.Entities.RedeamV12.RatesWrapper GetRedeamV12RatesWrapperData(List<Isango.Entities.RedeamV12.ProductData> productData, string token)
        {
            var ratesDataList = new List<Isango.Entities.RedeamV12.RateData>();
            var priceDataList = new List<Isango.Entities.RedeamV12.PriceData>();
            var passengerTypeDataList = new List<Isango.Entities.RedeamV12.PassengerTypeData>();

            var productsBySupplierId = productData.GroupBy(x => x.SupplierId);
            foreach (var value in productsBySupplierId)
            {
                var productDataList = value.ToList();
                foreach (var data in productDataList)
                {
                    var criteria = new Isango.Entities.CanocalizationCriteria
                    {
                        SupplierId = value.Key,
                        ProductId = data.ProductId
                    };
                    var ratesWrapper = _redeamV12Adapter.GetRatesWrapper(criteria, token).Result;
                    if (ratesWrapper == null) continue;

                    ratesDataList.AddRange(ratesWrapper.Rates);
                    priceDataList.AddRange(ratesWrapper.Prices);
                    passengerTypeDataList.AddRange(ratesWrapper.TravelerTypes);
                }
            }

            var result = new Isango.Entities.RedeamV12.RatesWrapper
            {
                Prices = priceDataList,
                Rates = ratesDataList,
                TravelerTypes = passengerTypeDataList
            };
            return result;
        }

        /// <summary>
        /// Fetch the suppliers data and save it in the database
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<string> SaveRedeamSupplierData(string token)
        {
            var suppliers = GetRedeamSuppliersData(token);
            if (suppliers == null) return null;

            // Save the suppliers in the database
            _redeamPersistence.SaveSuppliers(suppliers);
            var supplierIds = suppliers.Select(x => x.SupplierId).ToList();
            return supplierIds;
        }
        private List<string> SaveRedeamV12SupplierData(string token)
        {
            var suppliers = (List<Isango.Entities.RedeamV12.SupplierData>)_icanocalizationService.GetAgeGroupData(token, null, Convert.ToString(ServiceAdapters.RedeamV12.RedeamV12.Entities.MethodType.GetSuppliers), APIType.RedeamV12);
            //GetRedeamV12SuppliersData();
            if (suppliers == null) return null;

            // Save the suppliers in the database
            _redeamPersistence.SaveSuppliersV12(suppliers);
            var supplierIds = suppliers.Select(x => x.SupplierId).ToList();
            return supplierIds;
        }

        /// <summary>
        /// Fetch the products data for the given supplier Ids and save it in the database
        /// </summary>
        /// <param name="supplierIds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<ProductData> SaveRedeamProductsData(List<string> supplierIds, string token)
        {
            // Fetch the products data for the given supplier Ids
            var products = GetRedeamProductsData(supplierIds, token);
            if (products == null) return null;

            // Save the products in the database
            _redeamPersistence.SaveProducts(products);
            return products;
        }
        private List<Isango.Entities.RedeamV12.ProductData> SaveRedeamV12ProductsData(List<string> supplierIds, string token)
        {
            // Fetch the products data for the given supplier Ids
            var products = //GetRedeamV12ProductsData(supplierIds, token);
                (List<Isango.Entities.RedeamV12.ProductData>)_icanocalizationService.GetAgeGroupData(token, supplierIds, Convert.ToString(ServiceAdapters.RedeamV12.RedeamV12.Entities.MethodType.GetProducts), APIType.RedeamV12);
            if (products == null) return null;

            // Save the products in the database
            _redeamPersistence.SaveProductsV12(products);
            return products;
        }

        /// <summary>
        /// Fetch the rates, price and age group data and save it in the database
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        private void SaveRedeamRatesData(List<ProductData> products, string token)
        {
            var ratesWrapper = GetRedeamRatesWrapperData(products, token);

            // Save the rates in the database
            _redeamPersistence.SaveRates(ratesWrapper.Rates);

            // Save the prices in the database
            _redeamPersistence.SavePrices(ratesWrapper.Prices);

            // Save the age groups in the database
            _redeamPersistence.SaveAgeGroups(ratesWrapper.TravelerTypes);
        }
        private void SaveRedeamV12RatesData(List<Isango.Entities.RedeamV12.ProductData> products, string token)
        {
            var ratesWrapper = //GetRedeamV12RatesWrapperData(products, token);
                 (Isango.Entities.RedeamV12.RatesWrapper)_icanocalizationService.GetAgeGroupData(token, products, Convert.ToString(ServiceAdapters.RedeamV12.RedeamV12.Entities.MethodType.GetRates), APIType.RedeamV12);
            // Save the rates in the database
            _redeamPersistence.SaveRatesV12(ratesWrapper.Rates);

            // Save the prices in the database
            _redeamPersistence.SavePricesV12(ratesWrapper.Prices);

            // Save the age groups in the database
            _redeamPersistence.SaveAgeGroupsV12(ratesWrapper.TravelerTypes);
        }

        /// <summary>
        /// AgeGroupProductMapper
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private AgeGroupProductMapper GetRezdyProducts(string token)
        {
            var ageGroupProductMapper = new AgeGroupProductMapper();
            try
            {
                var rezdyProductDetails = new List<RezdyProductDetail>();
                var ageGroupMappings = new List<AgeGroupMapping>();
                var bookingFields = new List<BookingFieldMapping>();
                var prodExtraDetailsList = new List<ProductWiseExtraDetails>();

                //fetch all isango supported rezdy supplier
                var rezdySuppliersId = ConfigurationManagerHelper.GetValuefromAppSettings("SupplierIds");//.Split(',');

                //foreach (var supplierId in rezdySuppliersId)
                //{
                //Get products for each supplier
                var supplierId = rezdySuppliersId;
                var rezdyProducts = _rezdyAdapter.GetAllRezdyProducts(Convert.ToInt32(supplierId), string.Empty, token)?.GetAwaiter().GetResult();
                //From API , we get below AgentPaymentType:
                //1.PAYOUTS, 2.FULL_AGENT, 3.DOWNPAYMENT, 4.FULL_SUPPLIER, 5.NONE

                foreach (var product in rezdyProducts)
                {
                    try
                    {
                        if (product?.AgentPaymentType.ToLowerInvariant() != "full_agent")
                        {
                            continue;
                        }
                        if (product.PriceOptions != null)
                        {
                            ageGroupMappings.AddRange(product?.PriceOptions?.Select(
                                item => new AgeGroupMapping
                                {
                                    SupplierOptionId = item.Id,
                                    Label = item?.Label,
                                    MaxQuantity = item.MaxQuantity,
                                    MinQuantity = item.MinQuantity,
                                    PriceGroupType = item?.PriceGroupType,
                                    ProductCode = item?.ProductCode,
                                    SeatsUsed = item.SeatsUsed
                                }
                            ));
                        }

                        if (product.Extras != null)
                        {
                            foreach (var extraDet in product.Extras?.ToList())
                            {
                                prodExtraDetailsList.Add(new ProductWiseExtraDetails
                                {
                                    ProductCode = product.ProductCode,
                                    Name = extraDet.Name,
                                    Description = extraDet.Description,
                                    Price = extraDet.Price,
                                    ExtraPriceType = extraDet.ExtraPriceType
                                });
                            }
                        }

                        var rezdyProductDetail = new RezdyProductDetail
                        {
                            CancellationPolicyDays = Convert.ToInt32(product?.CancellationPolicyDays),
                            Currency = product?.Currency,
                            Description = product?.Description,
                            ShortDescription = product?.ShortDescription,
                            PickUpId = Convert.ToInt32(product?.PickupId),
                            ProductCode = product?.ProductCode,
                            ProductName = product?.Name,
                            SupplierAlias = product?.SupplierAlias,
                            ProductType = product?.ProductType,
                            QuantityRequired = Convert.ToBoolean(product?.QuantityRequired),
                            QuantityRequiredMax = product.QuantityRequiredMax != 0 ? product.QuantityRequiredMax : 99,
                            QuantityRequiredMin = product.QuantityRequiredMin != 0 ? product.QuantityRequiredMin : 1,
                            SupplierId = Convert.ToInt32(product?.SupplierId),
                            SupplierName = product?.SupplierName,
                            InternalCode = product?.InternalCode
                        };

                        bookingFields?.AddRange(product?.BookingFields?.Select(
                            item => new BookingFieldMapping
                            {
                                ProductCode = product?.ProductCode,
                                Label = item?.Label,
                                VisiblePerBooking = item.VisiblePerBooking,
                                RequiredPerBooking = item.RequiredPerBooking,
                                VisiblePerParticipant = item.VisiblePerParticipant,
                                RequiredPerParticipant = item.RequiredPerParticipant
                            }
                        ));

                        rezdyProductDetails.Add(rezdyProductDetail);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("AgeGroupService|GetRezdyProducts(rezdyProducts)", ex);
                        continue;
                    }
                }
                //}

                ageGroupProductMapper = new AgeGroupProductMapper
                {
                    RezdyProductDetails = rezdyProductDetails,
                    AgeGroupMappings = ageGroupMappings,
                    BookingFields = bookingFields,
                    ListOfProductWiseExtraDetails = prodExtraDetailsList
                };
            }
            catch (Exception ex)
            {
                _log.Error("AgeGroupService|GetRezdyProducts", ex);
            }
            return ageGroupProductMapper;
        }
        public void csswebjob()
        {
            _cssbookingservice.ProcessCssIncompleteBooking();
        }

        public void csswebjobCancellation()
        {
            _cssbookingservice.ProcessIncompleteCancellation();
        }
        public void csswebjobRedemption()
        {
            _cssbookingservice.ProcessIncompleteRedemption();
        }
        public void SaveCssRedemptionBooking(string tokenId)
        {
            var criteria = new TourCMSRedemptionCriteria
            {
                CheckinDate = DateTime.Now.AddMonths(-2),
                CheckoutDate = DateTime.Now
            };
            var redemptionDataList = new List<RedemptionBooking>();

            try
            {
                var request = string.Empty;
                var response = string.Empty;

                var channelIds = _tourCMSPersistence.GetTourCmsChannelId();
                if (channelIds != null)
                {
                    //foreach (var channelId in channelIds)
                    Parallel.ForEach(channelIds, channelId =>
                    {
                        // Set the current channel ID in the criteria
                        criteria.ChannelId = channelId;

                        int currentPage = 1;
                        var channelRedemptionDataList = new List<RedemptionBooking>();

                        while (true)
                        {
                            criteria.page = currentPage;
                            var redemptionData = _tourCMSAdapter.RedemptionBookingData(criteria, tokenId, out request, out response);

                            if (redemptionData == null || redemptionData.Bookings == null || redemptionData.Bookings.Bookings.Count == 0)
                            {
                                // No more data to fetch for this channel, exit the loop
                                break;
                            }

                            // Add the redemptionData to the channel-specific list
                            channelRedemptionDataList.AddRange(redemptionData.Bookings.Bookings);

                            currentPage++;
                        }

                        // Lock and add the channel-specific data to the main list
                        lock (redemptionDataList)
                        {
                            redemptionDataList.AddRange(channelRedemptionDataList);
                        }
                    });
                    //}
                }

                // After all channels are processed, insert the data into the database
                if (redemptionDataList.Count > 0)
                {
                    _redemptionService.TourCmsRedemptionService(redemptionDataList);
                    string redemptionDataString = JsonConvert.SerializeObject(redemptionDataList);
                    _tourCMSPersistence.InsertRedemptionData(redemptionDataString);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = "CssRedemptionBooking",
                    Token = "GetCalendarCriteriaRequest-Token"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }



        #endregion Private Methods
    }
}