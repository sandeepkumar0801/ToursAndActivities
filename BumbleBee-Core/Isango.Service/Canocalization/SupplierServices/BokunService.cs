using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Bokun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class BokunService : SupplierServiceBase, ISupplierService
    {
        private readonly IBokunAdapter _bokunAdapter;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;

        public BokunService(IBokunAdapter bokunAdapter, IMasterService masterService, ILogger log = null)
        {
            _bokunAdapter = bokunAdapter;
            _masterService = masterService;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var bokunCriteria = (BokunCriteria)criteria;
            if (bokunCriteria.FactSheetIds.Count > 0)
            {
                var activities = _bokunAdapter.CheckAvailabilities(bokunCriteria, activity, token);
                var message = $"{Util.ErrorMessages.ACTIVITY_OPTION_NOT_FOUND_FROM_API} {Constant.BokunAPI} Id: {activity.ID}.";
                if (activities?.Count > 0)
                {
                    if (activities?.FirstOrDefault()?.ProductOptions == null)
                    {
                        UpdateError(activity, criteria, message);
                    }
                    return MapActivity(activity, activities, criteria);
                }
                else
                {
                    UpdateError(activity, criteria, message);
                }
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var bokunCriteria = new BokunCriteria
            {
                ActivityId = activity.ID,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                CurrencyIsoCode = activity.CurrencyIsoCode,
                FactSheetIds = new List<int>(),
                Token = criteria.Token ?? clientInfo.ApiToken,
                Language = criteria.Language
            };

            var mps = _masterService.GetBokunPriceCategoryByActivityAsync().GetAwaiter().GetResult()
                               .ToList().Where(x => x.ServiceId == activity.ID).ToList();
            var query = from mp in mps
                        from pg in criteria.NoOfPassengers
                        where mp.PassengerTypeId == pg.Key
                              && mp.ServiceId == activity.ID
                        select mp;
            bokunCriteria.PriceCategoryIdMapping = query.ToList();

            bokunCriteria.AllPriceCategoryIdMapping = mps;

            var bokunActivityOption = activity.ProductOptions;
            foreach (var productOption in bokunActivityOption)
            {
                try
                {
                    int.TryParse(productOption?.SupplierOptionCode?.Trim(), out var apiActivityCode);
                    if (!bokunCriteria.FactSheetIds.Contains(apiActivityCode))
                    {
                        bokunCriteria.ActivityCode = apiActivityCode.ToString();
                        bokunCriteria.FactSheetIds.Add(apiActivityCode);
                    }
                }
                catch (Exception ex)
                {
                    // logging required
                }
            }
            return bokunCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var bokunCriteria = (BokunCriteria)criteria;
            var activityFromAPI = activitiesFromAPI.FirstOrDefault(act => act.ID == activity.ID);
            activity.Errors = activityFromAPI.Errors;
            if (activityFromAPI?.ProductOptions?.Count > 0)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOption in activityFromAPI.ProductOptions)
                {
                    var activityOptionFromAPI = (ActivityOption)activityOption;
                    var activityOptionFromCache = new ProductOption();

                    //var checkForSingleMultiplePax = bokunCriteria.AllPriceCategoryIdMapping?.GroupBy(x => x.PassengerTypeId)?.ToList();

                    try
                    {
                        if (activityOptionFromAPI?.Id != null && activityOptionFromAPI?.Id != 0)
                        {
                            activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                            x => x.Id == activityOptionFromAPI.Id);
                        }
                        else if (activity.ProductOptions.Any(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode)))
                        {
                            activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                            x => (x).PrefixServiceCode?.Trim() == activityOptionFromAPI.RateKey?.Trim());
                        }
                        else
                        {
                            activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                            x => (x).SupplierOptionCode.Trim() == activityOptionFromAPI.SupplierOptionCode.Trim());
                        }
                    }
                    catch (Exception ex)
                    {
                        if (activity.ProductOptions.Any(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode)))
                        {
                            activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                            x => (x).PrefixServiceCode?.Trim() == activityOptionFromAPI.RateKey?.Trim());
                        }
                        else
                        {
                            activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                            x => (x).SupplierOptionCode.Trim() == activityOptionFromAPI.SupplierOptionCode.Trim());
                        }
                    }


                    if (activityOptionFromCache != null)
                    {
                        var option = MapActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
                        option.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                        productOptions.Add(option);
                    }
                }
                activity.ProductOptions = productOptions;
                activity.ProductOptions.OrderBy(x => ((ActivityOption)x).RateKey);

                if (activity.ProductOptions.Count == 0)
                {
                    activity.ProductOptions = activityFromAPI.ProductOptions;
                }
            }
            return activity;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "BokunService",
                MethodName = "GetAvailability",
                Params = $"{activityId}"
            };
            _log.Error(isangoErrorEntity, new Exception(message));
            var data = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                ReasonPhrase = message
            };
            throw new HttpResponseException(data);
        }

        private void UpdateError(Activity activity, Criteria criteria, string message)
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
            if (activity?.Errors?.Any(x => x?.Code?.ToUpper() == Util.ErrorCodes.AVAILABILITY_ERROR?.ToUpper()) == false)
            {
                activity.Errors.Add(new Error
                {
                    Code = Util.ErrorCodes.AVAILABILITY_ERROR,
                    HttpStatus = System.Net.HttpStatusCode.NotFound,
                    Message = message
                });
            }
        }
    }
}