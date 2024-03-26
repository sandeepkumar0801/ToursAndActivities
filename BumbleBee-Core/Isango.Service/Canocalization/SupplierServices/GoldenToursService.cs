using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GoldenTours;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GoldenTours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class GoldenToursService : SupplierServiceBase, ISupplierService
    {
        private readonly IGoldenToursAdapter _goldenToursAdapter;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;
        public GoldenToursService(IGoldenToursAdapter goldenToursAdapter, IMasterService masterService,ILogger log = null)
        {
            _goldenToursAdapter = goldenToursAdapter;
            _masterService = masterService;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var goldenToursCriteria = (GoldenToursCriteria)criteria;
            var activities = _goldenToursAdapter.GetProductDetails(goldenToursCriteria, token);
            if (activities?.Count > 0)
            {
                if (activities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.GoldenTourAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, activities, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.GoldenTourAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var passengerMapping = _masterService.GetPassengerMapping(APIType.Goldentours).GetAwaiter().GetResult();
            var queryPaxMapping = from pm in passengerMapping
                                  from cm in criteria?.NoOfPassengers
                                  where Convert.ToInt32(cm.Key) == Convert.ToInt32(pm.PassengerTypeId)
                                  select pm;
            var supplierOptionCodes = activity.ProductOptions?
                .Where(y => !string.IsNullOrWhiteSpace(y.SupplierOptionCode))?
                .Select(x => x.SupplierOptionCode)?
                .Distinct().ToList();
            if (queryPaxMapping.Any())
            {
                passengerMapping = queryPaxMapping.ToList();
            }

            if (supplierOptionCodes.Any())
            {
                var goldenToursCriteria = new GoldenToursCriteria
                {
                    CheckinDate = criteria.CheckinDate,
                    CheckoutDate = criteria.CheckoutDate,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages,
                    PassengerInfo = criteria.PassengerInfo,
                    PassengerMappings = passengerMapping,
                    SupplierOptionCodes = supplierOptionCodes
                    ,
                    Token = criteria.Token
                };
                return goldenToursCriteria;
            }
            return null;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitesFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();
            foreach (var activityOption in activitesFromAPI.FirstOrDefault().ProductOptions)
            {
                var activityOptionFromAPI = (ActivityOption)activityOption;
                var activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                    x => (x).SupplierOptionCode.Trim() == activityOptionFromAPI.SupplierOptionCode.Trim());
                if (activityOptionFromCache == null) continue;

                var option = MapActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
                option.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                option.PickupLocations = activityOptionFromAPI.PickupLocations;
                productOptions.Add(option);
            }
            activity.ProductOptions = productOptions;
            if (activity.ProductOptions.Count == 0)
            {
                activity.ProductOptions = activitesFromAPI.FirstOrDefault().ProductOptions;
            }
            return activity;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "GoldenToursService",
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