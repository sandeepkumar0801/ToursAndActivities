using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Prio;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.PrioTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Activity = Isango.Entities.Activities.Activity;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class PrioCriteriaService : IPrioCriteriaService
    {
        private readonly IPrioTicketAdapter _prioTicketAdapter;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;

        public PrioCriteriaService(IPrioTicketAdapter prioTicketAdapter, IMasterService masterService, ILogger logger)
        {
            _prioTicketAdapter = prioTicketAdapter;
            _masterService = masterService;
            _log = logger;
        }

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            try
            {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                if (criteria?.MappedProducts?.Count <= 0) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

                //Get activities with price and availabilities
                var activities = Task.Run(() => GetActivities(criteria)).ConfigureAwait(false).GetAwaiter().GetResult();

                return activities;
            }
            catch (Exception ex)
            {
                _log.Error("PrioCriteriaService|GetAvailability", ex);
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
                    var mappedProductsById = mappedProducts.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)).ToList();

                    if (mappedProductsById?.Count <= 0) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProductsById);
                    serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("PrioCriteriaService|GetServiceDetails", ex);
                throw;
            }
        }

        #region "Private Methods"

        /// <summary>
        /// Get Activities with price and availabilities
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private async Task<List<Activity>> GetActivities(ServiceAvailability.Criteria criteria)
        {
            var activities = new List<Activity>();
            var mappedOptions = criteria.MappedProducts;
            var groupedOptions = mappedOptions.GroupBy(e => e.IsangoHotelBedsActivityId);
            var taskArrayLength = groupedOptions.Count() * criteria.Counter;
            var taskArray = new Task<List<Activity>>[taskArrayLength];
            var count = 0;

            try
            {
                foreach (var activityWithOptions in groupedOptions)
                {
                    var activityId = activityWithOptions.Key;
                    var ageGroups = await _masterService.GetPrioAgeGroupAsync(activityId, APIType.Prio);
                    var validPassengerType = ageGroups.Select(e => e.PassengerType).Distinct();
                    var noOfPassengers = validPassengerType.ToDictionary(e => e, e => 1);
                    var supplierOptionCodes = activityWithOptions.Select(e => e.HotelBedsActivityCode).ToList();

                    for (int i = 1; i <= criteria?.Counter; i++)
                    {
                        var prioCriteria = new PrioCriteria
                        {
                            NoOfPassengers = noOfPassengers,
                            CheckinDate = DateTime.Now.AddDays(criteria.Days2Fetch * (i - 1)).Date,
                            CheckoutDate = DateTime.Now.AddDays(criteria.Days2Fetch * i).Date,
                            SupplierOptionCodes = supplierOptionCodes,
                            ActivityCode = activityId.ToString()
                        };
                        taskArray[count] = Task.Run(() => _prioTicketAdapter.UpdateOptionforPrioActivity(prioCriteria, Guid.NewGuid().ToString()));
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("PrioCriteriaService|GetActivities", ex);
            }

            try
            {
                if (taskArray?.Length > 0)
                {
                    taskArray = taskArray?.Where(t => t != null).ToArray();
                    if (taskArray.Length > 0)
                    {
                        Task.WaitAll(taskArray);
                        foreach (var task in taskArray)
                        {
                            var data = task.GetAwaiter().GetResult();
                            if (data != null)
                            {
                                //As UpdateOptionforPrioActivity only gives 1 activity in activity list
                                var activity = data.FirstOrDefault();
                                if (activity?.ProductOptions != null && activity.ProductOptions.Count > 0)
                                {
                                    var priceTypeId = (PriceTypeId)mappedOptions.First(x => x.IsangoHotelBedsActivityId == activity.ID)
                                        .PriceTypeId;
                                    activity.PriceTypeId = priceTypeId;
                                    activity.Code = activity.ID.ToString();
                                    activity.CurrencyIsoCode = activity.ProductOptions.FirstOrDefault().BasePrice.Currency.IsoCode;
                                    activities.Add(activity);
                                }
                            }
                        }
                    }
                }
            }
            catch (AggregateException ex)
            {
                _log.Error("PrioCriteriaService|GetActivities", ex);
                return activities;
            }
            catch (Exception ex)
            {
                _log.Error("PrioCriteriaService|GetActivities", ex);
                return activities;
            }
            return activities;
        }

        #endregion "Private Methods"
    }
}