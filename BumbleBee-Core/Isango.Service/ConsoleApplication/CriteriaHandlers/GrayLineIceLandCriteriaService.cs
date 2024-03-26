using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GrayLineIceLand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class GrayLineIceLandCriteriaService : IGrayLineIceLandCriteriaService
    {
        private readonly IMasterService _masterService;
        private readonly IGrayLineIceLandAdapter _grayLineIceLandAdapter;
        private readonly ILogger _log;

        public GrayLineIceLandCriteriaService(IMasterService masterService, IGrayLineIceLandAdapter grayLineIceLandAdapter, ILogger logger)
        {
            _masterService = masterService;
            _grayLineIceLandAdapter = grayLineIceLandAdapter;
            _log = logger;
        }

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <param name="serviceCriteria"></param>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria serviceCriteria)
        {
            var lstActivity = new List<Activity>();
            try
            {
                var counter = serviceCriteria?.Counter;

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                if (!(counter > 0)) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

                for (var i = 1; i <= counter; i++)
                {
                    for (var start = DateTime.Now.AddDays(serviceCriteria.Days2Fetch * (i - 1)).Date; start.Date < DateTime.Now.AddDays(serviceCriteria.Days2Fetch * i).Date; start = start.AddDays(serviceCriteria.Days2Fetch).Date)
                    {
                        var mappedProducts = serviceCriteria.MappedProducts;
                        foreach (var item in mappedProducts)
                        {
                            try
                            {
                                var criteria = PrepareCriteria(item, serviceCriteria, start);
                                var activities = GetAvailabilities(item, criteria, serviceCriteria);
                                if (activities == null) continue;
                                lstActivity.AddRange(activities);
                            }
                            catch (Exception ex)
                            {
                                Task.Run(() =>
                                    _log.Error(new IsangoErrorEntity
                                    {
                                        ClassName = "GrayLineIceLandCriteriaService",
                                        MethodName = "GetAvailability",
                                        Token = serviceCriteria.Token,
                                        Params = Util.SerializeDeSerializeHelper.Serialize(item)
                                    }, ex)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandCriteriaService",
                    MethodName = "GetAvailability",
                    Token = serviceCriteria.Token
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
            return lstActivity;
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            var serviceDetails = new List<ServiceAvailability.TempHBServiceDetail>();
            try
            {
                //Not added null check in this method as it is already added in the parent method
                foreach (var activity in activities)
                {
                    try
                    {
                        if (activity == null) continue;
                        var mappedProduct = mappedProducts?.FirstOrDefault(x => x.HotelBedsActivityCode.Equals(activity.Code));

                        if (mappedProduct == null) continue;
                        var serviceMapper = new ServiceMapper();
                        var details = serviceMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProduct);
                        serviceDetails.AddRange(details);
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() =>
                            _log.Error(new IsangoErrorEntity
                            {
                                ClassName = "GrayLineIceLandCriteriaService",
                                MethodName = "GetServiceDetails",
                                Params = Util.SerializeDeSerializeHelper.Serialize(activity)
                            }, ex)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return serviceDetails;
        }

        #region Private Methods

        private Criteria PrepareCriteria(IsangoHBProductMapping item, ServiceAvailability.Criteria serviceCriteria, DateTime start)
        {
            var ageGroups = _masterService.GetGLIAgeGroupAsync(item.IsangoHotelBedsActivityId, APIType.Graylineiceland)
                .GetAwaiter().GetResult();
            var noOfPassengers = ageGroups.Select(x => x.PassengerType).Distinct()
                .ToDictionary(x => x, x => item.MinAdultCount);
            var ageGroupIds = ageGroups.ToDictionary(x => x.PassengerType, x => x.AgeGroupId);

            var criteria = new GrayLineIcelandCriteria
            {
                NoOfPassengers = noOfPassengers,
                Ages = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Child, 0 }
                },
                ActivityCode = item.HotelBedsActivityCode,
                CheckinDate = start,
                CheckoutDate = start.AddDays(serviceCriteria.Days2Fetch),
                Language = "0",
                PaxAgeGroupIds = ageGroupIds
            };

            return criteria;
        }

        private List<Activity> GetAvailabilities(IsangoHBProductMapping item, Criteria criteria, ServiceAvailability.Criteria serviceCriteria)
        {
            var gliCriteria = criteria as GrayLineIcelandCriteria;

            var activities = _grayLineIceLandAdapter.GetAvailabilityAndPrice(gliCriteria, serviceCriteria.Token);
            if (activities == null || activities?.Count == 0) return null;

            var lstActivity = new List<Activity>();
            if (activities.FirstOrDefault()?.ProductOptions != null)
            {
                activities.ForEach(x =>
                {
                    x.ID = item.IsangoHotelBedsActivityId;
                    x.ProductOptions.ForEach(y => y.Id = item.ServiceOptionInServiceid);
                    x.CurrencyIsoCode = "ISK";
                    x.PriceTypeId = (PriceTypeId)item.PriceTypeId;
                });
            }
            lstActivity.AddRange(activities);
            return lstActivity;
        }

        #endregion Private Methods
    }
}