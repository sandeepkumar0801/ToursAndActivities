using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GoldenTours;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GoldenTours;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class GoldenToursCriteriaService : IGoldenToursCriteriaService
    {
        private readonly IGoldenToursAdapter _goldenToursAdapter;
        private readonly IMasterService _masterService;
        private readonly IActivityPersistence _activityPersistence;
        private readonly ILogger _log;

        public GoldenToursCriteriaService(IGoldenToursAdapter goldenToursAdapter, ILogger log, IActivityService activityService, IMasterService masterService, IActivityPersistence activityPersistence)
        {
            _goldenToursAdapter = goldenToursAdapter;
            _masterService = masterService;
            _activityPersistence = activityPersistence;
            _log = log;
        }

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            try
            {
                var counter = criteria?.Counter;
                if (counter <= 0) return null;
                var tokenId = Guid.NewGuid().ToString();
                var activities = new List<Activity>();
                var passengerMappings = _masterService.GetPassengerMapping(APIType.Goldentours).GetAwaiter().GetResult();

                for (var i = 1; i <= counter; i++)
                {
                    var dateTimeNow = DateTime.Now;
                    var daysToFetch = criteria.Days2Fetch;

                    for (var start = dateTimeNow.AddDays(daysToFetch * (i - 1)).Date; start.Date < dateTimeNow.AddDays(daysToFetch * i).Date; start = start.AddDays(daysToFetch).Date)
                    {
                        var mappedProducts = criteria.MappedProducts;
                        var activityId = 0;
                        var noOfPassengers = new Dictionary<PassengerType, int>();
                        foreach (var item in mappedProducts)
                        {
                            try
                            {
                                // Fetching 'GetPassengerInfoDetails' for ActivityId, avoiding extra calls for each optionId as their ActivityId will be same
                                if (activityId != item.IsangoHotelBedsActivityId)
                                {
                                    var activityAgeGroups = _activityPersistence.GetPassengerInfoDetails(item.IsangoHotelBedsActivityId.ToString());
                                    noOfPassengers = activityAgeGroups.ToDictionary(x => (PassengerType)x.PassengerTypeId, x => item.MinAdultCount);
                                    activityId = item.IsangoHotelBedsActivityId;
                                }

                                var activity = GetGoldenToursActivities(item, criteria.Days2Fetch, start, noOfPassengers, passengerMappings, tokenId);
                                if (activity == null) continue;
                                activities.Add(activity);
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "GoldenToursCriteriaService",
                                    MethodName = "GetAvailability",
                                    Token = criteria.Token,
                                    Params = $"{item.IsangoHotelBedsActivityId},{item.HotelBedsActivityCode}"
                                };
                                _log.Error(isangoErrorEntity, ex);
                                // ignored // failing one item should not fail entire dumping.
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
                    ClassName = "GoldenToursCriteriaService",
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
                    catch(Exception ex)
                    {
                        //ignored - probably wrong supplier code
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "GoldenToursCriteriaService",
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

        #region "Private Methods"

        private Activity GetGoldenToursActivities(IsangoHBProductMapping item, double daysToFetch, DateTime startDate, Dictionary<PassengerType, int> noOfPassengers, List<PassengerMapping> passengerMappings, string tokenId = null)
        {
            if (string.IsNullOrWhiteSpace(tokenId))
            {
                tokenId = Guid.NewGuid().ToString();
            }

            var goldenToursCriteria = new GoldenToursCriteria
            {
                SupplierOptionCode = item.HotelBedsActivityCode,
                CheckinDate = startDate,
                CheckoutDate = startDate.AddDays(daysToFetch),
                NoOfPassengers = noOfPassengers,
                PassengerMappings = passengerMappings,
                SupplierOptionCodes = new List<string> { item.HotelBedsActivityCode }
            };

            //#TODO Check-in and checkout date ranges implementation is missing.
            //var getAvailOptions = _goldenToursAdapter.GetProductDetails(goldenToursCriteria, tokenId);

            //Get price and availability from supplier api
            var productOptions = _goldenToursAdapter.GetPriceAvailabilityForDumping(goldenToursCriteria, tokenId);

            if (productOptions?.Count > 0)
            {
                // Passing OptionId and margin here as we are getting only one option from the adapter.
                productOptions.ForEach(option =>
                {
                    option.Id = item.ServiceOptionInServiceid;
                    option.CommisionPercent = item.MarginAmount;
                });
                var activity = new Activity
                {
                    ID = item.IsangoHotelBedsActivityId,
                    Code = item.HotelBedsActivityCode,
                    FactsheetId = item.FactSheetId,
                    ApiType = APIType.Goldentours,
                    CurrencyIsoCode = item.CurrencyISOCode,
                    PriceTypeId = (PriceTypeId)item.PriceTypeId,
                    ProductOptions = productOptions
                };
                return activity;
            }
            return null;
        }

        #endregion "Private Methods"
    }
}