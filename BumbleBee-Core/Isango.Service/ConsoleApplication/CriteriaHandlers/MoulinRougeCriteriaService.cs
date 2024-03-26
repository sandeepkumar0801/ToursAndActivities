using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.MoulinRouge;
using System;
using System.Collections.Generic;
using System.Linq;
using Activity = Isango.Entities.Activities.Activity;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class MoulinRougeCriteriaService : IMoulinRougeCriteriaService
    {
        private readonly IMoulinRougeAdapter _moulinRougeAdapter;
        private readonly ILogger _log;

        public MoulinRougeCriteriaService(IMoulinRougeAdapter moulinRougeAdapter, ILogger logger)
        {
            _moulinRougeAdapter = moulinRougeAdapter;
            _log = logger;
        }

        /// <summary>
        /// Get Moulin Rouge Availability
        /// </summary>
        /// <param name="serviceCriteria"></param>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria serviceCriteria)
        {
            try
            {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                if (serviceCriteria == null) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

                var activities = new List<Activity>();

                for(int i = 1; i <= serviceCriteria.Counter; i++)
                {
                    var activities_temp = _moulinRougeAdapter.GetConvertedActivtyDateAndPrice(DateTime.Now.AddDays(serviceCriteria.Days2Fetch * (i-1)).Date, DateTime.Now.Date.AddDays(serviceCriteria.Days2Fetch * i), 1, serviceCriteria.Token);
                    activities.AddRange(activities_temp);
                }
                

                var mappedProducts = serviceCriteria.MappedProducts;
                foreach (var activity in activities)
                {
                    var priceTypeId = (PriceTypeId)mappedProducts.FirstOrDefault(x => x.IsangoHotelBedsActivityId == activity.ID).PriceTypeId;
                    activity.PriceTypeId = priceTypeId;
                }
                return activities;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MoulinRougeCriteriaService",
                    MethodName = "GetAvailability",
                    Token = serviceCriteria.Token
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            try
            {
                var serviceDetails = new List<ServiceAvailability.TempHBServiceDetail>();

                //Not added null check in this method as it is already added in the parent method
                foreach (var activity in activities)
                {
                    if (activity == null) continue;
                    var mappedProduct = mappedProducts?.Where(x => x.FactSheetId.Equals(activity.FactsheetId)).ToList();

                    if (mappedProduct == null) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProduct);
                    serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MoulinRougeCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}