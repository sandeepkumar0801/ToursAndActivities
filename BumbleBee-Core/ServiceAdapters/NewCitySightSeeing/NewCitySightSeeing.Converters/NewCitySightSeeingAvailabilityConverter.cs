using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.NewCitySightSeeing;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability;
using Util;
using Activity = Isango.Entities.Activities.Activity;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters
{
    public class NewCitySightSeeingAvailabilityConverter : ConverterBase, INewCitySightSeeingAvailabilityConverter
    {
        private string _currencyISOCode;
        private int _adultCount;
        private int _childCount;
        private int _infantCount;
        private int _totalPaxCount;
        private int _tempInt;

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

        private List<ProductOption> ConvertAvailabilityResult(AvailabilityResponse result,
            object criteria)
        {
            var activity = new Activity();
            _criteria = criteria as NewCitySightSeeingCriteria;
            try
            {
              
                if (result != null && result.Days?.Count > 0)
                {

                    var ctSightApiActivity = result?.Days.FirstOrDefault();
                    var apiCode = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.ProductCode;
                    var apiCurrency = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.Currency;

                    var apiVariantSource = ctSightApiActivity?.Availabilities?.FirstOrDefault()?.Source;
                    //At last moment API client remove source property for some products from API response.
                    //We will retain it and use VariantCondition to Differ products
                    //Whole code is depend upon source property
                    if (apiVariantSource == null || apiVariantSource=="")
                    {
                        if (result?.Days != null)
                        {
                            foreach (var item in result?.Days)
                            {
                                if (item?.Availabilities != null)
                                {
                                    foreach (var data in item.Availabilities)
                                    {
                                        data.Source = apiCode;
                                    }
                                }
                            }
                        }
                        
                    }
                   
                    int.TryParse(_criteria.IsangoActivityId, out _tempInt);
                    var options = new List<ActivityOption>();

                    try
                    {
                        _currencyISOCode = apiCurrency;
                        _adultCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value ?? 0;
                        _childCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value ?? 0;
                        _infantCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value ?? 0;
                        _totalPaxCount = _adultCount + _childCount + _infantCount;

                        activity.ProductOptions = new List<ProductOption>();

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
                            
                               //Check how many sources Exist
                                var SourcesExist = result?.Days?.SelectMany(p => p.Availabilities)?.Select(x => x.Source)?.Distinct();
                                foreach (var apiSourceExist in SourcesExist)
                                {
                                    var option = CreateOption(travelInfo, _criteria,
                                     result?.Days, apiSourceExist?.ToLower(), apiVariantSource);
                                    if (option != null

                                    )
                                    {
                                        option.Name = $"{_criteria.ProductOptionName}";
                                        //only add option if count of pax is same
                                        //Filter options, if they dont match with user selection
                                        var totaldifferentPassengers = travelInfo?.NoOfPassengers?.Count;
                                        if (totaldifferentPassengers > 0)
                                        {
                                            foreach (var item in option?.BasePrice?.DatePriceAndAvailabilty?.Where(kvp => kvp.Value?.PricingUnits?.Count != totaldifferentPassengers)?.ToList())
                                            {
                                                option.BasePrice.DatePriceAndAvailabilty.Remove(item.Key);
                                            }
                                        }
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
                        activity.ProductOptions = availableOptions?.Cast<ProductOption>()?.ToList();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        //TODOlogging
                        activity = null;
                    }
                 }
            }
            catch (Exception ex)
            {
                //ignored
                //##TODO Add logging here
            }
            return activity.ProductOptions;
        }

        private ActivityOption CreateOption(
            TravelInfo travelInfo, NewCitySightSeeingCriteria criteria,
            List<Day> days, string apiSourceExist,string apiVariantSource)
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
                var apiAvailabilities = apiSourceExist == string.Empty ? (days.FirstOrDefault().Availabilities) : (days.SelectMany(y => y.Availabilities.Where(z => z.Source?.ToLower() == apiSourceExist))).ToList();
                var apiCode = apiAvailabilities?.FirstOrDefault()?.ProductCode;
                var apiCurrency = apiAvailabilities?.FirstOrDefault()?.Currency;
                var apiSource = apiAvailabilities?.FirstOrDefault()?.Source;
                option = new ActivityOption
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Code = apiCode,
                    RateKey = apiSource,
                    ServiceOptionId = _criteria.ServiceOptionId.ToInt(),
                    SupplierOptionCode = _criteria.ActivityCode,
                    PrefixServiceCode = _criteria.ActivityCode,
                    TravelInfo = new TravelInfo
                    {
                        Ages = criteria.Ages,
                        NoOfPassengers = criteria.NoOfPassengers,
                        NumberOfNights = 0,
                        StartDate = criteria.CheckinDate
                    },
                   
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE,

                };
                option.VariantCondition = true;
                if (String.IsNullOrEmpty(apiVariantSource))
                {
                    option.VariantCondition = false;//no variant code
                }

                if (apiAvailabilities?.Count > 0)
                {
                    foreach (var day in days)
                    {

                        var availabilityData = new List<AvailabilityData>();
                        if (apiSourceExist != string.Empty)
                        {
                            availabilityData = day?.Availabilities?.Where(x => x.Rate?.ToLower() == "adult" && x.Source?.ToLower() == apiSourceExist)?.ToList();
                        }
                        else
                        {
                            availabilityData = day?.Availabilities?.Where(x => x.Rate?.ToLower() == "adult")?.ToList();
                        }
                        if (day.IsAvailable == true && availabilityData?.FirstOrDefault()?.Availability > 0)
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
                                 0, day, apiSourceExist);
                            priceSell = UpdatePricePerPax(travelInfo, criteria, priceSell,
                                 1, day, apiSourceExist);

                            priceGateBase = UpdatePricePerPax(travelInfo, criteria, priceSell,
                                 2, day, apiSourceExist);

                            totalSellPrice = priceSell.TotalPrice;
                            totalCostPrice = priceCost.TotalPrice;
                            totalGateBasePrice = priceGateBase.TotalPrice;

                            var opDate = day.Date.ToString().ToDateTimeExactV1();

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

                //Cost Price(option.CostPrice)
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
            int isSellPriceUnit, Day day, string apiSourceExist)
        {
            var apiAvailabilities = apiSourceExist == string.Empty ? (day?.Availabilities) : (day?.Availabilities?.Where(x => x.Source?.ToLower() == apiSourceExist))?.ToList();
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

            var adultPriceQuery = from m in apiAvailabilities
                                  from pax in travelInfo.Ages
                                  where pax.Key == PassengerType.Adult
                                  && m.Price > 0
                                  && m.Availability > 0
                                  && m.Rate.ToString().ToLower().Contains("adult")
                                  select m;


            if (adultPriceQuery?.Any() == true)
            {
                _adultCount = 1;
            }

            var childPriceQuery = from m in apiAvailabilities
                                  from pax in travelInfo.Ages
                                  where pax.Key == PassengerType.Child
                                  && m.Price > 0 
                                  && m.Availability>0
                                  && m.Rate.ToString().ToLower().Contains("child")
                                  select m;

            if (childPriceQuery?.Any() == true)
            {
                _childCount = 1;
            }

            var infantPriceQuery = from m in apiAvailabilities
                                   from pax in travelInfo.Ages
                                   where pax.Key == PassengerType.Infant
                                   && m.Price <= 0
                                   && m.Availability > 0
                                   && m.Rate.ToString().ToLower().Contains("infant")
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


            if (_adultCount > 0 && adultPriceNode!=null)
            {
                var priceSet= 0.0M;
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
                    Quantity = _adultCount
                };
                price.PricingUnits.Add(adultPriceUnit);
                price.Capacity =System.Convert.ToInt32(adultPriceNode?.Availability);
            }


            if (_childCount > 0 && childPriceNode!=null)
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
                    Quantity = _childCount
                };
                price.PricingUnits.Add(childPriceUnit);
                price.Capacity = System.Convert.ToInt32(childPriceNode?.Availability);
            }


            if (_infantCount > 0 && infantPriceNode!=null)
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
                    Quantity = _infantCount
                };
                price.PricingUnits.Add(infantPriceUnit);
                price.Capacity = System.Convert.ToInt32(infantPriceNode?.Availability);
            }
            //
            if (isSellPriceUnit == 0)
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