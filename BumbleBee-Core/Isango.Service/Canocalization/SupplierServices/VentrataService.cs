using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isango.Service.Contract;
using ServiceAdapters.Ventrata;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Ventrata;
using Logger.Contract;
using Isango.Service.Constants;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CacheManager.Contract;
using Util;

namespace Isango.Service.SupplierServices
{
    public class VentrataService : SupplierServiceBase, ISupplierService
    {
        private readonly IVentrataAdapter _ventrataAdapter;
        private readonly ILogger _log;
        private readonly IMasterService _masterService;
        private readonly IMasterCacheManager _masterCacheManager;
        public VentrataService(IVentrataAdapter ventrataAdapter, ILogger log, IMasterService masterService, IMasterCacheManager masterCacheManager)
        {
            _ventrataAdapter = ventrataAdapter;
            _log = log;
            _masterService = masterService;
            _masterCacheManager = masterCacheManager;
        }

        /// <summary>
        /// Create Ventrata Criteria
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var distinctProductOptionIds = activity.ProductOptions.Select(x => x.Id).ToList();
            //var result = _masterPersistence.GetTourCMSPaxMappings().Where(x => x.APIType == APIType.TourCMS).ToList();
            //var tourCMSPaxMapping = result.Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
            var ventrataCMSPaxMapping = _masterCacheManager.GetVentrataPaxMappings().Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
            var ventrataAvailCriteria = new VentrataAvailabilityCriteria
            {
                ProductId = activity.Id,
                SupplierOptionCodesAndProductIdVsApiOptionIds = new Dictionary<string, List<string>>(),
                NoOfPassengers = criteria.NoOfPassengers,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                Ages = criteria.Ages
                ,
                Token = criteria.Token,
                IsSupplementOffer= clientInfo.IsSupplementOffer,
                VentrataPaxMappings = ventrataCMSPaxMapping
            };

            var ventrataProductOptions = activity.ProductOptions;
            foreach (var thisOption in ventrataProductOptions)
            {
                ventrataAvailCriteria.SupplierOptionCodesAndProductIdVsApiOptionIds.Add(thisOption.Id + "*" + thisOption.SupplierOptionCode + "*" + thisOption.PrefixServiceCode, new List<string>());
            }

            var getVentrataData = _masterService.GetVentrataData();
            var getVentrataBaseURLData = getVentrataData?.Where(x => x.SupplierBearerToken == activity.Code)?.FirstOrDefault()?.BaseURL;
            if (!string.IsNullOrEmpty(getVentrataBaseURLData))
            {
                ventrataAvailCriteria.VentrataBaseURL = getVentrataBaseURLData;
            }
            return ventrataAvailCriteria;
        }

        /// <summary>
        /// Get Ventrata Availability
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var ventrataAvailCriteria = (VentrataAvailabilityCriteria)criteria;
            ventrataAvailCriteria.SupplierBearerToken = activity.Code;

            var ventrataActivities = _ventrataAdapter.GetOptionsForVentrataActivity(ventrataAvailCriteria, token);
            if (ventrataActivities?.FirstOrDefault()?.ProductOptions?.Count > 0)
            {
                return MapActivity(activity, ventrataActivities, criteria);
            }
            else
            {
                var message = Constant.APIActivityOptionsNot + Constant.VentrataAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }
        
        /// <summary>
        /// Map activity from cache with Activity from API. Basically the options.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="activitiesFromAPI"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();

            foreach (var option in activity.ProductOptions)
            {
                var optionsFromApi = activitiesFromAPI.FirstOrDefault().ProductOptions.FindAll(x => ((ActivityOption)x).SupplierOptionCode.Trim() == option.SupplierOptionCode.Trim() &&
                                        ((ActivityOption)x).VentrataProductId.Trim() == option.PrefixServiceCode.Trim());
                if (optionsFromApi != null)
                {
                    foreach (ActivityOption optionAPI in optionsFromApi)
                    {
                        var castedOption = MapActivityOption(optionAPI, option, criteria);
                        castedOption.TravelInfo = option.TravelInfo;
                        castedOption.VentrataBaseURL = optionAPI.VentrataBaseURL;
                        castedOption.VentrataSupplierId = optionAPI.VentrataSupplierId;
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
                ClassName = "VentrataService",
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

