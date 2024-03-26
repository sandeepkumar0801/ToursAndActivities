using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.Payment;
using Isango.Entities.v1Css;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using PriceRuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using WebAPI.Models.RequestModels;
using Logger.Contract;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Helper
{
    /// <summary>
    /// Helper method for activity controller
    /// </summary>
    public class ActivityHelper
    {
        private readonly IActivityService _activityService;
        private readonly PricingController _pricingController;
        private readonly IAffiliateService _affiliateService;
        private readonly IMasterService _masterService;
        private readonly ISearchService _searchService;
        private readonly IApplicationService _applicationService;
        private ParallelOptions _parallelOptions;
        private readonly ILogger _log;

        /// <summary>
        /// Initialize activity helper class.
        /// </summary>
        /// <param name="activityService"></param>
        /// <param name="affiliateService"></param>
        /// <param name="pricingController"></param>
        /// <param name="masterService"></param>
        /// <param name="searchService"></param>
        /// <param name="applicationService"></param>
        /// <param name="log"></param>
        public ActivityHelper(IActivityService activityService, IAffiliateService affiliateService, PricingController pricingController
        , IMasterService masterService, ISearchService searchService, IApplicationService applicationService = null, ILogger log = null)
        {
            _activityService = activityService;
            _pricingController = pricingController;
            _affiliateService = affiliateService;
            _masterService = masterService;
            _searchService = searchService;
            _applicationService = applicationService;
            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Constant.ParallelProcessorCount
            };
            _log = log;
        }

        /// <summary>
        /// Get Search Details
        /// </summary>
        /// <param name="requestCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public SearchStack GetSearchDetails(SearchRequestCriteria requestCriteria, ClientInfo clientInfo)
        {
            var criteria = GetDefaultCriteria();
            var searchCriteria = new SearchCriteria
            {
                RegionId = requestCriteria.RegionId
            };

            var searchData = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria)?.GetAwaiter().GetResult();

            return searchData;
        }

        /// <summary>
        /// Get Search Details
        /// </summary>
        /// <param name="requestCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public SearchStack GetSearchDetailsV2(SearchRequestCriteria requestCriteria, ClientInfo clientInfo)
        {
            var criteria = GetDefaultCriteria();
            var searchCriteria = new SearchCriteria
            {
                RegionId = Convert.ToInt32(requestCriteria.RegionId),
                CategoryId = Convert.ToInt32(requestCriteria.CategoryId),
                Keyword = requestCriteria.KeyWord
            };

            var searchData = _searchService.GetSearchDataV2Async(searchCriteria, clientInfo, criteria)?.GetAwaiter().GetResult();

            return searchData;
        }

        /// <summary>
        /// Prepare Client Info Input
        /// </summary>
        /// <param name="availabilityInput"></param>
        /// <returns></returns>
        public ClientInfo PrepareClientInfoInput(CheckAvailabilityRequest availabilityInput)
        {
            var affiliate = _affiliateService.GetAffiliateInformationAsync(availabilityInput.AffiliateId).Result;
            var languageCode = string.IsNullOrWhiteSpace(availabilityInput.LanguageCode) ? "en" : availabilityInput.LanguageCode;
            var currencyCode = string.IsNullOrWhiteSpace(availabilityInput.CurrencyIsoCode) ? "gbp" : availabilityInput.CurrencyIsoCode;
            var clientInfo = new ClientInfo
            {
                LanguageCode = languageCode,
                Currency = new Currency { IsoCode = currencyCode },
                AffiliateId = availabilityInput.AffiliateId,
                IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate,
                B2BAffiliateId = affiliate.B2BAffiliateId ?? availabilityInput.AffiliateId,
                IsB2BNetPriceAffiliate = affiliate.AffiliateConfiguration.IsB2BNetPriceAffiliate,
                ApiToken = string.IsNullOrWhiteSpace(availabilityInput.TokenId)
                    ? Guid.NewGuid().ToString()
                    : availabilityInput.TokenId,
                CountryIp = availabilityInput.CountryIp,
                IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer
            };

            return clientInfo;
        }

        /// <summary>
        /// Prepare Client Info Input For Bundle Option
        /// </summary>
        /// <param name="availabilityInput"></param>
        /// <returns></returns>
        public ClientInfo PrepareClientInfoInputForBundleOption(CheckBundleAvailabilityRequest availabilityInput)
        {
            var affiliate = _affiliateService.GetAffiliateInformationAsync(availabilityInput?.AffiliateId)?.GetAwaiter().GetResult();
            var clientInfo = new ClientInfo
            {
                LanguageCode = availabilityInput?.LanguageCode ?? "en",
                Currency = new Currency
                {
                    IsoCode = availabilityInput?.CurrencyIsoCode ?? "GBP"
                },
                ApiToken = string.IsNullOrWhiteSpace(availabilityInput.TokenId)
                    ? Guid.NewGuid().ToString()
                    : availabilityInput.TokenId,
                AffiliateId = availabilityInput.AffiliateId,
                IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate,
                B2BAffiliateId = affiliate.B2BAffiliateId ?? availabilityInput.AffiliateId,
                IsB2BNetPriceAffiliate = affiliate.AffiliateConfiguration.IsB2BNetPriceAffiliate,
                CountryIp = availabilityInput.CountryIp,
                IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer
            };

            return clientInfo;
        }

        /// <summary>
        /// Get Activities After Price Rule Engine
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="clientInfo"></param>
        /// <param name="checkinDateTime"></param>
        /// <returns></returns>
        public List<Activity> GetActivitiesAfterPriceRuleEngine(List<Activity> activities, ClientInfo clientInfo, DateTime checkinDateTime)
        {
            //foreach (var activity in activities)
            //{
            //    if (activity.ActivityType != ActivityType.Bundle)
            //    {
            //        activity.ProductOptions = GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId, activity.ProductOptions,
            //            clientInfo, checkinDateTime);
            //    }
            //    else
            //    {
            //        activity.ProductOptions = GetBundleProductOptionsAfterPriceRuleEngine(activity.ProductOptions, clientInfo, activity.PriceTypeId);
            //    }
            //}
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            Parallel.ForEach(activities, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, activity =>
            {
                try
                {
                    if (activity.ActivityType != ActivityType.Bundle)
                    {
                        activity.ProductOptions = GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId, activity.ProductOptions,
                            clientInfo, checkinDateTime);
                    }
                    else
                    {
                        activity.ProductOptions = GetBundleProductOptionsAfterPriceRuleEngine(activity.ProductOptions, clientInfo, activity.PriceTypeId);
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            });

            return activities;
        }

        /// <summary>
        /// Get Default Criteria
        /// </summary>
        /// <returns></returns>
        public Criteria GetDefaultCriteria()
        {
            var checkinDate = DateTime.Today;
            var criteria = new Criteria
            {
                CheckinDate = checkinDate,
                CheckoutDate = checkinDate.AddDays(6),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 }
                },
                Ages = new Dictionary<PassengerType, int>()
            };
            return criteria;
        }

        /// <summary>
        /// Get Product Availability
        /// </summary>
        /// <param name="availabilityInput"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Activity GetProductAvailability(CheckAvailabilityRequest availabilityInput, ClientInfo clientInfo, Criteria criteria)
        {
            var activity = new Activity
            {
                ID = criteria.ActivityId
            };

            foreach (var item in availabilityInput.PaxDetails)
            {
                if (item.Count != 0)
                {
                    try
                    {
                        if (!criteria.NoOfPassengers.Keys.Contains(item.PassengerTypeId))
                            criteria.NoOfPassengers.Add(item.PassengerTypeId, item.Count);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ActivityHelper",
                            MethodName = "GetProductAvailability",
                            Token = clientInfo?.ApiToken,
                            AffiliateId = clientInfo?.AffiliateId,
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
            }

            try
            {
                activity = _activityService.GetProductAvailabilityAsync(availabilityInput.ActivityId, clientInfo, criteria)?.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityHelper",
                    MethodName = "GetProductAvailability",
                    Token = clientInfo?.ApiToken,
                    AffiliateId = clientInfo?.AffiliateId,
                };

                activity.Errors.Add(new Error
                {
                    Code = CommonErrorCodes.AvailabilityError.ToString(),
                    HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });

                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// Get Bundle Product Availability
        /// </summary>
        /// <param name="availabilityInput"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteriaForActivity"></param>
        /// <returns></returns>
        public Activity GetBundleProductAvailability(CheckBundleAvailabilityRequest availabilityInput, ClientInfo clientInfo, Dictionary<int, Criteria> criteriaForActivity)
        {
            var activity = _activityService.GetBundleProductAvailabilityAsync(availabilityInput.ActivityId, clientInfo, criteriaForActivity)?.GetAwaiter().GetResult();
            return activity;
        }

        /// <summary>
        /// Get Product Options After Price Rule Engine
        /// </summary>
        /// <param name="priceTypeId"></param>
        /// <param name="productOptions"></param>
        /// <param name="clientInfo"></param>
        /// <param name="checkinDateTime"></param>
        /// <param name="apiType"></param>
        /// <param name="isQrScanDiscountApplicable"></param>
        /// <returns></returns>
        public List<ProductOption> GetProductOptionsAfterPriceRuleEngine(PriceTypeId priceTypeId, List<ProductOption> productOptions, ClientInfo clientInfo, DateTime checkinDateTime, APIType apiType = APIType.Undefined, bool isQrScanDiscountApplicable = false)
        {
            var pricingRequest = new PricingRuleRequest
            {
                PriceTypeId = priceTypeId,
                Criteria = new Criteria
                {
                    CheckinDate = checkinDateTime
                },
                //Creating product option for both Available and Not Available status.
                ProductOptions = productOptions.ToList(),//.Where(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE).ToList(),
                ClientInfo = clientInfo,
                IsQrScanDiscountApplicable = isQrScanDiscountApplicable
            };
            var updatedOptions = _pricingController.Process(pricingRequest);
            updatedOptions.ForEach(e =>
            {
                if (string.IsNullOrEmpty(e.BundleOptionID.ToString()) || e.BundleOptionID == 0)
                    productOptions[productOptions.FindIndex(x => x.Id == e.Id && x.StartTime == e.StartTime)] = e;
                else
                    productOptions[productOptions.FindIndex(x => x.Id == e.Id && x.BundleOptionID == e.BundleOptionID)] = e;
            });

            return productOptions;
        }

        /// <summary>
        /// Calculate Activity With Min Prices
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public Activity CalculateActivityWithMinPrices(Activity activity)
        {
            return _activityService.CalculateActivityWithMinPricesAsync(activity)?.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Bundle Product Options After Price Rule Engine
        /// </summary>
        /// <param name="productOptions"></param>
        /// <param name="clientInfo"></param>
        /// <param name="priceTypeId"></param>
        /// <param name="componentActivityDetails"></param>
        /// <param name="isQrScanDiscountApplicable"></param>
        /// <returns></returns>
        public List<ProductOption> GetBundleProductOptionsAfterPriceRuleEngine(List<ProductOption> productOptions, ClientInfo clientInfo, PriceTypeId priceTypeId, List<ComponentActivityDetail> componentActivityDetails = null, bool isQrScanDiscountApplicable = false)
        {
            DateTime checkinDate;
            foreach (var componentServiceId in productOptions.Select(e => e.ComponentServiceID).Distinct().ToList())
            {
                if (componentActivityDetails != null)
                {
                    checkinDate = Convert.ToDateTime(componentActivityDetails
                        .Where(x => x.ComponentActivityId == componentServiceId).Select(x => x.CheckinDate).First());
                }
                else
                {
                    checkinDate = DateTime.Now;
                }

                var options = productOptions.Where(e => e.ComponentServiceID == componentServiceId).ToList();
                var updatedOptions = GetProductOptionsAfterPriceRuleEngine(priceTypeId, options, clientInfo, checkinDate, APIType.Undefined, isQrScanDiscountApplicable);
                updatedOptions.ForEach(e =>
                {
                    productOptions[productOptions.FindIndex(x => x.Id == e.Id && x.BundleOptionID == e.BundleOptionID)] = e;
                });
            }

            return productOptions;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public IEnumerable<CalendarAvailability> GetPriceAndAvailabilities(int activityId, string affiliateId)
        {
            var activityPriceAndAvailability = _activityService.GetCalendarAvailabilityAsync(activityId, affiliateId)?.GetAwaiter().GetResult();
            //var filterActivityPriceAndAvailability = activityPriceAndAvailability.Count > 0
            //    ? activityPriceAndAvailability
            //    : _activityService.GetCalendarAvailabilityAsync(activityId, "default").Result;
            return activityPriceAndAvailability;
        }

        /// <summary>
        /// Get Affiliate Information
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public Affiliate GetAffiliateInfo(string affiliateId)
        {
            var affiliate = _affiliateService.GetAffiliateInformationAsync(affiliateId)?.GetAwaiter().GetResult();
            return affiliate;
        }

        public Affiliate GetAffiliateInfoV2(string affiliateId)
        {
            var affiliate = _affiliateService.GetAffiliateInformationV2Async(affiliateId)?.GetAwaiter().GetResult();
            return affiliate;
        }

        /// <summary>
        /// Get Activities After Price Rule Engine
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="checkinDateTime"></param>
        /// <returns></returns>
        public Activity GetActivityAfterPriceRuleEngine(Activity activity, ClientInfo clientInfo, DateTime checkinDateTime)
        {
            if (activity.ActivityType != ActivityType.Bundle)
            {
                activity.ProductOptions = GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId, activity.ProductOptions,
                    clientInfo, checkinDateTime);
            }
            else
            {
                activity.ProductOptions = GetBundleProductOptionsAfterPriceRuleEngine(activity.ProductOptions, clientInfo, activity.PriceTypeId);
            }
            return activity;
        }

        /// <summary>
        /// Get exchange rate
        /// </summary>
        /// <param name="activityIsoCode"></param>
        /// <param name="isoCodes"></param>
        /// <returns></returns>
        public Dictionary<string, decimal> GetConversionFactor(string activityIsoCode, List<string> isoCodes)
        {
            var exchangeRateValues = new Dictionary<string, decimal>();
            foreach (var isoCode in isoCodes)
            {
                var exchangeRate = _masterService.GetConversionFactorAsync(isoCode, activityIsoCode).Result;
                exchangeRateValues.Add(isoCode, exchangeRate);
            }
            return exchangeRateValues;
        }

        /// <summary>
        /// Prepare Client Info For Activity
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ClientInfo PrepareClientInfoForActivity(string languageCode)
        {
            var clientInfo = new ClientInfo();
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };

            clientInfo.AffiliateDisplayName = "Isango";
            clientInfo.AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            clientInfo.AffiliateName = "Isango";
            clientInfo.AffiliatePartner = null;
            clientInfo.B2BAffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            clientInfo.CityCode = null;
            clientInfo.CompanyAlias = "ien";
            clientInfo.CountryIp = "GB";

            clientInfo.DiscountCode = "";
            clientInfo.DiscountCodePercentage = 0M;
            clientInfo.FacebookAppId = "656660554485822";
            clientInfo.FacebookAppSecret = "af34c66444b9c19d38bc4e55cf2d54cf";
            clientInfo.GtmIdentifier = "GTM-PSQPTWZ";
            clientInfo.IsB2BAffiliate = true;
            clientInfo.IsB2BNetPriceAffiliate = true;
            clientInfo.IsSupplementOffer = true;
            clientInfo.LanguageCode = !String.IsNullOrEmpty(languageCode) ? languageCode.ToUpperInvariant() : languageCode;
            clientInfo.LineOfBusiness = "TOURS & ACTIVITIES - isango!";
            clientInfo.PaymentMethodType = PaymentMethodType.Transaction;
            clientInfo.WidgetDate = DateTime.Now; // This date value is not valid

            currency.IsPostFix = false;
            currency.IsoCode = "GBP";
            currency.Name = "GBP";
            currency.Symbol = "£";

            clientInfo.Currency = currency;
            clientInfo.ApiToken = Guid.NewGuid().ToString();

            return clientInfo;
        }

        public Dictionary<int, Tuple<decimal, decimal, string>> GetSearchAffiliateWiseServiceMinPrice(List<int> activities, Affiliate affiliate, ClientInfo clientInfo)
        {
            if (activities?.Any() == false)
            {
                return null;
            }
            var resultPrice = Tuple.Create(decimal.MaxValue, decimal.MaxValue, string.Empty);
            var finalResult = new Dictionary<int, Tuple<decimal, decimal, string>>();
            var targetCurrency = clientInfo?.Currency?.IsoCode ?? Constant.DefaultCurrencyISOCode;

            var isB2BNetPriceAffiliate = affiliate?.AffiliateConfiguration?.IsB2BNetPriceAffiliate ?? false;
            var isSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;
            var affiliateWiseServiceMinPrices = GetAffiliateWiseServiceMinPrices();
            var b2bNetRateRule = _applicationService.GetB2BNetRateRuleAsync(affiliate.Id)?.GetAwaiter().GetResult();

            var netRatePercent = b2bNetRateRule?.NetRatePercent / 100;
            var netPriceType = b2bNetRateRule?.NetPriceType;

            Parallel.ForEach(activities, _parallelOptions, serviceId =>
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
            });

            return finalResult;
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

        public Activity LoadActivity(int activityId, DateTime startDate, ClientInfo clientInfo)
        {
            var activity = _activityService.LoadActivityAsync(activityId, startDate, clientInfo)?.GetAwaiter().GetResult();

            return activity;
        }

        public void UpdateErrorActivity(Activity activity, Criteria criteria, string errorCode, string message, System.Net.HttpStatusCode httpStatusCode)
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
                    HttpStatus = httpStatusCode,
                    Message = message
                });
            }
        }

        public void UpdateErrorCheckAvailabilityResponse(List<Models.ResponseModels.CheckAvailabilityResponse> checkAvailabilitiesResponses, string errorCode, string message, System.Net.HttpStatusCode httpStatusCode, Models.ResponseModels.CheckAvailabilityResponse checkAvailabilitiesResponse = null)
        {
            if (checkAvailabilitiesResponse == null)
            {
                checkAvailabilitiesResponse = new Models.ResponseModels.CheckAvailabilityResponse();
            }
            if (checkAvailabilitiesResponse.Errors == null)
            {
                checkAvailabilitiesResponse.Errors = new List<Error>();
            }
            if (checkAvailabilitiesResponse?.Errors?.Any(x => x.Code?.ToUpper() == errorCode?.ToUpper()) == false)
            {
                checkAvailabilitiesResponse.Errors?.Add(new Error
                {
                    Code = errorCode,
                    HttpStatus = httpStatusCode,
                    Message = message
                });
            }
            if (checkAvailabilitiesResponses?.FirstOrDefault() == null)
            {
                checkAvailabilitiesResponses = new List<Models.ResponseModels.CheckAvailabilityResponse>();
                checkAvailabilitiesResponses.Add(checkAvailabilitiesResponse);
            }
            else
            {
                if (checkAvailabilitiesResponses?.Any(y =>
                    y?.Errors?.Any(x => x.Code?.ToUpper() == errorCode?.ToUpper()) == true)
                    == false
                )
                {
                    checkAvailabilitiesResponses?.FirstOrDefault().Errors.AddRange(checkAvailabilitiesResponse.Errors);
                }
            }
        }

        public AvailablePersonTypes GetPersonTypeOptionCacheAvailability(int? activityId = null, int? serviceOptionId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return _masterService.GetPersonTypeOptionCacheAvailability(activityId, serviceOptionId, fromDate, toDate);
        }
    }
}