using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.TourCMS;
using Isango.Entities.TourCMSCriteria;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.CheckAvailabilityResponse;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public class AvailabilityConverter : ConverterBase, IAvailabilityConverter
    {

        public AvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        private string _currencyISOCode;
        private int _adultCount;
        private int _childCount;
        private int _infantCount;

        private int _youthCount;
        private int _seniorCount;

        private int _familyCount;
        private int _studentCount;

        private int _totalPaxCount;
        private int _tempInt;

        private TourCMSCriteria _criteria;
        #region Private Methods
        object IConverterBase.Convert(object apiResponse, Entities.MethodType methodType, object criteria)
        {

            var result = SerializeDeSerializeHelper.DeSerializeXml
                     <CheckAvailabilityResponse>(apiResponse as string);
            return result != null ? ConvertAvailabilityResult(result, criteria) : null;
        }
        private Activity ConvertAvailabilityResult(CheckAvailabilityResponse result, object criteria)
        {

            _criteria = criteria as TourCMSCriteria;
            int? lineofbusinessid = _criteria.LineOfBusinessId;
            var tourCMSApiActivityComponent = result?.AvailableComponents?.Component;
            var activity = new Activity();
            //Multiple Times a day
            var options = new List<ActivityOption>();

            //check if only single component return  ,then don't append time
            var IsHaveOnlySingleComponent = false;
            if (tourCMSApiActivityComponent?.Count == 1)
            {
                IsHaveOnlySingleComponent = true;
            }
            //if all components have same startdate and enddate ,then don't append time
            var IsAllComponentsHaveSameTime = false;
            var dupes = tourCMSApiActivityComponent?.GroupBy(x => new { x.StartTime, x.EndTime })
                ?.Where(grp => grp.Count() > 1)?.Sum(grp => grp.Count());
            if (dupes > 1)
            {
                //below condition means all are match duplicate
                if (tourCMSApiActivityComponent.Count() == dupes)
                {
                    IsAllComponentsHaveSameTime = true;
                }
            }

            foreach (var itemTourCMSApiActivityComponent in
                tourCMSApiActivityComponent)
            {
                if (_criteria == null || itemTourCMSApiActivityComponent == null)
                {
                    return null;
                }

                try
                {
                    _currencyISOCode = itemTourCMSApiActivityComponent?.SaleCurrency;
                    _adultCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value ?? 0;
                    _childCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value ?? 0;
                    _infantCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value ?? 0;
                    _youthCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Youth)?.FirstOrDefault().Value ?? 0;
                    _seniorCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Senior)?.FirstOrDefault().Value ?? 0;

                    _familyCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Family)?.FirstOrDefault().Value ?? 0;
                    _studentCount = _criteria?.NoOfPassengers?.Where(x => x.Key == PassengerType.Student)?.FirstOrDefault().Value ?? 0;


                    _totalPaxCount = _adultCount + _childCount + _infantCount + _youthCount + _seniorCount + _familyCount + _studentCount;

                    int.TryParse(_criteria.IsangoActivityId, out _tempInt);
                    activity.ID = _tempInt;
                    activity.Name = result.TourName;
                    activity.Code = System.Convert.ToString(result.TourId);
                    activity.CategoryIDs = new List<int> { 1 };

                    var travelInfo = new TravelInfo
                    {
                        Ages = _criteria.Ages,
                        NoOfPassengers = _criteria.NoOfPassengers,
                        NumberOfNights = 0,
                        StartDate = _criteria.CheckinDate
                    };
                    try
                    {
                        //Run In Every Case 
                        #region Start: Options without Extras
                        var option = CreateOption(travelInfo, _criteria,
                          itemTourCMSApiActivityComponent, result?.TourName,
                          result.TourId, result?.SaleQuantityRule);
                        if (option != null)
                        {

                            if (!string.IsNullOrEmpty(itemTourCMSApiActivityComponent.StartTime) && !string.IsNullOrEmpty(itemTourCMSApiActivityComponent.EndTime))
                            {
                                string startTimeString = itemTourCMSApiActivityComponent.StartTime;
                                string endTimeString = itemTourCMSApiActivityComponent.EndTime;

                                option.StartTime = TimeSpan.TryParse(startTimeString, out TimeSpan startTime) ? startTime : TimeSpan.Zero;
                                option.EndTime = TimeSpan.TryParse(endTimeString, out TimeSpan endTime) ? endTime : TimeSpan.Zero;


                                string noteValueSet = itemTourCMSApiActivityComponent?.Note;
                                string timeValueSet = itemTourCMSApiActivityComponent.StartTime + " - " + itemTourCMSApiActivityComponent.EndTime;
                                string tourNameSet = result.TourName;
                                try
                                {
                                    tourNameSet = string.IsNullOrEmpty(result.TourNameLong) ? result.TourName : result.TourNameLong;
                                }
                                catch (Exception ex)
                                {
                                    tourNameSet = result.TourName;
                                }
                                if (!string.IsNullOrEmpty(noteValueSet))
                                {
                                    option.Name = noteValueSet;

                                    //Commenting its because concatination of start time becoming from both bumblebee and Phoenix
                                    //if ((IsAllComponentsHaveSameTime == true || IsHaveOnlySingleComponent == true) && (lineofbusinessid == 5 || lineofbusinessid == 2))
                                    //{
                                    //    option.Name = noteValueSet;
                                    //}
                                    //else
                                    //{
                                    //    option.Name = noteValueSet;
                                    //}
                                }
                                else
                                {
                                    option.Name = tourNameSet;

                                    //if ((IsAllComponentsHaveSameTime == true || IsHaveOnlySingleComponent == true) && (lineofbusinessid == 5 || lineofbusinessid == 2))
                                    //{
                                    //    option.Name = tourNameSet;
                                    //}
                                    //else
                                    //{
                                    //    option.Name = tourNameSet  ;
                                    //}
                                }
                            }
                            else //when no startdate and enddate get from API
                            {
                                var noteValueSet = itemTourCMSApiActivityComponent?.Note;
                                string tourNameSet = result.TourName;
                                try
                                {
                                    tourNameSet = string.IsNullOrEmpty(result.TourNameLong) ? result.TourName : result.TourNameLong;
                                }
                                catch (Exception ex)
                                {
                                    tourNameSet = result.TourName;
                                }
                                if (!string.IsNullOrEmpty(noteValueSet))
                                {
                                    option.Name = noteValueSet;
                                }
                                else
                                {
                                    option.Name = tourNameSet;
                                }
                            }
                            options.Add(option);

                        }
                        #endregion End: Options without Extras

                        //Options Loop
                        #region Start: Options With Extras
                        var extraOptions = itemTourCMSApiActivityComponent?.Options?.ComponentOptionsOption;
                        if (extraOptions != null && extraOptions.Count > 0)
                        {
                            foreach (var extraItem in extraOptions)
                            {
                                if (extraItem != null)
                                {
                                    foreach (var qtyAndPricesSelection in extraItem?.QuantitiesAndPrices?.Selection)
                                    {
                                        if (qtyAndPricesSelection != null)
                                        {
                                            var getOption = CreateOption(travelInfo, _criteria,
                                            itemTourCMSApiActivityComponent, result?.TourName,
                                            result.TourId, result?.SaleQuantityRule,
                                            qtyAndPricesSelection, extraItem);
                                            if (getOption != null)
                                            {
                                                //Confirmed with Supplier:DateCode should not use in optionname

                                                if (!string.IsNullOrEmpty(itemTourCMSApiActivityComponent.StartTime) && !string.IsNullOrEmpty(itemTourCMSApiActivityComponent.EndTime))
                                                {
                                                    string startTimeString = itemTourCMSApiActivityComponent.StartTime;
                                                    string endTimeString = itemTourCMSApiActivityComponent.EndTime;

                                                    option.StartTime = TimeSpan.TryParse(startTimeString, out TimeSpan startTime) ? startTime : TimeSpan.Zero;
                                                    option.EndTime = TimeSpan.TryParse(endTimeString, out TimeSpan endTime) ? endTime : TimeSpan.Zero;

                                                    string noteValueSet = itemTourCMSApiActivityComponent?.Note;
                                                    string timeValueSet = itemTourCMSApiActivityComponent.StartTime + " - " + itemTourCMSApiActivityComponent.EndTime;
                                                    string tourNameSet = result.TourName;
                                                    try
                                                    {
                                                        tourNameSet = string.IsNullOrEmpty(result.TourNameLong) ? result.TourName : result.TourNameLong;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        tourNameSet = result.TourName;
                                                    }

                                                    if (!string.IsNullOrEmpty(noteValueSet))
                                                    {
                                                        getOption.Name = getOption.Name + " ," + noteValueSet;

                                                        //if ((IsAllComponentsHaveSameTime == true || IsHaveOnlySingleComponent == true) && (lineofbusinessid == 5 || lineofbusinessid == 2))
                                                        //{
                                                        //    getOption.Name = getOption.Name + " ," + noteValueSet;
                                                        //}
                                                        //else
                                                        //{
                                                        //    getOption.Name = getOption.Name + " ," + noteValueSet;
                                                        //}
                                                    }
                                                    else
                                                    {
                                                        getOption.Name = tourNameSet + ", " + getOption.Name;

                                                        //if ((IsAllComponentsHaveSameTime == true || IsHaveOnlySingleComponent == true) && (lineofbusinessid == 5 || lineofbusinessid == 2))
                                                        //{
                                                        //    getOption.Name = tourNameSet + ", " + getOption.Name;
                                                        //}
                                                        //else
                                                        //{
                                                        //    getOption.Name = tourNameSet + ", " + getOption.Name ;
                                                        //}
                                                    }
                                                }
                                                else //if starttime and endTime not come from API
                                                {
                                                    string tourNameSet = result.TourName;
                                                    try
                                                    {
                                                        tourNameSet = string.IsNullOrEmpty(result.TourNameLong) ? result.TourName : result.TourNameLong;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        tourNameSet = result.TourName;
                                                    }
                                                    var noteValueSet = itemTourCMSApiActivityComponent?.Note;

                                                    if (!string.IsNullOrEmpty(noteValueSet))
                                                    {
                                                        getOption.Name = noteValueSet + ", " + getOption.Name;
                                                    }
                                                    else
                                                    {
                                                        getOption.Name = tourNameSet + ", " + getOption.Name;
                                                    }
                                                }
                                                options.Add(getOption);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion End: Options With Extras
                    }
                    catch (Exception ex)
                    {
                        //ignore
                        //#TODO add logging here;
                    }


                }
                catch (Exception ex)
                {
                    //ignore
                    //TODOlogging
                    activity = null;
                }
            }
            activity.FactsheetId = result.TourId;
            var availableOptions = options;
            activity.ProductOptions = availableOptions?.Cast<ProductOption>()?.ToList();
            return activity;
        }

        private ActivityOption CreateOption(TravelInfo travelInfo,
            TourCMSCriteria criteria,
            Component component, string tourName, int tourId,
            string saleQuantityRule,
            QuantitiesAndPricesSelection quantitySelection = null,
            ComponentOptionsOption componentOptionsOption = null)
        {
            var resultAPIComponent = component;
            if (resultAPIComponent == null)
                return null;

            var option = default(ActivityOption);

            var basePrice = new Price();

            var pAndABasePrice = new Dictionary<DateTime, PriceAndAvailability>();

            try
            {
                option = new ActivityOption
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Name = quantitySelection != null ?
                    componentOptionsOption?.OptionName + " (" + "x" + quantitySelection.Quantity + ")" :
                    tourName,
                    Code = System.Convert.ToString(tourId),
                    RateKey = quantitySelection != null ?
                    resultAPIComponent?.ComponentKey + "!!!" + quantitySelection?.ComponentKey :
                    resultAPIComponent?.ComponentKey,

                    ServiceOptionId = _criteria.ServiceOptionId.ToInt(),
                    SupplierOptionCode = System.Convert.ToString(_criteria.TourId),
                    PrefixServiceCode = _criteria.ActivityCode,
                    TravelInfo = new TravelInfo
                    {
                        Ages = criteria.Ages,
                        NoOfPassengers = criteria.NoOfPassengers,
                        NumberOfNights = 0,
                        StartDate = criteria.CheckinDate
                    },
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE,
                    CommisionPercent = criteria.IsCommissionPercent == true ? criteria.CommissionPercent : 0
                };
                //Questions and Answers
                if (resultAPIComponent?.Questions != null && resultAPIComponent?.Questions.Quest != null && resultAPIComponent?.Questions?.Quest?.Count > 0)
                {
                    option.ContractQuestions = resultAPIComponent?.Questions?.Quest?.Select(q => new ContractQuestion
                    {
                        Code = q?.QuestionKey,
                        Description = q?.Question,
                        IsRequired = q?.AnswerMandatory.ToLower() == "yes"
                        ? true : false,
                        Name = q?.QuestionInternal
                    })?.ToList();
                }
                //Set Pickup Locations
                if (component?.PickupPoints?.Pickup != null && component?.PickupPoints?.Pickup?.Count > 0)
                {
                    var pickUpList = component?.PickupPoints?.Pickup;
                    option.PickupPointsForTourCMS = new List<PickupPointsForTourCMS>();
                    if (pickUpList != null)
                    {
                        foreach (var pickupItem in pickUpList)
                        {
                            var pickUpPoint = new PickupPointsForTourCMS
                            {
                                PickupKey = pickupItem.PickupKey,
                                TimePickup = pickupItem.TimePickup,
                                PickUpName = pickupItem.PickUpName,
                                PickupId = pickupItem.PickupId,
                                Description = pickupItem.Description,
                                Address1 = pickupItem.Address1,
                                Address2 = pickupItem.Address2,
                                PostCode = pickupItem.PostCode,
                                GeoCode = pickupItem.GeoCode
                            };
                            option.PickupPointsForTourCMS.Add(pickUpPoint);
                        }
                    }
                }
                //Set Capacity Check
                option.IsCapacityCheckRequired = component?.SpacesRemaining > 0 ? true : false;
                if (component?.SpacesRemaining > 0)
                {
                    option.Capacity = component.SpacesRemaining;
                }
                var priceBasePrice = new DefaultPriceAndAvailability
                {
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE

                };

                if (resultAPIComponent?.TotalPrice <= 0)
                {
                    option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    //continue;
                }

                if (saleQuantityRule?.ToUpper().Contains("PERSON") == true)
                {
                    priceBasePrice = UpdatePricePerPax(travelInfo, criteria, priceBasePrice,
                         component, quantitySelection,
                         componentOptionsOption);

                }
                else if (saleQuantityRule?.ToUpper().Contains("GROUP") == true)
                {
                    priceBasePrice = UpdatePricePerUnit(travelInfo, criteria, priceBasePrice,
                        component, quantitySelection,
                        componentOptionsOption);
                }
                else
                {
                    option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    return option;
                }

                var opDate = resultAPIComponent.StartDate;

                pAndABasePrice.Add(opDate, priceBasePrice);

                basePrice.DatePriceAndAvailabilty = pAndABasePrice;
                basePrice.Amount = System.Convert.ToDecimal(priceBasePrice.TotalPrice);
                basePrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.BasePrice = basePrice;
                option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;

                //TimeSpan.TryParse(component?.StartTime, out TimeSpan startTime);
                //option.StartTime = startTime;

                //TimeSpan.TryParse(component?.EndTime, out TimeSpan endTime);
                //option.EndTime = endTime;

                option.QuantityRequiredMin = component.MinBookingSize;
                option.QuantityRequiredMax = component.SpacesRemaining;


            }
            catch (Exception ex)
            {
                //ignorred
                //#TODO add logging here
                option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }
            option.GateBasePrice = option.BasePrice.DeepCopy();
            return option;
        }

        private DefaultPriceAndAvailability UpdatePricePerPax(
            TravelInfo travelInfo, TourCMSCriteria criteria,
            DefaultPriceAndAvailability price,
            Component component,
            QuantitiesAndPricesSelection quantitySelection = null,
            ComponentOptionsOption componentOptionsOption = null
           )
        {

            var resultAPIComponent = component;

            decimal adultBasePrice = 0;
            decimal childBasePrice = 0;
            decimal infantBasePrice = 0;
            decimal youthBasePrice = 0;
            decimal seniorBasePrice = 0;

            decimal familyBasePrice = 0;
            decimal studentBasePrice = 0;

            price.PricingUnits = new List<PricingUnit>();

            var servicePrice = System.Convert.ToDecimal(resultAPIComponent?.TotalPrice);
            //Filtration of prices based on the ages passed in input , and pax type mapping
            //Using child ages mapping to distinguish b/w in Youth, Child and Infant
            var adultPriceQuery = from m in criteria.TourCMSMappings
                                  from pax in travelInfo.Ages
                                  from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                  where
                                  //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                  //&&
                                  pax.Key == PassengerType.Adult
                                  && m.PassengerType.ToString().ToLowerInvariant()
                                  == PassengerType.Adult.ToString().ToLowerInvariant()
                                  && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                  select pricebreakdown;

            var youthPriceQuery = from m in criteria.TourCMSMappings
                                  from pax in travelInfo.Ages
                                  from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                  where
                                  //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                  //&&
                                  pax.Key == PassengerType.Youth
                                  && m.PassengerType.ToString().ToLowerInvariant()
                                  == PassengerType.Youth.ToString().ToLowerInvariant()
                                  && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                  select pricebreakdown;

            var seniorPriceQuery = from m in criteria.TourCMSMappings
                                   from pax in travelInfo.Ages
                                   from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                   where
                                   //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                   //&&
                                   pax.Key == PassengerType.Senior
                                   && m.PassengerType.ToString().ToLowerInvariant()
                                   == PassengerType.Senior.ToString().ToLowerInvariant()
                                   && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                   select pricebreakdown;

            var childPriceQuery = from m in criteria.TourCMSMappings
                                  from pax in travelInfo.Ages
                                  from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                  where
                                  //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                  //&&
                                  pax.Key == PassengerType.Child
                                  && m.PassengerType.ToString().ToLowerInvariant()
                                  == PassengerType.Child.ToString().ToLowerInvariant()
                                  && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                  select pricebreakdown;

            var infantPriceQuery = from m in criteria.TourCMSMappings
                                   from pax in travelInfo.Ages
                                   from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                   where
                                   //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                   //&&
                                   pax.Key == PassengerType.Infant
                                   && m.PassengerType.ToString().ToLowerInvariant()
                                   == PassengerType.Infant.ToString().ToLowerInvariant()
                                   && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                   select pricebreakdown;


            var familyPriceQuery = from m in criteria.TourCMSMappings
                                   from pax in travelInfo.Ages
                                   from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                   where
                                   //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                   //&&
                                   pax.Key == PassengerType.Family
                                   && m.PassengerType.ToString().ToLowerInvariant()
                                   == PassengerType.Family.ToString().ToLowerInvariant()
                                   && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                   select pricebreakdown;

            var studentPriceQuery = from m in criteria.TourCMSMappings
                                    from pax in travelInfo.Ages
                                    from pricebreakdown in component.PriceBreakdown.PriceBreakdownPrice
                                    where
                                    //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                    //&&
                                    pax.Key == PassengerType.Student
                                    && m.PassengerType.ToString().ToLowerInvariant()
                                    == PassengerType.Student.ToString().ToLowerInvariant()
                                    && m.AgeGroupCode.ToString() == pricebreakdown.RateId
                                    select pricebreakdown;

            var adultPriceNode = adultPriceQuery?.FirstOrDefault();
            var childPriceNode = childPriceQuery?.FirstOrDefault();
            var infantPriceNode = infantPriceQuery?.FirstOrDefault();
            var youthPriceNode = youthPriceQuery?.FirstOrDefault();
            var seniorPriceNode = seniorPriceQuery?.FirstOrDefault();

            var familyPriceNode = familyPriceQuery?.FirstOrDefault();
            var studentPriceNode = studentPriceQuery?.FirstOrDefault();

            var noOfAdults = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value;
            var noOfChild = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value;
            var noOfInfant = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value;
            var noOfYouth = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Youth)?.FirstOrDefault().Value;
            var noOfSenor = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Senior)?.FirstOrDefault().Value;

            var noOfFamily = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Family)?.FirstOrDefault().Value;
            var noOfStudent = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Student)?.FirstOrDefault().Value;

            //SET Base PRICE
            adultBasePrice = System.Convert.ToDecimal(adultPriceNode?.Price);
            childBasePrice = System.Convert.ToDecimal(childPriceNode?.Price);
            infantBasePrice = System.Convert.ToDecimal(infantPriceNode?.Price);
            youthBasePrice = System.Convert.ToDecimal(youthPriceNode?.Price);
            seniorBasePrice = System.Convert.ToDecimal(seniorPriceNode?.Price);

            familyBasePrice = System.Convert.ToDecimal(familyPriceNode?.Price);
            studentBasePrice = System.Convert.ToDecimal(studentPriceNode?.Price);

            if (noOfAdults > 0)
            {
                adultBasePrice = (adultBasePrice / System.Convert.ToDecimal(noOfAdults));
            }
            if (noOfChild > 0)
            {
                childBasePrice = (childBasePrice / System.Convert.ToDecimal(noOfChild));
            }
            if (noOfInfant > 0)
            {
                infantBasePrice = (infantBasePrice / System.Convert.ToDecimal(noOfInfant));
            }
            if (noOfYouth > 0)
            {
                youthBasePrice = (youthBasePrice / System.Convert.ToDecimal(noOfYouth));
            }
            if (noOfSenor > 0)
            {
                seniorBasePrice = (seniorBasePrice / System.Convert.ToDecimal(noOfSenor));
            }
            if (noOfFamily > 0)
            {
                familyBasePrice = (familyBasePrice / System.Convert.ToDecimal(noOfFamily));
            }
            if (noOfStudent > 0)
            {
                studentBasePrice = (studentBasePrice / System.Convert.ToDecimal(noOfStudent));
            }
            //if have Extras then Price Should increase according to quantity
            if (quantitySelection != null)
            {
                adultBasePrice = adultBasePrice + ((quantitySelection.Quantity) * (quantitySelection.Price));
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
            {
                if (adultBasePrice > 0)
                {
                    var adultPriceUnit = new AdultPricingUnit
                    {
                        Price = adultBasePrice,
                        Quantity = _adultCount
                    };
                    price.PricingUnits.Add(adultPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Youth))
            {
                if (youthBasePrice > 0)
                {
                    var youthPriceUnit = new YouthPricingUnit
                    {
                        Price = youthBasePrice,
                        Quantity = _youthCount
                    };
                    price.PricingUnits.Add(youthPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Senior))
            {
                if (seniorBasePrice > 0)
                {
                    var seniorPriceUnit = new SeniorPricingUnit
                    {
                        Price = seniorBasePrice,
                        Quantity = _seniorCount
                    };
                    price.PricingUnits.Add(seniorPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
            {
                //if (childBasePrice > 0)
                //{
                var childPriceUnit = new ChildPricingUnit
                {
                    Price = childBasePrice,
                    Quantity = _childCount
                };
                price.PricingUnits.Add(childPriceUnit);
                //}
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Infant))
            {
                var infantPriceUnit = new InfantPricingUnit
                {
                    Price = infantBasePrice,
                    Quantity = _infantCount
                };
                price.PricingUnits.Add(infantPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Family))
            {
                if (familyBasePrice > 0)
                {
                    var familyPriceUnit = new FamilyPricingUnit
                    {
                        Price = familyBasePrice,
                        Quantity = _familyCount
                    };
                    price.PricingUnits.Add(familyPriceUnit);
                }
            }
            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Student))
            {
                if (studentBasePrice > 0)
                {
                    var studentPriceUnit = new StudentPricingUnit
                    {
                        Price = studentBasePrice,
                        Quantity = _studentCount
                    };
                    price.PricingUnits.Add(studentPriceUnit);
                }
            }

            price.TotalPrice = System.Convert.ToDecimal(
                (noOfAdults * adultBasePrice) +
                (noOfChild * childBasePrice) +
                (noOfYouth * youthBasePrice) +
                (noOfInfant * infantBasePrice) +
                (noOfSenor * seniorBasePrice) +
                (noOfFamily * familyBasePrice) +
                (noOfStudent * studentBasePrice));
            //servicePrice;

            //if have Extras then Price Should increase according to quantity
            if (quantitySelection != null)
            {
                price.TotalPrice = price.TotalPrice; //+ ((quantitySelection.Quantity) * (quantitySelection.Price));
            }

            if (price.TotalPrice <= 0)
            {
                price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }

            return price;
        }

        private DefaultPriceAndAvailability UpdatePricePerUnit(
            TravelInfo travelInfo, TourCMSCriteria criteria,
            DefaultPriceAndAvailability price,
            Component component,
            QuantitiesAndPricesSelection quantitySelection = null,
            ComponentOptionsOption componentOptionsOption = null)
        {
            var resultAPIComponent = component;
            price.PricingUnits = new List<PricingUnit>();

            var servicePrice = System.Convert.ToDecimal(resultAPIComponent?.TotalPrice);
            var perPersonPrice = servicePrice / _totalPaxCount;
            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
            {
                if (perPersonPrice > 0)
                {
                    var adultPriceUnit = new AdultPricingUnit
                    {
                        Price = perPersonPrice,
                        Quantity = _adultCount
                    };
                    price.PricingUnits.Add(adultPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Youth))
            {
                if (perPersonPrice > 0)
                {
                    var youthPriceUnit = new YouthPricingUnit
                    {
                        Price = perPersonPrice,
                        Quantity = _youthCount
                    };
                    price.PricingUnits.Add(youthPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Senior))
            {
                if (perPersonPrice > 0)
                {
                    var seniorPriceUnit = new SeniorPricingUnit
                    {
                        Price = perPersonPrice,
                        Quantity = _seniorCount
                    };
                    price.PricingUnits.Add(seniorPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
            {
                if (perPersonPrice > 0)
                {
                    var childPriceUnit = new ChildPricingUnit
                    {
                        Price = perPersonPrice,
                        Quantity = _childCount
                    };
                    price.PricingUnits.Add(childPriceUnit);
                }
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Infant))
            {
                var infantPriceUnit = new InfantPricingUnit
                {
                    Price = perPersonPrice,
                    Quantity = _infantCount
                };
                price.PricingUnits.Add(infantPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Family))
            {
                if (perPersonPrice > 0)
                {
                    var familyPriceUnit = new FamilyPricingUnit
                    {
                        Price = perPersonPrice,
                        Quantity = _familyCount
                    };
                    price.PricingUnits.Add(familyPriceUnit);
                }
            }
            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Student))
            {
                if (perPersonPrice > 0)
                {
                    var studentPriceUnit = new StudentPricingUnit
                    {
                        Price = perPersonPrice,
                        Quantity = _studentCount
                    };
                    price.PricingUnits.Add(studentPriceUnit);
                }
            }

            price.TotalPrice = servicePrice;

            if (price.TotalPrice <= 0)
            {
                price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }
            return price;
        }

        public override object Convert(object objectResponse, object criteria)
        {
            throw new NotImplementedException();
        }

        public override object Convert<T>(string response, T request)
        {
            throw new NotImplementedException();
        }



        #endregion Private Methods
    }
}