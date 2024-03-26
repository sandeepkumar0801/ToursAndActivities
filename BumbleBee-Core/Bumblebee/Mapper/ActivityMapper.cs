using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.Master;
using Isango.Entities.Review;
using Isango.Entities.v1Css;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using WebAPI.Helper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using WebAPI.Models.ResponseModels.CheckAvailability;
using WebAPI.Models.ResponseModels.DeltaActivity;
using WebAPI.Models.v1Css;
using Constant = WebAPI.Constants.Constant;
using PriceAndAvailability = WebAPI.Models.ResponseModels.CheckAvailability.PriceAndAvailability;
using PricingUnit = WebAPI.Models.ResponseModels.CheckAvailability.PricingUnit;

namespace WebAPI.Mapper
{
    /// <summary>
    /// Create Response OUTPUT for Bumble Bee from internal entities
    /// </summary>
    public class ActivityMapper
    {
        private readonly IActivityService _activityService;
        private readonly IApplicationService _applicationService;
        private ParallelOptions _parallelOptions;
        private readonly ActivityHelper _activityHelper;

        public ActivityMapper(IActivityService activityService,
            IApplicationService applicationService = null,
            ActivityHelper activityHelper = null)
        {
            _activityService = activityService;
            _applicationService = applicationService;
            _activityHelper = activityHelper;
            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Isango.Service.Constants.Constant.ParallelProcessorCount
            };
        }

        /// <summary>
        /// Maps the Criteria object from the API request
        /// </summary>
        /// <param name="checkAvailabilityRequest"></param>
        /// <returns></returns>
        public Criteria MapProductAvailabilityCriteria(CheckAvailabilityRequest checkAvailabilityRequest)
        {
            var criteria = new Criteria
            {
                CheckinDate = checkAvailabilityRequest.CheckinDate.Date,
                CheckoutDate = checkAvailabilityRequest.CheckoutDate.Date,
                NoOfPassengers = new Dictionary<PassengerType, int>()
            };
            foreach (var item in checkAvailabilityRequest.PaxDetails)
            {
                criteria.NoOfPassengers.Add(item.PassengerTypeId, item.Count);
            }
            return criteria;
        }

        /// <summary>
        /// Maps the ClientInfo object for the CancellationPolicy API from the API request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClientInfo MapClientInfoForCancellationPolicy(CancellationPolicyRequest request)
        {
            return new ClientInfo
            {
                AffiliateId = request.AffiliateId,
                ApiToken = request.TokenId,
                LanguageCode = request.LanguageCode
            };
        }

        /// <summary>
        /// Maps the ClientInfo object for the ActivityDetail API from the API request
        /// </summary>
        /// <param name="activityCriteria"></param>
        /// <returns></returns>
        public ClientInfo MapClientInfoForActivityDetail(ActivityCriteria activityCriteria)
        {
            return new ClientInfo
            {
                AffiliateId = activityCriteria.AffiliateId,
                LanguageCode = activityCriteria.LanguageCode,
                Currency = new Currency
                {
                    IsoCode = activityCriteria.CurrencyIsoCode
                },
                CountryIp = activityCriteria.CountryIp
            };
        }

        /// <summary>
        /// Maps the ClientInfo object for the Search API from the API request
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="affiliate"></param>
        /// <returns></returns>
        public ClientInfo MapClientInfoForSearch(SearchRequestCriteria criteria, Affiliate affiliate)
        {
            return new ClientInfo
            {
                AffiliateId = criteria.AffiliateId,
                CountryIp = criteria.CountryIp,
                IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer,
                IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate,
                LanguageCode = string.IsNullOrEmpty(criteria.LanguageCode) ? Constant.DefaultLanguage : criteria.LanguageCode,
                Currency = new Currency
                {
                    IsoCode = string.IsNullOrEmpty(criteria.CurrencyIsoCode) ? Constant.DefaultCurrency : criteria.CurrencyIsoCode
                },
                B2BAffiliateId = affiliate.B2BAffiliateId
            };
        }

        public ClientInfo MapClientInfoForCalendar(ActivityCriteria criteria, Affiliate affiliate)
        {
            return new ClientInfo
            {
                AffiliateId = criteria.AffiliateId,
                CountryIp = criteria.CountryIp,
                IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer,
                IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate,
                LanguageCode = string.IsNullOrEmpty(criteria.LanguageCode) ? Constant.DefaultLanguage : criteria.LanguageCode,
                Currency = new Currency
                {
                    IsoCode = string.IsNullOrEmpty(criteria.CurrencyIsoCode) ? Constant.DefaultCurrency : criteria.CurrencyIsoCode
                },
                B2BAffiliateId = affiliate.B2BAffiliateId
            };
        }

        /// <summary>
        /// This operation is used to map service response to cancellation detail response model
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<CancellationPolicyResponse> MapCancellationResponse(Activity activity, CancellationPolicyRequest request)
        {
            var cancellationResponseList = new List<CancellationPolicyResponse>();

            var cancellationPriceList = activity?.ProductOptions.FirstOrDefault(x => x.Id == request.ServiceOptionId)?.CancellationPrices;
            if (cancellationPriceList == null) return cancellationResponseList;
            foreach (var cancellationPrice in cancellationPriceList)
            {
                var cancellationResponse = new CancellationPolicyResponse
                {
                    CancellationPolicy = activity.CancellationPolicy,
                    Amount = cancellationPrice.CancellationAmount,
                    FromDate = cancellationPrice.CancellationFromdate,
                    ToDate = cancellationPrice.CancellationToDate,
                    TravelDate = cancellationPrice.CancellationDateRelatedToOpreationDate
                };

                cancellationResponseList.Add(cancellationResponse);
            }
            return cancellationResponseList;
        }

        /// <summary>
        /// This operation is used to map service response to search data response model
        /// </summary>
        /// <param name="searchData"></param>
        /// <param name="AffiliateName"></param>
        /// <returns></returns>
        public List<SearchDetails> MapSearchData(SearchStack searchData, string AffiliateName = "")
        {
            var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 1.0));
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = processorCount
            };

            var searchDetailList = new List<SearchDetails>();
            if (searchData != null)
            {
                //var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(searchData.Activities, parallelOptions, activity =>
                {
                    var searchDetail = new SearchDetails
                    {
                        ProductType = activity.ProductType,
                        ActualServiceUrl = activity.ActualServiceUrl + "?utm_source=" + AffiliateName + "&utm_medium=Partnerships_API_Search",
                        ActivityType = activity.ActivityType,
                        DayBadge = activity.DayBadge,
                        BaseMinPrice = activity.BaseMinPrice,
                        GateBaseMinPrice = activity.GateBaseMinPrice,
                        ActivityId = activity.ID,
                        Images = activity.Images,
                        Name = activity.Name,
                        OfferPercentage = activity.OfferPercentage,
                        OverAllRating = activity.OverAllRating,
                        ScheduleOperates = activity.ScheduleOperates,
                        ShortIntroduction = activity.ShortIntroduction,
                        Length = activity.DurationString,
                        Themes = activity.CategoryIDs,
                        TotalReviews = activity.TotalReviews,
                        BulletPoints = activity.ReasonToBook,
                        Coordinates = activity.CoOrdinates,
                        Badges = activity.Badges,
                        CurrencyIsoCode = activity.CurrencyIsoCode
                    };
                    if (searchDetail != null)
                    {
                        searchDetailList.Add(searchDetail);
                    }
                });
            }

            searchDetailList.Where(x => x == null).ToList().ForEach(y => searchDetailList.Remove(y));
            return searchDetailList;
        }

        /// <summary>
        /// This operation is used to map service response to Activity detail response model
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="calendarResponse"></param>
        /// <returns></returns>
        public ActivityDetails MapActivityDetail(Activity activity, CalendarResponse calendarResponse = null)
        {
            if (activity != null)
            {
                return new ActivityDetails
                {
                    Id = activity.ID,
                    Name = activity.Name,
                    AdditionalInfo = activity.AdditionalInfo,
                    BaseMinPrice = (calendarResponse != null && calendarResponse.DatePriceAvailability.Count() > 0) ? calendarResponse.DatePriceAvailability.Min(x => x.BaseMinPrice) : activity.BaseMinPrice,
                    CancellationPolicy = activity.CancellationPolicy,
                    DurationString = activity.DurationString,
                    Exclusions = activity.Exclusions,
                    GateBaseMinPrice = (calendarResponse != null && calendarResponse.DatePriceAvailability.Count() > 0) ? calendarResponse.DatePriceAvailability.Min(x => x.GateBaseMinPrice) : activity.GateBaseMinPrice,
                    HotelPickUpLocation = activity.HotelPickUpLocation,
                    Images = activity.Images,
                    Inclusions = activity.Inclusions,
                    Introduction = activity.Introduction,
                    Itineraries = activity.Itineraries,
                    MeetingPointCoordinate = activity.MeetingPointCoordinate,
                    PleaseNote = activity.PleaseNote,
                    ReasonToBook = activity.ReasonToBook,
                    Reviews = activity.Reviews,
                    Schedule = activity.Schedule,
                    ScheduleOperates = activity.ScheduleOperates,
                    ScheduleReturnDetails = activity.ScheduleReturnDetails,
                    TotalReviews = activity.TotalReviews,
                    PassengerInfo = activity.PassengerInfo,
                    ComponentServices = GetComponentServices(activity.ProductOptions),
                    CurrencyIsoCode = (calendarResponse != null && !String.IsNullOrEmpty(calendarResponse?.CurrencyIsoCode)) ? calendarResponse?.CurrencyIsoCode : activity?.CurrencyIsoCode,
                    LineOfBusinessId = activity.LineOfBusinessId,
                    AttractionsCovered = activity.AttractionsCovered,
                    ToDoOnArrival = activity.ToDoOnArrival,
                    WhyDoThis = activity.WhyDoThis,
                    LiveOnDate = activity.LiveOnDate,
                    DurationDay = activity.DurationDay,
                    DurationTime = activity.DurationTime,
                    DurationAdditionText = activity.DurationAdditionText,
                    ContractQuestions = GetContractQuestions(activity?.APIContractQuestion, activity?.APIContractAnswer),
                    MinNoOfPassengers = activity.MinNoOfPax,
                    MaxNoOfPassengers = activity.MaxNoOfPax,
                    DownloadLinks = activity.DownloadLinks,
                    IsHideGatePrice = activity.IsHideGatePrice

                };
            }
            return null;
        }
        public NewsletterActivityDetails MapNewsletterActivityDetail(ActivityDetailsResponse activityDetailsResponse, Activity activity, decimal factor = 0, CalendarResponse calendarResponse = null)
        {
            if (activity != null)
            {
                return new NewsletterActivityDetails
                {

                    Id = activity.ID,
                    Name = activity.Name,
                    BaseMinPrice = factor * activityDetailsResponse.Activity.BaseMinPrice,
                    GateBaseMinPrice = factor * activityDetailsResponse.Activity.GateBaseMinPrice,
                    ActivityUrls = activity.ActualServiceUrl,
                    CurrencyIsoCode = (calendarResponse != null && !String.IsNullOrEmpty(calendarResponse?.CurrencyIsoCode)) ? calendarResponse?.CurrencyIsoCode : activity?.CurrencyIsoCode,
                    Images = activity.Images,
                    OverAllRating = activity.OverAllRating,
                    ReasonToBook = activity.ReasonToBook[0]

                };
            }
            return null;
        }

        /// <summary>
        /// This method map cache data to calendar response
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="calendarAvailabilities"></param>
        /// <param name="calendarWithDefaultAffiliateIdAvailabilities"></param>
        /// <param name="affiliate"></param>
        /// <param name="activityId"></param>
        /// <param name="clientInfo"></param>
        /// <param name="currencyIsoCode"></param>
        /// <returns></returns>
        /// <returns></returns>
        public object MapCalendarDetails(int activityId, string affiliateId, IEnumerable<CalendarAvailability> calendarAvailabilities, IEnumerable<CalendarAvailability> calendarWithDefaultAffiliateIdAvailabilities, Affiliate affiliate, ClientInfo clientInfo = null, string currencyIsoCode = null, string calendaravailability = null)
        {
            var isB2B = affiliate?.AffiliateConfiguration?.IsB2BAffiliate ?? false; ;
            var isB2BNetPriceAffiliate = affiliate?.AffiliateConfiguration?.IsB2BNetPriceAffiliate ?? false;
            var isSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;

            var b2bNetRateRule = _applicationService.GetB2BNetRateRuleAsync(affiliateId)?.GetAwaiter().GetResult();

            var netRatePercent = b2bNetRateRule?.NetRatePercent > 0
                                    ? b2bNetRateRule.NetRatePercent / 100
                                    : 0;

            var netPriceType = b2bNetRateRule?.NetPriceType;

            if (calendarAvailabilities?.Count() <= 0 && calendarWithDefaultAffiliateIdAvailabilities?.Count() <= 0)
                return null;
            var datePriceAvailability = new Dictionary<DateTime, string>();
            CalendarResponse_activity calendarResponse_activity = null;
            CalendarResponse calendarResponse = null;
            if (calendaravailability == "true")
            {
                var finalListDatePriceAvailability = new List<DatePriceAvailability_activity>();
                calendarResponse_activity = new CalendarResponse_activity
                {
                    ActivityId = activityId,
                    AffiliateId = affiliateId,
                    CurrencyIsoCode = !string.IsNullOrEmpty(currencyIsoCode) ? currencyIsoCode : (calendarAvailabilities?.FirstOrDefault()?.Currency ?? calendarWithDefaultAffiliateIdAvailabilities?.FirstOrDefault()?.Currency),
                    DatePriceAvailability = new List<DatePriceAvailability_activity>()
                };
            }
            else
            {
                var finalListDatePriceAvailability = new List<DatePriceAvailability>();
                calendarResponse = new CalendarResponse
                {
                    ActivityId = activityId,
                    AffiliateId = affiliateId,
                    CurrencyIsoCode = !string.IsNullOrEmpty(currencyIsoCode) ? currencyIsoCode : (calendarAvailabilities?.FirstOrDefault()?.Currency ?? calendarWithDefaultAffiliateIdAvailabilities?.FirstOrDefault()?.Currency),
                    DatePriceAvailability = new List<DatePriceAvailability>()
                };
            }


            foreach (var availability in calendarWithDefaultAffiliateIdAvailabilities)
            {
                var price = CalculateSellPrice(availability, isB2BNetPriceAffiliate, netPriceType, isSupplementOffer, netRatePercent, currencyIsoCode, clientInfo);
                var range = Enumerable.Range(0, 1 + availability.EndDate.Subtract(availability.StartDate).Days).Select(i => availability.StartDate.AddDays(i));
                foreach (var item in range)
                {
                    if (!datePriceAvailability.Keys.Contains(item))
                    {
                        datePriceAvailability.Add(item, price);
                    }
                }
            }

            foreach (var availability in calendarAvailabilities)
            {
                var price = CalculateSellPrice(availability, isB2BNetPriceAffiliate, netPriceType, isSupplementOffer, netRatePercent, currencyIsoCode, clientInfo);
                var range = Enumerable.Range(0, 1 + availability.EndDate.Subtract(availability.StartDate).Days).Select(i => availability.StartDate.AddDays(i));
                foreach (var item in range)
                {
                    if (datePriceAvailability.ContainsKey(item))
                    {
                        datePriceAvailability[item] = price;
                    }
                    else
                    {
                        datePriceAvailability.Add(item, price);
                    }
                }
            }

            var datePrice = datePriceAvailability.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value);
            foreach (var item in datePrice)
            {
                if (calendaravailability == "true")
                {
                    var finalDatePriceAvailability = new DatePriceAvailability_activity

                    {
                        DateTimeAvailability = item.Key.Date

                    };
                    calendarResponse_activity.DatePriceAvailability.Add(finalDatePriceAvailability);

                }
                else
                {
                    var finalDatePriceAvailability = new DatePriceAvailability
                    {
                        DateTimeAvailability = item.Key.Date,
                        BaseMinPrice = Math.Round(decimal.Parse(item.Value.Split('|')[0]), 2, MidpointRounding.AwayFromZero),
                        GateBaseMinPrice = Math.Round(decimal.Parse(item.Value.Split('|')[1]), 2, MidpointRounding.AwayFromZero)
                    };
                    calendarResponse.DatePriceAvailability.Add(finalDatePriceAvailability);
                }

            }
            if (calendaravailability == "true")
            {
                return calendarResponse_activity;

            }
            else
            {
                return calendarResponse;
            }
        }
        /// <summary>
        /// Calculate Sell Price
        /// </summary>
        /// <param name="availability"></param>
        /// <param name="isB2BNetPriceAffiliate"></param>
        /// <param name="netPriceType"></param>
        /// <param name="isSupplementOffer"></param>
        /// <param name="netRatePercent"></param>
        /// <param name="targetCurrency"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private string CalculateSellPrice(CalendarAvailability availability, bool isB2BNetPriceAffiliate, int? netPriceType, bool isSupplementOffer, decimal? netRatePercent, string targetCurrency = null, ClientInfo clientInfo = null)
        {
            var costPrice = availability.CostPrice;
            var basePrice = availability.B2BBasePrice;
            var sellPrice = availability.B2CBasePrice;
            var sellPriceComputed = availability.B2CBasePrice;

            if (isB2BNetPriceAffiliate)
            {
                if (netPriceType == 2)
                {
                    sellPriceComputed = (costPrice) * (100 / (100 - (netRatePercent * 100))) ?? availability.B2CBasePrice;
                }
                else if (isSupplementOffer && netPriceType == 1)
                {
                    sellPriceComputed = (costPrice + ((sellPrice - costPrice) * netRatePercent)) ?? availability.B2CBasePrice;
                }
                else if (netPriceType == 1)
                {
                    sellPriceComputed = (costPrice + ((basePrice - costPrice) * netRatePercent)) ?? availability.B2CBasePrice;
                }
            }
            else if (isSupplementOffer)
            {
                sellPriceComputed = availability.B2CBasePrice;
            }
            else
            {
                sellPriceComputed = availability.B2BBasePrice;
            }

            if (!string.IsNullOrEmpty(targetCurrency))
            {
                #region update Prices as per  customer currency

                basePrice = _activityHelper.GetContextPrice(basePrice, availability.Currency, clientInfo, 2, targetCurrency);
                sellPrice = _activityHelper.GetContextPrice(sellPriceComputed, availability.Currency, clientInfo, 2, targetCurrency);
                costPrice = _activityHelper.GetContextPrice(costPrice, availability.Currency, clientInfo, 2, targetCurrency);
                return sellPrice + "|" + basePrice;

                #endregion update Prices as per  customer currency
            }
            else
            {
                return sellPriceComputed + "|" + basePrice;
            }
        }

        /// <summary>
        /// This method maps activity and calender data
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="activityDetailsWithCalendar"></param>
        /// <param name="calendarWithDefaultAffiliateIdAvailabilityList"></param>
        /// <param name="affiliate"></param>
        /// <param name="clientInfo"></param>
        /// <param name="currencyIsoCode"></param>
        /// <returns></returns>
        public ActivityDetailsResponse MapActivityWithCalendarAvailability(string affiliateId, ActivityDetailsWithCalendarResponse activityDetailsWithCalendar, IEnumerable<CalendarAvailability> calendarWithDefaultAffiliateIdAvailabilityList, Affiliate affiliate, ClientInfo clientInfo, string currencyIsoCode)
        {
            var calendarResponse = (CalendarResponse)MapCalendarDetails(activityDetailsWithCalendar.Activity.ID, affiliateId, activityDetailsWithCalendar.CalendarAvailabilityList, calendarWithDefaultAffiliateIdAvailabilityList, affiliate, clientInfo, currencyIsoCode);
            var activity = MapActivityDetail(activityDetailsWithCalendar.Activity, calendarResponse);

            var activityWithAvailability = new ActivityDetailsResponse
            {
                Activity = activity,
                CalendarAvailability = calendarResponse
            };

            return activityWithAvailability;
        }

        /// <summary>
        /// This method maps the Check Availability Response
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="tokenId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public CheckAvailabilityResponse MapCheckAvailabilityResponse(Activity activity, Criteria criteria, string tokenId)
        {
            if (activity == null)
            {
                UpdateError(activity, criteria, Util.ErrorCodes.AVAILABILITY_ERROR, Util.ErrorMessages.ACTIVITY_NOT_FOUND);
            }
            if (!(activity.ProductOptions?.Count > 0))
            {
                UpdateError(activity, criteria, Util.ErrorCodes.AVAILABILITY_ERROR, Util.ErrorMessages.ACTIVITY_OPTION_NOT_FOUND);
            }
            foreach (var option in activity.ProductOptions)
            {
                option.BundleOptionName = activity.Name;
            }
            var responseModel = new CheckAvailabilityResponse
            {
                ActivityId = activity.ID,
                Name = activity.Name,
                Description = activity.MetaDescription,
                TokenId = tokenId,
                Options = activity.ProductOptions?.Count > 0 ? MapOptionData(activity.ProductOptions, criteria) : null,
                IsPaxDetailRequired = activity.IsPaxDetailRequired,
                Errors = activity.Errors
            };
            //if (activity.ApiType == APIType.Tiqets )
            //{
            responseModel.IsPaxDetailRequiredDuringReservation = true;
            //}

            return responseModel;
        }

        /// <summary>
        /// Map Check Bundle Availability Response
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteriaForActivity"></param>
        /// <param name="tokenId"></param>
        /// <param name="exchangeRateValues"></param>
        /// <returns></returns>
        public CheckBundleAvailabilityResponse MapCheckBundleAvailabilityResponse(Activity activity, Dictionary<int, Criteria> criteriaForActivity, string tokenId,
            Dictionary<string, decimal> exchangeRateValues)
        {
            var mapBundleOptions = MapBundleOptionData(activity, criteriaForActivity, exchangeRateValues);
            if (!(mapBundleOptions?.Count > 0))
            {
                mapBundleOptions = null;
            }
            var responseModel = new CheckBundleAvailabilityResponse
            {
                ActivityId = activity.ID,
                Name = activity.Name,
                Description = activity.MetaDescription,
                TokenId = tokenId,
                BundleOptions = mapBundleOptions,
                IsPaxDetailRequired = activity.IsPaxDetailRequired
            };

            return responseModel;
        }

        public CheckAvailabilityResponse MapBundleToAvailabilityResponse(CheckBundleAvailabilityResponse bundleresponse, CheckAvailabilityResponse responseModel)
        {
            responseModel = new CheckAvailabilityResponse
            {
                ActivityId = bundleresponse.ActivityId,
                Description = bundleresponse.Description,
                Errors = responseModel.Errors,
                IsPaxDetailRequired = bundleresponse.IsPaxDetailRequired,
                Name = bundleresponse.Name,
                TokenId = bundleresponse.TokenId,
                Options = new List<Option>(),
                IsBundle = true,
            };

            var optionOrder = 0;
            if (bundleresponse?.BundleOptions?.Count > 0)
            {
                foreach (var bo in bundleresponse.BundleOptions)
                {
                    foreach (var option in bo.ComponentOptions)
                    {
                        optionOrder++;
                        option.OptionOrder = optionOrder;
                        option.BundleDetails = new BundleDetails
                        {
                            BasePrice = bo.BasePrice,
                            BundleOptionID = bo.Id,
                            BundleOptionName = bo.BundleOptionName,
                            BundleOptionOrder = bo.BundleOptionOrder,
                            BundleOptionReferenceIds = bo.BundleOptionReferenceIds,
                            CurrencyIsoCode = bo.CurrencyIsoCode,
                            GateBasePrice = bo.GateBasePrice,
                            ComponentServiceID = option.ComponentServiceID,
                            EndTime = option.EndTime,
                            StartTime = option.StartTime,
                            Variant = option.Variant
                        };
                        responseModel.Options.Add(option);
                    }
                }
            }
            else
            {
                responseModel.Errors = new List<Error>();
                responseModel.Errors.Add(new Error
                {
                    Code = Util.ErrorCodes.BUNDLE_OPTION_NOT_FOUND,
                    HttpStatus = System.Net.HttpStatusCode.NotFound,
                    Message = Util.ErrorMessages.BUNDLE_OPTION_NOT_FOUND,
                });
            }

            return responseModel;
        }

        #region Private Methods

        private List<BundleOption> MapBundleOptionData(Activity activity, Dictionary<int, Criteria> criteriaForActivity, Dictionary<string, decimal> exchangeRateValues)
        {
            var groupedBundleOptions = activity.ProductOptions.GroupBy(x => x.BundleOptionID);

            var timeOption = activity.ProductOptions.Where(x => x.StartTime.ToString(@"hh\:mm\:ss") != "00:00:00").ToList();
            var nonTimeOption = activity.ProductOptions.Where(x => x.StartTime.ToString(@"hh\:mm\:ss") == "00:00:00").ToList();
            var bundleOptions = new List<BundleOption>();
            int order = 0;
            if (timeOption?.Any() == true && nonTimeOption?.Any() == true)
            {
                foreach (var t in timeOption)
                {
                    var bo = new List<ProductOption>();

                    var ntOptions = nonTimeOption.Where(x => x.BundleOptionID == t.BundleOptionID
                        && x.ServiceOptionId != t.ServiceOptionId
                    ).ToList();

                    if (ntOptions.Count > 0)
                    {
                        bo.Add(t);
                        bo.AddRange(ntOptions);
                        var componentOptions = MapComponentOption(bo, criteriaForActivity);
                        if (componentOptions?.Any() == false)
                        {
                            continue;
                        }
                        order++;

                        var bundleOption = new BundleOption
                        {
                            Id = t.BundleOptionID,
                            BundleOptionName = bo.FirstOrDefault()?.BundleOptionName,
                            BasePrice = CalculateTotalBasePrice(bo.Where(e => e.BundleOptionID == t.BundleOptionID)?.ToList(), exchangeRateValues),
                            GateBasePrice = CalculateTotalGateBasePrice(bo.Where(e => e.BundleOptionID == t.BundleOptionID)?.ToList(), exchangeRateValues),
                            ComponentOptions = componentOptions?.OrderBy(e => e.OptionOrder).ToList(),
                            BundleOptionOrder = order,
                            BundleOptionReferenceIds = GetComponentOptionReferenceIds(componentOptions),
                            CurrencyIsoCode = componentOptions?.FirstOrDefault()?.BasePrice?.CurrencyIsoCode,
                            IsCapacityCheckRequired = bo.Any(x => x.IsCapacityCheckRequired),
                        };

                        //if any option has IsCapacityCheckRequired = true
                        if (bo.Any(x => x.IsCapacityCheckRequired))
                        {
                            if (bo.All(x => x.IsCapacityCheckRequired))
                            {
                                bundleOption.IsCapacityCheckRequired = true;
                                bundleOption.Capacity = bo.Min(x => x.AllocationCapacity);
                            }
                            else
                            {
                                var checkCapacityTrueOption = bo.OrderBy(e => e.AllocationCapacity).FirstOrDefault(x => x.IsCapacityCheckRequired);
                                if (checkCapacityTrueOption != null)
                                {
                                    bundleOption.IsCapacityCheckRequired = checkCapacityTrueOption.IsCapacityCheckRequired;
                                    bundleOption.Capacity = checkCapacityTrueOption.AllocationCapacity;
                                }
                            }
                        }
                        else
                        {
                            bundleOption.Capacity = bo.Min(e => e.AllocationCapacity);
                            bundleOption.IsCapacityCheckRequired = false;
                        }

                        bundleOptions.Add(bundleOption);
                        bundleOptions = bundleOptions
                                        .OrderBy(e => e.BundleOptionOrder)
                                        .ThenBy(e => e.StartTime)
                                        .ThenBy(e => e.BasePrice)
                                        .ThenBy(e => e.Id).ToList();
                    }
                }
            }
            else
            {
                foreach (var groupedBundleOption in groupedBundleOptions)
                {
                    var options = groupedBundleOption.Select(x => x).ToList();

                    var componentOptions = MapComponentOption(options, criteriaForActivity);
                    if (componentOptions?.Any() == false)
                    {
                        continue;
                    }
                    order++;

                    var bundleOption = new BundleOption
                    {
                        Id = groupedBundleOption.Key,
                        BundleOptionName = options.FirstOrDefault()?.BundleOptionName,
                        BasePrice = CalculateTotalBasePrice(activity.ProductOptions.Where(e => e.BundleOptionID == groupedBundleOption.Key)?.ToList(), exchangeRateValues),
                        GateBasePrice = CalculateTotalGateBasePrice(activity.ProductOptions.Where(e => e.BundleOptionID == groupedBundleOption.Key)?.ToList(), exchangeRateValues),
                        ComponentOptions = componentOptions?.OrderBy(e => e.OptionOrder).ToList(),
                        BundleOptionOrder = order,
                        BundleOptionReferenceIds = GetComponentOptionReferenceIds(componentOptions),
                        CurrencyIsoCode = activity.CurrencyIsoCode,
                        IsCapacityCheckRequired = options.Any(x => x.IsCapacityCheckRequired),
                    };

                    //if any option has IsCapacityCheckRequired = true
                    if (options.Any(x => x.IsCapacityCheckRequired))
                    {
                        if (options.All(x => x.IsCapacityCheckRequired))
                        {
                            bundleOption.IsCapacityCheckRequired = true;
                            bundleOption.Capacity = options.Min(x => x.AllocationCapacity);
                        }
                        else
                        {
                            var checkCapacityTrueOption = options.OrderBy(e => e.AllocationCapacity).FirstOrDefault(x => x.IsCapacityCheckRequired);
                            if (checkCapacityTrueOption != null)
                            {
                                bundleOption.IsCapacityCheckRequired = checkCapacityTrueOption.IsCapacityCheckRequired;
                                bundleOption.Capacity = checkCapacityTrueOption.AllocationCapacity;
                            }
                        }
                    }
                    else
                    {
                        bundleOption.Capacity = options.Min(e => e.AllocationCapacity);
                        bundleOption.IsCapacityCheckRequired = false;
                    }

                    bundleOptions.Add(bundleOption);
                    bundleOptions = bundleOptions.OrderBy(e => e.BundleOptionOrder).ThenBy(e => e.BasePrice).ThenBy(e => e.Id).ToList();
                }
            }

            return bundleOptions;
        }

        private decimal CalculateTotalGateBasePrice(List<ProductOption> productOptions, Dictionary<string, decimal> exchangeRateValues)
        {
            decimal totalGateBasePrice = 0;
            foreach (var option in productOptions)
            {
                var optionExchangeRate = exchangeRateValues[option.GateBasePrice.Currency.IsoCode];
                totalGateBasePrice += option.GateBasePrice.Amount * optionExchangeRate;
            }
            return totalGateBasePrice;
        }

        private decimal CalculateTotalBasePrice(List<ProductOption> productOptions, Dictionary<string, decimal> exchangeRateValues)
        {
            decimal basePrice = 0;
            foreach (var option in productOptions)
            {
                var optionExchangeRate = exchangeRateValues[option.BasePrice.Currency.IsoCode];
                basePrice += option.BasePrice.Amount * optionExchangeRate;
            }
            return basePrice;
        }

        private List<Option> MapComponentOption(List<ProductOption> productOptions, Dictionary<int, Criteria> criteriaForActivity)
        {
            var componentOptions = new List<Option>();
            foreach (var option in productOptions)
            {
                try
                {
                    var criteria = criteriaForActivity[option.ComponentServiceID];

                    var optionId = $"{Convert.ToString(option.ServiceOptionId)}|{Convert.ToString(option.BundleOptionID)}";

                    var componentOption = new Option
                    {
                        Id = option.Id,
                        BundleOptionID = option.BundleOptionID,
                        ServiceOptionId = option.ServiceOptionId,
                        OptionName = option.Name,
                        AvailabilityStatus = option.AvailabilityStatus.ToString(),
                        BasePrice = CreatePrice(option, criteria.NoOfPassengers, 0),
                        GateBasePrice = CreatePrice(option, criteria.NoOfPassengers, 1),
                        OptionOrder = option.ComponentOrder,
                        ComponentServiceID = option.ComponentServiceID,
                        StartTime = option.StartTime,
                        Variant = option.Variant,
                        CancellationPolicy = option.CancellationText,
                        Capacity = option.Capacity,
                        EndTime = option.EndTime,
                        IsCapacityCheckRequired = option.IsCapacityCheckRequired,
                        PickupOptionType = option.PickUpOption
                    };

                    var isSameDateOtherOption = productOptions.Any(x => x.BundleOptionID == option.BundleOptionID
                    && x.ServiceOptionId != option.ServiceOptionId
                    && x.BasePrice.DatePriceAndAvailabilty.ContainsKey(criteria.CheckinDate)
                    && option.BasePrice.DatePriceAndAvailabilty.ContainsKey(criteria.CheckinDate)
                    && x.BasePrice.DatePriceAndAvailabilty[criteria.CheckinDate].AvailabilityStatus ==
                        option.BasePrice.DatePriceAndAvailabilty[criteria.CheckinDate].AvailabilityStatus
                    );

                    if (isSameDateOtherOption)
                    {
                        componentOptions.Add(componentOption);
                    }
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return componentOptions;
        }

        private string GetComponentOptionReferenceIds(List<Option> componentOptions)
        {
            var componentOptionReferenceIds = "";
            var checkIndate = componentOptions.FirstOrDefault().BasePrice.PriceAndAvailabilities.FirstOrDefault().DateAndTime;
            componentOptions = componentOptions.OrderBy(x => x.OptionOrder).ToList();
            foreach (var componentOption in componentOptions)
            {
                var refid = $"{componentOption.BasePrice.PriceAndAvailabilities.FirstOrDefault(x => x.DateAndTime == checkIndate)?.ReferenceId ?? string.Empty}";
                if (!string.IsNullOrWhiteSpace(refid))
                {
                    componentOptionReferenceIds += $"|";
                }
            }
            componentOptionReferenceIds = componentOptionReferenceIds.Substring(0, componentOptionReferenceIds.Length - 1);
            return componentOptionReferenceIds;
        }

        private List<Option> MapOptionData(List<ProductOption> productOptions, Criteria criteria)
        {
            var options = new List<Option>();
            var optionOrderCounter = 1;

            var productOptionsQuery = from po in productOptions
                                      from pna in
                                                  po?.BasePrice?.DatePriceAndAvailabilty ??
                                                  po?.CostPrice?.DatePriceAndAvailabilty ??
                                                  po?.GateBasePrice?.DatePriceAndAvailabilty ??
                                                  po?.GateSellPrice?.DatePriceAndAvailabilty
                                          //where pna.Key >= criteria.CheckinDate
                                          //&& pna.Key <= criteria.CheckoutDate
                                          //commented to support multi-day support for availability call
                                      select po;

            productOptions = productOptionsQuery?.Distinct()?.ToList();

            foreach (var productOption in productOptions)
            {
                var option = new Option
                {
                    Id = productOption.Id,
                    BundleOptionID = productOption.BundleOptionID,
                    OptionName = productOption.Name?.Trim(),
                    OptionOrder = productOption.OptionOrder > 0 ? productOption.OptionOrder : optionOrderCounter,
                    Description = productOption.Description,
                    BasePrice = CreatePrice(productOption, criteria.NoOfPassengers, 0),
                    GateBasePrice = CreatePrice(productOption, criteria.NoOfPassengers, 1),
                    IsCapacityCheckRequired = productOption.IsCapacityCheckRequired,
                    Capacity = productOption.AllocationCapacity,
                    Variant = productOption.Variant,
                    StartTime = productOption.StartTime,
                    EndTime = productOption.EndTime,
                    ServiceOptionId = productOption.ServiceOptionId,
                    CancellationPolicy = productOption.CancellationText,
                    PickupOptionType = productOption.PickUpOption,
                };
                if (productOption.BundleOptionID > 0)
                {
                    option.BundleDetails = new BundleDetails
                    {
                        BundleOptionID = productOption.BundleOptionID,
                        ComponentServiceID = productOption.ComponentServiceID,
                        BundleOptionName = productOption.BundleOptionName,
                    };
                    productOption.BundleOptionName = string.Empty;
                }
                options.Add(option);
                optionOrderCounter++;
            }
            return options.OrderBy(e => e.OptionOrder).ThenBy(e => e.BasePrice.PriceAndAvailabilities.FirstOrDefault().TotalPrice).ThenBy(e => e.Id).ToList();
        }

        private Models.ResponseModels.CheckAvailability.Price CreatePrice(ProductOption option, Dictionary<PassengerType, int> noOfPassengers, int priceType)
        {
            var price = new Models.ResponseModels.CheckAvailability.Price
            {
                PriceAndAvailabilities = new List<PriceAndAvailability>()
            };

            switch (priceType)
            {
                case 0: // Base Price
                    if (option.BasePrice != null)
                    {
                        price.CurrencyIsoCode = option.BasePrice.Currency?.IsoCode;
                        if (option.BasePrice.DatePriceAndAvailabilty != null)
                        {
                            foreach (var item in option.BasePrice.DatePriceAndAvailabilty)
                            {
                                price.PriceAndAvailabilities.Add(MapPriceAndAvailability(item, noOfPassengers));
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;

                case 1:// Gate Base Price
                    if (option.GateBasePrice != null)
                    {
                        price.CurrencyIsoCode = option.GateBasePrice.Currency?.IsoCode;
                        if (option.GateBasePrice.DatePriceAndAvailabilty != null)
                        {
                            foreach (var item in option.GateBasePrice.DatePriceAndAvailabilty)
                            {
                                price.PriceAndAvailabilities.Add(MapPriceAndAvailability(item, noOfPassengers));
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;
            }
            return price;
        }

        private PriceAndAvailability MapPriceAndAvailability(KeyValuePair<DateTime, Isango.Entities.PriceAndAvailability> datePriceAndAvailabilty, Dictionary<PassengerType, int> noOfPassengers)
        {
            if (datePriceAndAvailabilty.Value.PricingUnits == null)
                return null;

            if (datePriceAndAvailabilty.Value.PricingUnits.Count == 0)
                return null;

            var unitType = datePriceAndAvailabilty.Value.PricingUnits.FirstOrDefault()?.UnitType;
            var unitQty = unitType != UnitType.PerUnit ?
                        datePriceAndAvailabilty.Value.PricingUnits.First().Quantity :
                        (datePriceAndAvailabilty.Value.UnitQuantity > 0 ?
                            datePriceAndAvailabilty.Value.UnitQuantity : 1);

            var priceAndAvailabilities = new PriceAndAvailability
            {
                ReferenceId = datePriceAndAvailabilty.Value.ReferenceId?.ToString(),
                AvailabilityStatus = datePriceAndAvailabilty.Value.AvailabilityStatus.ToString(),
                DateAndTime = datePriceAndAvailabilty.Key,
                UnitType = EnumNameResolver.GetNameFromEnum(typeof(UnitType), unitType),
                Quantity = unitQty,

                IsCapacityCheckRequired = datePriceAndAvailabilty.Value.IsCapacityCheckRequired,
                Capacity = datePriceAndAvailabilty.Value.Capacity,
                PricingUnits = new List<PricingUnit>()
            };

            foreach (var item in datePriceAndAvailabilty.Value.PricingUnits)
            {
                /*
                setting adult as default when PassengerType not available i.e in case ArgumentOutOfRangeException unit type in hotelbeds its adult
                */
                var passengerType = PassengerType.Adult;
                var perPersonPricingUnit = (item as PerPersonPricingUnit);
                //if (item is PerUnitPricingUnit)
                //{
                //    passengerType = PassengerType.Adult;
                //}
                //else
                //{
                passengerType = perPersonPricingUnit?.PassengerType ?? PassengerType.Adult;
                //}

                if (!noOfPassengers.ContainsKey(passengerType)) continue;

                var paxCount = noOfPassengers[passengerType];
                var pricingUnit = new PricingUnit
                {
                    PassengerTypeName = EnumNameResolver.GetNameFromEnum(typeof(PassengerType), passengerType),
                    PassengerTypeId = passengerType,
                    //PassengerTypeId = (int)passengerType,
                    Price = item.Price,
                    Count = paxCount,
                    MinimumSellingPrice = perPersonPricingUnit.MinimumSellingPrice,
                    Currency = perPersonPricingUnit.Currency,
                    IsMinimumSellingPriceRestrictionApplicable = perPersonPricingUnit.IsMinimumSellingPriceRestrictionApplicable
                };

                priceAndAvailabilities.PricingUnits.Add(pricingUnit);
            }

            priceAndAvailabilities.TotalPrice = datePriceAndAvailabilty.Value.TotalPrice;

            return priceAndAvailabilities;
        }

        private List<ComponentService> GetComponentServices(List<ProductOption> productOptions)
        {
            var componentServices = new List<ComponentService>();
            var componentServiceIds = productOptions.Where(x => x.ComponentServiceID > 0).Select(x => x.ComponentServiceID).Distinct();

            foreach (var componentServiceId in componentServiceIds)
            {
                var activity = _activityService.GetActivityById(componentServiceId, DateTime.Today, Constant.EN)?.GetAwaiter().GetResult();
                if (activity == null) continue;

                var componentService = new ComponentService
                {
                    Id = activity.ID,
                    Name = activity.Name
                };
                componentServices.Add(componentService);
            }
            return componentServices;
        }

        /// <summary>
        /// Get Contract Questions
        /// </summary>
        /// <param name="apiContractQuestions"></param>
        /// <param name="apiContractAnswers"></param>
        /// <returns></returns>
        private List<ContractQuestions> GetContractQuestions(List<APIContractQuestion> apiContractQuestions, List<APIContractAnswers> apiContractAnswers)
        {
            var contractQuestions = new List<ContractQuestions>();
            var apiContractQuestionsGroupBy = apiContractQuestions?.GroupBy(o => o.ServiceOptionid);
            if (apiContractQuestionsGroupBy != null && apiContractQuestionsGroupBy.Count() > 0)
            {
                foreach (var apiQuestionGroup in apiContractQuestionsGroupBy)
                {
                    if (apiQuestionGroup.FirstOrDefault().Status == true)
                    {
                        var contractQuestion = new ContractQuestions
                        {
                            ServiceOptionId = apiQuestionGroup.FirstOrDefault().ServiceOptionid,
                            Questions = new List<Questions>()
                        };
                        var QuestionList = new List<Questions>();
                        if (apiQuestionGroup != null && apiQuestionGroup.Count() > 0)
                        {
                            foreach (var apiQuestion in apiQuestionGroup)
                            {
                                var question = new Questions
                                {
                                    QuestionId = apiQuestion?.QuestionId,
                                    Label = apiQuestion?.Label,
                                    Required = apiQuestion?.Required,
                                    SelectFromOptions = apiQuestion?.SelectFromOptions,
                                    Assignedservicequestionid = apiQuestion.Assignedservicequestionid,
                                    ServiceOptionQuestionStatus = apiQuestion.ServiceOptionQuestionStatus,
                                    Answers = new List<Answers>()
                                };
                                //Answers
                                if (apiContractAnswers != null && apiContractAnswers.Count() > 0)
                                {
                                    var selectedAnswers = apiContractAnswers.Where
                                        (x => x?.Serviceid == apiQuestion?.Serviceid
                                        && x?.ServiceOptionid == apiQuestion?.ServiceOptionid
                                        && x.QuestionId == apiQuestion.QuestionId
                                         && x.AnswerStatus == true
                                        )?.ToList();
                                    var answerList = new List<Answers>();
                                    if (selectedAnswers != null && selectedAnswers.Count() > 0)
                                    {
                                        foreach (var item in selectedAnswers)
                                        {
                                            var answer = new Answers
                                            {
                                                Label = item.Label,
                                                Value = item.Value
                                            };
                                            answerList.Add(answer);
                                        }
                                    }
                                    question.Answers = answerList;
                                }

                                QuestionList.Add(question);
                            }
                        }
                        contractQuestion.Questions = QuestionList;
                        contractQuestions.Add(contractQuestion);
                    }
                }
            }
            return contractQuestions;
        }

        internal PersonTypesResponseModelV1 GetPersonTypeOptionCacheAvailability(AvailablePersonTypes personTypes, Activity activity)
        {
            try
            {
                var personTypesResponseModel = new PersonTypesResponseModelV1
                {
                    bookablePersonTypesInDateRanges = new Bookablepersontypesindateranges
                    {
                        AvailableDates = personTypes?.AvailableDates?.Select(x => new Availabledate
                        {
                            AvailableOn = x.AvailableOn,
                            Capacity = x.Capacity
                        })?.ToList(),

                        AvailablePersonTypes = new List<Availablepersontype>()
                    }
                };
                var availablePersonTypes = personTypes.AvailablePassengerTypes;

                var GroupByMultipleKeysMS = availablePersonTypes
                            .GroupBy(x => new { x.FromDate, x.ToDate })
                            .OrderBy(g => g.Key.FromDate).ThenBy(g => g.Key.ToDate)
                            .Select(g => new Availablepersontype
                            {
                                fromDate = g.Key.FromDate,
                                toDate = g.Key.ToDate,
                                personTypes = g.OrderBy(x => x.PassengerTypeID)
                                            .Where(z => z.PassengerTypeID > 0)
                                            .Select(y => new Persontype
                                            {
                                                name = ((PassengerType)y.PassengerTypeID).ToString(),
                                                maxAge = y.ToAge > 99 ? 99 : y.ToAge,
                                                minAge = y.FromAge < 0 ? 0 : y.FromAge,
                                                personTypeId = y.PassengerTypeID
                                            })?.ToList()
                            });

                personTypesResponseModel.bookablePersonTypesInDateRanges.AvailablePersonTypes = GroupByMultipleKeysMS?.ToList();

                /*
                var paxInfos = activity.PassengerInfo;

                var paxtypes = paxInfos.Select(x =>
                    new Persontype
                    {
                        name = ((PassengerType)x.PassengerTypeId).ToString(),
                        maxAge = x.ToAge > 99 ? 99 : x.ToAge,
                        minAge = x.FromAge
                    }
                )?.ToList();
                */

                return personTypesResponseModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Private Methods

        #region [Delta Activity]

        /// <summary>
        /// This operation is used to map service response to passenger info data response model
        /// </summary>
        /// <param name="passengerInfoData"></param>
        /// <returns></returns>
        public List<PassengerInfoResponse> MapPassengerInfoData(List<Isango.Entities.Booking.PassengerInfo> passengerInfoData)
        {
            var passengerInfoList = new List<PassengerInfoResponse>();
            if (passengerInfoData != null)
            {
                foreach (var passengerinfoItem in passengerInfoData)
                {
                    var passengerInfo = new PassengerInfoResponse
                    {
                        ServiceID = passengerinfoItem.ActivityId,
                        PassengerTypeID = passengerinfoItem.PassengerTypeId,
                        FromAge = passengerinfoItem.FromAge,
                        ToAge = passengerinfoItem.ToAge,
                        MinSize = passengerinfoItem.MinSize,
                        MaxSize = passengerinfoItem.MaxSize,
                        PaxDesc = String.IsNullOrEmpty(passengerinfoItem.PaxDesc) ? string.Empty : passengerinfoItem.PaxDesc.Trim(),
                        IsIndependablePax = passengerinfoItem.IndependablePax,
                        Label = passengerinfoItem.Label,
                        MeasurementDesc = passengerinfoItem.MeasurementDesc
                    };
                    passengerInfoList.Add(passengerInfo);
                }
            }
            return passengerInfoList;
        }

        /// <summary>
        /// This operation is used to map service response to review data response model
        /// </summary>
        /// <param name="reviewData"></param>
        /// <returns></returns>
        public List<ReviewResponse> MapReviewData(List<Review> reviewData)
        {
            var reviewResponseList = new List<ReviewResponse>();
            if (reviewData != null)
            {
                foreach (var reviewtem in reviewData)
                {
                    var review = new ReviewResponse
                    {
                        Title = reviewtem.Title,
                        Rating = reviewtem.Rating,
                        Text = reviewtem.Text,
                        UserName = reviewtem.UserName,
                        Country = reviewtem.Country,
                        ServiceId = reviewtem.ServiceId,
                        SubmittedDate = reviewtem.SubmittedDate,
                        IsFeefo = reviewtem.IsFeefo
                    };
                    reviewResponseList.Add(review);
                }
            }
            return reviewResponseList;
        }

        /// <summary>
        /// This operation is used to map activity response to activity data response model
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="languageCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActivityResponse MapActivityData(Activity activity, string languageCode, bool status)
        {
            if (activity != null)
            {
                return new ActivityResponse
                {
                    ProductType = activity.ProductType,
                    ActivityType = activity.ActivityType,
                    BaseMinPrice = activity.BaseMinPrice,
                    Name = activity.Name,
                    OverAllRating = activity.OverAllRating,
                    ScheduleOperates = activity.ScheduleOperates,
                    ShortIntroduction = activity.ShortIntroduction,
                    Images = activity.Images,
                    TotalReviews = activity.TotalReviews,
                    ActualServiceURL = activity.ActualServiceUrl,
                    AdditionalInfo = activity.AdditionalInfo,
                    AlertNote = activity.AlertNote,
                    APIType = activity.ApiType,
                    Badges = activity.Badges,
                    BookingWindow = activity.BookingWindow,
                    CancellationPolicy = activity.CancellationPolicy,
                    CanonicalURL = activity.CanonicalURL,
                    CategoryIDs = activity.CategoryIDs,
                    CategoryTypes = activity.CategoryTypes,
                    ChildPolicy = activity.ChildPolicy,
                    Code = activity.Code,
                    CoOrdinates = activity.CoOrdinates,
                    CurrencyCode = activity.CurrencyIsoCode,
                    CustomerPrototypes = activity.CustomerPrototypes,
                    DoDont = activity.DoDont,
                    Duration = activity.Duration,
                    Exclusions = activity.Exclusions,
                    FactsheetID = activity.FactsheetId,
                    HotelPickUpLocation = activity.HotelPickUpLocation,
                    ID = activity.ID,
                    Inclusions = activity.Inclusions,
                    Itineraries = activity.Itineraries,
                    Introduction = activity.Introduction,
                    IsPackage = activity.IsPackage,
                    IsPaxDetailRequired = activity.IsPaxDetailRequired,
                    IsReceipt = activity.IsReceipt,
                    IsServiceLevelPickUp = activity.IsServiceLevelPickUp,
                    MeetingPointCoordinate = activity.MeetingPointCoordinate,
                    MetaDescription = activity.MetaDescription,
                    MetaKeywords = activity.MetaKeywords,
                    OfferPercentage = activity.OfferPercentage,
                    OnSale = activity.OnSale,
                    PleaseNote = activity.PleaseNote,
                    Priority = activity.Priority,
                    PriorityWiseCategory = activity.PriorityWiseCategory,
                    ReasonToBook = activity.ReasonToBook,
                    Schedule = activity.Schedule,
                    ScheduleLocation = activity.ScheduleLocation,
                    ScheduleReturnDetails = activity.ScheduleReturnDetails,
                    ScheduleUnavailableDates = activity.ScheduleUnavailableDates,
                    SellMinPrice = activity.SellMinPrice,
                    ShortName = activity.ShortName,
                    Title = activity.Title,
                    YouTubeLink = activity.YouTubeLink,
                    LanguageCode = languageCode,
                    Status = status,
                    Bundle = (activity.ActivityType == ActivityType.Bundle) ? ActivityBundleBind(activity.ProductOptions) : new List<BundleResponse>(),
                    Regions = activity.Regions,
                    Margin = activity.Margin,
                    PassengerInfo = null,
                    AllOptions = null,
                    Availability = string.Empty,
                    DurationString = activity.DurationString,
                    PickUpOption = activity.PickUpOption,
                    WhereYouStay = activity.WhereYouStay,
                    WhyYouLove = activity.WhyYouLove,
                    DownloadLinks = activity.DownloadLinks,
                    ActivityOffers = activity.ActivityOffers,
                    IsNoIndex = activity.IsNoIndex != null ? activity.IsNoIndex : true,
                    IsFollow = activity.IsFollow != null ? activity.IsFollow : true,
                    IsHighDefinationImages = activity.IsHighDefinationImages == null ? false : activity.IsHighDefinationImages,
                    IsGoogleFeed = activity.IsGoogleFeed,
                    Ratings = ActivityRatingsBind(activity.Ratings),
                    TimesOfDaysOptionWise = ActivityTimeofDayBind(activity.ProductOptions),
                    AttractionsCovered = activity.AttractionsCovered,
                    DurationDay = activity.DurationDay,
                    DurationTime = activity.DurationTime,
                    DurationAdditionText = activity.DurationAdditionText,
                    DurationSummary = !String.IsNullOrEmpty(activity.DurationSummary) ? activity.DurationSummary : activity.DurationString,
                    LineOfBusinessId = activity.LineOfBusinessId,
                    LiveOnDate = activity.LiveOnDate == null ? DateTime.MinValue : activity.LiveOnDate,
                    ToDoOnArrival = activity.ToDoOnArrival,
                    WhyDoThis = activity.WhyDoThis,
                    TourLaunchDate = activity.TourLaunchDate == null ? DateTime.MinValue : activity.TourLaunchDate,
                    IsTimeBase = activity.IsTimeBase,
                    CancellationSummary = !String.IsNullOrEmpty(activity.CancellationSummary) ? activity.CancellationSummary : activity.CancellationPolicy,
                    StartTimeSummary = !String.IsNullOrEmpty(activity.StartTimeSummary) ? activity.StartTimeSummary : activity.Schedule,
                    SupplierID = activity.SupplierID == null ? 0 : activity.SupplierID
                };
            }
            else
            {
                return new ActivityResponse();
            }
        }

        private List<BundleResponse> ActivityBundleBind(List<ProductOption> productOption)
        {
            var BundleResponseList = new List<BundleResponse>();
            foreach (var item in productOption)
            {
                var bundleResponse = new BundleResponse
                {
                    ServiceId = item.ComponentServiceID,
                    IsSameDayBookable = item.IsSameDayBookable,
                    ServiceName = item.ComponentServiceName
                };
                if (!BundleResponseList.Any(x => x.ServiceId == bundleResponse.ServiceId && x.ServiceName == bundleResponse.ServiceName && x.IsSameDayBookable == bundleResponse.IsSameDayBookable))
                {
                    BundleResponseList.Add(bundleResponse);
                }
            }
            return BundleResponseList;
        }

        /// <summary>
        /// This operation is used to map service response to activity price data response model
        /// </summary>
        /// <param name="activityMinPriceData"></param>
        /// <returns></returns>
        public List<ActivityMinPriceResponse> MapActivityPriceData(List<ActivityMinPrice> activityMinPriceData)
        {
            var activityMinPriceResponseList = new List<ActivityMinPriceResponse>();
            if (activityMinPriceData != null)
            {
                foreach (var Item in activityMinPriceData)
                {
                    var activityMinPriceResponse = new ActivityMinPriceResponse
                    {
                        AffiliateID = Item.AffiliateID,
                        BasePrice = Item.BasePrice,
                        OfferPercent = Item.Offer_Percent,
                        SellPrice = Item.SellPrice,
                        Serviceid = Item.Serviceid
                    };
                    activityMinPriceResponseList.Add(activityMinPriceResponse);
                }
            }
            return activityMinPriceResponseList;
        }

        /// <summary>
        /// This operation is used to map service response to activity price data response model
        /// </summary>
        /// <param name="activityAvailableDays"></param>
        /// <returns></returns>
        public List<ActivityAvailableDaysResponse> MapActivityAvailableData(List<ActivityAvailableDays> activityAvailableDays)
        {
            var activityAvailableDaysResponseList = new List<ActivityAvailableDaysResponse>();
            if (activityAvailableDays != null)
            {
                foreach (var Item in activityAvailableDays)
                {
                    var activityMinPriceResponse = new ActivityAvailableDaysResponse
                    {
                        Serviceid = Item.Serviceid,
                        Availability = Item.AvailableDays
                    };
                    activityAvailableDaysResponseList.Add(activityMinPriceResponse);
                }
            }
            return activityAvailableDaysResponseList;
        }

        private List<TimesOfDayResponse> ActivityTimeofDayBind(List<ProductOption> productOption)
        {
            var TimesOfDayResponseList = new List<TimesOfDayResponse>();
            if (productOption != null)
            {
                foreach (var item in productOption)
                {
                    var timeofDayResponse = new TimesOfDayResponse
                    {
                        ServiceOptionID = item.Id,
                        TimesOfDay = item.TimesOfDays
                    };
                    TimesOfDayResponseList.Add(timeofDayResponse);
                }
            }
            return TimesOfDayResponseList;
        }

        private List<Rating> ActivityRatingsBind(List<Rating> rating)
        {
            var RatingList = new List<Rating>();
            if (rating != null)
            {
                foreach (var item in rating)
                {
                    var ratingResponse = new Rating
                    {
                        ServiceTypeRatingId = item.ServiceTypeRatingId,
                        ServiceTypeRatingTypeId = item.ServiceTypeRatingTypeId,
                        ServiceTypeRatingName = item?.ServiceTypeRatingName?.Trim(),
                        ServiceTypeRatingTypeName = item?.ServiceTypeRatingTypeName?.Trim()
                    };
                    RatingList.Add(ratingResponse);
                }
            }
            return RatingList;
        }

        #endregion [Delta Activity]

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