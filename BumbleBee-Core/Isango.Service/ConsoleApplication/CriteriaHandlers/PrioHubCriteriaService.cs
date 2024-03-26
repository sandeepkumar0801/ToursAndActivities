using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.PrioHub;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.PrioHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Activity = Isango.Entities.Activities.Activity;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class PrioHubCriteriaService : IPrioHubCriteriaService
    {
        private readonly IPrioHubAdapter _prioHubAdapter;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;

        public PrioHubCriteriaService(IPrioHubAdapter prioTicketAdapter, IMasterService masterService, ILogger logger)
        {
            _prioHubAdapter = prioTicketAdapter;
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
                if (criteria?.MappedProducts?.Count <= 0) return null;

                //Get activities with price and availabilities
                var activities = GetActivities(criteria);

                return activities;

            }
            catch (Exception ex)
            {
                _log.Error("NewPrioCriteriaService|GetAvailability", ex);
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
                    try
                    {
                        if (activity == null) continue;
                        var mappedProductsById = mappedProducts.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID))?.ToList();
                        if (mappedProductsById?.Count == 0)
                        {
                            continue;
                        }
                        //if (mappedProductsById?.Count <= 0) continue;
                        var serviceMapper = new ServiceMapper();

                        foreach (var option in activity.ProductOptions)
                        {
                            var mappedProduct = mappedProductsById?.Where(x => x.HotelBedsActivityCode == ((Entities.Activities.ActivityOption)option).Code)?.FirstOrDefault();
                            if (mappedProduct != null)
                            {
                                if (mappedProduct?.IsIsangoMarginApplicable ?? false)
                                {
                                    var details = serviceMapper.ProcessServiceDetailsWithBasePricePrioHub(activity, option, mappedProducts);
                                    if (details != null)
                                    {
                                        serviceDetails.AddRange(details);
                                    }
                                }
                                else
                                {
                                    var details = serviceMapper.ProcessServiceDetailsWithBaseGateBaseAndCostPricesPrioHub(activity, option, mappedProduct);
                                    if (details != null)
                                    { 
                                        serviceDetails.AddRange(details);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("PrioHubCriteriaService|GetServiceDetails", ex);
                    }

                        //var details = serviceMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProductsById);
                   
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("NewPrioCriteriaService|GetServiceDetails", ex);
                throw;
            }
        }

        #region "Private Methods"
        /// <summary>
        /// Get Activities with price and availabilities
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<Activity> GetActivities(ServiceAvailability.Criteria criteria)
        {
            var activities = new List<Activity>();
            try
            {
                var activitiesFromAPI = new List<Activity>();
                var mappedOptions = criteria?.MappedProducts?.Where(x => x.PrefixServiceCode != string.Empty)?.ToList();

                var mappedoptionsGroupBy = mappedOptions.GroupBy(u => u.PrefixServiceCode)?.Select(grp => grp.ToList())?.ToList();
                var count = 0;

                try
                {
                    foreach (var mappedoptionsSingle in mappedoptionsGroupBy)
                    {
                        var filterGroupBy = mappedoptionsSingle.GroupBy(u => u.MinAdultCount)?.Select(grp => grp.ToList())?.ToList();
                        //1,2,3
                        foreach (var filterData in filterGroupBy)
                        {
                            for (int countData = 0; countData < filterData.Count; countData += 20)
                            {
                                var apiActivityId = filterData?.Skip(countData)?.Take(20)?.Select(x => x.HotelBedsActivityCode)?.ToList();
                                var multipleAPIActivityids = string.Join(",", apiActivityId);
                                var ageGroups = new List<AgeGroup>();
                                var noOfPassengers = new Dictionary<PassengerType, int>
                            {{PassengerType.Adult, filterData.FirstOrDefault().MinAdultCount},
                             { PassengerType.Family, filterData.FirstOrDefault().MinAdultCount}};

                                for (int i = 1; i <= criteria?.Counter; i++)
                                {
                                    var prioCriteria = new PrioHubCriteria
                                    {
                                        NoOfPassengers = noOfPassengers,
                                        CheckinDate = DateTime.Now.AddDays(criteria.Days2Fetch * (i - 1)).Date,
                                        CheckoutDate = DateTime.Now.AddDays(criteria.Days2Fetch * i).Date,
                                        SupplierMultipleCodes = multipleAPIActivityids,
                                        ProductMapping = filterData,
                                        DistributorId = Convert.ToInt32(filterData?.FirstOrDefault()?.PrefixServiceCode)
                                    };
                                    activitiesFromAPI.AddRange(_prioHubAdapter.UpdateOptionforPrioHubActivity(prioCriteria, Guid.NewGuid().ToString()));
                                    count++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("NewPrioHubCriteriaService|GetActivities", ex);
                }

                try
                {
                    foreach (var data in activitiesFromAPI)
                    {
                        if (data != null)
                        {
                            //As UpdateOptionforPrioActivity only gives 1 activity in activity list
                            var activity = data;
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
                catch (AggregateException ex)
                {
                    _log.Error("NewPrioHubCriteriaService|GetActivities", ex);
                    return activities;
                }
                catch (Exception ex)
                {
                    _log.Error("NewPrioHubCriteriaService|GetActivities", ex);
                    return activities;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return activities;
        }

        #endregion "Private Methods"
    }
}