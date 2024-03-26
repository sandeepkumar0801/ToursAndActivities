using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.NewCitySightSeeing;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace Isango.Service.SupplierServices
{
    public class NewCitySightSeeingService : SupplierServiceBase, ISupplierService
    {
        private INewCitySightSeeingAdapter _newCitySightSeeingAdapter;
        private readonly ILogger _log;

        public NewCitySightSeeingService(INewCitySightSeeingAdapter ticketAdapter,
             ILogger log)
        {
            _newCitySightSeeingAdapter = ticketAdapter;
            _log = log;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var po = activity.ProductOptions?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.SupplierOptionCode));
            if (criteria.CheckoutDate <= criteria.CheckinDate)
            {
                criteria.CheckoutDate = criteria.CheckoutDate.AddDays(Constant.AddFourteenDays);
            }

            if ((criteria.CheckoutDate - criteria.CheckinDate).Days < Constant.AddFourteenDays)
            {
                criteria.CheckoutDate = criteria.CheckinDate.AddDays(Constant.AddFourteenDays);
            }

            var newCitySightSeeingCriteria = new NewCitySightSeeingCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                Language = clientInfo?.LanguageCode,
                Destination = string.Empty,
                PassengerInfo = criteria.PassengerInfo,
                ActivityCode = po?.PrefixServiceCode,
                IsangoActivityId = activity?.Id.ToString(),
                ServiceOptionId = po?.Id.ToString(),
                Token = criteria.Token,
                ModalityCode = po?.PrefixServiceCode,
                Days2Fetch = Constant.AddFourteenDays,
                ProductOptionName = po.Name,
                ActivityName = activity.Name
            };

            return newCitySightSeeingCriteria;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var activityList = new List<Activity>();
            //#region New Json based API Apitude HotelBeds  Criteria
            var newCitySightSeeingCriteria = (NewCitySightSeeingCriteria)criteria;
            try
            {

                Activity newCitySightSeeingActivities = null;
                newCitySightSeeingActivities = GetNewCitySightSeeingActivitiesAsync(activity, newCitySightSeeingCriteria, token)?.GetAwaiter().GetResult();

                activityList.Add(newCitySightSeeingActivities);

                var apiResult = MapActivity(activity, activityList, newCitySightSeeingCriteria);
                if (apiResult?.ProductOptions?.Count > 0)
                {
                    activity = apiResult;
                }

            }
            catch (Exception ex)
            {
                _log.Error($"NewCitySightSeeingService|GetAvailability|IsangoActivityId :- {activity?.Id}" +
                    $", CheckinDate :- {criteria?.CheckinDate}, " +
                    $", Language :- {newCitySightSeeingCriteria?.Language}", ex);
                //ignored
            }
            return activity;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromApi, Criteria criteria)
        {

            var newCitySightSeeingProductOptions = activitiesFromApi.FirstOrDefault().ProductOptions;

            if (newCitySightSeeingProductOptions != null)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOptionFromCache in activity.ProductOptions)
                {
                    var activityOptionFromAPI = newCitySightSeeingProductOptions
                        .Where(x => ((ActivityOption)x).ServiceOptionId == activityOptionFromCache.Id).ToList();

                    foreach (var productOptionFromAPI in activityOptionFromAPI)
                    {
                        var totalPassengers = 0;
                        var option = GetMappedActivityOption((ActivityOption)productOptionFromAPI, activityOptionFromCache, criteria);
                        option.Name = option.Name;
                        option.TravelInfo.NoOfPassengers = activityOptionFromCache.TravelInfo.NoOfPassengers;
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
                VariantCondition = option.VariantCondition
            };
        }
        /// <summary>
        /// It sequentially call api for each option for hotelbeds Apitude Api
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria">List of activity, there will be one activity for each option as iSango option is mapped with an activity at api end</param>
        /// <returns></returns>
        private Task<Activity> GetNewCitySightSeeingActivitiesAsync(Activity activity, NewCitySightSeeingCriteria criteria, string token)
        {
            var productOptionsIsango = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.SupplierOptionCode))?.ToList();
            var newCitySightSeeingActivity = new Activity();
            newCitySightSeeingActivity.ProductOptions = new List<ProductOption>();
            int.TryParse(activity?.Id.ToString(), out int _tempInt);
            newCitySightSeeingActivity.ID = _tempInt;
            newCitySightSeeingActivity.Name = activity.Name;
            newCitySightSeeingActivity.ApiType = APIType.NewCitySightSeeing;
            
            var findAllSupplierOptionCodeDistinct = new List<string>();
            var findAllSupplierPrefixServiceCodeDistinct = new List<string>();
            findAllSupplierOptionCodeDistinct = productOptionsIsango?.Select(x => x.SupplierOptionCode)?.Distinct()?.ToList();
            findAllSupplierPrefixServiceCodeDistinct = productOptionsIsango?.Select(x => x.PrefixServiceCode)?.Distinct()?.ToList();

            if (findAllSupplierOptionCodeDistinct != null && findAllSupplierOptionCodeDistinct.Count > 0)
            {
                foreach (var po in findAllSupplierOptionCodeDistinct)
                {
                    try
                    {
                        var newCitySightSeeingCriteria = GetNewCitySightSeeingCriteriaUpdate(activity, criteria, po);
                        string request = "";
                        string response = "";
                        var newCitySightSeeingProductOptions =
                            _newCitySightSeeingAdapter.GetActivityAvailability
                            (newCitySightSeeingCriteria,
                            token, out request, out response);

                        ////Assign Isango Data according to API variantId and Filter Data 
                        if (newCitySightSeeingProductOptions != null && newCitySightSeeingProductOptions.Count>0)
                        {
                            // Remove api options if not match with isango mapping
                            if (findAllSupplierPrefixServiceCodeDistinct != null && findAllSupplierPrefixServiceCodeDistinct[0] != "")
                            {
                                newCitySightSeeingProductOptions.RemoveAll(e => !findAllSupplierPrefixServiceCodeDistinct.Contains(((ActivityOption)e).RateKey));
                            }
                            if (newCitySightSeeingProductOptions != null && newCitySightSeeingProductOptions.Count > 0)
                            {
                                foreach (var itemData in newCitySightSeeingProductOptions)
                                {
                                    var rateKey = ((ActivityOption)itemData).RateKey;
                                    if (productOptionsIsango != null && productOptionsIsango[0].PrefixServiceCode != "")
                                    {
                                        var isangoOption = productOptionsIsango.Where(x => x.PrefixServiceCode?.ToLower() == rateKey.ToLower())?.FirstOrDefault();
                                        if (isangoOption != null)
                                        {
                                            //Set OptionName from Database
                                            ((ActivityOption)itemData).Name = isangoOption?.Name;
                                            ((ActivityOption)itemData).ServiceOptionId = isangoOption.Id;
                                        }
                                    }
                                    else
                                    {
                                        var isangoOption = productOptionsIsango.Where(x => x.SupplierOptionCode?.ToLower() == rateKey.ToLower())?.FirstOrDefault();
                                        if (isangoOption != null)
                                        {
                                            //Set OptionName from Database
                                            ((ActivityOption)itemData).Name = isangoOption?.Name;
                                            ((ActivityOption)itemData).ServiceOptionId = isangoOption.Id;
                                        }
                                    }
                                    
                                }
                                if (newCitySightSeeingProductOptions.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true)
                                {
                                    newCitySightSeeingActivity.ProductOptions.AddRange(newCitySightSeeingProductOptions);
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"ActivityService|GetNewCitySightSeeingActivitiesAsync|IsangoActivityId :- {activity?.Id}" +
                            $", Criteria :- {SerializeDeSerializeHelper.Serialize(criteria)},", ex);
                    }
                }
            }
            return Task.FromResult(newCitySightSeeingActivity);
        }

        private NewCitySightSeeingCriteria GetNewCitySightSeeingCriteria(Activity activity, NewCitySightSeeingCriteria criteria, ProductOption productOption)
        {
            var destinationCode = productOption?.SupplierOptionCode?.Split('~')?.LastOrDefault();

            var newCitySightSeeingCriteria = new NewCitySightSeeingCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,

                Language = criteria.Language,
                PassengerInfo = criteria.PassengerInfo,

                ActivityCode = productOption?.PrefixServiceCode,
                IsangoActivityId = activity?.Id,
                ServiceOptionId = productOption?.Id.ToString(),
                Destination = destinationCode,
                ModalityCode = productOption?.PrefixServiceCode,
                Days2Fetch = Constant.AddFourteenDays,
                ProductOptionName = productOption.Name,
                ActivityName = activity.Name,
                SupplierOptionNewCitySeeing = productOption?.SupplierOptionCode
            };

            return newCitySightSeeingCriteria;
        }
        private NewCitySightSeeingCriteria GetNewCitySightSeeingCriteriaUpdate(Activity activity, NewCitySightSeeingCriteria criteria, string supplierCode)
        {
            var newCitySightSeeingCriteria = new NewCitySightSeeingCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,

                Language = criteria.Language,
                PassengerInfo = criteria.PassengerInfo,
                IsangoActivityId = activity?.Id,
                Days2Fetch = Constant.AddFourteenDays,
                ActivityName = activity.Name,
                SupplierOptionNewCitySeeing = supplierCode

                //ActivityCode = productOption?.PrefixServiceCode,
                //ServiceOptionId = productOption?.Id.ToString(),
                //Destination = destinationCode,
                //ModalityCode = productOption?.PrefixServiceCode,
                //ProductOptionName = productOption.Name,
             };
             return newCitySightSeeingCriteria;
        }



    }
}