using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Ventrata;
using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Converters.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;

namespace ServiceAdapters.Ventrata.Ventrata.Converters
{
    public class AvailabilityConverter : ConverterBase, IAvailabilityConverter
    {
        public AvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert(object objectResult, object criteria)
        {
            return ConvertAvailablityResult(objectResult, criteria);
        }

        public override object Convert(object objectResponse)
        {
            throw new NotImplementedException();
        }

        public List<Activity> ConvertAvailablityResult(object optionsFromAPI, object criteria)
        {
            var optionsObjFromAPI = optionsFromAPI as Dictionary<string, List<AvailabilityRes>>;
            var ventrataCriteria = (VentrataAvailabilityCriteria)criteria;
            var activities = new List<Activity>();
            var activity = new Activity();
            activity.ProductOptions = new List<ProductOption>();
            activity.Id = ventrataCriteria.ProductId;
            var defaultDate = new DateTime(1970, 01, 01, 00, 00, 00);

            if (optionsObjFromAPI?.Count > 0)
            {
                //Bring out the api options against each Supplier option code in criteria
                foreach (var supplierCodeVsApiOptions in ventrataCriteria.SupplierOptionCodesAndProductIdVsApiOptionIds)
                {
                    try
                    {
                        var isangoOptionId = System.Convert.ToInt32(supplierCodeVsApiOptions.Key.Split('*')[0]);
                        var supplierOptionCode = supplierCodeVsApiOptions.Key.Split('*')[1];
                        var supplierProductId = supplierCodeVsApiOptions.Key.Split('*')[2];

                        if (!string.IsNullOrEmpty(supplierOptionCode) && !string.IsNullOrEmpty(supplierProductId))
                        {
                            var apiOptionIdlist = supplierCodeVsApiOptions.Value;
                            if (apiOptionIdlist?.Count > 0)
                            {
                                var optionsForThisSupplierOptionCode = optionsObjFromAPI?.First(thisKeyValuePair => thisKeyValuePair.Key.Equals(supplierCodeVsApiOptions.Key)).Value;
                                if (optionsForThisSupplierOptionCode != null && optionsForThisSupplierOptionCode.Count > 0)
                                {
                                    var selectedDateOption = optionsForThisSupplierOptionCode.Find(thisOption => DateTimeOffset.Parse(thisOption.LocalDateTimeStart.ToString(), null, DateTimeStyles.RoundtripKind).Date >= ventrataCriteria.CheckinDate && thisOption.Available);
                                    if (selectedDateOption == null)
                                    {
                                        continue;
                                    }

                                    var isAllDayActivity = !optionsForThisSupplierOptionCode?.Any(x => x?.AllDay == false) ?? false;
                                    //Check for all day activity and create options accordingly
                                    if (isAllDayActivity == false)
                                    {
                                        //var optionslistStartTimeVsDates
                                        var listOfUniqueStartTime = new List<TimeSpan>();
                                        listOfUniqueStartTime = optionsForThisSupplierOptionCode?.Select(thisOption => DateTimeOffset.Parse(thisOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).DateTime.TimeOfDay).Distinct().ToList();

                                        var uniqueTimePairs = optionsForThisSupplierOptionCode?
                                                                .Select(thisOption => new
                                                                {
                                                                    StartTime = DateTimeOffset.Parse(thisOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).DateTime.TimeOfDay,
                                                                    EndTime = DateTimeOffset.Parse(thisOption.LocalDateTimeEnd, null, DateTimeStyles.RoundtripKind).DateTime.TimeOfDay
                                                                })
                                                                .Distinct();
                                        //var uniqueTimePairs= uniqueTimePairsData.ToDictionary(timePair => timePair.StartTime, timePair => timePair.EndTime);

                                        //var dictOfStartTimeAndAPIOptions = new Dictionary<TimeSpan, List<AvailabilityRes>>();

                                        //foreach (var startTime in listOfUniqueStartTime)
                                        //{
                                        //    //DateTime timeStart = DateTime.Parse(apiOption.LocalDateTimeStart, null, System.Globalization.DateTimeStyles.RoundtripKind)
                                        //    var optionsForThisStartTime = optionsForThisSupplierOptionCode.FindAll(thisOption => DateTimeOffset.Parse(thisOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).DateTime.TimeOfDay.Equals(startTime)).ToList();
                                        //    //dictOfStartTimeAndAPIOptions.Add(startTime, optionsForThisStartTime);
                                        //    var activityOptionForThisStartTime = Createoption(optionsForThisStartTime, ventrataCriteria, supplierOptionCode, supplierProductId, startTime, isangoOptionId, ventrataCriteria.SupplierBearerToken,ventrataCriteria.VentrataBaseURL);
                                        //    if (activityOptionForThisStartTime != null)
                                        //        activity.ProductOptions.Add(activityOptionForThisStartTime);
                                        //}
                                        foreach (var timePair in uniqueTimePairs)
                                        {
                                            var startTime = timePair.StartTime;
                                            var endTime = timePair.EndTime;
                                            var optionsForThisTimeRange = optionsForThisSupplierOptionCode
                                                                             .FindAll(thisOption =>
                                                                                 DateTimeOffset.Parse(thisOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).DateTime.TimeOfDay.Equals(startTime) &&
                                                                                 DateTimeOffset.Parse(thisOption.LocalDateTimeEnd, null, DateTimeStyles.RoundtripKind).DateTime.TimeOfDay.Equals(endTime))
                                                                             .ToList();

                                            if (optionsForThisTimeRange.Count > 0)
                                            {
                                                // Create and add the corresponding activityOption
                                                var activityOptionForThisTimeRange = Createoption(optionsForThisTimeRange, ventrataCriteria, supplierOptionCode, supplierProductId, startTime, endTime, isangoOptionId, ventrataCriteria.SupplierBearerToken, ventrataCriteria.VentrataBaseURL);
                                                if (activityOptionForThisTimeRange != null)
                                                {
                                                    activity.ProductOptions.Add(activityOptionForThisTimeRange);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var activityOptionAllDAy = Createoption(optionsForThisSupplierOptionCode, ventrataCriteria, supplierOptionCode, supplierProductId, default(TimeSpan), default(TimeSpan), isangoOptionId, ventrataCriteria.SupplierBearerToken, ventrataCriteria.VentrataBaseURL);
                                        if (activityOptionAllDAy != null)
                                            activity.ProductOptions.Add(activityOptionAllDAy);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(new IsangoErrorEntity
                        {
                            ClassName = nameof(AvailabilityConverter),
                            MethodName = nameof(ConvertAvailablityResult),
                            Token = ventrataCriteria.Token,
                            Params = "Ventrata API response conversion Error"
                        }, ex);
                    }
                }
            }

            activities.Add(activity);
            return activities;
        }

        public ActivityOption Createoption(List<AvailabilityRes> listOfAPIOptions,
            VentrataAvailabilityCriteria ventrataCriteria,
            string supplierOptionCode, string supplierProductId,
            TimeSpan startTime, TimeSpan endTime, int IsangoOptionId, string ventrataSupplierId, string ventrataBaseURL)
        {
            var activityOption = new ActivityOption();
            if (listOfAPIOptions != null && listOfAPIOptions.Count > 0)
            {
                activityOption.VentrataProductId = supplierProductId;
                activityOption.VentrataSupplierId = ventrataSupplierId;
                activityOption.VentrataBaseURL = ventrataBaseURL;

                activityOption.Id = IsangoOptionId;
                //Todo Availability Id from API now goes to Ventrata P&A level because each date with its startTime has its own Availability Id. Thats why commenting it.
                //activityOption.RateKey = apiOption.Id;
                activityOption.SupplierOptionCode = supplierOptionCode;

                var totalPaxCount = ventrataCriteria.NoOfPassengers.Sum(x => x.Value);
                var needCapacityCheck = (listOfAPIOptions.Any(x => x.Capacity == null) && listOfAPIOptions.Any(x => x.MaxUnits == null)) ? false : true;
                var optionAvailStatus = listOfAPIOptions.Any(thisOption => thisOption.Available == true);
                activityOption.AvailabilityStatus = optionAvailStatus && (needCapacityCheck ? (listOfAPIOptions.Any(x => System.Convert.ToInt32(x.Capacity) >= totalPaxCount) || listOfAPIOptions.Any(x => System.Convert.ToInt32(x.MaxUnits) >= totalPaxCount)) : true) ? Isango.Entities.Enums.AvailabilityStatus.AVAILABLE : Isango.Entities.Enums.AvailabilityStatus.NOTAVAILABLE;

                //Set capacity of option
                try
                {
                    if (needCapacityCheck)
                    {
                        var capacities = listOfAPIOptions.Where(y => y.Capacity != null)?.ToList()?.Select(x => System.Convert.ToInt32(x.Capacity))?.ToList();
                        activityOption.Capacity = capacities.Max();
                    }
                    else
                    {
                        activityOption.Capacity = totalPaxCount;
                    }
                }
                catch (Exception ex)
                {
                    activityOption.Capacity = needCapacityCheck == false ? totalPaxCount : (listOfAPIOptions.First().Capacity == null ? System.Convert.ToInt32(listOfAPIOptions.First().MaxUnits) : System.Convert.ToInt32(listOfAPIOptions.First().Capacity));
                }

                //Set Base price and Cost and GateBase price
                var costPrice = new Price
                {
                    Amount = 0,
                    Currency = new Currency(),
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                };
                var basePrice = new Price
                {
                    Amount = 0,
                    Currency = new Currency(),
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                };
                var gateBasePrice = new Price
                {
                    Amount = 0,
                    Currency = new Currency(),
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                };

                //For each option in listOfAPIOptions, we need to create DatePrice and Availability object and add it to the Price and Avail object of this activity option
                foreach (var apiOption in listOfAPIOptions)
                {
                    var pricingUnitPricingFromAPI = apiOption?.UnitPricing;
                    var pricingTotalFromAPI = apiOption?.Pricing;
                    //Create Price and avail for Cost, Gate, GateBase
                    var defaultPriceAndAvailForCost = new VentrataPriceAndAvailability();
                    var defaultPriceAndAvailForBase = new VentrataPriceAndAvailability();
                    var defaultPriceAndAvailForGateBase = new VentrataPriceAndAvailability();

                    defaultPriceAndAvailForCost.PricingUnits = new List<PricingUnit>();
                    defaultPriceAndAvailForBase.PricingUnits = new List<PricingUnit>();
                    defaultPriceAndAvailForGateBase.PricingUnits = new List<PricingUnit>();
                    //Set availability
                    defaultPriceAndAvailForCost.AvailabilityId = apiOption.Id;
                    defaultPriceAndAvailForBase.AvailabilityId = apiOption.Id;
                    defaultPriceAndAvailForGateBase.AvailabilityId = apiOption.Id;

                    //Check and create pricing units of those pax which are in the criteria
                    //TODO - Create Base Price, Cost Price and GateBase Price
                    decimal totalPriceNet = 0;
                    decimal totalPriceRetail = 0;
                    decimal totalPriceOriginal = 0;
                    if (apiOption.UnitPricing != null && apiOption.UnitPricing.Count() > 0)
                    {
                        foreach (var unitPricing in apiOption.UnitPricing)
                        {
                            //Code block for getting those passengerTypes from criteria for which pricing unit needs to be created
                            var passengerTypesInCriteria = ventrataCriteria.NoOfPassengers.Where(thisPass => thisPass.Value > 0).Select(thispass => thispass.Key).Distinct().ToList();
                            var stringPassengerType = unitPricing.UnitId;

                            if (stringPassengerType != null)
                            {
                                var passengerTypeIsango = ventrataCriteria.VentrataPaxMappings?.Where(x => x.AgeGroupCode == stringPassengerType)?.FirstOrDefault()?.PassengerType ?? PassengerType.Undefined;
                                //bool isPaxTypeInIsango = ConstantsForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(stringPassengerType, out Isango.Entities.Enums.PassengerType passengerTypeIsango);
                                if (passengerTypeIsango != null && passengerTypesInCriteria.Contains(passengerTypeIsango))
                                {
                                    PricingUnit prcUnitForCost = GetPricingUnit(unitPricing.Net, unitPricing.Currency, passengerTypeIsango, unitPricing.CurrencyPrecision);
                                    int? countNet = ventrataCriteria.NoOfPassengers?.Where(x => x.Key == passengerTypeIsango)?.FirstOrDefault().Value;
                                    if (countNet != null)
                                    {
                                        totalPriceNet = totalPriceNet + (System.Convert.ToInt32(countNet) * unitPricing.Net);
                                    }
                                    if (prcUnitForCost != null)
                                    {
                                        try
                                        {
                                            //TODO Capacity at pricing unit level
                                            defaultPriceAndAvailForCost.AvailabilityStatus = apiOption.Available ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                            defaultPriceAndAvailForCost.Capacity = apiOption.Capacity != null ? System.Convert.ToInt32(apiOption.Capacity) : 0;
                                            defaultPriceAndAvailForCost.PricingUnits.Add(prcUnitForCost);
                                        }
                                        catch (Exception ex)
                                        {
                                            //ignore
                                        }
                                    }

                                    PricingUnit prcUnitForBase = GetPricingUnit(unitPricing.Retail, unitPricing.Currency, passengerTypeIsango, unitPricing.CurrencyPrecision);

                                    int? countRetail = ventrataCriteria.NoOfPassengers?.Where(x => x.Key == passengerTypeIsango)?.FirstOrDefault().Value;
                                    if (countRetail != null)
                                    {
                                        totalPriceRetail = totalPriceRetail + (System.Convert.ToInt32(countRetail) * unitPricing.Retail);
                                    }
                                    if (prcUnitForBase != null)
                                    {
                                        try
                                        {
                                            //TODO Capacity at pricing unit level
                                            defaultPriceAndAvailForBase.AvailabilityStatus = apiOption.Available ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                            defaultPriceAndAvailForBase.Capacity = apiOption.Capacity != null ? System.Convert.ToInt32(apiOption.Capacity) : 0;
                                            defaultPriceAndAvailForBase.PricingUnits.Add(prcUnitForBase);
                                        }
                                        catch (Exception ex)
                                        {
                                            //ignore
                                        }
                                    }

                                    PricingUnit prcUnitForGateBase = GetPricingUnit(unitPricing.Original, unitPricing.Currency, passengerTypeIsango, unitPricing.CurrencyPrecision);

                                    int? countOriginal = ventrataCriteria.NoOfPassengers?.Where(x => x.Key == passengerTypeIsango)?.FirstOrDefault().Value;
                                    if (countOriginal != null)
                                    {
                                        totalPriceOriginal = totalPriceOriginal + (System.Convert.ToInt32(countOriginal) * unitPricing.Original);
                                    }
                                    if (prcUnitForGateBase != null)
                                    {
                                        try
                                        {
                                            //TODO Capacity at pricing unit level
                                            defaultPriceAndAvailForGateBase.AvailabilityStatus = apiOption.Available ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                            defaultPriceAndAvailForGateBase.Capacity = apiOption.Capacity != null ? System.Convert.ToInt32(apiOption.Capacity) : 0;
                                            defaultPriceAndAvailForGateBase.PricingUnits.Add(prcUnitForGateBase);
                                        }
                                        catch (Exception ex)
                                        {
                                            //ignore
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var passengerInCriteria in ventrataCriteria.NoOfPassengers)
                        {
                            //Code block for getting those passengerTypes from criteria for which pricing unit needs to be created
                            var stringPassengerType = passengerInCriteria.Key;
                            var totalNoOfPassengers = ventrataCriteria.NoOfPassengers.Where(thisPass => thisPass.Key != Isango.Entities.Enums.PassengerType.Infant).Sum(thisPass => thisPass.Value);
                            if (stringPassengerType != Isango.Entities.Enums.PassengerType.Infant)
                            {
                                PricingUnit prcUnitForCost = GetPricingUnit(apiOption.Pricing.Net / totalNoOfPassengers, apiOption.Pricing.Currency, stringPassengerType, apiOption.Pricing.CurrencyPrecision);
                                if (prcUnitForCost != null)
                                {
                                    //TODO Capacity at pricing unit level
                                    prcUnitForCost.UnitType = UnitType.PerUnit;
                                    prcUnitForCost.PriceType = PriceType.PerUnit;
                                    defaultPriceAndAvailForCost.AvailabilityStatus = apiOption.Available ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                    defaultPriceAndAvailForCost.Capacity = apiOption.Capacity != null ? System.Convert.ToInt32(apiOption.Capacity) : 0;
                                    defaultPriceAndAvailForCost.PricingUnits.Add(prcUnitForCost);
                                }

                                PricingUnit prcUnitForBase = GetPricingUnit(apiOption.Pricing.Retail / totalNoOfPassengers, apiOption.Pricing.Currency, stringPassengerType, apiOption.Pricing.CurrencyPrecision);
                                if (prcUnitForBase != null)
                                {
                                    //TODO Capacity at pricing unit level
                                    prcUnitForBase.UnitType = UnitType.PerUnit;
                                    prcUnitForBase.PriceType = PriceType.PerUnit;
                                    defaultPriceAndAvailForBase.AvailabilityStatus = apiOption.Available ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                    defaultPriceAndAvailForBase.Capacity = apiOption.Capacity != null ? System.Convert.ToInt32(apiOption.Capacity) : 0;
                                    defaultPriceAndAvailForBase.PricingUnits.Add(prcUnitForBase);
                                }

                                PricingUnit prcUnitForGateBase = GetPricingUnit(apiOption.Pricing.Original / totalNoOfPassengers, apiOption.Pricing.Currency, stringPassengerType, apiOption.Pricing.CurrencyPrecision);
                                if (prcUnitForGateBase != null)
                                {
                                    //TODO Capacity at pricing unit level
                                    prcUnitForGateBase.UnitType = UnitType.PerUnit;
                                    prcUnitForGateBase.PriceType = PriceType.PerUnit;
                                    defaultPriceAndAvailForGateBase.AvailabilityStatus = apiOption.Available ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                    defaultPriceAndAvailForGateBase.Capacity = apiOption.Capacity != null ? System.Convert.ToInt32(apiOption.Capacity) : 0;
                                    defaultPriceAndAvailForGateBase.PricingUnits.Add(prcUnitForGateBase);
                                }
                            }

                        }
                    }
                    var currencyPrecision = 2;
                    //pricingUnitPricingFromAPI only
                    if (pricingTotalFromAPI == null &&
                        pricingUnitPricingFromAPI != null &&
                        pricingUnitPricingFromAPI.Length > 0)
                    {
                        currencyPrecision = pricingUnitPricingFromAPI.First().CurrencyPrecision;

                        var netGet = (System.Convert.ToDouble(totalPriceNet) / Math.Pow(10, currencyPrecision));
                        var retailGet = (System.Convert.ToDouble(totalPriceRetail) / Math.Pow(10, currencyPrecision));
                        var originalGet = (System.Convert.ToDouble(totalPriceOriginal) / Math.Pow(10, currencyPrecision));

                        costPrice.Amount = System.Convert.ToDecimal(netGet);
                        basePrice.Amount = System.Convert.ToDecimal(retailGet);
                        gateBasePrice.Amount = System.Convert.ToDecimal(originalGet);

                        totalPriceNet = System.Convert.ToDecimal(netGet);
                        totalPriceRetail = System.Convert.ToDecimal(retailGet);
                        totalPriceOriginal = System.Convert.ToDecimal(originalGet);

                        var currencyGet = pricingUnitPricingFromAPI?.FirstOrDefault()?.Currency;

                        costPrice.Currency = new Currency { IsoCode = currencyGet };
                        basePrice.Currency = new Currency { IsoCode = currencyGet };
                        gateBasePrice.Currency = new Currency { IsoCode = currencyGet };
                    }
                    else//pricingTotalFromAPI only
                    {
                        currencyPrecision = pricingTotalFromAPI.CurrencyPrecision;

                        var netGet = (pricingTotalFromAPI.Net / Math.Pow(10, currencyPrecision));
                        var retailGet = (pricingTotalFromAPI.Retail / Math.Pow(10, currencyPrecision));
                        var originalGet = (pricingTotalFromAPI.Original / Math.Pow(10, currencyPrecision));

                        costPrice.Amount = System.Convert.ToDecimal(netGet);
                        basePrice.Amount = System.Convert.ToDecimal(retailGet);
                        gateBasePrice.Amount = System.Convert.ToDecimal(originalGet);

                        totalPriceNet = System.Convert.ToDecimal(netGet);
                        totalPriceRetail = System.Convert.ToDecimal(retailGet);
                        totalPriceOriginal = System.Convert.ToDecimal(originalGet);

                        var currencyGet = pricingTotalFromAPI?.Currency;
                        costPrice.Currency = new Currency { IsoCode = currencyGet };
                        basePrice.Currency = new Currency { IsoCode = currencyGet };
                        gateBasePrice.Currency = new Currency { IsoCode = currencyGet };
                    }

                    //TOChange/TODO - change done temporarily to get price in said currency as they send values in cents. Enhancement will be created for this and will be taken care of in that task.
                    defaultPriceAndAvailForCost.TotalPrice = System.Convert.ToDecimal(totalPriceNet);
                    defaultPriceAndAvailForBase.TotalPrice = System.Convert.ToDecimal(totalPriceRetail);
                    defaultPriceAndAvailForGateBase.TotalPrice = System.Convert.ToDecimal(totalPriceOriginal);

                    costPrice.DatePriceAndAvailabilty.Add(DateTimeOffset.Parse(apiOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).Date, defaultPriceAndAvailForCost);
                    basePrice.DatePriceAndAvailabilty.Add(DateTimeOffset.Parse(apiOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).Date, defaultPriceAndAvailForBase);
                    gateBasePrice.DatePriceAndAvailabilty.Add(DateTimeOffset.Parse(apiOption.LocalDateTimeStart, null, DateTimeStyles.RoundtripKind).Date, defaultPriceAndAvailForGateBase);
                }

                if (startTime != default(TimeSpan))
                    activityOption.StartTime = startTime;
                if (endTime != default(TimeSpan))
                    activityOption.EndTime = endTime;

                //Set Price - GateBase-Original, Base-Retail, Cost-Net
                activityOption.CostPrice = costPrice.DeepCopy();
                activityOption.CostPrice.Amount = costPrice.Amount;

                activityOption.GateBasePrice = gateBasePrice.DeepCopy();
                activityOption.GateBasePrice.Amount = gateBasePrice.Amount;
                if (ventrataCriteria.IsSupplementOffer == false)
                {
                    activityOption.BasePrice = gateBasePrice.DeepCopy();
                    activityOption.BasePrice.Amount = gateBasePrice.Amount;
                }
                else
                {
                    activityOption.BasePrice = basePrice.DeepCopy();
                    activityOption.BasePrice.Amount = basePrice.Amount;
                }

                activityOption.PrefixServiceCode = supplierProductId;

                //Set travel info in option
                activityOption.TravelInfo = new TravelInfo
                {
                    NoOfPassengers = ventrataCriteria.NoOfPassengers,
                    StartDate = (ventrataCriteria.CheckinDate != null)
                                    ? new DateTime(ventrataCriteria.CheckinDate.Year, ventrataCriteria.CheckinDate.Month, ventrataCriteria.CheckinDate.Day)
                                        : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                };

                if (listOfAPIOptions.First().PickupAvailable && listOfAPIOptions.First().PickupRequired)
                {
                    var randomIntegerGenrator = new Random();
                    activityOption.PickupPointsDetailsForVentrata = new List<PickupPointsDetailsForVentrata>();
                    foreach (var pickupPoint in listOfAPIOptions?.First()?.PickupPoints)
                    {
                        try
                        {
                            var singlePickUpPoint = new PickupPointsDetailsForVentrata();
                            singlePickUpPoint.RandomIntegerId = randomIntegerGenrator.Next();
                            singlePickUpPoint.PickupPointAddress = "Time : " + DateTimeOffset.Parse(pickupPoint.LocalDateTime.ToString(), null, DateTimeStyles.RoundtripKind).TimeOfDay + " ," + pickupPoint.Name + (pickupPoint.Street != null ? ("," + pickupPoint.Street) : "") + (pickupPoint.Locality != null ? ("," + pickupPoint.Locality) : "")
                                                + (pickupPoint.Region != null ? ("," + pickupPoint.Region) : "") + (pickupPoint.State != null ? ("," + pickupPoint.State) : "") +
                                                (pickupPoint.Country != null ? ("," + pickupPoint.Country) : "") + (pickupPoint.PostalCode != null ? ("," + pickupPoint.PostalCode) : "");
                            singlePickUpPoint.Id = pickupPoint.Id;
                            singlePickUpPoint.LocalDateTime = pickupPoint.LocalDateTime;
                            singlePickUpPoint.GooglePlaceId = pickupPoint.GooglePlaceId;

                            activityOption.PickupPointsDetailsForVentrata.Add(singlePickUpPoint);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(new IsangoErrorEntity
                            {
                                ClassName = nameof(AvailabilityConverter),
                                MethodName = nameof(Createoption),
                                Token = ventrataCriteria.Token,
                                Params = "Ventrata API response conversion Error"
                            }, ex);
                        }
                    }
                }

                if (listOfAPIOptions?.First()?.MeetingPoint != null)
                {
                    var firstOption = listOfAPIOptions?.First();
                    activityOption.MeetingPointDetails = new MeetingPointDetails
                    {
                        MeetingPointAddresses = firstOption.MeetingPoint,
                        TimeAndDates = System.Convert.ToDateTime(firstOption.MeetingLocalDateTime) != DateTime.MinValue ? DateTimeOffset.Parse(firstOption.MeetingLocalDateTime.ToString(), null, DateTimeStyles.RoundtripKind).DateTime.ToString() : string.Empty,
                        //TODiscuss - No meeting point coordinates as of now in any activity
                        MeetingPointCoordinates = firstOption.MeetingPointCoordinates != null ? firstOption.MeetingPointCoordinates.ToString() : null
                    };
                }

                if (listOfAPIOptions?.First()?.OpeningHours != null && listOfAPIOptions?.First()?.OpeningHours?.Count() > 0)
                {
                    activityOption.OpeningHoursDetails = new List<OpeningHours>();

                    listOfAPIOptions.ForEach(thisOption =>
                    {
                        thisOption.OpeningHours?.ToList().ForEach(openingHr =>
                        {
                            var openingHourObj = new OpeningHours
                            {
                                Date = thisOption.Id,
                                From = openingHr.From,
                                To = openingHr.To
                            };

                            activityOption.OpeningHoursDetails.Add(openingHourObj);
                        });
                    });
                }

                //ToDiscuss- Offer code and offer Title
                activityOption.OfferCode = listOfAPIOptions?.First()?.OfferCode?.ToString();
                activityOption.OfferTitle = listOfAPIOptions?.First()?.OfferTitle?.ToString();
            }
            return activityOption;
        }

        private PricingUnit GetPricingUnit(int unitPriceFromApi, string currencyFromApi, Isango.Entities.Enums.PassengerType passengerTypeIsango, int currencyPrecision)
        {
            PricingUnit prcUnit = PricingUnitFactory.GetPricingUnit(passengerTypeIsango);
            if (prcUnit != null)
            {
                //TOChange/TODO - change done temporarily to get price in said currency as they send values in cents. Enhancement will be created for this and will be taken care of in that task.
                prcUnit.Price = System.Convert.ToDecimal(unitPriceFromApi / Math.Pow(10, currencyPrecision));
                prcUnit.Currency = currencyFromApi;
            }
            if (passengerTypeIsango != PassengerType.Infant && passengerTypeIsango != PassengerType.Child && prcUnit.Price == 0)
            {
                //TOChange/TODO - change done temporarily to get price in said currency as they send values in cents. Enhancement will be created for this and will be taken care of in that task.
                prcUnit = null;
            }

            return prcUnit;
        }
    }
}