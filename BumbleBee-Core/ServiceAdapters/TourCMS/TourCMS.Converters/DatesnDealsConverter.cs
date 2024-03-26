using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.TourCMS;
using Isango.Entities.TourCMSCriteria;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.DatesnDealsResponse;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public class DatesnDealsConverter : ConverterBase, IDatesnDealsConverter
    {
        public DatesnDealsConverter(ILogger logger) : base(logger)
        {
        }
        object IConverterBase.Convert(object objectResult, object criteria)
        {
            return ConvertAvailablityResult(objectResult, criteria);
        }

        public List<Activity> ConvertAvailablityResult(object optionsFromAPI, object criteria)
        {
            var optionsObjFromAPI = optionsFromAPI as Dictionary<string, DatesnDealsResponse>;
            var datesnDealsResponseAPI = optionsObjFromAPI.Values.FirstOrDefault();
            var tourCMSCriteria = (TourCMSCriteria)criteria;
            var activities = new List<Activity>();
            var activity = new Activity();
            activity.ProductOptions = new List<ProductOption>();
            activity.Id = tourCMSCriteria.ProductId;
            activity.Code = datesnDealsResponseAPI?.TourId;
            activity.CurrencyIsoCode = datesnDealsResponseAPI?.DatesAndPrices?.DateList?.FirstOrDefault()?.SaleCurrency;
            var defaultDate = new DateTime(1970, 01, 01, 00, 00, 00);
            if (optionsObjFromAPI?.Count > 0)
            {
                //Bring out the api options against each Supplier option code in criteria
                foreach (var supplierCodeVsApiOptions in tourCMSCriteria.SupplierOptionCodesAndProductIdVsApiOptionIds)
                {
                    try
                    {
                        var isangoOptionId = System.Convert.ToInt32(supplierCodeVsApiOptions.Key.Split('_')[0]);
                        var supplierOptionCode = supplierCodeVsApiOptions.Key.Split('_')[1];
                        var supplierProductId = supplierCodeVsApiOptions.Key.Split('_')[1];

                        if (!string.IsNullOrEmpty(supplierOptionCode) && !string.IsNullOrEmpty(supplierProductId))
                        {
                            var apiOptionIdlist = supplierCodeVsApiOptions.Value;
                            if (apiOptionIdlist?.Count > 0)
                            {
                                var optionsForThisSupplierOptionCode = optionsObjFromAPI?.First(thisKeyValuePair => thisKeyValuePair.Key.Equals(supplierCodeVsApiOptions.Key)).Value;
                                if (optionsForThisSupplierOptionCode != null && optionsForThisSupplierOptionCode.DatesAndPrices.DateList.Count > 0)
                                {
                                    var selectedDateOption = optionsForThisSupplierOptionCode.DatesAndPrices.DateList.Find(thisOption => DateTime.Parse(thisOption.StartDate.ToString()).Date == tourCMSCriteria.CheckinDate);

                                    if (selectedDateOption != null && selectedDateOption.Status.ToLowerInvariant() != "open")
                                    {
                                        continue;
                                    }
                                    var key = tourCMSCriteria.NoOfPassengers.FirstOrDefault().Key;
                                    if (tourCMSCriteria.NoOfPassengers.ContainsKey(key))
                                    {
                                        tourCMSCriteria.NoOfPassengers[key] = System.Convert.ToInt32(selectedDateOption?.MinBookingSize) == 0 ? 1 : System.Convert.ToInt32(selectedDateOption?.MinBookingSize);
                                    }

                                    var listOfUniqueStartTime = new List<TimeSpan>();
                                    var checkTimeExistorNot = optionsForThisSupplierOptionCode?.DatesAndPrices?.DateList?.Select(x => x.StartTime).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(checkTimeExistorNot))
                                    {
                                        listOfUniqueStartTime = optionsForThisSupplierOptionCode?.DatesAndPrices?.DateList?
                                            .Select(thisOption => DateTime.Parse(string.IsNullOrEmpty(thisOption?.StartTime) ? "00:00" : thisOption?.StartTime).TimeOfDay).Distinct().ToList();
                                        if (listOfUniqueStartTime != null && listOfUniqueStartTime?.Count > 0)
                                        {
                                            foreach (var startTime in listOfUniqueStartTime)
                                            {
                                                var optionsForThisStartTime = optionsForThisSupplierOptionCode?.DatesAndPrices?.DateList?.FindAll(thisOption => DateTime.Parse(string.IsNullOrEmpty(thisOption?.StartTime) ? "00:00" : thisOption?.StartTime).TimeOfDay.Equals(startTime)).ToList();
                                                var activityOptionForThisStartTime = Createoption(optionsForThisStartTime, tourCMSCriteria, supplierOptionCode, supplierProductId, startTime, isangoOptionId);
                                                if (activityOptionForThisStartTime != null)
                                                {
                                                    activityOptionForThisStartTime.Code = optionsForThisStartTime?.FirstOrDefault()?.SupplierNote;
                                                    activityOptionForThisStartTime.SupplierOptionNote = optionsForThisStartTime?.FirstOrDefault()?.Note;
                                                    activity.ProductOptions.Add(activityOptionForThisStartTime);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var dateList = optionsForThisSupplierOptionCode?.DatesAndPrices?.DateList;
                                        var activityOptionForThisStartTime = Createoption(dateList, tourCMSCriteria, supplierOptionCode, supplierProductId, default(TimeSpan), isangoOptionId);
                                        if (activityOptionForThisStartTime != null)
                                        {
                                            activityOptionForThisStartTime.Code = dateList?.FirstOrDefault()?.SupplierNote;
                                            activityOptionForThisStartTime.SupplierOptionNote = dateList?.FirstOrDefault()?.Note;
                                            activity.ProductOptions.Add(activityOptionForThisStartTime);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(new IsangoErrorEntity
                        {
                            ClassName = nameof(DatesnDealsConverter),
                            MethodName = nameof(ConvertAvailablityResult),
                            Token = tourCMSCriteria.Token,
                            Params = "Tour CMS API response conversion Error"
                        }, ex);
                    }
                }
            }

            activities.Add(activity);
            return activities;

        }

        public ActivityOption Createoption(List<DateList> listOfAPIOptions,
            TourCMSCriteria tourCMSCriteria, string supplierOptionCode,
            string supplierProductId, TimeSpan startTime, int IsangoOptionId)
        {
            var activityOption = new ActivityOption();
            if (listOfAPIOptions != null && listOfAPIOptions.Count > 0)
            {
                activityOption.TourCMSProductId = supplierProductId;
                activityOption.Id = IsangoOptionId;
                activityOption.SupplierOptionCode = supplierOptionCode;

                var totalPaxCount = tourCMSCriteria.NoOfPassengers.Sum(x => x.Value);
                var getCapacity = listOfAPIOptions?.First()?.SpacesRemaining.ToUpper() == "UNLIMITED" ? 999 : int.Parse(listOfAPIOptions.First().SpacesRemaining);
                var needCapacityCheck = (getCapacity > 0) ? true : false;
                var optionAvailStatus = listOfAPIOptions.Any(thisOption => thisOption.Status.ToLowerInvariant() == "open");
                activityOption.AvailabilityStatus = optionAvailStatus && (needCapacityCheck ?
                    (System.Convert.ToInt32(getCapacity) >=
                    totalPaxCount) : true)
                    ? Isango.Entities.Enums.AvailabilityStatus.AVAILABLE :
                    Isango.Entities.Enums.AvailabilityStatus.NOTAVAILABLE;

                //Set capacity of option
                activityOption.Capacity = needCapacityCheck == false ? totalPaxCount :
                    (listOfAPIOptions.First().SpacesRemaining == null ? 0 :
                    System.Convert.ToInt32(listOfAPIOptions.First().SpacesRemaining));

                //Set Base price and Cost and GateBase price
                var getSaleCurrencyIsoCode = listOfAPIOptions?.First()?.SaleCurrency;
                var getSellPrice = System.Convert.ToDecimal(listOfAPIOptions?.First()?.Price1);

                var basePrice = new Price
                {
                    Amount = getSellPrice,
                    Currency = new Currency { IsoCode = getSaleCurrencyIsoCode },
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                };
                //For each option in listOfAPIOptions, we need to create DatePrice and Availability object and 
                //add it to the Price and Avail object of this activity option
                if (listOfAPIOptions != null && listOfAPIOptions.Count > 0)
                {
                    foreach (var apiOption in listOfAPIOptions)
                    {
                        //Create Price and avail for Cost, Gate, GateBase
                        var defaultPriceAndAvailForBase = new TourCMSPriceAndAvailability();
                        defaultPriceAndAvailForBase.PricingUnits = new List<PricingUnit>();

                        //Set availability
                        //defaultPriceAndAvailForCost.AvailabilityId = apiOption.StartDate;
                        defaultPriceAndAvailForBase.AvailabilityId = apiOption.StartDate;
                        //defaultPriceAndAvailForGateBase.AvailabilityId = apiOption.StartDate;

                        if (tourCMSCriteria?.NoOfPassengers != null)
                        {
                            foreach (var passengerInCriteria in tourCMSCriteria?.NoOfPassengers)
                            {
                                //Code block for getting those passengerTypes from criteria for which pricing unit needs to be created
                                var stringPassengerType = passengerInCriteria.Key;
                                var totalNoOfPassengers = tourCMSCriteria.NoOfPassengers.Where(thisPass => thisPass.Key != Isango.Entities.Enums.PassengerType.Infant).Sum(thisPass => thisPass.Value);

                                var spacesRemainingAPI = apiOption.SpacesRemaining.ToUpper() == "UNLIMITED" ? 999 : int.Parse(apiOption.SpacesRemaining);
                                var optionPriceAPI = System.Convert.ToDecimal(apiOption.Price1);
                                var activityOptionStatus = activityOption.AvailabilityStatus;
                                var minCapacityAPI = int.Parse(apiOption.MinBookingSize);

                                if (stringPassengerType != Isango.Entities.Enums.PassengerType.Infant)
                                {
                                    PricingUnit prcUnitForBase = GetPricingUnit(System.Convert.ToDecimal(optionPriceAPI),
                                        apiOption.SaleCurrency, stringPassengerType);
                                    if (prcUnitForBase != null)
                                    {
                                        //prcUnitForBase.Mincapacity = minCapacityAPI;
                                        prcUnitForBase.TotalCapacity = spacesRemainingAPI;
                                        prcUnitForBase.UnitType = UnitType.PerPerson;
                                        prcUnitForBase.PriceType = PriceType.PerPerson;
                                        defaultPriceAndAvailForBase.AvailabilityStatus = activityOptionStatus;
                                        defaultPriceAndAvailForBase.PricingUnits.Add(prcUnitForBase);
                                    }
                                }

                            }
                        }
                        var optionApiPrice = System.Convert.ToDecimal(apiOption.Price1);
                        var optionApiStartDate = DateTime.Parse(apiOption.StartDate);
                        defaultPriceAndAvailForBase.TotalPrice = optionApiPrice;
                        basePrice.DatePriceAndAvailabilty.Add(optionApiStartDate, defaultPriceAndAvailForBase);

                    }
                }
                if (startTime != default(TimeSpan))
                    activityOption.StartTime = startTime;

                activityOption.BasePrice = basePrice.DeepCopy();
                activityOption.BasePrice.Amount = basePrice.Amount;

                activityOption.GateBasePrice = basePrice.DeepCopy();

                activityOption.PrefixServiceCode = supplierProductId;
                activityOption.CommisionPercent = tourCMSCriteria.IsCommissionPercent == true ? tourCMSCriteria.CommissionPercent : 0;
                //Set travel info in option
                activityOption.TravelInfo = new TravelInfo
                {
                    NoOfPassengers = tourCMSCriteria.NoOfPassengers,
                    StartDate = (tourCMSCriteria.CheckinDate != null)
                                    ? new DateTime(tourCMSCriteria.CheckinDate.Year, tourCMSCriteria.CheckinDate.Month, tourCMSCriteria.CheckinDate.Day)
                                        : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                };

            }
            return activityOption;
        }

        private PricingUnit GetPricingUnit(decimal unitPriceFromApi, string currencyFromApi, Isango.Entities.Enums.PassengerType passengerTypeIsango)
        {
            PricingUnit prcUnit = PricingUnitFactory.GetPricingUnit(passengerTypeIsango);
            if (prcUnit != null)
            {
                prcUnit.Price = System.Convert.ToDecimal(unitPriceFromApi);
                prcUnit.Currency = currencyFromApi;
            }
            return prcUnit;
        }


        object IConverterBase.Convert(object apiResponse, Entities.MethodType methodType, object criteria)
        {
            throw new NotImplementedException();
        }
        public override object Convert<T>(string response, T request)
        {
            throw new NotImplementedException();
        }
        public override object Convert(object objectResult, object criteria)
        {
            throw new NotImplementedException();
        }
        List<Activity> IDatesnDealsConverter.ConvertAvailablityResult(object listOfOptionsFromAPI, object criteria)
        {
            throw new NotImplementedException();
        }
    }
}