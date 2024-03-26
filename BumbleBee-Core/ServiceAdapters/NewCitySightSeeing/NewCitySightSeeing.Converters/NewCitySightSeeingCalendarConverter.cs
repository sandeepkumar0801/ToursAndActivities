using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.NewCitySightSeeing;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability;
using Util;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters
{
    public class NewCitySightSeeingCalendarConverter : ConverterBase, INewCitySightSeeingCalendarConverter
    {
        private string _currencyISOCode;
        private int _adultCount;
        private int _childCount;
        private int _infantCount;
        private int _totalPaxCount;
       
        private NewCitySightSeeingCriteria _criteria;

        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (AvailabilityResponse)apiResponse;
            return result != null ? ConvertAvailabilityResult(result, criteria) : null;
        }

        #region Private Methods

        private List<ProductOption> ConvertAvailabilityResult(AvailabilityResponse result, object criteria)
        {
            var returnProductOption = new List<ProductOption>();
            _criteria = criteria as NewCitySightSeeingCriteria;
            var productMappings = _criteria?.ProductMapping; //retrieve single only

            try
            {
                if (!(productMappings?.Count > 0))
                {
                    return null;
                }
                if (result != null && result.Days?.Count > 0)
                {

                    var ctSightApiActivity = result?.Days.FirstOrDefault();
                    var apiCode = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.Source;
                    var apiProductCode = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.ProductCode;
                    var apiCurrency = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.Currency;
                    var apiVariantSource = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.Source;

                    //At last moment API client remove source property for some products from API response.
                    //We will retain it and use VariantCondition to Differ products
                    //Whole code is depend upon source property
                    if (apiVariantSource == null || apiVariantSource == "")
                    {
                        if (result?.Days != null)
                        {
                            foreach (var item in result?.Days)
                            {
                                if (item?.Availabilities != null)
                                {
                                    foreach (var data in item.Availabilities)
                                    {
                                        data.Source = apiProductCode;
                                    }
                                }
                            }
                        }

                    }

                    var mappedProduct = productMappings?.FirstOrDefault();
                    
                    if (ctSightApiActivity == null)
                    {
                        return null;
                    }

                    

                    var options = new List<ActivityOption>();

                    try
                    {
                        _currencyISOCode = apiCurrency;
                        _adultCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value ?? 0;
                        _childCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value ?? 0;
                        _infantCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value ?? 0;
                        _totalPaxCount = _adultCount + _childCount + _infantCount;

                       

                        var productOption = new ProductOption();
                        var travelInfo = new TravelInfo
                        {
                            Ages = _criteria.Ages,
                            NoOfPassengers = _criteria.NoOfPassengers,
                            NumberOfNights = 0,
                            StartDate = _criteria.CheckinDate
                        };
                        productOption.TravelInfo = travelInfo;

                        try
                        {
                            
                                //if no varaiant Exists, then make dynamic
                                //Check how many sources Exist
                                var SourcesExist = result?.Days?.SelectMany(p => p.Availabilities)?.Select(x => x.Source)?.Distinct();
                                foreach (var apiSourceExist in SourcesExist)
                                {

                                    var option = CreateOption(travelInfo, _criteria,
                                         result?.Days, apiSourceExist?.ToLower()
                                        , productMappings.FirstOrDefault().IsangoHotelBedsActivityId);
                                    if (option != null

                                    )
                                    {
                                        option.Name = $"{apiSourceExist}";
                                        options.Add(option);
                                    }
                                }
                           

                        }
                        catch (Exception ex)
                        {
                            //ignore
                            //#TODO add logging here;
                        }
                        var availableOptions = options.Where(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                        returnProductOption = availableOptions?.Cast<ProductOption>()?.ToList();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        //TODOlogging
                        //activity = null;
                    }
                    

                }
            }
            catch (Exception ex)
            {
                //ignored
                //##TODO Add logging here
            }
            return returnProductOption;
        }

        private ActivityOption CreateOption(
            TravelInfo travelInfo, NewCitySightSeeingCriteria criteria,
            List<Day> days,string apiSourceExist,int IsangoHotelBedsActivityId)
        {
            var option = default(ActivityOption);

            var costPrice = new Price();
            var sellPrice = new Price();
            var gatePrice = new Price();

            var pAndACost = new Dictionary<DateTime, PriceAndAvailability>();
            var pAndASell = new Dictionary<DateTime, PriceAndAvailability>();
            var pAndAGate = new Dictionary<DateTime, PriceAndAvailability>();

            decimal totalSellPrice=0;
            decimal totalCostPrice=0;
            decimal totalGateBasePrice = 0;

            try
            {
                var apiAvailabilities = apiSourceExist==string.Empty ? (days.FirstOrDefault().Availabilities): (days.SelectMany(y => y.Availabilities.Where(z => z.Source?.ToLower() == apiSourceExist))).ToList();
                var apiCode = apiAvailabilities?.FirstOrDefault()?.ProductCode;
                var apiCurrency = apiAvailabilities?.FirstOrDefault()?.Currency;
                var apiSource = apiAvailabilities?.FirstOrDefault()?.Source;
                option = new ActivityOption
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Code = apiCode,
                    RateKey = apiSource,
                    ServiceOptionId = 0,
                    SupplierOptionCode = apiCode,
                    PrefixServiceCode = apiSource,
                    TravelInfo = new TravelInfo
                    {
                        Ages = criteria.Ages,
                        NoOfPassengers = criteria.NoOfPassengers,
                        NumberOfNights = 0,
                        StartDate = criteria.CheckinDate
                    },
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE,
                };

                if (apiAvailabilities?.Count > 0)
                {
                    foreach (var day in days)
                    {
                        var availabilityData =new List<AvailabilityData>();
                        if (apiSourceExist != string.Empty)
                        {
                            availabilityData = day?.Availabilities?.Where(x => x.Rate?.ToLower() == "adult" && x.Source?.ToLower() == apiSourceExist)?.ToList();
                        }
                        else
                        {
                            availabilityData = day?.Availabilities?.Where(x => x.Rate?.ToLower() == "adult")?.ToList();
                        }
                        if (day.IsAvailable == true && availabilityData?.FirstOrDefault()?.Availability>0)
                        {
                            var priceCost = new DefaultPriceAndAvailability
                            {
                                AvailabilityStatus = AvailabilityStatus.AVAILABLE,

                            };

                            var priceSell = new DefaultPriceAndAvailability
                            {
                                AvailabilityStatus = AvailabilityStatus.AVAILABLE,

                            };

                            var priceGateBase = new DefaultPriceAndAvailability
                            {
                                AvailabilityStatus = AvailabilityStatus.AVAILABLE,

                            };
                            
                           
                            priceCost = UpdatePricePerPax(travelInfo, criteria, priceCost,
                                IsangoHotelBedsActivityId, 0, day, apiCurrency,  apiSourceExist);
                            priceSell = UpdatePricePerPax(travelInfo, criteria, priceSell,
                                IsangoHotelBedsActivityId, 1, day, apiCurrency,  apiSourceExist);
                            priceGateBase = UpdatePricePerPax(travelInfo, criteria, priceSell,
                                IsangoHotelBedsActivityId, 2, day, apiCurrency,  apiSourceExist);

                            totalSellPrice = priceSell.TotalPrice;
                            totalCostPrice = priceCost.TotalPrice;
                            totalGateBasePrice = priceGateBase.TotalPrice;

                            var opDate = day.Date.Date.ToString().ToDateTimeExactV1();

                            if (!pAndACost.Keys.Contains(opDate))
                            {
                                pAndACost.Add(opDate, priceCost);
                            }

                            if (!pAndASell.Keys.Contains(opDate))
                            {
                                pAndASell.Add(opDate, priceSell);
                            }
                            if (!pAndAGate.Keys.Contains(opDate))
                            {
                                pAndAGate.Add(opDate, priceGateBase);
                            }
                        }
                    }
                }
                else
                {
                    option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    return option;
                }
                //Price and Currency
                
                //Cost Price (option.CostPrice)
                costPrice.DatePriceAndAvailabilty = pAndACost;
                costPrice.Amount = System.Convert.ToDecimal(totalCostPrice);
                costPrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.CostPrice = costPrice;

                //Sell Price(option.BasePrice)
                sellPrice.DatePriceAndAvailabilty = pAndASell;
                sellPrice.Amount = System.Convert.ToDecimal(totalSellPrice);
                sellPrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.BasePrice = sellPrice;

                //GatePrice (option.GateBasePrice)
                gatePrice.DatePriceAndAvailabilty = pAndAGate;
                gatePrice.Amount = System.Convert.ToDecimal(totalGateBasePrice);
                gatePrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.GateBasePrice = gatePrice;

                option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
            }
            catch (Exception ex)
            {
                //ignorred
                //#TODO add logging here
                option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }

            return option;
        }

        private DefaultPriceAndAvailability UpdatePricePerPax(
            TravelInfo travelInfo, NewCitySightSeeingCriteria criteria,
            DefaultPriceAndAvailability price,
            int IsangoHotelBedsActivityId,
            int isSellPriceUnit, Day day, string apiCurrency,string apiSourceExist)
        {
            var apiAvailabilities = apiSourceExist==string.Empty? (day?.Availabilities) : (day?.Availabilities?.Where(x=>x.Source?.ToLower()== apiSourceExist))?.ToList();
            decimal adultCostPrice = 0;
            decimal childCostPrice = 0;
            decimal infantCostPrice = 0;


            decimal adultSellPrice = 0;
            decimal childSellPrice = 0;
            decimal infantSellPrice = 0;

            decimal adultGateBasePrice = 0;
            decimal childGateBasePrice = 0;
            decimal infantGateBasePrice = 0;

            price.PricingUnits = new List<PricingUnit>();

            var passengerInfos = criteria?.PassengerInfo?.Where(x => x.ActivityId == IsangoHotelBedsActivityId)?.ToList();

            var adultPriceQuery = from m in apiAvailabilities
                                  where m.Rate.ToString().ToLower().Contains("adult")
                                  && m.Availability > 0
                                  && m.Price > 0
                                  select m;
            if (adultPriceQuery?.Any() == true)
            {
                _adultCount = 1;
            }

            var childPriceQuery = from m in apiAvailabilities
                                  where m.Rate.ToString().ToLower().Contains("child")
                                 && m.Price > 0
                                 && m.Availability > 0
                                  select m;

            if (childPriceQuery?.Any() == true)
            {
                _childCount = 1;
            }

            var infantPriceQuery = from m in apiAvailabilities
                                   where m.Rate.ToString().ToLower().Contains("infant")
                                  && m.Price <= 0
                                   && m.Availability > 0
                                   select m;


            if (infantPriceQuery?.Any() == true)
            {
                _infantCount = 1;
            }

            var adultPriceNode = adultPriceQuery?.FirstOrDefault();
            var childPriceNode = childPriceQuery?.FirstOrDefault();

            var infantPriceNode = infantPriceQuery?.FirstOrDefault();

            //Sell Price SET
            adultSellPrice = System.Convert.ToDecimal(adultPriceNode?.DiscountedPrice);
            childSellPrice = System.Convert.ToDecimal(childPriceNode?.DiscountedPrice);
            infantSellPrice = System.Convert.ToDecimal(infantPriceNode?.DiscountedPrice);

            //Cost Price SET
            adultCostPrice = System.Convert.ToDecimal(adultPriceNode?.CostPrice);
            childCostPrice = System.Convert.ToDecimal(childPriceNode?.CostPrice);
            infantCostPrice = System.Convert.ToDecimal(infantPriceNode?.CostPrice);

            //Gate Price SET
            adultGateBasePrice = System.Convert.ToDecimal(adultPriceNode?.Price);
            childGateBasePrice = System.Convert.ToDecimal(childPriceNode?.Price);
            infantGateBasePrice = System.Convert.ToDecimal(infantPriceNode?.Price);


            if (_adultCount > 0 && adultPriceNode != null)
            {
                var priceSet = 0.0M;
                if (isSellPriceUnit == 0)
                {
                    priceSet = adultCostPrice;
                }
                else if (isSellPriceUnit == 1)
                {
                    priceSet = adultSellPrice;
                }
                else if (isSellPriceUnit == 2)
                {
                    priceSet = adultGateBasePrice;
                }

                var adultPriceUnit = new AdultPricingUnit
                {
                    Price = priceSet,
                    Quantity = _adultCount,
                    Currency= apiCurrency
                };
                price.PricingUnits.Add(adultPriceUnit);
                price.Capacity =System.Convert.ToInt32(adultPriceNode?.Availability);
            }


            if (_childCount > 0 && childPriceNode != null)
            {
                var priceSet = 0.0M;
                if (isSellPriceUnit == 0)
                {
                    priceSet = childCostPrice;
                }
                else if (isSellPriceUnit == 1)
                {
                    priceSet = childSellPrice;
                }
                else if (isSellPriceUnit == 2)
                {
                    priceSet = childGateBasePrice;
                }
                var childPriceUnit = new ChildPricingUnit
                {
                    Price = priceSet,
                    Quantity = _childCount,
                    Currency = apiCurrency
                };
                price.PricingUnits.Add(childPriceUnit);
                price.Capacity = System.Convert.ToInt32(childPriceNode?.Availability);
            }


            if (_infantCount > 0 && infantPriceNode != null)
            {
                var priceSet = 0.0M;
                if (isSellPriceUnit == 0)
                {
                    priceSet = infantCostPrice;
                }
                else if (isSellPriceUnit == 1)
                {
                    priceSet = infantSellPrice;
                }
                else if (isSellPriceUnit == 2)
                {
                    priceSet = infantGateBasePrice;
                }
                var infantPriceUnit = new InfantPricingUnit
                {
                    Price = priceSet,
                    Quantity = _infantCount,
                    Currency = apiCurrency
                };
                price.PricingUnits.Add(infantPriceUnit);
                price.Capacity = System.Convert.ToInt32(infantPriceNode?.Availability);
            }

            if (isSellPriceUnit==0)
            {
                price.TotalPrice = (adultCostPrice * _adultCount)
                  + (childCostPrice * _childCount)
                  + (infantCostPrice + _infantCount);
            }
            else if (isSellPriceUnit == 1)
            {
                price.TotalPrice = (adultSellPrice * _adultCount)
                    + (childSellPrice * _childCount)
                    + (infantSellPrice + _infantCount);
            }
            else if (isSellPriceUnit == 2)
            {
                price.TotalPrice = (adultGateBasePrice * _adultCount)
                    + (childGateBasePrice * _childCount)
                    + (infantGateBasePrice + _infantCount);
            }

            if (!string.IsNullOrEmpty(apiSourceExist))
            {
                if (price.TotalPrice <= 0 || day.IsAvailable != true ||
                    day.Availabilities.Where(x => x.Rate?.ToLower() == "adult"
                    && x.Source?.ToLower() == apiSourceExist)
                    ?.FirstOrDefault()?.Availability <= 0)
                {
                    price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                }
            }
            else
            {
                if (price.TotalPrice <= 0 || day.IsAvailable != true ||
                    day.Availabilities.Where(x => x.Rate?.ToLower() == "adult")
                    ?.FirstOrDefault()?.Availability <= 0)
                {
                    price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                }
            }

            return price;
        }
        #endregion Private Methods
    }
}