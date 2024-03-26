using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.HB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace Isango.Service.SupplierServices
{
    public class HBApitudeService : SupplierServiceBase, ISupplierService
    {
        private IHBAdapter _hbApitudeAdapter;
        private readonly IMemCache _memCache;
        private bool _isHBApiCallParallel = false;
        private readonly ILogger _log;

        public HBApitudeService(IHBAdapter ticketAdapter, IMemCache memCache, ILogger log)
        {
            _hbApitudeAdapter = ticketAdapter;
            _memCache = memCache;
            _log = log;
            try
            {
                _isHBApiCallParallel = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsHBApiCallParallel) == "1";
            }
            catch
            {
                _isHBApiCallParallel = false;
            }
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var po = activity.ProductOptions?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode));
            if (criteria.CheckoutDate <= criteria.CheckinDate)
            {
                criteria.CheckoutDate = criteria.CheckoutDate.AddDays(Constant.AddFourteenDays);
            }
            
            if ((criteria.CheckoutDate - criteria.CheckinDate).Days < Constant.AddFourteenDays)
            {
                criteria.CheckoutDate = criteria.CheckinDate.AddDays(Constant.AddFourteenDays);
            }

            var hotelbedCriteriaApitude = new HotelbedCriteriaApitude
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,//.AddDays(Constant.AddSixDays),
                Language = clientInfo?.LanguageCode,
                Destination = string.Empty,//destinationCode,
                //PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                PassengerInfo = criteria.PassengerInfo,
                ActivityCode = po?.PrefixServiceCode,
                IsangoActivityId = activity?.Id.ToString(),
                ServiceOptionId = po?.Id.ToString(),
                Token = criteria.Token,
                ActivityMargin = activity?.Margin?.Value
            };
            return hotelbedCriteriaApitude;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            //#region New Json based API Apitude HotelBeds  Criteria
            var hbCriteria = (HotelbedCriteriaApitude)criteria;
            try
            {
                //var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
                List<Activity> hbActivities = null;

                if (_isHBApiCallParallel)
                {
                    hbActivities = GetHBActivitiesParallalAsync(activity, hbCriteria, token)?.GetAwaiter().GetResult();
                }
                else
                {
                    hbActivities = GetHBActivitiesSequentiallyAsync(activity, hbCriteria, token)?.GetAwaiter().GetResult();
                }

                var apiResult = MapActivity(activity, hbActivities, hbCriteria);
                if (apiResult?.ProductOptions?.Count > 0)
                {
                    activity = apiResult;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"ActivityService|GetHBLiveActivity|IsangoActivityId :- {activity?.Id}" +
                    $", CheckinDate :- {criteria?.CheckinDate}, " +
                    $", Language :- {hbCriteria?.Language}", ex);
                //ignored
            }
            return activity;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromApi, Criteria criteria)
        {
            if (activity?.Regions != null
                 && activity.ProductOptions != null
                 && activitiesFromApi?.Count > 0
            )
            {
                var productOptions = new List<ProductOption>();
                var updatedOptions = new List<ProductOption>();
                var productOptionsCopy = new ProductOption[activity.ProductOptions.Count];
                activity.ProductOptions.CopyTo(productOptionsCopy);
                activity.ProductOptions.Clear();
                foreach (var hbActivity in activitiesFromApi)
                {
                    try
                    {
                        foreach (var item in productOptionsCopy?.ToList())
                        {
                            var hbActivityOptions = hbActivity?.ProductOptions?
                                .Where(x => x.PrefixServiceCode == item.PrefixServiceCode)?
                                .ToList();

                            if (hbActivityOptions?.Count > 0)
                            {
                                productOptions = hbActivityOptions
                                    .Select(p =>
                                    {
                                        //p.Name = $"{item.Name} - {p.Name}";
                                        p.Margin = item.Margin;
                                        return p;
                                    }).ToList();
                            }
                        }

                        updatedOptions = UpdateBasePricesHbApitude(productOptions, activity.HotelPickUpLocation, criteria)
                            ?.Where(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE)?.ToList();

                        if (updatedOptions?.Count > 0)
                        {
                            activity.ProductOptions.AddRange(updatedOptions);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"ActivityService|LoadMappedDataHB", ex);
                    }
                }
            }

            return activity;
        }

        /// <summary>
        /// It try to call api for each option in parallel for hotelbeds Apitude Api
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria">List of activity, there will be one activity for each option as iSango option is mapped with an activity at api end</param>
        /// <returns></returns>
        private Task<List<Activity>> GetHBActivitiesParallalAsync(Activity activity, HotelbedCriteriaApitude criteria, string token)
        {
            var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
            var hbActivities = new List<Activity>();

            var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 1.0));

            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Constant.MaxParallelThreadCount);
            Parallel.ForEach(productOptions, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, po =>
            {
                try
                {
                    var hotelbedCriteriaApitude = GetHotelbedCriteriaApitude(activity, criteria, po);

                    var hbActivity = _hbApitudeAdapter.ActivityDetailsAsync(hotelbedCriteriaApitude,
                        token).GetAwaiter().GetResult();

                    if (hbActivity?.ProductOptions?.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true)
                    {
                        hbActivities.Add(hbActivity);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"ActivityService|GetHBActivitiesParallalAsync|IsangoActivityId :- {activity?.Id}" +
                        $", Criteria :- {SerializeDeSerializeHelper.Serialize(criteria)},", ex);
                }
            });
            return Task.FromResult(hbActivities);
        }

        /// <summary>
        /// It sequentially call api for each option for hotelbeds Apitude Api
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria">List of activity, there will be one activity for each option as iSango option is mapped with an activity at api end</param>
        /// <returns></returns>
        private Task<List<Activity>> GetHBActivitiesSequentiallyAsync(Activity activity, HotelbedCriteriaApitude criteria, string token)
        {
            var productOptions = activity.ProductOptions?.Where(x => !string.IsNullOrWhiteSpace(x.PrefixServiceCode))?.ToList();
            var hbActivities = new List<Activity>();

            foreach (var po in productOptions)
            {
                try
                {
                    var hotelbedCriteriaApitude = GetHotelbedCriteriaApitude(activity, criteria, po);

                    var hbActivity = _hbApitudeAdapter.ActivityDetailsAsync(hotelbedCriteriaApitude,
                        token).GetAwaiter().GetResult();

                    if (hbActivity?.ProductOptions?.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true)
                    {
                        hbActivities.Add(hbActivity);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"ActivityService|GetHBActivitiesSequentiallyAsync|IsangoActivityId :- {activity?.Id}" +
                        $", Criteria :- {SerializeDeSerializeHelper.Serialize(criteria)},", ex);
                }
            }
            return Task.FromResult(hbActivities);
        }

        private HotelbedCriteriaApitude GetHotelbedCriteriaApitude(Activity activity, HotelbedCriteriaApitude criteria, ProductOption productOption)
        {
            var destinationCode = productOption?.SupplierOptionCode?.Split('~')?.LastOrDefault();

            var hotelbedCriteriaApitude = new HotelbedCriteriaApitude
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,//.AddDays(Constant.AddFourteenDays),

                Language = criteria.Language,
                //PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                PassengerInfo = criteria.PassengerInfo,

                ActivityCode = productOption?.PrefixServiceCode,
                IsangoActivityId = activity?.Id,
                ServiceOptionId = productOption?.Id.ToString(),
                Destination = destinationCode,
                ActivityMargin = activity?.Margin?.Value
            };

            return hotelbedCriteriaApitude;
        }

        /// <summary>
        /// Updates HotelBeds Apitude product options' "Sell Prices" to the current currency. Note: margin is ignored at the moment!
        /// </summary>
        /// <param name="productOptions"></param>
        /// <param name="hotelPickupLocation"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<ProductOption> UpdateBasePricesHbApitude(List<ProductOption> productOptions, string hotelPickupLocation,
            Criteria criteria)
        {
            if (productOptions == null || (productOptions.Count <= 0)) return productOptions;

            var options = new List<ProductOption>();
            foreach (var option in productOptions)
            {
                var actOption = (ActivityOption)option;
                if (actOption == null) continue;

                actOption.BasePrice = CalculatePriceForAllPaxHBApitude(actOption.SellPrice, criteria);
                actOption.GateBasePrice = CalculatePriceForAllPaxHBApitude(actOption.SellPrice, criteria);
                actOption.CostPrice = CalculatePriceForAllPaxHBApitude(actOption.CostPrice, criteria);

                if (actOption?.IsMandatoryApplyAmount == true)
                {
                    actOption.Margin = null;
                }
                actOption.HotelPickUpLocation = hotelPickupLocation;
                options.Add(actOption);
            }
            return options;
        }

        private Price CalculatePriceForAllPaxHBApitude(Price inputPrice, Criteria criteria)
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