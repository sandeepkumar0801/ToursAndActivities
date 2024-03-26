using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Prio;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.PrioTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class PrioService : SupplierServiceBase, ISupplierService
    {
        private readonly IPrioTicketAdapter _prioTicketAdapter;
        private readonly ILogger _log;
        public PrioService(IPrioTicketAdapter prioTicketAdapter, ILogger log = null)
        {
            _prioTicketAdapter = prioTicketAdapter;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var prioCriteria = (PrioCriteria)criteria;
            var prioActivities = _prioTicketAdapter.UpdateOptionforPrioActivity(prioCriteria, token);
            if (prioActivities.FirstOrDefault()?.ProductOptions != null)
            {
                if (prioActivities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.PRIOAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, prioActivities, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.PRIOAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var prioCriteria = new PrioCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                SupplierOptionCodes = new List<string>()
                ,
                Token = criteria.Token
            };
            var prioActivityOption = activity.ProductOptions;
            foreach (var item in prioActivityOption)
            {
                prioCriteria.SupplierOptionCodes.Add(item.SupplierOptionCode);
            }
            return prioCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();

            foreach (var option in activitiesFromAPI?.FirstOrDefault()?.ProductOptions)
            {
                var optionFromCache = activity.ProductOptions.FirstOrDefault(x => x.SupplierOptionCode.Trim() == option.SupplierOptionCode.Trim());
                if (optionFromCache != null)
                {
                    var castedOption = MapActivityOption((ActivityOption)option, optionFromCache, criteria);
                    castedOption.TravelInfo = option.TravelInfo;
                    castedOption.Id = option.Id ==0 ? optionFromCache.Id : option.Id;
                    castedOption.Name = optionFromCache.Name;
                    productOptions.Add(castedOption);
                }
            }
            activity.ProductOptions = productOptions;
            return activity;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "PrioService",
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