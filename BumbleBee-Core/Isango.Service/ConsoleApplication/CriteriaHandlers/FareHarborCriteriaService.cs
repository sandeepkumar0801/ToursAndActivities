using CacheManager.Contract;
using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Activity = Isango.Entities.Activities.Activity;
using FareHarborProduct = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;
using TravelInfo = Isango.Entities.TravelInfo;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class FareHarborCriteriaService : IFareHarborCriteriaService
    {
        private readonly IFareHarborAdapter _fareHarborAdapter;
        private readonly IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManager;
        private readonly IFareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManager;
        private readonly ILogger _log;

        public FareHarborCriteriaService(IFareHarborAdapter fareHarborAdapter, IFareHarborUserKeysCacheManager fareHarborUserKeysCacheManager, IFareHarborCustomerPrototypesCacheManager fareHarborCustomerPrototypesCacheManager, ILogger logger)
        {
            _fareHarborAdapter = fareHarborAdapter;
            _fareHarborUserKeysCacheManager = fareHarborUserKeysCacheManager;
            _fareHarborCustomerPrototypesCacheManager = fareHarborCustomerPrototypesCacheManager;
            _log = logger;
        }

        /// <summary>
        /// GetAvailability for Fareharbor dumping
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            try
            {
                var counter = criteria?.Counter;

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                if (!(counter > 0)) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

                var activities = new List<Activity>();
                var customerPrototypeMapping = _fareHarborCustomerPrototypesCacheManager
                                                        .GetCustomerPrototypeList().ToList();
                var userKeys = _fareHarborUserKeysCacheManager.GetFareHarborUserKeys();
                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount();
                if (maxParallelThreadCount > 4)
                {
                    maxParallelThreadCount = 4;
                }
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount };
                for (var i = 1; i <= counter; i++)
                {
                    var dateTimeNow = DateTime.Now;
                    for (var start = dateTimeNow.AddDays(criteria.Days2Fetch * (i - 1)).Date; start.Date < dateTimeNow.AddDays(criteria.Days2Fetch * i).Date; start = start.AddDays(criteria.Days2Fetch).Date)
                    {
                        
                        foreach (var batch in criteria.MappedProducts.Batch(10))
                        {
                            try
                            {
                                var taskArray = new Task<Activity>[batch.Count()];
                                var count = 0;
                                foreach (var item in batch)
                                {
                                    taskArray[count] = FillFareHarborActivities(start, item, criteria, customerPrototypeMapping.ToList(), userKeys.ToList());
                                    count++;
                                }
                                try
                                {
                                    if (taskArray?.Length > 0)
                                    {
                                        Task.WaitAll(taskArray);

                                        Parallel.ForEach(taskArray, parallelOptions, task =>
                                        {
                                        //foreach (var task in taskArray?.Where(x => x != null).ToList())
                                        //{
                                        try
                                            {
                                                var data = task?.GetAwaiter().GetResult();
                                                if (data?.ProductOptions?.Count > 0)
                                                    activities.Add(data);
                                            }
                                            catch (Exception ex)
                                            {
                                                Task.Run(() => _log.Error(new IsangoErrorEntity
                                                {
                                                    ClassName = "FareHarborCriteriaService",
                                                    MethodName = "GetAvailability",
                                                    Token = criteria.Token
                                                }, ex));

                                            // throw;
                                        }
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {

                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "FareHarborCriteriaService",
                                    MethodName = "GetAvailability",
                                    Token = criteria.Token
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }
                       
                    }
                }

                return activities;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborCriteriaService",
                    MethodName = "GetAvailability",
                    Token = criteria.Token
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
                    try
                    {
                        if (activity == null) continue;
                        var mappedProduct = mappedProducts.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)).ToList();

                        if (mappedProduct == null) continue;
                        var serviceMapper = new ServiceMapper();
                        var details = serviceMapper.ProcessServiceDetailsWithBasePrice(activity, mappedProduct);
                        serviceDetails.AddRange(details);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "FareHarbourCriteriaService",
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
                    ClassName = "FareHarborCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region "Private Methods"

        private Task<Activity> FillFareHarborActivities(DateTime start, IsangoHBProductMapping item, ServiceAvailability.Criteria criteria, List<Entities.CustomerPrototype> customerPrototypeMapping, List<FareHarborUserKey> userKeys)
        {
            var activity = default(Activity);
            try
            {
                var customerPrototypeMappingForActivity = customerPrototypeMapping.Where(e => e.ServiceId == item.IsangoHotelBedsActivityId).ToList();
                if (customerPrototypeMappingForActivity?.Count <= 0)
                {
                    activity = new Activity();
                    return Task.FromResult(activity);
                }
                var userkey = userKeys?.FirstOrDefault(x => x.CompanyShortName.Trim().Equals(item.HotelBedsActivityCode));
                var noOfPassengers = customerPrototypeMappingForActivity.Select(e => e.PassengerType).Distinct().ToDictionary(x => x, x => 1);
                var fhbCriteria = new FareHarborCriteria
                {
                    UserKey = userkey?.UserKey,
                    NoOfPassengers = noOfPassengers,
                    ActivityCode = item.FactSheetId.ToString(),
                    CompanyName = item.HotelBedsActivityCode,
                    CheckinDate = start,
                    CheckoutDate = start.AddDays(criteria.Days2Fetch),
                    CustomerPrototypes = customerPrototypeMappingForActivity
                };
                activity = new Activity
                {
                    ID = item.IsangoHotelBedsActivityId,
                    Code = item.HotelBedsActivityCode,
                    FactsheetId = item.FactSheetId,
                    CurrencyIsoCode = userkey != null ? userkey.Currency : "USD",
                    ApiType = APIType.Fareharbor,
                    PriceTypeId = (PriceTypeId)item.PriceTypeId,
                    ProductOptions = new List<ProductOption>()
                };
                var availabilitiesDTO = GetProductAvailability(fhbCriteria, item.FactSheetId, criteria.Token);
                if (availabilitiesDTO?.Availabilities?.Length > 0)
                {
                    foreach (var avail in availabilitiesDTO.Availabilities?.ToList())
                    {
                        try
                        {
                            foreach (var availTypeRate in avail.CustomerTypeRates)
                            {
                                var validPassengerTypes = customerPrototypeMappingForActivity.Where(e => e.CustomerPrototypeId == availTypeRate.CustomerPrototype.Pk).Select(e => e.PassengerType);
                                var totalIncludingTax = availTypeRate.CustomerPrototype.TotalIncludingTax != 0 ? availTypeRate.CustomerPrototype.TotalIncludingTax / 100 : 0;
                                foreach (var validPassengerType in validPassengerTypes)
                                {
                                    var dateTimeOffset = DateTimeOffset.Parse(avail.StartAt, null);
                                    var activityOption = new ActivityOption
                                    {
                                        Code = availTypeRate.CustomerPrototype.Pk.ToString(),
                                        Id = item.ServiceOptionInServiceid,
                                        ServiceOptionId = item.ServiceOptionInServiceid,
                                        StartTime = dateTimeOffset.TimeOfDay,
                                        Capacity = avail.Capacity ?? 0,
                                        CommisionPercent = item.MarginAmount
                                    };

                                    var price = new Price
                                    {
                                        Amount = totalIncludingTax,
                                        Currency = new Currency() { IsoCode = activity.CurrencyIsoCode },
                                        DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                                    };

                                    var PricingUnit = CreatePricingUnit(totalIncludingTax, validPassengerType);
                                    price.DatePriceAndAvailabilty.Add(dateTimeOffset.DateTime, new FareHarborPriceAndAvailability
                                    {
                                        TotalPrice = totalIncludingTax,
                                        PricingUnits = new List<PricingUnit> { PricingUnit },
                                        AvailabilityStatus = AvailabilityStatus.AVAILABLE
                                    });

                                    activityOption.BasePrice = price;

                                    activityOption.TravelInfo = new TravelInfo
                                    {
                                        NoOfPassengers = fhbCriteria.NoOfPassengers,
                                        NumberOfNights = 1,
                                        Ages = fhbCriteria.Ages,
                                        StartDate = start
                                    };
                                    activity.ProductOptions.Add(activityOption);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                activity = new Activity();
            }
            if (activity?.ProductOptions?.Any() != true)
            {
                activity = new Activity();
            }
            return Task.FromResult(activity);
        }

        private static PricingUnit CreatePricingUnit(decimal totalIncludingTax, PassengerType passengerType)
        {
            var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
            if (pricingUnit == null)
                return pricingUnit;
            pricingUnit.Price = totalIncludingTax;
            return pricingUnit;
        }

        private ItemDTO GetProductAvailability(FareHarborCriteria criteria, int factsheetId, string token)
        {
            var product = new FareHarborProduct
            {
                UserKey = criteria.UserKey,
                ServiceId = Convert.ToInt32(criteria.ActivityCode),
                SupplierName = criteria.CompanyName,
                FactsheetId = factsheetId,
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate
            };
            var response = _fareHarborAdapter.GetCustomerPrototypesByProductId(product, token);
            return response;
        }

        #endregion "Private Methods"
    }
}