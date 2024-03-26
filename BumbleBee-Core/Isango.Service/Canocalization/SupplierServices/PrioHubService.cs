using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Prio;
using Isango.Entities.PrioHub;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.PrioHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class PrioHubService : SupplierServiceBase, ISupplierService
    {
        private readonly IPrioHubAdapter _prioHubAdapter;
        private readonly ILogger _log;
        public PrioHubService(IPrioHubAdapter prioHubAdapter, ILogger log = null)
        {
            _prioHubAdapter = prioHubAdapter;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var prioActivities = new List<Activity>();
            var prioCriteria = (PrioHubCriteria)criteria;

            var supplierOptionWithDistributerId = prioCriteria?.SupplierOptionCodesWithDistributerId;

            //var mappedoptionsGroupBy = supplierOptionWithDistributerId?.GroupBy(u => u.Value)?.Select(grp => grp.ToList())?.ToList();
            if (supplierOptionWithDistributerId != null)
            {
                // foreach (var mappedoption in mappedoptionsGroupBy)
                foreach(KeyValuePair<string, int> entry in supplierOptionWithDistributerId)
                {
                    
                    prioCriteria.DistributorId = entry.Value;
                    prioCriteria.SupplierOptionCodes = new List<string>();
                    prioCriteria.SupplierMultipleCodes = string.Empty;
                    prioCriteria.SupplierOptionCodes.Add(entry.Key);
                    prioCriteria.SupplierMultipleCodes = string.Join(",", prioCriteria.SupplierOptionCodes);
                    prioActivities.AddRange(_prioHubAdapter.UpdateOptionforPrioHubActivity(prioCriteria, token));
                }
            }
            if (prioActivities.FirstOrDefault()?.ProductOptions != null)
            { 
                if (prioActivities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.PRIOHUBAPI + " .Id:" + activity.ID;
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
            var prioHubCriteria = new PrioHubCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                SupplierOptionCodes = new List<string>(),
                SupplierOptionCodesWithDistributerId = new Dictionary<string, int>(),
                Token = criteria.Token,
                Language = criteria.Language
            };
            var prioActivityOption = activity.ProductOptions;
            foreach (var item in prioActivityOption)
            {
                if (!prioHubCriteria.SupplierOptionCodesWithDistributerId.ContainsKey(item.SupplierOptionCode))
                {
                    if (!string.IsNullOrEmpty(item.SupplierOptionCode) &&!string.IsNullOrEmpty(item.PrefixServiceCode))
                    {
                        prioHubCriteria.SupplierOptionCodesWithDistributerId.Add(item.SupplierOptionCode, Convert.ToInt32(item.PrefixServiceCode));
                    }
                }
            }
            prioHubCriteria.IsangoActivityId = activity.Id;
            return prioHubCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();
            foreach (var activitySingle in activitiesFromAPI)
            {
                foreach (var option in activitySingle?.ProductOptions)
                {
                    var optionFromCache = activity.ProductOptions.FirstOrDefault(x => x.SupplierOptionCode.Trim() == option.SupplierOptionCode.Trim());
                    if (optionFromCache != null)
                    {
                        var castedOption = MapActivityOption((ActivityOption)option, optionFromCache, criteria);
                        castedOption.TravelInfo = option.TravelInfo;
                        castedOption.Id = option.Id == 0 ? optionFromCache.Id : option.Id;
                        castedOption.Name = (option.Name == string.Empty) ? (optionFromCache?.Name?.Trim()) : optionFromCache?.Name?.Trim() + " " + option?.Name?.Trim();
                        castedOption.PickUpPointForPrioHub = ((ActivityOption)option)?.PickUpPointForPrioHub;
                        castedOption.PrioHubProductTypeStatus = ((ActivityOption)option).PrioHubProductTypeStatus;
                        castedOption.PrioHubProductGroupCode = ((ActivityOption)option).PrioHubProductGroupCode;
                        castedOption.PrioHubProductPaxMapping = ((ActivityOption)option).PrioHubProductPaxMapping;
                        castedOption.ProductCombiDetails = ((ActivityOption)option).ProductCombiDetails;
                        castedOption.PrioHubComboSubProduct = ((ActivityOption)option).PrioHubComboSubProduct;
                        castedOption.PrioHubDistributerId = ((ActivityOption)option)?.PrioHubDistributerId;
                        castedOption.Cluster = ((ActivityOption)option).Cluster;

                        castedOption.CancellationText = ((ActivityOption)option)?.CancellationText;
                        castedOption.Cancellable =Convert.ToBoolean(((ActivityOption)option).Cancellable);
                        castedOption.ApiCancellationPolicy = ((ActivityOption)option)?.ApiCancellationPolicy;
                        castedOption.IsIsangoMarginApplicable = optionFromCache?.IsIsangoMarginApplicable ?? false;

                        productOptions.Add(castedOption);
                    }
                }
            }
            activity.ProductOptions = productOptions;
            return activity;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "PrioHubService",
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