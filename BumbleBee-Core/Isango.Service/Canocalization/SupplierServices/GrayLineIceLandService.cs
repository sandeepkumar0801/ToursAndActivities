using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GrayLineIceLand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class GrayLineIceLandService : SupplierServiceBase, ISupplierService
    {
        private readonly IGrayLineIceLandAdapter _grayLineIceLandAdapter;
        private readonly IMemCache _memCache;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;
        public GrayLineIceLandService(IGrayLineIceLandAdapter grayLineIceLandAdapter, IMemCache memCache,
            IMasterService masterService, ILogger log = null)
        {
            _grayLineIceLandAdapter = grayLineIceLandAdapter;
            _memCache = memCache;
            _masterService = masterService;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var grayLineIceLandCriteria = (GrayLineIcelandCriteria)criteria;
            var gliActivities = _grayLineIceLandAdapter.GetAvailabilityAndPrice(grayLineIceLandCriteria, token);
            if (gliActivities?.Count > 0)
            {
                if (gliActivities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.GrayLineIcelandAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, gliActivities, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.GrayLineIcelandAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var gliCriteria = new GrayLineIcelandCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                ActivityCode = activity.Code,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate.AddDays(1),
                Language = _memCache.GetMappedLanguage().Find(x => x.IsangoLanguageCode.ToLowerInvariant().Equals(clientInfo.LanguageCode.ToLowerInvariant()))?.GliLanguageCode.ToString(),
                PaxAgeGroupIds = GetGLIPaxAgeGroupIdsAsync(activity.ID, activity.ApiType)?.GetAwaiter().GetResult(),
                 
                Token = criteria.Token
            };
            return gliCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> gliActivities, Criteria criteria)
        {
            var gliActivity = gliActivities.FirstOrDefault();

            // Will get only option for the GLI product so updating the optionId of the API option from the DB option
            gliActivity.ProductOptions.FirstOrDefault().Id = activity.ProductOptions.FirstOrDefault().Id;
            gliActivity.ProductOptions.FirstOrDefault().ServiceOptionId = activity.ProductOptions.FirstOrDefault().Id;
            activity.ProductOptions = UpdateBasePrices(gliActivity.ProductOptions, activity.HotelPickUpLocation, criteria);
            return activity;
        }

        private async Task<Dictionary<PassengerType, int>> GetGLIPaxAgeGroupIdsAsync(int activityId, APIType apiType)
        {
            var paxAgeGroupIds = new Dictionary<PassengerType, int>();
            var ageGroups = _masterService.GetGLIAgeGroupAsync(activityId, apiType)?.GetAwaiter().GetResult();

            var adultAgeGroup = ageGroups?.Where(x => x.PassengerType == PassengerType.Adult).FirstOrDefault();

            if (adultAgeGroup != null)
            {
                paxAgeGroupIds.Add(PassengerType.Adult, adultAgeGroup.AgeGroupId);
            }

            var childAgeGroup = ageGroups?.Where(x => x.PassengerType == PassengerType.Child).FirstOrDefault();
            if (childAgeGroup != null)
            {
                paxAgeGroupIds.Add(PassengerType.Child, childAgeGroup.AgeGroupId);
            }

            var youthAgeGroup = ageGroups?.Where(x => x.PassengerType == PassengerType.Youth).FirstOrDefault();
            if (youthAgeGroup != null)
            {
                paxAgeGroupIds.Add(PassengerType.Youth, youthAgeGroup.AgeGroupId);
            }

            return await Task.FromResult(paxAgeGroupIds);
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "GrayLineIceLandService",
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