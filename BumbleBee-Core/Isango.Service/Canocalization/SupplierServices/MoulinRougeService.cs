using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.MoulinRouge;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.MoulinRouge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class MoulinRougeService : SupplierServiceBase, ISupplierService
    {
        private readonly IMoulinRougeAdapter _moulinRougeAdapter;
        private readonly ILogger _log;
        public MoulinRougeService(IMoulinRougeAdapter moulinRougeAdapter, ILogger log = null)
        {
            _moulinRougeAdapter = moulinRougeAdapter;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var moulinRougeCriteria = (MoulinRougeCriteria)criteria;
            var mrActivities = _moulinRougeAdapter.GetConvertedActivtyDateAndPrice(moulinRougeCriteria.CheckinDate, moulinRougeCriteria.CheckoutDate, 1, token)
                .Where(x => x.ID == activity.ID).ToList() as List<Activity>;
            if (mrActivities?.Count > 0)
            {
                if (mrActivities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.MoulinRougeAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, mrActivities, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.MoulinRougeAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var mrCriteria = new MoulinRougeCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate
                ,
                Token = criteria.Token
            };
            return mrCriteria;
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
                        option.TravelInfo.NoOfPassengers = activityOptionFromCache.TravelInfo.NoOfPassengers;
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
                ClassName = "MoulinRougeService",
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