using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.SiteMap;
using Isango.Entities.v1Css;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Adyen;
using System.Runtime.Caching;
using Util;
using CacheHelper = Util.CacheHelper;
using Constant = Isango.Service.Constants.Constant;
using GoldenTours = Isango.Entities.GoldenTours;
using TicketByRegion = Isango.Entities.TicketByRegion;

namespace Isango.Service
{
    public class MasterService : IMasterService
    {
        #region Variables

        private readonly IMasterPersistence _masterPersistence;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly ILogger _log;
        private readonly IAgeGroupsCacheManager _ageGroupsCacheManager;
        private readonly IPickupLocationsCacheManager _pickupLocationsCacheManager;
        private readonly IAdyenAdapter _adyenAdapter;
        //private readonly IActivityService _activityService;

        #endregion Variables

        #region Constructor

        public MasterService(IMasterPersistence masterPersistence, IMasterCacheManager masterCacheManager, ILogger log, IAgeGroupsCacheManager ageGroupsCacheManager, IPickupLocationsCacheManager pickupLocationsCacheManager, IAdyenAdapter adyenAdapter/*, IActivityService activityService*/)
        {
            _masterPersistence = masterPersistence;
            _masterCacheManager = masterCacheManager;
            _log = log;
            _ageGroupsCacheManager = ageGroupsCacheManager;
            _pickupLocationsCacheManager = pickupLocationsCacheManager;
            _adyenAdapter = adyenAdapter;
            //_activityService = activityService;
        }

        #endregion Constructor

        //TODO: will be removed after reviewed by Prashant.
        /// <summary>
        /// This Method with return the list of currencies related to the affiliateID.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns> List of Currency</returns>
        public async Task<List<Currency>> GetCurrenciesAsync(string affiliateId)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetCurrencies(affiliateId));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetCurrencies",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        //TODO: will be removed after reviewed by Prashant.
        /// <summary>
        /// This method will return teh Currency object for the provided Currency Code
        /// Not sure whether to keep the result into the cache or not.
        /// Only implemented in the persistence layer.
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns> Currency Object</returns>
        public async Task<Currency> GetCurrencyAsync(string currencyCode)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetCurrency(currencyCode));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetCurrency",
                    Params = $"{currencyCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        //TODO: will be removed after reviewed by Prashant.
        /// <summary>
        /// This method will return Currency code for the provided Country code.
        /// Not sure whether to keep the result into the cache or not.
        /// Only implemented in the persistence layer.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public async Task<string> GetCurrencyCodeForCountryAsync(string countryCode)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetCurrencyCodeForCountry(countryCode));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetCurrencyCodeForCountry",
                    Params = $"{countryCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method return list of region based on language code.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task<List<Region>> GetCountriesAsync(string languageCode)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetCountries(languageCode));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetCountries",
                    Params = $"{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method return region id from geo tree for activity id
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<int> GetRegionIdFromGeotreeAsync(int activityId)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetRegionIdFromGeotree(activityId));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetRegionIdFromGeotree",
                    Params = $"{activityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method return list of ticket by region
        /// </summary>
        /// <returns></returns>
        public async Task<List<TicketByRegion>> GetFilteredThemeparkTicketsAsync()
        {
            try
            {
                var filteredThemeParkTicket = _masterCacheManager.GetFilteredTickets(Constant.FilteredTickets);

                if (filteredThemeParkTicket == null)
                {
                    var cacheValue = _masterPersistence.GetFilteredThemeparkTickets();

                    filteredThemeParkTicket = new CacheKey<TicketByRegion>
                    {
                        Id = Constant.FilteredTickets,
                        CacheValue = cacheValue
                    };

                    _masterCacheManager.SetFilteredTicketsToCache(filteredThemeParkTicket);
                }

                return await Task.FromResult(filteredThemeParkTicket.CacheValue);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetFilteredThemeparkTickets"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method return list of regions
        /// </summary>
        /// <returns></returns>
        public async Task<List<LatLongVsurlMapping>> GetRegionAsync()
        {
            try
            {
                // Get data from cache
                var cacheResult = _masterCacheManager.GetRegionData(Constant.GeoCoordinateMasterMappingKey);

                List<LatLongVsurlMapping> result;

                if (cacheResult == null || cacheResult.CacheValue.Count == 0)
                {
                    // Get data from database
                    result = _masterPersistence.GetRegionData();
                    if (result != null)
                    {
                        cacheResult = new CacheKey<LatLongVsurlMapping>
                        {
                            Id = Constant.GeoCoordinateMasterMappingKey,
                            CacheValue = result
                        };

                        _masterCacheManager.SetRegionData(cacheResult);
                    }
                }
                else
                {
                    result = cacheResult.CacheValue;
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetRegion"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        //public async Task<List<Activity>> GetCrossSellProducts(int? lineOfBusinessId)
        //{
        //    try
        //    {
        //        //var CrossSellDataFromCacheTime = Stopwatch.StartNew();
        //        var allCrossSellProducts = GetAllCrossSellProductsAsync()?.GetAwaiter().GetResult();

        //        var crossSellLogic = GetCrossSellLogic();
        //        //CrossSellDataFromCacheTime.Stop();
        //        //var timeElapsed = CrossSellDataFromCacheTime.Elapsed;
        //        var crossSellProducts = FilterCrossSell(allCrossSellProducts, crossSellLogic, lineOfBusinessId);

        //        return await Task.FromResult(crossSellProducts);
        //    }
        //    catch (Exception ex)
        //    {
        //        var isangoErrorEntity = new IsangoErrorEntity
        //        {
        //            ClassName = "MasterService",
        //            MethodName = "GetCrossSellProducts"
        //        };
        //        _log.Error(isangoErrorEntity, ex);
        //        return null;
        //    }
        //}

        /// <summary>
        /// This method will return list of the country and its service no.
        /// </summary>
        public async Task<Dictionary<string, string>> GetSupportPhonesWithCountryCodeAsync(string affiliateId, string language)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetSupportPhonesWithCountryCode(affiliateId, language));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetSupportPhonesWithCountryCode",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}{language}"
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
                //throw;
            }
        }

        /// <summary>
        /// This method will return all the cross sell product list.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<int, List<CrossSellProduct>>> GetAllCrossSellProductsAsync()
        {
            try
            {
                var CrossSellProducts = default(Dictionary<int, List<CrossSellProduct>>);
                var key = "CrossSellProducts";
                if (!CacheHelper.Exists(key) || !CacheHelper.Get(key, out CrossSellProducts))
                {
                    CrossSellProducts = _masterPersistence.GetAllCrossSellProducts();
                    if (CrossSellProducts.Count > 0)
                    {
                        CacheHelper.Set(key, CrossSellProducts);
                    }
                }
                return await Task.FromResult(CrossSellProducts);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetAllCrossSellProducts"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<CrossSellLogic> GetCrossSellLogic()
        {
            var result = default(List<CrossSellLogic>);
            try
            {
                var key = "CrossSellLogicMaster";
                if (!CacheHelper.Exists(key) || !CacheHelper.Get(key, out result))
                {
                    result = _masterPersistence.GetCrossSellLogic();
                    try
                    {
                        CacheHelper.Set(key, result);
                    }
                    catch (System.Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "CrossSellCache",
                            MethodName = "GetCrossSellLogic",
                            Params = SerializeDeSerializeHelper.Serialize(result)
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CrossSellCache",
                    MethodName = "GetAllCrossSellProducts",
                };
                _log.Error(isangoErrorEntity, ex);
            }

            return result;
        }

        //private List<Activity> FilterCrossSell(Dictionary<int, List<CrossSellProduct>> crossSellproducts, List<CrossSellLogic> crossSellLogic, int? lineOfBusinessId)
        //{
        //    //ActivityService activityService = new ActivityService();
        //    var productsToShow = new List<Activity>();
        //    try
        //    {
        //        // Final XSell products that are in same region as the selected products but are having different attraction Ids.
        //        var filteredProducts = new List<CrossSellProduct>();
        //        var crossSellBusinessId = crossSellLogic.Where(x => x.LineofBusinessid == lineOfBusinessId)?.FirstOrDefault()?.CROSSSELLBUSINESSNAME1;

        //        filteredProducts = (crossSellproducts.Where(w => w.Key == lineOfBusinessId).SelectMany(s => s.Value).ToList());

        //        if (productsToShow.Count < 3)
        //        {
        //            var products = GetActivitiesToAddInCrossSellProducts(filteredProducts);
        //            if (productsToShow.Count == 0)
        //            {
        //                productsToShow = products;
        //            }
        //            else
        //            {
        //                foreach (var product in products)
        //                {
        //                    productsToShow.Add(product);
        //                    if (productsToShow.Count.Equals(3))
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var isangoErrorEntity = new IsangoErrorEntity
        //        {
        //            ClassName = "UIHelper",
        //            MethodName = "FilterCrossSell",
        //            Params = string.Empty
        //        };
        //        _log.Error(isangoErrorEntity, ex);
        //    }
        //    return productsToShow;
        //    //Return top 3 products
        //}

        //private List<Activity> GetActivitiesToAddInCrossSellProducts(List<CrossSellProduct> crossSellProducts)
        //{
        //    var productsToShow = new List<Activity>();
        //    try
        //    {
        //        crossSellProducts = (from cp in crossSellProducts orderby cp.Priority select cp).ToList();
        //        foreach (var item in crossSellProducts)
        //        {
        //            var activityFromBBCache = _activityService.LoadActivityAsync(item.Id, DateTime.Today, null)?.GetAwaiter().GetResult();

        //            if (activityFromBBCache != null)
        //            {
        //                productsToShow.Add(activityFromBBCache);
        //                if (productsToShow.Count.Equals(3))
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var isangoErrorEntity = new IsangoErrorEntity
        //        {
        //            ClassName = "UIHelper",
        //            MethodName = "GetActivitiesToAddInCrossSellProducts",
        //            Params = string.Empty,
        //        };
        //        _log.Error(isangoErrorEntity, ex);
        //    }
        //    return productsToShow;
        //}

        /// <summary>
        /// This method return list of site map data.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="siteType"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task<Tuple<List<SiteMapData>, int>> GetSiteMapDataAsync(string affiliateId, string languageCode)
        {
            try
            {
                var result = _masterPersistence.GetSiteMapData(affiliateId, languageCode);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetSiteMapData",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method return collection of attraction and region
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> LoadIndexedAttractionToRegionUrlsAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadIndexedAttractionToRegionUrls());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "LoadIndexedAttractionToRegionUrls"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method return collection of supported languages by isango
        /// </summary>
        /// <returns>List of languages</returns>
        public async Task<List<Language>> GetSupportedLanguagesAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetSupportedLanguages());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetSupportedLanguages"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the currency exchange table.
        /// </summary>
        /// <returns></returns>

        public async Task<List<CurrencyExchangeRates>> LoadCurrencyExchangeRatesAsync()
        {
            try
            {
                var currencyExchangeRate = _masterCacheManager.GetCurrencyExchangeRate() ?? _masterPersistence.LoadExchangeRates();

                return await Task.FromResult(currencyExchangeRate);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "LoadCurrencyExchangeRates"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the currency exchange table.
        /// </summary>
        /// <returns></returns>
        public async Task<List<MappedLanguage>> LoadMappedLanguageAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadMappedLanguage());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "LoadMappedLanguage"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        //TODO: will be removed after reviewed by Prashant.
        /// <summary>
        /// This method returns list of the currency exchange table.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entities.HotelBeds.MappedRegion>> LoadRegionVsDestinationAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.RegionVsDestination());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "RegionVsDestination"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<decimal> GetConversionFactorAsync(string baseCurrencyCode, string currencyCode)
        {
            decimal conversionFactorWithBuffer = 1;
            try
            {
                if ((baseCurrencyCode + "").Trim().ToLowerInvariant().Equals(currencyCode.Trim().ToLowerInvariant()))
                    return conversionFactorWithBuffer;

                var exchangeRateData = _masterPersistence.LoadExchangeRates().AsEnumerable()
                                        .FirstOrDefault(x => x.FromCurrencyCode.ToLower().Equals((baseCurrencyCode + "").ToLowerInvariant())
                                             && x.ToCurrencyCode.ToLower().Equals(currencyCode.ToLowerInvariant()));

                if (exchangeRateData != null)
                {
                    conversionFactorWithBuffer = exchangeRateData.ExchangeRate;

                    // Adding x% in exchange rate in case of non wirecard currencies
                    if ((
                            currencyCode.ToUpperInvariant().Equals("NZD")
                        || currencyCode.ToUpperInvariant().Equals("SGD")
                        || currencyCode.ToUpperInvariant().Equals("HKD")
                        || currencyCode.ToUpperInvariant().Equals("MYR")
                        || currencyCode.ToUpperInvariant().Equals("BRL")
                        || currencyCode.ToUpperInvariant().Equals("INR"))
                        && ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ExchangeRateBuffer) != null)
                    {
                        if (int.TryParse(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ExchangeRateBuffer), out var exchangeRateBuffer))
                        {
                            var factor = 100 + exchangeRateBuffer;
                            conversionFactorWithBuffer = (conversionFactorWithBuffer * factor) / 100;
                            return conversionFactorWithBuffer;
                        }
                    }
                }
                return await Task.FromResult(conversionFactorWithBuffer);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetConversionFactor"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<LocalizedDestinations>> GetLocalizedDestinationsAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetLocalizedDestinations());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetLocalizedDestinationsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<LocalizedCategories>> GetLocalizedCategoriesAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetLocalizedCategories());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetLocalizedCategoriesAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<AgeGroup>> GetGLIAgeGroupAsync(int activityId, APIType apiType)
        {
            try
            {
                var ageGroup = _ageGroupsCacheManager.GetAgeGroup((int)apiType, activityId);
                if (ageGroup?.Any() != true && apiType == APIType.Graylineiceland)
                {
                    var gliAgeGroup = _masterPersistence.GetGliAgeGroupsByActivity();
                    ageGroup = gliAgeGroup.Where(x => x.ActivityId == activityId).ToList();
                }
                return await Task.FromResult(ageGroup);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetGLIAgeGroup"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<AgeGroup>> GetPrioAgeGroupAsync(int activityId, APIType apiType)
        {
            var ageGroup = default(List<AgeGroup>);
            try
            {
                ageGroup = _ageGroupsCacheManager.GetAgeGroup((int)apiType, activityId);
                if (ageGroup?.Any() != true && apiType == APIType.Prio)
                {
                    var gliAgeGroup = _masterPersistence.GetPrioAgeGroupsByActivity();
                    ageGroup = gliAgeGroup.Where(x => x.ActivityId == activityId).ToList();
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                        _log.Error(
                                    new IsangoErrorEntity
                                    {
                                        ClassName = nameof(MasterService),
                                        MethodName = nameof(GetPrioAgeGroupAsync),
                                        Params = $"ActivityId: {activityId}"
                                    }, ex
                            )
                    );
            }
            return await Task.FromResult(ageGroup);
        }

        public async Task<List<AgeGroup>> GetPrioHubAgeGroupAsync(int activityId, APIType apiType)
        {
            var ageGroup = default(List<AgeGroup>);
            try
            {
                ageGroup = _ageGroupsCacheManager.GetAgeGroup((int)apiType, activityId);
                if (ageGroup?.Any() != true && apiType == APIType.PrioHub)
                {
                    var gliAgeGroup = _masterPersistence.GetPrioHubAgeGroupsByActivity();
                    ageGroup = gliAgeGroup.Where(x => x.ActivityId == activityId).ToList();
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                        _log.Error(
                                    new IsangoErrorEntity
                                    {
                                        ClassName = nameof(MasterService),
                                        MethodName = nameof(GetPrioAgeGroupAsync),
                                        Params = $"ActivityId: {activityId}"
                                    }, ex
                            )
                    );
            }
            return await Task.FromResult(ageGroup);
        }

        public async Task<List<NetPriceMasterData>> LoadNetPriceMasterDataAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadNetPriceMasterData());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "LoadNetPriceMasterDataAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<PriceCategory>> GetBokunPriceCategoryByActivityAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.GetBokunPriceCategoryByActivity());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetBokunPriceCategoryByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<HotelBedsCredentials>> GetHBAuthorizationDataAsync()
        {
            try
            {
                return await Task.FromResult(_masterCacheManager.GetHBAuthorizationData());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetHBAuthorizationDataAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<PickupLocation>> GetPickupLocationsByActivityAsync(int activityId)
        {
            try
            {
                return await Task.FromResult(_pickupLocationsCacheManager.GetPickupLocationsByActivity(activityId));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetPickupLocationsByActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        ///  This method returns list of the Affiliate table.
        /// </summary>
        /// <returns></returns>
        public async Task<AffiliateAPI> GetDeltaAffiliateAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaAffiliate());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetDeltaAffiliateAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Localized Merchandising table.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task<List<LocalizedMerchandising>> GetDeltaAttractionsAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaAttractions());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetDeltaAttractionsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Region Attraction table.
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegionCategoryMapping>> GetDeltaRegionAttractionAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaRegionAttraction());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetDeltaRegionAttractionAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Delta Region Attraction table.
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegionSubAttraction>> GetDeltaRegionSubAttractionAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaRegionSubAttraction());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetDeltaRegionSubAttractionAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Marketing CJ Feed.
        /// </summary>
        /// <param name="currencyid"></param>
        /// <returns></returns>
        public async Task<string> GetMarketingCJFeedAsync(int currencyid)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadMarketingCJFeed(currencyid));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetMarketingCJFeedAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Marketing Criteo Feed.
        /// </summary>
        /// <param name="currencycode"></param>
        /// <returns></returns>
        public async Task<List<CJFeedProduct>> GetMarketingCriteoFeedAsync(string currencycode)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadMarketingCriteoFeed(currencycode));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetMarketingCriteoFeedAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the passenger mappings for Golden Tours
        /// </summary>
        /// <returns></returns>
        public async Task<List<GoldenTours.PassengerMapping>> GetPassengerMapping(APIType apiType)
        {
            try
            {
                var passengerMappings = _masterCacheManager.GetGoldenToursPaxMappings();
                if (passengerMappings?.Any() != true)
                {
                    passengerMappings = _masterPersistence.GetPassengerMapping()?.Where(x => x.ApiType == (int)apiType)
                        ?.ToList();
                }
                return await Task.FromResult(passengerMappings);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetPassengerMapping"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the GeoDetail Data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<GeoDetails>> GetGeoDetailAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaGeoDetails());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetGeoDetailAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Destination Data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Destination>> GetDestinationAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaDestination());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetGeoDetailAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
		/// This method returns list of the Product Supplier Data.
		/// </summary>
		/// <returns></returns>
		public async Task<List<ProductSupplier>> GetProductSupplierAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadDeltaProductSupplier());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetProductSupplierAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Currency Data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Currency>> GetCurrencyAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadMasterCurrency());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetCurrencyAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Languages Data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Language>> GetMasterLanguagesAsync()
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadMasterLanguages());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetMasterLanguagesAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the GeoDetail Data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<GeoDetails>> GetMasterGeoDetailAsync(string language)
        {
            try
            {
                return await Task.FromResult(_masterPersistence.LoadMasterGeoDetail(language));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetMasterGeoDetailAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Region Wise Data.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<List<RegionWiseProductDetails>> GetMasterRegionWiseAsync(string affiliateId, string categoryid = null, string B2B = null)
        {
            try
            {
                if (B2B == "true")
                {
                    return await Task.FromResult(_masterPersistence.LoadMasterRegionWise(affiliateId, categoryid));

                }
                else
                {
                    return await Task.FromResult(_masterPersistence.LoadMasterRegionWise(affiliateId));
                }

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetMasterRegionWiseAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<Entities.Rezdy.RezdyLabelDetail>> GetLabelDetailsAsync()
        {
            try
            {
                var labelDetails = _masterCacheManager.GetRezdyLabelDetails() ?? _masterPersistence.GetRezdyLabelDetails().ToList();
                return await Task.FromResult(labelDetails);
            }
            catch (Exception ex)
            {
                _log.Error($"MasterService|GetLabelDetails", ex);
                throw;
            }
        }

        public async Task<List<Entities.Rezdy.RezdyPaxMapping>> GetPaxMappingsAsync()
        {
            try
            {
                var paxMappings = _masterCacheManager.GetRezdysPaxMappings() ?? _masterPersistence.GetRezdyPaxMappings().ToList();
                return await Task.FromResult(paxMappings);
            }
            catch (Exception ex)
            {
                _log.Error($"MasterService|GetLabelDetails", ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Currency Data.
        /// </summary>
        /// <returns></returns>
        public async Task<PaymentMethodsResponse> GetAdyenPaymentMethodsAsync(string countryCode
            , string shopperLocale, string amount, string currency)
        {
            try
            {
                var paymentMethods = default(PaymentMethodsResponse);
                var key = $"PaymentMethods_{countryCode}_{shopperLocale}_{currency}_{amount}";
                if (!CacheHelper.Exists(key) || !CacheHelper.Get<PaymentMethodsResponse>(key, out paymentMethods))
                {
                    paymentMethods = await Task.FromResult(_adyenAdapter.
                        PaymentMethods(countryCode
                  , shopperLocale, Math.Ceiling((amount?.ToDecimal() ?? 0) * 100).ToString("0"), currency, "adyenPaymentMethods"));

                    CacheHelper.Set<PaymentMethodsResponse>(key, paymentMethods);
                }
                return paymentMethods;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetAdyenPaymentMethodsAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<string> GetAdyenPaymentMethodsV2Async(string countryCode
            , string shopperLocale, string amount, string currency)
        {
            try
            {
                var paymentMethods = string.Empty;
                var key = $"PaymentMethodsV2_{countryCode}_{shopperLocale}_{currency}_{amount}";
                if (!CacheHelper.Exists(key) || !CacheHelper.Get<string>(key, out paymentMethods))
                {
                    paymentMethods = await Task.FromResult(_adyenAdapter.
                        PaymentMethodsV2(countryCode
                  , shopperLocale, Math.Ceiling((amount?.ToDecimal() ?? 0) * 100).ToString("0"), currency, "adyenPaymentMethods"));

                    CacheHelper.Set<string>(key, paymentMethods);
                }
                return paymentMethods;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetAdyenPaymentMethodsV2Async"
                };
                _log.Error(isangoErrorEntity, ex);
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
                _log.Error("MasterService|GetVentrataData", ex);
                throw;
            }
        }

        public AvailablePersonTypes GetPersonTypeOptionCacheAvailability(int? activityId = null, int? serviceOptionId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var result = default(AvailablePersonTypes);
            var fromDateString = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
            var toDateString = Convert.ToDateTime(toDate).ToString("yyyy-MM-dd");
            var key = $"GetPersonTypeOptionCacheAvailability_{serviceOptionId}_{fromDateString}_{toDateString}"?.ToLower();
            try
            {
                if (!CacheHelper.Exists(key) || !CacheHelper.Get(key, out result))
                {
                    if (result == null)
                    {
                        try
                        {
                            result = _masterPersistence.GetPersonTypeOptionCacheAvailability(null, serviceOptionId, fromDate, toDate);
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                                 _log.Error(new IsangoErrorEntity
                                 {
                                     ClassName = "MasterService",
                                     MethodName = "GetPersonTypeOptionCacheAvailability",
                                     Params = $"ActivityId - {activityId}\n ,serviceOptionId -{serviceOptionId}"
                                 }, ex)
                                 );
                        }

                        if (result != null && result?.AvailableDates?.Count > 0 && result?.AvailablePassengerTypes?.Count > 0)
                        {
                            CacheHelper.Set(key, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("MasterService|GetPersonTypeOptionCacheAvailability", ex);
                throw;
            }

            //if (activityId > 0)
            //{
            //    result = result?.Where(x => x.ServiceId == activityId)?.ToList();
            //}
            //if (serviceOptionId > 0)
            //{
            //    result = result?.Where(x => x.ServiceOptionId == serviceOptionId)?.ToList();
            //}
            //if (fromDate != null && fromDate != default(DateTime)
            //    && toDate != null && toDate != default(DateTime)
            //)
            //{
            //    result = result.Where(x =>
            //    fromDate >= x.FromDate && fromDate <= x.ToDate
            //    && toDate >= x.FromDate && toDate <= x.ToDate
            //    )?.ToList()
            //    ?.OrderBy(y => y.Capacity)?.Take(1)?.ToList();
            //}

            return result;
        }

        public void SaveImageAltText(string ImageName, string AltText)
        {
            try
            {
                _masterPersistence.SaveImageAltText(ImageName, AltText);
            }
            catch (Exception ex)
            {
                var isangoerrorentity = new IsangoErrorEntity
                {
                    ClassName = "masterservice",
                    MethodName = "SaveImageAltText"
                };
                _log.Error(isangoerrorentity, ex);
            }
        }
    }
}