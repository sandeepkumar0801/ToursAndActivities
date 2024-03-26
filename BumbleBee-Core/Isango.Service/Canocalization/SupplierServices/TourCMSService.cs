using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.TourCMSCriteria;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.TourCMS;
using Util;

namespace Isango.Service.SupplierServices
{
    public class TourCMSService : SupplierServiceBase, ISupplierService
    {
        private ITourCMSAdapter _tourCMSAdapter;
        private readonly IMemCache _memCache;
        private readonly ILogger _log;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IMasterPersistence _masterPersistence;


        public TourCMSService(ITourCMSAdapter tourCMSAdapter,
            IMemCache memCache, ILogger log, IMasterCacheManager masterCacheManager, IMasterPersistence masterPersistence)
        {
            _tourCMSAdapter = tourCMSAdapter;
            _memCache = memCache;
            _log = log;
            _masterCacheManager = masterCacheManager;
            _masterPersistence = masterPersistence;

        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var distinctProductOptionIds = activity.ProductOptions.Select(x => x.Id).ToList();
            //var result = _masterPersistence.GetTourCMSPaxMappings().Where(x => x.APIType == APIType.TourCMS).ToList();
            //var tourCMSPaxMapping = result.Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
            var tourCMSPaxMapping = _masterCacheManager.GetTourCMSMappings().Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
            var po = activity.ProductOptions?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode) && Int32.TryParse(x.PrefixServiceCode.Split('_')[0], out var channelIdTemp) && channelIdTemp != 0);
            if (criteria.CheckoutDate <= criteria.CheckinDate)
            {
                criteria.CheckoutDate = criteria.CheckoutDate.AddDays(Constant.AddFourteenDays);
            }

            if ((criteria.CheckoutDate - criteria.CheckinDate).Days < Constant.AddFourteenDays)
            {
                criteria.CheckoutDate = criteria.CheckinDate.AddDays(Constant.AddFourteenDays);
            }
            Int32.TryParse(po.PrefixServiceCode.Split('_')[0], out var channelId);
            Int32.TryParse(po.PrefixServiceCode.Split('_')[1], out var accountId);
            var tourCMSCriteria = new TourCMSCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                //FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,//.AddDays(Constant.AddSixDays),
                Language = clientInfo?.LanguageCode,
                //Destination = string.Empty,//destinationCode,
                ////PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                PassengerInfo = criteria.PassengerInfo,
                ActivityCode = po?.PrefixServiceCode,
                IsangoActivityId = activity?.Id.ToString(),
                ServiceOptionId = po?.Id.ToString(),
                Token = criteria.Token,
                TourCMSMappings = tourCMSPaxMapping,
                ChannelId = channelId,
                TourId = Convert.ToInt32(po.SupplierOptionCode),
                AccountId = accountId,
                CommissionPercent = po.Margin.Value,
                IsCommissionPercent = po.Margin.IsPercentage
            };
            return tourCMSCriteria;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            //#region New Json based API Apitude HotelBeds  Criteria
            var tourCMSCriteria = (TourCMSCriteria)criteria;
            try
            {
                //var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
                List<Activity> tourCMSActivities = null;

                tourCMSActivities = GetTourCMSActivitiesAsync(activity, tourCMSCriteria, token)?.GetAwaiter().GetResult();


                var apiResult = MapActivity(activity, tourCMSActivities, tourCMSCriteria);
                if (apiResult?.ProductOptions?.Count > 0)
                {
                    activity = apiResult;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"TourCMSService|GetAvailability|IsangoActivityId :- {activity?.Id}" +
                    $", CheckinDate :- {criteria?.CheckinDate}, " +
                    $", Language :- {tourCMSCriteria?.Language}", ex);
                //ignored
            }
            return activity;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromApi, Criteria criteria)
        {

            var tourCMSProductOptions = activitiesFromApi?.SelectMany(x => x.ProductOptions);

            if (tourCMSProductOptions != null)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOptionFromCache in activity.ProductOptions)
                {
                    var activityOptionFromAPI = tourCMSProductOptions
                        .Where(x => ((ActivityOption)x).ServiceOptionId == activityOptionFromCache.Id).ToList();

                    foreach (var productOptionFromAPI in activityOptionFromAPI)
                    {
                        var totalPassengers = 0;
                        var option = GetMappedActivityOption((ActivityOption)productOptionFromAPI, activityOptionFromCache, criteria);
                        if (!string.IsNullOrEmpty(option.Name))
                        {
                            option.Name = $"{option.Name}";
                        }
                        else
                        {
                            option.Name = $"{activityOptionFromCache.Name.Trim()}";
                        }

                        option.TravelInfo.NoOfPassengers = activityOptionFromCache.TravelInfo.NoOfPassengers;
                        option.PickupPointsForTourCMS = ((ActivityOption)productOptionFromAPI)?.PickupPointsForTourCMS;
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
                        productOptions.Add(option);
                    }
                }
                activity.ProductOptions = productOptions;
            }
            return activity;
        }

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

        /// <summary>
        /// It sequentially call api for each option for hotelbeds Apitude Api
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria">List of activity, there will be one activity for each option as iSango option is mapped with an activity at api end</param>
        /// <returns></returns>
        private Task<List<Activity>> GetTourCMSActivitiesAsync(Activity activity, TourCMSCriteria criteria, string token)
        {
            var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
            var tourCMSActivities = new List<Activity>();

            foreach (var po in productOptions)
            {
                try
                {
                    var tourCMSCriteriaGet = GetTourCMSCriteria(activity, criteria, po);
                    var tourCMSActivity = new Isango.Entities.Activities.Activity();
                    var checkIn = tourCMSCriteriaGet.CheckinDate;
                    var checkOut = tourCMSCriteriaGet.CheckinDate.AddDays(2);

                    //Check Availability upto 7 days, if not exist for today date.
                    for (var day = checkIn; day.Date <= checkOut; day = day.AddDays(1))
                    {
                        tourCMSCriteriaGet.CheckinDate = day;
                        tourCMSActivity = _tourCMSAdapter.ActivityDetails(tourCMSCriteriaGet,
                           token);
                        if (tourCMSActivity?.ProductOptions != null && tourCMSActivity?.ProductOptions.Count > 0)
                        {
                            break;
                        }
                    }
                    if (tourCMSActivity?.ProductOptions?.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true)
                    {
                        tourCMSActivities.Add(tourCMSActivity);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"ActivityService|GetTourCMSActivitiesAsync|IsangoActivityId :- {activity?.Id}" +
                        $", Criteria :- {SerializeDeSerializeHelper.Serialize(criteria)},", ex);
                }
            }
            return Task.FromResult(tourCMSActivities);
        }

        private TourCMSCriteria GetTourCMSCriteria(Activity activity, TourCMSCriteria criteria, ProductOption productOption)
        {
            var destinationCode = productOption?.SupplierOptionCode?.Split('~')?.LastOrDefault();
            int channelId = Convert.ToInt32(productOption.PrefixServiceCode.Split('_')[0]);
            int accountId = Convert.ToInt32(productOption.PrefixServiceCode.Split('_')[1]);
            var tourCMSCriteria = new TourCMSCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                //FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,//.AddDays(Constant.AddFourteenDays),

                Language = criteria.Language,
                ////PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                PassengerInfo = criteria.PassengerInfo,

                ActivityCode = productOption?.PrefixServiceCode,
                IsangoActivityId = activity?.Id,
                ServiceOptionId = productOption?.Id.ToString(),
                //Destination = destinationCode,
                TourCMSMappings = criteria.TourCMSMappings?.Where(x => x.ServiceOptionId == productOption.Id)?.ToList(),
                ChannelId = channelId,
                TourId = Convert.ToInt32(productOption.SupplierOptionCode),
                AccountId = accountId,
                CommissionPercent = criteria.CommissionPercent,
                IsCommissionPercent = criteria.IsCommissionPercent,
                LineOfBusinessId = activity?.LineOfBusinessId
            };

            return tourCMSCriteria;
        }

        /// <summary>
        /// Updates HotelBeds Apitude product options' "Sell Prices" to the current currency. Note: margin is ignored at the moment!
        /// </summary>
        /// <param name="productOptions"></param>
        /// <param name="hotelPickupLocation"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<ProductOption> UpdateBasePricesTourCMS(List<ProductOption> productOptions, string hotelPickupLocation,
            Criteria criteria)
        {
            if (productOptions == null || (productOptions.Count <= 0)) return productOptions;

            var options = new List<ProductOption>();
            foreach (var option in productOptions)
            {
                var actOption = (ActivityOption)option;
                if (actOption == null) continue;

                actOption.BasePrice = CalculatePriceForAllPax(actOption.SellPrice, criteria);
                actOption.GateBasePrice = CalculatePriceForAllPax(actOption.SellPrice, criteria);
                actOption.CostPrice = CalculatePriceForAllPax(actOption.CostPrice, criteria);

                if (actOption?.IsMandatoryApplyAmount == true)
                {
                    actOption.Margin = null;
                }
                actOption.HotelPickUpLocation = hotelPickupLocation;
                options.Add(actOption);
            }
            return options;
        }

        private Price CalculatePriceForAllPax(Price inputPrice, Criteria criteria)
        {
            if (inputPrice == null) return null;
            var price = inputPrice.DeepCopy();

            var isPerUnit = false;
            var perUnitPrice = new decimal();
            var perPersonPrice = new decimal();

            foreach (var priceAndAvailability in price.DatePriceAndAvailabilty)
            {
                if (priceAndAvailability.Value?.PricingUnits == null) continue;
                var pricingUnits = priceAndAvailability.Value.PricingUnits;
                foreach (var pricingUnit in pricingUnits)
                {
                    if (pricingUnit is PerUnitPricingUnit)
                    {
                        perUnitPrice = pricingUnit.Price;
                        isPerUnit = true;
                    }
                    //else if (pricingUnit is PricingUnit)
                    //{
                    //    perUnitPrice = pricingUnit.Price;
                    //    isPerUnit = true;
                    //}
                    else
                    {
                        var passengerType = ((PerPersonPricingUnit)pricingUnit).PassengerType;
                        var paxCount = GetPaxCountByPaxType(criteria, passengerType);
                        perPersonPrice = pricingUnit.Price * paxCount;
                    }
                }

                //priceAndAvailability.Value.TotalPrice = isPerUnit ? perUnitPrice : perPersonPrice;
            }
            //price.Amount = price.DatePriceAndAvailabilty.Where(x => x.Key.Date == criteria.CheckinDate.Date).
            //    Select(x => x.Value.TotalPrice).
            //    FirstOrDefault();

            return price;
        }
    }
}