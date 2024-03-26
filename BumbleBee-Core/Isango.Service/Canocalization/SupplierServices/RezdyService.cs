using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Rezdy;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Rezdy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class RezdyService : SupplierServiceBase, ISupplierService
    {
        private readonly IRezdyAdapter _rezdyAdapter;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly ILogger _log;

        public RezdyService(IRezdyAdapter rezdyAdapter, IMasterCacheManager masterCacheManager, ILogger log)
        {
            _rezdyAdapter = rezdyAdapter;
            _masterCacheManager = masterCacheManager;
            _log = log;
        }

        /// <summary>
        /// GetAvailability
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var rezdyCriteria = (RezdyCriteria)criteria;
            var rezdyActivities = _rezdyAdapter.GetAvailability(rezdyCriteria, token)?.GetAwaiter().GetResult();
            if (rezdyActivities?.Count > 0)
            {
                var getActivity = new List<Activity>();
                var rezdyActivity = new Activity
                {
                    ProductOptions = new List<ProductOption>()
                };
                rezdyActivity.ProductOptions = rezdyActivities;
                getActivity.Add(rezdyActivity);

                if (rezdyActivity.ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.RezdyAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, getActivity, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.RezdyAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        /// <summary>
        /// CreateCriteria
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var distinctProductOptionIds = activity.ProductOptions.Select(x => x.Id).ToList();
            var rezdyPaxMapping = _masterCacheManager.GetRezdysPaxMappings().Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
            var productCodes = activity.ProductOptions?.Select(x => x.SupplierOptionCode).Distinct().ToList();
            if (productCodes.Any())
            {
                var rezdyCriteria = new RezdyCriteria
                {
                    ProductCodes = productCodes,
                    ProductName = activity.Name,
                    CheckinDate = criteria.CheckinDate,
                    CheckoutDate = criteria.CheckoutDate,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages,
                    Currency = activity.CurrencyIsoCode,
                    RezdyPaxMappings = rezdyPaxMapping,
                    Dumping = "false",
                    Token = criteria.Token,
                    Language = criteria.Language
                };
                return rezdyCriteria;
            }
            return null;
        }

        /// <summary>
        /// MapActivity
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="rezdyActivities"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Activity MapActivity(Activity activity, List<Activity> rezdyActivities, Criteria criteria)
        {
            var rezdyProductOptions = rezdyActivities.FirstOrDefault().ProductOptions;

            if (rezdyProductOptions != null)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOptionFromCache in activity.ProductOptions)
                {
                    var activityOptionFromAPI = rezdyProductOptions
                        .Where(x => ((ActivityOption)x).ServiceOptionId == activityOptionFromCache.Id).ToList();

                    foreach (var productOptionFromAPI in activityOptionFromAPI)
                    {
                        var totalPassengers = 0;
                        var option = GetMappedActivityOption((ActivityOption)productOptionFromAPI, activityOptionFromCache, criteria);
                        option.Name = $"{activityOptionFromCache.Name.Trim()} @ {option.Name}";
                        option.TravelInfo.NoOfPassengers = activityOptionFromCache.TravelInfo.NoOfPassengers;
                        // REZDY API SEAT Available and Quantity is Between QuantityRequiredMin and QuantityRequiredMax
                        //Calculate total Passengers
                        var pricignUnit = productOptionFromAPI?.BasePrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value?.PricingUnits;
                        if (pricignUnit != null && pricignUnit.Count() > 0)
                        {
                            totalPassengers =
                            (from pricingUnit in pricignUnit
                             join noOfPassengersDict in criteria.NoOfPassengers
                             on ((PerPersonPricingUnit)pricingUnit).PassengerType.ToString() equals noOfPassengersDict.Key.ToString()
                             select pricingUnit.Quantity * noOfPassengersDict.Value).Sum();
                        }
                        if ((totalPassengers <= option.SeatsAvailable) && (totalPassengers >= option.QuantityRequiredMin))
                        {
                            if (option.QuantityRequiredMax != 0)
                            {
                                if (totalPassengers <= option.QuantityRequiredMax)
                                {
                                    productOptions.Add(option);
                                }
                                else
                                {
                                    _log.Warning($"ActivityService|LoadMappedDataRezdy| {activity.Id},{option.Id} : Qty is greater than Max Value of API");
                                }
                            }
                            else
                            {
                                productOptions.Add(option);
                            }
                        }
                        else
                        {
                            _log.Warning($"ActivityService|LoadMappedDataRezdy| {activity.Id},{option.Id} :Seat Not Available or Qty not between max and min Value");
                        }
                    }
                }
                activity.ProductOptions = productOptions;
            }
            return activity;
        }

        /// <summary>
        /// Map ActivityOption data from source to destination object
        /// </summary>
        /// <param name="option"></param>
        /// <param name="productOption"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private ActivityOption GetMappedActivityOption(ActivityOption option, ProductOption productOption, Criteria criteria)
        {
            return new ActivityOption
            {
                ActivitySeasons = option.ActivitySeasons,
                AvailToken = option.AvailToken,
                Code = option.Code,
                Contract = option.Contract,
                ContractQuestions = option.ContractQuestions,
                HotelPickUpLocation = option.HotelPickUpLocation,
                PickUpOption = option.PickUpOption,
                PickupPointDetails = option.PickupPointDetails,
                PickupPoints = option.PickupPoints,
                PricingCategoryId = option.PricingCategoryId,
                PrioTicketClass = option.PrioTicketClass,
                RateKey = option.RateKey,
                ScheduleReturnDetails = option.ScheduleReturnDetails,
                StartTimeId = option.StartTimeId,
                OptionType = option.OptionType,
                ServiceType = option.ServiceType,
                RoomType = option.RoomType,
                ScheduleId = option.ScheduleId,
                ProductType = option.ProductType,
                RefNo = option.RefNo,

                Cancellable = option.Cancellable,
                Holdable = option.Holdable,
                Refundable = option.Refundable,
                Type = option.Type,
                HoldablePeriod = option.HoldablePeriod,
                Time = option.Time,
                RateId = option.RateId,
                PriceId = option.PriceId,
                SupplierId = option.SupplierId,

                BasePrice = CalculatePriceForAllPax(option.BasePrice, criteria),
                CostPrice = CalculatePriceForAllPax(option.CostPrice, criteria),
                GateBasePrice = CalculatePriceForAllPax(option.GateBasePrice, criteria),
                AvailabilityStatus = option.AvailabilityStatus,
                Customers = option.Customers,
                TravelInfo = option.TravelInfo,
                CommisionPercent = option.CommisionPercent,
                CancellationPrices = option.CancellationPrices,
                IsSelected = option.IsSelected,

                Id = option.Id,
                ServiceOptionId = productOption.Id,
                Name = option.Name,
                SupplierName = productOption.SupplierName,
                Description = productOption.Description,
                BookingStatus = productOption.BookingStatus,
                OptionKey = productOption.OptionKey,
                Capacity = productOption.Capacity,
                Quantity = productOption.Quantity,
                SupplierOptionCode = productOption.SupplierOptionCode,
                Margin = productOption.Margin,

                Seats = option.Seats,
                SeatsAvailable = option.SeatsAvailable,
                AllDay = option.AllDay,
                StartLocalTime = option.StartLocalTime,
                EndLocalTime = option.EndLocalTime,
                PickUpId = option.PickUpId,
                QuantityRequired = option.QuantityRequired,
                QuantityRequiredMin = option.QuantityRequiredMin,
                QuantityRequiredMax = option.QuantityRequiredMax,
                StartTime = option.StartTime,
                EndTime = option.EndTime,

                OptionOrder = productOption.OptionOrder > 0 ?
                                productOption.OptionOrder :
                                option.OptionOrder,

                CancellationText = option.CancellationText,
                ApiCancellationPolicy = option.ApiCancellationPolicy,
            };
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "RezdyService",
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