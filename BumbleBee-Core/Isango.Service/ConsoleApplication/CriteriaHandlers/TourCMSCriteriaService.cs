using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.TourCMSCriteria;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.TourCMS;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class TourCMSCriteriaService : ITourCMSCriteriaService
    {
        private readonly ITourCMSAdapter _tourCMSAdapter;
        private readonly IMasterService _masterService;
        private readonly IActivityPersistence _activityPersistence;
        private readonly ILogger _log;

        public TourCMSCriteriaService(ITourCMSAdapter tourCMSAdapter, ILogger log,
            IActivityService activityService,
            IMasterService masterService, 
            IActivityPersistence activityPersistence)
        {
            _tourCMSAdapter = tourCMSAdapter;
            _masterService = masterService;
            _activityPersistence = activityPersistence;
            _log = log;
        }

        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            List<Activity> activitiesList = new List<Activity>();

            try
            {
                foreach (IsangoHBProductMapping mappedProduct in criteria.MappedProducts)
                {
                    try
                    {

                        //isangoOptionId_tourCMSId_(ChannelId_AccountId)
                        var tourCMSOptionCodeAndProductId =
                            mappedProduct.ServiceOptionInServiceid
                            + "_" + mappedProduct.HotelBedsActivityCode
                            + "_" + mappedProduct.PrefixServiceCode;

                        var tourCMSAvailCriteria = new TourCMSCriteria()
                        {
                            ProductId = Convert.ToString(mappedProduct.IsangoHotelBedsActivityId),
                            SupplierOptionCodesAndProductIdVsApiOptionIds = new Dictionary<string, List<string>>(),
                            CheckinDate = DateTime.Now.Date,
                            Token = criteria.Token,
                            CheckoutDate = DateTime.Now.AddDays(criteria.Days2Fetch * criteria.Months2Fetch).Date,
                            CommissionPercent = mappedProduct.MarginAmount,
                            IsCommissionPercent = mappedProduct.IsMarginPercent
                        };
                        tourCMSAvailCriteria.NoOfPassengers = new Dictionary<Entities.Enums.PassengerType, int>();
                        tourCMSAvailCriteria.NoOfPassengers.Add(Entities.Enums.PassengerType.Adult, mappedProduct.MinAdultCount);
                        tourCMSAvailCriteria.SupplierOptionCodesAndProductIdVsApiOptionIds.Add(tourCMSOptionCodeAndProductId, new List<string>());
                        List<Activity> activities = _tourCMSAdapter.GetOptionsForTourCMSActivity(tourCMSAvailCriteria, tourCMSAvailCriteria.Token);
                        if (activities != null && activities.Count > 0)
                        {
                            activities[0].ID = mappedProduct.IsangoHotelBedsActivityId;
                            activitiesList.Add(activities[0]);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return activitiesList;
        }


        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name = "activities" ></ param >
        /// < param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            try
            {
                var serviceDetails = new List<ServiceAvailability.TempHBServiceDetail>();
                foreach (var activity in activities)
                {
                    if (activity == null) continue;
                    var mappedProductsById = mappedProducts.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)).ToList();
                  
                    if (mappedProductsById?.Count <= 0 || mappedProductsById == null) continue;
                    var serviceMapper = new ServiceMapper();
                    try
                    {
                        var details = serviceMapper.ProcessServiceDetailsWithBasePrice(activity, mappedProductsById);
                        if (details != null)
                            serviceDetails.AddRange(details);
                    }
                    catch (Exception ex)
                    {
                        //ignored - probably wrong supplier code
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "TourCMSCriteriaService",
                            MethodName = "GetServiceDetails"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }

                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoldenToursCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

    }
}