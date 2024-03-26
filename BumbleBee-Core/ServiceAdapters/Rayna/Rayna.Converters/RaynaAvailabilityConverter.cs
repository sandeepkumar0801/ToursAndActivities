using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Rayna;
using ServiceAdapters.Rayna.Rayna.Converters.Contracts;
using ServiceAdapters.Rayna.Rayna.Entities;

namespace ServiceAdapters.Rayna.Rayna.Converters
{
    public class RaynaAvailabilityConverter : ConverterBase, IRaynaAvailabilityConverter
    {
        private string _currencyISOCode;
        private int _adultCount;
        private int _childCount;
        private int _infantCount;
        private int _totalPaxCount;
        private int _tempInt;

        /// <summary>
        /// Convert API Result Entities to Isango.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (AvailabilityReturnData)apiResponse;
            return result != null ? ConvertAvailabilityResult(result, criteria) : null;
        }

        #region Private Methods

        private Activity ConvertAvailabilityResult(AvailabilityReturnData result, object criteria)
        {
            var activitiesIsango = new Activity();

            var availabilityTourOptionRS = result?.AvailabilityTourOptionRS;
            var availabilityTimeSlotRS = result?.AvailabilityTimeSlotRS;
            var availabilityRES = result?.AvailabilityRES;
            var _criteria = criteria as RaynaCriteria;
            var productMappings = _criteria?.ProductMapping;
            if (_criteria?.ProductMapping != null && _criteria?.ProductMapping.Count > 0)
            {
                foreach (var dataItem in _criteria?.ProductMapping)
                {
                    if (dataItem.HotelBedsActivityCode.Contains('_'))
                    {
                        dataItem.HotelBedsActivityCode = dataItem.HotelBedsActivityCode.Split('_')[0];
                    }
                }
            }
            try
            {
                if (!(productMappings?.Count > 0))
                {
                    return null;
                }
                if (result != null && availabilityTourOptionRS?.Count > 0)
                {
                    var getAllOptionsofAllDates = availabilityTourOptionRS.SelectMany(x => x.Values.SelectMany(y => y.AvailabilityOptionResult)).ToList();
                    //filter options by tourId,tourOptionId and transferId
                    var distinctGetAllOptionsofAllDates = getAllOptionsofAllDates
                    ?.GroupBy(grp => new
                    {
                        grp.TourId,
                        grp.TourOptionId,
                        grp.TransferId
                    })?.Select(grp => grp.FirstOrDefault())?.ToList();

                    var tourId = System.Convert.ToString(distinctGetAllOptionsofAllDates?.FirstOrDefault()?.TourId);
                    var tourCurrency = availabilityTourOptionRS?.FirstOrDefault()?.Values?.FirstOrDefault()?.Currency;
                    //default currency of API is AED
                    tourCurrency = String.IsNullOrEmpty(tourCurrency) ? "AED" : tourCurrency;
                    var mappedProduct = productMappings?.FirstOrDefault(x => x.HotelBedsActivityCode == tourId);
                    if (mappedProduct == null) return null;

                    activitiesIsango = new Activity
                    {
                        ID = mappedProduct.IsangoHotelBedsActivityId,//isangoId
                        Code = mappedProduct.HotelBedsActivityCode, //api id
                        CurrencyIsoCode = tourCurrency,
                        ApiType = mappedProduct.ApiType
                    };

                    var options = new List<ActivityOption>();

                    try
                    {
                        _currencyISOCode = tourCurrency;
                        _adultCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value ?? 0;
                        _childCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value ?? 0;
                        _infantCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value ?? 0;
                        _totalPaxCount = _adultCount + _childCount + _infantCount;

                        activitiesIsango.ProductOptions = new List<ProductOption>();

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
                            foreach (var availabilityOption in distinctGetAllOptionsofAllDates)
                            {
                                var getAvailabilityTimeSlotRS = availabilityTimeSlotRS?.SelectMany(x => x.Item5.ResultAvailabilityTimeSlot)?.ToList();
                                var filterGetAvailabilityTimeSlotRS = getAvailabilityTimeSlotRS?.Where(x => x.TourOptionId == availabilityOption.TourOptionId && x.TransferId == availabilityOption.TransferId)?.ToList();
                                var distinctGetAvailabilityTimeSlotRS = filterGetAvailabilityTimeSlotRS
                                  ?.GroupBy(grp => new
                                  {
                                      grp.TourOptionId,
                                      grp.TransferId,
                                      grp.TimeSlotId
                                  })?.Select(grp => grp.FirstOrDefault())?.ToList();

                                //Time-Based
                                if (distinctGetAvailabilityTimeSlotRS != null && distinctGetAvailabilityTimeSlotRS.Count > 0)
                                {
                                    foreach (var slot in distinctGetAvailabilityTimeSlotRS)
                                    {
                                        var option = CreateOption(travelInfo: travelInfo, criteria: _criteria,
                                                 availabilityOption: availabilityOption,
                                                result: result, slot: slot);

                                        if (option != null)
                                        {
                                            option.Name = $"{option.Name}";
                                            options.Add(option);
                                        }
                                    }
                                }
                                else
                                {

                                    var option = CreateOption(travelInfo: travelInfo, criteria: _criteria,
                                             availabilityOption: availabilityOption,
                                            result: result, slot: null);

                                    if (option != null)
                                    {
                                        option.Name = $"{option.Name}";
                                        options.Add(option);
                                    }
                                }

                            }


                        }
                        catch (Exception ex)
                        {
                            //ignore
                            //#TODO add logging here;
                        }
                        var availableOptions = options.Where(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                        activitiesIsango.ProductOptions = availableOptions?.Cast<ProductOption>()?.ToList();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        //TODOlogging
                        activitiesIsango = null;
                    }
                    if (activitiesIsango?.ProductOptions?.Count > 0)
                    {
                        return activitiesIsango;

                    }

                }
            }
            catch (Exception ex)
            {
                //ignored
                //##TODO Add logging here
            }
            return activitiesIsango;
        }

        private ActivityOption CreateOption(TravelInfo travelInfo, RaynaCriteria criteria,
            AvailabilityOptionResult availabilityOption,
            AvailabilityReturnData result, ResultAvailabilityTimeSlot slot)
        {
            var option = default(ActivityOption);
            var costPrice = new Price();
            var pAndACost = new Dictionary<DateTime, PriceAndAvailability>();
            decimal totalCostPrice = 0;
            int totalCapacity = 0;
            var availabilityTourOptionRS = result?.AvailabilityTourOptionRS;
            var availabilityTimeSlotRS = result?.AvailabilityTimeSlotRS;
            var availabilityRES = result?.AvailabilityRES;
            try
            {

                TimeSpan.TryParse(slot?.TimeSlot, out var startTimeSlot);
                Int32.TryParse(slot?.TimeSlotId, out var timeSlotId);
                option = new ActivityOption
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Name = availabilityOption?.TransferName,
                    Code = System.Convert.ToString(availabilityOption?.TourOptionId)+"_"+System.Convert.ToString(availabilityOption.TransferId),
                    SupplierOptionCode = System.Convert.ToString(availabilityOption?.TourId),//API product id
                    RateKey = System.Convert.ToString(availabilityOption.TransferId),//API transfer Id
                    PrefixServiceCode = System.Convert.ToString(availabilityOption?.TourOptionId),//API option Id
                    ServiceOptionId = System.Convert.ToInt32(criteria.ServiceOptionID),//isango option ID 
                    TravelInfo = new TravelInfo
                    {
                        Ages = criteria.Ages,
                        NoOfPassengers = criteria.NoOfPassengers,
                        NumberOfNights = 0,
                        StartDate = criteria.CheckinDate
                    },
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE,
                    ApiType = APIType.Rayna,
                    IsTimeBasedOption = slot == null ? false : true,
                    StartTime = startTimeSlot,
                    TimeSlotId= timeSlotId,
                    Variant= System.Convert.ToString(availabilityOption.TransferId),//API transfer Id
                };

                foreach (var dayData in availabilityTourOptionRS.FirstOrDefault().Keys)
                {
                    var priceCost = new DefaultPriceAndAvailability
                    {
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE
                    };

                    var passDateTimeValue = dayData;

                    priceCost = UpdatePricePerPax(travelInfo, criteria, priceCost,
                     availabilityOption, result, passDateTimeValue, slot);

                    if (priceCost != null)
                    {
                        totalCostPrice = priceCost.TotalPrice;
                        totalCapacity = priceCost.PricingUnits.FirstOrDefault().TotalCapacity;
                        priceCost.Capacity = totalCapacity;
                        if (!pAndACost.Keys.Contains(passDateTimeValue))
                        {
                            pAndACost.Add(passDateTimeValue, priceCost);
                        }
                    }
                }
                //Price and Currency
                costPrice.DatePriceAndAvailabilty = pAndACost;
                costPrice.Amount = System.Convert.ToDecimal(totalCostPrice);
                costPrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.CostPrice = costPrice;
                option.Capacity = totalCapacity;
                option.AvailabilityStatus = option?.CostPrice?.Amount > 0 ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
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
            TravelInfo travelInfo, RaynaCriteria criteria,
            DefaultPriceAndAvailability price,
            AvailabilityOptionResult availabilityOption, 
            AvailabilityReturnData result,
            DateTime passDateTime,
            ResultAvailabilityTimeSlot slot)
        {
            //Filter Data only by Particular Date (All Three)
            var availabilityTourOptionRS = result?.AvailabilityTourOptionRS?.SelectMany(x => x)?.Where(x => x.Key == passDateTime)?.FirstOrDefault();
            var availabilityTimeSlotRS = result?.AvailabilityTimeSlotRS?.Where(x => x.Item1 == passDateTime)?.ToList();
            var availabilityRES = result?.AvailabilityRES?.Where(x => x.Item1 == passDateTime.Date).ToList();

            if ((availabilityTourOptionRS.Value.Key == DateTime.MinValue) || (availabilityRES == null && availabilityRES.Count > 0))
            {
                return null;
            }

            //Filter by tourId, tourOptionId and transferId
            var filterAvailabilityTourOptionRS = availabilityTourOptionRS.Value.Value.AvailabilityOptionResult?.Where(x => x.TourId == availabilityOption.TourId && x.TourOptionId == availabilityOption.TourOptionId && x.TransferId == availabilityOption.TransferId)?.FirstOrDefault();

            //Filter by parameter slot pass- single
            var filterAvailabilityTimeSlotRS = availabilityTimeSlotRS?.Where(x => x.Item3 == slot.TourOptionId && x.Item4 == slot.TransferId)?.FirstOrDefault()?.Item5.ResultAvailabilityTimeSlot.Where(x=>x.TimeSlotId== slot.TimeSlotId)?.FirstOrDefault();

            var filterAvailabilityRES = availabilityRES?.Where(x => x.Item2 == availabilityOption.TourId && x.Item3 == availabilityOption.TourOptionId && x.Item4 == availabilityOption.TransferId)?.FirstOrDefault()?.Item5;

            if (filterAvailabilityTourOptionRS == null || filterAvailabilityRES == null)
            {
                return null;
            }

            decimal adultCostPrice = 0;
            decimal childCostPrice = 0;
            decimal infantCostPrice = 0;

            price.PricingUnits = new List<PricingUnit>();

            //dynamic price case
            if (filterAvailabilityTimeSlotRS!=null && filterAvailabilityTimeSlotRS.IsDynamicPrice == true)
            {
                var adultPriceQuery = from m in new List<ResultAvailabilityTimeSlot> { filterAvailabilityTimeSlotRS }
                                      from pax in travelInfo.Ages
                                      where pax.Key == PassengerType.Adult
                                      && m.Available > 0
                                      && m.AdultPrice > 0
                                      select m;
                if (adultPriceQuery?.Any() == true)
                {
                    _adultCount = 1;
                }

                var childPriceQuery = from m in new List<ResultAvailabilityTimeSlot> { filterAvailabilityTimeSlotRS }
                                      from pax in travelInfo.Ages
                                      where pax.Key == PassengerType.Child
                                      && m.Available > 0
                                      && m.ChildPrice > 0
                                      select m;
                if (childPriceQuery?.Any() == true)
                {
                    _childCount = 1;
                }

                var infantPriceQuery = from m in new List<ResultAvailabilityTimeSlot> { filterAvailabilityTimeSlotRS }
                                       from pax in travelInfo.Ages
                                       where pax.Key == PassengerType.Infant
                                       && m.Available > 0
                                       //&& m.InfantPrice > 0
                                       select m;
                if (infantPriceQuery?.Any() == true)
                {
                    _infantCount = 1;
                }

                var adultPriceNode = adultPriceQuery?.FirstOrDefault();
                var childPriceNode = childPriceQuery?.FirstOrDefault();
                var infantPriceNode = infantPriceQuery?.FirstOrDefault();


                adultCostPrice = System.Convert.ToDecimal(adultPriceNode?.AdultPrice);
                childCostPrice = System.Convert.ToDecimal(childPriceNode?.ChildPrice);
                infantCostPrice = System.Convert.ToDecimal(infantPriceNode?.InfantPrice);


                if (_adultCount > 0 && adultPriceNode != null)
                {
                    var adultPriceUnit = new AdultPricingUnit
                    {
                        Price = adultCostPrice,
                        Quantity = _adultCount,
                        TotalCapacity = slot != null ? System.Convert.ToInt32(filterAvailabilityTimeSlotRS.Available) : 0
                    };
                    price.PricingUnits.Add(adultPriceUnit);
                }


                if (_childCount > 0 && childPriceNode != null)
                {
                    var childPriceUnit = new ChildPricingUnit
                    {
                        Price = childCostPrice,
                        Quantity = _childCount,
                        TotalCapacity = slot != null ? System.Convert.ToInt32(filterAvailabilityTimeSlotRS?.Available) : 0
                    };
                    price.PricingUnits.Add(childPriceUnit);
                }


                if (_infantCount > 0 && infantPriceNode != null)
                {
                    var infantPriceUnit = new InfantPricingUnit
                    {
                        Price = infantCostPrice,
                        Quantity = _infantCount
                    };
                    price.PricingUnits.Add(infantPriceUnit);
                }

                price.TotalPrice = (adultCostPrice * _adultCount)
                    + (childCostPrice * _childCount)
                    + (infantCostPrice + _infantCount);



                if (price.TotalPrice <= 0 || filterAvailabilityRES?.ResultAvailability?.Message?.ToLowerInvariant() != "success")
                {
                    price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                }
                //if timeslot products
                if (slot != null)
                {
                    if (filterAvailabilityTimeSlotRS.Available <= 0)
                    {
                        price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    }
                }

                return price;

            }
            //simple case and if slot products have IsDynamicPrice=false
            else
            {

                var adultPriceQuery = from m in new List<AvailabilityOptionResult> { filterAvailabilityTourOptionRS }
                                      from pax in travelInfo.Ages
                                      where pax.Key == PassengerType.Adult
                                      && m.AdultPrice > 0
                                      select m;
                if (adultPriceQuery?.Any() == true)
                {
                    _adultCount = 1;
                }

                var childPriceQuery = from m in new List<AvailabilityOptionResult> { filterAvailabilityTourOptionRS }
                                      from pax in travelInfo.Ages
                                      where pax.Key == PassengerType.Child
                                      && m.ChildPrice > 0
                                      select m;
                if (childPriceQuery?.Any() == true)
                {
                    _childCount = 1;
                }

                var infantPriceQuery = from m in new List<AvailabilityOptionResult> { filterAvailabilityTourOptionRS }
                                       from pax in travelInfo.Ages
                                       where pax.Key == PassengerType.Infant
                                       //&& m.InfantPrice > 0
                                       select m;
                if (infantPriceQuery?.Any() == true)
                {
                    _infantCount = 1;
                }

                var adultPriceNode = adultPriceQuery?.FirstOrDefault();
                var childPriceNode = childPriceQuery?.FirstOrDefault();
                var infantPriceNode = infantPriceQuery?.FirstOrDefault();


                adultCostPrice = System.Convert.ToDecimal(adultPriceNode?.AdultPrice);
                childCostPrice = System.Convert.ToDecimal(childPriceNode?.ChildPrice);
                infantCostPrice = System.Convert.ToDecimal(infantPriceNode?.InfantPrice);


                if (_adultCount > 0 && adultPriceNode != null)
                {
                    var adultPriceUnit = new AdultPricingUnit
                    {
                        Price = adultCostPrice,
                        Quantity = _adultCount,
                        TotalCapacity = slot != null ? System.Convert.ToInt32(filterAvailabilityTimeSlotRS?.Available) : 0
                    };
                    price.PricingUnits.Add(adultPriceUnit);
                }


                if (_childCount > 0 && childPriceNode != null)
                {
                    var childPriceUnit = new ChildPricingUnit
                    {
                        Price = childCostPrice,
                        Quantity = _childCount,
                        TotalCapacity = slot != null ? System.Convert.ToInt32(filterAvailabilityTimeSlotRS?.Available) : 0
                    };
                    price.PricingUnits.Add(childPriceUnit);
                }


                if (_infantCount > 0 && infantPriceNode != null)
                {
                    var infantPriceUnit = new InfantPricingUnit
                    {
                        Price = infantCostPrice,
                        Quantity = _infantCount
                    };
                    price.PricingUnits.Add(infantPriceUnit);
                }
                var servicePrice = System.Convert.ToDecimal(filterAvailabilityTourOptionRS?.FinalAmount);
                if (servicePrice.Equals(0))
                {
                    price.TotalPrice = (adultCostPrice * _adultCount)
                    + (childCostPrice * _childCount)
                    + (infantCostPrice + _infantCount);
                }
                else
                    price.TotalPrice = servicePrice;

                if (price.TotalPrice <= 0 || filterAvailabilityRES?.ResultAvailability?.Message?.ToLowerInvariant() != "success")
                {
                    price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                }
                //if timeslot products
                if (slot != null)
                {
                    if (filterAvailabilityTimeSlotRS.Available <= 0)
                    {
                        price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    }
                }

                return price;

            }
        }
        #endregion Private Methods
    }
}