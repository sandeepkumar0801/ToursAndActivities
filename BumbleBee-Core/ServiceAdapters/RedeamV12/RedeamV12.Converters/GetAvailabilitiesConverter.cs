using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.RedeamV12;
using ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetAvailabilities;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetRate;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.PricingSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;
using Constant = ServiceAdapters.RedeamV12.Constants.Constant;
using CONSTANTCANCELLATION = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.RedeamV12.RedeamV12.Converters
{
    public class GetAvailabilitiesConverter : ConverterBase, IGetAvailabilitiesConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="extraRequest"></param>
        /// <returns></returns>
        //public override object Convert<T>(T response, T request, T extraRequest)
        //{
        //    // Intializing result if response is null in case of FS and PASS type
        //    var result = new AvailabilitiesResponse();
        //    if (response != null)
        //    {
        //        result = SerializeDeSerializeHelper.DeSerialize<AvailabilitiesResponse>(response.ToString());
        //    }
        //    return ConvertAvailabilityResult(result, extraRequest as GetRateResponse, request as CanocalizationCriteria);
        //}

        public override object Convert<T>(T response, T request, T extraRequest, T pricing)
        {
            // Intializing result if response is null in case of FS and PASS type
            var result = new AvailabilitiesResponse();
            if (response != null)
            {
                result = SerializeDeSerializeHelper.DeSerialize<AvailabilitiesResponse>(response.ToString());
            }
            return ConvertAvailabilityResult(result, extraRequest as GetRateResponse, request as CanocalizationCriteria, pricing as Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>>);
        }

        #region Private Methods

        /// <summary>
        /// Prepare the product options using the response data of supplier
        /// </summary>
        /// <param name="availabilitiesResponse"></param>
        /// <param name="ratesResponse"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<ProductOption> ConvertAvailabilityResult(AvailabilitiesResponse availabilitiesResponse, GetRateResponse ratesResponse,
            CanocalizationCriteria criteria,
            Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>> pricing)
        {
            if (ratesResponse != null && ratesResponse?.Rate?.Prices != null && ratesResponse?.Rate?.Prices.Count > 0)
            {
                if (ratesResponse.Rate.Prices.All(x => x.Labels.Count() > 0))
                {
                    string dataFilter = "ISANGO";
                    var data = ratesResponse?.Rate?.Prices?.Where(priceItem => priceItem.Labels.Contains(dataFilter)).ToList();
                    if (data != null && data.Count > 0)
                    {
                        ratesResponse.Rate.Prices = data;
                    }

                }
            }



            var productOptions = new List<ProductOption>();
            var rate = ratesResponse.Rate;
            var rateType = criteria.RateIdAndType?.FirstOrDefault(x => x.Key == $"{criteria.ProductId}#{criteria.RateId}").Value;
            if (string.IsNullOrEmpty(rateType) || rate == null) return productOptions;

            var maxTravelersAllowed = rate?.MaxTravelers;
            var selectedPax = criteria.NoOfPassengers?.Sum(x => x.Value);

            // creating availabilities for the Pass and FreeSale type, this will override in case of Reserved type
            var apiAvailabilities = new List<Availability>();

            if (availabilitiesResponse.Availabilities?.ByRate?.String != null && rateType == Constant.ReservedType)
            {
                var availabilitiesByRateData = availabilitiesResponse.Availabilities.ByRate.String.Select(x => x);
                var availabilitiesByRate = availabilitiesByRateData.ToDictionary(x => x.Key,
                        y => SerializeDeSerializeHelper.DeSerialize<ByProduct>(y.Value.ToString()).Availability);

                var availabilitiesByRateId = availabilitiesByRate.FirstOrDefault(x => x.Key.Equals(criteria.RateId)).Value;
                if (availabilitiesByRateId == null) return productOptions;

                apiAvailabilities = availabilitiesByRateId;
            }
            else
            {
                //if criteria checkin date and checkout date is same, then adding 1 day's availability

                //setting it to one as date rage is required in price
                //For each day one option with singe P&A not needed
                /*
                var totalDays = criteria.CheckoutDate.Subtract(criteria.CheckinDate).TotalDays;
                if (totalDays == 0)
                    totalDays++;
                //

                for (int day = 0; day < totalDays; day++)
                {
                    var availabledate = new Availability { Start = criteria.CheckinDate.AddDays(day) };
                    availabilities.Add(availabledate);
                }
                */
                var availabledate = new Availability { Start = criteria.CheckinDate };
                apiAvailabilities.Add(availabledate);
            }
            var currentAvailabilities = apiAvailabilities.Where(x => x.Start.Date == criteria.CheckinDate.Date).ToList();
            if (currentAvailabilities == null || currentAvailabilities.Count==0)
            {
                for (var dateData = criteria.CheckinDate; dateData <= criteria.CheckoutDate; dateData = dateData.AddDays(1))
                {
                    currentAvailabilities = apiAvailabilities?.Where(x => x.Start.Date == dateData.Date)?.ToList();
                    if (currentAvailabilities.Count!= 0)
                    {
                        break;
                    }
                }
            }
            foreach (var availability in currentAvailabilities)
            {
                var validPaxCriteriaForReserved = CheckPaxForReserveType(availability, selectedPax, rateType) ? AvailabilityStatus.NOTAVAILABLE : AvailabilityStatus.AVAILABLE;
                var availabilityStatus = (maxTravelersAllowed < selectedPax) ? AvailabilityStatus.NOTAVAILABLE : validPaxCriteriaForReserved;

                var productOption = MapProductOptionAndPrice(ratesResponse, criteria, availability,
                    availabilityStatus, apiAvailabilities, rateType, pricing);
                if (productOption != null)
                {
                    if (availability.Start != null)
                        productOptions.Add(productOption);
                }
            }
            return productOptions;
        }

        /// <summary>
        /// Prepare options for different rate type
        /// </summary>
        /// <param name="ratesResponse"></param>
        /// <param name="criteria"></param>
        /// <param name="time"></param>
        /// <param name="availabilityStatus"></param>
        /// <returns></returns>
        private ProductOption MapProductOptionAndPrice(GetRateResponse ratesResponse,
            CanocalizationCriteria criteria,
            Availability availability, AvailabilityStatus availabilityStatus,
            List<Availability> lstAPIAvailabilities, string rateType,
            Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>> pricing)
        {

            //difference between StartTime (timeZoneformat) and RedeamAvailabilitystart(api UTC format)

            var timeZone = "Central Standard Time";

            var language = criteria?.Language?.ToLower() ?? "en";
            var rate = ratesResponse.Rate;
            var currency = criteria.Currency;
            //note:pick up currency based on database,
            //not from APi(bec api give multiple currencies)
            var timeString = availability.Start.ToString(Constant.TimeFormat);
            if (rate?.Prices == null) return null;

            //Need to convert UTC (api receive time) to CST TimeZone 
            var tzCstTime = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var utcTime = availability.Start;
            DateTime getCstTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzCstTime);

            var productOption = new ActivityOption()
            {
                SupplierOptionCode = $"{criteria.ProductId}#{criteria.RateId}",
                //Name = timeString == "12:00 AM" ? string.Empty : timeString,
                TravelInfo = new TravelInfo
                {
                    StartDate = criteria.CheckinDate.Date,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages,
                    NumberOfNights = (criteria.CheckoutDate - criteria.CheckinDate).Days
                },
                AvailabilityStatus = availabilityStatus,
                Cancellable = rate.Cancelable,
                Holdable = rate.Holdable,
                Refundable = rate.Refundable,
                HoldablePeriod = rate.HoldablePeriod,
                Time = timeString,
                Type = rate?.Type,
                RateId = criteria?.RateId,
                SupplierId = criteria?.SupplierId,
                ServiceOptionId = criteria.ServiceOptionId,
                ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(new
                {
                    RateCode = rate.Code,
                    IsRefundable = rate.Refundable,
                    IsCancelable = rate.Cancelable,
                    rate.Cutoff,
                }),
                StartTime = getCstTime.TimeOfDay,// it is in timezone format
                Capacity = System.Convert.ToInt32(rateType == Constant.ReservedType ? availability?.Capacity : rate.MaxTravelers),
            };

            //Set Cancellation text and cancel-able here
            if (productOption.Refundable == false)
            {
                productOption.Cancellable = false;
                productOption.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANTCANCELLATION.CancellationPolicyNonRefundable);
            }
            else if (productOption.Refundable == true)
            {
                productOption.Cancellable = true;
                productOption.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANTCANCELLATION.CancellationPolicyDefaultFree24Hours);
            }

           
            var ratePriceQuery = from price in rate.Prices
                                 from paxtype in criteria.NoOfPassengers
                                 where string.Equals(price.TravelerType.AgeBand
                                                        , paxtype.Key.ToString()
                                                        , StringComparison.CurrentCultureIgnoreCase
                                                    )
                                 select price;

            if (ratePriceQuery.Any())
            {
                rate.Prices = ratePriceQuery
                                .Where(y => string.Equals(y.Status
                                                        , "active"
                                                        , StringComparison.CurrentCultureIgnoreCase
                                                    )
                                                    )
                                .OrderBy(x => x.TravelerType.AgeBand)
                                //.ThenBy(x => x.Net.Amount)
                                .ThenBy(x => x.Name)
                                .ToList();
            }
            else
            {
                return null;
            }

            var filterPrice = pricing?.Select(x => x.Value)?.ToList();
            productOption.CostPrice = new Isango.Entities.Price
            {
                Amount =0,
                Currency = new Currency { IsoCode = currency },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>(),
            };

            productOption.BasePrice = new Isango.Entities.Price
            {
                Amount = 0,
                Currency = new Currency { IsoCode = currency },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
            };

            productOption.GateBasePrice = new Isango.Entities.Price
            {
                Amount = 0,
                Currency = new Currency { IsoCode = currency },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
            };

            for (var date = criteria.CheckinDate; date <= criteria.CheckoutDate; date = date.AddDays(1))
            {
                var lstPricingScheduleResponse =new List<PricingScheduleResponse>();
                if (filterPrice != null)
                {
                    try
                    {
                        if (filterPrice.FirstOrDefault().ContainsKey(date.ToString(Constant.DateTimeStringFormatSingle)))
                        {
                            var finalFilteredPrice = filterPrice?.FirstOrDefault()?.Where(x => x.Key == date.ToString(Constant.DateTimeStringFormatSingle))?.ToDictionary(k => k.Key, k => k.Value);
                            var finalFilteredPriceData = finalFilteredPrice?.Values?.ToList();
                            string filterData = "ISANGO";
                            if (finalFilteredPriceData?.FirstOrDefault()?.FirstOrDefault()?.labels?.FirstOrDefault() != null)
                            {
                                lstPricingScheduleResponse = finalFilteredPriceData?.FirstOrDefault()?.Where(priceData => priceData.labels.Contains(filterData))?.ToList();
                            }
                            else
                            {
                                lstPricingScheduleResponse = finalFilteredPriceData?.FirstOrDefault()?.ToList();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                //,retail=sell,net=cost,original=gatebaseprice
                var pandaCost = PriceAndAvailabilityCostData(availability, rate, criteria, availabilityStatus,lstPricingScheduleResponse);
                var pandaBase = PriceAndAvailabilitySellData(availability, rate, criteria, availabilityStatus,lstPricingScheduleResponse);
                var pandaGateBase = PriceAndAvailabilityGateBaseData(availability, rate, criteria, availabilityStatus, lstPricingScheduleResponse);

                var priceIds = new Dictionary<string, string>();
                if (lstPricingScheduleResponse != null && lstPricingScheduleResponse.Count > 0)
                {
                    foreach (var priceAndAgeBand in lstPricingScheduleResponse)
                    {
                        if (priceAndAgeBand.net.currency?.ToUpper() == criteria.Currency?.ToUpper()
                            || priceAndAgeBand.retail.currency?.ToUpper() == criteria.Currency?.ToUpper()
                            || priceAndAgeBand.original.currency?.ToUpper() == criteria.Currency?.ToUpper())
                        {
                            if (!priceIds.ContainsKey(priceAndAgeBand.travelerType.ageBand))
                                priceIds.Add(priceAndAgeBand.travelerType.ageBand, priceAndAgeBand.id.ToString());
                        }
                    }
                }
                else
                {
                    foreach (var priceAndAgeBand in rate.Prices)
                    {
                        if (!priceIds.ContainsKey(priceAndAgeBand.TravelerType.AgeBand))
                            priceIds.Add(priceAndAgeBand.TravelerType.AgeBand, priceAndAgeBand.Id.ToString());
                    }

                    
                }
               

                    //for calendar dumping
                    productOption.CostPrice.Amount = pandaCost.TotalPrice;
                    productOption.BasePrice.Amount = pandaBase.TotalPrice;
                    productOption.GateBasePrice.Amount = pandaGateBase.TotalPrice;
                

                if (!productOption.CostPrice.DatePriceAndAvailabilty.ContainsKey(date))
                {
                    var getlstAPIAvailabilities = lstAPIAvailabilities;
                    var getFilterData = getlstAPIAvailabilities?.Where(x => x.Start.Date == date.Date && x.Start.TimeOfDay == availability?.Start.TimeOfDay)?.FirstOrDefault();
                    if (getFilterData != null)
                    {
                        string GetPriceIds = string.Join(";", priceIds.Select(x => x.Key + "=" + x.Value));
                        if (rateType == Constant.ReservedType)
                        {  //availabilityid

                            ////Need to convert UTC (api receive time) to CST TimeZone 
                            //var tzCstTimeData = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                            //var utcTimeData = getFilterData.Start;
                            //DateTime getCstTimeData = TimeZoneInfo.ConvertTimeFromUtc(utcTimeData, tzCstTimeData);

                            pandaCost.ReferenceId = 
                                System.Convert.ToString(getFilterData.Id) + 
                                "|"+ System.Convert.ToString(getFilterData.Start) +
                                "|"+ GetPriceIds +
                                "|"+ pandaCost.TotalPrice+"|"+ pandaBase.TotalPrice+"|"+ pandaGateBase.TotalPrice;
                            pandaCost.Capacity = System.Convert.ToInt32(getFilterData.Capacity);
                            pandaCost.IsCapacityCheckRequired = pandaCost.Capacity > 0 ? true : false;
                        }
                        else
                        {
                            pandaCost.ReferenceId = "" + 
                                "|" + "" +
                                "|" + GetPriceIds+
                                "|" + pandaCost.TotalPrice + "|" + pandaBase.TotalPrice + "|" + pandaGateBase.TotalPrice; 
                        }
                    }
                    else
                    {
                        pandaCost.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    }
                    productOption.CostPrice.DatePriceAndAvailabilty.Add(date, pandaCost);
                    
                }

                if (!productOption.BasePrice.DatePriceAndAvailabilty.ContainsKey(date))
                {
                    var getlstAPIAvailabilities = lstAPIAvailabilities;
                    var getFilterData = getlstAPIAvailabilities?.Where(x => x.Start.Date == date.Date && x.Start.TimeOfDay == availability?.Start.TimeOfDay)?.FirstOrDefault();
                    if (getFilterData != null)
                    {
                       string GetPriceIds = string.Join(";", priceIds.Select(x => x.Key + "=" + x.Value));
                        if (rateType == Constant.ReservedType)
                        {
                            //Need to convert UTC (api receive time) to CST TimeZone 
                            //var tzCstTimeData = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                            //var utcTimeData = getFilterData.Start;
                            //DateTime getCstTimeData = TimeZoneInfo.ConvertTimeFromUtc(utcTimeData, tzCstTimeData);

                            //availabilityid
                            pandaBase.ReferenceId = System.Convert.ToString(getFilterData.Id) +
                                "|" + System.Convert.ToString(getFilterData.Start) +
                                "|" + GetPriceIds +
                                "|" + pandaCost.TotalPrice + "|" + pandaBase.TotalPrice + "|" + pandaGateBase.TotalPrice;
                            pandaBase.Capacity = System.Convert.ToInt32(getFilterData.Capacity);
                            pandaBase.IsCapacityCheckRequired = pandaCost.Capacity > 0 ? true : false;

                        }
                        else
                        {
                            pandaBase.ReferenceId = "" +
                                "|" + "" +
                                "|" + GetPriceIds +
                                "|" + pandaCost.TotalPrice + "|" + pandaBase.TotalPrice + "|" + pandaGateBase.TotalPrice;
                        }
                    }
                    else
                    {
                        pandaBase.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    }
                    productOption.BasePrice.DatePriceAndAvailabilty.Add(date, pandaBase);
                }
                if (!productOption.GateBasePrice.DatePriceAndAvailabilty.ContainsKey(date))
                {
                    var getlstAPIAvailabilities = lstAPIAvailabilities;
                    var getFilterData = getlstAPIAvailabilities?.Where(x => x.Start.Date == date.Date && x.Start.TimeOfDay == availability?.Start.TimeOfDay)?.FirstOrDefault();
                    if (getFilterData != null)
                    {
                        string GetPriceIds = string.Join(";", priceIds.Select(x => x.Key + "=" + x.Value));
                        //Need to convert UTC (api receive time) to CST TimeZone 
                        //var tzCstTimeData = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                        //var utcTimeData = getFilterData.Start;
                        //DateTime getCstTimeData = TimeZoneInfo.ConvertTimeFromUtc(utcTimeData, tzCstTimeData);

                        if (rateType == Constant.ReservedType)
                        {  //availabilityid
                            pandaGateBase.ReferenceId = System.Convert.ToString(getFilterData.Id) +
                                "|" + System.Convert.ToString(getFilterData.Start) +
                                "|" + GetPriceIds +
                                "|" + pandaCost.TotalPrice + "|" + pandaBase.TotalPrice + "|" + pandaGateBase.TotalPrice;
                            pandaGateBase.Capacity = System.Convert.ToInt32(getFilterData.Capacity);
                            pandaGateBase.IsCapacityCheckRequired = pandaCost.Capacity > 0 ? true : false;
                        }
                        else
                        {
                            pandaGateBase.ReferenceId = "" +
                                "|" + "" +
                                "|" + GetPriceIds +
                                "|" + pandaCost.TotalPrice + "|" + pandaBase.TotalPrice + "|" + pandaGateBase.TotalPrice;
                        }
                    }
                    else
                    {
                        pandaGateBase.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    }
                    productOption.GateBasePrice.DatePriceAndAvailabilty.Add(date, pandaGateBase);

                }
            }

            return productOption;
        }

        private PriceAndAvailability PriceAndAvailabilityCostData(Availability availability, Rate rate,
            CanocalizationCriteria criteria, AvailabilityStatus availabilityStatus,
            List<PricingScheduleResponse> lstPricingScheduleResponse)
        {
            var pricingUnitsCostPrice = new List<PricingUnit>();
            if (lstPricingScheduleResponse != null && lstPricingScheduleResponse.Count > 0)
            {
                pricingUnitsCostPrice = CreatePricingUnitsSchedule(lstPricingScheduleResponse, criteria, 2);
            }
            else
            {
                pricingUnitsCostPrice = CreatePricingUnits(rate.Prices, criteria, 2);
            }

            if (pricingUnitsCostPrice.Count == 0) return null;

            foreach (var pu in pricingUnitsCostPrice)
            {
                if (availability != null && (availability.Capacity > 0))
                {
                    System.Int32.TryParse(availability?.Capacity.ToString(), out var tempintMax);
                    pu.TotalCapacity = tempintMax;
                }
                else
                {
                    System.Int32.TryParse(rate?.MaxTravelers.ToString(), out var tempintMax);
                    pu.TotalCapacity = tempintMax;
                }

                System.Int32.TryParse(rate?.MinTravelers.ToString(), out var tempintMin);
                pu.Mincapacity = tempintMin;
            }

            var costPriceAndAvailability = new DefaultPriceAndAvailability
            {
                AvailabilityStatus = availabilityStatus,
                TotalPrice = pricingUnitsCostPrice.Sum(x => x.Price),
                PricingUnits = pricingUnitsCostPrice
            };


            return costPriceAndAvailability;
        }

        private PriceAndAvailability PriceAndAvailabilitySellData(Availability availability, Rate rate,
            CanocalizationCriteria criteria, AvailabilityStatus availabilityStatus,
            List<PricingScheduleResponse> lstPricingScheduleResponse)
        {
            var pricingUnitsBasePrice = new List<PricingUnit>();
            if (lstPricingScheduleResponse != null && lstPricingScheduleResponse.Count > 0)
            {
                pricingUnitsBasePrice = CreatePricingUnitsSchedule(lstPricingScheduleResponse, criteria, 1);
            }
            else
            {
                pricingUnitsBasePrice = CreatePricingUnits(rate.Prices, criteria, 1);
            }

            if (pricingUnitsBasePrice.Count == 0) return null;

            foreach (var puBase in pricingUnitsBasePrice)
            {
                if (availability != null && (availability.Capacity > 0))
                {
                    System.Int32.TryParse(availability?.Capacity.ToString(), out var tempintMax);
                    puBase.TotalCapacity = tempintMax;
                }
                else
                {
                    System.Int32.TryParse(rate?.MaxTravelers.ToString(), out var tempintMax);
                    puBase.TotalCapacity = tempintMax;
                }
                System.Int32.TryParse(rate?.MinTravelers.ToString(), out var tempintMin);
                puBase.Mincapacity = tempintMin;
            }

            var basePriceAndAvailability = new DefaultPriceAndAvailability
            {
                AvailabilityStatus = availabilityStatus,
                TotalPrice = pricingUnitsBasePrice.Sum(x => x.Price),
                PricingUnits = pricingUnitsBasePrice
            };

            return basePriceAndAvailability;
        }
        private PriceAndAvailability PriceAndAvailabilityGateBaseData(Availability availability, Rate rate,
            CanocalizationCriteria criteria, AvailabilityStatus availabilityStatus,
            List<PricingScheduleResponse> lstPricingScheduleResponse)
        {
            var pricingUnitsGateBasePrice = new List<PricingUnit>();
            if (lstPricingScheduleResponse != null && lstPricingScheduleResponse.Count > 0)
            {
                pricingUnitsGateBasePrice = CreatePricingUnitsSchedule(lstPricingScheduleResponse, criteria, 3);
            }
            else
            {
                pricingUnitsGateBasePrice = CreatePricingUnits(rate.Prices, criteria, 3);
            }

            if (pricingUnitsGateBasePrice.Count == 0) return null;

            foreach (var puBase in pricingUnitsGateBasePrice)
            {
                if (availability != null && (availability.Capacity > 0))
                {
                    System.Int32.TryParse(availability?.Capacity.ToString(), out var tempintMax);
                    puBase.TotalCapacity = tempintMax;
                }
                else
                {
                    System.Int32.TryParse(rate?.MaxTravelers.ToString(), out var tempintMax);
                    puBase.TotalCapacity = tempintMax;
                }
                System.Int32.TryParse(rate?.MinTravelers.ToString(), out var tempintMin);
                puBase.Mincapacity = tempintMin;
            }

            var basePriceAndAvailability = new DefaultPriceAndAvailability
            {
                AvailabilityStatus = availabilityStatus,
                TotalPrice = pricingUnitsGateBasePrice.Sum(x => x.Price),
                PricingUnits = pricingUnitsGateBasePrice
            };

            return basePriceAndAvailability;
        }

        private List<PricingUnit> CreatePricingUnits(List<Entities.GetRate.Price> prices, 
            CanocalizationCriteria criteria, int IsBasePrice)
        {
            var pricingUnits = new List<PricingUnit>();

            foreach (var price in prices)
            {
                var netPrice = price.Net.Amount / 100;
                var basePrice = price.Retail.Amount / 100;

                var gatebasePrice = price.original.Amount / 100;
                decimal finalpriceData = 0.0m;

                if(IsBasePrice == 1)
                {
                    finalpriceData = basePrice;
                }
                else if (IsBasePrice == 2)
                {
                    finalpriceData = netPrice;
                }
                else if (IsBasePrice == 3)
                {
                    finalpriceData = gatebasePrice;
                }

                var passengerType = GetPassengerType(price.TravelerType.AgeBand);
                var pricingUnit = CreatePricingUnit(passengerType, finalpriceData, criteria);
                if (pricingUnit == null) continue;
                if (!pricingUnits.Any(x => ((Isango.Entities.PerPersonPricingUnit)pricingUnit).PassengerType == ((Isango.Entities.PerPersonPricingUnit)x).PassengerType))
                {
                    pricingUnits.Add(pricingUnit);
                }
            }

            int infantCount = 0;
            try
            {
                infantCount = criteria.NoOfPassengers[PassengerType.Infant];
                if (!pricingUnits.Any(x => ((PerPersonPricingUnit)x).PassengerType == PassengerType.Infant))
                {
                    var pricingUnit = CreatePricingUnit(PassengerType.Infant, 0, criteria);
                    pricingUnits.Add(pricingUnit);
                }
            }
            catch (Exception ex)
            {
            }

            return pricingUnits;
        }
        private List<PricingUnit> CreatePricingUnitsSchedule(List<PricingScheduleResponse> prices,
            CanocalizationCriteria criteria, int IsBasePrice)
        {
            var pricingUnits = new List<PricingUnit>();

            foreach (var price in prices)
            {
                //currency match with database currency
                if (price.net.currency?.ToUpper() == criteria.Currency?.ToUpper()
                    || price.retail.currency?.ToUpper() == criteria.Currency?.ToUpper()
                    || price.original.currency?.ToUpper() == criteria.Currency?.ToUpper())
                {
                    var netPrice = price.net.amount / 100;
                    var basePrice = price.retail.amount / 100;

                    var passengerType = GetPassengerType(price.travelerType.ageBand);

                    var gatebasePrice = price.original.amount / 100;
                    decimal finalpriceData = 0.0m;

                    if (IsBasePrice == 1)
                    {
                        finalpriceData = basePrice;
                    }
                    else if (IsBasePrice == 2)
                    {
                        finalpriceData = netPrice;
                    }
                    else if (IsBasePrice == 3)
                    {
                        finalpriceData = gatebasePrice;
                    }

                    var pricingUnit = CreatePricingUnit(passengerType, finalpriceData, criteria);
                    if (pricingUnit == null) continue;
                    if (!pricingUnits.Any(x => ((Isango.Entities.PerPersonPricingUnit)pricingUnit).PassengerType == ((Isango.Entities.PerPersonPricingUnit)x).PassengerType))
                    {
                        pricingUnits.Add(pricingUnit);
                    }
                
            

            int infantCount = 0;
            try
            {
                infantCount = criteria.NoOfPassengers[PassengerType.Infant];
                if (!pricingUnits.Any(x => ((PerPersonPricingUnit)x).PassengerType == PassengerType.Infant))
                {
                     pricingUnit = CreatePricingUnit(PassengerType.Infant, 0, criteria);
                    pricingUnits.Add(pricingUnit);
                }
            }
            catch (Exception ex)
            {
            }

            return pricingUnits;
                }
            }
            return null;
        }
        /// <summary>
        /// Create Pricing Unit
        /// </summary>
        /// <param name="passengerType"></param>
        /// <param name="price"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private PricingUnit CreatePricingUnit(PassengerType passengerType, decimal price,
            CanocalizationCriteria criteria)
        {
            var paxCount = criteria.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(passengerType)).Value;
            if (paxCount <= 0) return null;
            var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
            pricingUnit.Price = price;

            //if (pricingUnit is PerPersonPricingUnit perPersonPricingUnit)
            //    perPersonPricingUnit.PassengerType = ageGroupId;
            //else
            //    return null;

            return pricingUnit;
        }

        /// <summary>
        /// Temporary method for PassengerType mapping
        /// </summary>
        /// <param name="paxType"></param>
        /// <returns></returns>
        private PassengerType GetPassengerType(string paxType)
        {
            var passengerType = PassengerType.Adult;
            switch (paxType)
            {
                case Constant.Adult:
                    passengerType = PassengerType.Adult;
                    break;

                case Constant.Child:
                    passengerType = PassengerType.Child;
                    break;

                case Constant.Infant:
                    passengerType = PassengerType.Infant;
                    break;

                case Constant.Student:
                    passengerType = PassengerType.Student;
                    break;

                case Constant.Youth:
                    passengerType = PassengerType.Youth;
                    break;

                case Constant.Senior:
                    passengerType = PassengerType.Senior;
                    break;

                    //case Constant.Any:
                    //    passengerType = PassengerType.TwoAndUnder;
                    //    break;

                    //case Constant.Unknown:
                    //    passengerType = PassengerType.Adult;
                    //    break;
            }
            return passengerType;
        }

        /// <summary>
        /// Checking valid pax criteria for RESERVED rate type
        /// </summary>
        /// <param name="availability"></param>
        /// <param name="selectedPax"></param>
        /// <param name="rateType"></param>
        /// <returns></returns>
        public bool CheckPaxForReserveType(Availability availability, int? selectedPax, string rateType) => availability.Capacity < selectedPax && rateType == Constant.ReservedType;

        #endregion Private Methods
    }
}