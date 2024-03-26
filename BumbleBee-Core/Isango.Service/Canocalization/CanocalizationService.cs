using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Canocalization;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTix;
using Isango.Entities.RedeamV12;
using Isango.Persistence.Contract;
using Isango.Service.ConsoleApplication;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels;
using ServiceAdapters.RedeamV12;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.CreateHold;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.Booking;
using Util;
using ConsoleCriteria = Isango.Entities.ConsoleApplication.ServiceAvailability;
using ConsoleService = Isango.Service.ConsoleApplication;
using Price = Isango.Entities.Price;

namespace Isango.Service.Canocalization
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CanocalizationService : ICanocalizationService
    {
        private readonly IRedeamV12Adapter _redeamV12Adapter;
        private readonly IGlobalTixAdapterV3 _globalTixAdapterV3;
        private readonly IBookingPersistence _bookingPersistence;
        private readonly ISupplierBookingService _supplierBookingService;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;
        private readonly Dictionary<APIType, List<SelectedProduct>> _failedSupplierProducts = new Dictionary<APIType, List<SelectedProduct>>();
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly ISupplierBookingPersistence _supplierBookingPersistence;
        private readonly bool _IsReservation;
        private readonly IMasterCacheManager _masterCacheManager;

        public CanocalizationService(IBookingPersistence bookingPersistence, ILogger log
            , ISupplierBookingService supplierBookingService
            , IMasterService masterService
            , IRedeamV12Adapter redeamV12Adapter
            , ITableStorageOperation tableStorageOperation
            , ISupplierBookingPersistence supplierBookingPersistence
            , IGlobalTixAdapterV3 globalTixAdapterV3
            , IMasterCacheManager masterCacheManager
            )
        {
            _bookingPersistence = bookingPersistence;
            _supplierBookingService = supplierBookingService;
            _masterService = masterService;
            _log = log;
            _redeamV12Adapter = redeamV12Adapter;
            _IsReservation = Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsReservation));
            _tableStorageOperation = tableStorageOperation;
            _supplierBookingPersistence = supplierBookingPersistence;
            _globalTixAdapterV3 = globalTixAdapterV3;
            _masterCacheManager = masterCacheManager;
        }

        #region  [GetAgeGroupData- Static Data Dumping]
        public object GetAgeGroupData(string token, object data, string methodType, APIType apiType)
        {
            //Redeam V1.2 API
            if (apiType == APIType.RedeamV12)
            {
                return AgeGroupRedeam12(token, data, methodType);
            }
            return null;
        }

        private object AgeGroupRedeam12(string token, object data, string methodType)
        {
            if (methodType == Convert.ToString(ServiceAdapters.RedeamV12.RedeamV12.Entities.MethodType.GetSuppliers))
            {
                return _redeamV12Adapter.GetSuppliers(token).Result;
            }
            else if (methodType == Convert.ToString(ServiceAdapters.RedeamV12.RedeamV12.Entities.MethodType.GetProducts))
            {
                var productDataList = new List<Isango.Entities.RedeamV12.ProductData>();
                foreach (var supplierId in ((List<string>)data))
                {
                    var criteria = new Isango.Entities.CanocalizationCriteria
                    {
                        SupplierId = supplierId
                    };
                    var productData = _redeamV12Adapter.GetProducts(criteria, token).Result;

                    if (productData == null) continue;
                    productDataList.AddRange(productData);
                }
                return productDataList;
            }
            else if (methodType == Convert.ToString(ServiceAdapters.RedeamV12.RedeamV12.Entities.MethodType.GetRates))
            {
                var ratesDataList = new List<Isango.Entities.RedeamV12.RateData>();
                var priceDataList = new List<Isango.Entities.RedeamV12.PriceData>();
                var passengerTypeDataList = new List<Isango.Entities.RedeamV12.PassengerTypeData>();

                var productsBySupplierId = ((List<Isango.Entities.RedeamV12.ProductData>)data).GroupBy(x => x.SupplierId);
                foreach (var value in productsBySupplierId)
                {
                    var productDataList = value.ToList();
                    foreach (var datainner in ((List<Isango.Entities.RedeamV12.ProductData>)data))
                    {
                        var criteria = new Isango.Entities.CanocalizationCriteria
                        {
                            SupplierId = value.Key,
                            ProductId = datainner.ProductId
                        };
                        var ratesWrapper = _redeamV12Adapter.GetRatesWrapper(criteria, token).Result;
                        if (ratesWrapper == null) continue;

                        ratesDataList.AddRange(ratesWrapper.Rates);
                        priceDataList.AddRange(ratesWrapper.Prices);
                        passengerTypeDataList.AddRange(ratesWrapper.TravelerTypes);
                    }
                }

                var result = new Isango.Entities.RedeamV12.RatesWrapper
                {
                    Prices = priceDataList,
                    Rates = ratesDataList,
                    TravelerTypes = passengerTypeDataList
                };
                return result;

            }
            return null;
        }

        private Activity GetAPIActivities(IsangoHBProductMapping item, ConsoleCriteria.Criteria criteria, DateTime startDate, string tokenId = null)
        {
            var splitResult = item?.HotelBedsActivityCode?.Split('#');

            if (splitResult?.Length != 2)
            {
                var message = "Incorrect mapping SupplierOptionCode should be in  'ProductId#RateId' format";
                var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = message
                };
                throw new HttpResponseException(data);
            }

            var supplierId = item.SupplierCode;

            if (string.IsNullOrWhiteSpace(tokenId))
            {
                tokenId = Guid.NewGuid().ToString();
            }
            var apiCriteria = new CanocalizationCriteria
            {
                RateIds = new List<string>
                {
                    item.HotelBedsActivityCode
                },
                SupplierId = supplierId,
                CheckinDate = startDate,
                CheckoutDate = startDate.AddDays(criteria.Days2Fetch),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Adult, 1 }
                },
                RateIdAndType = new Dictionary<string, string>
                {
                    { item.HotelBedsActivityCode, item.PrefixServiceCode }
                },
                ServiceOptionId = item.ServiceOptionInServiceid,
                Currency = item.CurrencyISOCode
            };

            //Get price and availability from supplier api
            var productOptions = _redeamV12Adapter.GetAvailabilities(apiCriteria, tokenId)?.GetAwaiter().GetResult();
            if (!(productOptions?.Count > 0)) return null;

            // Passing OptionId and margin here as we are getting only one option from the adapter.
            productOptions.ForEach(option =>
            {
                option.Id = item.ServiceOptionInServiceid;
                option.Margin = new Margin
                {
                    Value = item.MarginAmount,
                    IsPercentage = true
                };
            });
            var activity = new Activity
            {
                ID = item.IsangoHotelBedsActivityId,
                Code = item.HotelBedsActivityCode,
                FactsheetId = item.FactSheetId,
                ApiType = APIType.RedeamV12,
                CurrencyIsoCode = item.CurrencyISOCode,
                ProductOptions = productOptions //filteredOptions.Cast<ProductOption>().ToList()
            };
            return activity;
        }
        #endregion

        #region [Data Dumping -Calendar Data Dumping]

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetAvailability(ConsoleCriteria.Criteria criteria)
        {
            try
            {
                if (criteria.ApiType == APIType.RedeamV12)
                {
                    var counter = criteria?.Counter;
                    if (counter <= 0) return null;
                    var tokenId = Guid.NewGuid().ToString();
                    var activities = new List<Activity>();
                    for (var i = 1; i <= counter; i++)
                    {
                        var dateTimeNow = DateTime.Now;
                        var daysToFetch = criteria.Days2Fetch;

                        for (var start = dateTimeNow.AddDays(daysToFetch * (i - 1)).Date; start.Date < dateTimeNow.AddDays(daysToFetch * i).Date; start = start.AddDays(daysToFetch).Date)
                        {
                            var mappedProducts = criteria.MappedProducts;
                            foreach (var item in mappedProducts)
                            {
                                try
                                {
                                    var activity = GetAPIActivities(item, criteria, start, tokenId);
                                    if (activity == null) continue;
                                    activities.Add(activity);
                                }
                                catch (Exception ex)
                                {
                                    _log.Error($"CanocalizationService|GetAvailability IsangoServiceId{item.IsangoHotelBedsActivityId}, APIServiceId {item.HotelBedsActivityCode}", ex);
                                    // ignored // failing one item should not fail entire dumping.
                                }
                            }
                        }
                    }

                    return activities;
                }
                else if (criteria.ApiType == APIType.GlobalTixV3)
                {
                    List<Activity> activitiesList = new List<Activity>();

                    foreach (IsangoHBProductMapping mappedProduct in criteria.MappedProducts)
                    {
                        try
                        {
                            var distinctProductOptionIds = mappedProduct.ServiceOptionInServiceid;

                            var globalTixV3MappingFilter = _masterCacheManager.GetGlobalTixV3Mappings();

                            var globalTixV3Mapping = globalTixV3MappingFilter.Where(x => x.ServiceOptionId == distinctProductOptionIds).ToList();
                            if (globalTixV3Mapping == null)
                            {
                                continue;
                            }


                            var gtCriteria = new CanocalizationCriteria()
                            {
                                ActivityIdStr = mappedProduct.HotelBedsActivityCode,
                                ServiceOptionID = mappedProduct?.ServiceOptionInServiceid ?? 0,
                                FactSheetId = mappedProduct.FactSheetId,
                                Days2Fetch = (criteria.Days2Fetch * criteria.Months2Fetch),
                                CheckinDate = DateTime.Now,
                                GlobalTixV3Mapping = globalTixV3Mapping
                            };
                            gtCriteria.NoOfPassengers = new Dictionary<Entities.Enums.PassengerType, int>();
                            gtCriteria.NoOfPassengers.Add(Entities.Enums.PassengerType.Adult, mappedProduct.MinAdultCount);
                            //Temp code starts
                            Activity activity = _globalTixAdapterV3.GetActivityInformation(gtCriteria, criteria.Token, mappedProduct.CountryId == 6667 ? false : true);

                            if (activity != null)
                            {
                                activity.ID = mappedProduct.IsangoHotelBedsActivityId;
                                activity.Id = mappedProduct.IsangoHotelBedsActivityId.ToString();

                                var matchedOptions = criteria?.MappedProducts?.Where(x => x.IsangoHotelBedsActivityId == activity.ID);
                                //match with isango data
                                foreach (IsangoHBProductMapping mappedProductItem in matchedOptions)
                                {
                                    try
                                    {
                                        var databaseServiceCode = mappedProductItem.HotelBedsActivityCode;
                                        var isangoData = activity.ProductOptions?.Where(x => ((ActivityOption)x).Code == databaseServiceCode)?.FirstOrDefault();
                                        if (isangoData != null)
                                        {
                                            isangoData.Id = mappedProductItem.ServiceOptionInServiceid;
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }

                            //Temp code ends
                            if (activity != null)
                            {
                                activitiesList.Add(activity);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("GlobalTixCriteriaService|GetAvailability", ex);
                        }
                    }

                    return activitiesList;
                }
            }
            catch (Exception ex)
            {
                _log.Error("CanocalizationService|GetAvailability", ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ConsoleCriteria.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts,
            PriceDataType priceDataType, APIType apiType)
        {
            var serviceDetails = new List<ConsoleCriteria.TempHBServiceDetail>();
            try
            {
                if (apiType == APIType.RedeamV12)
                {

                    foreach (var activity in activities)
                    {
                        if (activity == null) continue;

                        var mappedProduct = mappedProducts
                                            .FirstOrDefault(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)
                                                            && activity.Code == x.HotelBedsActivityCode
                                            );

                        if (mappedProduct == null) continue;
                        var serviceMapper = new ConsoleService.ServiceMapper();
                        var details = new List<TempHBServiceDetail>();
                        if (priceDataType == PriceDataType.Cost)
                        {
                            details = serviceMapper.ProcessServiceDetailsWithCostPrice(activity, mappedProduct);
                        }
                        else if (priceDataType == PriceDataType.Sell)
                        {
                            var mappedProductsById = mappedProducts?.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)).ToList();
                            details = serviceMapper.ProcessServiceDetailsWithBasePrice(activity, mappedProductsById);
                        }
                        else if (priceDataType == PriceDataType.CostAndSell)
                        {
                            details = serviceMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProduct);
                        }
                        if (details != null)
                        {
                            foreach (var item in details)
                            {
                                //if (!serviceDetails.Contains(item))
                                if (!serviceDetails.Any(prod => prod?.ActivityId == item?.ActivityId
                                && prod.AvailableOn == item?.AvailableOn
                                && prod.StartTime == item?.StartTime
                                && prod.ServiceOptionID == item?.ServiceOptionID
                                && prod.ProductCode == item?.ProductCode))

                                {
                                    serviceDetails.Add(item);
                                }
                                //serviceDetails.AddRange(details);
                            }
                        }
                    }

                    return serviceDetails;
                }
                else if (apiType == APIType.GlobalTixV3)
                {
                    ServiceMapper svcMapper = new ServiceMapper();
                    foreach (Activity activity in activities)
                    {
                        try
                        {
                            //activityid of MappedData 
                            var getActivityid = activity?.Id; //same for all options
                                                              //Filter Get All Options of activityid
                            var mappedProductsList = mappedProducts?.Where(x => x.IsangoHotelBedsActivityId.ToString() == getActivityid)?.ToList();
                            if (mappedProductsList != null && mappedProductsList.Count > 0)
                            {
                                //Filter All Options ids of activityid
                                var lstofOptionsId = mappedProductsList?.Select(x => x.HotelBedsActivityCode)?.ToList();
                                //Filter activity  options based on 
                                if (lstofOptionsId != null && lstofOptionsId.Count > 0)
                                {
                                    activity.ProductOptions.RemoveAll(x => !lstofOptionsId.Contains(((ActivityOption)x).Code));
                                }
                                //activity?.ProductOptions?.ForEach(x => x.Id = x.ServiceOptionId);
                                foreach (var mappedProduct in mappedProductsList)
                                {
                                    List<TempHBServiceDetail> svcDetailsForActivity = svcMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProduct);
                                    if (svcDetailsForActivity != null)
                                    {
                                        serviceDetails.AddRange(svcDetailsForActivity);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("GlobalTixCriteriaService|GetServiceDetails", ex);
                        }
                    }
                    return serviceDetails;
                }
                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("CanocalizationService|GetServiceDetails", ex);
                throw;
            }
        }

        #endregion

        #region [CheckAvailability]
        public Activity GetAvailability(Activity activity, CanocalizationCriteria criteria, string token)
        {
            try
            {
                if (activity.ApiType == APIType.RedeamV12)
                {
                    var redeamProductOptions = _redeamV12Adapter.GetAvailabilities(criteria, token)?.GetAwaiter().GetResult();
                    if (redeamProductOptions?.Count > 0)
                    {
                        var apiActivity = LoadMappedDataAPI(activity, redeamProductOptions, criteria, null);
                        return apiActivity;
                    }
                    return activity;
                }
                else if (activity.ApiType == APIType.GlobalTixV3)
                {
                    var globalTixCriteria = (CanocalizationCriteria)criteria;
                    bool isNonThailandProduct = !activity.Regions?.Where(x => x.Type == Entities.Enums.RegionType.Country)?.FirstOrDefault().Name?.ToLower().Equals("thailand") ?? true;
                    var multipleOptionIds = string.Join(",", activity.ProductOptions.Select(x => x.SupplierOptionCode)?.ToList());
                    globalTixCriteria.ActivityIdStr = multipleOptionIds;
                    var getActivity = _globalTixAdapterV3.GetActivityInformation(globalTixCriteria, token, isNonThailandProduct) as Activity;
                    var activities = new List<Activity>();
                    if (getActivity != null)
                    {
                        activities.Add(getActivity);
                        if (activities?.Count > 0)
                        {
                            if (activities.FirstOrDefault().ProductOptions == null)
                            {
                                var message = Constant.APIActivityOptionsNot + Constant.GlobalTixAPI + " .Id:" + activity.ID;
                                SendException(activity.ID, message);
                            }
                            return LoadMappedDataAPI(activity, null, criteria, activities);
                        }
                    }
                    else
                    {
                        var message = Constant.APIActivityNot + Constant.GlobalTixAPI + " .Id:" + activity.ID;
                        SendException(activity.ID, message);
                    }
                    return activity;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CanocalizationServie",
                    MethodName = "GetAvailability",
                    Token = token,
                    Params = $"criteria:{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return null;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "CanocalizationService",
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

        private Activity LoadMappedDataAPI(Activity activity, List<ProductOption> optionsFromAPI,
            CanocalizationCriteria criteria, List<Activity> activitiesFromAPI)
        {
            if (activity.ApiType == APIType.RedeamV12)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOption in optionsFromAPI)
                {
                    var activityOptionFromAPI = (ActivityOption)activityOption;
                    var activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                        x => (x).SupplierOptionCode == activityOptionFromAPI.SupplierOptionCode);
                    if (activityOptionFromCache == null) continue;

                    var option = GetMappedActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
                    option.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                    option.PickupLocations = activityOptionFromAPI.PickupLocations;
                    option.StartTime = activityOptionFromAPI.StartTime;
                    option.RedeamAvailabilityId = activityOptionFromAPI.RedeamAvailabilityId;
                    option.RedeamAvailabilityStart = activityOptionFromAPI.RedeamAvailabilityStart;
                    // Update name of Option with time from API
                    if (!string.IsNullOrWhiteSpace(activityOptionFromAPI.Name)
                        && (activityOptionFromAPI.Name != Constant.ZeroTime)
                        )
                        option.Name = $"{activityOptionFromCache.Name.TrimEnd()} @ {activityOptionFromAPI.Name}";
                    productOptions.Add(option);
                }
                activity.ProductOptions = productOptions;
                if (activity.ProductOptions.Count == 0)
                {
                    activity.ProductOptions = optionsFromAPI;
                }
                return activity;
            }
            else if (activity.ApiType == APIType.GlobalTixV3)
            {
                var activityFromAPI = activitiesFromAPI.FirstOrDefault(act => act.ID == activity.FactsheetId);
                if (activityFromAPI != null)
                {
                    var productOptions = new List<ProductOption>();
                    foreach (var activityOptionFromCache in activity.ProductOptions)
                    {
                        var activityOptionFromAPILst = activityFromAPI.ProductOptions.Where(
                            x => ((ActivityOption)x).Code.Trim() == activityOptionFromCache.SupplierOptionCode.Trim())?.ToList();
                        if (activityOptionFromAPILst != null && activityOptionFromAPILst.Count > 0)
                        {
                            foreach (var activityOptionFromAPI in activityOptionFromAPILst)
                            {
                                var option = GetMappedActivityOption((ActivityOption)activityOptionFromAPI, activityOptionFromCache, criteria);
                                if (option != null)
                                {
                                    option.ContractQuestionForGlobalTix3 = ((ActivityOption)activityOptionFromAPI).ContractQuestionForGlobalTix3;
                                    if (activityOptionFromAPI.StartTime != null && activityOptionFromAPI.StartTime != TimeSpan.Zero)
                                    {
                                        option.StartTime = activityOptionFromAPI.StartTime;
                                        option.Id = activityOptionFromAPI.Id;
                                        //option.Name = option.Name +" @ "+ option.StartTime;
                                    }
                                    productOptions.Add(option);
                                }
                            }
                        }
                    }
                    activity.ProductOptions = productOptions;
                    if (activity.ProductOptions.Count == 0)
                    {
                        activity.ProductOptions = activityFromAPI.ProductOptions;
                    }
                }
                return activity;

            }
            return null;
        }
        /// <summary>
        /// Map ActivityOption data from source to destination object
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="productOption"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private ActivityOption GetMappedActivityOption(ActivityOption option, ProductOption productOption, CanocalizationCriteria criteria)
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

                Id = productOption.Id,
                ServiceOptionId = productOption.Id,
                Name = productOption.Name,
                SupplierName = productOption.SupplierName,
                Description = productOption.Description,
                BookingStatus = productOption.BookingStatus,
                OptionKey = productOption.OptionKey,
                Capacity = productOption.Capacity,
                Quantity = productOption.Quantity,
                SupplierOptionCode = productOption.SupplierOptionCode,
                Margin = productOption.Margin,
                CancellationText = option.CancellationText,
                ApiCancellationPolicy = option.ApiCancellationPolicy,
            };
        }

        public Price CalculatePriceForAllPax(Price inputPrice, CanocalizationCriteria criteria)
        {
            if (inputPrice == null) return null;
            var price = inputPrice.DeepCopy();

            var isPerUnit = false;
            var perUnitPrice = new decimal();
            var perPersonPrice = new decimal();

            if (price?.DatePriceAndAvailabilty?.Any() == true)
            {
                foreach (var priceAndAvailability in price?.DatePriceAndAvailabilty)
                {
                    perPersonPrice = 0.0M;
                    if (priceAndAvailability.Value?.PricingUnits == null) continue;
                    var pricingUnits = priceAndAvailability.Value.PricingUnits;
                    foreach (var pricingUnit in pricingUnits)
                    {
                        if (pricingUnit is PerUnitPricingUnit)
                        {
                            perUnitPrice = pricingUnit.Price;
                            isPerUnit = true;
                        }
                        else
                        {
                            var pu = ((PerPersonPricingUnit)pricingUnit);
                            var passengerType = pu.PassengerType;
                            var paxCount = pu.UnitType == UnitType.PerPerson ?
                                            GetPaxCountByPaxType(criteria, passengerType) :
                                            priceAndAvailability.Value?.UnitQuantity > 0 ?
                                                priceAndAvailability.Value.UnitQuantity : 1
                                                ;
                            perPersonPrice += pricingUnit.Price * paxCount;
                        }
                    }

                    priceAndAvailability.Value.TotalPrice = isPerUnit ? perUnitPrice : perPersonPrice;
                }
                price.Amount = price.DatePriceAndAvailabilty.
                    Select(x => x.Value.TotalPrice).
                    FirstOrDefault();
            }
            return price;
        }


        private int GetPaxCountByPaxType(CanocalizationCriteria criteria, PassengerType passengerType) => criteria.NoOfPassengers.Where(x => x.Key == passengerType).Select(x => x.Value).FirstOrDefault();

        public CanocalizationCriteria CreateCriteria(Activity activity, Entities.Criteria criteria, ClientInfo clientInfo)
        {
            if (activity.ApiType == APIType.RedeamV12)
            {
                var supplierId = activity.Code;
                var productId = string.Empty;
                var rateIds = new List<string>();
                /*checking for activity code to get supplier id and product id and
                                if length after spiting code by # is 2, then both supplier id and product id are present*/
                activity.ProductOptions = activity?.ProductOptions?.Where(x => x?.SupplierOptionCode?.Split('#').Length == 2).ToList();
                if (!string.IsNullOrWhiteSpace(activity?.Code) && activity?.ProductOptions?.Count > 0)
                {
                    //var splitResult = activity.Code.Split('#');

                    rateIds = activity?.ProductOptions?
                        .Where(y => !string.IsNullOrWhiteSpace(y.SupplierOptionCode))?
                        .Select(x => x.SupplierOptionCode)?
                        .Distinct().ToList();

                    var rateIdAndType = new Dictionary<string, string>();

                    activity?.ProductOptions?.ForEach(x =>
                    {
                        if (!rateIdAndType.Keys.Contains(x.SupplierOptionCode))
                        {
                            rateIdAndType.Add(x.SupplierOptionCode, x.PrefixServiceCode);
                        }
                    });

                    var apiCriteria = new CanocalizationCriteria
                    {
                        CheckinDate = criteria.CheckinDate,
                        CheckoutDate = criteria.CheckoutDate,
                        NoOfPassengers = criteria.NoOfPassengers,
                        SupplierId = supplierId,
                        ProductId = productId,
                        RateIds = rateIds,
                        RateIdAndType = rateIdAndType,
                        ApiToken = clientInfo.ApiToken,
                        Token = criteria.Token,
                        Language = criteria.Language,
                        Currency = criteria.CurrencyFromDataBase
                    };

                    return apiCriteria;
                }
                else
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "CanocalizationService",
                        MethodName = "CreateCriteria",
                        Token = clientInfo.ApiToken,
                        AffiliateId = clientInfo.AffiliateId,
                        Params = $"supplierId:-{supplierId},productId:-{productId}, RateIds:-{string.Join(",", rateIds?.ToArray())}"
                    };
                    _log.Error(isangoErrorEntity, new Exception("Supplier Ids and Product Ids not found."));
                }
            }
            else if (activity.ApiType == APIType.GlobalTixV3)
            {
                var distinctProductOptionIds = activity.ProductOptions.Select(x => x.Id).ToList();

                var globalTixV3Mapping = _masterCacheManager.GetGlobalTixV3Mappings().Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
                if (globalTixV3Mapping == null)
                {
                    return null;
                }

                var globalTixCriteria = new CanocalizationCriteria
                {
                    ActivityIdStr = activity.Id.ToString(),
                    FactSheetId = activity.FactsheetId,
                    CheckinDate = criteria.CheckinDate,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Days2Fetch = 1,
                    Token = criteria.Token,
                    Language = criteria.Language,
                    GlobalTixV3Mapping = globalTixV3Mapping
                };
                if (criteria.CheckoutDate != null && criteria.CheckinDate != null)
                {
                    globalTixCriteria.Days2Fetch = System.Convert.ToInt32(((criteria.CheckoutDate - criteria.CheckinDate).TotalDays) + 1);
                }
                return globalTixCriteria;
            }
            return null;
        }

        #endregion

        private dynamic GetAPIReservationMapper(APIType apiType, ReservationAPIEnum type,
            CanocalizationSelectedProduct canoSelectedProduct, SelectedProduct selectedProduct,
            string token, int bookingId, object apiProductResponse, out string request, out string response,
            out HttpStatusCode httpStatusCode)
        {

            if (type == ReservationAPIEnum.APIReservationEntity)// API reservation Entity
            {
                request = "";
                response = "";
                httpStatusCode = HttpStatusCode.BadGateway;
                if (apiType.ToString().ToLower() == APIType.RedeamV12.ToString().ToLower())
                {
                    return new Dictionary<string, ServiceAdapters.RedeamV12.RedeamV12.Entities.CreateHold.CreateHoldResponse>();
                }
                else if (apiType.ToString().ToLower() == APIType.GlobalTixV3.ToString().ToLower())
                {
                    return new Dictionary<string, ServiceAdapters.GlobalTixV3.GlobalTix.Entities.ReservationRS>();
                }

            }
            else if (type == ReservationAPIEnum.APIHoldableorNot)// API Holdable or not
            {
                request = "";
                response = "";
                httpStatusCode = HttpStatusCode.BadGateway;
                if (apiType.ToString().ToLower() == APIType.RedeamV12.ToString().ToLower())
                {
                    return ((ActivityOption)canoSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected))?.Holdable ?? false;
                }
                else if (apiType.ToString().ToLower() == APIType.GlobalTixV3.ToString().ToLower())
                {
                    return true;
                }
            }
            else if (type == ReservationAPIEnum.APIReservationResponse)// API Reservation Response
            {
                if (apiType.ToString().ToLower() == APIType.RedeamV12.ToString().ToLower())
                {
                    request = "";
                    response = "";
                    httpStatusCode = HttpStatusCode.BadGateway;

                    return (CreateHoldResponse)_redeamV12Adapter.CreateHoldAPIOnly(selectedProduct, token, out request, out response, out httpStatusCode);
                }
                else if (apiType.ToString().ToLower() == APIType.GlobalTixV3.ToString().ToLower())
                {
                    return (ReservationRS)_globalTixAdapterV3.CreateReservation(canoSelectedProduct, canoSelectedProduct.BookingReferenceNumber.ToString(), token, out request, out response, out httpStatusCode);
                }
            }
            else if (type == ReservationAPIEnum.APIHoldId) //API hold id
            {
                request = "";
                response = "";
                httpStatusCode = HttpStatusCode.BadGateway;
                if (apiType.ToString().ToLower() == APIType.RedeamV12.ToString().ToLower())
                {
                    return ((CreateHoldResponse)apiProductResponse).Hold.Id.ToString();
                }
                else if (apiType.ToString().ToLower() == APIType.GlobalTixV3.ToString().ToLower())
                {
                    return ((ReservationRS)apiProductResponse).Data.ReferenceNumber;
                }
            }
            else if (type == ReservationAPIEnum.APIHoldStatus)//API hold Status
            {
                request = "";
                response = "";
                httpStatusCode = HttpStatusCode.BadGateway;
                if (apiType.ToString().ToLower() == APIType.RedeamV12.ToString().ToLower())
                {
                    return ((CreateHoldResponse)apiProductResponse).Hold.Status;
                }
                else if (apiType.ToString().ToLower() == APIType.GlobalTixV3.ToString().ToLower())
                {
                    return ((ReservationRS)apiProductResponse).Data.Status;
                }
            }

            request = "";
            response = "";
            httpStatusCode = HttpStatusCode.BadGateway;
            return null;
        }

        #region [Reservation]
        public List<BookedProduct> CreateReservation(CanocalizationActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = new List<SelectedProduct>();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            HttpStatusCode httpStatusCode = HttpStatusCode.BadGateway;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(criteria.APIType)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = criteria.APIType,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    dynamic createOrderProducts = GetAPIReservationMapper(criteria.APIType, ReservationAPIEnum.APIReservationEntity, null, null, criteria.Token, criteria.Booking.BookingId, null, out request, out response, out httpStatusCode);
                    foreach (var product in selectedProducts)
                    {
                        var canoSelectedProduct = product;
                        try
                        {
                            _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var selectedProduct = product as CanocalizationSelectedProduct;

                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };

                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;
                        ((CanocalizationSelectedProduct)canoSelectedProduct).BookingReferenceNumber = criteria?.Booking?.ReferenceNumber ?? string.Empty;
                        selectedProduct.BookingReferenceNumber = criteria?.Booking?.ReferenceNumber ?? string.Empty;
                        var isHoldable = false;
                        isHoldable = GetAPIReservationMapper(criteria.APIType, ReservationAPIEnum.APIHoldableorNot, selectedProduct, null, criteria.Token, criteria.Booking.BookingId, null, out request, out response, out httpStatusCode);


                        //Reservation API Call if ticket class is two or three


                        //Set values in  selectedProduct as PrioReservationReference, PrioDistributorReference and PrioBookingStatus
                        if (isHoldable)
                        {
                            dynamic apiProductResponse = new object();
                            apiProductResponse = GetAPIReservationMapper(criteria.APIType, ReservationAPIEnum.APIReservationResponse, selectedProduct, selectedProduct, criteria.Token, criteria.Booking.BookingId, null, out request, out response, out httpStatusCode);

                            if (apiProductResponse != null)
                            {

                                ((CanocalizationSelectedProduct)canoSelectedProduct).HoldId = GetAPIReservationMapper(criteria.APIType, ReservationAPIEnum.APIHoldId, selectedProduct, null, criteria.Token, criteria.Booking.BookingId, apiProductResponse, out request, out response, out httpStatusCode);
                                ((CanocalizationSelectedProduct)canoSelectedProduct).HoldStatus = GetAPIReservationMapper(criteria.APIType, ReservationAPIEnum.APIHoldStatus, selectedProduct, null, criteria.Token, criteria.Booking.BookingId, apiProductResponse, out request, out response, out httpStatusCode);

                                var reservationDetails = new SupplierBookingReservationResponse()
                                {
                                    ApiType = Convert.ToInt32(criteria.APIType),
                                    ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                    AvailabilityReferenceId = product.AvailabilityReferenceId,
                                    Status = apiProductResponse != null ? Constant.StatusSuccess : Constant.StatusFailed,
                                    BookedOptionId = criteria.Booking.BookingId,
                                    BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                    OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                                    ReservationResponse = SerializeDeSerializeHelper.Serialize(apiProductResponse),
                                    ReservationReferenceId = ((CanocalizationSelectedProduct)canoSelectedProduct).HoldId.ToString(),
                                    Token = criteria.Token
                                };

                                _tableStorageOperation.InsertReservationDetails(reservationDetails);

                                InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                                try
                                {
                                    _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }

                                createOrderProducts.Add(product.AvailabilityReferenceId, apiProductResponse);
                            }
                            else
                            {
                                InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusFailed);
                                var errorMessageData = string.Empty;

                                if (criteria.APIType == APIType.RedeamV12)
                                {
                                    var getData = SerializeDeSerializeHelper.DeSerialize<CreateHoldResponse>(response.ToString());
                                    //Api booking failed
                                    errorMessageData = getData?.Error?.Message;
                                }
                                else if (criteria.APIType == APIType.GlobalTixV3)
                                {
                                    var getData = SerializeDeSerializeHelper.DeSerialize<ReservationRS>(response.ToString());
                                    errorMessageData = String.IsNullOrEmpty(getData?.Error?.Message) ? getData?.Error?.Code : getData?.Error?.Message;
                                }
                                //If the create order call is failed for any of the product then set all product booking status as failed
                                bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , httpStatusCode == HttpStatusCode.OK ? HttpStatusCode.BadGateway : httpStatusCode
                                , errorMessageData);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                         ((CanocalizationSelectedProduct)canoSelectedProduct).HoldId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(criteria.APIType), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }

                                return bookedProducts;
                            }
                        }


                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }

                    foreach (var product in createOrderProducts)
                    {
                        var commonAPISelectedProduct = selectedProducts.OfType<CanocalizationSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);
                        try
                        {
                            commonAPISelectedProduct.ProductOptions = UpdateOptionStatus(commonAPISelectedProduct.ProductOptions, ((CanocalizationSelectedProduct)commonAPISelectedProduct).HoldId);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == commonAPISelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;
                            if (criteria.APIType == APIType.RedeamV12)
                            {
                                bookedProduct = MapProductForRedeamV12(bookedProduct, commonAPISelectedProduct);
                            }
                            else if (criteria.APIType == APIType.GlobalTixV3)
                            {
                                bookedProduct.APIExtraDetail = commonAPISelectedProduct.APIDetails;
                            }
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            commonAPISelectedProduct.ProductOptions = UpdateOptionStatus(commonAPISelectedProduct.ProductOptions, ((CanocalizationSelectedProduct)commonAPISelectedProduct).HoldId);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == commonAPISelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = commonAPISelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(((CanocalizationSelectedProduct)commonAPISelectedProduct).HoldId, option.AvailabilityStatus);
                            bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                            if (!string.IsNullOrWhiteSpace(((CanocalizationSelectedProduct)commonAPISelectedProduct).HoldId))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = ((CanocalizationSelectedProduct)commonAPISelectedProduct).HoldId;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "CanocalizationService",
                                MethodName = "CreateReservation",
                                Token = criteria.Token,
                                Params = $"{commonAPISelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(commonAPISelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, commonAPISelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                               ((CanocalizationSelectedProduct)commonAPISelectedProduct).HoldId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(criteria.APIType), commonAPISelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                commonAPISelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, commonAPISelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                               , System.Net.HttpStatusCode.BadGateway
                               , $"Exception\n {ex.Message}\n{response}");
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CanocalizationService",
                    MethodName = "CreateReservation",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }



        private List<BookedProduct> SetFailedBookingStatus(List<SelectedProduct> apiSelectedProducts, CanocalizationActivityBookingCriteria activityBookingCriteria)
        {
            var bookedProducts = new List<BookedProduct>();
            foreach (var apiProduct in apiSelectedProducts)
            {
                var failedProduct = activityBookingCriteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == apiProduct.AvailabilityReferenceId);
                if (failedProduct == null) continue;
                failedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                failedProduct.Errors = activityBookingCriteria.Booking.Errors;
                bookedProducts.Add(failedProduct);
            }

            return bookedProducts;
        }
        private void InsertLogPurchaseInDb(LogPurchaseXmlCriteria logPurchaseXmlCriteria, string requestXml, string responseXml, string methodName, string status)
        {
            logPurchaseXmlCriteria.RequestXml = requestXml;
            logPurchaseXmlCriteria.ResponseXml = responseXml;
            logPurchaseXmlCriteria.Bookingtype = methodName;
            logPurchaseXmlCriteria.Status = status;
            _supplierBookingPersistence.LogPurchaseXML(logPurchaseXmlCriteria);
        }
        #endregion


        #region[Cancel Booking]
        /// <summary>
        /// Cancel booking call for Redeam
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelBooking(List<SelectedProduct> selectedProducts,
            string token, APIType apitype)
        {
            var cancellationStatus = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;
            HttpStatusCode httpStatusCode = HttpStatusCode.BadGateway;

            var bookingReferenceNumbers = new List<string>();
            if (apitype == APIType.RedeamV12)
            {
                bookingReferenceNumbers = selectedProducts?.Select(x => ((CanocalizationSelectedProduct)x).BookingReferenceNumber)?.Distinct()?.ToList();
                selectedProducts?.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(((CanocalizationSelectedProduct)x)?.BookingReferenceNumber) && !cancellationStatus.Keys.Contains(((CanocalizationSelectedProduct)x).BookingReferenceNumber))
                    {
                        cancellationStatus.Add(((CanocalizationSelectedProduct)x).BookingReferenceNumber, false);
                    }
                });
            }
            else if (apitype == APIType.GlobalTixV3)
            {
                bookingReferenceNumbers = selectedProducts?.Select(x => ((CanocalizationSelectedProduct)x)?.APIDetails?.SupplieReferenceNumber)?.Distinct()?.ToList();
                selectedProducts?.ForEach(x =>
                {
                    cancellationStatus.Add(x?.AvailabilityReferenceId, false);
                });
            }

            foreach (var bookingReferenceNumber in bookingReferenceNumbers)
            {
                if (!string.IsNullOrWhiteSpace(bookingReferenceNumber))
                {
                    try
                    {
                        var result = false;
                        if (apitype == APIType.RedeamV12)
                        {
                            result = _redeamV12Adapter.CancelBooking(bookingReferenceNumber, token, out request, out response, out httpStatusCode);
                            cancellationStatus[bookingReferenceNumber] = result;
                        }
                        else if (apitype == APIType.GlobalTixV3)
                        {
                            var productSelected = selectedProducts.Find(thisProduct => ((CanocalizationSelectedProduct)thisProduct).APIDetails.SupplieReferenceNumber.Equals(bookingReferenceNumber));
                            bool isNonThailandProduct = false;
                            if (productSelected.RegionId != null)
                            {

                                isNonThailandProduct = !productSelected.RegionId.ToLowerInvariant().Equals("6667");
                            }

                            result = _globalTixAdapterV3.CancelByBooking(bookingReferenceNumber, token, out request, out response, out httpStatusCode, isNonThailandProduct);
                            cancellationStatus[productSelected?.AvailabilityReferenceId] = result;
                        }

                    }
                    catch (Exception ex)
                    {
                        _log.Error($"CanocalizationService|CancelBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                    }
                }
            }

            foreach (var apiSelectedProduct in selectedProducts)
            {
                try
                {
                    var isCancelled = false;
                    if (apitype == APIType.RedeamV12)
                    {
                        isCancelled = cancellationStatus?.FirstOrDefault(x => x.Key == ((CanocalizationSelectedProduct)apiSelectedProduct)?.BookingReferenceNumber).Value ?? false;
                    }
                    else if (apitype == APIType.GlobalTixV3)
                    {
                        isCancelled = cancellationStatus?.FirstOrDefault(x => x.Key == ((CanocalizationSelectedProduct)apiSelectedProduct)?.AvailabilityReferenceId).Value ?? false;
                    }
                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = apitype,
                        Bookingtype = "Cancel Booking"
                    };
                    if (apitype == APIType.RedeamV12)
                    {
                        logCriteria.BookingReferenceNumber = ((CanocalizationSelectedProduct)apiSelectedProduct).BookingReferenceNumber;
                    }
                    else if (apitype == APIType.GlobalTixV3)
                    {
                        //logCriteria.BookingReferenceNumber = ((CanocalizationSelectedProduct)apiSelectedProduct)?.APIDetails?.SupplieReferenceNumber;
                        logCriteria.BookingReferenceNumber = ((CanocalizationSelectedProduct)apiSelectedProduct).BookingReferenceNumber;
                        logCriteria.ApiRefNumber = ((CanocalizationSelectedProduct)apiSelectedProduct)?.APIDetails?.SupplieReferenceNumber;
                    }

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    _log.Error($"CanocalizationService|CancelBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                }
            }
            return cancellationStatus;
        }

        #endregion

        #region [Booking]

        private dynamic GetAPIBoookingMapper(APIType aPIType, BookingAPIEnum bookingEnum,
            SelectedProduct selectedProduct, SupplierBookingReservationResponse reservationDetails,
            string token, string refNo, out string request, out string response, out HttpStatusCode httpStatusCode)
        {

            if (bookingEnum == BookingAPIEnum.APIHoldable)
            {
                request = "";
                response = "";
                httpStatusCode = HttpStatusCode.BadGateway;
                if (aPIType == APIType.GlobalTixV3)
                {
                    return true;
                }
                else if (aPIType == APIType.RedeamV12)
                {
                    return ((ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected))?.Holdable ?? false;
                }
            }
            else if (bookingEnum == BookingAPIEnum.APIReservationResponse)
            {
                request = "";
                response = "";
                httpStatusCode = HttpStatusCode.BadGateway;
                if (aPIType == APIType.GlobalTixV3)
                {
                    return SerializeDeSerializeHelper.DeSerialize<ReservationRS>(reservationDetails?.ReservationResponse);
                }
                else if (aPIType == APIType.RedeamV12)
                {
                    return SerializeDeSerializeHelper.DeSerialize<CreateHoldResponse>(reservationDetails?.ReservationResponse);
                }

            }
            else if (bookingEnum == BookingAPIEnum.APIReservationCreate)
            {
                if (aPIType == APIType.GlobalTixV3)
                {

                    var refNumber = ((CanocalizationSelectedProduct)selectedProduct).BookingReferenceNumber;
                    var apiResponse = _globalTixAdapterV3.CreateReservation(selectedProduct, refNumber, token, out request, out response, out httpStatusCode);
                    ((CanocalizationSelectedProduct)selectedProduct).HoldId = apiResponse?.Data?.ReferenceNumber;
                    ((CanocalizationSelectedProduct)selectedProduct).HoldStatus = apiResponse?.Data?.Status;
                    return selectedProduct;
                }
                else if (aPIType == APIType.RedeamV12)
                {
                    request = "";
                    response = "";
                    httpStatusCode = HttpStatusCode.BadGateway;
                    return _redeamV12Adapter.CreateHold(selectedProduct, token, out request, out response, out httpStatusCode);
                }
            }

            else if (bookingEnum == BookingAPIEnum.APICreateBooking)
            {

                if (aPIType == APIType.GlobalTixV3)
                {
                    request = "";
                    response = "";
                    return _globalTixAdapterV3.CreateBooking(selectedProduct, refNo, token, out request, out response, out httpStatusCode);
                }
                else if (aPIType == APIType.RedeamV12)
                {
                    //if supplier email exist use that otherwise use isango support email.
                    var email = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunNotificationEmailAddressIsango"));
                    ((CanocalizationSelectedProduct)selectedProduct).SupplierEmail = email;
                    return _redeamV12Adapter.CreateBooking(selectedProduct, token, out request, out response, out httpStatusCode);
                }
            }
            request = "";
            response = "";
            httpStatusCode = HttpStatusCode.BadGateway;
            return null;
        }

        ///<summary>
        ///Create Redeam bookings
        ///</summary>
        ///<param name="criteria"></param>
        ///<returns></returns>
        public List<BookedProduct> CreateBooking(CanocalizationActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            HttpStatusCode httpStatusCode = HttpStatusCode.BadGateway;
            var selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(criteria.APIType)).ToList();
            try
            {
                foreach (var selectedProduct in selectedProducts)
                {
                    var apiDataSelectedProduct = selectedProduct;
                    ((CanocalizationSelectedProduct)apiDataSelectedProduct).BookingReferenceNumber = criteria?.Booking?.ReferenceNumber ?? string.Empty;
                    var isHoldable = (bool)GetAPIBoookingMapper(criteria.APIType, BookingAPIEnum.APIHoldable, selectedProduct, null,
                        criteria.Token, "", out request, out response, out httpStatusCode);
                    if (isHoldable)
                    {

                        //Start- Get Reservation from Storage
                        var rowKey = $"{criteria.Booking.ReferenceNumber}-{selectedProduct.AvailabilityReferenceId}";
                        var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                        var createOrderResponse = GetAPIBoookingMapper(criteria.APIType, BookingAPIEnum.APIReservationResponse, apiDataSelectedProduct,
                            ReservationDetails, criteria.Token, ReservationDetails?.BookingReferenceNo, out request, out response, out httpStatusCode);

                        if (ReservationDetails != null)
                        {
                            APIReservationGet((CanocalizationSelectedProduct)apiDataSelectedProduct, createOrderResponse, criteria.Booking, criteria.Token, criteria.APIType);
                        }
                        else
                        {
                            // Create Hold before Booking
                            apiDataSelectedProduct = GetAPIBoookingMapper(criteria.APIType, BookingAPIEnum.APIReservationCreate, apiDataSelectedProduct, ReservationDetails, criteria.Token, "", out request, out response, out httpStatusCode);
                        }

                        // Prepare BookedProduct with Failed Status
                        if (apiDataSelectedProduct == null || ((CanocalizationSelectedProduct)selectedProduct).HoldId == null)
                        {
                            // Mapping the booked product of the IsangoBookingData as per the fail booking
                            var commonBookedProduct = criteria?.Booking?.IsangoBookingData?.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            if (commonBookedProduct == null) continue;

                            commonBookedProduct.Errors = criteria.Booking.Errors;

                            if (criteria.APIType == APIType.RedeamV12)
                            {

                                var getData = SerializeDeSerializeHelper.DeSerialize<CreateHoldResponse>(response.ToString());
                                //Api booking failed
                                var errorMessageData = getData?.Error?.Message;
                                criteria.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                        , httpStatusCode == HttpStatusCode.OK ? HttpStatusCode.BadGateway : httpStatusCode
                                        , errorMessageData);

                                commonBookedProduct = MapProductForRedeamV12(commonBookedProduct, selectedProduct as CanocalizationSelectedProduct);
                            }
                            else if (criteria.APIType == APIType.GlobalTixV3)
                            {

                                var getData = SerializeDeSerializeHelper.DeSerialize<ReservationRS>(response.ToString());
                                //Api booking failed
                                var errorMessageData = String.IsNullOrEmpty(getData?.Error?.Message) ? getData?.Error?.Code : getData?.Error?.Message;
                                criteria.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                        , httpStatusCode == HttpStatusCode.OK ? HttpStatusCode.BadGateway : httpStatusCode
                                        , errorMessageData);
                                //commonBookedProduct.APIExtraDetail = commonAPISelectedProduct.APIDetails;
                            }
                            commonBookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                            bookedProducts.Add(commonBookedProduct);
                            return bookedProducts;
                        }
                    }
                    // Supplier Create Booking call initiated
                    apiDataSelectedProduct = GetAPIBoookingMapper(criteria.APIType, BookingAPIEnum.APICreateBooking, apiDataSelectedProduct, null, criteria.Token, ((CanocalizationSelectedProduct)selectedProduct)?.HoldId, out request, out response, out httpStatusCode);
                    //Prepare BookedProduct with Failed Status
                    if (apiDataSelectedProduct == null)
                    {
                        var errorMessageData = string.Empty;
                        if (criteria.APIType == APIType.GlobalTixV3)
                        {
                            var getData = SerializeDeSerializeHelper.DeSerialize<BookingDetailsResponse>(response.ToString());
                            //Api booking failed
                            errorMessageData = String.IsNullOrEmpty(getData?.error?.Message) ? getData?.error?.Code : getData?.error?.Message;
                        }
                        else if (criteria.APIType == APIType.RedeamV12)
                        {
                            var getData = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.RedeamV12.RedeamV12.Entities.CreateBooking.CreateBookingResponse>(response.ToString());
                            //Api booking failed
                            errorMessageData = getData?.Error?.Message;
                        }
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , httpStatusCode == HttpStatusCode.OK ? HttpStatusCode.BadGateway : httpStatusCode
                                , errorMessageData);
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, apiDataSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(criteria.APIType), apiDataSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            apiDataSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, apiDataSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }

                        // Mapping the booked product of the IsangoBookingData as per the fail booking
                        var cmnBookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                        if (cmnBookedProduct == null) continue;
                        cmnBookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        cmnBookedProduct.Errors = criteria.Booking.Errors;
                        if (criteria.APIType == APIType.RedeamV12)
                        {
                            cmnBookedProduct = MapProductForRedeamV12(cmnBookedProduct, selectedProduct as CanocalizationSelectedProduct);
                        }
                        else if (criteria.APIType == APIType.GlobalTixV3)
                        {
                            var selectedOption = (ActivityOption)selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
                            cmnBookedProduct.OptionStatus = GetBookingStatusNumber(((CanocalizationSelectedProduct)selectedProduct)?.BookingReferenceNumber, selectedOption?.AvailabilityStatus);
                        }
                        cmnBookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(cmnBookedProduct);
                        return bookedProducts;
                    }
                    var commonSelectedBookedProduct = (CanocalizationSelectedProduct)apiDataSelectedProduct;

                    //Setting the booking status of all the product options by checking the supplier's TicketReferenceNumber
                    apiDataSelectedProduct.ProductOptions = UpdateOptionStatus(commonSelectedBookedProduct.ProductOptions, commonSelectedBookedProduct.BookingReferenceNumber);

                    // Logging the supplier booking details in the DB
                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = !string.IsNullOrWhiteSpace(commonSelectedBookedProduct.BookingReferenceNumber)
                            ? Constant.StatusSuccess
                            : Constant.StatusFailed,
                        BookingId = criteria.Booking.BookingId,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                        APIType = criteria.APIType,
                        ApiRefNumber = commonSelectedBookedProduct.BookingReferenceNumber
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                    // Mapping the booked product of the IsangoBookingData as per the success/fail booking
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId == apiDataSelectedProduct.AvailabilityReferenceId);
                    if (bookedProduct == null || apiDataSelectedProduct == null) continue;
                    if (criteria.APIType == APIType.RedeamV12)
                    {
                        bookedProduct = MapProductForRedeamV12(bookedProduct, commonSelectedBookedProduct);
                    }
                    else if (criteria.APIType == APIType.GlobalTixV3)
                    {
                        bookedProduct.APIExtraDetail = ((CanocalizationSelectedProduct)selectedProduct).APIDetails;
                        bookedProduct.OptionStatus = ((int)selectedProduct.Status).ToString();
                    }
                    bookedProducts.Add(bookedProduct);
                }
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;

                foreach (var selectedProduct in selectedProducts)
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(selectedProduct.AvailabilityReferenceId));

                    if (bookedProduct == null) continue;
                    // Checking if already contains the booked product, to maintain the booking status
                    if (bookedProducts.Contains(bookedProduct)) continue;

                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                }

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                _log.Error($"SupplierBookingService|CreateRedeamV12ProductsBooking|{SerializeDeSerializeHelper.Serialize(criteria)}", ex);

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber ?? string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(criteria.APIType), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
            return bookedProducts;
        }
        private List<ProductOption> UpdateOptionStatus(List<ProductOption> productOptions, string referenceNumber)
        {
            if (productOptions == null) return productOptions;
            foreach (var productOption in productOptions)
            {
                productOption.BookingStatus = GetOptionBookingStatus(referenceNumber, productOption.AvailabilityStatus);
            }
            return productOptions;
        }
        private OptionBookingStatus GetOptionBookingStatus(string referenceNumber, AvailabilityStatus? availabilityStatus)
        {
            var status = OptionBookingStatus.Failed;
            if (!string.IsNullOrWhiteSpace(referenceNumber) && availabilityStatus != null)
            {
                if (availabilityStatus.Equals(AvailabilityStatus.ONREQUEST))
                    status = OptionBookingStatus.Requested;
                else if (availabilityStatus.Equals(AvailabilityStatus.AVAILABLE))
                    status = OptionBookingStatus.Confirmed;
            }
            return status;
        }
        private BookedProduct MapProductForRedeamV12(BookedProduct bookedProduct, CanocalizationSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct?.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = (ActivityOption)selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            var noOfPassengers = selectedOption?.TravelInfo?.NoOfPassengers?.Values.Sum(x => x);
            var noOfQRCodes = selectedProduct.QrCodes?.Count();
            var isQRCodePerPax = noOfQRCodes > 1;

            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.BookingReferenceNumber;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct?.HotelPickUpLocation;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.RedeamV12);
            apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;
            apiExtraDetail.SupplierCancellationPolicy = selectedOption?.ApiCancellationPolicy;

            var codeType = selectedProduct?.QrCodes?.FirstOrDefault().Type;

            if (!isQRCodePerPax)
            {
                var qrCodeValues = selectedProduct?.QrCodes?.Select(x => x.Value).ToList();
                if (qrCodeValues != null)
                {
                    var qrCodeValue = string.Join(", ", qrCodeValues);
                    var qrCodeString = $"{codeType}~{qrCodeValue}";
                    apiExtraDetail.QRCode = qrCodeString;
                }
            }
            else if (selectedProduct?.QrCodes != null)
            {
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                // Looping on the QrCodes and setting them in the ApiTicketDetail
                foreach (var qrCode in selectedProduct?.QrCodes)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = qrCode?.Value,
                        FiscalNumber = qrCode?.PassengerType,
                        APIOrderId = selectedProduct?.BookingReferenceNumber,
                        QRCodeType = MapAPICodeFormatWithIsangoCode(codeType,APIType.RedeamV12)
                    };
                    apiExtraDetail?.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.Time = selectedOption?.Time;
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct?.BookingReferenceNumber, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        private string GetBookingStatusNumber(string referenceNumber, AvailabilityStatus? availabilityStatus)
        {
            return ((int)GetOptionBookingStatus(referenceNumber, availabilityStatus)).ToString();
        }
        private void APIReservationGet(
           CanocalizationSelectedProduct selectedProduct,
           dynamic createHoldResponse, Booking booking,
            string token, APIType apiType)
        {

            try
            {

                if (createHoldResponse != null)
                {
                    if (apiType == APIType.RedeamV12)
                    {
                        selectedProduct.HoldId = ((CreateHoldResponse)createHoldResponse).Hold.Id.ToString();
                        selectedProduct.HoldStatus = ((CreateHoldResponse)createHoldResponse).Hold.Status;
                    }
                    else if (apiType == APIType.GlobalTixV3)
                    {
                        selectedProduct.HoldId = ((ReservationRS)createHoldResponse).Data.ReferenceNumber.ToString();
                        selectedProduct.HoldStatus = ((ReservationRS)createHoldResponse).Data.Status;
                    }
                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , "");
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                             selectedProduct.HoldId, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.RedeamV12), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "RedeamReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}");
                throw;
            }
        }
        public void LogBookingFailureInDB(Booking failedBooking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel)
        {
            Task.Run(() => _supplierBookingPersistence.LogBookingFailureInDB(failedBooking, bookingRefNo, serviceID, tokenID, apiRefID, custEmail, custContact, ApiType, optionID, optionName, avlbltyRefID, ErrorLevel));
        }

        private string MapAPICodeFormatWithIsangoCode(string apiCodeFormat, APIType apiType)
        {
            var returnIsangoCodeFormat = apiCodeFormat;
            if (!string.IsNullOrEmpty(apiCodeFormat))
            {
                switch (apiType)
                {
                    case APIType.PrioHub:
                    case APIType.Prio:
                        {
                            if (apiCodeFormat.ToUpper().Contains(Constant.PrioHub_QRCODE.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoQRCODE;
                            }
                            else if (apiCodeFormat.ToUpper().Contains(Constant.PrioHub_BARCODE.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoBARCODE;
                            }
                            else if (apiCodeFormat.ToUpper().Contains(Constant.PrioHub_LINK.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoLINK;
                            }
                            break;
                        }

                    case APIType.TourCMS:
                        if (apiCodeFormat.ToLower().Contains(Constant.TourCMS_BAR.ToLower()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoBARCODE;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        break;

                    case APIType.Rayna:
                        if (apiCodeFormat.ToLower().Contains(Constant.API_Link.ToLower()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        break;
                    case APIType.GoCity:
                    case APIType.Tiqets:
                        if (apiCodeFormat.ToLower().Contains(Constant.API_Link.ToLower()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        break;

                    case APIType.NewCitySightSeeing:
                    case APIType.Citysightseeing:
                    case APIType.Bokun:
                    case APIType.Redeam:
                    case APIType.RedeamV12:
                    case APIType.Rezdy:
                    case APIType.BigBus:
                        {
                            if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoQRCODE;
                            }
                            break;
                        }

                    case APIType.Ventrata:
                        if (apiCodeFormat.ToUpper().Contains("String".ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        else if (apiCodeFormat.ToUpper().Contains("Link".ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        break;

                    case APIType.Goldentours:
                        if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.API_Link.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        break;


                    case APIType.Hotelbeds:
                        if (apiCodeFormat.ToUpper().Contains(Constant.API_Link.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        break;

                    default:
                        returnIsangoCodeFormat = apiCodeFormat;
                        break;

                }
            }
            return returnIsangoCodeFormat;
        }
        #endregion

    }
}
