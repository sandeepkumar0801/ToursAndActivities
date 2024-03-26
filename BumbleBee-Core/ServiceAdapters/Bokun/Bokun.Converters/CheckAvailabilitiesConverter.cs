using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.CheckAvailabilities;
using ServiceAdapters.Bokun.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public class CheckAvailabilitiesConverter : ConverterBase, ICheckAvailabilitiesConverter
    {
        private bool _isAppendBokunAPIDescription = false;
        private BokunAPIConfig _apiConfig = null;
        private int _optionCapacity;

        public CheckAvailabilitiesConverter(ILogger logger) : base(logger)
        {
            _apiConfig = BokunAPIConfig.GetInstance();
            _isAppendBokunAPIDescription = _apiConfig.IsAppendBokunAPIDescription;
            try
            {
                int.TryParse(ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"), out int tempInt);
                _optionCapacity = tempInt;
            }
            catch
            {
                _optionCapacity = 0;
            }
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Get Availabilities Call</param>
        /// <param name="criteria">Generic request model</param>
        /// <returns></returns>
        public object Convert<T>(T objectResult, T criteria)
        {
            var result = (List<CheckAvailabilitiesRs>)(object)objectResult;
            if (result != null)
            {
                var availabilityList = ConvertToActivitiesResult(result, criteria as BokunCriteria);
                return availabilityList;
            }
            else
            {
                return null;
            }
        }

        public object Convert<T>(object inputContext, MethodType methodType, T inputRequest, Isango.Entities.Activities.Activity activity)
        {
            var result = (List<CheckAvailabilitiesRs>)inputContext;
            if (result != null)
            {
                var availabilityList = ConvertToActivitiesResult(result, inputRequest as BokunCriteria, activity);
                return availabilityList;
            }
            else
            {
                return null;
            }
        }

        private List<Activity> ConvertToActivitiesResult(List<CheckAvailabilitiesRs> result, BokunCriteria criteria, Isango.Entities.Activities.Activity isangoActvivity = null)
        {
            var activities = new List<Activity>();
            var activity = new Activity();
            var options = new List<ActivityOption>();

            var queryCurrency = from act in result
                                from r in act?.PricesByRate
                                    //from ppc in r?.PricePerCategoryUnit
                                select r?.PricePerCategoryUnit?.FirstOrDefault()?.Amount?.Currency
                                        ?? r?.PricePerBooking?.Currency;

            var currency = queryCurrency?.FirstOrDefault();

            activity.ApiType = APIType.Bokun;
            activity.ID = criteria.ActivityId;
            activity.CurrencyIsoCode = criteria.CurrencyIsoCode?.ToLower() == currency?.ToLower() ? currency : string.Empty;

            var serviceOptionsForIsango = result.Select(ser => ser.ActivityId).Distinct().ToList();
            if (serviceOptionsForIsango.Count > 0)
            {
                foreach (var serviceOptionCode in serviceOptionsForIsango)
                {
                    var items = result.Where(item => item.ActivityId == serviceOptionCode).ToList();
                    var itemsOnDifferentStartTime = items.GroupBy(x => x.StartTimeId).OrderBy(y => y.Key).ToList();
                    foreach (var startTime in itemsOnDifferentStartTime)
                    {
                        var itemsByTime = items.Where(x => x.StartTimeId == startTime.Key).ToList();
                        if (items?.Count > 0)
                        {
                            var queryRateId = from act in itemsByTime
                                              from r in act?.PricesByRate
                                              select r?.ActivityRateId;

                            var activityRateIds = queryRateId?.Distinct()?.ToList();
                            if (activityRateIds?.Count > 0)
                            {
                                foreach (var rateId in activityRateIds)
                                {
                                    try
                                    {
                                        var queryRate = from act in itemsByTime
                                                        from r in act?.Rates
                                                        where r?.Id == rateId
                                                        select r;

                                        var rate = queryRate?.Distinct()?.FirstOrDefault();

                                        var chargablePaxTypeTotalCount = criteria?.NoOfPassengers?.Where(x => x.Key != PassengerType.Infant)?.Sum(y => y.Value) ?? 0;

                                        if (rate?.MinPerBooking != null && rate?.MinPerBooking != 0)
                                        {
                                            if (rate?.MinPerBooking > 0 && chargablePaxTypeTotalCount < rate?.MinPerBooking)
                                            {
                                                continue;
                                            }
                                        }

                                        if (rate?.MaxPerBooking != null && rate?.MaxPerBooking != 0)
                                        {
                                            if (rate?.MaxPerBooking > 0 && chargablePaxTypeTotalCount > rate?.MaxPerBooking)
                                            {
                                                continue;
                                            }
                                        }

                                        //var checkForSingleMultiplePax = criteria.AllPriceCategoryIdMapping?.GroupBy(x => x.PassengerTypeId)?.ToList();

                                        //if (criteria.AllPriceCategoryIdMapping?.Count > 1 && checkForSingleMultiplePax?.Count == 1) // this means this profuct has only 1 pax type which has different rates - Option creation should be done on Pax Level - Abhishek Malik
                                        //{
                                        //    foreach (var priceCatId in criteria.PriceCategoryIdMapping)
                                        //    {
                                        //        var tempCriteria = new BokunCriteria
                                        //        {
                                        //            ActivityCode = criteria.ActivityCode,
                                        //            FactSheetIds = criteria.FactSheetIds,
                                        //            ActivityId = criteria.ActivityId,
                                        //            ActivityMargin = criteria.ActivityMargin,
                                        //            Ages = criteria.Ages,
                                        //            CheckinDate = criteria.CheckinDate,
                                        //            CheckoutDate = criteria.CheckoutDate,
                                        //            CurrencyIsoCode = criteria.CurrencyIsoCode,
                                        //            IsBundle = criteria.IsBundle,
                                        //            Language = criteria.Language,
                                        //            MarginPercentage = criteria.MarginPercentage,
                                        //            NoOfPassengers = criteria.NoOfPassengers,
                                        //            PriceCategoryIdMapping = new List<PriceCategory>() { priceCatId },
                                        //            PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                                        //            PassengerInfo = criteria.PassengerInfo,
                                        //            Token = criteria.Token
                                        //        };

                                        //        var option = CreateOption(itemsByTime, tempCriteria, rateId, activity);

                                        //        if (option != null)
                                        //        {
                                        //            option.TravelInfo = new TravelInfo
                                        //            {
                                        //                StartDate = criteria.CheckinDate,
                                        //                NumberOfNights = 0,
                                        //                NoOfPassengers = criteria.NoOfPassengers
                                        //            };
                                        //            options.Add(option);
                                        //        }
                                        //    }
                                        //}
                                        if (isangoActvivity != null)
                                        {
                                            try
                                            {
                                                foreach (var opt in isangoActvivity.ProductOptions)
                                                {
                                                    try
                                                    {
                                                        if (opt.SupplierOptionCode == serviceOptionCode.ToString() && opt.PrefixServiceCode == rateId.ToString())
                                                        {

                                                            var priceCatIds = criteria.PriceCategoryIdMapping?.Where(x => x.OptionId == opt.Id)?.ToList();

                                                            var tempCriteria = new BokunCriteria
                                                            {
                                                                ActivityCode = criteria.ActivityCode,
                                                                FactSheetIds = criteria.FactSheetIds,
                                                                ActivityId = criteria.ActivityId,
                                                                ActivityMargin = criteria.ActivityMargin,
                                                                Ages = criteria.Ages,
                                                                CheckinDate = criteria.CheckinDate,
                                                                CheckoutDate = criteria.CheckoutDate,
                                                                CurrencyIsoCode = criteria.CurrencyIsoCode,
                                                                IsBundle = criteria.IsBundle,
                                                                Language = criteria.Language,
                                                                MarginPercentage = criteria.MarginPercentage,
                                                                NoOfPassengers = criteria.NoOfPassengers,
                                                                PriceCategoryIdMapping = priceCatIds,
                                                                PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                                                                PassengerInfo = criteria.PassengerInfo,
                                                                Token = criteria.Token
                                                            };

                                                            var option = CreateOption(itemsByTime, tempCriteria, rateId, activity);

                                                            if (option != null)
                                                            {
                                                                option.TravelInfo = new TravelInfo
                                                                {
                                                                    StartDate = criteria.CheckinDate,
                                                                    NumberOfNights = 0,
                                                                    NoOfPassengers = criteria.NoOfPassengers
                                                                };
                                                                option.Id = opt.Id;
                                                                options.Add(option);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        //ignore
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                var isangoErrorEntity = new IsangoErrorEntity
                                                {
                                                    ClassName = "Bokun.CheckAvailabilitiesConverter",
                                                    MethodName = "ConvertToActivitiesResult.CreateOption",
                                                    Params = $"serviceOptionCode : {serviceOptionCode}, rateId : {rateId}",
                                                    Token = criteria.Token
                                                };
                                                _logger.Error(isangoErrorEntity, ex);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var option = CreateOption(itemsByTime, criteria, rateId, activity);

                                                if (option != null)
                                                {
                                                    option.TravelInfo = new TravelInfo
                                                    {
                                                        StartDate = criteria.CheckinDate,
                                                        NumberOfNights = 0,
                                                        NoOfPassengers = criteria.NoOfPassengers
                                                    };
                                                    options.Add(option);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                var isangoErrorEntity = new IsangoErrorEntity
                                                {
                                                    ClassName = "Bokun.CheckAvailabilitiesConverter",
                                                    MethodName = "ConvertToActivitiesResult.CreateOption",
                                                    Params = $"serviceOptionCode : {serviceOptionCode}, rateId : {rateId}",
                                                    Token = criteria.Token
                                                };
                                                _logger.Error(isangoErrorEntity, ex);
                                                continue;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        try
                                        {
                                            var option = CreateOption(itemsByTime, criteria, rateId, activity);

                                            if (option != null)
                                            {
                                                option.TravelInfo = new TravelInfo
                                                {
                                                    StartDate = criteria.CheckinDate,
                                                    NumberOfNights = 0,
                                                    NoOfPassengers = criteria.NoOfPassengers
                                                };
                                                options.Add(option);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            var isangoErrorEntity = new IsangoErrorEntity
                                            {
                                                ClassName = "Bokun.CheckAvailabilitiesConverter",
                                                MethodName = "ConvertToActivitiesResult.CreateOption",
                                                Params = $"serviceOptionCode : {serviceOptionCode}, rateId : {rateId}",
                                                Token = criteria.Token
                                            };
                                            _logger.Error(isangoErrorEntity, ex);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            activity.ProductOptions = options.Cast<ProductOption>().ToList();
            activities.Add(activity);
            return activities;
        }

        private ActivityOption CreateOption(List<CheckAvailabilitiesRs> checkAvailabilities, BokunCriteria criteria, int? rateId, Activity activity)
        {
            var totalPaxCount = criteria.NoOfPassengers.Sum(x => x.Value);
            var capacity = _optionCapacity;
            var currency = string.Empty;

            var queryAvailability = from act in checkAvailabilities
                                    from r in act?.PricesByRate
                                    where r?.ActivityRateId == rateId
                                    select act;

            var queryRate = from act in checkAvailabilities
                            from r in act?.Rates
                            where r?.Id == rateId
                            select r;

            var queryPricesByRate = from act in checkAvailabilities
                                    from r in act?.PricesByRate
                                    where r?.ActivityRateId == rateId
                                    select r;

            var availabilityRs = queryAvailability?.FirstOrDefault();
            var rate = queryRate?.FirstOrDefault();
            var isPerPersonPrice = rate?.PricedPerPerson ?? false;
            var isTieredPricing = rate?.TieredPricingEnabled ?? false;
            var priceByRate = queryPricesByRate?.FirstOrDefault();
            var checkAvailability = availabilityRs;

            var option = new ActivityOption();
            var defaultDate = new DateTime(1970, 01, 01, 00, 00, 00);
            var pricingCategoryIds = new List<int>();

            var mappedPricingCategoryIds = criteria.PriceCategoryIdMapping?.Where(x => x.ServiceOptionCode == checkAvailability?.ActivityId)?.ToList();
            var mappedPricingCategoryIdsQuery = from mp in mappedPricingCategoryIds
                                                from pcid in priceByRate?.PricePerCategoryUnit?.ToList()
                                                where mp.PriceCategoryId == pcid.Id
                                                select mp;
            mappedPricingCategoryIds = mappedPricingCategoryIdsQuery?.ToList();

            if (mappedPricingCategoryIdsQuery?.Any() == false)
            {
                mappedPricingCategoryIdsQuery = from mp in criteria.PriceCategoryIdMapping?.Where(x => x.ServiceOptionCode == checkAvailability?.ActivityId)?.ToList()
                                                from pcid in rate?.PricingCategoryIds
                                                where mp.PriceCategoryId == pcid
                                                select mp;

                mappedPricingCategoryIds = mappedPricingCategoryIdsQuery?.ToList();
            }

            if (mappedPricingCategoryIds?.Any() == false && isPerPersonPrice)
            {
                var code = "PriceCategoryMappingError";
                var errMsg = "PriceCategoryMappingError mapping not found";
                if (activity.Errors == null && activity != null)
                {
                    activity.Errors = new List<Error>();
                }
                var error = new Error
                {
                    Code = code,
                    Message = errMsg
                };
                if (activity?.Errors?.Any(x => x.Code == code) == false)
                {
                    activity.Errors.Add(error);
                }
                return null;
            }

            var pricePerCategoryUnits = new List<Pricepercategoryunit>();

            if (isTieredPricing && isPerPersonPrice) //Tiered Pricing - Calculate according to criteria given
            {
                try
                {
                    foreach (var criteriaPax in criteria.NoOfPassengers)
                    {
                        var mappedPax = mappedPricingCategoryIds?.Where(x => x.PassengerTypeId == criteriaPax.Key)?.FirstOrDefault();
                        var pricepercategoryunit = priceByRate?.PricePerCategoryUnit?.Where(x => x.Id == mappedPax.PriceCategoryId && (x.MinParticipantsRequired == null || x.MinParticipantsRequired <= criteriaPax.Value) && (x.MaxParticipantsRequired == null || x.MaxParticipantsRequired >= criteriaPax.Value))?.FirstOrDefault();
                        pricePerCategoryUnits.Add(pricepercategoryunit);
                    }
                }
                catch (Exception ex) //Fallback to old logic
                {
                    Task.Run(() => _logger.Error(new IsangoErrorEntity
                    {
                        Params = SerializeDeSerializeHelper.Serialize(checkAvailability),
                        ClassName = nameof(CheckAvailabilitiesConverter),
                        MethodName = nameof(CreateOption),
                        Token = criteria.Token
                    }, ex));

                    pricePerCategoryUnits = priceByRate?.PricePerCategoryUnit?.ToList();
                }
            }
            else if (isPerPersonPrice) //Price per person from api
            {
                pricePerCategoryUnits = priceByRate?.PricePerCategoryUnit?.ToList();
            }
            else
            {
                //Price per booking from api (pricing per unit is applicable)
                var pricepercategoryunit = new Pricepercategoryunit
                {
                    Amount = new Amounts
                    {
                        Amount = priceByRate.PricePerBooking.Amount,
                        Currency = priceByRate.PricePerBooking.Currency
                    },
                    Id = mappedPricingCategoryIds?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Adult)?.PriceCategoryId
                };
                pricePerCategoryUnits.Add(pricepercategoryunit);
            }

            if (pricePerCategoryUnits != null)
            {
                currency = pricePerCategoryUnits?.FirstOrDefault()?.Amount?.Currency;
            }

            foreach (var passengerType in validPassengerTypes)
            {
                if (criteria.NoOfPassengers.ContainsKey(passengerType))
                {
                    var paxCount = criteria.NoOfPassengers[passengerType];
                    var mappedChildPricingCategory = mappedPricingCategoryIds.FirstOrDefault(x => x.PassengerTypeId == passengerType);
                    if (mappedChildPricingCategory != null && paxCount > 0)
                    {
                        pricingCategoryIds.AddRange(Enumerable.Repeat(mappedChildPricingCategory.PriceCategoryId, paxCount));
                    }
                }
            }

            option.PricingCategoryId = pricingCategoryIds;
            var apiCancellationPolicy = new Cancellationpolicy();
            if (checkAvailability != null)
            {
                option.SupplierOptionCode = checkAvailability?.ActivityId?.ToString();
                var priceAndAvailabiltyBase = new Dictionary<DateTime, PriceAndAvailability>();

                foreach (var item in checkAvailabilities)
                {
                    try
                    {
                        var dt = defaultDate.AddMilliseconds(item.Date);
                        var dateKey = dt.ToString(Constant.DateFormatyyyyMMdd).ToDateTimeExact();
                        var pricingUnits = new List<PricingUnit>();

                        foreach (var passengerType in validPassengerTypes)
                        {
                            try
                            {
                                var pricingUnit = CreatePricingUnit(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, passengerType, isPerPersonPrice);

                                if (pricingUnit != null)// && pricingUnit?.Price > 0M)
                                {
                                    if (!isPerPersonPrice)

                                    {
                                        pricingUnit.UnitType = UnitType.PerUnit;
                                        pricingUnit.PriceType = PriceType.PerPerson;
                                    }
                                    var untisToBook = rate?.MaxPerBooking > 0
                                                        ? totalPaxCount / System.Convert.ToDecimal(rate.MaxPerBooking)
                                                        : totalPaxCount / System.Convert.ToDecimal(capacity);

                                    pricingUnit.Quantity = isPerPersonPrice ? (int)Math.Ceiling(untisToBook) : totalPaxCount;

                                    pricingUnit.TotalCapacity = rate?.MaxPerBooking > 0
                                                        ? System.Convert.ToInt32(rate.MaxPerBooking)
                                                        : capacity;

                                    pricingUnit.Mincapacity = rate?.MinPerBooking > 0
                                                        ? System.Convert.ToInt32(rate.MinPerBooking)
                                                        : 1;

                                    if (pricingUnit.UnitType == UnitType.PerUnit)
                                    {
                                        pricingUnit.Price = pricingUnit.Price / totalPaxCount;
                                    }

                                    pricingUnits.Add(pricingUnit);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (criteria.Ages.ContainsKey(passengerType))
                                {
                                    var code = "PricingUnitError";
                                    var errMsg = ex.Message;
                                    if (activity.Errors == null && activity != null)
                                    {
                                        activity.Errors = new List<Error>();
                                    }
                                    var error = new Error
                                    {
                                        Code = code,
                                        Message = errMsg
                                    };
                                    if (activity?.Errors?.Any(x => x.Code == code) == false)
                                    {
                                        activity.Errors.Add(error);
                                    }
                                }
                            }
                        }
                        int.TryParse(item?.AvailabilityCount?.ToString(), out int tempint);
                        var minTotlaCapacity = (pricingUnits?.OrderBy(x => x?.TotalCapacity)?.FirstOrDefault()?.TotalCapacity) ??
                      0;
                        tempint = minTotlaCapacity > 0 ? minTotlaCapacity : tempint;
                        capacity = tempint > 0 ? tempint : capacity;

                        var chargablePUs = criteria.NoOfPassengers.Where(y => y.Key != PassengerType.Infant)?.Sum(x => x.Value);
                        var minMinCapacity = (pricingUnits?.OrderByDescending(x => x?.Mincapacity)?.FirstOrDefault()?.Mincapacity) ??
                      0;
                        if (chargablePUs < minMinCapacity)
                        {
                            var code = "MinimumRequiredPassengerError";
                            var errMsg = $"Minimum  {minMinCapacity} passenger are required for booking.";
                            if (activity.Errors == null && activity != null)
                            {
                                activity.Errors = new List<Error>();
                            }
                            var error = new Error
                            {
                                Code = code,
                                Message = errMsg
                            };
                            if (activity?.Errors?.Any(x => x.Code == code) == false)
                            {
                                activity.Errors.Add(error);
                            }
                            option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                            return option;
                        }

                        if (option.Capacity < capacity)
                        {
                            option.Capacity = capacity;
                        }

                        var priceBase = new BokunPriceAndAvailability
                        {
                            //TODO: Need to validate AvailabilityStatus logic
                            AvailabilityStatus = pricingUnits.Sum(e => e.Price) > 0
                                ? AvailabilityStatus.AVAILABLE
                                : AvailabilityStatus.NOTAVAILABLE,
                            IsSelected = false,
                            TotalPrice = pricingUnits.Sum(e => e.Price),
                            PricingUnits = pricingUnits,
                            Capacity = capacity,
                            DefaultRateId = rateId ?? System.Convert.ToInt32(item?.DefaultRateId),
                            UnitQuantity = isPerPersonPrice ? 0 : pricingUnits.FirstOrDefault().Quantity,
                        };

                        if (!priceAndAvailabiltyBase.Keys.Contains(dateKey))
                            priceAndAvailabiltyBase.Add(dateKey, priceBase);
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => _logger.Error(new IsangoErrorEntity
                        {
                            Params = SerializeDeSerializeHelper.Serialize(item),
                            ClassName = nameof(CheckAvailabilitiesConverter),
                            MethodName = nameof(CreateOption),
                            Token = criteria.Token
                        }, ex)
                         );
                    }
                }

                option.BasePrice = new Isango.Entities.Price
                {
                    Amount = priceAndAvailabiltyBase.FirstOrDefault().Value.PricingUnits.Sum(e => e.Price),
                    Currency = new Currency { IsoCode = currency, IsPostFix = true, Name = "", Symbol = "" },
                    DatePriceAndAvailabilty = priceAndAvailabiltyBase
                };
                option.GateBasePrice = option.BasePrice.DeepCopy(); // Preparing GateBasePrice here as its needed in PriceRuleEngine
                var activitySeason = GenerateActivitySeason(checkAvailability, currency, pricePerCategoryUnits, mappedPricingCategoryIds);
                option.ActivitySeasons = new List<ActivitySeason> { activitySeason };
                option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                option.Code = checkAvailabilities.FirstOrDefault()?.ActivityId.ToString();

                apiCancellationPolicy = checkAvailability.Rates.Where(x => x.Id == rateId)
                    .Select(x => x.CancellationPolicy).FirstOrDefault();
                var penaltyRule = apiCancellationPolicy?.PenaltyRules?.FirstOrDefault();
                if (penaltyRule?.CutoffHours != null || apiCancellationPolicy?.PolicyType?.ToUpper() == "NON_REFUNDABLE")
                {
                    UpdateCancellationPolicy(
                      cancellationPrices: option?.CancellationPrices
                      , penaltyRule: penaltyRule
                      , defaultDate: defaultDate
                      , checkAvailability: checkAvailability
                      , option: option
                      , criteria: criteria
                      , cancellationpolicy: apiCancellationPolicy
                      );
                }
            }

            option.RateId = option.RateKey = option.PrefixServiceCode = System.Convert.ToString(rateId);

            option.StartTimeId = System.Convert.ToInt32(availabilityRs.StartTimeId);
            var rateDetails = availabilityRs?.Rates?.FirstOrDefault(r => r.Id == rateId);
            if (rateDetails?.Title.ToLower().Trim() == "standard rate" || rateDetails?.Title.ToLower().Trim() == "default rate")
            {
                rateDetails.Title = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(rateDetails?.Title) && _isAppendBokunAPIDescription)
            {
                option.Description = string.IsNullOrWhiteSpace(option?.Description) ?
                    $"({rateDetails?.Title} {", " + apiCancellationPolicy?.Title})" :
                    $"({option?.Description}-{rateDetails?.Title})";
            }

            TimeSpan.TryParse(availabilityRs?.StartTime, out TimeSpan startTime);
            option.StartTime = startTime;
            return option;
        }

        private static ActivitySeason GenerateActivitySeason(CheckAvailabilitiesRs checkAvailability, string currency, List<Pricepercategoryunit> pricePerCategoryUnits, List<PriceCategory> mappedPricingCategoryIds)
        {
            var aPriceAdult = new PolicyCategory();
            var aPriceChild = new PolicyCategory();
            var aPriceInfant = new PolicyCategory();
            var aPriceYouth = new PolicyCategory();
            var aPriceSenior = new PolicyCategory();
            var aPriceConcession = new PolicyCategory();
            var aPriceFamily = new PolicyCategory();
            var aPriceStudent = new PolicyCategory();

            if (checkAvailability.IsAdultAllowed)
            {
                var adultBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Adult);
                aPriceAdult.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = adultBasePrice,
                    IsAllowed = true,
                    IsPercent = false,
                };
                aPriceAdult.PolicyCategoryType = PassengerType.Adult;
                aPriceAdult.FromAge = Constant.AdultFromAge;
                aPriceAdult.ToAge = Constant.AdultToAge;
                if (checkAvailability.MinParticipants != null) aPriceAdult.MinimumCustomers = checkAvailability.MinParticipants.Value;
                if (checkAvailability.AvailabilityCount != null && checkAvailability.BookedParticipants != null)
                    aPriceAdult.MaximumCustomers = checkAvailability.AvailabilityCount.Value + checkAvailability.BookedParticipants.Value;
            }
            if (checkAvailability.IsChildAllowed)
            {
                var childBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Child);
                aPriceChild.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = childBasePrice,
                    IsAllowed = false,
                    IsPercent = false,
                };
                aPriceChild.PolicyCategoryType = PassengerType.Child;
                aPriceChild.FromAge = Constant.ChildFromAge;
                aPriceChild.ToAge = Constant.ChildToAge;
            }
            if (checkAvailability.IsInfantAllowed)
            {
                var infantBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Infant);
                aPriceInfant.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = infantBasePrice,
                    IsAllowed = checkAvailability.IsInfantAllowed,
                    IsPercent = false,
                };
                aPriceInfant.PolicyCategoryType = PassengerType.Infant;
                aPriceInfant.FromAge = checkAvailability.MinInfantAge;
                aPriceInfant.ToAge = checkAvailability.MaxInfantAge;
            }
            if (checkAvailability.IsYouthAllowed)
            {
                var youthBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Youth);
                aPriceYouth.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = youthBasePrice,
                    IsAllowed = checkAvailability.IsYouthAllowed,
                    IsPercent = false,
                };
                aPriceYouth.PolicyCategoryType = PassengerType.Youth;
                aPriceYouth.FromAge = checkAvailability.MinYouthAge;
                aPriceYouth.ToAge = checkAvailability.MaxYouthAge;
            }
            if (checkAvailability.IsSeniorAllowed)
            {
                var seniorBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Senior);
                aPriceSenior.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = seniorBasePrice,
                    IsAllowed = checkAvailability.IsSeniorAllowed,
                    IsPercent = false,
                };
                aPriceSenior.PolicyCategoryType = PassengerType.Senior;
                aPriceSenior.FromAge = checkAvailability.MinSeniorAge;
                aPriceSenior.ToAge = checkAvailability.MaxSeniorAge;
            }
            if (checkAvailability.IsConcessionAllowed)
            {
                var concessionBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Concession);
                aPriceConcession.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = concessionBasePrice,
                    IsAllowed = checkAvailability.IsConcessionAllowed,
                    IsPercent = false,
                };
                aPriceConcession.PolicyCategoryType = PassengerType.Concession;
                aPriceConcession.FromAge = checkAvailability.MinConcessionAge;
                aPriceConcession.ToAge = checkAvailability.MaxConcessionAge;
            }
            if (checkAvailability.IsFamilyAllowed)
            {
                var familyBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Family);
                aPriceFamily.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = familyBasePrice,
                    IsAllowed = checkAvailability.IsFamilyAllowed,
                    IsPercent = false,
                };
                aPriceFamily.PolicyCategoryType = PassengerType.Family;
                aPriceFamily.FromAge = checkAvailability.MinConcessionAge;
                aPriceFamily.ToAge = checkAvailability.MaxConcessionAge;
            }
            if (checkAvailability.IsStudentAllowed)
            {
                var studentBasePrice = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, PassengerType.Student);
                aPriceStudent.PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = currency,
                    BasePrice = studentBasePrice,
                    IsAllowed = checkAvailability.IsStudentAllowed,
                    IsPercent = false,
                };
                aPriceStudent.PolicyCategoryType = PassengerType.Student;
                aPriceStudent.FromAge = checkAvailability.MinStudentAge;
                aPriceStudent.ToAge = checkAvailability.MaxStudentAge;
            }

            var season = new ActivitySeason
            {
                ActivityPolicies = new List<ActivityPolicy>
                    {
                        new ActivityPolicy
                        {
                            PolicyCategories = new List<PolicyCategory>
                            {
                                aPriceAdult, aPriceChild, aPriceYouth, aPriceInfant, aPriceSenior, aPriceConcession, aPriceFamily, aPriceStudent
                            }
                        }
                    }
            };
            return season;
        }

        private static decimal GetPriceByPriceCategoryId(CheckAvailabilitiesRs checkAvailability, List<Pricepercategoryunit> pricePerCategoryUnits, List<PriceCategory> mappedPricingCategoryIds, PassengerType passengerType)
        {
            var basePrice = 0.0M;
            var priceCategory = mappedPricingCategoryIds?.FirstOrDefault(x => x.PassengerTypeId == passengerType);
            if (priceCategory != null && checkAvailability != null)// && checkAvailability.PaxAgeGroupIds.ContainsValue(priceCategory.PriceCategoryId))
            {
                basePrice = pricePerCategoryUnits.FirstOrDefault(x => x.Id == priceCategory?.PriceCategoryId)?.Amount?.Amount ?? 0;
            }

            return basePrice;
        }

        private PricingUnit CreatePricingUnit(CheckAvailabilitiesRs checkAvailability, List<Pricepercategoryunit> pricePerCategoryUnits, List<PriceCategory> mappedPricingCategoryIds, PassengerType passengerType, bool isPerPersonPrice = true)
        {
            PricingUnit pricingUnit = null;
            if (IsPassengerAllowed(checkAvailability, passengerType))
            {
                var price = GetPriceByPriceCategoryId(checkAvailability, pricePerCategoryUnits, mappedPricingCategoryIds, passengerType);

                pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);

                if (pricingUnit != null)
                {
                    pricingUnit.Price = price;
                }
            }
            //else
            //{
            //    throw new Exception($"PassengerType {passengerType} not allowed");
            //}
            return pricingUnit;
        }

        private bool IsPassengerAllowed(CheckAvailabilitiesRs checkAvailability, PassengerType passengerType, BokunCriteria criteria = null)
        {
            switch (passengerType)
            {
                case PassengerType.Adult:
                    return checkAvailability.IsAdultAllowed;

                case PassengerType.Child:
                    return checkAvailability.IsChildAllowed;

                case PassengerType.Youth:
                    return checkAvailability.IsYouthAllowed;

                case PassengerType.Infant:
                    return checkAvailability.IsInfantAllowed;

                case PassengerType.Senior:
                    return checkAvailability.IsSeniorAllowed;

                case PassengerType.Concession:
                    return checkAvailability.IsConcessionAllowed;

                case PassengerType.Family:
                    return checkAvailability.IsFamilyAllowed;

                case PassengerType.Student:
                    return checkAvailability.IsStudentAllowed;

                default:
                    return false;
            }
        }
    }
}