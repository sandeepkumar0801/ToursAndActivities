using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Aot;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Aot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class AotService : SupplierServiceBase, ISupplierService
    {
        private readonly IAotAdapter _aotAdapter;
        private readonly ILogger _log;
        public AotService(IAotAdapter aotAdapter,ILogger log= null)
        {
            _aotAdapter = aotAdapter;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var aotCriteria = (AotCriteria)criteria;
            var activities = _aotAdapter.GetDetailedPricingAvailability(aotCriteria, token) as List<Activity>;
            if (activities?.Count > 0)
            {
                if (activities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.AOTAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, activities, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.AOTAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var aotCriteria = new AotCriteria
            {
                ActivityId = activity.ID,
                OptCode = new List<string>(),
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                Token = criteria.Token,
                Language = criteria.Language
            };
            var productOptions = activity.ProductOptions;
            foreach (var productOption in productOptions)
            {
                if (!string.IsNullOrEmpty(productOption.SupplierOptionCode))
                {
                    aotCriteria.OptCode.Add(productOption.SupplierOptionCode);
                }
            }

            return aotCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var activityFromAPI = activitiesFromAPI.FirstOrDefault(act => act.ID == activity.ID);
            if (activityFromAPI != null)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOptionFromCache in activity.ProductOptions)
                {
                    var activityOptionFromAPI = (ActivityOption)activityFromAPI.ProductOptions.FirstOrDefault(
                        x => ((ActivityOption)x).SupplierOptionCode.Trim() == activityOptionFromCache.SupplierOptionCode.Trim());
                    if (activityOptionFromAPI != null)
                    {
                        var option = MapActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
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
                ClassName = "AOTService",
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
    }
}