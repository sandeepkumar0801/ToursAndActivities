using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Availability;
using Isango.Entities.ElasticData;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.GoldenTours;
using Isango.Entities.HotelBeds;
using Isango.Entities.PricingRules;
using Isango.Entities.Region;
using Isango.Entities.Rezdy;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Util;
using Activity = Isango.Entities.Activities.Activity;
using MappedLanguage = Isango.Entities.MappedLanguage;

namespace Isango.Service
{
    public class CacheLoaderService : ICacheLoaderService
    {
        #region Variables

        private readonly IMasterPersistence _masterPersistence;
        private readonly IActivityPersistence _activityPersistence;
        private readonly IAffiliatePersistence _affiliatePersistence;
        private readonly IAgeGroupsCacheManager _ageGroupsCacheManager;
        private readonly IHbProductMappingCacheManager _hbProductMappingCacheManager;
        private readonly IMemCache _memCache;
        private readonly INetPriceCacheManager _netPriceCacheManager;
        private readonly ISimilarProductsRegionAttractionCacheManager _similarProductsRegionAttractionCacheManager;
        private readonly IHotelBedsActivitiesCacheManager _hotelBedsActivitiesCacheManager;
        private readonly ILogger _log;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IActivityCacheManager _activityCacheManager;
        private readonly IAffiliateCacheManager _affiliateCacheManager;
        private readonly IFareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManager;
        private readonly IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManager;
        private readonly IPriceRuleEnginePersistence _priceRuleEnginePersistence;
        private readonly IPricingRulesCacheManager _pricingRulesCacheManager;
        private readonly ICalendarAvailabilityCacheManager _calendarAvailabilityCacheManager;
        private readonly IPickupLocationsCacheManager _pickupLocationsCacheManager;
        private readonly ITiqetsPaxMappingCacheManager _tiqetsPaxMappingCacheManager;
        private readonly IMailerService _mailerService;
        private readonly IGoogleMapsPersistence _googleMapsPersistence;
        private readonly int _maxParallelThreadCount;
        private static string collectionDatabase = ConfigurationManagerHelper.GetValuefromAppSettings("CollectionDataBase");
        #endregion Variables

        #region Constructor

        public CacheLoaderService(
          IMasterPersistence masterPersistence,
          IActivityPersistence activityPersistence,
          IAffiliatePersistence affiliatePersistence,
          IAgeGroupsCacheManager ageGroupsCacheManager,
          IHbProductMappingCacheManager hbProductMappingCacheManager,
          IHotelBedsActivitiesCacheManager hotelBedsActivitiesCacheManager,
          IMemCache memCache,
          INetPriceCacheManager netPriceCacheManager,
          ISimilarProductsRegionAttractionCacheManager similarProductsRegionAttractionCacheManager,
          IMasterCacheManager masterCacheManager,
          IActivityCacheManager activityCacheManager,
          IAffiliateCacheManager affiliateCacheManager,
          IFareHarborCustomerPrototypesCacheManager fareHarborCustomerPrototypesCacheManager,
          IFareHarborUserKeysCacheManager fareHarborUserKeysCacheManager,
          IPriceRuleEnginePersistence priceRuleEnginePersistence,
          IPricingRulesCacheManager pricingRulesCacheManager,
          ICalendarAvailabilityCacheManager calendarAvailabilityCacheManager,
          ILogger log,
          IPickupLocationsCacheManager pickupLocationsCacheManager,
          ITiqetsPaxMappingCacheManager tiqetsPaxMappingCacheManager,
          IGoogleMapsPersistence googleMapsPersistence, IMailerService mailerService = null
      )
        {
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Constant.MaxParallelThreadCount);
            _masterPersistence = masterPersistence;
            _activityPersistence = activityPersistence;
            _affiliatePersistence = affiliatePersistence;
            _ageGroupsCacheManager = ageGroupsCacheManager;
            _hbProductMappingCacheManager = hbProductMappingCacheManager;
            _memCache = memCache;
            _netPriceCacheManager = netPriceCacheManager;
            _similarProductsRegionAttractionCacheManager = similarProductsRegionAttractionCacheManager;
            _hotelBedsActivitiesCacheManager = hotelBedsActivitiesCacheManager;
            _masterCacheManager = masterCacheManager;
            _activityCacheManager = activityCacheManager;
            _affiliateCacheManager = affiliateCacheManager;
            _fareHarborCustomerPrototypesCacheManager = fareHarborCustomerPrototypesCacheManager;
            _fareHarborUserKeysCacheManager = fareHarborUserKeysCacheManager;
            _priceRuleEnginePersistence = priceRuleEnginePersistence;
            _pricingRulesCacheManager = pricingRulesCacheManager;
            _calendarAvailabilityCacheManager = calendarAvailabilityCacheManager;
            _log = log;
            _pickupLocationsCacheManager = pickupLocationsCacheManager;
            _tiqetsPaxMappingCacheManager = tiqetsPaxMappingCacheManager;
            _googleMapsPersistence = googleMapsPersistence;
            _mailerService = mailerService;
        }

        #endregion Constructor

        public async Task<bool> LoadGliAgeGroupByActivityAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var ageGroupByActivityList = _masterPersistence.GetGliAgeGroupsByActivity();

                if (ageGroupByActivityList != null)
                {
                    var failedInsertList = _ageGroupsCacheManager.LoadAgeGroupByActivity(ageGroupByActivityList);
                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadGliAgeGroupByActivityAsync|{failedInsertList}");
                    }
                    if (ageGroupByActivityList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnData = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadGliAgeGroupByActivityAsync", "Success"));
                        return returnData;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadGliAgeGroupByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadGliAgeGroupByActivityAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadAotAgeGroupByActivityAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var ageGroupByActivityList = _masterPersistence.GetAotAgeGroupsByActivity();

                if (ageGroupByActivityList != null)
                {
                    var failedInsertList = _ageGroupsCacheManager.LoadAgeGroupByActivity(ageGroupByActivityList);
                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadAotAgeGroupByActivityAsync|{failedInsertList}");
                    }
                    if (ageGroupByActivityList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnData = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadAotAgeGroupByActivityAsync", "Success"));
                        return returnData;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadAotAgeGroupByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadAotAgeGroupByActivityAsync", "Error:" + ex));
                throw;
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadPrioAgeGroupByActivityAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var ageGroupByActivityList = _masterPersistence.GetPrioAgeGroupsByActivity();

                if (ageGroupByActivityList != null)
                {
                    var failedInsertList = _ageGroupsCacheManager.LoadAgeGroupByActivity(ageGroupByActivityList);
                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadPrioAgeGroupByActivityAsync|{failedInsertList}");
                    }
                    if (ageGroupByActivityList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnData = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadPrioAgeGroupByActivityAsync", "Success"));
                        return returnData;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadPrioAgeGroupByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadPrioAgeGroupByActivityAsync", "Error:" + ex));
                throw;
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadFareHarborAgeGroupByActivityAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var ageGroupByActivityList = _masterPersistence.GetFareHarborAgeGroupsByActivity();

                if (ageGroupByActivityList != null)
                {
                    var failedInsertList = _ageGroupsCacheManager.LoadFhAgeGroupByActivity(ageGroupByActivityList);
                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadFareHarborAgeGroupByActivityAsync|{failedInsertList}");
                    }
                    if (ageGroupByActivityList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnData = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadFareHarborAgeGroupByActivityAsync", "Success"));
                        return returnData;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadAgeGroupByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadFareHarborAgeGroupByActivityAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadCacheMappingAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var isangoHBProductMappingList = _masterPersistence.LoadFactSheetMapping();

                if (isangoHBProductMappingList != null)
                {
                    var failedInsertList = _hbProductMappingCacheManager.LoadCacheMapping(isangoHBProductMappingList);
                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadCacheMappingAsync|{failedInsertList}");
                    }
                    if (isangoHBProductMappingList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnData = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadCacheMappingAsync", "Success"));
                        return returnData;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadCacheMappingAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadCacheMappingAsync", "Error:" + ex));
                throw;
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> RegionCategoryMappingAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var regionCategoryList = _activityPersistence.LoadRegionCategoryMapping();
                var regionCategoryGroup = regionCategoryList.GroupBy(x => x.Languagecode);
                foreach (var group in regionCategoryGroup)
                {
                    var languageCode = group.Key.ToLower();
                    try
                    {
                        if (regionCategoryList != null)
                        {
                            var regionCategoryCached = new CacheKey<RegionCategoryMapping>()
                            {
                                Id = languageCode.ToLower() == "en" ? Constant.RegionCategoryMapping : $"{Constant.RegionCategoryMapping}_{languageCode}",
                                CacheValue = group.ToList()
                            };
                            var returnValue = await Task.FromResult(_memCache.RegionCategoryMapping(regionCategoryCached));
                            mailErrorLog.Add(Tuple.Create($"CacheLoaderService|RegionCategoryMappingAsync|{languageCode}", "Success"));
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "CacheLoaderService",
                            MethodName = $"RegionCategoryMappingAsync_{languageCode}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        mailErrorLog.Add(Tuple.Create($"CacheLoaderService|RegionCategoryMappingAsync|{group.Key}", "Error:" + ex));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "RegionCategoryMappingAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|RegionCategoryMappingAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> RegionDestinationMappingAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var regionVsDestinationList = _masterPersistence.RegionVsDestination();

                if (regionVsDestinationList != null)
                {
                    var regionVsDestinationCached = new CacheKey<MappedRegion>()
                    {
                        Id = Constant.RegionVsDestination,
                        CacheValue = regionVsDestinationList
                    };

                    var returnValue = await Task.FromResult(_memCache.RegionDestinationMapping(regionVsDestinationCached));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|RegionDestinationMappingAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "RegionDestinationMappingAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|RegionDestinationMappingAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadMappedLanguageAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var mappedLanguageList = _masterPersistence.LoadMappedLanguage();

                if (mappedLanguageList != null)
                {
                    var mappedLanguageCached = new CacheKey<MappedLanguage>()
                    {
                        Id = Constant.MappedLanguage,
                        CacheValue = mappedLanguageList
                    };

                    var returnValue = await Task.FromResult(_memCache.LanguageCodeMapping(mappedLanguageCached));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadMappedLanguageAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadMappedLanguageAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadMappedLanguageAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadNetPriceMasterDataAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var netPriceDataList = _masterPersistence.LoadNetPriceMasterData();

                if (netPriceDataList != null)
                {
                    var failedInsertList = _netPriceCacheManager.LoadNetPriceMasterData(netPriceDataList);

                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadCacheMappingAsync|{failedInsertList}");
                    }
                    if (netPriceDataList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnData = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadNetPriceMasterDataAsync", "Success"));
                        return returnData;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadNetPriceMasterDataAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadNetPriceMasterDataAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadRegionCategoryMappingProductsAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var regionCategoryList = _masterPersistence.GetRegionCategoryMapping();

                if (regionCategoryList != null)
                {
                    var failedInsertList = _similarProductsRegionAttractionCacheManager.RegionCategoryMappingProducts(regionCategoryList);

                    if (failedInsertList != string.Empty)
                    {
                        _log.Warning($"CacheLoaderService|LoadCacheMappingAsync|{failedInsertList}");
                    }
                    if (regionCategoryList.Count > failedInsertList.Split(',').Count())
                    {
                        var returnValue = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadRegionCategoryMappingProductsAsync", "Success"));
                        return returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadRegionCategoryMappingProductsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadRegionCategoryMappingProductsAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task InsertOptionAvailability()
        {
            try
            {
                _activityPersistence.InsertOptionAvailability();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "InsertOptionAvailability"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method load the HotelBedAvailabilitycache and update activity cache for Base, GateBase and Cost price
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadAvailabilityCacheAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var isangoAvailabilities = _activityPersistence.GetOptionAvailability();

                var availabilityList = CreateAvailabilityList(isangoAvailabilities);

                var failedInsertList = _hotelBedsActivitiesCacheManager.LoadAvailabilityCache(availabilityList);
                if (failedInsertList != string.Empty)
                {
                    _log.Warning($"CacheLoaderService|LoadAvailabilityCacheAsync|{failedInsertList}");
                }

                if (availabilityList.Count > failedInsertList.Split(',').Count())
                {
                    var returnValue = await Task.FromResult(true);
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadAvailabilityCacheAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadAvailabilityCacheAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadAvailabilityCacheAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        //Master Cache Manager calls

        public async Task<bool> LoadCurrencyExchangeRatesAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var currencyExchangeRates = _masterPersistence.LoadExchangeRates();
                if (currencyExchangeRates != null)
                {
                    var prepareCacheData = new CacheKey<CurrencyExchangeRates>
                    {
                        Id = Constant.CurrencyExchangeRate,
                        CacheValue = currencyExchangeRates
                    };
                    var returnValue = await Task.FromResult(_masterCacheManager.LoadCurrencyExchangeRate(prepareCacheData));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadCurrencyExchangeRatesAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadCurrencyExchangeRatesAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadCurrencyExchangeRatesAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> ClearMongoSessions()
        {
            try
            {
                if (ConfigurationManagerHelper.GetValuefromAppSettings("IsMongoCacheEnabledForPhoenixSession") == "1")
                {
                    var collectionName = ConfigurationManagerHelper.GetValuefromAppSettings("SessionDataCollection");
                    var dateTime = DateTime.UtcNow;
                    var dateTimeOffset = new DateTimeOffset(dateTime);
                    var timeStamp = dateTimeOffset.ToUnixTimeSeconds();
                    var returnValue = await Task.FromResult(_masterCacheManager.DeleteOlderDocument(collectionName, timeStamp));
                    return await Task.FromResult(true);
                }
                if (ConfigurationManagerHelper.GetValuefromAppSettings("IsRedisCacheEnabledForPhoenixSession") == "1")
                {
                    RedixManagement.RemoveInactiveSessions();
                    return await Task.FromResult(true);

                }

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "ClearMongoSessions"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return await Task.FromResult(false);
        }

        //Load Activity collection

        public async Task<bool> LoadActivitiesCollectionAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var languages = _masterPersistence.GetSupportedLanguages();

                var passengerInfoList = _activityPersistence.GetPassengerInfoDetails();
                // Get option availability
                var isangoAvailabilities = _activityPersistence.GetOptionAvailability();

                var availabilityList = CreateAvailabilityList(isangoAvailabilities);

                var failedInsertList = _hotelBedsActivitiesCacheManager.LoadAvailabilityCache(availabilityList);

                if (failedInsertList.Length > 0)
                {
                    _log.Warning($"CacheLoaderService|LoadAvailabilityCache|{failedInsertList}");
                }

                Parallel.ForEach(languages, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (language) =>
                {
                    if (_activityCacheManager.DeleteAndCreateCollection(language.Code))
                    {
                        var activityIds = _activityPersistence.GetLiveActivityIds(language.Code);

                        var activityIdsList = activityIds != null ? string.Join(",", activityIds) : string.Empty;

                        var liveActivitiesList = _activityPersistence.GetActivitiesByActivityIds(activityIdsList, language.Code);

                        var liveHbActivitiesList = _activityPersistence.LoadLiveHbActivities(0, language.Code);

                        var activityList = liveActivitiesList.Union(liveHbActivitiesList).ToList();

                        if (passengerInfoList.Count > 0)
                        {
                            foreach (var activity in activityList)
                            {
                                var passengerInfo = passengerInfoList.Where(x => x.ActivityId.ToString() == activity.Id).ToList();
                                activity.PassengerInfo = passengerInfo;
                            }
                        }

                        activityList = UpdateActivityDatePriceAvailability(activityList, isangoAvailabilities);

                        var activitiesWithLanguageMismatch = activityList.Where(x => x.LanguageCode.ToLowerInvariant() != language.Code.ToLowerInvariant()).ToList();
                        if (activitiesWithLanguageMismatch != null && activitiesWithLanguageMismatch.Count > 0)
                        {
                            _log.Warning($"CacheLoaderService|IncorrectLanguageAct|{SerializeDeSerializeHelper.Serialize(activitiesWithLanguageMismatch)},{language.Code}");
                        }

                        activityList.RemoveAll(x =>
                            x.LanguageCode.ToLowerInvariant() != language.Code.ToLowerInvariant());

                        var failedActivity = InsertDocumentInCache(activityList, language.Code.ToLower());

                        if (failedActivity.Length > 0)
                        {
                            _log.Warning($"CacheLoaderService|LoadActivitiesCollectionAsync|{failedActivity}");
                        }
                    }
                });
                var returnData = await Task.FromResult(true);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|RegionCategoryMappingAsync", "Success"));
                return returnData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadActivitiesCollectionAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadActivitiesCollectionAsync", "Error:" + ex));

                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
        }

        public async Task<bool> LoadSelectedActivitiesAsync(string activityIds)
        {
            try
            {
                var languages = _masterPersistence.GetSupportedLanguages();

                var passengerInfoList = _activityPersistence.GetPassengerInfoDetails(activityIds);

                var isangoAvailabilities = _activityPersistence.GetOptionAvailability(string.Empty, activityIds);

                foreach (var language in languages)
                {
                    var liveActivitiesList = _activityPersistence.GetActivitiesByActivityIds(activityIds, language.Code);

                    var liveHbActivitiesList = new List<Activity>();

                    foreach (var activityId in activityIds.Split(','))
                    {
                        try
                        {
                            var activity = _activityPersistence.LoadLiveHbActivities(Convert.ToInt32(activityId), language.Code).FirstOrDefault();
                            if (activity != null)
                                liveHbActivitiesList.Add(activity);
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "CacheLoaderService",
                                MethodName = "LoadSelectedActivitiesAsync"
                            };
                            _log.Error(isangoErrorEntity, ex);
                        }
                    }

                    var activityList = liveActivitiesList.Union(liveHbActivitiesList).ToList();

                    if (passengerInfoList.Count > 0)
                    {
                        foreach (var activity in activityList)
                        {
                            var passengerInfo = passengerInfoList.Where(x => x.ActivityId.ToString() == activity.Id).ToList();
                            activity.PassengerInfo = passengerInfo;
                        }
                    }

                    activityList = UpdateActivityDatePriceAvailability(activityList, isangoAvailabilities);

                    var failedActivity = InsertDocumentInCache(activityList, language.Code.ToLower());

                    var changedActivity = new ActivityChangeTracker()
                    {
                        ActivityId = activityList?.FirstOrDefault()?.ID ?? 0
                    };

                    var changedActivities = new List<ActivityChangeTracker>()
                    {
                        changedActivity
                    };

                    ClearActivityWebsiteCache(changedActivities);

                    if (failedActivity.Length > 0)
                    {
                        _log.Warning($"CacheLoaderService|LoadSelectedActivitiesAsync|{failedActivity}");
                    }
                }
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadSelectedActivitiesAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<bool> SetRegionAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var result = _masterPersistence.GetRegionData();
                if (result != null)
                {
                    var cacheResult = new CacheKey<LatLongVsurlMapping>
                    {
                        Id = Constant.GeoCoordinateMasterMappingKey,
                        CacheValue = result
                    };

                    var returnData = await Task.FromResult(_masterCacheManager.SetRegionData(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetRegionAsync", "Success"));
                    return returnData;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "SetRegionAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetRegionAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> SetAffiliateFiltersAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var result = _affiliatePersistence.GetAffiliateFilter();
                if (result != null)
                {
                    var cacheResult = new CacheKey<AffiliateFilter>
                    {
                        Id = Constant.AffiliateFilterCacheKey,
                        CacheValue = result
                    };

                    var returnData = await Task.FromResult(_affiliateCacheManager.SetAffiliateFilterToCache(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetAffiliateFiltersAsync", "Success"));
                    return returnData;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "SetAffiliateFiltersAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetAffiliateFiltersAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> SetAffiliateFilterAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                if (_affiliateCacheManager.DeleteAndCreateAffiliateFilterCollection())
                {
                    var result = _affiliatePersistence.GetAffiliateFilter();
                    if (result != null)
                    {
                        var returnValue = await Task.FromResult(_affiliateCacheManager.SetAffiliateToCache(result));
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetAffiliateAsync", "Success"));
                        return returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "SetAffiliateAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetAffiliateAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> SetFilteredTicketAsync()
        {
            try
            {
                var result = _masterPersistence.GetFilteredThemeparkTickets();
                if (result != null)
                {
                    var cacheResult = new CacheKey<Entities.TicketByRegion>
                    {
                        Id = Constant.FilteredTickets,
                        CacheValue = result
                    };

                    return await Task.FromResult(_masterCacheManager.SetFilteredTicketsToCache(cacheResult));
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "SetFilteredTicketAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return await Task.FromResult(false);
        }

        //Load FareHarbor data

        public async Task<bool> GetCustomerPrototypeByActivityAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var result = _masterPersistence.GetCustomerPrototypeByActivity();
                if (result != null)
                {
                    var cacheResult = new CacheKey<CustomerPrototype>
                    {
                        Id = Constant.CustomerPrototypeCacheKey,
                        CacheValue = result
                    };

                    var returnValue = await Task.FromResult(_fareHarborCustomerPrototypesCacheManager.SetCustomerPrototypeByActivityToCache(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|GetCustomerPrototypeByActivityAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "GetCustomerPrototypeByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|GetCustomerPrototypeByActivityAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> GetAllFareHarborUserKeysAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var result = _masterPersistence.GetAllFareHarborUserKeys();
                if (result != null)
                {
                    var cacheResult = new CacheKey<FareHarborUserKey>
                    {
                        Id = Constant.FareHarborUserCacheKey,
                        CacheValue = result
                    };

                    var returnValue = await Task.FromResult(_fareHarborUserKeysCacheManager.SetAllFareHarborUserKeysToCache(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|GetAllFareHarborUserKeysAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "GetAllFareHarborUserKeysAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|GetAllFareHarborUserKeysAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> SetUrlPageIdMappingMappingAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var result = _masterPersistence.UrlPageIdMappingList();
                if (result != null)
                {
                    var cacheResult = new CacheKey<UrlPageIdMapping>
                    {
                        Id = Constant.UrlPageIdMapping,
                        CacheValue = result
                    };

                    var returnData = await Task.FromResult(_memCache.LoadURLVsPageID(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetUrlPageIdMappingMappingAsync", "Success"));
                    return returnData;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "UrlPageIdMappingMappingAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|SetUrlPageIdMappingMappingAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        //Load Affiliate data

        public async Task<bool> LoadAffiliateDataByDomainAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                if (_affiliateCacheManager.DeleteAndCreateCollection())
                {
                    var affiliateIds = _affiliatePersistence.GetModifiedAffiliates();

                    if (affiliateIds.Count > 0)
                    {
                        foreach (var affiliateId in affiliateIds)
                        {
                            var affiliateInformation = _affiliatePersistence.GetAffiliateInfo("", "", affiliateId);
                            if (affiliateInformation != null)
                            {
                                var domainName = affiliateInformation.AffiliateCompanyDetail?.CompanyWebSite;
                                if (!string.IsNullOrWhiteSpace(domainName) ||
                                    !string.IsNullOrWhiteSpace(affiliateInformation.Alias))
                                {
                                    affiliateInformation.Id = string.IsNullOrWhiteSpace(domainName)
                                        ? affiliateInformation.Alias
                                        : domainName;

                                    _affiliateCacheManager.SetAffiliateInfoToCache(affiliateInformation);
                                }
                            }
                        }
                        var returnValue = await Task.FromResult(true);
                        mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadAffiliateDataByDomainAsync", "Success"));
                        return returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadAffiliateDataByDomain"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadAffiliateDataByDomainAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        // Load Pricing Rules

        public async Task<bool> LoadPricingRulesAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                if (!_pricingRulesCacheManager.DeleteAndCreateCollection()) return await Task.FromResult(false);

                LoadProductSaleRules();
                LoadB2BSaleRules();
                LoadB2BNetRules();
                LoadSupplierSaleRules();
                LoadProductCostSaleRules();

                if (!_pricingRulesCacheManager.CreateCollectionIfNotExist()) return await Task.FromResult(false);

                var isangoConfiguration = new IsangoConfiguration
                {
                    Id = Constant.PreExpirationId,
                    ExpirationTime = DateTime.UtcNow
                };
                _pricingRulesCacheManager.InsertExpirationTime(isangoConfiguration);
                var returnValue = await Task.FromResult(true);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadPricingRulesAsync", "Success"));
                return returnValue;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadPricingRulesAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadPricingRulesAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
        }

        public async Task<List<Activity>> GetAllActivities()
        {
            return await Task.FromResult(_activityCacheManager.GetAllActivities());
        }

        public async Task<Activity> GetSingleActivity(string activityId)
        {
            return await Task.FromResult(_activityCacheManager.GetActivity(activityId));
        }

        public async Task<List<Availability>> GetPriceAndAvailability()
        {
            return await Task.FromResult(_hotelBedsActivitiesCacheManager.GetAllPriceAndAvailability());
        }

        public async Task<string> LoadCalendarAvailability()
        {
            var watch = Stopwatch.StartNew();
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var dateTime = DateTime.UtcNow;
                var dateTimeOffset = new DateTimeOffset(dateTime);
                var timeStamp = dateTimeOffset.ToUnixTimeSeconds();
                var failedToInsert = string.Empty;

                var isLoadingRequired = _activityPersistence.GetCalendarFlag();

                if (!isLoadingRequired)
                {
                    return string.Empty;
                }

                var calendarAvailabilityList = _activityPersistence.GetCalendarAvailability();

                Int32.TryParse(collectionDatabase, out int collectionType);
                if (calendarAvailabilityList != null)
                {
                    if (collectionType == (int)CollectionType.MongoDB)
                    {
                        var result = _calendarAvailabilityCacheManager.LoadCalendarAvailability();
                        if (result)
                        {
                            var deleted = _calendarAvailabilityCacheManager.DeleteManyCalendarDocuments();
                            failedToInsert = InsertCalendarDocumentInCache(calendarAvailabilityList);
                            //return await Task.FromResult(failedToInsert);
                        }
                    }
                    else
                    {
                        var result = _calendarAvailabilityCacheManager.LoadCalendarAvailability();
                        if (result)
                        {
                            failedToInsert = InsertCalendarDocumentInCache(calendarAvailabilityList);
                            //return await Task.FromResult(failedToInsert);
                        }
                    }
                }
                watch.Stop();
                if (collectionType == (int)CollectionType.CosmosDB)
                {
                    var delete = _calendarAvailabilityCacheManager.DeleteOldCalendarActivityFromCache(_calendarAvailabilityCacheManager.GetOldCalendarAvailability(timeStamp.ToString()));
                }
                var returnValue = await Task.FromResult(failedToInsert);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadCalendarAvailability", "Success"));
                ClearCalanderWebsiteCache();
                return returnValue;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadCalendarAvailability"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadCalendarAvailability", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
        }

        public async Task<bool> LoadHBAuthorizationDataAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            bool result = false;
            try
            {
                var credentials = _masterPersistence.LoadHBauthData();
                if (credentials != null)
                {
                    var cacheResult = new CacheKey<HotelBedsCredentials>
                    {
                        Id = Constant.HbAuthorizationData,
                        CacheValue = credentials
                    };
                    result = await Task.FromResult(_masterCacheManager.SetHBAuthorizationData(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadHBAuthorizationDataAsync", "Success"));
                    return result;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadHBAuthorizationDataAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadHBAuthorizationDataAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(result);
        }

        public async Task<string> LoadPickupLocationsDataAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var pickupLocations = _masterPersistence.GetPickupLocationsByActivity();
                if (pickupLocations.Count > 0 && _pickupLocationsCacheManager.CreateCollection())
                {
                    var failedToInsert = InsertPickupLocationsInCache(pickupLocations);
                    var returnValue = await Task.FromResult(failedToInsert);
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|GetPickupLocationsByActivityAsync", "Success"));
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "GetPickupLocationsByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|GetPickupLocationsByActivityAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(string.Empty);
        }

        //Load Tiqets data

        public async Task<bool> LoadTiqetsPaxMappingsAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var result = _masterPersistence.LoadTiqetsPaxMappings()?.Where(x => x.APIType == APIType.Tiqets).ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<TiqetsPaxMapping>
                    {
                        Id = Constant.TiqetsPaxMapping,
                        CacheValue = result
                    };

                    var returnData = await Task.FromResult(_tiqetsPaxMappingCacheManager.SetPaxMappingToCache(cacheResult));
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadTiqetsPaxMappingsAsync", "Success"));
                    return returnData;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadTiqetsPaxMappingsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadTiqetsPaxMappingsAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadGoldenToursPaxMappingsAsync()
        {
            try
            {
                var result = _masterPersistence.GetPassengerMapping().Where(x => x.ApiType == (int)APIType.Goldentours).ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<PassengerMapping>
                    {
                        Id = Constant.GoldenToursPaxMapping,
                        CacheValue = result
                    };

                    return await Task.FromResult(_masterCacheManager.LoadGoldenToursPassengerMappings(cacheResult));
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadGoldenToursPaxMappingsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> LoadRezdyLabelDetailsAsync()
        {
            try
            {
                var result = _masterPersistence.GetRezdyLabelDetails().ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<RezdyLabelDetail>
                    {
                        Id = Constant.RezdyLabelDetails,
                        CacheValue = result
                    };
                    return await Task.FromResult(_masterCacheManager.LoadRezdyLabelDetailsMappings(cacheResult));
                }
            }
            catch (Exception ex)
            {
                _log.Error("CacheLoaderService|LoadRezdyLabelDetailsAsync", ex);
                throw;
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> LoadRezdyPaxMappingsAsync()
        {
            try
            {
                var result = _masterPersistence.GetRezdyPaxMappings().Where(x => x.APIType == APIType.Rezdy).ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<RezdyPaxMapping>
                    {
                        Id = Constant.RezdyPaxMapping,
                        CacheValue = result
                    };

                    return await Task.FromResult(_masterCacheManager.LoadRezdyPaxMappings(cacheResult));
                }
            }
            catch (Exception ex)
            {
                _log.Error("CacheLoaderService|LoadRezdyPaxMappingsAsync", ex);
                throw;
            }

            return await Task.FromResult(false);
        }


        public async Task<bool> LoadTourCMSPaxMappingsAsync()
        {
            try
            {
                var result = _masterPersistence.GetTourCMSPaxMappings().Where(x => x.APIType == APIType.TourCMS).ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<TourCMSMapping>
                    {
                        Id = Constant.TourCMSPaxMapping,
                        CacheValue = result
                    };

                    return await Task.FromResult(_masterCacheManager.LoadTourCMSPaxMappings(cacheResult));
                }
            }
            catch (Exception ex)
            {
                _log.Error("CacheLoaderService|LoadTourCMSPaxMappingsAsync", ex);
                throw;
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> LoadVentrataPaxMappingsAsync()
        {
            try
            {
                var result = _masterPersistence.GetVentrataPaxMappings().Where(x => x.APIType == APIType.Ventrata).ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<VentrataPaxMapping>
                    {
                        Id = Constant.VentrataPaxMapping,
                        CacheValue = result
                    };

                    return await Task.FromResult(_masterCacheManager.LoadVentrataPaxMappings(cacheResult));
                }
            }
            catch (Exception ex)
            {
                _log.Error("CacheLoaderService|LoadTourCMSPaxMappingsAsync", ex);
                throw;
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> LoadGlobalTixV3PaxMappingsAsync()
        {
            try
            {
                var result = _masterPersistence.GetGlobalTixV3PaxMappings().Where(x => x.APIType == APIType.GlobalTixV3).ToList();

                if (result?.Count > 0)
                {
                    var cacheResult = new CacheKey<GlobalTixV3Mapping>
                    {
                        Id = Constant.GlobalTixPaxMapping,
                        CacheValue = result
                    };

                    return await Task.FromResult(_masterCacheManager.LoadGlobalTixV3Mappings(cacheResult));
                }
            }
            catch (Exception ex)
            {
                _log.Error("CacheLoaderService|LoadTourCMSPaxMappingsAsync", ex);
                throw;
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> LoadElasticSearchDestinationsAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var destiList = new List<DestinationDatum>();
                var elasticDestinations = _masterPersistence.LoadElasticSearchDestinations();
                elasticDestinations = elasticDestinations.Where(x => x.landingkeyword != "").ToList();
                elasticDestinations.Where(w => w.Location == ",").ToList().ForEach(i => i.Location = "");

                if (elasticDestinations != null && elasticDestinations.Count > 0)
                {
                    string output = JsonConvert.SerializeObject(elasticDestinations);
                    var apiBaseURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticSearchAPIEndPoint);
                    var apiDestinationURl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticDestinationEndPoint);
                    bool returnData = ElasticDataPost(output, apiBaseURL, apiDestinationURl);
                    // start- Data Get and Save
                    int pageSize = 20;
                    int totalRecords = 20000;
                    for (int i = 0; i < totalRecords; i++)
                    {
                        try
                        {
                            var getData = ElasticDataGetAndSave(apiBaseURL, apiDestinationURl, i + 1, pageSize);
                            if (getData != null)
                            {
                                destiList.AddRange(getData);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    if (destiList != null && destiList.Count() > 0)
                    {
                        _masterPersistence.SaveElasticDestination(destiList);
                    }
                    // end- Data Get and Save
                    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticSearchDestinationsAsync", "Success"));
                    return await Task.FromResult(returnData);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadElasticSearchDestinationsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticSearchDestinationsAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }
        private List<DestinationDatum> ElasticDataGetAndSave(string baseAddress, string apiURL, int page, int perpage)
        {
            try
            {
                var elasticDestination = new ElasticDestination();
                using (var client = new HttpClient())
                {
                    var httpTimeout = TimeSpan.FromMinutes(10);
                    client.Timeout = httpTimeout;
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Add(contentType);
                    var response = client.GetAsync(apiURL + "?page=" + page + "&perPage=" + perpage + "");
                    response.Wait();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        elasticDestination = response?.Result?.Content.ReadAsAsync<ElasticDestination>()?.GetAwaiter().GetResult();
                        if (elasticDestination?.Data?.Count() == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return elasticDestination?.Data;
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
        public async Task<bool> LoadElasticSearchProductsAsync()
        {
            var productList = new List<ProductDatum>();
            var mailErrorLog = new List<Tuple<string, string>>();
            bool returnData = false;
            try
            {
                var elasticProducts = _masterPersistence.LoadElasticSearchProducts();
                string output = JsonConvert.SerializeObject(elasticProducts);
                var apiBaseURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticSearchAPIEndPoint);
                var apiProductURl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticProductsEndPoint);
                if (elasticProducts != null && elasticProducts.Count > 0)
                {
                    for (int i = 0; i < elasticProducts.Count; i = i + 1000)
                    {
                        try
                        {
                            var itemsElasticProducts = elasticProducts?.Skip(i)?.Take(1000);
                            string itemsOutput = JsonConvert.SerializeObject(itemsElasticProducts);

                            returnData = ElasticDataPost(itemsOutput, apiBaseURL, apiProductURl);

                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "CacheLoaderService",
                                MethodName = "LoadElasticSearchProductsAsync"
                            };
                            _log.Error(isangoErrorEntity, ex);
                            mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticSearchProductsAsync", "Error:" + ex));
                            throw;
                        }
                    }

                    //Note:Push (save in database) move to desktop application: DataDumping Console
                    //try
                    //{
                    //    var getData = ElasticProductDataGetAndSave(apiBaseURL, apiProductURl);
                    //    if (getData != null)
                    //    {
                    //        productList.AddRange(getData);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    var isangoErrorEntity = new IsangoErrorEntity
                    //    {
                    //        ClassName = "CacheLoaderService",
                    //        MethodName = "LoadElasticSearchProductsAsync"
                    //    };
                    //    _log.Error(isangoErrorEntity, ex);
                    //    mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticSearchProductsAsync", "Error:" + ex));
                    //    throw;
                    //}

                    //if (productList != null && productList.Count() > 0)
                    //{
                    //    _masterPersistence.SaveElasticProducts(productList);
                    //}
                    // end- Data Get and Save
                    return await Task.FromResult(returnData);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadElasticSearchProductsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticSearchProductsAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
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
                        elasticProduct = response?.Result?.Content.ReadAsAsync<ElasticProduct>()?.GetAwaiter().GetResult();
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

        public async Task<bool> LoadElasticSearchAttractionsAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {
                var attractionList = new List<AttractionDatum>();
                var elasticAttractions = _masterPersistence.LoadElasticSearchAttractions();
                if (elasticAttractions != null && elasticAttractions.Count > 0)
                {
                    string output = JsonConvert.SerializeObject(elasticAttractions);
                    var apiBaseURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticSearchAPIEndPoint);
                    var apiAttractionURl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticAttractionsEndPoint);
                    bool returnData = ElasticDataPut(output, apiBaseURL, apiAttractionURl);

                    // start- Data Get and Save
                    int pageSize = 20;
                    int totalRecords = 20000;
                    for (int i = 0; i < totalRecords; i++)
                    {
                        try
                        {
                            var getData = ElasticAttractionDataGetAndSave(apiBaseURL, apiAttractionURl, i + 1, pageSize);
                            if (getData != null)
                            {
                                attractionList.AddRange(getData);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    if (attractionList != null && attractionList.Count() > 0)
                    {
                        _masterPersistence.SaveElasticAttraction(attractionList);
                    }
                    // end- Data Get and Save

                    return await Task.FromResult(returnData);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadElasticSearchProductsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticSearchProductsAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LoadElasticAffiliateAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            try
            {

                var elasticAffiliate = _masterPersistence.LoadElasticAffiliate();

                var affilateDataPass = new ElasticAffiliatePass
                {
                    Default = 46,
                    All = elasticAffiliate.Select(s => Convert.ToInt32(s.AffiliateKey)).ToArray()
                };

                if (elasticAffiliate != null && elasticAffiliate.Count > 0)
                {
                    string output = JsonConvert.SerializeObject(affilateDataPass);
                    var apiBaseURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticSearchAPIEndPoint);
                    var apiAffiliateURl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ElasticAffiliateEndPoint);
                    bool returnData = ElasticDataPut(output, apiBaseURL, apiAffiliateURl);
                    return await Task.FromResult(returnData);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "LoadElasticAffiliateAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("CacheLoaderService|LoadElasticAffiliateAsync", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMail) == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            return await Task.FromResult(false);
        }
        private List<AttractionDatum> ElasticAttractionDataGetAndSave(string baseAddress, string apiURL, int page, int perpage)
        {
            try
            {
                var elasticAttraction = new ElasticAttraction();
                using (var client = new HttpClient())
                {
                    var httpTimeout = TimeSpan.FromMinutes(10);
                    client.Timeout = httpTimeout;
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Add(contentType);
                    var response = client.GetAsync(apiURL + "?page=" + page + "&perPage=" + perpage + "");
                    response.Wait();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        elasticAttraction = response?.Result?.Content.ReadAsAsync<ElasticAttraction>()?.GetAwaiter().GetResult();
                        if (elasticAttraction?.Data?.Count() == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return elasticAttraction?.Data;
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

        private bool ElasticDataPut(string jsonData, string baseAddress, string api)
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
                var response = client.PutAsync(api, contentData);
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
        #region Private Methods

        private ClientInfo DefaultClientInfo()
        {
            var clientInfo = new ClientInfo()
            {
                AffiliateId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.DefaultAffiliateId),
                Currency = new Currency
                {
                    IsoCode = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.DefaultCurrencyCode)
                }
            };

            return clientInfo;
        }

        private List<Availability> CreateAvailabilityList(DataTable isangoAvailabilities)
        {
            var availabilities = new List<Availability>();

            var serviceIds = isangoAvailabilities.AsEnumerable().Select(s => Convert.ToInt32(s[Constant.ServiceId])).Distinct().ToList();

            foreach (var serviceId in serviceIds)
            {
                var serviceRows = isangoAvailabilities.Select($"{Constant.ServiceId}=" + serviceId);

                var serviceOptions = serviceRows.Select(x => Convert.ToInt32(x[Constant.ServiceOptionId])).Distinct().ToList();

                foreach (var serviceOptionId in serviceOptions)
                {
                    var baseData = new Dictionary<DateTime, decimal>();
                    var costData = new Dictionary<DateTime, decimal>();

                    var availability = new Availability();
                    var regionId = 0;
                    var currency = "";

                    foreach (var isangoAvailability in serviceRows.Where(x => Convert.ToInt32(x[Constant.ServiceOptionId]) == serviceOptionId))
                    {
                        var startDate = DbPropertyHelper.DateTimePropertyFromRow(isangoAvailability, Constant.StartDate);
                        var endDate = DbPropertyHelper.DateTimePropertyFromRow(isangoAvailability, Constant.EndDate);
                        var range = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days).Select(i => startDate.AddDays(i)).ToList();

                        if (regionId.Equals(0))
                        {
                            regionId = DbPropertyHelper.Int32PropertyFromRow(isangoAvailability, Constant.RegionId);
                        }

                        currency = DbPropertyHelper.StringPropertyFromRow(isangoAvailability, Constant.CurrencyISOCode);

                        baseData = range.ToDictionary(x => x.Date, x => DbPropertyHelper.DecimalPropertyFromRow(isangoAvailability, Constant.GatePrice));
                        costData = range.ToDictionary(x => x.Date, x => DbPropertyHelper.DecimalPropertyFromRow(isangoAvailability, Constant.CostPrice));
                    }

                    availability.Id = serviceOptionId.ToString();
                    availability.ServiceId = serviceId;
                    availability.ServiceOptionId = serviceOptionId;
                    availability.RegionId = regionId;
                    availability.BaseDateWisePriceAndAvailability = baseData;
                    availability.CostDateWisePriceAndAvailability = costData;
                    availability.Currency = currency;
                    availabilities.Add(availability);
                }
            }

            return availabilities;
        }

        private List<Activity> UpdateActivityDatePriceAvailability(List<Activity> activityList, DataTable updatedPriceAndAvailability)
        {
            activityList.RemoveAll(x => x.ProductOptions == null || x.ProductOptions?.Count == 0);

            foreach (var activity in activityList)
            {
                foreach (var productOption in activity.ProductOptions)
                {
                    try
                    {
                        var result = updatedPriceAndAvailability.Select(activity.ActivityType == ActivityType.Bundle ? $"{Constant.ServiceId} = {productOption.ComponentServiceID} and {Constant.ServiceOptionId} = {productOption.Id}" : $"{Constant.ServiceId} = {activity.Id} and {Constant.ServiceOptionId}= {productOption.Id}");

                        if (result.Any())
                        {
                            var baseValue = Convert.ToDecimal(result.Min(x => x[Constant.GatePrice]));
                            decimal costValue;

                            // if multiple rows exists having same gate price then find min of cost price.
                            if (result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue)
                                    .Select(x => x[Constant.CostPrice]).Count() > 1)
                            {
                                costValue = Convert.ToDecimal(result
                                    .Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue)
                                    .Min(x => x[Constant.CostPrice]));
                            }
                            else // else get cost price of row having min base price
                            {
                                costValue = Convert.ToDecimal(result
                                    .Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue)
                                    .Select(x => x[Constant.CostPrice]).First());
                            }

                            //As currency for the ServiceID is same for any duration, so taking currency of the first row if multiple rows exists.
                            var currency = new Currency()
                            {
                                IsoCode = result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue)
                                    .Select(x => x[Constant.CurrencyISOCode]).First().ToString()
                            };

                            var costPrice = new Price()
                            {
                                Currency = currency,
                                Amount = costValue
                            };

                            var basePrice = new Price()
                            {
                                Currency = currency,
                                Amount = baseValue
                            };

                            var gateBasePrice = new Price()
                            {
                                Currency = currency,
                                Amount = baseValue
                            };

                            productOption.CostPrice = costPrice;
                            productOption.BasePrice = basePrice;
                            productOption.GateBasePrice = gateBasePrice;
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "CacheLoaderService",
                            MethodName = "UpdateActivityDatePriceAvailability",
                            Params = $"{activity?.Id}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
            }
            return activityList;
        }

        // Load Product Sale Rules
        private void LoadProductSaleRules()
        {
            var productSaleRules = _priceRuleEnginePersistence.GetProductSaleRule();

            LoadProductSaleRulesByActivity(productSaleRules.ProductSaleRulesByActivity);
            LoadProductSaleRulesByOption(productSaleRules.ProductSaleRulesByOption);
            LoadProductSaleRulesByAffiliate(productSaleRules.ProductSaleRulesByAffiliate);
            LoadProductSaleRulesByCountry(productSaleRules.ProductSaleRulesByCountry);
        }

        private void LoadProductCostSaleRules()
        {
            var productSaleRules = _priceRuleEnginePersistence.GetProductCostSaleRule();

            LoadProductCostSaleRulesByActivity(productSaleRules.ProductSaleRulesByActivity);
            LoadProductCostSaleRulesByOption(productSaleRules.ProductSaleRulesByOption);
            LoadProductCostSaleRulesByAffiliate(productSaleRules.ProductSaleRulesByAffiliate);
            LoadProductCostSaleRulesByCountry(productSaleRules.ProductSaleRulesByCountry);
        }


        private void LoadProductSaleRulesByActivity(List<ProductSaleRuleByActivity> productSaleRulesByActivity)
        {
            var rulesData = new CacheKey<ProductSaleRuleByActivity>()
            {
                Id = Constant.ProductSaleRuleByActivity,
                CacheKeyName = Constant.ProductSaleRuleByActivity,
                CacheValue = productSaleRulesByActivity
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadProductSaleRulesByOption(List<ProductSaleRuleByOption> productSaleRulesByOption)
        {
            var rulesData = new CacheKey<ProductSaleRuleByOption>()
            {
                Id = Constant.ProductSaleRuleByOption,
                CacheKeyName = Constant.ProductSaleRuleByOption,
                CacheValue = productSaleRulesByOption
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadProductSaleRulesByAffiliate(List<ProductSaleRuleByAffiliate> productSaleRulesByAffiliate)
        {
            var rulesData = new CacheKey<ProductSaleRuleByAffiliate>()
            {
                Id = Constant.ProductSaleRuleByAffiliate,
                CacheKeyName = Constant.ProductSaleRuleByAffiliate,
                CacheValue = productSaleRulesByAffiliate
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadProductSaleRulesByCountry(List<ProductSaleRuleByCountry> productSaleRulesByCountry)
        {
            var rulesData = new CacheKey<ProductSaleRuleByCountry>()
            {
                Id = Constant.ProductSaleRuleByCountry,
                CacheKeyName = Constant.ProductSaleRuleByCountry,
                CacheValue = productSaleRulesByCountry
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }
        /// <summary>
        /// Load Product Cost Rule
        /// </summary>
        private void LoadProductCostSaleRulesByActivity(List<ProductSaleRuleByActivity> productSaleRulesByActivity)
        {
            var rulesData = new CacheKey<ProductSaleRuleByActivity>()
            {
                Id = Constant.ProductCostSaleRuleByActivity,
                CacheKeyName = Constant.ProductCostSaleRuleByActivity,
                CacheValue = productSaleRulesByActivity
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadProductCostSaleRulesByOption(List<ProductSaleRuleByOption> productSaleRulesByOption)
        {
            var rulesData = new CacheKey<ProductSaleRuleByOption>()
            {
                Id = Constant.ProductCostSaleRuleByOption,
                CacheKeyName = Constant.ProductCostSaleRuleByOption,
                CacheValue = productSaleRulesByOption
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadProductCostSaleRulesByAffiliate(List<ProductSaleRuleByAffiliate> productSaleRulesByAffiliate)
        {
            var rulesData = new CacheKey<ProductSaleRuleByAffiliate>()
            {
                Id = Constant.ProductCostSaleRuleByAffiliate,
                CacheKeyName = Constant.ProductCostSaleRuleByAffiliate,
                CacheValue = productSaleRulesByAffiliate
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadProductCostSaleRulesByCountry(List<ProductSaleRuleByCountry> productSaleRulesByCountry)
        {
            var rulesData = new CacheKey<ProductSaleRuleByCountry>()
            {
                Id = Constant.ProductCostSaleRuleByCountry,
                CacheKeyName = Constant.ProductCostSaleRuleByCountry,
                CacheValue = productSaleRulesByCountry
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        // Load B2B Sale Rules
        private void LoadB2BSaleRules()
        {
            var b2BSaleRules = _priceRuleEnginePersistence.GetB2BSaleRules();
            var b2BSaleData = new CacheKey<B2BSaleRule>()
            {
                Id = Constant.B2BSaleRules,
                CacheKeyName = Constant.B2BSaleRules,
                CacheValue = b2BSaleRules
            };
            _pricingRulesCacheManager.InsertDocuments(b2BSaleData);
        }

        // Load B2B Net Rate Rules
        private void LoadB2BNetRules()
        {
            var b2BNetRateRules = _priceRuleEnginePersistence.GetB2BNetRateRules();
            var b2BNetRateData = new CacheKey<B2BNetRateRule>()
            {
                Id = Constant.B2BNetRateRules,
                CacheKeyName = Constant.B2BNetRateRules,
                CacheValue = b2BNetRateRules
            };
            _pricingRulesCacheManager.InsertDocuments(b2BNetRateData);
        }

        // Load Supplier Sale Rules
        private void LoadSupplierSaleRules()
        {
            var supplierSaleRule = _priceRuleEnginePersistence.GetSupplierSaleRule();
            LoadSupplierSaleRulesByActivity(supplierSaleRule.SupplierSaleRulesByActivity);
            LoadSupplierSaleRulesByOption(supplierSaleRule.SupplierSaleRulesByOption);
        }

        private void LoadSupplierSaleRulesByActivity(List<SupplierSaleRuleByActivity> supplierSaleRulesByActivity)
        {
            var rulesData = new CacheKey<SupplierSaleRuleByActivity>()
            {
                Id = Constant.SupplierSaleRuleByActivity,
                CacheKeyName = Constant.SupplierSaleRuleByActivity,
                CacheValue = supplierSaleRulesByActivity
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private void LoadSupplierSaleRulesByOption(List<SupplierSaleRuleByOption> supplierSaleRulesByOption)
        {
            var rulesData = new CacheKey<SupplierSaleRuleByOption>()
            {
                Id = Constant.SupplierSaleRuleByOption,
                CacheKeyName = Constant.SupplierSaleRuleByOption,
                CacheValue = supplierSaleRulesByOption
            };
            _pricingRulesCacheManager.InsertDocuments(rulesData);
        }

        private string InsertDocumentInCache(List<Activity> activityList, string languageCode)
        {
            var sb = new StringBuilder();
            Int32.TryParse(collectionDatabase, out int collectionType);

            Parallel.ForEach(activityList, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (activity) =>
            {
                try
                {
                    if (!_activityCacheManager.InsertActivity(activity, languageCode))
                    {
                        sb.Append(sb.Length == 0 ? activity.Id : "," + activity.Id);
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "CacheLoaderService",
                        MethodName = "InsertDocumentInCache",
                        Params = $"{activity?.Id}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            });

            return sb.ToString();
        }

        private string InsertCalendarDocumentInCache(List<CalendarAvailability> calendarAvailabilityList)
        {
            var sb = new StringBuilder();
            Int32.TryParse(collectionDatabase, out int collectionType);
            if (collectionType == (int)CollectionType.MongoDB)
            {
                _calendarAvailabilityCacheManager.InsertManyCalendarDocuments(calendarAvailabilityList);
            }
            else
            {
                //foreach (var calendarAvailability in calendarAvailabilityList)
                Parallel.ForEach(calendarAvailabilityList, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount * 2 }, (calendarAvailability) =>
                {
                    try
                    {
                        if (!_calendarAvailabilityCacheManager.InsertCalendarDocuments(calendarAvailability))
                        {
                            sb.Append(sb.Length == 0 ? calendarAvailability.ActivityId.ToString() : "," + calendarAvailability.ActivityId);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "CacheLoaderService",
                            MethodName = "InsertCalendarDocumentInCache",
                            Params = $"{calendarAvailability.ActivityId.ToString()}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
                );
            }
            return sb.ToString();
        }

        private string InsertPickupLocationsInCache(List<PickupLocation> pickupLocations)
        {
            var sb = new StringBuilder();
            foreach (var location in pickupLocations)
            {
                try
                {
                    if (!_pickupLocationsCacheManager.InsertDocuments(location))
                        sb.Append(sb.Length == 0 ? location.ActivityId.ToString() : "," + location.ActivityId);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "CacheLoaderService",
                        MethodName = "InsertPickupLocationsInCache",
                        Params = $"{location.ActivityId.ToString()}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Helper Method for Sending Success and Failure Mail
        /// </summary>
        private void SendMail(List<Tuple<string, string>> data)
        {
            if (Convert.ToString(ConfigurationManager.AppSettings["ErrorMail"]) == "1")
            {
                _mailerService?.SendErrorMail(data);
            }
        }

        /// <summary>
        /// Clear website activity cache
        /// </summary>
        /// <param name="activityId"></param>
        private void ClearCalanderWebsiteCache()
        {
            var sites = GetSites();
            var appSettingsCacheDeleteToken = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AppSettingsCacheDeleteToken);

            string websitesCacheKeyActivityAll = string.Format(Constant.WebsitesCacheKeyActivityAll, true, appSettingsCacheDeleteToken);
            try
            {
                if (sites?.Any() == true)
                {
                    foreach (var site in sites)
                    {
                        var url = $"{site}{websitesCacheKeyActivityAll}";
                        CallSitesUsingCache(url);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SynchronizerService",
                    MethodName = "ClearActivityWebsiteCache-AcitvityChageListError",
                    Params = $"ActivityId : All"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        private List<string> GetSites()
        {
            var result = default(List<string>);
            var sites = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebsitesToClearActivityCache);
            if (!string.IsNullOrWhiteSpace(sites))
            {
                result = sites?.Split(',')?.ToList()?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()?.ToList();
            }
            return result;
        }

        private void CallSitesUsingCache(string url)
        {
            try
            {
                var request = HttpWebRequest.Create(url);
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var urlText = reader.ReadToEnd();
                var info = $"CacheLoaderService|CallSitesUsingCache|{url}\r\n{urlText}";
                _log.Info(info);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "CallSitesUsingCache",
                    Params = $"{url}\r\n{ex.Message}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        /// <summary>
        /// Clear website activity cache
        /// </summary>
        /// <param name="activityId"></param>
        public void ClearActivityWebsiteCache(List<ActivityChangeTracker> changedActivities)
        {
            /*
             changedActivities = new List<ActivityChangeTracker>
                 {
                     new ActivityChangeTracker
                     {
                         ActivityId = 5850,
                     },
                     new ActivityChangeTracker
                     {
                         ActivityId = 23066,
                     }
                 };

             //for (int i = 0; i < 400; i++)
             //{
             //    changedActivities.Add(
             //        new ActivityChangeTracker
             //        {
             //            ActivityId = 5850 + i,
             //        }
             //        );
             //}
             //*/

            changedActivities = changedActivities?.Where(x => x.ActivityId > 0)?.Distinct()?.ToList();
            var sites = GetSites();
            var appSettingsCacheDeleteToken = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AppSettingsCacheDeleteToken);

            if (changedActivities.Count > 400)
            {
                string websitesCacheKeyActivityAll = string.Format(Constant.WebsitesCacheKeyActivityAll, true, appSettingsCacheDeleteToken);
                try
                {
                    if (sites?.Any() == true)
                    {
                        foreach (var site in sites)
                        {
                            var url = $"{site}{websitesCacheKeyActivityAll}";
                            CallSitesUsingCache("All", url);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SynchronizerService",
                        MethodName = "ClearActivityWebsiteCache-AcitvityChageListError",
                        Params = $"ActivityId : All"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }
            else
            {
                foreach (var batch in changedActivities.Batch(100))
                {
                    if (changedActivities?.Any() == true)
                    {
                        var sb = new StringBuilder();

                        foreach (var activity in batch)
                        {
                            sb.Append($"activity_{activity.ActivityId},");
                        }
                        var activityIds = sb.ToString().Remove(sb.ToString().Length - 1);

                        try
                        {
                            var websitesCacheKeyActivity = string.Format(Constant.WebsitesCacheKeyActivity, activityIds, appSettingsCacheDeleteToken);

                            if (sites?.Any() == true)
                            {
                                foreach (var site in sites)
                                {
                                    var url = $"{site}{websitesCacheKeyActivity}";
                                    CallSitesUsingCache(activityIds, url);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SynchronizerService",
                                MethodName = "ClearActivityWebsiteCache-AcitvityChageListError",
                                Params = $"ActivityId : {activityIds}"
                            };
                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
        }

        private void CallSitesUsingCache(string activityIds, string url)
        {
            try
            {
                var request = HttpWebRequest.Create(url);
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var urlText = reader.ReadToEnd();
                var info = $"CacheLoaderServuce|ClearActivityWebsiteCache-AcitvityCacheClear|{url}\r\n{urlText}";
                _log.Info(info);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CacheLoaderService",
                    MethodName = "ClearActivityWebsiteCache-AcitvityCacheClear",
                    Params = $"ActivityId : {activityIds}\r\n{url}\r\n{ex.Message}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        #endregion Private Methods
    }
}