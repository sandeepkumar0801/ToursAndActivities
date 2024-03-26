using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Rayna;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Rayna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class RaynaService : SupplierServiceBase, ISupplierService
    {
        private readonly IRaynaAdapter _raynaAdapter;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;

        public RaynaService(IRaynaAdapter raynaAdapter, IMasterService masterService, ILogger log = null)
        {
            _raynaAdapter = raynaAdapter;
            _masterService = masterService;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var lstActivity = new List<Activity>();
            var raynaCriteria = (RaynaCriteria)criteria;
            string request = "";
            string response = "";
            var act = _raynaAdapter.GetActivity(raynaCriteria, token, out request,out response);
            if (act != null)
            {
                lstActivity.Add(act);
            }
            var message = $"{Util.ErrorMessages.ACTIVITY_OPTION_NOT_FOUND_FROM_API} {Constant.BokunAPI} Id: {activity.ID}.";
            if (lstActivity != null && lstActivity.Count>0)
            {
                if (act?.ProductOptions == null)
                {
                    UpdateError(activity, criteria, message);
                }
                
                return MapActivity(activity, lstActivity, criteria);
            }
            else
            {
                UpdateError(activity, criteria, message);
            }

            return lstActivity.FirstOrDefault();
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var poSupplierOptionCode = activity.ProductOptions.Where(x=>!String.IsNullOrEmpty(x.SupplierOptionCode));
            var supplierCode = poSupplierOptionCode?.FirstOrDefault()?.SupplierOptionCode;

            
            var supplierOptionsIds= poSupplierOptionCode.Select(x => x.PrefixServiceCode).ToList();

            var raynacriteria = new RaynaCriteria
            {
                ActivityId =Convert.ToString(activity.ID),
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                CurrencyIsoCode = activity.CurrencyIsoCode,
                Token = criteria.Token ?? clientInfo.ApiToken,
                Language = criteria.Language,
                TourId = Convert.ToInt32(supplierCode),
                ModalityCode = activity?.Code,
                IsCalendarDumping=false,
                SupplierOptionIds= supplierOptionsIds
            };
            //make rayna hit only for 1 days:because there is code performance issue 
            raynacriteria.CheckoutDate = raynacriteria.CheckinDate;

            raynacriteria.ProductMapping = new List<IsangoHBProductMapping>();
            var dataMapping = new IsangoHBProductMapping
            {
                IsangoHotelBedsActivityId = activity.ID,
                HotelBedsActivityCode= supplierCode+"_"+ activity?.Code,
                ApiType=Entities.Enums.APIType.Rayna
            };
            
            raynacriteria.ProductMapping.Add(dataMapping);

            return raynacriteria;
        }

        public Activity MapActivity(Activity activity, 
            List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var activityFromAPI = activitiesFromAPI.FirstOrDefault(act => act.ID == activity.ID);
            activity.Errors = activityFromAPI.Errors;
            if (activityFromAPI?.ProductOptions?.Count > 0)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOption in activityFromAPI.ProductOptions)
                {
                    var activityOptionFromAPI = (ActivityOption)activityOption;
                    var activityOptionFromCache = new ProductOption();

                    if (activity.ProductOptions.Any(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode)))
                    {
                        activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                        x => (x).PrefixServiceCode?.Trim() == activityOptionFromAPI.PrefixServiceCode?.Trim());
                    }
                    if (activityOptionFromCache != null)
                    {
                        var option = MapActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);

                        option.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                        option.Name =option.Name?.Trim() + " " + activityOptionFromAPI?.Name;
                        option.SupplierOptionCode = activityOptionFromAPI?.SupplierOptionCode;
                        option.PrefixServiceCode = activityOptionFromAPI?.PrefixServiceCode;
                        option.RateKey = activityOptionFromAPI?.RateKey;
                        option.TimeSlotId = activityOptionFromAPI.TimeSlotId;
                        option.StartTime = activityOptionFromAPI.StartTime;
                        option.Variant = activityOptionFromAPI.Variant;
                        productOptions.Add(option);
                    }
                }
                activity.ProductOptions = productOptions;

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
                ClassName = "RaynaService",
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