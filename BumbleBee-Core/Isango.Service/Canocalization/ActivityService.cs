using CacheManager.Contract;
using Factories;
using GeoTimeZone;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Redeam;
using Isango.Entities.Region;
using Isango.Entities.Ticket;
using Isango.Persistence.Contract;
using Isango.Service.Canocalization;
using Isango.Service.Contract;
using Isango.Service.Factory;
using Logger.Contract;
using ServiceAdapters.Redeam;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TimeZoneConverter;
using Util;
using Constant = Isango.Service.Constants.Constant;

namespace Isango.Service
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityCacheManager _activityCacheManager;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IHotelBedsActivitiesCacheManager _hotelBedsActivitiesCacheManager;
        private readonly IActivityPersistence _activityPersistence;
        private readonly ILogger _log;
        private readonly IAffiliateService _affiliateService;
        private readonly IAttractionActivityMappingCacheManager _attractionActivityMappingCacheManager;
        private readonly INetPriceCacheManager _netPriceCacheManager;
        private readonly IMasterService _masterService;
        private readonly ICalendarAvailabilityCacheManager _calendarAvailabilityCacheManager;
        private readonly SupplierFactory _supplierFactory;
        private readonly IApplicationService _applicationService;
        private readonly ServiceAdapters.HB.IHBAdapter _hBAdapter;
        private readonly IRedeamAdapter _redeamAdapter;
        private static readonly int _maxParallelThreadCount;
        private static string[] _apiAvailabilityFromDB;
        private static bool _isCosmosInsertDeleteLogging;
        private readonly ICanocalizationService _icanocalizationService;

        static ActivityService()
        {
            try
            {
                _apiAvailabilityFromDB = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.APIIdsAvailabilityFromDB)?.Split(',');
                _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Constant.MaxParallelThreadCount);
                try
                {
                    _isCosmosInsertDeleteLogging = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsCosmosInsertDeleteLogging) == "1";
                }
                catch (Exception)
                {
                    _isCosmosInsertDeleteLogging = false;
                }
            }
            catch (Exception)
            {
                _apiAvailabilityFromDB = new string[] { "0", "1", "12" };
                _maxParallelThreadCount = 1;
            }
        }

        public ActivityService(IActivityPersistence activityPersistence
            , IActivityCacheManager activityCacheManager
            , IMasterCacheManager masterCacheManager
            , IHotelBedsActivitiesCacheManager hotelBedsActivitiesCacheManager
            , ILogger log
            , IAffiliateService affiliateService
            , IAttractionActivityMappingCacheManager attractionActivityMappingCacheManager
            , INetPriceCacheManager netPriceCacheManager
            , IMasterService masterService

            , ICalendarAvailabilityCacheManager calendarAvailabilityCacheManager
            , SupplierFactory supplierFactory
            , ServiceAdapters.HB.IHBAdapter hBAdapter
            , IRedeamAdapter redeamAdapter
            , IApplicationService applicationService
            , ICanocalizationService icanocalizationService
            )
        {
            _activityPersistence = activityPersistence;
            _activityCacheManager = activityCacheManager;
            _masterCacheManager = masterCacheManager;
            _hotelBedsActivitiesCacheManager = hotelBedsActivitiesCacheManager;
            _log = log;
            _affiliateService = affiliateService;
            _attractionActivityMappingCacheManager = attractionActivityMappingCacheManager;
            _netPriceCacheManager = netPriceCacheManager;
            _masterService = masterService;

            _calendarAvailabilityCacheManager = calendarAvailabilityCacheManager;
            _supplierFactory = supplierFactory;
            _hBAdapter = hBAdapter;
            _redeamAdapter = redeamAdapter;
            _applicationService = applicationService;
            _icanocalizationService = icanocalizationService;
        }

        /// <summary>
        /// Method will load activity data according to client info
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="startDate"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public async Task<Activity> LoadActivityAsync(int activityId, DateTime startDate, ClientInfo clientInfo, string B2BAffiliate = null)
        {
            try
            {
                var activity = default(Activity);
                if (B2BAffiliate == "true")
                {
                    activity = GetActivityById_B2B(activityId, startDate, clientInfo?.LanguageCode ?? "en")?.GetAwaiter().GetResult();

                }
                else
                {
                    activity = GetActivityById(activityId, startDate, clientInfo?.LanguageCode ?? "en")?.GetAwaiter().GetResult();
                }

                if (activity == null)
                {
                    return null;
                }
                var key = $"affiliateFilter_{clientInfo.AffiliateId}_{clientInfo?.LanguageCode ?? "en"}";
                if (!CacheHelper.Exists(key) || !CacheHelper.Get<AffiliateFilter>(key, out AffiliateFilter affiliateFilter))
                {
                    affiliateFilter = await _affiliateService.GetAffiliateFilterByIdAsync(clientInfo?.AffiliateId);
                    if (affiliateFilter?.Activities?.Any() == true)
                    {
                        CacheHelper.Set<Activity>(key, activity);
                    }
                }

                if (affiliateFilter == null || MatchAllAffiliateCriteria(affiliateFilter, activity))
                    return activity;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadActivityAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{activityId}, {SerializeDeSerializeHelper.Serialize(startDate)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        private List<Activity> FilterCrossSell(List<Activity> crossSellproducts, List<CrossSellLogic> crossSellLogic, int? lineOfBusinessId)
        {
            //ActivityService activityService = new ActivityService();
            var productsToShow = new List<Activity>();
            try
            {
                // Final XSell products that are in same region as the selected products but are having different attraction Ids.
                //var filteredProducts = new List<CrossSellProduct>();
                var crossSellBusinessId = crossSellLogic.Where(x => x.LineofBusinessid == lineOfBusinessId)?.FirstOrDefault()?.CrossSellBusiness1;

                var filteredProducts = (crossSellproducts.Where(w => w.LineOfBusinessId == crossSellBusinessId).ToList());

                productsToShow = filteredProducts.Take(3).ToList();

                return productsToShow;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "UIHelper",
                    MethodName = "FilterCrossSell",
                    Params = string.Empty
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return productsToShow;
            //Return top 3 products
        }

        private List<Activity> GetActivitiesToAddInCrossSellProducts(List<CrossSellProduct> crossSellProducts)
        {
            var productsToShow = new List<Activity>();
            try
            {
                crossSellProducts = (from cp in crossSellProducts orderby cp.Priority select cp).ToList();
                foreach (var item in crossSellProducts)
                {
                    var activityFromBBCache = GetActivityById(item.Id, DateTime.Today, "en")?.GetAwaiter().GetResult();

                    if (activityFromBBCache != null)
                    {
                        productsToShow.Add(activityFromBBCache);
                        if (productsToShow.Count.Equals(3))
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "UIHelper",
                    MethodName = "GetActivitiesToAddInCrossSellProducts",
                    Params = string.Empty,
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return productsToShow;
        }

        public async Task<List<Activity>> GetCrossSellProducts(int? lineOfBusinessId, Affiliate affiliate, ClientInfo clientInfo, string regionId)
        {
            try
            {
                var crossSellLogic = _masterService.GetCrossSellLogic();

                var crossSellBusinessIdFirst = crossSellLogic.Where(x => x.LineofBusinessid == lineOfBusinessId)?.FirstOrDefault()?.CrossSellBusiness1;

                var crossSellBusinessIdSecond = crossSellLogic.Where(x => x.LineofBusinessid == lineOfBusinessId)?.FirstOrDefault()?.CrossSellBusiness2;

                var crossSellProductsFirst = _activityCacheManager.GetAllActivitiesWithLineOfBusiness(crossSellBusinessIdFirst, clientInfo?.LanguageCode, regionId)?.ToList();

                var crossSellProductsSecond = _activityCacheManager.GetAllActivitiesWithLineOfBusiness(crossSellBusinessIdSecond, clientInfo?.LanguageCode, regionId)?.ToList();

                var distinctActivitiesFirst = crossSellProductsFirst?.Select(x => x.ID)?.Distinct()?.ToList();
                var distinctActivitiesSecond = crossSellProductsSecond?.Select(x => x.ID)?.Distinct()?.ToList();
                var pricesFirst = GetSearchAffiliateWiseServiceMinPrice(distinctActivitiesFirst, affiliate, clientInfo);
                var pricesSecond = GetSearchAffiliateWiseServiceMinPrice(distinctActivitiesSecond, affiliate, clientInfo);
                foreach (var item in crossSellProductsFirst)
                {
                    item.GateBaseMinPrice = pricesFirst.Where(x => x.Key == item.ID).Select(x => x.Value.Item1).FirstOrDefault();
                    item.BaseMinPrice = item.SellMinPrice = pricesFirst.Where(x => x.Key == item.ID).Select(x => x.Value.Item2).FirstOrDefault();
                    item.CurrencyIsoCode = pricesFirst.Where(x => x.Key == item.ID).Select(x => x.Value.Item3).FirstOrDefault();
                }

                foreach (var item in crossSellProductsSecond)
                {
                    item.GateBaseMinPrice = pricesSecond.Where(x => x.Key == item.ID).Select(x => x.Value.Item1).FirstOrDefault();
                    item.BaseMinPrice = item.SellMinPrice = pricesSecond.Where(x => x.Key == item.ID).Select(x => x.Value.Item2).FirstOrDefault();
                    item.CurrencyIsoCode = pricesSecond.Where(x => x.Key == item.ID).Select(x => x.Value.Item3).FirstOrDefault();
                }

                var FinalProducts = new List<Activity>();

                if (crossSellProductsFirst?.Count > 0)
                {
                    var OrderedProductsFirst = crossSellProductsFirst.OrderBy(x => x.Priority).ToList();
                    var filteredProductsFirst = crossSellProductsFirst?.Where(x => x.GateBaseMinPrice > 0 && x.SellMinPrice > 0)?.ToList()?.Take(2)?.ToList();
                    if (filteredProductsFirst?.Count > 0)
                    {
                        FinalProducts.Add(filteredProductsFirst?.FirstOrDefault());
                    }
                }

                if (crossSellProductsSecond?.Count > 0)
                {
                    var OrderedProductsSecond = crossSellProductsSecond.OrderBy(x => x.Priority).ToList();
                    var filteredProductsSecond = crossSellProductsSecond?.Where(x => x.GateBaseMinPrice > 0 && x.SellMinPrice > 0)?.ToList()?.Take(2)?.ToList();
                    if (filteredProductsSecond?.Count > 0)
                    {
                        foreach (var product in filteredProductsSecond)
                        {
                            if (!FinalProducts.Any(x => x.ID == product?.ID))
                            {
                                FinalProducts.Add(product);
                            }
                        }
                    }
                }

                if (crossSellProductsFirst?.Count > 1)
                {
                    var OrderedProductsFirst = crossSellProductsFirst.OrderBy(x => x.Priority).ToList();
                    var filteredProductsFirst = crossSellProductsFirst?.Where(x => x.GateBaseMinPrice > 0 && x.SellMinPrice > 0)?.ToList()?.Take(2)?.ToList();
                    foreach (var product in filteredProductsFirst)
                    {
                        if (!FinalProducts.Any(x => x.ID == product?.ID))
                        {
                            FinalProducts.Add(product);
                        }
                    }
                }
                else if (crossSellProductsSecond?.Count > 0)
                {
                    var OrderedProductsSecond = crossSellProductsSecond.OrderBy(x => x.Priority).ToList();
                    var filteredProductsSecond = crossSellProductsSecond?.Where(x => x.GateBaseMinPrice > 0 && x.SellMinPrice > 0)?.ToList()?.Take(2)?.ToList();
                    foreach (var product in filteredProductsSecond)
                    {
                        if (!FinalProducts.Any(x => x.ID == product?.ID))
                        {
                            FinalProducts.Add(product);
                        }
                    }
                }

                return await Task.FromResult(FinalProducts?.Take(2)?.ToList());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterService",
                    MethodName = "GetCrossSellProducts"
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
            }
        }

        public Dictionary<int, Tuple<decimal, decimal, string>> GetSearchAffiliateWiseServiceMinPrice(List<int> activities, Affiliate affiliate, ClientInfo clientInfo)
        {
            if (activities?.Any() == false)
            {
                return null;
            }
            var resultPrice = Tuple.Create(decimal.MaxValue, decimal.MaxValue, string.Empty);
            var finalResult = new Dictionary<int, Tuple<decimal, decimal, string>>();
            var masterCurrency = _masterService.GetCurrencyAsync().Result;
            var currencyIsoCode = masterCurrency?.FirstOrDefault(c => c.Symbol == clientInfo?.Currency?.Symbol).IsoCode;

            var targetCurrency = clientInfo?.Currency?.IsoCode ?? currencyIsoCode ?? Constant.DefaultCurrencyISOCode;

            var isB2BNetPriceAffiliate = affiliate?.AffiliateConfiguration?.IsB2BNetPriceAffiliate ?? false;
            var isSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;
            var affiliateWiseServiceMinPrices = GetAffiliateWiseServiceMinPrices();
            var b2bNetRateRule = _applicationService.GetB2BNetRateRuleAsync(affiliate.Id)?.GetAwaiter().GetResult();

            var netRatePercent = b2bNetRateRule?.NetRatePercent / 100;
            var netPriceType = b2bNetRateRule?.NetPriceType;

            foreach (var serviceId in activities)
            {
                var price = new AffiliateWiseServiceMinPrice
                {
                    AffiliateId = affiliate.Id,
                    BasePrice = decimal.MaxValue,
                    CostPrice = decimal.MaxValue,
                    CurrencyIsoCode = Constant.DefaultCountryISOCode,
                    OfferPercent = 0,
                    SellPrice = decimal.MaxValue,
                    ServiceId = serviceId
                };

                var serviceMinPrice = affiliateWiseServiceMinPrices?.FirstOrDefault(x =>

                             x.ServiceId == serviceId &&
                             string.Equals(x.AffiliateId, affiliate.Id
                                         , System.StringComparison.OrdinalIgnoreCase)
                          );

                if (!(serviceMinPrice?.BasePrice > 0 && serviceMinPrice?.SellPrice > 0))
                {
                    serviceMinPrice = affiliateWiseServiceMinPrices?.FirstOrDefault(x =>

                              x.ServiceId == serviceId &&
                              string.Equals(x.AffiliateId, "default"
                                          , System.StringComparison.OrdinalIgnoreCase)
                           );
                }

                if (serviceMinPrice?.BasePrice > 0
                       && serviceMinPrice?.SellPrice > 0
                       && !string.IsNullOrWhiteSpace(serviceMinPrice.CurrencyIsoCode)
                       && !string.IsNullOrWhiteSpace(targetCurrency)
                )
                {
                    var costPrice = serviceMinPrice.CostPrice;
                    var basePrice = serviceMinPrice.BasePrice;
                    var sellPrice = serviceMinPrice.SellPrice;
                    var sellPriceComputed = serviceMinPrice.SellPrice;

                    #region B2bNetRule application

                    if (isB2BNetPriceAffiliate)
                    {
                        if (netPriceType == 2)
                        {
                            sellPriceComputed = (costPrice) * (100 / (100 - (netRatePercent * 100))) ?? serviceMinPrice.SellPrice;
                        }
                        else if (isSupplementOffer && netPriceType == 1)
                        {
                            sellPriceComputed = (costPrice + ((sellPrice - costPrice) * netRatePercent)) ?? serviceMinPrice.SellPrice;
                        }
                        else if (netPriceType == 1)
                        {
                            sellPriceComputed = (costPrice + ((basePrice - costPrice) * netRatePercent)) ?? serviceMinPrice.SellPrice;
                        }
                    }
                    else if (isSupplementOffer)
                    {
                        sellPriceComputed = serviceMinPrice.SellPrice;
                    }
                    else
                    {
                        sellPriceComputed = serviceMinPrice.BasePrice;
                    }

                    #endregion B2bNetRule application

                    #region update Prices as per  customer currency

                    price.CurrencyIsoCode = targetCurrency;
                    price.BasePrice = GetContextPrice(basePrice, serviceMinPrice.CurrencyIsoCode, clientInfo, 2, targetCurrency);
                    price.SellPrice = GetContextPrice(sellPriceComputed, serviceMinPrice.CurrencyIsoCode, clientInfo, 2, targetCurrency);
                    price.CostPrice = GetContextPrice(costPrice, serviceMinPrice.CurrencyIsoCode, clientInfo, 2, targetCurrency);
                    resultPrice = Tuple.Create(price.BasePrice, price.SellPrice, targetCurrency);
                    finalResult.Add(serviceId, resultPrice);

                    #endregion update Prices as per  customer currency
                }
            }

            return finalResult;
        }

        public decimal GetContextPrice(decimal basePrice, string baseCurrency, ClientInfo clientInfo, int roundOff = 2, string targetCurrency = "")
        {
            try
            {
                decimal exchangeRate = GetCurrencyExchangeRate(baseCurrency, clientInfo, targetCurrency);
                return Math.Round(basePrice * exchangeRate, roundOff);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public decimal GetCurrencyExchangeRate(string baseCurrency, ClientInfo clientInfo, string targetCurrency = "")
        {
            decimal exchangeRate = 1.0m;
            if (!string.IsNullOrWhiteSpace(baseCurrency))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(targetCurrency))
                    {
                        //GET CURRENT CONTEXT CURRENCY
                        var currentContextCurrency = clientInfo;
                        if (currentContextCurrency != null)
                        {
                            targetCurrency = currentContextCurrency.Currency.IsoCode.ToUpper();
                        }
                    }

                    var currencyExchangeRates = GetCurrencyExchangeRates();

                    //GET EXCHANGE RATE FROM BASE CURRENCY
                    var currency = currencyExchangeRates
                                   ?.FirstOrDefault(x =>
                                            string.Equals(x.FromCurrencyCode, baseCurrency
                                                            , StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(x.ToCurrencyCode, targetCurrency
                                                            , StringComparison.OrdinalIgnoreCase)
                                            );
                    if (currency != null && !string.IsNullOrEmpty(targetCurrency))
                    {
                        if (currency.ToCurrencyCode != null)
                        {
                            exchangeRate = currency.ExchangeRate;
                        }
                    }
                    return exchangeRate;
                }
                catch (Exception ex)
                {
                    var msg = $"PriceExtension : GetCurrencyExchangeRate() baseCurrency : {baseCurrency}, targetCurrency {targetCurrency} ";

                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "PriceExtension",
                        MethodName = "GetCurrencyExchangeRate",
                        Params = msg,
                        //Token = SessionHelper.Get<string>(Constants.SessionKey.TokenId),
                    };
                    //_logService.Error(isangoErrorEntity, ex);
                }
            }
            return exchangeRate;
        }

        public List<CurrencyExchangeRates> GetCurrencyExchangeRates()
        {
            var key = Constant.GetCurrencyExchangeRates;
            var exchangeRates = default(List<CurrencyExchangeRates>);

            if (!CacheHelper.Exists(key) || !CacheHelper.Get(key, out exchangeRates))
            {
                exchangeRates = _masterService.LoadCurrencyExchangeRatesAsync()?.GetAwaiter().GetResult();

                try
                {
                    CacheHelper.Set(key, exchangeRates);
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
            return exchangeRates;
        }

        public List<AffiliateWiseServiceMinPrice> GetAffiliateWiseServiceMinPrices()
        {
            var key = Constant.AffiliateWiseServiceMinPrice;
            var serviceMinPrices = default(List<AffiliateWiseServiceMinPrice>);

            if (!CacheHelper.Exists(key) || !CacheHelper.Get(key, out serviceMinPrices))
            {
                serviceMinPrices = _affiliateService.GetAffiliateInformationAsync()?.GetAwaiter().GetResult();

                try
                {
                    CacheHelper.Set(key, serviceMinPrices);
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
            return serviceMinPrices;
        }

        /// <summary>
        /// Fetches the activity details with the available product options with live price
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<Activity> GetProductAvailabilityAsync(int activityId, ClientInfo clientInfo, Criteria criteria)
        {
            var activity = default(Activity);
            try
            {
                activity = LoadActivityAsync(activityId, criteria.CheckinDate, clientInfo).GetAwaiter().GetResult();
                if (activity == null)
                {
                    var message = $"Activity {activityId} " + Constant.NotFound;
                    SendException(clientInfo, criteria, activityId, message, "GetProductAvailabilityAsync", "ActivityService");
                }

                var passengerInfos = activity?.PassengerInfo.Any() == true ? activity?.PassengerInfo : new List<Entities.Booking.PassengerInfo> {
                    new Entities.Booking.PassengerInfo
                    {
                        ActivityId = activityId,
                        FromAge = 2,
                        ToAge = 999,
                        PassengerTypeId = (int)(PassengerType.Adult),
                        PaxDesc = PassengerType.Adult.ToString(),
                        MinSize = 1,
                        MaxSize = 10,
                        IndependablePax = true
                    }
                };

                var filterPassengerInfosAsPerCritera = from pi in passengerInfos
                                                       from ci in criteria?.NoOfPassengers
                                                       where pi.PassengerTypeId == Convert.ToInt32(ci.Key)
                                                       select pi;

                if (filterPassengerInfosAsPerCritera.Any())
                {
                    passengerInfos = filterPassengerInfosAsPerCritera.ToList();
                    criteria.PassengerInfo = passengerInfos;
                }

                var anyIndependentBookablePax = passengerInfos.FirstOrDefault(x => x.IndependablePax);
                if (anyIndependentBookablePax == null)
                {
                    //At least one pax required that can be booked independently.
                    //return null;
                    var message = Constant.OnePaxRequired;
                    SendException(clientInfo, criteria, activityId, message, "GetProductAvailabilityAsync", "ActivityService");
                }
                if (criteria?.Ages?.Any() == false || criteria?.Ages == null)
                {
                    criteria.Ages = new Dictionary<PassengerType, int>();
                }
                if (passengerInfos?.Count > 0)
                {
                    foreach (var passengerInfo in passengerInfos)
                    {
                        if (passengerInfo != null)
                        {
                            var paxType = (PassengerType)passengerInfo.PassengerTypeId;
                            if (paxType == PassengerType.Adult)
                            {
                                if (criteria?.Ages?.ContainsKey(paxType) == false)
                                {
                                    criteria.Ages.Add(paxType, 30);
                                }
                            }
                            else
                            {
                                if (!criteria.Ages.ContainsKey(paxType))
                                {
                                    criteria.Ages.Add(paxType, passengerInfo.ToAge);
                                }
                            }
                            //criteria.PassengerAgeGroupIds[paxType] = passengerInfo.AgeGroupId;
                        }
                    }
                }
                if (criteria?.Ages?.Count < 1)
                {
                    var message = Constant.AgeGroupNotMatched;
                    SendException(clientInfo, criteria, activityId, message, "GetProductAvailabilityAsync", "ActivityService");
                }

                activity.ProductOptions?.ForEach(x =>
                {
                    if (x.TravelInfo == null) x.TravelInfo = new TravelInfo();
                    x.TravelInfo.StartDate = criteria.CheckinDate;
                    x.TravelInfo.NoOfPassengers = criteria.NoOfPassengers;
                    x.TravelInfo.Ages = criteria.Ages;
                });

                //Get DatePriceAndAvailabilities for the iSango products

                var IsValidToGetAvailabilityFromDb = ValidAPIToGetAvailabilityFromDB((int)activity.ApiType);
                var orgCheckinDate = criteria.CheckinDate;
                var orgCheckOutDate = criteria.CheckoutDate;
                //var watch = System.Diagnostics.Stopwatch.StartNew();
                //long time = 0;
                if (IsValidToGetAvailabilityFromDb)
                {
                    activity.ProductOptions = LoadDefaultActivityWithPrice(activity, criteria, clientInfo);
                    //watch.Stop();
                    //time = watch.ElapsedMilliseconds;
                }
                else
                {
                    //Get live price and availabilities
                    var result = new List<Activity> { activity };
                    //watch = System.Diagnostics.Stopwatch.StartNew();
                    activity = GetLivePriceByApiType(result, clientInfo, criteria)?.FirstOrDefault();
                    //watch.Stop();
                    //time = watch.ElapsedMilliseconds;
                }

                activity.ProductOptions?.ForEach(x =>
                {
                    x.ApiType = activity.ApiType;
                    if (x.PriceTypeID == 0 && (int)activity.PriceTypeId != 0)
                    {
                        x.PriceTypeID = (int)activity.PriceTypeId;
                    }
                });

                if (activity.ApiType == APIType.Redeam)
                {
                    activity = FilterAvailabilityfromDB(activity, criteria, clientInfo);
                }
                
                //Remove options where option does not contain same passengers types as of input criteria.
                ValidateOptionPaxCriteria(activity.ProductOptions, criteria.NoOfPassengers, orgCheckinDate, orgCheckOutDate, clientInfo.ApiToken, activity);

                //Remove datePriceAndAvailabilty from option as requested PaxCount did not match with Capacity coming from supplier response.
                ValidateOptionForCapacity(activity.ProductOptions, criteria.NoOfPassengers);

                activity.ProductOptions?.RemoveAll(x =>
                       (x.BasePrice == null || x.BasePrice?.Amount == 0 || x.BasePrice.DatePriceAndAvailabilty?.Count == 0 || x.BasePrice.DatePriceAndAvailabilty == null) && (x.CostPrice == null ||
                        x.CostPrice?.Amount == 0 || x.CostPrice.DatePriceAndAvailabilty?.Count == 0 || x.CostPrice.DatePriceAndAvailabilty == null));

                if (activity?.ProductOptions?.Any() == false)
                {
                    var message = Constant.ProductOptionPriceNotExist;
                    UpdateError(activity, criteria, Util.ErrorCodes.AVAILABILITY_ERROR, message);
                }
            }
            catch (Exception ex)
            {
                var message = $"Activity {activity.Id}\r\n{ex.Message + ex.StackTrace}";
                UpdateError(activity, criteria, Util.ErrorCodes.AVAILABILITY_ERROR, message);

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityDetailsAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{activityId}, {SerializeDeSerializeHelper.Serialize(clientInfo)}, {SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                //SendException(clientInfo, criteria, activityId, string.Empty, "GetProductAvailabilityAsync", "ActivityService", ex);
                //throw;
            }
            return await Task.FromResult(activity);
        }

        /// <summary>
        /// Fetches the activity details with live price for the products having IsLivePrice flag true
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<Activity> GetActivityDetailsAsync(int activityId, ClientInfo clientInfo, Criteria criteria)
        {
            try
            {
                var activity = GetActivityDetailsWithLivePrice(activityId, clientInfo, criteria);

                //var activity = await GetProductAvailabilityAsync(activityId, clientInfo, criteria);
                return await Task.FromResult(activity);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityDetailsAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{activityId}, {SerializeDeSerializeHelper.Serialize(clientInfo)}, {SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<ProductOption>> GetAllOptionsAvailabilityAsync(Activity activity, ClientInfo clientInfo)
        {
            try
            {
                var travelInfo = activity.ProductOptions.First().TravelInfo;
                var startDate = travelInfo.StartDate;
                var endDate = travelInfo.StartDate.AddDays(travelInfo.NumberOfNights);
                var productOptionList = _activityPersistence.GetAllOptionsAvailability(activity, startDate, endDate);

                return await Task.FromResult(productOptionList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetAllOptionsAvailability",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(activity)},{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Isango products Availabilities from db
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public async Task<List<ProductOptionAvailabilty>> GetAllOptionsAvailabilitiesAsync(Activity activity, ClientInfo clientInfo)
        {
            try
            {
                var travelInfo = activity.ProductOptions.First().TravelInfo;
                var startDate = travelInfo.StartDate;
                var endDate = travelInfo.StartDate.AddDays(travelInfo.NumberOfNights);
                var productOptionList = _activityPersistence.GetAllOptionsAvailabilities(activity, startDate, endDate);

                return await Task.FromResult(productOptionList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetAllOptionsAvailabilitiesAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(activity)},{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method will update the activity for the MIN prices of Base, Cost, Sell, GateBase and GateSell at product level
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public async Task<Activity> CalculateActivityWithMinPricesAsync(Activity activity)
        {
            try
            {
                activity = activity.ActivityType != ActivityType.Bundle
              ? CalculateActivityWithMinPrices(activity)
              : CalculateBundleActivityWithMinPrices(activity);

                return await Task.FromResult(activity);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CalculateActivityWithMinPricesAsync",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activity)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        ///  <summary>
        ///  Get calendar availability
        ///  </summary>
        ///  <param name="productId"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        ///
        public async Task<List<CalendarAvailability>> GetCalendarAvailabilityAsync(int productId, string affiliateId)
        {
            try
            {
                var calendarList = GetCalendarAvailability(productId, affiliateId);
                return await Task.FromResult(calendarList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetCalendarAvailabilityAsync",
                    AffiliateId = affiliateId,
                    Params = $"{productId},{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Operation to fetch activity details with calendar details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActivityDetailsWithCalendarResponse> GetActivityDetailsWithCalendar(ActivityDetailsWithCalendarRequest request, string B2BAffiliate = null)
        {
            try
            {
                var activity = GetActivityDetailsWithLivePrice(request.ActivityId, request.ClientInfo, request.Criteria, B2BAffiliate);
                var calendarAvailability = GetCalendarAvailability(request.ActivityId, request.ClientInfo.AffiliateId);
                var activityDetailsWithCalendar = MapActivityDetailsWithCalendar(activity, calendarAvailability);

                return await Task.FromResult(activityDetailsWithCalendar);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityDetailsWithCalendar",
                    Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Operation to fetch pax price
        /// </summary>
        /// <param name="paxPriceRequest"></param>
        /// <returns></returns>
        public async Task<List<OptionDetail>> GetPaxPriceAsync(PaxPriceRequest paxPriceRequest)
        {
            try
            {
                var result = _activityPersistence.GetPaxPrices(paxPriceRequest);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetPaxPriceAsync",
                    Params = $"{paxPriceRequest}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<WidgetMappedData>> GetWidgetData()
        {
            try
            {
                var result = _activityPersistence.GetRegionMappedDataForWidget();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetPaxPriceAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Operation to check availability of bundle product
        /// </summary>
        /// <param name="bundleActivityId"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteriaForActivity"></param>
        /// <returns></returns>
        public async Task<Activity> GetBundleProductAvailabilityAsync(int bundleActivityId, ClientInfo clientInfo, Dictionary<int, Criteria> criteriaForActivity)
        {
            try
            {
                //As we are not getting Check-In date for Bundle activity, Check-In date for first activity in request is passed.
                var bundleActivityFromCache = LoadActivityAsync(bundleActivityId, criteriaForActivity.FirstOrDefault().Value.CheckinDate, clientInfo)?.GetAwaiter().GetResult();

                if (bundleActivityFromCache == null) return null;
                bundleActivityFromCache.Errors = new List<Error>();
                if (bundleActivityFromCache.ActivityType == ActivityType.Bundle)
                {
                    //get component Activity
                    var componentActivityIdList = bundleActivityFromCache?.ProductOptions?.Select(x => x.ComponentServiceID)?.Distinct()?.ToList();

                    var productOptionlist = new List<ProductOption>();

                    //fill component activity details
                    foreach (var componentActivityId in componentActivityIdList)
                    {
                        try
                        {
                            //var bundleComponentOptionIds = bundleActivityFromCache.ProductOptions?.Where(e => e.ComponentServiceID == componentActivityId)?.Select(e => e.Id)?.Distinct()?.ToList();

                            var bundleComponentSeriveOptionIds = bundleActivityFromCache.ProductOptions?.Where(e => e.ComponentServiceID == componentActivityId)?.Select(e => e.ServiceOptionId)?.Distinct()?.ToList();

                            var componentActivityCriteria = criteriaForActivity[componentActivityId];
                            componentActivityCriteria.ActivityId = componentActivityId;

                            var componentActivityResult = GetProductAvailabilityAsync(componentActivityId, clientInfo, componentActivityCriteria)?.GetAwaiter().GetResult();

                            if (componentActivityResult?.Errors?.Any(x => x != null) == true)
                            {
                                bundleActivityFromCache.Errors.AddRange(componentActivityResult?.Errors);
                                return bundleActivityFromCache;
                            }

                            //foreach (var po in componentActivityResult.ProductOptions)
                            //{
                            //    bool isAvaialble = FilterBundlePricesAsPerCheckInDate(po.BasePrice, componentActivityCriteria);
                            //    isAvaialble = FilterBundlePricesAsPerCheckInDate(po.GateBasePrice, componentActivityCriteria);
                            //    isAvaialble = FilterBundlePricesAsPerCheckInDate(po.CostPrice, componentActivityCriteria);

                            //}

                            if (componentActivityResult == null) continue;
                            var matchedComponentOptions = componentActivityResult.ProductOptions?.Where(e => bundleComponentSeriveOptionIds.Contains(e.ServiceOptionId));
                            if (matchedComponentOptions != null)
                            {
                                foreach (var matchedComponentOption in matchedComponentOptions)
                                {
                                    var count = bundleComponentSeriveOptionIds.Count(x => x == matchedComponentOption.ServiceOptionId);

                                    for (int i = 0; i < count; i++)
                                    {
                                        if (matchedComponentOption.AvailabilityStatus == AvailabilityStatus.AVAILABLE)
                                        {
                                            productOptionlist.Add(matchedComponentOption);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "ActivityService",
                                MethodName = "GetBundleProductAvailabilityAsync",
                                Token = clientInfo.ApiToken,
                                AffiliateId = clientInfo.AffiliateId,
                                Params = $"{bundleActivityId}, {SerializeDeSerializeHelper.Serialize(criteriaForActivity)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                            };
                            bundleActivityFromCache.Errors = new List<Error>();

                            bundleActivityFromCache.Errors.Add(new Error
                            {
                                Code = Util.ErrorCodes.BUNDLE_AVAILABILITY_ERROR,
                                HttpStatus = HttpStatusCode.InternalServerError,
                                Message = ex.Message + ex.StackTrace
                            });
                            _log.Error(isangoErrorEntity, ex);
                        }
                    }

                    if (productOptionlist?.Count == 0) return null;
                    var inValidBundleOptionIds = bundleActivityFromCache.ProductOptions.Where(e => !productOptionlist.Select(x => x.ServiceOptionId).Contains(e.ServiceOptionId)).Select(x => x.BundleOptionID).ToList();
                    bundleActivityFromCache.ProductOptions.RemoveAll(e => inValidBundleOptionIds.Contains(e.BundleOptionID));

                    var newbundleOptions = new List<ProductOption>();

                    foreach (var bundleActivityProductOption in bundleActivityFromCache.ProductOptions)
                    {
                        var pos = productOptionlist?.Where(e => e.ServiceOptionId == bundleActivityProductOption.ServiceOptionId
                                    //&& e.BundleOptionID == bundleActivityProductOption.BundleOptionID
                                    )?.ToList();

                        if (pos?.Any() == false) continue;

                        foreach (var po in pos)
                        {
                            try
                            {
                                var poCopy = SerializeDeSerializeHelper.Serialize(po);
                                var poCopy1 = SerializeDeSerializeHelper.DeSerialize<ActivityOption>(poCopy);
                                var aoCopy1 = SerializeDeSerializeHelper.DeSerialize<ProductOption>(poCopy);

                                var bao = MapActivityOptionFromAPI(bundleActivityProductOption, po);
                                if (bao != null)
                                {
                                    newbundleOptions.Add(bao);
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "ActivityService",
                                    MethodName = "GetBundleProductAvailabilityAsync",
                                    Token = clientInfo.ApiToken,
                                    AffiliateId = clientInfo.AffiliateId,
                                    Params = $"{bundleActivityId}, {SerializeDeSerializeHelper.Serialize(criteriaForActivity)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                                };
                                Task.Run(() => _log.Error(isangoErrorEntity, ex));
                            }
                        }
                    }

                    if (newbundleOptions?.Any() == true)
                    {
                        bundleActivityFromCache.ProductOptions = newbundleOptions;
                    }
                }

                var unavailableBundleOptions = bundleActivityFromCache?.ProductOptions.Where(
                                                    e => e.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE
                                                    || e.AvailabilityStatus == AvailabilityStatus.ONREQUEST
                                               ).Select(e => e.BundleOptionID).Distinct();

                bundleActivityFromCache.ProductOptions.ForEach(o =>
                {
                    if (unavailableBundleOptions.Contains(o.BundleOptionID))
                    {
                        o.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    }
                }
                                                               );

                bundleActivityFromCache.ProductOptions.RemoveAll(e => unavailableBundleOptions.Contains(e.BundleOptionID));
                return await Task.FromResult(bundleActivityFromCache);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetBundleProductAvailabilityAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{bundleActivityId}, {SerializeDeSerializeHelper.Serialize(criteriaForActivity)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Fetches the activity from CosmosDB or from the database
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="startDate"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public Task<Activity> GetActivityById(int activityId, DateTime startDate, string languageCode)
        {
            var activity = default(Activity);
            try
            {
                activity = _activityCacheManager.GetActivity(activityId.ToString(), languageCode);
                if (activity?.ProductOptions?.Any() != true)
                {
                    activity = GetActivityFromPersistence(activityId, languageCode);
                    var key = $"Activity_{activityId}_{languageCode}";
                    if (activity?.ProductOptions?.Any() == true)
                    {
                        CacheHelper.Set<Activity>(key, activity);
                    }
                    else
                    {
                        var message = Constant.ActivityOptionMapping + " " + activityId;
                        SendException(null, null, activityId, message, "GetActivityById", "ActivityService");
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityFromCacheOrDatabase",
                    Params = $"{activityId}, {startDate}, {languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return Task.FromResult(activity); ;
        }
        public Task<Activity> GetActivityById_B2B(int activityId, DateTime startDate, string languageCode)
        {
            var activity = default(Activity);
            try
            {
                activity = _activityCacheManager.GetActivity(activityId.ToString(), languageCode);
                if (activity != null)
                {
                    if (activity.Images != null && activity.Images.Any())
                    {
                        var firstThreeImages = activity.Images.Take(3).ToList();
                        activity.Images = firstThreeImages;
                    }
                }
                

                activity.Introduction = null;
                activity.ReasonToBook = null;
                activity.Reviews = null;
                activity.TotalReviewCount = 0;
                activity.TotalReviews = 0;


                if (activity?.ProductOptions?.Any() != true)
                {
                    activity = GetActivityFromPersistence(activityId, languageCode);
                    var key = $"Activity_{activityId}_{languageCode}";
                    if (activity?.ProductOptions?.Any() == true)
                    {
                        CacheHelper.Set<Activity>(key, activity);
                    }
                    else
                    {
                        var message = Constant.ActivityOptionMapping + " " + activityId;
                        SendException(null, null, activityId, message, "GetActivityById", "ActivityService");
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityFromCacheOrDatabase",
                    Params = $"{activityId}, {startDate}, {languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return Task.FromResult(activity); ;
        }

        #region Private Methods

        private Activity CalculateActivityWithMinPrices(Activity activity)
        {
            try
            {
                var basePrice = activity.ProductOptions?
                        .Where(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE)?
                        .Min(x => x.BasePrice?.Amount) ?? 0;

                if (basePrice > 0)
                {
                    activity.BaseMinPrice = basePrice;
                }

                var costPrice = activity.ProductOptions?
                        .Where(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE)?
                        .Min(x => x.CostPrice?.Amount) ?? 0;

                if (costPrice > 0)
                {
                    activity.CostMinPrice = costPrice;
                }

                var gateBasePrice = activity.ProductOptions?
                        .Where(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE)?
                        .Min(x => x.GateBasePrice?.Amount) ?? 0;

                if (costPrice > 0)
                {
                    activity.GateBaseMinPrice = gateBasePrice;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CalculateActivityWithMinPrices"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// Method to calculate bundle activity minimum prices
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private Activity CalculateBundleActivityWithMinPrices(Activity activity)
        {
            try
            {
                // Assuming BasePrice, CostPrice, and GateBasePrice are of type Price or Nullable<Price>
                activity.BaseMinPrice = activity.ProductOptions
                    .Where(po => po.BasePrice != null)
                    .GroupBy(e => e.BundleOptionID)
                    .Select(g => g.Min(x => x.BasePrice.Amount))
                    .DefaultIfEmpty(0) // Default to 0 if there are no non-null values
                    .Min();

                activity.CostMinPrice = activity.ProductOptions
                    .Where(po => po.CostPrice != null)
                    .GroupBy(e => e.BundleOptionID)
                    .Select(g => g.Min(x => x.CostPrice.Amount))
                    .DefaultIfEmpty(0)
                    .Min();

                activity.GateBaseMinPrice = activity.ProductOptions
                    .Where(po => po.GateBasePrice != null)
                    .GroupBy(e => e.BundleOptionID)
                    .Select(g => g.Min(x => x.GateBasePrice.Amount))
                    .DefaultIfEmpty(0)
                    .Min();

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CalculateBundleActivityWithMinPrices"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }
        public List<Activity> GetActivitiesWithLivePrice(ClientInfo clientInfo, Criteria criteria, List<Activity> activities)
        {
            try
            {
                var activitiesForLivePrice = activities.Where(x => x.IsLivePrice).ToList();
                if (activitiesForLivePrice?.Count() == 0)
                    return activities;
                var activitiesAfterLivePrice = GetLivePriceByApiType(activitiesForLivePrice, clientInfo, criteria);
                activities.RemoveAll(x => x.IsLivePrice);
                activities.AddRange(activitiesAfterLivePrice);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivitiesWithLivePrice"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activities;
        }

        /// <summary>
        /// This method is used to map the cached activity with activities fetched from API
        /// </summary>
        /// <param name="activity">Activity loaded from cache</param>
        /// <param name="hbActivities">Activity returned from API</param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private Activity LoadMappedDataHB(Activity activity, List<Activity> hbActivities, ClientInfo clientInfo, Criteria criteria)
        {
            try
            {
                if (activity?.Regions != null
                     && activity.ProductOptions != null
                     && hbActivities?.Count > 0
                )
                {
                    var productOptions = new List<ProductOption>();
                    var updatedOptions = new List<ProductOption>();
                    var productOptionsCopy = new ProductOption[activity.ProductOptions.Count];
                    activity.ProductOptions.CopyTo(productOptionsCopy);
                    activity.ProductOptions.Clear();
                    foreach (var hbActivity in hbActivities)
                    {
                        try
                        {
                            foreach (var item in productOptionsCopy?.ToList())
                            {
                                var hbActivityOptions = hbActivity?.ProductOptions?
                                    .Where(x => x.PrefixServiceCode == item.PrefixServiceCode)?
                                    .ToList();

                                if (hbActivityOptions?.Count > 0)
                                {
                                    productOptions = hbActivityOptions
                                        .Select(p =>
                                        {
                                            //p.Name = $"{item.Name} - {p.Name}";
                                            p.Margin = item.Margin;
                                            return p;
                                        }).ToList();
                                }
                            }

                            updatedOptions = UpdateBasePricesHbApitude(productOptions, activity.HotelPickUpLocation, clientInfo, criteria)
                                ?.Where(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE)?.ToList();

                            if (updatedOptions?.Count > 0)
                            {
                                activity.ProductOptions.AddRange(updatedOptions);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"ActivityService|LoadMappedDataHB", ex);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "ActivityService",
                                MethodName = "LoadMappedDataHB",
                                Params = $"{activity.Id}"
                            };
                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadMappedDataHB"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// This method calculates the all affiliate criteria
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool MatchAllAffiliateCriteria(AffiliateFilter affiliateFilter, Product product)
        {
            var matchesAffiliateCriteria = true;
            try
            {
                if (product == null)
                    return false;

                if (affiliateFilter == null) return true;

                if (affiliateFilter.DurationTypeFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliateDurationType(affiliateFilter, product);
                }

                if (matchesAffiliateCriteria && affiliateFilter.RegionFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliateRegion(affiliateFilter, product);
                }

                if (matchesAffiliateCriteria && affiliateFilter.ActivityFilter)
                {
                    if (affiliateFilter.IsServiceExclusionFilter)
                        matchesAffiliateCriteria = !MatchAffiliateService(affiliateFilter, product); // Exclusion
                    else if (!affiliateFilter.IsServiceExclusionFilter)
                        matchesAffiliateCriteria = MatchAffiliateService(affiliateFilter, product); // Inclusion
                }

                if (matchesAffiliateCriteria && affiliateFilter.AffiliateActivityPriorityFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliatePriority(affiliateFilter, product);
                }

                if (matchesAffiliateCriteria && affiliateFilter.IsMarginFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliateMargin(affiliateFilter, product);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MatchAllAffiliateCriteria"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return matchesAffiliateCriteria;
        }

        /// <summary>
        /// This method matches the margin of the product and affiliate filter
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateMargin(AffiliateFilter affiliateFilter, Product product)
        {
            try
            {
                if (product?.ID == 0)
                    return false;

                if (!affiliateFilter.IsMarginFilter) return true;
                return product?.Margin != null && product.Margin.Value > affiliateFilter.AffiliateMargin;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MatchAffiliateMargin"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method is used to match the product with activities allowed for an affiliate
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateService(AffiliateFilter affiliateFilter, Product product)
        {
            try
            {
                if (product?.ID == 0 || affiliateFilter?.Activities == null)
                    return false;

                if (!affiliateFilter.ActivityFilter || (affiliateFilter.Activities?.Count < 0)) return false;
                return affiliateFilter.Activities.Any(activityId => product != null && product.ID == activityId && activityId != 0);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MatchAffiliateService"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method is used to match the region of the affiliate with the product
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateRegion(AffiliateFilter affiliateFilter, Product product)
        {
            try
            {
                if (product == null)
                    return false;
                if (!affiliateFilter.RegionFilter || affiliateFilter.Regions == null || affiliateFilter.Regions.Count <= 0)
                    return false;
                return affiliateFilter.Regions
                    .Select(regionId => product.Regions.FindAll(region => region.Id.Equals(regionId)))
                    .Any(filteredRegions => filteredRegions.Count > 0);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MatchAffiliateRegion"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method is used to match the activity type of the product to affiliate filter
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateDurationType(AffiliateFilter affiliateFilter, Product product)
        {
            try
            {
                if (product == null || !(product is ActivityLite activity) || !affiliateFilter.DurationTypeFilter || (affiliateFilter?.DurationTypes?.Count < 0))
                    return false;

                var type = activity.ActivityType;
                return affiliateFilter?.DurationTypes?.Any(durationType => type == durationType) ?? true;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MatchAffiliateDurationType"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<RegionCategoryMapping> LoadRegionCategoryMapping()
        {
            List<RegionCategoryMapping> regionCategoryMappings = null;
            try
            {
                try
                {
                    regionCategoryMappings = _activityCacheManager.GetRegioncategoryMapping();
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                                                         _log.Error(new IsangoErrorEntity
                                                         {
                                                             ClassName = "ActivityService",
                                                             MethodName = "LoadRegionCategoryMapping",
                                                             Params = "Not able to read data from cosmos."
                                                         }, ex)
                                                         );
                }
                if (regionCategoryMappings?.Any() == false)
                {
                    regionCategoryMappings = _activityPersistence.LoadRegionCategoryMapping();
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                                                     _log.Error(new IsangoErrorEntity
                                                     {
                                                         ClassName = "ActivityService",
                                                         MethodName = "LoadRegionCategoryMapping",
                                                     }, ex)
                                                     );
            }
            return regionCategoryMappings;
        }

        private List<RegionCategoryMapping> LoadRegionCategoryMappingV2(string languageCode)
        {
            try
            {
                var key = (languageCode?.ToLower() == "en" ? "RegionCategoryMapping" : "RegionCategoryMapping_" + languageCode?.ToLower());
                var regionCategoryMappings = _activityCacheManager.GetRegioncategoryMappingV2(key)?.CacheValue;
                if (regionCategoryMappings == null || regionCategoryMappings.Any() == false)
                {
                    regionCategoryMappings = _activityPersistence.LoadRegionCategoryMapping()?.Where(x => x.Languagecode == languageCode?.ToLower()).ToList();
                }
                return regionCategoryMappings;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadRegionCategoryMappingV2"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// get live price of activities based on api type.
        /// </summary>
        /// <param name="activityList"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<Activity> GetLivePriceByApiType(List<Activity> activityList, ClientInfo clientInfo, Criteria criteria)
        {
            var activities = new List<Activity>();
            var taskArray = new Task<Activity>[activityList.Count];
            var count = 0;

            foreach (var activity in activityList)
            {
                var supplierService = _supplierFactory.GetSupplierService(activity.ApiType);

                //Redeam V1.2 API separate Canonicalization
                if (activity.ApiType == APIType.RedeamV12)
                {
                    var supplierCriteria = _icanocalizationService.CreateCriteria(activity, criteria, clientInfo);
                    if (supplierCriteria != null)
                    {
                        taskArray[count] = Task.Factory.StartNew(() => _icanocalizationService.GetAvailability(activity, supplierCriteria, clientInfo.ApiToken));
                    }
                }
                else if (activity.ApiType == APIType.GlobalTixV3)
                {
                    var supplierCriteria = _icanocalizationService.CreateCriteria(activity, criteria, clientInfo);
                    if (supplierCriteria != null)
                    {
                        taskArray[count] = Task.Factory.StartNew(() => _icanocalizationService.GetAvailability(activity, supplierCriteria, clientInfo.ApiToken));
                    }
                }
                else
                {
                    var supplierCriteria = supplierService.CreateCriteria(activity, criteria, clientInfo);
                    taskArray[count] = Task.Factory.StartNew(() => supplierService.GetAvailability(activity, supplierCriteria, clientInfo.ApiToken));
                }
                //supplierService.GetAvailability(activity, supplierCriteria, clientInfo.ApiToken);
                count++;
            }
            try
            {
                if (taskArray?.Length > 0)
                {
                    Task.WaitAll(taskArray);
                    Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, task =>
                    {
                        var data = task.GetAwaiter().GetResult();
                        if (data != null)
                        {
                            activities.Add(data);
                            foreach (var option in data?.ProductOptions)
                            {
                                try
                                {
                                    var priceNode = option?.BasePrice != null ? option?.BasePrice : option?.CostPrice;
                                    if (priceNode == null)
                                    {
                                        continue;
                                    }
                                    if (priceNode?.DatePriceAndAvailabilty != null)
                                    {
                                        foreach (var key in priceNode?.DatePriceAndAvailabilty?.Keys)
                                        {
                                            if (priceNode.DatePriceAndAvailabilty.ContainsKey(key))
                                            {
                                                foreach (var pricingUnit in priceNode.DatePriceAndAvailabilty[key]?.PricingUnits)
                                                {
                                                    try
                                                    {
                                                        var paxType = ((PerPersonPricingUnit)pricingUnit).PassengerType;
                                                        if (criteria.NoOfPassengers.ContainsKey(paxType))
                                                        {
                                                            var paxCount = criteria.NoOfPassengers[paxType];
                                                            pricingUnit.Quantity = paxCount;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Task.Run(() =>
                                                         _log.Error(new IsangoErrorEntity
                                                         {
                                                             ClassName = "ActivityService",
                                                             MethodName = "GetLivePriceByApiType",
                                                             Token = clientInfo.ApiToken,
                                                             AffiliateId = clientInfo.AffiliateId,
                                                             Params = $"{option.Id},{option.ServiceOptionId}{SerializeDeSerializeHelper.Serialize(pricingUnit)}"
                                                         }, ex)
                                                         );
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Task.Run(() =>
                                       _log.Error(new IsangoErrorEntity
                                       {
                                           ClassName = "ActivityService",
                                           MethodName = "GetLivePriceByApiType",
                                           Token = clientInfo.ApiToken,
                                           AffiliateId = clientInfo.AffiliateId,
                                           Params = $"{option.Id},{option.ServiceOptionId}"
                                       }, ex)
                                    );
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetLivePriceByApiType"
                };
                _log.Error(isangoErrorEntity, ex);
                if (activityList == null)
                {
                    activityList = new List<Activity>();
                }
                if (activityList?.FirstOrDefault() == null)
                {
                    activityList.Add(new Activity
                    {
                        ID = criteria.ActivityId,
                        Id = criteria.ActivityId.ToString(),
                    });
                }
                if (activityList?.FirstOrDefault()?.Errors == null)
                {
                    activityList.FirstOrDefault().Errors = new List<Error>();
                }
                activityList.FirstOrDefault().Errors.Add(new Error
                {
                    Code = "ACTIVITY_SERVICE_ERROR",
                    HttpStatus = HttpStatusCode.InternalServerError,
                    Message = ex.Message + ex.StackTrace
                });
                return activityList;
            }
            return activities;
        }

        private Activity GetActivityDetailsWithLivePrice(int activityId, ClientInfo clientInfo, Criteria criteria, string B2BAffiliate = null)
        {
            try
            {
                var startDate = DateTime.Today.AddDays(10);
                var activity = LoadActivityAsync(activityId, startDate, clientInfo, B2BAffiliate).Result;
                if (activity == null) return null;

                //Get live price and availabilities
                var result = new List<Activity> { activity };
                var updatedActivityWithLivePrice = GetActivitiesWithLivePrice(clientInfo, criteria, result).FirstOrDefault();

                //updatedActivityWithLivePrice?.ProductOptions?.RemoveAll(x =>
                //    (x.BasePrice == null || x.BasePrice?.Amount == 0 && x.CostPrice == null ||
                //     x.CostPrice?.Amount == 0) || x.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE);

                //if (updatedActivityWithLivePrice != null && updatedActivityWithLivePrice.ProductOptions?.Count == 0)
                //    return null;
                return updatedActivityWithLivePrice;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityDetailsWithLivePrice"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private ActivityDetailsWithCalendarResponse MapActivityDetailsWithCalendar(Activity activity, List<CalendarAvailability> calendarAvailabilityList)
        {
            try
            {
                var activityDetailsWithCalendar = new ActivityDetailsWithCalendarResponse
                {
                    Activity = activity,
                    CalendarAvailabilityList = calendarAvailabilityList
                };
                return activityDetailsWithCalendar;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MapActivityDetailsWithCalendar"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<CalendarAvailability> GetCalendarAvailability(int productId, string affiliateId)
        {
            try
            {
                var result = _calendarAvailabilityCacheManager.GetCalendarAvailability(productId, affiliateId);
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetCalendarAvailability"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<ProductOption> LoadDefaultActivityWithPrice(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            try
            {
                var dateDifference = GetDateDifference(criteria.CheckoutDate, criteria.CheckinDate);
                var travelInfo = new TravelInfo
                {
                    StartDate = criteria.CheckinDate,
                    NumberOfNights = dateDifference,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages
                };

                activity.ProductOptions.ForEach(x =>
                {
                    x.TravelInfo = travelInfo;
                    x.ServiceOptionId = x.Id;
                });

                //activity.ProductOptions = GetAllOptionsAvailabilityAsync(activity, clientInfo)?.GetAwaiter().GetResult();
                var productOptionsAvailabilties = GetAllOptionsAvailabilitiesAsync(activity, clientInfo)?.GetAwaiter().GetResult();

                //activity.ProductOptions = activity.ProductOptions.Where(e => e.AvailabilityStatus == AvailabilityStatus.AVAILABLE || (e.AvailabilityStatus == AvailabilityStatus.ONREQUEST)).ToList();

                var paxDetail = new StringBuilder();
                paxDetail.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><PAXDETAIL>");

                foreach (var item in criteria.NoOfPassengers)
                {
                    paxDetail.Append($"<PAXTYPE PAXTYPEID=\"{Convert.ToInt32(item.Key)}\" COUNT=\"{item.Value}\"/>");
                }
                paxDetail.Append("</PAXDETAIL>");

                //call to procedure.
                var paxDetails = new PaxPriceRequest
                {
                    AffiliateId = clientInfo.AffiliateId,
                    CheckIn = criteria.CheckinDate,
                    CheckOut = criteria.CheckoutDate,
                    ServiceId = Convert.ToInt32(activity.Id),
                    PaxDetail = paxDetail.ToString()
                };
                var optionsWithPaxPrices = GetPaxPriceAsync(paxDetails)?.GetAwaiter().GetResult();

                if (!(optionsWithPaxPrices?.Any() == true && optionsWithPaxPrices?.Any(x => x?.PaxPrices?.Any() == true) == true))
                {
                    throw new Exception(Util.ErrorCodes.PASSENGER_PRICES_NOT_FOUND_IN_DB);
                }
                if (optionsWithPaxPrices.Count != 0)
                {
                    foreach (var option in activity.ProductOptions)
                    {
                        try
                        {
                            var paxPricesForOption = optionsWithPaxPrices.FirstOrDefault(x => x.ServiceOptionId == option.Id);

                            if (paxPricesForOption == null)
                            {
                                continue;
                            }
                            var currencyISOCode = paxPricesForOption.CurrencyIsoCode;
                            var currency = new Currency
                            {
                                IsoCode = currencyISOCode
                            };

                            option.GateBasePrice = new Price { Currency = currency };
                            option.CostPrice = new Price { Currency = currency };
                            option.BasePrice = new Price { Currency = currency };
                            Dictionary<DateTime, PriceAndAvailability> pAndaGate = null;
                            Dictionary<DateTime, PriceAndAvailability> pAndaCost = null;

                            if (paxPricesForOption != null)
                            {
                                var optionFromDb = productOptionsAvailabilties.FirstOrDefault(
                                                        x => x.PriceDate >= criteria.CheckinDate
                                                        && x.PriceDate <= criteria.CheckoutDate
                                                        && x.ServiceOptionId == option.Id
                                                    );
                                var optionAvailabilityStatus = optionFromDb != null ? optionFromDb.AvailableState : option.AvailabilityStatus;

                                option.AvailabilityStatus = optionAvailabilityStatus;

                                var baseDatePriceAndAvailability = option.BasePrice?.DatePriceAndAvailabilty != null && option.BasePrice?.DatePriceAndAvailabilty.Count > 0
                                    ? option.BasePrice?.DatePriceAndAvailabilty[criteria.CheckinDate]
                                    : new DefaultPriceAndAvailability
                                    {
                                        AvailabilityStatus = optionAvailabilityStatus,
                                        Capacity = optionFromDb != null ? optionFromDb.Capacity : 0,
                                        IsCapacityCheckRequired = optionFromDb != null ? optionFromDb.IsCapacityCheckRequired : false,
                                        PricingUnits = new List<PricingUnit>()
                                    };
                                var costDatePriceAndAvailability = option.CostPrice?.DatePriceAndAvailabilty != null &&
                                                                      option.CostPrice?.DatePriceAndAvailabilty.Count > 0
                                    ? option.CostPrice?.DatePriceAndAvailabilty[criteria.CheckinDate]
                                    : new DefaultPriceAndAvailability
                                    {
                                        AvailabilityStatus = optionAvailabilityStatus,
                                        Capacity = optionFromDb != null ? optionFromDb.Capacity : 0,
                                        IsCapacityCheckRequired = optionFromDb != null ? optionFromDb.IsCapacityCheckRequired : false,
                                        PricingUnits = new List<PricingUnit>()
                                    };

                                var paxUnitType = (UnitType)paxPricesForOption.UnitType;
                                var paxPriceType = (PriceType)paxPricesForOption.PriceType;

                                if (paxUnitType == UnitType.PerPerson)
                                {
                                    pAndaGate = GetPriceAndAvailabilities(paxPricesForOption, baseDatePriceAndAvailability, criteria, productOptionsAvailabilties, false);
                                    pAndaCost = GetPriceAndAvailabilities(paxPricesForOption, baseDatePriceAndAvailability, criteria, productOptionsAvailabilties, true);
                                }
                                else if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerPerson)
                                {
                                    #region Algo

                                    //                 Loop Per Option
                                    //     1.NonSharablePaxCount = Sum(Option.Item.Count) for the Passenger Type when ShareablePax = false

                                    //   2.NonSharableMaxCostPrice = Max(CostPrice) from all the Passenger Type where ShareablePax = false

                                    //                   NonSharableMaxBasePrice = Max(BasePrice) from all the Passenger Type where ShareablePax = false

                                    //   3.MaxCapacity = Option Max Capacity

                                    //   4.CalculatedUnit = MATH.CEILING(NonSharablePaxCount / MaxCapacity)

                                    //   5.NonSharableTotalCostPrice = NonSharableMaxCostPrice * (CalculatedUnit * MaxCapacity)

                                    //   6.sharableCostPrice = Sum(PassengerType.item.Costprice) for the Passenger Type when ShareablePax = true

                                    //    sharableBaseprice = Sum(PassengerType.item.BasePrice) for the Passenger Type when ShareablePax = true

                                    //7.costPricePerPax =
                                    //                     if (PassengerType.ShareablePax = false)
                                    //                                 NonSharableTotalCostPrice / NonSharablePaxCount
                                    //                     else
                                    //                                 PassengerType.item.Costprice
                                    //    Same rule for BasePrice

                                    // 8.TotalCostPrice = NonSharableTotalCostPrice + sharableCostPrice

                                    //                 TotalBasePrice = NonSharableTotalBasePrice + sharableBasePrice

                                    #endregion Algo

                                    pAndaGate = GetPriceAndAvailabilitiesV1(paxPricesForOption, baseDatePriceAndAvailability, criteria, productOptionsAvailabilties, false);
                                    pAndaCost = GetPriceAndAvailabilitiesV1(paxPricesForOption, baseDatePriceAndAvailability, criteria, productOptionsAvailabilties, true);
                                }
                                else if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerUnit)
                                {
                                    pAndaGate = GetPriceAndAvailabilitiesV2(paxPricesForOption, baseDatePriceAndAvailability, criteria, productOptionsAvailabilties, false);
                                    pAndaCost = GetPriceAndAvailabilitiesV2(paxPricesForOption, baseDatePriceAndAvailability, criteria, productOptionsAvailabilties, true);
                                }

                                option.GateBasePrice = option.BasePrice;
                            }

                            option.GateBasePrice.DatePriceAndAvailabilty = pAndaGate;
                            option.GateBasePrice.Amount = pAndaGate?.FirstOrDefault(x =>
                                                                x.Key >= criteria.CheckinDate
                                                                && x.Key <= criteria.CheckoutDate
                                                          ).Value?.TotalPrice ?? 0;

                            option.CostPrice.DatePriceAndAvailabilty = pAndaCost;
                            option.CostPrice.Amount = pAndaCost?.FirstOrDefault(x =>
                                                                x.Key >= criteria.CheckinDate
                                                                && x.Key <= criteria.CheckoutDate
                                                          ).Value?.TotalPrice ?? 0;

                            option.BasePrice = option?.GateBasePrice.DeepCopy();
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                             _log.Error(new IsangoErrorEntity
                             {
                                 ClassName = "ActivityService",
                                 MethodName = "LoadDefaultActivityWithPrice",
                                 Token = clientInfo.ApiToken,
                                 AffiliateId = clientInfo.AffiliateId,
                                 Params = $"{option.Id},{option.ServiceOptionId}"
                             }, ex)
                             );
                            throw;
                        }
                    }

                    var productOptionsWithStartTime = CreateProductOptionBasedOnStartTime(activity, criteria);
                    activity.ProductOptions = FilterProductOptionsBasedOnCloseOut(activity.CoOrdinates, productOptionsWithStartTime, criteria);
                }
                else
                {
                    activity.ProductOptions = null;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadDefaultActivityWithPrice"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity.ProductOptions;
        }

        private PricingUnit CreatePricingUnit(decimal price, UnitType type, PassengerType passengerType, int quantity)
        {
            try
            {
                var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
                pricingUnit.UnitType = type;
                pricingUnit.Price = price;
                pricingUnit.Quantity = quantity;
                return pricingUnit;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CreatePricingUnit"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Activity GetActivityFromPersistence(int activityId, string languageCode)
        {
            try
            {
                var activity = _activityPersistence.GetActivitiesByActivityIds(activityId.ToString(), languageCode).FirstOrDefault() ??
                               _activityPersistence.LoadLiveHbActivities(activityId, languageCode).FirstOrDefault();
                if (activity == null)
                {
                    return null;
                }

                activity.PassengerInfo = _activityPersistence.GetPassengerInfoDetails(activityId.ToString());

                var isangoAvailabilities = _activityPersistence.GetOptionAvailability();

                activity = UpdateActivityPrice(activity, isangoAvailabilities);
                return activity;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityFromPersistence",
                    Params = $"{activityId}, {languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Check whether passed API Type is valid for getting Availability from DB
        /// </summary>
        /// <param name="apiType"></param>
        /// <returns></returns>
        private bool ValidAPIToGetAvailabilityFromDB(int apiType)
        {
            try
            {
                //var apiAvailabilityFromDB = ConfigurationManagerHelper.GetValuefromAppSettings("APIIdsAvailabilityFromDB").Split(',');
                if (_apiAvailabilityFromDB?.Length > 0)
                {
                    foreach (var item in _apiAvailabilityFromDB)
                    {
                        if (item == apiType.ToString())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "ValidAPIToGetAvailabilityFromDB"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return false;
        }

        /// <summary>
        /// Create dynamic option based on Start time
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<ProductOption> CreateProductOptionBasedOnStartTime(Activity activity, Criteria criteria)
        {
            var productOptionsWithStartTime = new List<ProductOption>();
            try
            {
                foreach (var productOption in activity.ProductOptions)
                {
                    try
                    {
                        if (productOption.TimesOfDays == null)
                        {
                            productOptionsWithStartTime.Add(productOption);
                            continue;
                        }

                        foreach (var timeOfDay in productOption.TimesOfDays)
                        {
                            if (criteria.CheckinDate < timeOfDay.AppliedFromDate || criteria.CheckoutDate > timeOfDay.AppliedToDate)
                            {
                                continue;
                            }

                            var productOptionWithStartTime = (ProductOption)productOption.Clone();
                            productOptionWithStartTime.ServiceOptionId = productOptionWithStartTime.Id;
                            productOptionWithStartTime.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                            productOptionWithStartTime.StartTime = timeOfDay.StartTime;
                            productOptionWithStartTime.BasePrice = productOptionWithStartTime.BasePrice?.DeepCopy();
                            productOptionWithStartTime.GateBasePrice = productOptionWithStartTime.GateBasePrice?.DeepCopy();
                            productOptionWithStartTime.CostPrice = productOptionWithStartTime.CostPrice?.DeepCopy();
                            productOptionsWithStartTime.Add(productOptionWithStartTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() =>
                                   _log.Error(new Isango.Entities.IsangoErrorEntity
                                   {
                                       ClassName = "ActivityService",
                                       MethodName = "CreateProductOptionBasedOnStartTime",
                                       Params = SerializeDeSerializeHelper.Serialize(productOption),
                                       Token = criteria.Token,
                                   }, ex)
                               );
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CreateProductOptionBasedOnStartTime"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return productOptionsWithStartTime;
        }

        private Activity FilterAvailabilityfromDB(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            try
            {
                var dateDifference = GetDateDifference(criteria.CheckoutDate, criteria.CheckinDate);
                var travelInfo = new TravelInfo
                {
                    StartDate = criteria.CheckinDate,
                    NumberOfNights = dateDifference,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages
                };

                activity.ProductOptions.ForEach(x =>
                {
                    x.TravelInfo = travelInfo;
                });

                //activity.ProductOptions = GetAllOptionsAvailabilityAsync(activity, clientInfo)?.GetAwaiter().GetResult();
                var productOptionsAvailabilties = GetAllOptionsAvailabilitiesAsync(activity, clientInfo)?.GetAwaiter().GetResult();

                foreach (var option in activity.ProductOptions)
                {
                    if (option?.BasePrice?.DatePriceAndAvailabilty?.Count > 0)
                    {
                        foreach (var availability in option?.BasePrice?.DatePriceAndAvailabilty)
                        {
                            option.BasePrice.DatePriceAndAvailabilty[availability.Key].AvailabilityStatus = productOptionsAvailabilties?.Where(x => x.ServiceOptionId == option.ServiceOptionId
                                                                                                            && x.PriceDate == availability.Key).FirstOrDefault().AvailableState ??
                                                                                                            AvailabilityStatus.NOTAVAILABLE;
                        }
                    }
                    if (option?.CostPrice?.DatePriceAndAvailabilty?.Count > 0)
                    {
                        foreach (var availability in option?.CostPrice?.DatePriceAndAvailabilty)
                        {
                            option.CostPrice.DatePriceAndAvailabilty[availability.Key].AvailabilityStatus = productOptionsAvailabilties?.Where(x => x.ServiceOptionId == option.ServiceOptionId
                                                                                                            && x.PriceDate == availability.Key).FirstOrDefault().AvailableState ??
                                                                                                            AvailabilityStatus.NOTAVAILABLE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "FilterAvailabilityfromDB"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// Make adapter calls and map the activity cache and supplier
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="redeamCriteria"></param>
        /// <returns></returns>
        private Activity GetRedeamActivity(Activity activity, ClientInfo clientInfo, RedeamCriteria redeamCriteria)
        {
            try
            {
                var redeamProductOptions = _redeamAdapter.GetAvailabilities(redeamCriteria, clientInfo.ApiToken).GetAwaiter().GetResult();
                if (redeamProductOptions?.Count > 0)
                {
                    var readeamActivity = LoadMappedDataRedeam(activity, redeamProductOptions, redeamCriteria);
                    return readeamActivity;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetRedeamActivity"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// Map data for Redeam
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="optionsFromAPI"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private Activity LoadMappedDataRedeam(Activity activity, List<ProductOption> optionsFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();
            try
            {
                foreach (var activityOption in optionsFromAPI)
                {
                    var activityOptionFromAPI = (ActivityOption)activityOption;
                    var activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                        x => (x).SupplierOptionCode == activityOptionFromAPI.SupplierOptionCode);
                    if (activityOptionFromCache == null) continue;

                    var option = GetMappedActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
                    option.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                    option.PickupLocations = activityOptionFromAPI.PickupLocations;
                    // Update name of Option with time from API
                    if (!string.IsNullOrWhiteSpace(activityOptionFromAPI.Name)
                        && (activityOptionFromAPI.Name != Constant.ZeroTime)
                        )
                        option.Name = $"{activityOptionFromCache.Name.TrimEnd()} - {activityOptionFromAPI.Name}";
                    productOptions.Add(option);
                }
                activity.ProductOptions = productOptions;
                if (activity.ProductOptions.Count == 0)
                {
                    activity.ProductOptions = optionsFromAPI;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadMappedDataRedeam"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// If criteria has adult and child , One option has price for adult & child and other option has only adult then option with adult & child should be available. This method  sets status to not NOTAVAILABLE when criteria is not fulfilled.
        /// </summary>
        /// <param name="productOptions"></param>
        /// <param name="noOfPassengers"></param>
        private void ValidateOptionPaxCriteria(List<ProductOption> productOptions, Dictionary<PassengerType, int> noOfPassengers, DateTime checkinDate, DateTime checkoutDate, string token, Activity activity)
        {
            if (productOptions?.Count > 0)
            {
                try
                {
                    var criteriaPaxes = string.Empty;
                    var puPaxes = string.Empty;

                    var cPaxes = noOfPassengers?.Keys?.ToArray()?
                        .ToList()?
                        .Where(y => y != PassengerType.Infant)?
                        .OrderBy(x => x);

                    if (cPaxes?.Any() != true)
                    {
                        return;
                    }

                    criteriaPaxes = string.Join(",", cPaxes);
                    //criteriaPaxes.ToLower().Replace("infant", string.Empty);
                    foreach (var po in productOptions)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(activity?.CancellationPolicy) && string.IsNullOrWhiteSpace(po.CancellationText))
                            {
                                po.CancellationText = activity?.CancellationPolicy;
                            }
                            if (po?.BasePrice?.DatePriceAndAvailabilty?.Any() == true)
                            {
                                po.BasePrice.DatePriceAndAvailabilty = FilterPriceAndAvailabilitiesAsPerInputDates(po?.BasePrice?.DatePriceAndAvailabilty, checkinDate, checkoutDate, token);
                            }
                            if (po?.GateBasePrice?.DatePriceAndAvailabilty?.Any() == true)
                            {
                                po.GateBasePrice.DatePriceAndAvailabilty = FilterPriceAndAvailabilitiesAsPerInputDates(po?.GateBasePrice?.DatePriceAndAvailabilty, checkinDate, checkoutDate, token);
                            }
                            if (po?.CostPrice?.DatePriceAndAvailabilty?.Any() == true)
                            {
                                po.CostPrice.DatePriceAndAvailabilty = FilterPriceAndAvailabilitiesAsPerInputDates(po?.CostPrice?.DatePriceAndAvailabilty, checkinDate, checkoutDate, token);
                            }
                            if (po?.GateSellPrice?.DatePriceAndAvailabilty?.Any() == true)
                            {
                                po.GateSellPrice.DatePriceAndAvailabilty = FilterPriceAndAvailabilitiesAsPerInputDates(po?.GateSellPrice?.DatePriceAndAvailabilty, checkinDate, checkoutDate, token);
                            }
                            if (po?.SellPrice?.DatePriceAndAvailabilty?.Any() == true)
                            {
                                po.SellPrice.DatePriceAndAvailabilty = FilterPriceAndAvailabilitiesAsPerInputDates(po?.SellPrice?.DatePriceAndAvailabilty, checkinDate, checkoutDate, token);
                            }

                            var datePriceAndAvailabilty = po?.BasePrice?.DatePriceAndAvailabilty ??
                                                                     po?.GateBasePrice?.DatePriceAndAvailabilty ??
                                                                     po?.CostPrice?.DatePriceAndAvailabilty ??
                                                                     po?.GateSellPrice?.DatePriceAndAvailabilty ??
                                                                     po?.SellPrice?.DatePriceAndAvailabilty;

                            if (datePriceAndAvailabilty?.Any() == false || datePriceAndAvailabilty == null)
                            {
                                continue;
                            }

                            var puQuery = from pa in datePriceAndAvailabilty
                                          from pu in pa.Value?.PricingUnits
                                          from nop in noOfPassengers
                                          where ((PerPersonPricingUnit)pu)?.PassengerType == nop.Key
                                          && ((PerPersonPricingUnit)pu)?.PassengerType != PassengerType.Infant
                                          && ((PerPersonPricingUnit)pu) != null
                                          select ((PerPersonPricingUnit)pu)?.PassengerType;

                            if (puQuery?.Any() == false || puQuery == null)
                            {
                                continue;
                            }

                            var pricingUnits = puQuery?.OrderBy(x => x)?.Distinct().ToList();

                            if (pricingUnits?.Count > 0)
                            {
                                puPaxes = string.Join(",", pricingUnits)?.ToLower();
                                criteriaPaxes = criteriaPaxes?.ToLower();
                                puPaxes = puPaxes?.ToLower();

                                if (criteriaPaxes != puPaxes
                                    && !string.IsNullOrWhiteSpace(puPaxes)
                                )
                                {
                                    po.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;

                                    var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                                    {
                                        ClassName = "ActivityService",
                                        MethodName = "ValidateOptionPaxCriteria",
                                        Token = token,
                                    };
                                    var errCode = "InvalidOptionPassengerCriteria";
                                    var errMsg = $"Searching availability for  : {criteriaPaxes} but it available only for : {puPaxes}. Please check for {puPaxes} only to get response.";

                                    if (activity.Errors == null)
                                    {
                                        activity.Errors = new List<Error>();
                                    }
                                    if (activity?.Errors?.Any(x => x.Code == errCode) == false)
                                    {
                                        var error = new Error
                                        {
                                            Code = errCode,
                                            Message = errMsg
                                        };
                                        activity.Errors.Add(error);
                                    }

                                    var ex = new Exception(errMsg);
                                    _log.Error(isangoErrorEntity, ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //_log.Error($"ActivityService|ValidateOptionPaxCriteria|criteriaPaxes :- {criteriaPaxes}, PriceUnitPaxes =:- {puPaxes}", ex);
                        }
                    }

                    productOptions.RemoveAll(x => x.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE);
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                                _log.Error(new Isango.Entities.IsangoErrorEntity
                                {
                                    ClassName = "ActivityService",
                                    MethodName = "ValidateOptionPaxCriteria",
                                    Token = token,
                                }, ex)
                            );
                    throw;
                }
            }
        }

        private void ValidateOptionForCapacity(List<ProductOption> productOptions, Dictionary<PassengerType, int> noOfPassengers)
        {
            try
            {
                if (productOptions?.Any() == true)
                {
                    foreach (var prodOption in productOptions)
                    {
                        var datePriceAndAvailabilty = prodOption?.BasePrice?.DatePriceAndAvailabilty ??
                                                                     prodOption?.GateBasePrice?.DatePriceAndAvailabilty ??
                                                                     prodOption?.CostPrice?.DatePriceAndAvailabilty ??
                                                                     prodOption?.GateSellPrice?.DatePriceAndAvailabilty ??
                                                                     prodOption?.SellPrice?.DatePriceAndAvailabilty;

                        if (datePriceAndAvailabilty?.Any() == false || datePriceAndAvailabilty == null)
                        {
                            continue;
                        }
                        var invalidDatesToRemove = new List<DateTime>();
                        foreach (var dpAvailability in datePriceAndAvailabilty)
                        {
                            if (dpAvailability.Value?.IsCapacityCheckRequired.Equals(true) == true)
                            {
                                if(prodOption.ApiType == APIType.Fareharbor)
                                {
                                    var chargablePaxTypeTotalCount = noOfPassengers?.Where(x => x.Key != PassengerType.Infant)?.Sum(y => y.Value) ?? 0;
                                    if (chargablePaxTypeTotalCount > dpAvailability.Value?.Capacity)
                                    {
                                        invalidDatesToRemove.Add(dpAvailability.Key);
                                    }
                                }
                                else if(noOfPassengers?.Sum(x => x.Value) > dpAvailability.Value?.Capacity)
                                {
                                    invalidDatesToRemove.Add(dpAvailability.Key);
                                }
                            }
                        }
                        if (invalidDatesToRemove?.Any() == true)
                        {
                            foreach (var invalidDate in invalidDatesToRemove)
                            {
                                datePriceAndAvailabilty.Remove(invalidDate);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                           _log.Error(new Isango.Entities.IsangoErrorEntity
                           {
                               ClassName = "ActivityService",
                               MethodName = "ValidateOptionForCapacity",
                           }, ex)
                       );
                throw;
            }
        }

        /// <summary>
		/// Filter Product options using Close out .
		/// </summary>
		/// <param name="coOrdinates"></param>
		/// <param name="productOptions"></param>
		/// <param name="criteria"></param>
		/// <returns></returns>
		private List<ProductOption> FilterProductOptionsBasedOnCloseOut(string coOrdinates, List<ProductOption> productOptions, Criteria criteria)
        {
            if (string.IsNullOrEmpty(coOrdinates) || coOrdinates == "0") return productOptions;

            try
            {
                var checkInDate = criteria.CheckinDate;
                var destinationCurrentDateTime = GetDestinationCurrentDateTime(coOrdinates);

                if (checkInDate.Date > destinationCurrentDateTime.Date)
                    return productOptions;

                var invalidProductIds = new List<int>();
                foreach (var productOption in productOptions)
                {
                    try
                    {
                        if (productOption.StartTime == default(TimeSpan))
                            continue;

                        var validCloseOut = productOption.CloseOuts?.FirstOrDefault(x => x.AppliedFromDate.Date <= checkInDate.Date && x.AppliedToDate.Date >= checkInDate.Date);
                        if (validCloseOut == null)
                            continue;

                        var destinationCloseOutTime = destinationCurrentDateTime.AddMinutes(validCloseOut.CloseOutMin).TimeOfDay;
                        if (productOption.StartTime < destinationCloseOutTime)
                            invalidProductIds.Add(productOption.Id);
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() =>
                        _log.Error(new Isango.Entities.IsangoErrorEntity
                        {
                            ClassName = "ActivityService",
                            MethodName = "FilterProductOptionsBasedOnCloseOut",
                            Params = $"coOrdinates : {coOrdinates}, ServiceOptionId : {productOptions.FirstOrDefault().ServiceOptionId}",
                        }, ex)
                        );
                    }
                }
                productOptions.RemoveAll(e => invalidProductIds.Contains(e.Id));
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                _log.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "FilterProductOptionsBasedOnCloseOut",
                    Params = $"coOrdinates : {coOrdinates}, ServiceOptionId : {productOptions.FirstOrDefault().ServiceOptionId}",
                }, ex)
                );
                throw;
            }
            return productOptions;
        }

        /// <summary>
        /// Get Current DateTime using Geo CoOrdinates.
        /// </summary>
        /// <param name="coOrdinates"></param>
        /// <returns></returns>
        private DateTime GetDestinationCurrentDateTime(string coOrdinates)
        {
            try
            {
                var timeZoneIANA = TimeZoneLookup.GetTimeZone(Convert.ToDouble(coOrdinates.Split(',')[0]), Convert.ToDouble(coOrdinates.Split(',')[1])).Result;
                var windowsTimeZoneId = TZConvert.IanaToWindows(timeZoneIANA);
                var destinationTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
                var destinationCurrentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, destinationTimeZoneInfo);
                return destinationCurrentDateTime;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDestinationCurrentDateTime"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// It try to call api for each option in parallel for hotelbeds Apitude Api
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria">List of activity, there will be one activity for each option as iSango option is mapped with an activity at api end</param>
        /// <returns></returns>
        private Task<List<Activity>> GetHBActivitiesParallalAsync(Activity activity, ClientInfo clientInfo, TicketCriteria criteria)
        {
            try
            {
                var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
                var hbActivities = new List<Activity>();

                var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 1.0));

                Parallel.ForEach(productOptions, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, po =>
                {
                    try
                    {
                        var hotelbedCriteriaApitude = GetHotelbedCriteriaApitude(activity, clientInfo, criteria, po);

                        var hbActivity = _hBAdapter.ActivityDetailsAsync(hotelbedCriteriaApitude,
                            clientInfo.ApiToken).GetAwaiter().GetResult();

                        if (hbActivity?.ProductOptions?.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true)
                        {
                            hbActivities.Add(hbActivity);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"ActivityService|GetHBActivitiesParallalAsync|IsangoActivityId :- {activity?.Id}" +
                            $", Criteria :- {SerializeDeSerializeHelper.Serialize(criteria)},", ex);
                    }
                });
                return Task.FromResult(hbActivities);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetHBActivitiesParallalAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// It sequentially call api for each option for hotelbeds Apitude Api
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria">List of activity, there will be one activity for each option as iSango option is mapped with an activity at api end</param>
        /// <returns></returns>
        private Task<List<Activity>> GetHBActivitiesSequentiallyAsync(Activity activity, ClientInfo clientInfo, TicketCriteria criteria)
        {
            try
            {
                var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
                var hbActivities = new List<Activity>();

                foreach (var po in productOptions)
                {
                    try
                    {
                        var hotelbedCriteriaApitude = GetHotelbedCriteriaApitude(activity, clientInfo, criteria, po);

                        var hbActivity = _hBAdapter.ActivityDetailsAsync(hotelbedCriteriaApitude,
                            clientInfo.ApiToken).GetAwaiter().GetResult();

                        if (hbActivity?.ProductOptions?.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true)
                        {
                            hbActivities.Add(hbActivity);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"ActivityService|GetHBActivitiesSequentiallyAsync|IsangoActivityId :- {activity?.Id}" +
                            $", Criteria :- {SerializeDeSerializeHelper.Serialize(criteria)},", ex);
                    }
                }
                return Task.FromResult(hbActivities);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetHBActivitiesSequentiallyAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private HotelbedCriteriaApitude GetHotelbedCriteriaApitude(Activity activity, ClientInfo clientInfo, TicketCriteria criteria, ProductOption productOption)
        {
            try
            {
                var destinationCode = productOption?.SupplierOptionCode?.Split('~')?.LastOrDefault();

                var hotelbedCriteriaApitude = new HotelbedCriteriaApitude
                {
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages,
                    FactSheetIds = new List<int> { activity.FactsheetId },
                    CheckinDate = criteria.CheckinDate,
                    CheckoutDate = criteria.CheckoutDate.AddDays(Constant.AddSixDays),

                    Language = clientInfo?.LanguageCode,
                    //PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                    PassengerInfo = criteria.PassengerInfo,

                    ActivityCode = productOption?.PrefixServiceCode,
                    IsangoActivityId = activity?.Id,
                    ServiceOptionId = productOption?.Id.ToString(),
                    Destination = destinationCode
                };
                return hotelbedCriteriaApitude;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetHotelbedCriteriaApitude"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private int GetDateDifference(DateTime toDate, DateTime fromdate, bool isRound = true)
        {
            int result = 0;
            try
            {
                var diff = (toDate - fromdate).TotalDays;
                if (isRound)
                {
                    int.TryParse(Math.Ceiling(diff).ToString(CultureInfo.InvariantCulture), out result);
                }
                else
                {
                    int.TryParse(diff.ToString(CultureInfo.InvariantCulture), out result);
                }
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDateDifference"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return result;
        }

        /// <summary>
        /// This method is used to match the Affiliate Services Priority for the given product
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliatePriority(AffiliateFilter affiliateFilter, Product product)
        {
            try
            {
                if (product == null || product.ID == 0)
                    return false;

                if (affiliateFilter.AffiliateActivityPriorityFilter && affiliateFilter.AffiliateServicesPriority != null
                                                                    && affiliateFilter.AffiliateServicesPriority.Count > 0)
                {
                    var servicePriority = affiliateFilter.AffiliateServicesPriority.Find(x => x.Key.Equals(product.ID));

                    if (servicePriority.Value >= 0 && product is ActivityLite activity)
                    {
                        activity.Priority = servicePriority.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "MatchAffiliatePriority"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return true;
        }

        private Activity UpdateActivityPrice(Activity activity, DataTable availability)
        {
            try
            {
                for (var i = 0; i < activity?.ProductOptions?.Count; i++)
                {
                    try
                    {
                        var result = availability.Select(activity.ActivityType == ActivityType.Bundle
                            ? $"{Constant.ServiceId} = {activity.ProductOptions[i].ComponentServiceID} and {Constant.ServiceOptionId} = {activity.ProductOptions[i].Id}"
                            : $"{Constant.ServiceId} = {activity.Id} and {Constant.ServiceOptionId} = {activity.ProductOptions[i].Id}");

                        if (result?.Length > 0)
                        {
                            var baseValue = Convert.ToDecimal(result.Min(x => x[Constant.GatePrice]));

                            // if multiple rows exists having same gate price then find min of cost price.
                            var costValue = Convert.ToDecimal(result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Select(x => x[Constant.CostPrice]).Count() > 1 ? result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Min(x => x[Constant.CostPrice]) : result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Select(x => x[Constant.CostPrice]).First());

                            //As currency for the ServiceID is same for any duration, so taking currency of the first row if multiple rows exists.
                            var currency = new Currency()
                            {
                                IsoCode = result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Select(x => x[Constant.CurrencyISOCode]).First().ToString()
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

                            activity.ProductOptions[i].CostPrice = costPrice;
                            activity.ProductOptions[i].BasePrice = basePrice;
                            activity.ProductOptions[i].GateBasePrice = gateBasePrice;
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ActivityService",
                            MethodName = "UpdateActivityPrice"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        //ignored
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "UpdateActivityPrice"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return activity;
        }

        #endregion Private Methods

        /// <summary>
        /// Updates HotelBeds Apitude product options' "Sell Prices" to the current currency. Note: margin is ignored at the moment!
        /// </summary>
        /// <param name="productOptions"></param>
        /// <param name="hotelPickupLocation"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<ProductOption> UpdateBasePricesHbApitude(List<ProductOption> productOptions, string hotelPickupLocation,
            ClientInfo clientInfo, Criteria criteria)
        {
            try
            {
                if (productOptions == null || (productOptions.Count <= 0)) return productOptions;

                var options = new List<ProductOption>();
                foreach (var option in productOptions)
                {
                    var actOption = (ActivityOption)option;
                    if (actOption == null) continue;

                    actOption.BasePrice = CalculatePriceForAllPaxHBApitude(actOption.SellPrice, criteria);
                    actOption.GateBasePrice = CalculatePriceForAllPaxHBApitude(actOption.SellPrice, criteria);
                    actOption.CostPrice = CalculatePriceForAllPaxHBApitude(actOption.CostPrice, criteria);

                    if (actOption?.IsMandatoryApplyAmount == true)
                    {
                        actOption.Margin = null;
                    }
                    actOption.HotelPickUpLocation = hotelPickupLocation;
                    options.Add(actOption);
                }
                return options;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "UpdateBasePricesHbApitude"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Price CalculatePriceForAllPaxHBApitude(Price inputPrice, Criteria criteria)
        {
            try
            {
                if (inputPrice == null) return null;
                var price = inputPrice.DeepCopy();

                var isPerUnit = false;
                var perUnitPrice = new decimal();
                var perPersonPrice = new decimal();

                foreach (var priceAndAvailability in price.DatePriceAndAvailabilty)
                {
                    if (priceAndAvailability.Value?.PricingUnits == null) continue;
                    var pricingUnits = priceAndAvailability.Value.PricingUnits;
                    foreach (var pricingUnit in pricingUnits)
                    {
                        if (pricingUnit is PerUnitPricingUnit)
                        {
                            perUnitPrice = pricingUnit.Price;
                            isPerUnit = true;
                        }
                        //else if (pricingUnit is PricingUnit)
                        //{
                        //    perUnitPrice = pricingUnit.Price;
                        //    isPerUnit = true;
                        //}
                        else
                        {
                            var passengerType = ((PerPersonPricingUnit)pricingUnit).PassengerType;
                            var paxCount = GetPaxCountByPaxType(criteria, passengerType);
                            perPersonPrice = pricingUnit.Price * paxCount;
                        }
                    }

                    //priceAndAvailability.Value.TotalPrice = isPerUnit ? perUnitPrice : perPersonPrice;
                }
                //price.Amount = price.DatePriceAndAvailabilty.Where(x => x.Key.Date == criteria.CheckinDate.Date).
                //    Select(x => x.Value.TotalPrice).
                //    FirstOrDefault();
                return price;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CalculatePriceForAllPaxHBApitude"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private int GetPaxCountByPaxType(Criteria criteria, PassengerType passengerType) => criteria.NoOfPassengers.Where(x => x.Key == passengerType).Select(x => x.Value).FirstOrDefault();

        #region MultiDay Support for isango products

        /// <summary>
        /// Get PriceAndAvailabilities From DB OptionDetails as per dateRange
        /// 1 UnitType: PerPerson, PriceType: PerPerson
        /// </summary>
        /// <param name="optionDetail"></param>
        /// <param name="defaultPriceAndAvailability"></param>
        /// <param name="criteria"></param>
        /// <param name="isCost">Default id gatePrice</param>
        /// <returns></returns>
        private Dictionary<DateTime, PriceAndAvailability> GetPriceAndAvailabilities(OptionDetail optionDetail, PriceAndAvailability defaultPriceAndAvailability, Criteria criteria, List<ProductOptionAvailabilty> productOptionsAvailabilties, bool isCost = false)
        {
            try
            {
                var result = new Dictionary<DateTime, PriceAndAvailability>();
                var travelDates = optionDetail.PaxPrices.Select(x => x.TravelDate).Distinct().ToList();
                var paxPrices = optionDetail.PaxPrices;
                var paxUnitType = (UnitType)optionDetail.UnitType;
                var paxPriceType = (PriceType)optionDetail.PriceType;

                foreach (var date in travelDates)
                {
                    //if option is available on selected date then pick the price from pax detail
                    var availableOptionDateQuery = from od in productOptionsAvailabilties
                                                   from pp in paxPrices
                                                   where pp.ServiceOptionId == od.ServiceOptionId
                                                   && pp.TravelDate == od.PriceDate
                                                   && pp.TravelDate == date
                                                   select pp;

                    var optionStatus = from od in productOptionsAvailabilties
                                       from pp in paxPrices
                                       where pp.ServiceOptionId == od.ServiceOptionId
                                       && pp.TravelDate == od.PriceDate
                                       && pp.TravelDate == date
                                       select od;

                    var dateWiseDetails = availableOptionDateQuery.ToList();

                    if (dateWiseDetails?.Count > 0)
                    {
                        var priceAndAvailability = defaultPriceAndAvailability.Clone() as PriceAndAvailability;
                        priceAndAvailability.AvailabilityStatus = optionStatus?.FirstOrDefault()?.AvailableState ?? AvailabilityStatus.NOTAVAILABLE;
                        foreach (var paxDetail in dateWiseDetails)
                        {
                            var paxType = (PassengerType)paxDetail.PassengerTypeId;
                            var price = isCost ? paxDetail.CostPrice : paxDetail.GateBasePrice;
                            var paxCount = criteria.NoOfPassengers.ContainsKey(paxType) ? criteria.NoOfPassengers[paxType] : 0;

                            var pricingUnit = CreatePricingUnit(price, paxUnitType, paxType, paxCount);

                            if (!priceAndAvailability.PricingUnits.Any(x => ((PerPersonPricingUnit)x)?.PassengerType == paxType))
                            {
                                priceAndAvailability.TotalPrice += price * paxCount;
                                priceAndAvailability.PricingUnits.Add(pricingUnit);
                            }
                        }
                        if (!result.Keys.Contains(date))
                        {
                            result.Add(date, priceAndAvailability);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetPriceAndAvailabilities"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get PriceAndAvailabilities From DB OptionDetails as per dateRange
        /// 2 UnitType- PerUnit, PriceType- PerPerson
        /// </summary>
        /// <param name="optionDetail"></param>
        /// <param name="defaultPriceAndAvailability"></param>
        /// <param name="criteria"></param>
        /// <param name="isCost">Default id gatePrice</param>
        /// <returns></returns>
        private Dictionary<DateTime, PriceAndAvailability> GetPriceAndAvailabilitiesV1(OptionDetail optionDetail, PriceAndAvailability defaultPriceAndAvailability, Criteria criteria, List<ProductOptionAvailabilty> productOptionsAvailabilties, bool isCost = false)
        {
            try
            {
                var result = new Dictionary<DateTime, PriceAndAvailability>();
                var travelDates = optionDetail.PaxPrices.Select(x => x.TravelDate).Distinct().ToList();
                var paxPrices = optionDetail.PaxPrices;
                var paxUnitType = (UnitType)optionDetail.UnitType;
                var paxPriceType = (PriceType)optionDetail.PriceType;
                var totalPaxCount = criteria?.NoOfPassengers?.Where(x => x.Key != PassengerType.Infant)?.Sum(y => y.Value) ?? 0;
                if (totalPaxCount == 0)
                {
                    var message = "Pax count not found in request criteria";
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    throw new HttpResponseException(data);
                }

                foreach (var date in travelDates)
                {
                    //if option is available on selected date then pick the price from pax detail
                    var availableOptionDateQuery = from od in productOptionsAvailabilties
                                                   from pp in paxPrices
                                                   where pp.ServiceOptionId == od.ServiceOptionId
                                                   && pp.TravelDate == od.PriceDate
                                                   && pp.TravelDate == date
                                                   select pp;

                    var optionStatus = from od in productOptionsAvailabilties
                                       from pp in paxPrices
                                       where pp.ServiceOptionId == od.ServiceOptionId
                                       && pp.TravelDate == od.PriceDate
                                       && pp.TravelDate == date
                                       select od;

                    var dateWiseDetails = availableOptionDateQuery.ToList();

                    if (dateWiseDetails?.Count > 0)
                    {
                        var priceAndAvailability = defaultPriceAndAvailability.Clone() as PriceAndAvailability;
                        priceAndAvailability.AvailabilityStatus = optionStatus?.FirstOrDefault()?.AvailableState ?? AvailabilityStatus.NOTAVAILABLE;
                        foreach (var paxDetail in dateWiseDetails)
                        {
                            var paxType = (PassengerType)paxDetail.PassengerTypeId;

                            #region if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerPerson)

                            var nonSharablePaxCount = 0;
                            foreach (var item in optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Distinct())
                            {
                                nonSharablePaxCount += criteria.NoOfPassengers[(PassengerType)item.PassengerTypeId];
                            }

                            var unitCount = Convert.ToInt32(Math.Ceiling((Double)nonSharablePaxCount / optionDetail.MaxCapacity));
                            decimal nonSharableCostPrice = 0;
                            decimal nonSharableGateBasePrice = 0;

                            optionDetail.MaxCapacity = optionDetail?.MaxCapacity <= 0 ? 1 : optionDetail.MaxCapacity;

                            if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerPerson)
                            {
                                nonSharableCostPrice = optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Max(x => x.CostPrice)
                                    * unitCount * optionDetail.MaxCapacity;
                                nonSharableGateBasePrice = optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Max(x => x.GateBasePrice)
                                    * unitCount * optionDetail.MaxCapacity;
                            }
                            else if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerUnit)
                            {
                                nonSharableCostPrice = optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Max(x => x.CostPrice)
                                    * unitCount;
                                nonSharableGateBasePrice = optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Max(x => x.GateBasePrice)
                                    * unitCount;
                            }
                            else
                            {
                                nonSharableCostPrice = optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Max(x => x.CostPrice) * nonSharablePaxCount;
                                nonSharableGateBasePrice = optionDetail.PaxPrices.Where(x => !x.ShareablePax && x.TravelDate == date).Max(x => x.GateBasePrice) * nonSharablePaxCount;
                            }

                            var sharableCostPrice = new decimal();
                            var sharableGateBasePrice = new decimal();

                            foreach (var item in optionDetail.PaxPrices.Where(x => x.ShareablePax && x.TravelDate == date))
                            {
                                sharableCostPrice += criteria.NoOfPassengers[(PassengerType)item.PassengerTypeId] * item.CostPrice;
                                sharableGateBasePrice += criteria.NoOfPassengers[(PassengerType)item.PassengerTypeId] * item.GateBasePrice;
                            }

                            //single unit price
                            var finalUnitCostPrice = nonSharableCostPrice;// * (optionDetail.MaxCapacity * unitCount);
                            var finalUnitBasePrice = nonSharableGateBasePrice;// * (optionDetail.MaxCapacity * unitCount);

                            foreach (var item in optionDetail.PaxPrices.Where(x => x.TravelDate == date && x.PassengerType == paxType))
                            {
                                var costPricePerPax = decimal.MaxValue;
                                var basePricePerPax = decimal.MaxValue;
                                var priceTotal = decimal.MaxValue;

                                var isWithInCacapcity = totalPaxCount <= item.MaxPaxCapacity;
                                if (item.ShareablePax.Equals(false))
                                {
                                    costPricePerPax = item.CostPrice == 0 ? 0 : finalUnitCostPrice / (isWithInCacapcity ? totalPaxCount : nonSharablePaxCount);
                                    basePricePerPax = item.GateBasePrice == 0 ? 0 : finalUnitBasePrice / (isWithInCacapcity ? totalPaxCount : nonSharablePaxCount);

                                    priceTotal = isCost ?
                                                            (isWithInCacapcity ?
                                                                            finalUnitCostPrice :
                                                                            finalUnitCostPrice + sharableCostPrice
                                                            )
                                                        :
                                                            (isWithInCacapcity ?
                                                                                finalUnitBasePrice :
                                                                                finalUnitBasePrice + sharableGateBasePrice
                                                            )
                                                        ;
                                }
                                else if (item.ShareablePax.Equals(true) && isWithInCacapcity)
                                {
                                    costPricePerPax = item.CostPrice == 0 ? 0 : finalUnitCostPrice / totalPaxCount;
                                    basePricePerPax = item.GateBasePrice == 0 ? 0 : finalUnitBasePrice / totalPaxCount;

                                    priceTotal = isCost ?
                                                    finalUnitCostPrice :
                                                    finalUnitBasePrice;
                                }
                                else
                                {
                                    costPricePerPax = item.CostPrice;
                                    basePricePerPax = item.GateBasePrice;

                                    priceTotal = isCost ?
                                                    finalUnitCostPrice + sharableCostPrice :
                                                    finalUnitBasePrice + sharableGateBasePrice;
                                }

                                var pricePerPax = isCost ? costPricePerPax : basePricePerPax;

                                var paxCount = criteria?.NoOfPassengers?.ContainsKey(paxType) == true ? criteria.NoOfPassengers[paxType] : 0;
                                var pricingUnit = CreatePricingUnit(pricePerPax, paxUnitType, paxType, paxCount);
                                pricingUnit.TotalCapacity = optionDetail.MaxCapacity;

                                if (!priceAndAvailability.PricingUnits.Any(x => ((PerPersonPricingUnit)x)?.PassengerType == paxType))
                                {
                                    priceAndAvailability.TotalPrice = priceTotal;
                                    priceAndAvailability.UnitQuantity = unitCount;
                                    priceAndAvailability.PricingUnits.Add(pricingUnit);
                                }
                            }

                            #endregion if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerPerson)
                        }
                        if (!result.Keys.Contains(date))
                        {
                            result.Add(date, priceAndAvailability);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetPriceAndAvailabilitiesV1"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get PriceAndAvailabilities From DB OptionDetails as per dateRange
        /// 2 UnitType- PerUnit, PriceType- PerUnit
        /// </summary>
        /// <param name="optionDetail"></param>
        /// <param name="defaultPriceAndAvailability"></param>
        /// <param name="criteria"></param>
        /// <param name="isCost">Default id gatePrice</param>
        /// <returns></returns>
        private Dictionary<DateTime, PriceAndAvailability> GetPriceAndAvailabilitiesV2(OptionDetail optionDetail, PriceAndAvailability defaultPriceAndAvailability, Criteria criteria, List<ProductOptionAvailabilty> productOptionsAvailabilties, bool isCost = false)
        {
            try
            {
                var result = new Dictionary<DateTime, PriceAndAvailability>();
                var travelDates = optionDetail.PaxPrices.Select(x => x.TravelDate).Distinct().ToList();
                var paxPrices = optionDetail.PaxPrices;
                var paxUnitType = (UnitType)optionDetail.UnitType;
                var paxPriceType = (PriceType)optionDetail.PriceType;
                var totalPaxCount = criteria?.NoOfPassengers?.Where(x => x.Key != PassengerType.Infant)?.Sum(y => y.Value) ?? 0;
                if (totalPaxCount == 0)
                {
                    var message = "Pax count not found in request criteria";
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    throw new HttpResponseException(data);
                }

                foreach (var date in travelDates)
                {
                    //if option is available on selected date then pick the price from pax detail
                    var availableOptionDateQuery = from od in productOptionsAvailabilties
                                                   from pp in paxPrices
                                                   where pp.ServiceOptionId == od.ServiceOptionId
                                                   && pp.TravelDate == od.PriceDate
                                                   && pp.TravelDate == date
                                                   select pp;

                    var optionStatus = from od in productOptionsAvailabilties
                                       from pp in paxPrices
                                       where pp.ServiceOptionId == od.ServiceOptionId
                                       && pp.TravelDate == od.PriceDate
                                       && pp.TravelDate == date
                                       select od;

                    var dateWiseDetails = availableOptionDateQuery.ToList();

                    if (dateWiseDetails?.Count > 0)
                    {
                        var priceAndAvailability = defaultPriceAndAvailability.Clone() as PriceAndAvailability;
                        priceAndAvailability.AvailabilityStatus = optionStatus?.FirstOrDefault()?.AvailableState ?? AvailabilityStatus.NOTAVAILABLE;

                        foreach (var paxDetail in dateWiseDetails)
                        {
                            var paxType = (PassengerType)paxDetail.PassengerTypeId;
                            var paxPricesAsPerDate = optionDetail?.PaxPrices?.Where(x => x.TravelDate == date)?.ToList();
                            var isWithInCacapcity = totalPaxCount <= paxDetail.MaxPaxCapacity;

                            #region if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerUnit)

                            var nonSharablePaxCount = 0;
                            foreach (var item in paxPricesAsPerDate.Where(x => !x.ShareablePax && x.TravelDate == date).Distinct())
                            {
                                nonSharablePaxCount += criteria.NoOfPassengers[(PassengerType)item.PassengerTypeId];
                            }

                            var unitCount = Convert.ToInt32(Math.Ceiling((Double)nonSharablePaxCount / optionDetail.MaxCapacity));
                            var maxCostPrice = paxPricesAsPerDate.Max(x => x.CostPrice);
                            var maxBasePrice = paxPricesAsPerDate.Max(x => x.GateBasePrice);
                            var paxCountWithNonZeroAmount = paxPricesAsPerDate.Count(x => x.GateBasePrice != 0 && x.CostPrice != 0);

                            foreach (var item in paxPricesAsPerDate)
                            {
                                var costPricePerPax = decimal.MaxValue;
                                var basePricePerPax = decimal.MaxValue;
                                var pricePerPaxMax = isCost ? maxCostPrice : maxBasePrice;
                                var totalPrice = pricePerPaxMax * unitCount;

                                costPricePerPax = totalPrice / totalPaxCount;
                                basePricePerPax = totalPrice / totalPaxCount;

                                var pricePerPax = isCost ? costPricePerPax : basePricePerPax;

                                var paxCount = criteria.NoOfPassengers.ContainsKey(paxType) ? criteria.NoOfPassengers[paxType] : 0;
                                var pricingUnit = CreatePricingUnit(pricePerPax, paxUnitType, paxType, paxCount);
                                pricingUnit.TotalCapacity = optionDetail.MaxCapacity;

                                if (!priceAndAvailability.PricingUnits.Any(x => ((PerPersonPricingUnit)x)?.PassengerType == paxType))
                                {
                                    priceAndAvailability.TotalPrice = totalPrice;
                                    priceAndAvailability.UnitQuantity = unitCount;
                                    priceAndAvailability.PricingUnits.Add(pricingUnit);
                                }
                            }

                            #endregion if (paxUnitType == UnitType.PerUnit && paxPriceType == PriceType.PerUnit)
                        }
                        if (!result.Keys.Contains(date))
                        {
                            result.Add(date, priceAndAvailability);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetPriceAndAvailabilitiesV2"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Filter out price and availability based on original check-in checkout date and availability status
        /// </summary>
        /// <param name="datePriceAndAvailbaities"></param>
        /// <param name="checkinDate"></param>
        /// <param name="checkoutDate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Dictionary<DateTime, PriceAndAvailability> FilterPriceAndAvailabilitiesAsPerInputDates(Dictionary<DateTime, PriceAndAvailability> datePriceAndAvailbaities, DateTime checkinDate, DateTime checkoutDate, string token)
        {
            try
            {
                var result = new Dictionary<DateTime, PriceAndAvailability>();
                if (datePriceAndAvailbaities?.Any() == true)
                {
                    foreach (var dpa in datePriceAndAvailbaities)
                    {
                        try
                        {
                            if (dpa.Key >= checkinDate && dpa.Key <= checkoutDate)
                            {
                                if (dpa.Value.AvailabilityStatus == AvailabilityStatus.AVAILABLE || dpa.Value.AvailabilityStatus == AvailabilityStatus.ONREQUEST)
                                {
                                    if (!result.ContainsKey(dpa.Key))
                                    {
                                        result.Add(dpa.Key, dpa.Value);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                            {
                                ClassName = "ActivityService",
                                MethodName = "FilterPriceAndAvailabilitiesAsPerInputDates",
                                Token = token,
                            };
                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "FilterPriceAndAvailabilitiesAsPerInputDates"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion MultiDay Support for isango products

        /// <summary>
        /// Map ActivityOption data from source to destination object
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="productOption"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private ActivityOption GetMappedActivityOption(ActivityOption option, ProductOption productOption, Criteria criteria)
        {
            try
            {
                return new ActivityOption
                {
                    ActivitySeasons = option.ActivitySeasons,
                    AvailToken = option.AvailToken,
                    Code = option.Code,
                    Contract = option.Contract,
                    ContractQuestions = option.ContractQuestions,
                    HotelPickUpLocation = option.HotelPickUpLocation,
                    PickUpOption = option.PickUpOption,
                    PickupPointDetails = option.PickupPointDetails,
                    PickupPoints = option.PickupPoints,
                    PricingCategoryId = option.PricingCategoryId,
                    PrioTicketClass = option.PrioTicketClass,
                    RateKey = option.RateKey,
                    ScheduleReturnDetails = option.ScheduleReturnDetails,
                    StartTimeId = option.StartTimeId,
                    OptionType = option.OptionType,
                    ServiceType = option.ServiceType,
                    RoomType = option.RoomType,
                    ScheduleId = option.ScheduleId,
                    ProductType = option.ProductType,
                    RefNo = option.RefNo,

                    Cancellable = option.Cancellable,
                    Holdable = option.Holdable,
                    Refundable = option.Refundable,
                    Type = option.Type,
                    HoldablePeriod = option.HoldablePeriod,
                    Time = option.Time,
                    RateId = option.RateId,
                    PriceId = option.PriceId,
                    SupplierId = option.SupplierId,

                    BasePrice = CalculatePriceForAllPax(option.BasePrice, criteria),
                    CostPrice = CalculatePriceForAllPax(option.CostPrice, criteria),
                    GateBasePrice = CalculatePriceForAllPax(option.GateBasePrice, criteria),
                    AvailabilityStatus = option.AvailabilityStatus,
                    Customers = option.Customers,
                    TravelInfo = option.TravelInfo,
                    CommisionPercent = option.CommisionPercent,
                    CancellationPrices = option.CancellationPrices,
                    IsSelected = option.IsSelected,

                    Id = productOption.Id,
                    ServiceOptionId = productOption.Id,
                    Name = productOption.Name,
                    SupplierName = productOption.SupplierName,
                    Description = productOption.Description,
                    BookingStatus = productOption.BookingStatus,
                    OptionKey = productOption.OptionKey,
                    Capacity = productOption.Capacity,
                    Quantity = productOption.Quantity,
                    SupplierOptionCode = productOption.SupplierOptionCode,
                    Margin = productOption.Margin
                };
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetMappedActivityOption"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Price CalculatePriceForAllPax(Price inputPrice, Criteria criteria)
        {
            try
            {
                if (inputPrice == null) return null;
                var price = inputPrice.DeepCopy();

                var isPerUnit = false;
                var perUnitPrice = new decimal();
                var perPersonPrice = new decimal();

                foreach (var priceAndAvailability in price.DatePriceAndAvailabilty)
                {
                    if (priceAndAvailability.Value?.PricingUnits == null) continue;
                    var pricingUnits = priceAndAvailability.Value.PricingUnits;
                    foreach (var pricingUnit in pricingUnits)
                    {
                        if (pricingUnit is PerUnitPricingUnit)
                        {
                            perUnitPrice = pricingUnit.Price;
                            isPerUnit = true;
                        }
                        //else if (pricingUnit is PricingUnit)
                        //{
                        //    perUnitPrice = pricingUnit.Price;
                        //    isPerUnit = true;
                        //}
                        else
                        {
                            var passengerType = ((PerPersonPricingUnit)pricingUnit).PassengerType;
                            var paxCount = GetPaxCountByPaxType(criteria, passengerType);
                            perPersonPrice += pricingUnit.Price * paxCount;
                        }
                    }

                    priceAndAvailability.Value.TotalPrice = isPerUnit ? perUnitPrice : perPersonPrice;
                }
                price.Amount = price.DatePriceAndAvailabilty.Where(x => x.Key.Date == criteria.CheckinDate.Date).
                    Select(x => x.Value.TotalPrice).
                    FirstOrDefault();

                return price;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CalculatePriceForAllPax"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private IsangoErrorEntity SendException(ClientInfo clientInfo, Criteria criteria,
            Int32 activityId, string message, string MethodName, string className,
            Exception ex = null)
        {
            var criteriaGet = new Criteria();
            var criteriaSerialize = string.Empty;
            var clientInfoGet = string.Empty;
            if (criteria != null)
            {
                criteriaSerialize = SerializeDeSerializeHelper.Serialize(criteria);
            }
            if (clientInfo != null)
            {
                clientInfoGet = SerializeDeSerializeHelper.Serialize(clientInfo);
            }
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = className,
                MethodName = MethodName,
                Token = clientInfo?.ApiToken,
                AffiliateId = clientInfo?.AffiliateId,
                Params = $"{activityId}, {criteriaSerialize}, {clientInfoGet}"
            };
            if (!string.IsNullOrEmpty(message))
            {
                _log.Error(isangoErrorEntity, new Exception(message));
                var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = message
                };
                _activityPersistence.InsertApiErrorLog(activityId, message, MethodName);
                //throw new HttpResponseException(data);
            }
            else
            {
                _log.Error(isangoErrorEntity, ex);
            }
            return isangoErrorEntity;
        }

        private ActivityOption MapActivityOptionFromAPI(ProductOption cachedAO, ProductOption apiPO)
        {
            try
            {
                var apiAO = apiPO as ActivityOption;
                var newActivityOption = new ActivityOption();
                if (apiAO != null)
                {
                    newActivityOption = new ActivityOption
                    {
                        ActivitySeasons = apiAO.ActivitySeasons,//apiAO?.ActivitySeasons?.Any() == true ? apiAO.ActivitySeasons : cachedAO.ActivitySeasons,
                        AvailToken = apiAO.AvailToken,//!string.IsNullOrWhiteSpace(apiAO.AvailToken) ? apiAO.AvailToken : cachedAO.AvailToken,
                        AllDay = apiAO.AllDay,
                        AllocationCapacity = cachedAO.AllocationCapacity,
                        ApiCancellationPolicy = cachedAO.ApiCancellationPolicy,
                        AvailabilityStatus = apiAO.AvailabilityStatus,
                        BasePrice = apiAO.BasePrice,
                        BookingStatus = cachedAO.BookingStatus,
                        BundleOptionID = cachedAO.BundleOptionID,
                        BundleOptionName = cachedAO.BundleOptionName,
                        Cancellable = apiAO.Cancellable == true,//? apiAO.Cancellable : cachedAO.Cancellable,
                        CancellationPrices = apiAO?.CancellationPrices?.Any() == true ? apiAO.CancellationPrices : cachedAO.CancellationPrices,
                        CancellationText = !string.IsNullOrWhiteSpace(apiAO.CancellationText) ? apiAO.CancellationText : cachedAO.CancellationText,
                        Capacity = apiAO.Capacity > 0 ? apiAO.Capacity : cachedAO.Capacity,
                        CloseOuts = apiAO?.CloseOuts?.Any() == true ? apiAO.CloseOuts : cachedAO.CloseOuts,
                        Code = apiAO.Code,
                        CommisionPercent = apiAO.CommisionPercent,
                        ComponentOrder = cachedAO.ComponentOrder,
                        ComponentServiceID = cachedAO.ComponentServiceID,
                        ComponentServiceName = cachedAO.ComponentServiceName,
                        Contract = apiAO?.Contract,// != null ? apiAO.Contract : cachedAO.Contract,
                        ContractQuestions = apiAO?.ContractQuestions,//?.Any() == true ? apiAO.ContractQuestions : cachedAO.ContractQuestions,
                        CostPrice = apiAO.CostPrice,
                        Customers = cachedAO.Customers,
                        Description = cachedAO.Description,
                        EndLocalTime = apiAO.EndLocalTime,
                        EndTime = apiAO.EndTime,
                        GateBasePrice = apiAO.GateBasePrice,
                        GateSellPrice = cachedAO.GateSellPrice,
                        Holdable = apiAO.Holdable,// == true ? apiAO.Holdable : cachedAO.Holdable,
                        HoldablePeriod = apiAO.HoldablePeriod,// > 0 ? apiAO.HoldablePeriod : cachedAO.HoldablePeriod,
                        HotelPickUpLocation = apiAO.HotelPickUpLocation,
                        Id = apiAO.Id,
                        IsCapacityCheckRequired = apiAO.IsCapacityCheckRequired == true ? apiAO.IsCapacityCheckRequired : cachedAO.IsCapacityCheckRequired,
                        IsMandatoryApplyAmount = apiAO.IsMandatoryApplyAmount,
                        IsSameDayBookable = apiAO.IsSameDayBookable == true ? apiAO.IsSameDayBookable : cachedAO.IsSameDayBookable,
                        IsSelected = apiAO.IsSelected == true ? apiAO.IsSelected : cachedAO.IsSelected,
                        Margin = apiAO.Margin,
                        MeetingPointDetails = apiAO?.MeetingPointDetails,// != null ? apiAO.MeetingPointDetails : cachedAO.MeetingPointDetails,
                        Name = apiAO.Name,
                        OfferCode = apiAO.OfferCode,
                        OfferTitle = apiAO.OfferTitle,
                        OpeningHoursDetails = apiAO.OpeningHoursDetails,
                        OptionKey = !string.IsNullOrWhiteSpace(apiAO.OptionKey) ? apiAO.OptionKey : cachedAO.OptionKey,
                        OptionOrder = cachedAO.OptionOrder,
                        OptionPickupLocations = apiAO?.OptionPickupLocations?.Any() == true ? apiAO.OptionPickupLocations : cachedAO.OptionPickupLocations,
                        OptionType = apiAO.OptionType,//!string.IsNullOrWhiteSpace(apiAO.OptionType) ? apiAO.OptionType : cachedAO.OptionType,
                        PickUpId = apiAO.PickUpId,// > 0 ? apiAO.PickUpId : cachedAO.PickUpId,
                        PickupLocations = apiAO.PickupLocations,//apiAO?.PickupLocations?.Any() == true ? apiAO.PickupLocations : cachedAO.PickupLocations,
                        PickUpOption = apiAO?.PickUpOption != PickUpDropOffOptionType.Undefined ? apiAO.PickUpOption : cachedAO.PickUpOption,
                        PickupPointDetails = apiAO.PickupPointDetails,//apiAO?.PickupPointDetails?.Any() == true ? apiAO.PickupPointDetails : cachedAO.PickupPointDetails,
                        pickupPointId = apiAO.pickupPointId,//!string.IsNullOrWhiteSpace(apiAO.pickupPointId) ? apiAO.pickupPointId : cachedAO.pickupPointId,
                        PickupPoints = apiAO.PickupPoints,//!string.IsNullOrWhiteSpace(apiAO.PickupPoints) ? apiAO.PickupPoints : cachedAO.PickupPoints,
                        PickupPointsDetailsForVentrata = apiAO.PickupPointsDetailsForVentrata,
                        PrefixServiceCode = cachedAO.PrefixServiceCode,
                        PriceId = apiAO.PriceId,//apiAO?.PriceId?.Any() == true ? apiAO.PriceId : cachedAO.PriceId,
                        PriceOffers = cachedAO.PriceOffers,
                        PriceTypeID = apiAO.PriceTypeID,
                        PricingCategoryId = apiAO.PricingCategoryId,
                        PrioTicketClass = apiAO.PrioTicketClass,
                        ProductIDs = apiAO.ProductIDs,//apiAO?.ProductIDs?.Count > 0 ? apiAO.ProductIDs : cachedAO.ProductIDs,
                        ProductType = apiAO.ProductType,// !string.IsNullOrWhiteSpace(apiAO.ProductType) ? apiAO.ProductType : cachedAO.ProductType,
                        Quantity = apiAO.Quantity > 0 ? apiAO.Quantity : cachedAO.Quantity,
                        QuantityRequired = apiAO.QuantityRequired,//apiAO.QuantityRequired == true ? apiAO.QuantityRequired : cachedAO.QuantityRequired,
                        QuantityRequiredMax = apiAO.QuantityRequiredMax,//apiAO.QuantityRequiredMax > 0 ? apiAO.QuantityRequiredMax : cachedAO.QuantityRequiredMax,
                        QuantityRequiredMin = apiAO.QuantityRequiredMin,//apiAO.QuantityRequiredMin > 0 ? apiAO.QuantityRequiredMin : cachedAO.QuantityRequiredMin,
                        RateId = apiAO.RateId,//!string.IsNullOrWhiteSpace(apiAO.RateId) ? apiAO.RateId : cachedAO.RateId,
                        RateKey = apiAO.RateKey,//!string.IsNullOrWhiteSpace(apiAO.RateKey) ? apiAO.RateKey : cachedAO.RateKey,
                        RefNo = apiAO.RateKey,//!string.IsNullOrWhiteSpace(apiAO.RefNo) ? apiAO.RefNo : cachedAO.RefNo,
                        Refundable = apiAO.Refundable,//apiAO.Refundable == true ? apiAO.Refundable : cachedAO.Refundable,
                        RoomType = apiAO.RoomType,//!string.IsNullOrWhiteSpace(apiAO.RoomType) ? apiAO.RoomType : cachedAO.RoomType,
                        ScheduleId = apiAO.ScheduleId,//!string.IsNullOrWhiteSpace(apiAO.ScheduleId) ? apiAO.ScheduleId : cachedAO.ScheduleId,
                        ScheduleReturnDetails = apiAO.ScheduleReturnDetails,//cachedAO.ScheduleReturnDetails,
                        Seats = apiAO.Seats,//apiAO.Seats > 0 ? apiAO.Seats : cachedAO.Seats,
                        SeatsAvailable = apiAO.SeatsAvailable,//apiAO.SeatsAvailable > 0 ? apiAO.SeatsAvailable : cachedAO.SeatsAvailable,
                        SellPrice = apiAO.SellPrice,
                        ServiceOptionId = cachedAO.ServiceOptionId,
                        ServiceType = apiAO.ServiceType,//cachedAO.ServiceType,
                        ShowSale = cachedAO.ShowSale,
                        StartLocalTime = apiAO.StartLocalTime,
                        StartTime = apiAO.StartTime,
                        StartTimeId = apiAO.StartTimeId,
                        SupplierId = apiAO.SupplierId,
                        SupplierName = cachedAO.SupplierName,
                        SupplierOptionCode = cachedAO.SupplierOptionCode,
                        TicketTypeIds = apiAO.TicketTypeIds,//!string.IsNullOrWhiteSpace(apiAO.TicketTypeIds) ? apiAO.TicketTypeIds : cachedAO.TicketTypeIds,
                        Time = apiAO.Time,//!string.IsNullOrWhiteSpace(apiAO.Time) ? apiAO.Time : cachedAO.Time,
                        TimesOfDays = apiAO?.TimesOfDays?.Count > 0 ? apiAO?.TimesOfDays : cachedAO.TimesOfDays,
                        TravelInfo = apiAO.TravelInfo,
                        Type = apiAO.Type,//!string.IsNullOrWhiteSpace(apiAO.Type) ? apiAO.Type : cachedAO.Type,
                        UserKey = apiAO.UserKey,//!string.IsNullOrWhiteSpace(apiAO.UserKey) ? apiAO.UserKey : cachedAO.UserKey,
                        Variant = apiAO.Variant,
                        VentrataProductId = apiAO.VentrataProductId,
                        ApiType = apiAO.ApiType
                    };
                }
                else
                {
                    newActivityOption = new ActivityOption
                    {
                        //ActivitySeasons = apiPO.ActivitySeasons,//apiAO?.ActivitySeasons?.Any() == true ? apiPO.ActivitySeasons : cachedAO.ActivitySeasons,
                        //AvailToken = apiPO.AvailToken,//!string.IsNullOrWhiteSpace(apiPO.AvailToken) ? apiPO.AvailToken : cachedAO.AvailToken,
                        //AllDay = apiPO.AllDay,
                        AllocationCapacity = cachedAO.AllocationCapacity,
                        ApiCancellationPolicy = cachedAO.ApiCancellationPolicy,
                        AvailabilityStatus = apiPO.AvailabilityStatus,
                        BasePrice = apiPO.BasePrice,
                        BookingStatus = cachedAO.BookingStatus,
                        BundleOptionID = cachedAO.BundleOptionID,
                        BundleOptionName = cachedAO.BundleOptionName,
                        //Cancellable = apiPO.Cancellable == true,//? apiPO.Cancellable : cachedAO.Cancellable,
                        CancellationPrices = apiAO?.CancellationPrices?.Any() == true ? apiPO.CancellationPrices : cachedAO.CancellationPrices,
                        CancellationText = !string.IsNullOrWhiteSpace(apiPO.CancellationText) ? apiPO.CancellationText : cachedAO.CancellationText,
                        Capacity = apiPO.Capacity > 0 ? apiPO.Capacity : cachedAO.Capacity,
                        CloseOuts = apiAO?.CloseOuts?.Any() == true ? apiPO.CloseOuts : cachedAO.CloseOuts,
                        //Code = apiPO.Code,
                        CommisionPercent = apiPO.CommisionPercent,
                        ComponentOrder = cachedAO.ComponentOrder,
                        ComponentServiceID = cachedAO.ComponentServiceID,
                        ComponentServiceName = cachedAO.ComponentServiceName,
                        Contract = apiAO?.Contract,// != null ? apiPO.Contract : cachedAO.Contract,
                        ContractQuestions = apiAO?.ContractQuestions,//?.Any() == true ? apiPO.ContractQuestions : cachedAO.ContractQuestions,
                        CostPrice = apiPO.CostPrice,
                        Customers = cachedAO.Customers,
                        Description = cachedAO.Description,
                        //EndLocalTime = apiPO.EndLocalTime,
                        EndTime = apiPO.EndTime,
                        GateBasePrice = apiPO.GateBasePrice,
                        GateSellPrice = cachedAO.GateSellPrice,
                        //Holdable = apiPO.Holdable,// == true ? apiPO.Holdable : cachedAO.Holdable,
                        //HoldablePeriod = apiPO.HoldablePeriod,// > 0 ? apiPO.HoldablePeriod : cachedAO.HoldablePeriod,
                        //HotelPickUpLocation = apiPO.HotelPickUpLocation,
                        Id = apiPO.Id,
                        IsCapacityCheckRequired = apiPO.IsCapacityCheckRequired == true ? apiPO.IsCapacityCheckRequired : cachedAO.IsCapacityCheckRequired,
                        //IsMandatoryApplyAmount = apiPO.IsMandatoryApplyAmount,
                        IsSameDayBookable = apiPO.IsSameDayBookable == true ? apiPO.IsSameDayBookable : cachedAO.IsSameDayBookable,
                        IsSelected = apiPO.IsSelected == true ? apiPO.IsSelected : cachedAO.IsSelected,
                        Margin = apiPO.Margin,
                        MeetingPointDetails = apiAO?.MeetingPointDetails,// != null ? apiPO.MeetingPointDetails : cachedAO.MeetingPointDetails,
                        Name = apiPO.Name,
                        //OfferCode = apiPO.OfferCode,
                        //OfferTitle = apiPO.OfferTitle,
                        //OpeningHoursDetails = apiPO.OpeningHoursDetails,
                        OptionKey = !string.IsNullOrWhiteSpace(apiPO.OptionKey) ? apiPO.OptionKey : cachedAO.OptionKey,
                        OptionOrder = cachedAO.OptionOrder,
                        OptionPickupLocations = apiAO?.OptionPickupLocations?.Any() == true ? apiPO.OptionPickupLocations : cachedAO.OptionPickupLocations,
                        // OptionType = apiPO.OptionType,//!string.IsNullOrWhiteSpace(apiPO.OptionType) ? apiPO.OptionType : cachedAO.OptionType,
                        //PickUpId = apiPO.PickUpId,// > 0 ? apiPO.PickUpId : cachedAO.PickUpId,
                        // PickupLocations = apiPO.PickupLocations,//apiAO?.PickupLocations?.Any() == true ? apiPO.PickupLocations : cachedAO.PickupLocations,
                        //PickUpOption = apiAO?.PickUpOption != PickUpDropOffOptionType.Undefined ? apiPO.PickUpOption : cachedAO.PickUpOption,
                        //PickupPointDetails = apiPO.PickupPointDetails,//apiAO?.PickupPointDetails?.Any() == true ? apiPO.PickupPointDetails : cachedAO.PickupPointDetails,
                        // pickupPointId = apiPO.pickupPointId,//!string.IsNullOrWhiteSpace(apiPO.pickupPointId) ? apiPO.pickupPointId : cachedAO.pickupPointId,
                        ////PickupPoints = apiPO.PickupPoints,//!string.IsNullOrWhiteSpace(apiPO.PickupPoints) ? apiPO.PickupPoints : cachedAO.PickupPoints,
                        //PickupPointsDetailsForVentrata = apiPO.PickupPointsDetailsForVentrata,
                        PrefixServiceCode = cachedAO.PrefixServiceCode,
                        //PriceId = apiPO.PriceId,//apiAO?.PriceId?.Any() == true ? apiPO.PriceId : cachedAO.PriceId,
                        PriceOffers = cachedAO.PriceOffers,
                        PriceTypeID = apiPO.PriceTypeID,
                        //PricingCategoryId = apiPO.PricingCategoryId,
                        //PrioTicketClass = apiPO.PrioTicketClass,
                        //ProductIDs = apiPO.ProductIDs,//apiAO?.ProductIDs?.Count > 0 ? apiPO.ProductIDs : cachedAO.ProductIDs,
                        //ProductType = apiPO.ProductType,// !string.IsNullOrWhiteSpace(apiPO.ProductType) ? apiPO.ProductType : cachedAO.ProductType,
                        Quantity = apiPO.Quantity > 0 ? apiPO.Quantity : cachedAO.Quantity,
                        //QuantityRequired = apiPO.QuantityRequired,//apiPO.QuantityRequired == true ? apiPO.QuantityRequired : cachedAO.QuantityRequired,
                        //QuantityRequiredMax = apiPO.QuantityRequiredMax,//apiPO.QuantityRequiredMax > 0 ? apiPO.QuantityRequiredMax : cachedAO.QuantityRequiredMax,
                        //QuantityRequiredMin = apiPO.QuantityRequiredMin,//apiPO.QuantityRequiredMin > 0 ? apiPO.QuantityRequiredMin : cachedAO.QuantityRequiredMin,
                        //RateId = apiPO.RateId,//!string.IsNullOrWhiteSpace(apiPO.RateId) ? apiPO.RateId : cachedAO.RateId,
                        //RateKey = apiPO.RateKey,//!string.IsNullOrWhiteSpace(apiPO.RateKey) ? apiPO.RateKey : cachedAO.RateKey,
                        //RefNo = apiPO.RateKey,//!string.IsNullOrWhiteSpace(apiPO.RefNo) ? apiPO.RefNo : cachedAO.RefNo,
                        //Refundable = apiPO.Refundable,//apiPO.Refundable == true ? apiPO.Refundable : cachedAO.Refundable,
                        //RoomType = apiPO.RoomType,//!string.IsNullOrWhiteSpace(apiPO.RoomType) ? apiPO.RoomType : cachedAO.RoomType,
                        //ScheduleId = apiPO.ScheduleId,//!string.IsNullOrWhiteSpace(apiPO.ScheduleId) ? apiPO.ScheduleId : cachedAO.ScheduleId,
                        //ScheduleReturnDetails = apiPO.ScheduleReturnDetails,//cachedAO.ScheduleReturnDetails,
                        //Seats = apiPO.Seats,//apiPO.Seats > 0 ? apiPO.Seats : cachedAO.Seats,
                        //SeatsAvailable = apiPO.SeatsAvailable,//apiPO.SeatsAvailable > 0 ? apiPO.SeatsAvailable : cachedAO.SeatsAvailable,
                        SellPrice = apiPO.SellPrice,
                        ServiceOptionId = cachedAO.ServiceOptionId,
                        //ServiceType = apiPO.ServiceType,//cachedAO.ServiceType,
                        ShowSale = cachedAO.ShowSale,
                        //StartLocalTime = apiPO.StartLocalTime,
                        StartTime = apiPO.StartTime,
                        //StartTimeId = apiPO.StartTimeId,
                        //SupplierId = apiPO.SupplierId,
                        SupplierName = cachedAO.SupplierName,
                        SupplierOptionCode = cachedAO.SupplierOptionCode,
                        //TicketTypeIds = apiPO.TicketTypeIds,//!string.IsNullOrWhiteSpace(apiPO.TicketTypeIds) ? apiPO.TicketTypeIds : cachedAO.TicketTypeIds,
                        //Time = apiPO.Time,//!string.IsNullOrWhiteSpace(apiPO.Time) ? apiPO.Time : cachedAO.Time,
                        //TimesOfDays = apiAO?.TimesOfDays?.Count > 0 ? apiAO?.TimesOfDays : cachedAO.TimesOfDays,
                        TravelInfo = apiPO.TravelInfo,
                        //Type = apiPO.Type,//!string.IsNullOrWhiteSpace(apiPO.Type) ? apiPO.Type : cachedAO.Type,
                        //UserKey = apiPO.UserKey,//!string.IsNullOrWhiteSpace(apiPO.UserKey) ? apiPO.UserKey : cachedAO.UserKey,
                        Variant = apiPO.Variant,
                        //VentrataProductId = apiPO.VentrataProductId,
                        ApiType = apiPO.ApiType
                    };
                }

                return newActivityOption;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool FilterBundlePricesAsPerCheckInDate(Price price, Criteria criteria)
        {
            var result = price.DeepCopy();
            var basePrice = price?.DatePriceAndAvailabilty?.FirstOrDefault(x => x.Key == criteria.CheckinDate && x.Value != null).Value;
            if (basePrice != null)
            {
                price.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();
                price.DatePriceAndAvailabilty.Add(criteria.CheckinDate, basePrice);
            }
            else
            {
                return false;
            }
            return true;
        }

        private void UpdateError(Activity activity, Criteria criteria, string errorCode, string message)
        {
            if (activity == null)
            {
                activity = new Activity
                {
                    Id = criteria.ActivityId.ToString(),
                    ID = criteria.ActivityId,
                    Errors = new List<Error>()
                };
            }
            if (activity.Errors == null)
            {
                activity.Errors = new List<Error>();
            }
            if (activity?.Errors?.Any(x => x?.Code?.ToUpper() == errorCode?.ToUpper()) == false)
            {
                activity.Errors.Add(new Error
                {
                    Code = errorCode,
                    HttpStatus = System.Net.HttpStatusCode.NotFound,
                    Message = message
                });
            }
        }
    }
}