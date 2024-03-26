using Bumblebee.Models.RequestModels;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using OfficeOpenXml;
using ServiceAdapters.HB.HB.Entities.ActivityDetail;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels;
using Util;
using WebAPI.Filters;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using WebAPI.Models.v1Css;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [CustomActionWebApiFilter]
    [ApiController]
    public class ActivityController : ApiBaseController
    {
        private readonly IActivityService _activityService;
        private readonly IMasterPersistence _masterPersistence;
        private readonly ITableStorageOperation _TableStorageOperations;
        private readonly ActivityMapper _activityMapper;
        private readonly ActivityHelper _activityHelper;
        private readonly IMasterService _masterService;



        /// <summary>
        /// Parameterized Constructor to initialize all dependencies.
        /// </summary>
        /// <param name="activityService"></param>
        /// <param name="activityHelper"></param>
        /// <param name="tableStorageOperation"></param>
        /// <param name="activityMapper"></param>
        /// <param name="masterPersistence"></param>
        public ActivityController(IActivityService activityService, ActivityHelper activityHelper, ITableStorageOperation tableStorageOperation,
            ActivityMapper activityMapper,
            IMasterPersistence masterPersistence,
            IMasterService masterService

        )
        {
            _activityService = activityService;
            _TableStorageOperations = tableStorageOperation;
            _activityMapper = activityMapper;
            _activityHelper = activityHelper;
            _masterPersistence = masterPersistence;
            _masterService = masterService;


        }

        /// <summary>
        /// This operation retrieves availabilities of requested product
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        [Route("priceandavailability/{activityId}/{affiliateId}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetPriceAndAvailability(int activityId, string affiliateId)
        {
            var affiliate = _activityHelper.GetAffiliateInfo(affiliateId);
            if (affiliate == null) return GetResponseWithActionResult(affiliate);

            var calendarAvailabilityList = _activityHelper.GetPriceAndAvailabilities(activityId, affiliateId);
            var calendarWithDefaultAffiliateIdAvailabilityList = _activityHelper.GetPriceAndAvailabilities(activityId, "default");

            var calendarResponse = _activityMapper.MapCalendarDetails(activityId, affiliateId, calendarAvailabilityList, calendarWithDefaultAffiliateIdAvailabilityList, affiliate, null, null);
            return GetResponseWithActionResult(calendarResponse);
        }


        /// <summary>
        /// Get availability of product
        /// </summary>
        /// <param name="availabilityInput"></param>
        /// <returns></returns>
        //[Route("v1/checkavailability", Name = "CheckAvailabilityV1")]
        [Route("checkavailability", Name = "CheckAvailability")]
        [HttpPost]
        //[ValidateModel]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProductAvailability(CheckAvailabilityRequest availabilityInput)
        {
            var affiliateId = availabilityInput.AffiliateId;
            if (string.IsNullOrWhiteSpace(affiliateId))
            {
                affiliateId = GetAffiliateFromIdentity();
                availabilityInput.AffiliateId = affiliateId;
            }
            if (string.IsNullOrEmpty(availabilityInput.TokenId))
            {
                availabilityInput.TokenId = affiliateId;
            }
            ValidateAvailabilityRequest(availabilityInput);

            var affiliate = _activityHelper.GetAffiliateInfo(availabilityInput?.AffiliateId);
            if (affiliate == null) return GetResponseWithActionResult(affiliate, CommonErrorConstants.AffiliateNotFound);

            var clientInfo = _activityHelper.PrepareClientInfoInput(availabilityInput);
            clientInfo.IsSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;

            var activityLoaded = _activityHelper.LoadActivity(availabilityInput.ActivityId, availabilityInput.CheckinDate, clientInfo);

            if (activityLoaded == null || activityLoaded?.ProductOptions?.Any() == false)
            {
                return GetResponseWithActionResult(availabilityInput, "Activity Option not found", HttpStatusCode.NotFound);
            }

            if (activityLoaded.ActivityType == ActivityType.Bundle)
            {
                return GetProductBundleAvailability(availabilityInput);
            }

            var defaultLimit = ConfigurationManagerHelper.GetValuefromAppSettings("CheckAvailabilityDefaultLimit");
            if ((availabilityInput.CheckoutDate - availabilityInput.CheckinDate).TotalDays <= Convert.ToInt32(defaultLimit))
            {
                //Process pax where count is > 0
                if (availabilityInput?.PaxDetails != null)
                {
                    availabilityInput.PaxDetails = availabilityInput.PaxDetails?.Where(x => x.Count > 0).ToList();
                    if (availabilityInput?.PaxDetails?.Select(x => x.PassengerTypeId)?.Distinct()?.ToList()?.Count != availabilityInput?.PaxDetails?.Count)
                    {
                        return GetResponseWithActionResult(availabilityInput.PaxDetails, CommonErrorConstants.ActivityDuplicatePassengers);
                    }
                }

                var paxMappings = activityLoaded.PassengerInfo;

                if (paxMappings == null || paxMappings?.Any() == false)
                {
                    return GetResponseWithActionResult(availabilityInput, "Passenger Mapping not found for the activity", HttpStatusCode.NotFound);
                }

                var criteria = new Criteria
                {
                    // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S2259 // Null pointers should not be dereferenced
                    CheckinDate = availabilityInput.CheckinDate.Date,
#pragma warning restore S2259 // Null pointers should not be dereferenced
                    CheckoutDate = availabilityInput.CheckoutDate.Date,
                    NoOfPassengers = new Dictionary<PassengerType, int>(),
                    Token = availabilityInput?.TokenId ?? Guid.NewGuid().ToString(),
                    Language = availabilityInput.LanguageCode,
                    ActivityId = availabilityInput.ActivityId,
                    Ages = new Dictionary<PassengerType, int>(),
                    CurrencyFromDataBase = activityLoaded?.CurrencyIsoCode
                };

                #region Get pax band from ages

                /*
                var inputAges = new Dictionary<PassengerType, List<int>>();
                foreach (var item in availabilityInput.PaxDetails)
                {
                    if (item.Count != 0)
                    {
                        try
                        {
                            if (item?.Ages == null)
                            {
                                item.Ages = paxMappings.Where(x => x.PassengerTypeId == (int)item.PassengerTypeId)
                                    .Select(y => y.ToAge - 1).ToList();
                            }
                            foreach (var age in item?.Ages)
                            {
                                var mappedPax = paxMappings?.FirstOrDefault(x => (PassengerType)x.PassengerTypeId == item.PassengerTypeId && (age >= x.FromAge && age <= x.ToAge)) ??

                                    paxMappings?.FirstOrDefault(x => age >= x.FromAge && age <= x.ToAge) ??

                                    paxMappings?.FirstOrDefault(x => (PassengerType)x.PassengerTypeId == item.PassengerTypeId);
                                var mappedPaxType = (PassengerType)mappedPax.PassengerTypeId;

                                if (!inputAges.Keys.Contains(mappedPaxType) && mappedPax != null)
                                {
                                    var ags = new List<int>();
                                    ags.Add(age);
                                    inputAges.Add(mappedPaxType, ags);
                                }
                                else
                                {
                                    var mappedinputAges = inputAges[mappedPaxType];
                                    mappedinputAges.Add(age);
                                }

                                if (!criteria.NoOfPassengers.Keys.Contains(mappedPaxType) && mappedPax != null)
                                {
                                    criteria.NoOfPassengers.Add(mappedPaxType, 1);
                                }
                                else
                                {
                                    criteria.NoOfPassengers[mappedPaxType]++;
                                }
                                if (!criteria.Ages.Keys.Contains(mappedPaxType) && mappedPax != null)
                                {
                                    criteria.Ages.Add(mappedPaxType, age);
                                }
                            }
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
                            //_log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
                */

                #endregion Get pax band from ages

                //var watch = System.Diagnostics.Stopwatch.StartNew();
                //long time = 0;
                var activity = _activityHelper.GetProductAvailability(availabilityInput, clientInfo, criteria);
                //watch.Stop();
                //time = watch.ElapsedMilliseconds;
                //activity.ApiType;
                if (activity?.ProductOptions?.Count > 0)
                {
                    //if cancellationPolicy from Api is NULL or Empty
                    foreach (var item in activity?.ProductOptions)
                    {
                        if (string.IsNullOrEmpty(item.CancellationText) || item.CancellationText.ToLower() == "null")
                        {
                            item.CancellationText = activity.CancellationPolicy;
                        }
                    }

                    var apiType = activity.ApiType;

                    //watch = System.Diagnostics.Stopwatch.StartNew();
                    activity.ProductOptions = _activityHelper.GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId,
                       activity.ProductOptions, clientInfo, criteria.CheckinDate, apiType, availabilityInput.IsQrscan);
                    //watch.Stop();
                    //time = watch.ElapsedMilliseconds;
                    //activity = _activityHelper.CalculateActivityWithMinPrices(activity);
                    if (activity.ProductOptions.Any(x => x.BasePrice?.DatePriceAndAvailabilty?.Count > 0))
                    {
                        #region Get pax band from ages

                        /*
                        foreach (var item in activity.ProductOptions)
                        {
                            try
                            {
                                var bp = item.BasePrice;
                                var gp = item.GateBasePrice;
                                var cp = item.CostPrice;
                                try
                                {
                                    foreach (var dpa in bp?.DatePriceAndAvailabilty)
                                    {
                                        foreach (var pu in dpa.Value?.PricingUnits)
                                        {
                                            var pupaxType = ((PerPersonPricingUnit)pu).PassengerType;
                                            pu.Ages = inputAges[pupaxType];
                                        }
                                    }

                                    foreach (var dpa in gp?.DatePriceAndAvailabilty)
                                    {
                                        foreach (var pu in dpa.Value?.PricingUnits)
                                        {
                                            var pupaxType = ((PerPersonPricingUnit)pu).PassengerType;
                                            pu.Ages = inputAges[pupaxType];
                                        }
                                    }

                                    foreach (var dpa in cp?.DatePriceAndAvailabilty)
                                    {
                                        foreach (var pu in dpa.Value?.PricingUnits)
                                        {
                                            var pupaxType = ((PerPersonPricingUnit)pu).PassengerType;
                                            pu.Ages = inputAges[pupaxType];
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        */

                        #endregion Get pax band from ages

                        //watch = System.Diagnostics.Stopwatch.StartNew();
                        _TableStorageOperations.InsertData(activity, clientInfo.ApiToken);
                        //watch.Stop();
                        //time = watch.ElapsedMilliseconds;
                    }
                }


                var checkAvailabilitiesResponse = _activityMapper.MapCheckAvailabilityResponse(activity, criteria, clientInfo.ApiToken);
                if (activityLoaded?.IsHideGatePrice == true && clientInfo.IsB2BAffiliate != true)
                {
                    var options = checkAvailabilitiesResponse?.Options;
                    if (options != null)
                    {
                        foreach (var opt in options)
                        {
                            opt.GateBasePrice = opt.BasePrice;
                        }
                    }
                }

                return GetResponseWithActionResult(checkAvailabilitiesResponse);
            }
            var message = Constant.DefaultRangeExceed + " " + defaultLimit + Constant.DefaultRangeExceedDays;
            return GetResponseWithActionResult(message);
        }
        private string GetAffiliateFromIdentity()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var affiliateId = userClaims.FirstOrDefault(o => o.Type == "affiliateId")?.Value;
                return affiliateId;
            }
            return null;
        }
        [Route("GetProductBundleAvailabilityOld")]
        [HttpPost]
        public IActionResult GetProductBundleAvailabilityOld(CheckAvailabilityRequest availabilityInput)
        {
            var affiliate = _activityHelper.GetAffiliateInfo(availabilityInput.AffiliateId);
            var clientInfo = _activityHelper.PrepareClientInfoInput(availabilityInput);
            clientInfo.IsSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;
            var activityLoaded = _activityHelper.LoadActivity(availabilityInput.ActivityId, availabilityInput.CheckinDate, clientInfo);
            availabilityInput.IsBundle = (activityLoaded?.ActivityType == ActivityType.Bundle || availabilityInput?.IsBundle == true);

            availabilityInput.CheckoutDate = availabilityInput.CheckinDate;
            var checkAvailabilityResponses = new List<CheckAvailabilityResponse>();

            var checkAvailabilitiesResponse = new CheckAvailabilityResponse
            {
                ActivityId = availabilityInput.ActivityId,
                Errors = new List<Error>(),
                IsBundle = true,
                Name = activityLoaded?.Name ?? string.Empty
            };

            if (affiliate == null)
            {
                checkAvailabilitiesResponse.Errors.Add(new Error
                {
                    Code = CommonErrorConstants.AffiliateNotCODE,
                    HttpStatus = System.Net.HttpStatusCode.BadRequest,
                    Message = CommonErrorConstants.AffiliateNotFound + SerializeDeSerializeHelper.Serialize(availabilityInput?.AffiliateId)
                });
                checkAvailabilityResponses.Add(checkAvailabilitiesResponse);
                return GetResponseWithActionResult(checkAvailabilityResponses);
            }

            if (!(activityLoaded?.ProductOptions?.Count > 0))
            {
                if (activityLoaded == null)
                {
                    checkAvailabilitiesResponse.Errors.Add(new Error
                    {
                        Code = Util.ErrorCodes.ACTIVITY_NOT_FOUND,
                        HttpStatus = System.Net.HttpStatusCode.NotFound,
                        Message = Util.ErrorMessages.ACTIVITY_NOT_FOUND + SerializeDeSerializeHelper.Serialize(availabilityInput)
                    });
                }
                else
                {
                    checkAvailabilitiesResponse.Errors.Add(new Error
                    {
                        Code = Util.ErrorCodes.BUNDLE_OPTION_NOT_FOUND,
                        HttpStatus = System.Net.HttpStatusCode.NotFound,
                        Message = CommonErrorConstants.ActivityOptionNotFound + SerializeDeSerializeHelper.Serialize(availabilityInput)
                    });
                }
                return GetResponseWithActionResult(checkAvailabilitiesResponse);
            }

            var activities = new List<Activity>();

            var bundleAvailabilityInput = new CheckBundleAvailabilityRequest
            {
                ActivityId = availabilityInput.ActivityId,
                AffiliateId = availabilityInput.AffiliateId,

                CountryIp = availabilityInput.CountryIp,
                CurrencyIsoCode = availabilityInput.CurrencyIsoCode,
                ComponentActivityDetails = new List<ComponentActivityDetail>(),
                LanguageCode = availabilityInput.LanguageCode ?? "en",
                TokenId = availabilityInput.TokenId
            };

            var criteria = new Criteria
            {
                // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S2259 // Null pointers should not be dereferenced
                CheckinDate = availabilityInput.CheckinDate.Date,
#pragma warning restore S2259 // Null pointers should not be dereferenced
                CheckoutDate = availabilityInput.CheckoutDate.Date,
                NoOfPassengers = new Dictionary<PassengerType, int>(),
                Token = availabilityInput.TokenId,
                Language = availabilityInput.LanguageCode,
                ActivityId = availabilityInput.ActivityId
            };

            //# Test code
            //activityLoaded.ProductOptions = activityLoaded?.ProductOptions.Where(x => x.BundleOptionID == 150173).ToList();

            var distinctActivitiesInBundle = activityLoaded?.ProductOptions?.Select(x => x.ComponentServiceID)?.Distinct()?.ToList();

            if (distinctActivitiesInBundle?.Any() == true)
            {
                foreach (var actId in distinctActivitiesInBundle)
                {
                    var availabilitiesResponse = new CheckAvailabilityResponse();
                    try
                    {
                        var componentActivityDetail = new ComponentActivityDetail
                        {
                            CheckinDate = availabilityInput.CheckinDate,
                            CheckoutDate = availabilityInput.CheckoutDate,
                            ComponentActivityId = actId,
                            PaxDetails = availabilityInput.PaxDetails?.ToList()
                        };

                        bundleAvailabilityInput.ComponentActivityDetails.Add(componentActivityDetail);

                        #region Activity approach commented

                        /*
                                        var req = new CheckAvailabilityRequest
                                        {
                                            ActivityId = actId,
                                            AffiliateId = availabilityInput.AffiliateId,
                                            CheckinDate = availabilityInput.CheckinDate,
                                            CheckoutDate = availabilityInput.CheckoutDate,
                                            CountryIp = availabilityInput.CountryIp,
                                            CurrencyIsoCode = availabilityInput?.CurrencyIsoCode ?? "GBP",
                                            IsBundle = true,
                                            LanguageCode = availabilityInput?.LanguageCode ?? "en",
                                            PaxDetails = availabilityInput.PaxDetails?.ToList(),
                                            TokenId = availabilityInput.TokenId
                                        };

                                        var activityResult = _activityHelper.GetProductAvailability(req, clientInfo, criteria);
                                        //watch.Stop();
                                        //time = watch.ElapsedMilliseconds;
                                        //activity.ApiType;
                                        if (activityResult?.ProductOptions?.Count > 0)
                                        {
                                            activities.Add(activityResult);
                                        }
                                        //*/

                        #endregion Activity approach commented
                    }
                    catch (Exception ex)
                    {
                        var anyAct = checkAvailabilityResponses?.FirstOrDefault();
                        if (anyAct == null)
                        {
                            checkAvailabilityResponses = new List<CheckAvailabilityResponse>();
                        }
                        if (checkAvailabilitiesResponse == null)
                        {
                            checkAvailabilitiesResponse = new CheckAvailabilityResponse();
                        }
                        if (checkAvailabilitiesResponse.Errors == null)
                        {
                            checkAvailabilitiesResponse.Errors = new List<Error>();
                        }
                        if (checkAvailabilityResponses?.FirstOrDefault()?.Errors != null)
                            checkAvailabilityResponses?.FirstOrDefault()?.Errors?.Add(new Error
                            {
                                Code = CommonErrorCodes.AvailabilityError.ToString(),
                                HttpStatus = System.Net.HttpStatusCode.BadGateway,
                                Message = ex.Message
                            });
                    }

                    #region Activity approach commented

                    /*
                               if (activities?.Count == distinctActivitiesInBundle?.Count)
                               {
                                   foreach (var activityResult in activities)
                                   {
                                       try
                                       {
                                           var apiType = activityResult.ApiType;

                                           activityResult.ProductOptions = _activityHelper.GetProductOptionsAfterPriceRuleEngine(activityResult.PriceTypeId,
                                              activityResult.ProductOptions, clientInfo, criteria.CheckinDate, apiType);

                                           if (activityResult.ProductOptions.Any(x => x.BasePrice.DatePriceAndAvailabilty.Count > 0))
                                           {
                                               _TableStorageOperations.InsertData(activityResult, clientInfo.ApiToken);
                                           }
                                           else
                                           {
                                               checkAvailabilitiesResponse.Errors.Add(new Error
                                               {
                                                   Code = CommonErrorConstants.ActivityOptionNotFoundCODE.ToString(),
                                                   HttpStatus = System.Net.HttpStatusCode.BadGateway,
                                                   Message = CommonErrorConstants.ActivityOptionNotFound
                                               });
                                               break;
                                           }
                                           checkAvailabilitiesResponse = _activityMapper.MapCheckAvailabilityResponse(activityResult, criteria, clientInfo.ApiToken);
                                       }
                                       catch (Exception ex)
                                       {
                                           var anyAct = checkAvailabilityResponses?.FirstOrDefault();
                                           if (anyAct == null)
                                           {
                                               checkAvailabilityResponses = new List<CheckAvailabilityResponse>();
                                           }
                                           if (checkAvailabilitiesResponse == null)
                                           {
                                               checkAvailabilitiesResponse = new CheckAvailabilityResponse();
                                           }
                                           if (checkAvailabilitiesResponse.Errors == null)
                                           {
                                               checkAvailabilitiesResponse.Errors = new List<Error>();
                                           }
                                           if (checkAvailabilityResponses?.FirstOrDefault() != null)
                                               checkAvailabilityResponses?.FirstOrDefault()?.Errors?.Add(new Error
                                               {
                                                   Code = CommonErrorCodes.AvailabilityError.ToString(),
                                                   HttpStatus = System.Net.HttpStatusCode.BadGateway,
                                                   Message = ex.Message
                                               });
                                       }
                                       checkAvailabilityResponses.Add(checkAvailabilitiesResponse);
                                   }
                               }

                               //*/

                    #endregion Activity approach commented
                }
            }
            //return GetResponseWithActionResult(checkAvailabilityResponses);

            #region old code modified

            //*
            var criteriaForActivity = new Dictionary<int, Criteria>();

            foreach (var item in bundleAvailabilityInput.ComponentActivityDetails)
            {
                var bundleCriteria = new Criteria
                {
                    CheckinDate = item.CheckinDate.Date,
                    CheckoutDate = item.CheckoutDate.Date,
                    NoOfPassengers = new Dictionary<PassengerType, int>(),
                    ActivityId = item.ComponentActivityId,
                    Ages = criteria.Ages,
                    IsBundle = true,
                    Language = criteria?.Language ?? "en",
                    Token = availabilityInput.TokenId,
                    PassengerInfo = criteria.PassengerInfo
                };

                foreach (var paxDetails in item.PaxDetails)
                {
                    if (paxDetails.Count != 0)
                    {
                        bundleCriteria.NoOfPassengers.Add(paxDetails.PassengerTypeId, paxDetails.Count);
                    }
                }

                criteriaForActivity.Add(item.ComponentActivityId, bundleCriteria);
            }

            var activity = _activityHelper.GetBundleProductAvailability(bundleAvailabilityInput, clientInfo, criteriaForActivity);

            //activity.ProductOptions = activity?.ProductOptions.Where(x => x.BundleOptionID == 150173).ToList();
            CheckBundleAvailabilityResponse checkBundleAvailabilitiesResponse = null;
            if (activity != null && activity?.ProductOptions?.Count > 0 && activity?.Errors?.Count == 0)
            {
                activity.ProductOptions = _activityHelper.GetBundleProductOptionsAfterPriceRuleEngine(activity.ProductOptions, clientInfo, activity.PriceTypeId, bundleAvailabilityInput.ComponentActivityDetails, availabilityInput.IsQrscan);

                if (activity?.ProductOptions?.Count > 0)
                {
                    var acCopy = activity?.ProductOptions?.ToList();
                    var groups = acCopy.GroupBy(x => x.ApiType);
                    foreach (var group in groups)
                    {
                        activity.ApiType = group.Key;
                        activity.ProductOptions = group.ToList();
                        _TableStorageOperations.InsertData(activity, clientInfo.ApiToken);
                    }
                    activity.ProductOptions = acCopy;
                }
                var optionIsoCodes = activity.ProductOptions?.Select(x => x.GateBasePrice.Currency.IsoCode).Distinct().ToList();
                var exchangeRateValues = _activityHelper.GetConversionFactor(activity.CurrencyIsoCode, optionIsoCodes);

                checkBundleAvailabilitiesResponse =
                    _activityMapper.MapCheckBundleAvailabilityResponse(activity, criteriaForActivity, clientInfo.ApiToken, exchangeRateValues);
                checkAvailabilitiesResponse = _activityMapper.MapBundleToAvailabilityResponse(checkBundleAvailabilitiesResponse, checkAvailabilitiesResponse);

                foreach (var item in checkAvailabilitiesResponse?.Options)
                {
                    item.CancellationPolicy = "";
                }
                var firstOption = checkAvailabilitiesResponse?.Options?.FirstOrDefault();
                if (firstOption != null)
                {
                    firstOption.CancellationPolicy = activityLoaded.CancellationPolicy;
                }
            }

            if (!(checkAvailabilitiesResponse?.Options?.Count > 0))
            {
                if (activityLoaded == null)
                {
                    checkAvailabilitiesResponse.Errors.Add(new Error
                    {
                        Code = Util.ErrorCodes.ACTIVITY_NOT_FOUND,
                        HttpStatus = System.Net.HttpStatusCode.NotFound,
                        Message = Util.ErrorMessages.ACTIVITY_NOT_FOUND + SerializeDeSerializeHelper.Serialize(availabilityInput)
                    });
                }
                else
                {
                    checkAvailabilitiesResponse.Errors.Add(new Error
                    {
                        Code = Util.ErrorCodes.BUNDLE_OPTION_NOT_FOUND,
                        HttpStatus = System.Net.HttpStatusCode.NotFound,
                        Message = CommonErrorConstants.ActivityOptionNotFound + SerializeDeSerializeHelper.Serialize(availabilityInput)
                    });
                }
            }
            if (activity?.Errors?.Any() == true)
            {
                checkAvailabilitiesResponse.ActivityId = activityLoaded.ID;
                checkAvailabilitiesResponse.Name = activityLoaded.Name;
                checkAvailabilitiesResponse.TokenId = availabilityInput.TokenId;
                checkAvailabilitiesResponse.IsBundle = true;

                checkAvailabilitiesResponse.Errors = activity.Errors;
            }

            return GetResponseWithActionResult(checkAvailabilitiesResponse);
            //*/

            #endregion old code modified
        }

        /// <summary>
        /// This operation is used to Check availability of Bundle Products
        /// </summary>
        /// <param name="availabilityInput"></param>
        /// <returns></returns>
        [Route("checkbundleavailability")]
        [HttpPost]
        [ValidateModel]
        public IActionResult GetProductBundleAvailability(CheckAvailabilityRequest availabilityInput)
        {
            var activities = new List<Activity>();
            var loadedPaxInfos = new Dictionary<int, List<Isango.Entities.Booking.PassengerInfo>>();
            var inputAges = new Dictionary<int, Dictionary<PassengerType, List<int>>>();
            var noOfPassengers = new Dictionary<int, Dictionary<PassengerType, int>>();
            var Ages = new Dictionary<int, Dictionary<PassengerType, int>>();
            var criterias = new Dictionary<int, Criteria>();
            var infantCheck = false;
            var avoidInfantActivityId = 0;

            var affiliate = _activityHelper.GetAffiliateInfo(availabilityInput.AffiliateId);
            var clientInfo = _activityHelper.PrepareClientInfoInput(availabilityInput);
            clientInfo.IsSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;
            var activityBundleLoaded = _activityHelper.LoadActivity(availabilityInput.ActivityId, availabilityInput.CheckinDate, clientInfo);
            availabilityInput.IsBundle = (activityBundleLoaded?.ActivityType == ActivityType.Bundle || availabilityInput?.IsBundle == true);

            var defaultAges = new Dictionary<PassengerType, int>();
            defaultAges.Add(PassengerType.Adult, 30);
            defaultAges.Add(PassengerType.Senior, 61);
            defaultAges.Add(PassengerType.Infant, 1);

            availabilityInput.CheckoutDate = availabilityInput.CheckinDate;
            var checkAvailabilityResponses = new List<CheckAvailabilityResponse>();

            var checkAvailabilitiesResponse = new CheckAvailabilityResponse
            {
                ActivityId = availabilityInput.ActivityId,
                Errors = new List<Error>(),
                IsBundle = true,
                Name = activityBundleLoaded?.Name ?? string.Empty
            };

            if (affiliate == null)
            {
                checkAvailabilitiesResponse.Errors.Add(new Error
                {
                    Code = CommonErrorConstants.AffiliateNotCODE,
                    HttpStatus = System.Net.HttpStatusCode.BadRequest,
                    Message = CommonErrorConstants.AffiliateNotFound + SerializeDeSerializeHelper.Serialize(availabilityInput?.AffiliateId)
                });
                checkAvailabilityResponses.Add(checkAvailabilitiesResponse);
                return GetResponseWithActionResult(checkAvailabilityResponses);
            }

            if (!(activityBundleLoaded?.ProductOptions?.Count > 0))
            {
                if (activityBundleLoaded == null)
                {
                    _activityHelper.UpdateErrorCheckAvailabilityResponse(checkAvailabilityResponses, Util.ErrorCodes.ACTIVITY_NOT_FOUND.ToString(), Util.ErrorMessages.ACTIVITY_NOT_FOUND + SerializeDeSerializeHelper.Serialize(availabilityInput), System.Net.HttpStatusCode.NotFound);
                }
                else
                {
                    _activityHelper.UpdateErrorCheckAvailabilityResponse(checkAvailabilityResponses, Util.ErrorCodes.BUNDLE_OPTION_NOT_FOUND.ToString(), CommonErrorConstants.ActivityOptionNotFound + SerializeDeSerializeHelper.Serialize(availabilityInput), System.Net.HttpStatusCode.NotFound);
                }
                return GetResponseWithActionResult(checkAvailabilitiesResponse);
            }

            var bundleAvailabilityInput = new CheckBundleAvailabilityRequest
            {
                ActivityId = availabilityInput.ActivityId,
                AffiliateId = availabilityInput.AffiliateId,

                CountryIp = availabilityInput.CountryIp,
                CurrencyIsoCode = availabilityInput.CurrencyIsoCode,
                ComponentActivityDetails = new List<ComponentActivityDetail>(),
                LanguageCode = availabilityInput.LanguageCode ?? "en",
                TokenId = availabilityInput.TokenId
            };

            //# Test code
            //activityLoaded.ProductOptions = activityLoaded?.ProductOptions.Where(x => x.BundleOptionID == 150173).ToList();

            var distinctActivitiesInBundle = activityBundleLoaded?.ProductOptions?.Select(x => x.ComponentServiceID)?.Distinct()?.ToList();
            var dynamicBundleOptionId = activityBundleLoaded?.ProductOptions?.FirstOrDefault()?.BundleOptionID ?? 0;

            if (distinctActivitiesInBundle?.Any() == true)
            {
                var processorCount = 2;
                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = processorCount
                };

                try
                {
                    foreach (var actId in distinctActivitiesInBundle)
                    {
                        var activityLoaded = _activityHelper.LoadActivity(actId, availabilityInput.CheckinDate, clientInfo);
                        var paxMappings = activityLoaded.PassengerInfo;
                        loadedPaxInfos.Add(actId, paxMappings);

                        var _valueInputAges = new Dictionary<PassengerType, List<int>>();
                        var _valueNoOfPassengers = new Dictionary<PassengerType, int>();
                        var _valueAges = new Dictionary<PassengerType, int>();

                        foreach (var item in availabilityInput.PaxDetails)
                        {
                            if (item.Count != 0)
                            {
                                try
                                {
                                    if (item?.Ages == null)
                                    {
                                        item.Ages = new List<int>();

                                        try
                                        {
                                            var paxAge = defaultAges?.FirstOrDefault(x => (int)x.Key == (int)item.PassengerTypeId).Value ?? 0;

                                            if (paxAge == 0 && (int)item.PassengerTypeId == 2)
                                            {
                                                paxAge = paxMappings.Where(x => x.PassengerTypeId == (int)item.PassengerTypeId)
                                                                            .Select(y => y.FromAge + 1)
                                                                            .FirstOrDefault();
                                            }
                                            if (paxAge == 0 && (int)item.PassengerTypeId == 9)
                                            {
                                                paxAge = 0;
                                            }
                                            if (paxAge == 0)
                                            {
                                                paxAge = 30;
                                            }

                                            for (int i = 0; i < item.Count; i++)
                                            {
                                                item.Ages.Add(paxAge);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }

                                    foreach (var age in item?.Ages)
                                    {
                                        if (!paxMappings.Any(x => x.PassengerTypeId == 9) && item.PassengerTypeId == PassengerType.Infant)
                                        {
                                            infantCheck = true;
                                            avoidInfantActivityId = actId;
                                            continue;
                                        }
                                        var mappedPax = item.PassengerTypeId == PassengerType.Child ? paxMappings?.FirstOrDefault(x => (PassengerType)x.PassengerTypeId == item.PassengerTypeId && (age >= x.FromAge && age <= x.ToAge)) ??

                                            paxMappings?.FirstOrDefault(x => age >= x.FromAge && age <= x.ToAge) ??

                                            paxMappings?.FirstOrDefault(x => (PassengerType)x.PassengerTypeId == item.PassengerTypeId) :

                                            paxMappings?.FirstOrDefault(x => (PassengerType)x.PassengerTypeId == item.PassengerTypeId);

                                        var mappedPaxType = (PassengerType)mappedPax.PassengerTypeId;

                                        if (!_valueInputAges.Keys.Contains(mappedPaxType) && mappedPax != null)
                                        {
                                            var ags = new List<int>();
                                            ags.Add(age);
                                            _valueInputAges.Add(mappedPaxType, ags);
                                        }
                                        else
                                        {
                                            var mappedinputAges = _valueInputAges[mappedPaxType];
                                            mappedinputAges.Add(age);
                                        }

                                        if (!_valueNoOfPassengers.Keys.Contains(mappedPaxType) && mappedPax != null)
                                        {
                                            _valueNoOfPassengers.Add(mappedPaxType, 1);
                                        }
                                        else
                                        {
                                            _valueNoOfPassengers[mappedPaxType]++;
                                        }
                                        if (!_valueAges.Keys.Contains(mappedPaxType) && mappedPax != null)
                                        {
                                            _valueAges.Add(mappedPaxType, age);
                                        }
                                    }
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
                                    //_log.Error(isangoErrorEntity, ex);
                                }
                            }
                        }

                        if (_valueInputAges?.Any() == true)
                        {
                            inputAges.Add(actId, _valueInputAges);
                        }
                        if (_valueNoOfPassengers?.Any() == true)
                        {
                            noOfPassengers.Add(actId, _valueNoOfPassengers);
                        }
                        if (_valueAges?.Any() == true)
                        {
                            Ages.Add(actId, _valueAges);
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                //Parallel.ForEach(distinctActivitiesInBundle, parallelOptions, actId =>
                foreach (var actId in distinctActivitiesInBundle)
                {
                    var availabilitiesResponse = new CheckAvailabilityResponse();
                    var paxDetailsNew = new List<PaxDetail>();

                    var criteriaLocal = new Criteria
                    {
                        // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S2259 // Null pointers should not be dereferenced
                        CheckinDate = availabilityInput.CheckinDate.Date,
#pragma warning restore S2259 // Null pointers should not be dereferenced
                        CheckoutDate = availabilityInput.CheckoutDate.Date,
                        NoOfPassengers = new Dictionary<PassengerType, int>(),
                        Token = availabilityInput.TokenId,
                        Language = availabilityInput.LanguageCode,
                        ActivityId = actId,
                        Ages = new Dictionary<PassengerType, int>(),
                    };

                    try
                    {
                        var nopActivity = noOfPassengers[actId];
                        var ageActivity = Ages[actId];
                        var ageInputActivity = inputAges[actId];
                        criteriaLocal.NoOfPassengers = nopActivity;
                        criteriaLocal.Ages = ageActivity;

                        foreach (var nop in nopActivity)
                        {
                            var paxDetailNew = new PaxDetail
                            {
                                PassengerTypeId = nop.Key,
                                Count = nop.Value,
                                Ages = ageInputActivity[nop.Key]
                            };
                            paxDetailsNew.Add(paxDetailNew);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    try
                    {
                        var componentActivityDetail = new ComponentActivityDetail
                        {
                            CheckinDate = availabilityInput.CheckinDate,
                            CheckoutDate = availabilityInput.CheckoutDate,
                            ComponentActivityId = actId,
                            PaxDetails = paxDetailsNew //availabilityInput.PaxDetails?.ToList()
                        };

                        bundleAvailabilityInput.ComponentActivityDetails.Add(componentActivityDetail);

                        var req = new CheckAvailabilityRequest
                        {
                            ActivityId = actId,
                            AffiliateId = availabilityInput.AffiliateId,
                            CheckinDate = availabilityInput.CheckinDate,
                            CheckoutDate = availabilityInput.CheckoutDate,
                            CountryIp = availabilityInput.CountryIp,
                            CurrencyIsoCode = availabilityInput?.CurrencyIsoCode ?? "GBP",
                            IsBundle = true,
                            LanguageCode = availabilityInput?.LanguageCode ?? "en",
                            PaxDetails = paxDetailsNew, //availabilityInput.PaxDetails,
                            TokenId = availabilityInput.TokenId
                        };

                        var activityResult = _activityHelper.GetProductAvailability(req, clientInfo, criteriaLocal);
                        //watch.Stop();
                        //time = watch.ElapsedMilliseconds;
                        //activity.ApiType;
                        if (activityResult?.ProductOptions?.Count > 0 && activityResult?.ProductOptions?.Any(x => x.BasePrice?.DatePriceAndAvailabilty?.Any() == true || x.CostPrice?.DatePriceAndAvailabilty?.Any() == true) == true)
                        {
                            if (activityResult?.ProductOptions?.Any() == true)
                            {
                                foreach (var po in activityResult?.ProductOptions)
                                {
                                    po.BundleOptionID = dynamicBundleOptionId;
                                    po.ComponentServiceID = activityResult.ID;
                                    //po.CancellationText = CommonResourceManager.GetString(req.LanguageCode, "CancellationPolicyNonRefundable");
                                    po.CancellationText = activityBundleLoaded?.CancellationPolicy;
                                }
                            }
                            activities.Add(activityResult);
                        }
                        else
                        {
                            activities = null;
                            checkAvailabilitiesResponse.Errors.Add(new Error
                            {
                                Code = CommonErrorConstants.ActivityOptionNotFound_CODE,
                                HttpStatus = HttpStatusCode.NotFound,
                                IsLoggedInDB = false,
                                Message = CommonErrorConstants.BundleComponentyAvailabilityNotFound + " " + activityResult?.Id + "-" + activityResult?.Name
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _activityHelper.UpdateErrorCheckAvailabilityResponse(
                                checkAvailabilityResponses,
                                CommonErrorCodes.AvailabilityError.ToString(),
                                ex.Message,
                                System.Net.HttpStatusCode.BadGateway
                            );
                    }

                    criterias.Add(actId, criteriaLocal);
                }
                //);

                if (activities?.Count == distinctActivitiesInBundle?.Count)
                {
                    //Parallel.ForEach(activities, parallelOptions, activityResult =>
                    foreach (var activityResult in activities)
                    {
                        var criteria = criterias[activityResult.ID];
                        try
                        {
                            var apiType = activityResult.ApiType;

                            activityResult.ProductOptions = _activityHelper.GetProductOptionsAfterPriceRuleEngine(activityResult.PriceTypeId,
                               activityResult.ProductOptions, clientInfo, criteria.CheckinDate, apiType);

                            if (activityResult.ProductOptions?.Any(x => x.BasePrice?.DatePriceAndAvailabilty?.Count > 0) == true)
                            {
                                if (activityResult.ProductOptions.Any(x => x.BasePrice?.DatePriceAndAvailabilty?.Count > 0))
                                {
                                    foreach (var item in activityResult.ProductOptions)
                                    {
                                        try
                                        {
                                            var bp = item.BasePrice;
                                            var gp = item.GateBasePrice;
                                            var cp = item.CostPrice;
                                            var inputAge = inputAges[activityResult.ID];
                                            try
                                            {
                                                foreach (var dpa in bp?.DatePriceAndAvailabilty)
                                                {
                                                    foreach (var pu in dpa.Value?.PricingUnits)
                                                    {
                                                        var pupaxType = ((PerPersonPricingUnit)pu).PassengerType;
                                                        pu.Ages = inputAge[pupaxType];
                                                    }
                                                }

                                                foreach (var dpa in gp?.DatePriceAndAvailabilty)
                                                {
                                                    foreach (var pu in dpa.Value?.PricingUnits)
                                                    {
                                                        var pupaxType = ((PerPersonPricingUnit)pu).PassengerType;
                                                        pu.Ages = inputAge[pupaxType];
                                                    }
                                                }

                                                foreach (var dpa in cp?.DatePriceAndAvailabilty)
                                                {
                                                    foreach (var pu in dpa.Value?.PricingUnits)
                                                    {
                                                        var pupaxType = ((PerPersonPricingUnit)pu).PassengerType;
                                                        pu.Ages = inputAge[pupaxType];
                                                    }
                                                }
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }

                                    _TableStorageOperations.InsertData(activityResult, clientInfo.ApiToken);

                                    //infant related changes
                                    if (infantCheck && avoidInfantActivityId == activityResult.ID)
                                    {
                                        var infantcount = availabilityInput.PaxDetails.Where(x => x.PassengerTypeId == PassengerType.Infant).Count();
                                        criteria.NoOfPassengers.Add(PassengerType.Infant, infantcount);
                                        foreach (var item in activityResult.ProductOptions)
                                        {
                                            try
                                            {
                                                var bp = item.BasePrice;
                                                var gp = item.GateBasePrice;
                                                var cp = item.CostPrice;
                                                var inputAge = inputAges[activityResult.ID];
                                                try
                                                {
                                                    foreach (var dpa in bp?.DatePriceAndAvailabilty)
                                                    {
                                                        var infantPriceUnit = new InfantPricingUnit
                                                        {
                                                            Price = 0,
                                                            Quantity = infantcount,
                                                            UnitType = UnitType.PerPerson,
                                                        };
                                                        dpa.Value.PricingUnits.Add(infantPriceUnit);
                                                    }

                                                    foreach (var dpa in gp?.DatePriceAndAvailabilty)
                                                    {
                                                        var infantPriceUnit = new InfantPricingUnit
                                                        {
                                                            Price = 0,
                                                            Quantity = infantcount,
                                                            UnitType = UnitType.PerPerson,
                                                        };
                                                        dpa.Value.PricingUnits.Add(infantPriceUnit);
                                                    }

                                                    foreach (var dpa in cp?.DatePriceAndAvailabilty)
                                                    {
                                                        var infantPriceUnit = new InfantPricingUnit
                                                        {
                                                            Price = 0,
                                                            Quantity = infantcount,
                                                            UnitType = UnitType.PerPerson,
                                                        };
                                                        dpa.Value.PricingUnits.Add(infantPriceUnit);
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                    }
                                    checkAvailabilitiesResponse = _activityMapper.MapCheckAvailabilityResponse(activityResult, criteria, clientInfo.ApiToken);
                                }
                                else
                                {
                                    _activityHelper.UpdateErrorCheckAvailabilityResponse(
                                            checkAvailabilityResponses,
                                          CommonErrorConstants.ActivityOptionNotFound_CODE.ToString(),
                                            activityResult.ID + " " + CommonErrorConstants.ActivityOptionNotFound_Message,
                                            System.Net.HttpStatusCode.NotFound
                                        );
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _activityHelper.UpdateErrorCheckAvailabilityResponse(
                                        checkAvailabilityResponses,
                                      CommonErrorConstants.ActivityOptionNotFound_CODE.ToString(),
                                        activityResult.ID + " " + CommonErrorConstants.ActivityOptionNotFound_Message,
                                        System.Net.HttpStatusCode.NotFound
                                    );
                            break;
                        }
                        if (activityBundleLoaded?.IsHideGatePrice == true && clientInfo.IsB2BAffiliate != true)
                        {
                            var options = checkAvailabilitiesResponse?.Options;
                            if (options != null)
                            {
                                foreach (var opt in options)
                                {
                                    opt.GateBasePrice = opt.BasePrice;
                                }
                            }
                        }
                        checkAvailabilityResponses.Add(checkAvailabilitiesResponse);
                    }
                    //);
                }
            }


            var query = from a in checkAvailabilityResponses
                        from ao in a?.Options
                        select ao;

            checkAvailabilitiesResponse.ActivityId = activityBundleLoaded.ID;
            checkAvailabilitiesResponse.Name = activityBundleLoaded.Name;
            checkAvailabilitiesResponse.Description = activityBundleLoaded.MetaDescription;
            checkAvailabilitiesResponse.IsPaxDetailRequired = checkAvailabilityResponses.Any(x => x?.IsPaxDetailRequired == true);

            try
            {
                checkAvailabilitiesResponse.Options = query?.ToList();
                var activitiesInRes = checkAvailabilitiesResponse?.Options?.Select(x => x.BundleDetails.ComponentServiceID)?.Distinct().ToList();

                foreach (var activityid in distinctActivitiesInBundle)
                {
                    if (activitiesInRes?.Any(x => x == activityid) == false)
                    {
                        _activityHelper.UpdateErrorCheckAvailabilityResponse(
                                            checkAvailabilityResponses,
                                            CommonErrorConstants.ActivityOptionNotFound_CODE.ToString(),
                                            activityid + " " + CommonErrorConstants.ActivityOptionNotFound_Message,
                                            System.Net.HttpStatusCode.NotFound,
                                            checkAvailabilitiesResponse
                                        );
                    }
                }
                if (checkAvailabilitiesResponse?.Errors?.Any() == true
                    && checkAvailabilitiesResponse?.Options?.Any(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE.ToString()) == false
                )
                {
                    checkAvailabilitiesResponse.Options = null;
                }
                else
                {
                    checkAvailabilitiesResponse.Errors = null;
                }
            }
            catch
            {
            }
            return GetResponseWithActionResult(checkAvailabilitiesResponse);
        }

        /// <summary>
        ///  This operation retrieves cancellation policy details of requested product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("cancellationpolicy")]
        [HttpPost]
        [ValidateModel]
        public IActionResult GetCancellationPolicy(CancellationPolicyRequest request)
        {
            var startDate = DateTime.Now;
            var clientInfo = _activityMapper.MapClientInfoForCancellationPolicy(request);

            var result = _activityService.LoadActivityAsync(request.ActivityId, startDate, clientInfo);

            var cancellationPolicyResponse = _activityMapper.MapCancellationResponse(result.Result, request);
            return GetResponseWithActionResult(cancellationPolicyResponse);
        }

        /// <summary>
        /// This operation retrieves Activity details for given activityid
        /// </summary>
        /// <param name="activityCriteria"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpPost]
        public IActionResult GetActivityDetails(ActivityCriteria activityCriteria)
        {
            var affiliate = _activityHelper.GetAffiliateInfo(activityCriteria.AffiliateId);
            if (affiliate == null) return GetResponseWithActionResult(affiliate);

            var criteria = _activityHelper.GetDefaultCriteria();
            var clientInfo = _activityMapper.MapClientInfoForActivityDetail(activityCriteria);
            clientInfo.IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer;
            clientInfo.IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate;

            var activity = _activityService.GetActivityDetailsAsync(activityCriteria.ActivityId, clientInfo, criteria).Result;

            if (activity == null)
                return GetResponseWithActionResult((Activity)null);

            activity = _activityHelper.GetActivityAfterPriceRuleEngine(activity, clientInfo, criteria.CheckinDate);

            activity = _activityHelper.CalculateActivityWithMinPrices(activity);
            var activityDetails = _activityMapper.MapActivityDetail(activity);
            return GetResponseWithActionResult(activityDetails);
        }

        /*
        /// <summary>
        /// This operation gives the search result for given input criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>*/
        //[Route("search")]
        //[HttpPost]
        //[ValidateModel]
        //public IActionResult GetSearchData(SearchRequestCriteria criteria)
        //{
        //    var affiliate = _activityHelper.GetAffiliateInfo(criteria.AffiliateId);
        //    if (affiliate == null)
        //        return GetResponseWithActionResult(affiliate);
        //    var clientInfo = _activityMapper.MapClientInfoForSearch(criteria, affiliate);

        //    var searchData = _activityHelper.GetSearchDetails(criteria, clientInfo);
        //    searchData.Activities = _activityHelper.GetActivitiesAfterPriceRuleEngine(searchData.Activities, clientInfo, DateTime.Now); // DateTime.Now should be passed from Criteria
        //    searchData.Activities.ForEach(e => { _activityHelper.CalculateActivityWithMinPrices(e); });
        //    var searchDetails = _activityMapper.MapSearchData(searchData);

        //    return GetResponseWithActionResult(searchDetails);
        //}

        /// <summary>
        /// This operation gives the search result for given input criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [Route("search")]
        [HttpPost]
        [ValidateModel]
        public IActionResult GetSearchData(SearchRequestCriteria criteria)
        {
            var searchDetails = GetSearchDetails(criteria);
            return GetResponseWithActionResult(searchDetails);
        }

        [Route("searchProductsByRegion")]
        [HttpPost]
        [ValidateModel]
        public IActionResult SearchProductsByRegionForCSS(string regionname = "paris", string languagecode = "en", int? productlimit = 10)
        {
            var WidgetData = _activityService.GetWidgetData()?.GetAwaiter().GetResult();

            var regionId = WidgetData?.Where(x => x.CSRegionName.ToLower() == regionname.ToLower())?.FirstOrDefault()?.Isangoregionid;

            var criteria = new SearchRequestCriteria()
            {
                AffiliateId = "58c11104-34e6-47ba-926d-e89e4242b962",
                RegionId = regionId ?? 0,
                CountryIp = "127.0.0.0",
                LanguageCode = languagecode.ToLower()
            };

            var searchDetails = GetSearchDetails(criteria);

            var widgetResult = new WidgetResult()
            {
                RegionURL = "Example",
                Activities = new List<WidgetActivity>()
            };

            if (searchDetails?.Count > 0)
            {
                foreach (var act in searchDetails)
                {
                    var wActivity = new WidgetActivity()
                    {
                        ProductName = act?.Name,
                        Currency = act?.CurrencyIsoCode,
                        Price = act?.BaseMinPrice ?? 0,
                        ProductImage = act?.Images?.FirstOrDefault(x => x.ImageType == ImageType.Search)?.Name,
                        ProductUrl = act?.ActualServiceUrl
                    };
                    widgetResult.Activities.Add(wActivity);
                }
            }

            return GetResponseWithActionResult(widgetResult);
        }

        private List<SearchDetails> GetSearchDetails(SearchRequestCriteria criteria)
        {
            var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 1.0));
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = processorCount
            };

            var affiliate = _activityHelper.GetAffiliateInfoV2(criteria.AffiliateId);
            if (affiliate == null)
                return null;
            var clientInfo = _activityMapper.MapClientInfoForSearch(criteria, affiliate);
            //var stopWatchActivity = new Stopwatch();
            //stopWatchActivity.Start();
            var searchData = _activityHelper.GetSearchDetailsV2(criteria, clientInfo);
            //stopWatchActivity.Stop();
            //var timeActivity = stopWatchActivity.ElapsedMilliseconds;

            //Calculate Prices
            //var stopWatchPrice = new Stopwatch();
            //stopWatchPrice.Start();
            var distinctActivities = searchData?.Activities?.Select(x => x.ID)?.Distinct()?.ToList();
            var prices = _activityHelper.GetSearchAffiliateWiseServiceMinPrice(distinctActivities, affiliate, clientInfo);
            Parallel.ForEach(searchData?.Activities, parallelOptions, item =>
            {
                item.GateBaseMinPrice = prices.Where(x => x.Key == item.ID).Select(x => x.Value.Item1).FirstOrDefault();
                item.BaseMinPrice = item.SellMinPrice = prices.Where(x => x.Key == item.ID).Select(x => x.Value.Item2).FirstOrDefault();
                item.CurrencyIsoCode = prices.Where(x => x.Key == item.ID).Select(x => x.Value.Item3).FirstOrDefault();
            });
            // stopWatchPrice.Stop();
            // var timePrice = stopWatchPrice.ElapsedMilliseconds;

            // Remove Activities whose price is zero
            searchData?.Activities.RemoveAll(x => x.GateBaseMinPrice == 0 && x.BaseMinPrice == 0);
            var AffiliateName = "";
            if (!string.IsNullOrWhiteSpace(affiliate.Title))
            {
                AffiliateName = affiliate.Title.Replace(" ", "_");
            }
            var searchDetails = _activityMapper.MapSearchData(searchData, AffiliateName);

            return searchDetails;
        }

        /// <summary>
        /// Fetch Activity details with Calendar
        /// </summary>
        /// <param name="activityCriteria"></param>
        /// <returns></returns>
        [Route("detailswithcalendar")]
        [Route("{v1?}/detailswithcalendar")]
        [HttpPost]
        [ValidateModel]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetActivityDetailsWithCalendar(ActivityCriteria activityCriteria)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var affiliateId = activityCriteria.AffiliateId;
            if (string.IsNullOrWhiteSpace(affiliateId))
            {
                if (identity != null)
                {
                    var userClaims = identity.Claims;
                    affiliateId = userClaims.FirstOrDefault(o => o.Type == "affiliateId")?.Value;
                    activityCriteria.AffiliateId = affiliateId;

                }
            }
            if (string.IsNullOrEmpty(activityCriteria.TokenId))
            {
                activityCriteria.TokenId = affiliateId;
            }
            var affiliate = _activityHelper.GetAffiliateInfo(activityCriteria.AffiliateId);
            if (affiliate == null) return GetResponseWithActionResult(affiliate);

            var request = new ActivityDetailsWithCalendarRequest
            {
                ActivityId = activityCriteria.ActivityId,
                ClientInfo = _activityMapper.MapClientInfoForActivityDetail(activityCriteria),
                Criteria = _activityHelper.GetDefaultCriteria()
            };
            request.ClientInfo.IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer;
            request.ClientInfo.IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate;
            string B2BAffiliate = "true";
            var activityDetailsWithCalendar = _activityService.GetActivityDetailsWithCalendar(request, B2BAffiliate).Result;
            activityDetailsWithCalendar.Activity = _activityHelper.CalculateActivityWithMinPrices(activityDetailsWithCalendar.Activity);
            var calendarWithDefaultAffiliateIdAvailabilityList = _activityHelper.GetPriceAndAvailabilities(activityCriteria.ActivityId, "default");

            var clientInfo = _activityMapper.MapClientInfoForCalendar(activityCriteria, affiliate);
            var activityWithAvailability = _activityMapper.MapActivityWithCalendarAvailability(activityCriteria.AffiliateId, activityDetailsWithCalendar, calendarWithDefaultAffiliateIdAvailabilityList, affiliate, clientInfo, activityCriteria.CurrencyIsoCode);

            return GetResponseWithActionResult(activityWithAvailability);
        }



        /// <summary>
        /// Get availability of product
        /// </summary>
        /// <param name="checkAvailabilityRequest"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <remarks>
        /// Sample Request:
        ///
        /// POST
        ///
        ///
        ///       {
        ///         &quot;ActivityOptionId&quot;: 145263,
        ///         &quot;PaxDetails&quot;: [{
        ///                 &quot;PassengerTypeId&quot;: 1,
        ///                 &quot;Count&quot;: 2
        ///             }
        ///         ],
        ///         &quot;CheckinDate&quot;: &quot;2022-11-30&quot;,
        ///         &quot;CheckoutDate&quot;: &quot;2022-12-01&quot;
        ///       }
        ///
        ///
        /// Sample Response:
        ///
        ///{
        ///  "ActivityId": 5151,
        ///  "Name": "City Sightseeing Florence: Hop-On, Hop-Off Bus",
        ///  "Description": "Book this Florence Hop-on, Hop-off Tour and explore the city at your own pace with over 20     well-// placed/ stops.",
        ///  "TokenId": "c001bb8e-67f5-475c-8ddf-85141b607f93",
        ///  "Options": [
        ///    {
        ///      "ServiceOptionId": 135471,
        ///      "OptionName": "24 Hours Hop-on Hop-off Ticket - Florence",
        ///      "OptionOrder": 1,
        ///      "Description": "",
        ///      "GateBasePrice": {
        ///        "CurrencyIsoCode": "EUR",
        ///        "PriceAndAvailabilities": [
        ///          {
        ///            "DateAndTime": "2022-11-01T00:00:00",
        ///            "ReferenceId": "a4f21faa-5ee1-43b5-98d5-18a119f114e7",
        ///            "AvailabilityStatus": "AVAILABLE",
        ///            "TotalPrice": 60,
        ///            "UnitType": "PERPERSON",
        ///            "IsCapacityCheckRequired": false,
        ///            "Capacity": 1000,
        ///            "PricingUnits": [
        ///              {
        ///                "PassengerTypeName": "ADULT",
        ///                "PassengerTypeId": 1,
        ///                "Price": 18,
        ///                "Count": 2,
        ///                "IsMinimumSellingPriceRestrictionApplicable": false
        ///              },
        ///              {
        ///                "PassengerTypeName": "CHILD",
        ///                "PassengerTypeId": 2,
        ///                "Price": 12,
        ///                "Count": 2,
        ///                "IsMinimumSellingPriceRestrictionApplicable": false
        ///              }
        ///            ]
        ///          }
        ///        ]
        ///      },
        ///      "StartTime": "00:00:00",
        ///      "EndTime": "00:00:00",
        ///      "CancellationPolicy": "Free cancellation up to 24 hours before the tour/activity. Less than or equal to    24 /   hours /-/ 100% charges"
        ///    },
        ///    {
        ///      "ServiceOptionId": 135472,
        ///      "OptionName": "48 Hours Hop-on Hop-off Ticket - Florence",
        ///      "OptionOrder": 2,
        ///      "Description": "",
        ///      "GateBasePrice": {
        ///        "CurrencyIsoCode": "EUR",
        ///        "PriceAndAvailabilities": [
        ///          {
        ///            "DateAndTime": "2022-11-01T00:00:00",
        ///            "ReferenceId": "efa5a33e-ff5a-4227-aabb-5e810d8b434b",
        ///            "AvailabilityStatus": "AVAILABLE",
        ///            "TotalPrice": 128,
        ///            "UnitType": "PERPERSON",
        ///            "IsCapacityCheckRequired": false,
        ///            "Capacity": 1000,
        ///            "PricingUnits": [
        ///              {
        ///                "PassengerTypeName": "ADULT",
        ///                "PassengerTypeId": 1,
        ///                "Price": 50,
        ///                "Count": 2,
        ///                "IsMinimumSellingPriceRestrictionApplicable": false
        ///              },
        ///              {
        ///                "PassengerTypeName": "CHILD",
        ///                "PassengerTypeId": 2,
        ///                "Price": 14,
        ///                "Count": 2,
        ///                "IsMinimumSellingPriceRestrictionApplicable": false
        ///              }
        ///            ]
        ///          }
        ///        ]
        ///      },
        ///      "StartTime": "00:00:00",
        ///      "EndTime": "00:00:00",
        ///      "CancellationPolicy": "Free cancellation up to 24 hours before the tour/activity. Less than or equal to    24 /   hours /-/ 100% charges"
        ///    }
        ///  ],
        ///  "IsPaxDetailRequired": false,
        ///  "IsPaxDetailRequiredDuringReservation": true,
        ///  "Errors": []
        ///}
        /// </remarks>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        [Route("v1/availability")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetB2CProductAvailability(B2C_CheckAvailabilityRequest checkAvailabilityRequest)
        {
            try
            {
                var affiliateId = checkAvailabilityRequest.AffiliateId;
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    affiliateId = GetAffiliateFromIdentity();
                   checkAvailabilityRequest.AffiliateId = affiliateId;
                }
                if (string.IsNullOrEmpty(checkAvailabilityRequest.TokenId))
                {
                    checkAvailabilityRequest.TokenId = affiliateId;
                }



                ValidateCheckAvailabilityRequest(checkAvailabilityRequest);

                //if (checkAvailabilityRequest.IsBundle)
                //{
                //    GetProductBundleAvailability(checkAvailabilityRequest);
                //}
                var defaultLimit = ConfigurationManagerHelper.GetValuefromAppSettings("CheckAvailabilityDefaultLimit");
                if ((checkAvailabilityRequest.CheckoutDate - checkAvailabilityRequest.CheckinDate).TotalDays <= Convert.ToInt32(defaultLimit))
                {
                    //Process pax where count is > 0
                    if (checkAvailabilityRequest?.PaxDetails != null)
                    {
                        checkAvailabilityRequest.PaxDetails = checkAvailabilityRequest.PaxDetails?.Where(x => x.Count > 0).ToList();
                        if (checkAvailabilityRequest?.PaxDetails?.Select(x => x.PassengerTypeId)?.Distinct()?.ToList()?.Count != checkAvailabilityRequest?.PaxDetails?.Count)
                        {
                            return GetResponseWithActionResult(checkAvailabilityRequest, CommonErrorConstants.ActivityDuplicatePassengers, HttpStatusCode.BadRequest);
                        }
                    }
                    var affiliate = _activityHelper.GetAffiliateInfo(checkAvailabilityRequest?.AffiliateId);
                    if (affiliate == null) return GetResponseWithActionResult(affiliate, CommonErrorConstants.AffiliateNotFound, HttpStatusCode.BadRequest);

                    var clientInfo = _activityHelper.PrepareClientInfoInput(checkAvailabilityRequest);
                    clientInfo.IsSupplementOffer = affiliate?.AffiliateConfiguration?.IsSupplementOffer ?? false;

                    var criteria = new Criteria
                    {
                        // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S2259 // Null pointers should not be dereferenced
                        CheckinDate = checkAvailabilityRequest.CheckinDate.Date,
#pragma warning restore S2259 // Null pointers should not be dereferenced
                        CheckoutDate = checkAvailabilityRequest.CheckoutDate.Date,
                        NoOfPassengers = new Dictionary<PassengerType, int>(),
                        Token = checkAvailabilityRequest?.TokenId ?? checkAvailabilityRequest?.AffiliateId ?? Guid.NewGuid().ToString(),
                        Language = checkAvailabilityRequest.LanguageCode,
                        ActivityId = checkAvailabilityRequest.ActivityId
                    };

                    //var watch = System.Diagnostics.Stopwatch.StartNew();
                    //long time = 0;
                    var activity = _activityHelper.GetProductAvailability(checkAvailabilityRequest, clientInfo, criteria);
                    if (activity?.ProductOptions?.Count > 0 && checkAvailabilityRequest.ActivityOptionId > 0)
                    {
                        activity.ProductOptions = activity?.ProductOptions.Where(x =>
                        x.Id == checkAvailabilityRequest.ActivityOptionId
                        || x.ServiceOptionId == checkAvailabilityRequest.ActivityOptionId
                        )?.ToList();
                    }
                    //watch.Stop();
                    //time = watch.ElapsedMilliseconds;
                    //activity.ApiType;
                    if (activity?.ProductOptions?.Count > 0)
                    {
                        var apiType = activity.ApiType;

                        //watch = System.Diagnostics.Stopwatch.StartNew();
                        activity.ProductOptions = _activityHelper.GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId,
                           activity.ProductOptions, clientInfo, criteria.CheckinDate, apiType, checkAvailabilityRequest.IsQrscan);
                        //watch.Stop();
                        //time = watch.ElapsedMilliseconds;
                        //activity = _activityHelper.CalculateActivityWithMinPrices(activity);
                        if (activity.ProductOptions?.Any(x => x.BasePrice?.DatePriceAndAvailabilty?.Count > 0) == true)
                        {
                            //watch = System.Diagnostics.Stopwatch.StartNew();
                            _TableStorageOperations.InsertData(activity, clientInfo.ApiToken);

                            var availablityReferenceIdsQuery = from po in activity.ProductOptions
                                                               from dpa in po.BasePrice.DatePriceAndAvailabilty
                                                               select dpa.Value.ReferenceId;
                            var availablityReferenceIds = availablityReferenceIdsQuery?.ToList();

                            Task.Run(() =>
                            {
                                _masterPersistence.SaveTokenAndRefIds(checkAvailabilityRequest.TokenId, availablityReferenceIds);
                            });
                            //watch.Stop();
                            //time = watch.ElapsedMilliseconds;
                        }
                        else
                        {
                            return GetResponseWithActionResult(checkAvailabilityRequest, "Activity Option not found", HttpStatusCode.NotFound);
                        }
                    }
                    else
                    {
                        return GetResponseWithActionResult(checkAvailabilityRequest, "Activity Option not found", HttpStatusCode.NotFound);
                    }
                    var checkAvailabilitiesResponse = _activityMapper.MapCheckAvailabilityResponse(activity, criteria, clientInfo.ApiToken);

                    var b2cRes = SerializeDeSerializeHelper.DeSerialize<B2C_CheckAvailabilityResponse>(SerializeDeSerializeHelper.Serialize(checkAvailabilitiesResponse));

                    return GetResponseWithActionResult(b2cRes);
                }
                var message = Constant.DefaultRangeExceed + " " + defaultLimit + Constant.DefaultRangeExceedDays;
                return GetResponseWithActionResult(checkAvailabilityRequest, message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(checkAvailabilityRequest, CommonErrorConstants.UNEXPECTED_ERROR, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get bookable person types as per given date ranges.
        /// </summary>
        /// <param name="productOptionId">int example 135471</param>
        /// <param name="fromDate">Date "yyyy-MM-dd" example "2022-11-20"</param>
        /// <param name="toDate">Date "yyyy-MM-dd" example "2022-11-25"</param>
        /// <param name="activityId">int example 5151, At least one of productOptionId or activityId required </param>
        /// <remarks>
        /// Sample Request:
        ///
        /// GET
        ///
        /// http://{server}/api/activity/v1/personTypes?productOptionId=135471&amp;fromDate=2022-11-20&amp;toDate=2022-11-25
        ///
        /// Sample Response:
        ///
        /// {
        ///     &quot;AvailablePersonTypes&quot;:
        ///     [
        ///         {
        ///             &quot;fromDate&quot;: &quot;2022-09-29T08:37:29.164Z&quot;,
        ///             &quot;toDate&quot;: &quot;2022-09-29T08:37:29.164Z&quot;,
        ///             &quot;personTypes&quot;:
        ///             [
        ///                 {
        ///                     &quot;name&quot;: &quot;string&quot;,
        ///                     &quot;minAge&quot;: 0,
        ///                     &quot;maxAge&quot;: 0,
        ///                     &quot;personTypeId&quot;: 0
        ///                 }
        ///             ]
        ///         }
        ///     ]
        /// }
        /// </remarks>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <returns>Bookable Person Types </returns>
        [Route("v1/personTypes")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
       [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
       // [SwaggerResponse(HttpStatusCode.OK, "Person Types Response Model", typeof(BookablePersonTypesAvailablePersons))]
        public IActionResult GetPersonTypes(string productOptionId, string fromDate, string toDate, string? activityId = null)
        {
            var reqObject = new
            {
                productOptionId,
                fromDate,
                toDate,
                activityId
            };

            try
            {
                DateTime? startDate;
                DateTime? endDate;
                int.TryParse(productOptionId, out var serviceOptionId);
                int.TryParse(activityId, out var serviceId);
                var isValidfromDate = DateTime.TryParse(fromDate, out DateTime st);
                var isValidToDate = DateTime.TryParse(toDate, out DateTime ed);
                var message = string.Empty;

                if (st == default(DateTime))
                {
                    startDate = null;
                }
                else
                {
                    startDate = st;
                }

                if (ed == default(DateTime))
                {
                    endDate = null;
                }
                else
                {
                    endDate = ed;
                }

                if (!(serviceOptionId > 0) && !(serviceId > 0))
                {
                    message = "At least one of valid productOptionId/activityId required.";
                    return GetResponseWithActionResult(reqObject,
                    message, System.Net.HttpStatusCode.BadRequest);
                }

                if (!(isValidfromDate))
                {
                    message = "Please provide valid fromDate.";
                    return GetResponseWithActionResult(reqObject,
                    message, System.Net.HttpStatusCode.BadRequest);
                }
                if (!(isValidToDate))
                {
                    message = "Please provide valid toDate.";
                    return GetResponseWithActionResult(reqObject,
                    message, System.Net.HttpStatusCode.BadRequest);
                }
                var personTypes = _activityHelper.GetPersonTypeOptionCacheAvailability(serviceId, serviceOptionId, startDate, endDate);

                if (!(personTypes?.AvailableDates?.Count > 0))
                {
                    message = $"Bookable passengers' given available dates not found for option {serviceOptionId}.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                if (!(serviceId > 0))
                {
                    serviceId = Convert.ToInt32(personTypes?.AvailableDates?.FirstOrDefault()?.ServiceId);
                }

                if (!(serviceId > 0))
                {
                    message = $"Activity {serviceId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                var activity = _activityService.LoadActivityAsync(serviceId, Convert.ToDateTime(startDate), new ClientInfo { LanguageCode = "en" })?.GetAwaiter().GetResult();

                if (activity == null)
                {
                    message = $"Activity {serviceId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                if (!(activity?.ProductOptions?.Count > 0))
                {
                    message = $"Product option {serviceOptionId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }
                var pos = activity?.ProductOptions;
                if (serviceOptionId > 0)
                {
                    pos = activity?.ProductOptions?.Where(x =>
                        (x.ServiceOptionId == serviceOptionId)
                        || (x.Id == serviceOptionId)

                        )?.ToList();
                }
                if (!(pos?.Count > 0))
                {
                    message = $"Product option {serviceOptionId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                var resultV1 = _activityMapper.GetPersonTypeOptionCacheAvailability(personTypes, activity);
                var result = new BookablePersonTypesAvailablePersons
                {
                    AvailablePersonTypes = resultV1?.bookablePersonTypesInDateRanges?.AvailablePersonTypes
                };

                if (!(result?.AvailablePersonTypes.Count > 0))
                {
                    return GetResponseWithActionResult(reqObject,
                  message, System.Net.HttpStatusCode.NotFound);
                }
                return GetResponseWithActionResult(result);
            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(reqObject, ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get bookable date as per given date ranges.
        /// </summary>
        /// <param name="productOptionId">int example 135471</param>
        /// <param name="fromDate">Date "yyyy-MM-dd" example "2022-11-20"</param>
        /// <param name="toDate">Date "yyyy-MM-dd" example "2022-11-25"</param>
        /// <param name="activityId">int example 5151, At least one of productOptionId or activityId required </param>
        /// <remarks>
        /// Sample Request:
        ///
        /// GET
        ///
        /// http://{server}/api/activity/v1/calendar?productOptionId=135471&amp;fromDate=2022-11-20&amp;toDate=2022-11-25
        ///
        /// Sample Response:
        ///
        /// {
        ///   &quot;AvailableDates&quot;: [
        ///     {
        ///       &quot;AvailableOn&quot;: &quot;2022-11-20T00:00:00&quot;,
        ///       &quot;Capacity&quot;: 1000
        ///     },
        ///     {
        ///       &quot;AvailableOn&quot;: &quot;2022-11-21T00:00:00&quot;,
        ///       &quot;Capacity&quot;: 1000
        ///     },
        ///     {
        ///       &quot;AvailableOn&quot;: &quot;2022-11-22T00:00:00&quot;,
        ///       &quot;Capacity&quot;: 1000
        ///     },
        ///     {
        ///       &quot;AvailableOn&quot;: &quot;2022-11-23T00:00:00&quot;,
        ///       &quot;Capacity&quot;: 1000
        ///     },
        ///     {
        ///       &quot;AvailableOn&quot;: &quot;2022-11-24T00:00:00&quot;,
        ///       &quot;Capacity&quot;: 1005
        ///     },
        ///     {
        ///       &quot;AvailableOn&quot;: &quot;2022-11-25T00:00:00&quot;,
        ///       &quot;Capacity&quot;: 1000
        ///     }
        ///   ]
        /// }
        /// </remarks>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <returns>Bookable Person Types Available Dates</returns>
        [Route("v1/calendar")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[SwaggerResponse(HttpStatusCode.OK, "Available Dates Response Model", typeof(BookablepersontypesAvailableDates))]
        public IActionResult GetCalendarAvailableDates(string productOptionId, string fromDate, string toDate, string? activityId = null)
        {
            string affiliateId = null;
            string tokenId = null;
            if (string.IsNullOrWhiteSpace(affiliateId))
            {
                affiliateId = GetAffiliateFromIdentity();
                tokenId = affiliateId;
            }
            var reqObject = new
            {
                productOptionId,
                fromDate,
                toDate,
                activityId
            };

            try
            {
                DateTime? startDate;
                DateTime? endDate;
                int.TryParse(productOptionId, out var serviceOptionId);
                int.TryParse(activityId, out var serviceId);
                var isValidfromDate = DateTime.TryParse(fromDate, out DateTime st);
                var isValidToDate = DateTime.TryParse(toDate, out DateTime ed);
                var message = string.Empty;

                if (st == default(DateTime))
                {
                    startDate = null;
                }
                else
                {
                    startDate = st;
                }

                if (ed == default(DateTime))
                {
                    endDate = null;
                }
                else
                {
                    endDate = ed;
                }

                if (!(serviceOptionId > 0) && !(serviceId > 0))
                {
                    message = "At least one of valid productOptionId/activityId required.";
                    return GetResponseWithActionResult(reqObject,
                    message, System.Net.HttpStatusCode.BadRequest);
                }

                if (!(isValidfromDate))
                {
                    message = "Please provide valid fromDate.";
                    return GetResponseWithActionResult(reqObject,
                    message, System.Net.HttpStatusCode.BadRequest);
                }
                if (!(isValidToDate))
                {
                    message = "Please provide valid toDate.";
                    return GetResponseWithActionResult(reqObject,
                    message, System.Net.HttpStatusCode.BadRequest);
                }
                var personTypes = _activityHelper.GetPersonTypeOptionCacheAvailability(serviceId, serviceOptionId, startDate, endDate);

                if (!(personTypes?.AvailableDates?.Count > 0))
                {
                    message = $"Bookable passengers' given available dates not found for option {serviceOptionId}.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                if (!(serviceId > 0))
                {
                    serviceId = Convert.ToInt32(personTypes?.AvailableDates?.FirstOrDefault()?.ServiceId);
                }

                if (!(serviceId > 0))
                {
                    message = $"Activity {serviceId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                if (!(serviceId > 0))
                {
                    serviceId = Convert.ToInt32(personTypes?.AvailableDates?.FirstOrDefault()?.ServiceId);
                }

                var activity = _activityService.LoadActivityAsync(serviceId, Convert.ToDateTime(startDate), new ClientInfo { LanguageCode = "en" })?.GetAwaiter().GetResult();

                if (activity == null)
                {
                    message = $"Activity {serviceId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }
                if (!(activity?.ProductOptions?.Count > 0))
                {
                    message = $"Product option {serviceOptionId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }
                var pos = activity?.ProductOptions;
                if (serviceOptionId > 0)
                {
                    pos = activity?.ProductOptions?.Where(x =>
                        (x.ServiceOptionId == serviceOptionId)
                        || (x.Id == serviceOptionId)

                        )?.ToList();
                }
                if (!(pos?.Count > 0))
                {
                    message = $"Product option {serviceOptionId} not found.";
                    return GetResponseWithActionResult(reqObject,
                   message, System.Net.HttpStatusCode.NotFound);
                }

                var resultV1 = _activityMapper.GetPersonTypeOptionCacheAvailability(personTypes, activity);
                var result = new BookablepersontypesAvailableDates
                {
                    AvailableDates = resultV1.bookablePersonTypesInDateRanges.AvailableDates
                };

                if (!(result?.AvailableDates.Count > 0))
                {
                    return GetResponseWithActionResult(reqObject,
                  message, System.Net.HttpStatusCode.NotFound);
                }

                return GetResponseWithActionResult(result);
            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(reqObject, ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Validate and set default data to checkAvailabilityRequest
        /// </summary>
        /// <param name="checkAvailabilityRequest"></param>
        /// <returns></returns>
        private IActionResult ValidateCheckAvailabilityRequest(B2C_CheckAvailabilityRequest checkAvailabilityRequest)
        {
            try
            {
                #region LanguageCode and CurrencyIsoCode Validation

                var languageCode = checkAvailabilityRequest?.LanguageCode;
                var currencyCode = checkAvailabilityRequest?.CurrencyIsoCode;

                if (!string.IsNullOrWhiteSpace(currencyCode) && currencyCode?.Length != 3)
                {
                    return GetResponseWithActionResult(checkAvailabilityRequest, "Please provide valid 3 characters CurrencyIsoCode.", System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    currencyCode = checkAvailabilityRequest.CurrencyIsoCode = "GBP";
                }

                if (!string.IsNullOrWhiteSpace(languageCode) && languageCode?.Length != 2)
                {
                    return GetResponseWithActionResult(checkAvailabilityRequest, "Please provide valid 2 characters LanguageCode.", System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    languageCode = checkAvailabilityRequest.LanguageCode = "en";
                }

                #endregion LanguageCode and CurrencyIsoCode Validation

                #region AffiliateId Validation

                if (string.IsNullOrWhiteSpace(checkAvailabilityRequest.AffiliateId))
                {
                    return GetResponseWithActionResult(checkAvailabilityRequest, CommonErrorConstants.AffiliateNotFound, System.Net.HttpStatusCode.BadRequest);
                }

                #endregion AffiliateId Validation

                #region ActivityId and ActivityOptionId Validation

                if (checkAvailabilityRequest.ActivityOptionId == 0 && checkAvailabilityRequest.ActivityId == 0)
                {
                    return GetResponseWithActionResult(checkAvailabilityRequest, "Please provide at least one  ActivityOptionId Or ActivityId.", HttpStatusCode.BadRequest);
                }

                // Get ActivityId from ActivityOptionId

                if (checkAvailabilityRequest.ActivityOptionId > 0 && checkAvailabilityRequest.ActivityId == 0)
                {
                    checkAvailabilityRequest.ActivityId = _masterPersistence.GetActivityIdByOptionId(checkAvailabilityRequest.ActivityOptionId);
                }
                if (checkAvailabilityRequest.ActivityOptionId > 0 && checkAvailabilityRequest.ActivityId == 0)
                {
                    return GetResponseWithActionResult(checkAvailabilityRequest, "Could not resolve ActivityId from ActivityOptionId.", HttpStatusCode.NotFound);
                }


                #endregion ActivityId and ActivityOptionId Validation

                if (string.IsNullOrWhiteSpace(checkAvailabilityRequest.TokenId))
                {
                    checkAvailabilityRequest.TokenId = checkAvailabilityRequest.AffiliateId ?? Guid.NewGuid().ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        private IActionResult ValidateAvailabilityRequest(CheckAvailabilityRequest checkAvailabilityRequest)
        {
            try
            {
                #region LanguageCode and CurrencyIsoCode Validation

                var languageCode = checkAvailabilityRequest?.LanguageCode ?? "en";
                var currencyCode = checkAvailabilityRequest?.CurrencyIsoCode ?? "GBP";


                if (checkAvailabilityRequest.ActivityId == 0)
                {
                    return GetResponseWithActionResult(checkAvailabilityRequest, "Please provide at least one  ActivityId.", HttpStatusCode.BadRequest);
                }

                // Get ActivityId from ActivityOptionId

                #endregion ActivityId and ActivityOptionId Validation

                if (string.IsNullOrWhiteSpace(checkAvailabilityRequest.TokenId))
                {
                    checkAvailabilityRequest.TokenId = checkAvailabilityRequest.AffiliateId ?? Guid.NewGuid().ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        ///// <summary>
        ///// This operation retrieves availabilities of requested product
        ///// </summary>
        ///// <param name="activityId"></param>
        ///// <param name="AffiliateId"></param>
        ///// <returns></returns>
        [Route("CalendarAvailability/v1/{activityId}")]
        [HttpGet]
        [ValidateModel]
        [SwaggerOperation(Tags = new[] { "Activity Lite" })]
        //[SwaggerResponse(HttpStatusCode.OK, "Calendar Availability", typeof(CalendarResponse_activity))]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCalendarAvailability(int activityId, string? AffiliateId = null)
        {
            var affiliateId = AffiliateId;
            if (string.IsNullOrWhiteSpace(affiliateId))
            {
                affiliateId = GetAffiliateFromIdentity();
                AffiliateId = affiliateId;
            }
            
            var affiliate = _activityHelper.GetAffiliateInfo(affiliateId);
            if (affiliate == null) return GetResponseWithActionResult(affiliate);

            var calendarAvailabilityList = _activityHelper.GetPriceAndAvailabilities(activityId, affiliateId);
            var calendarWithDefaultAffiliateIdAvailabilityList = _activityHelper.GetPriceAndAvailabilities(activityId, "default");

            var calendarResponse = _activityMapper.MapCalendarDetails(activityId, affiliateId, calendarAvailabilityList, calendarWithDefaultAffiliateIdAvailabilityList, affiliate, null, null, "true");
            return GetResponseWithActionResult(calendarResponse);
        }

        [Route("Newsletterdetails")]
        [HttpPost]
        public IActionResult GetNewsletterDetails(ActivityCriteria activityCriteria)
        {
            if (string.IsNullOrEmpty(activityCriteria.TokenId))
            {
                activityCriteria.TokenId = activityCriteria.AffiliateId;
            }

            var affiliate = _activityHelper.GetAffiliateInfo(activityCriteria.AffiliateId);
            if (affiliate == null)
            {
                return GetResponseWithActionResult(affiliate);
            }

            var request = new ActivityDetailsWithCalendarRequest
            {
                ActivityId = activityCriteria.ActivityId,
                ClientInfo = _activityMapper.MapClientInfoForActivityDetail(activityCriteria),
                Criteria = _activityHelper.GetDefaultCriteria()
            };
            request.ClientInfo.IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer;
            request.ClientInfo.IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate;
            string B2BAffiliate = "true";

            var activityDetailsWithCalendar = _activityService.GetActivityDetailsWithCalendar(request, B2BAffiliate).Result;

            activityDetailsWithCalendar.Activity = _activityHelper.CalculateActivityWithMinPrices(activityDetailsWithCalendar.Activity);

            var calendarWithDefaultAffiliateIdAvailabilityList = _activityHelper.GetPriceAndAvailabilities(activityCriteria.ActivityId, "default");

            var clientInfo = _activityMapper.MapClientInfoForCalendar(activityCriteria, affiliate);
            var activityWithAvailability = _activityMapper.MapActivityWithCalendarAvailability(activityCriteria.AffiliateId, activityDetailsWithCalendar, calendarWithDefaultAffiliateIdAvailabilityList, affiliate, clientInfo, activityCriteria.CurrencyIsoCode);

            var activity = _activityService.GetActivityDetailsAsync(activityCriteria.ActivityId, clientInfo, request.Criteria).Result;

            if (activity == null)
            {
                return GetResponseWithActionResult((Activity)null);
            }

            var factor = _masterService.GetConversionFactorAsync(activityWithAvailability.Activity.CurrencyIsoCode, activityCriteria.CurrencyIsoCode).GetAwaiter().GetResult();
            activity.CurrencyIsoCode = activityCriteria.CurrencyIsoCode;

            var activityDetails = _activityMapper.MapNewsletterActivityDetail(activityWithAvailability, activity, factor);
            activityDetails.Images = activityDetails.Images.Select(image => new ProductImage { Name = image.Name.Replace(" ", "%20") }).ToList();

            return GetResponseWithActionResult(activityDetails);
        }

        [Route("TestNetCore")]
        [HttpGet]
        public IActionResult TestNetCore(string PartitionKey)
        {
            var loggingDataList = new List<LoggingData>();

            var availabilityModelResponses = _TableStorageOperations.RetrieveAdapterLoggingData("GetProductAvailability", "Isango", PartitionKey);

            availabilityModelResponses = availabilityModelResponses.Take(200).ToList();
            foreach (var availability in availabilityModelResponses)
            {
                var input = availability.Request;
                string req = null;
               if (input.StartsWith("[") && input.EndsWith("]"))
                {
                     req = input.Substring(1, input.Length - 2);
                }
                var request = SerializeDeSerializeHelper.DeSerialize<CheckAvailabilityRequest>(req);
                if (request.CheckinDate.Date <= DateTime.Now.Date)
                {
                    // If the date is current or before, increase it by 2 days
                    request.CheckinDate = request.CheckinDate.AddDays(2);
                }

                if (request.CheckoutDate.Date <= DateTime.Now.Date)
                {
                    // If the date is current or before, increase it by 2 days
                    request.CheckoutDate = request.CheckoutDate.AddDays(2);
                }

                var coreResponse = GetProductAvailability(request);
                var coreResponseValue = (coreResponse as ObjectResult)?.Value;

                var loggingData = new LoggingData
                {
                    Request = availability.Request,
                    CoreResponse = SerializeDeSerializeHelper.Serialize(coreResponseValue),
                    Oldresponse = availability.Response
                };




                loggingDataList.Add(loggingData);
            }
            using (var package = new ExcelPackage())
            {
                // Add a worksheet to the Excel package
                var worksheet = package.Workbook.Worksheets.Add("LoggingData");

                // Set headers in the first row
                worksheet.Cells[1, 1].Value = "Request";
                worksheet.Cells[1, 2].Value = "CoreResponse";
                worksheet.Cells[1, 3].Value = "OldResponse";

                // Populate data starting from the second row
                int row = 2;
                foreach (var loggingData in loggingDataList)
                {
                    worksheet.Cells[row, 1].Value = loggingData.Request.ToString();
                    worksheet.Cells[row, 2].Value = loggingData.CoreResponse.ToString();
                    worksheet.Cells[row, 3].Value = loggingData.Oldresponse.ToString();
                    row++;
                }

                // Save the Excel package to a MemoryStream
                MemoryStream stream = new MemoryStream(package.GetAsByteArray());

                // Set the position of the stream to the beginning
                stream.Seek(0, SeekOrigin.Begin);

                // Return the Excel file as a FileStreamResult
                return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "LoggingData.xlsx"
                };
            }
                //return GetResponseWithActionResult(loggingDataList);

        }

        


    }
}