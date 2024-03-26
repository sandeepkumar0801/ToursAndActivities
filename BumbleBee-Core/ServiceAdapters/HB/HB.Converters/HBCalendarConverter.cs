using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using ServiceAdapters.HB.HB.Entities.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;
using Activity = Isango.Entities.Activities.Activity;
using CONSTANT = Util.CommonUtilConstantCancellation;
using Modality = ServiceAdapters.HB.HB.Entities.Calendar.Modality;
using Rate = ServiceAdapters.HB.HB.Entities.Calendar.Rate;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBCalendarConverter : ConverterBase, IHbCalendarConverter
    {
        private string _currencyISOCode;
        private int _adultCount;
        private int _childCount;
        private int _infantCount;
        private int _youthCount;
        private int _totalPaxCount;
        private int _tempInt;

        private HotelbedCriteriaApitudeFilter _criteria;

        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (CalendarRs)apiResponse;
            return result != null ? ConvertAvailabilityResult(result, criteria) : null;
        }

        #region Private Methods

        private List<Activity> ConvertAvailabilityResult(CalendarRs result, object criteria)
        {
            var activitiesIsango = new List<Activity>();
            //var lstActivityRqPaxes = result.LstActivityRqPaxes;
            _criteria = criteria as HotelbedCriteriaApitudeFilter;
            var productMappings = _criteria?.ProductMapping;

            try
            {
                if (!(productMappings?.Count > 0))
                {
                    return null;
                }

                foreach (var pm in productMappings)
                {
                    pm.DestinationCode = pm?.HotelBedsActivityCode?.Split('~')?.LastOrDefault();
                }

                if (result != null && result.Activities?.Count > 0)
                {
                    foreach (var hbApiActivity in result?.Activities)
                    {
                        var mappedProduct = productMappings?.FirstOrDefault(x => x.SupplierCode == hbApiActivity.Code);
                        if (mappedProduct == null || hbApiActivity == null) continue;

                        var activity = new Activity
                        {
                            Code = mappedProduct.SupplierCode,
                            ID = mappedProduct.IsangoHotelBedsActivityId,
                            Id = mappedProduct.IsangoHotelBedsActivityId.ToString(),
                            FactsheetId = mappedProduct.FactSheetId,
                            CurrencyIsoCode = hbApiActivity.Currency,
                            Name = hbApiActivity.Name,

                            RegionName = hbApiActivity.Country?.Destinations?.FirstOrDefault() != null
                                        ? hbApiActivity.Country?.Destinations?.FirstOrDefault()?.Name
                                        : Constant.RegionName,

                            ApiType = mappedProduct.ApiType,

                            Margin = new Margin
                            {
                                CurrencyCode = mappedProduct.CurrencyISOCode,
                                IsPercentage = mappedProduct.IsMarginPercent,
                                Value = mappedProduct.MarginAmount
                            },

                            ShortName = mappedProduct.HotelBedsActivityCode,
                            CategoryIDs = new List<int> { 1 },
                        };

                        var options = new List<ActivityOption>();

                        try
                        {
                            _currencyISOCode = hbApiActivity.Currency;
                            _adultCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value ?? 0;
                            _childCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value ?? 0;
                            _infantCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value ?? 0;
                            _totalPaxCount = _adultCount + _childCount + _infantCount;

                            #region Inclusions

                            if (hbApiActivity?.Content?.FeatureGroups != null)
                            {
                                var inclusions = hbApiActivity.Content.FeatureGroups.FindAll(f => f.Included != null && f.Included.Length > 0).ToList();
                                if (inclusions.Count > 0)
                                {
                                    var sbInclusions = new StringBuilder();
                                    foreach (var inc in inclusions)
                                    {
                                        foreach (var item in inc.Included)
                                        {
                                            sbInclusions.AppendLine($"{item?.FeatureType}-{item?.Description}");
                                        }
                                    }
                                    activity.Inclusions = sbInclusions.ToString();
                                }
                            }

                            #endregion Inclusions

                            //#TODO assign selected options duration string in case of multi-option
                            //activity.DurationString = hbApiActivity.HBActivityCode.Split(Constant.Dash)[0];

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

                            foreach (var modality in hbApiActivity?.Modalities)
                            {
                                foreach (var rate in modality?.Rates)
                                {
                                    foreach (var rateDetail in rate?.RateDetails)
                                    {
                                        try
                                        {
                                            var destinationCodeFromAPI = modality?.DestinationCode?.ToUpper() ?? hbApiActivity?.Country?.Destinations?.FirstOrDefault()?.Code;

                                            if (mappedProduct?.DestinationCode?.ToUpper() == destinationCodeFromAPI
                                                || string.IsNullOrWhiteSpace(mappedProduct?.DestinationCode?.ToUpper())
                                            )
                                            {
                                                var option = CreateOption(modality: modality, travelInfo: travelInfo, rate: rate, rateDetail: rateDetail, criteria: _criteria, productMapping: mappedProduct);

                                                if (option != null
                                                // && _criteria?.Destination?.ToUpper() == modality?.DestinationCode?.ToUpper()
                                                )
                                                {
                                                    option.Name = $"{activity.Name} - {option.Name}";
                                                    options.Add(option);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //ignore
                                            //#TODO add logging here;
                                        }
                                    }
                                }
                            }

                            activity.FactsheetId = System.Convert.ToInt32(hbApiActivity.Content?.ContentId);

                            //int modalityCount = hbApiActivity.Modalities.Count;
                            //for (int index = 0; index < modalityCount; index++)
                            //{
                            //    var option = CreateOption(hbApiActivity.Modalities[index], productOption.TravelInfo);
                            //    option.Contract = new Isango.Entities.Contract();

                            //    var comments = new List<Isango.Entities.HotelBeds.Comment>();
                            //    var comment = new Isango.Entities.HotelBeds.Comment
                            //    {
                            //        Type = hbApiActivity.Modalities[index].Comments.FirstOrDefault()?.Type,
                            //        CommentText = hbApiActivity.Modalities[index].Comments.FirstOrDefault()?.Text
                            //    };
                            //    comments.Add(comment);
                            //    option.Contract.Comments = comments;

                            //    options.Add(option);
                            //}
                            var availableOptions = options.Where(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                            activity.ProductOptions = availableOptions?.Cast<ProductOption>()?.ToList();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                            //TODOlogging
                            activity = null;
                        }
                        if (activity?.ProductOptions?.Count > 0)
                        {
                            activitiesIsango.Add(activity);
                        }
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

        private ActivityOption CreateOption(Modality modality, TravelInfo travelInfo, Rate rate, ServiceAdapters.HB.HB.Entities.Calendar.Ratedetail rateDetail, HotelbedCriteriaApitude criteria, IsangoHBProductMapping productMapping)
        {
            if (modality == null)
                return null;

            var option = default(ActivityOption);
            var costPrice = new Price();
            var sellPrice = new Price();
            var pAndACost = new Dictionary<DateTime, PriceAndAvailability>();
            var pAndASell = new Dictionary<DateTime, PriceAndAvailability>();

            /*
                if isMandatoryApplyAmount is true and && rateDetail.TotalAmount.BoxOfficeAmount >= rateDetail.TotalAmount.Amount
                then SellPrice =  rateDetail.TotalAmount.BoxOfficeAmount
                and CostPrice = rateDetail.TotalAmount.Amount and margin and commission will not apply
            else
                CostPrice = rateDetail.TotalAmount.Amount and margin will be applied to calculate sell price

            */
            bool isMandatoryApplyAmount = rateDetail?.TotalAmount?.MandatoryApplyAmount == true
                                                && rateDetail.TotalAmount.BoxOfficeAmount >= rateDetail?.TotalAmount?.Amount;

            try
            {
                option = new ActivityOption
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Name = modality?.Name,
                    Code = modality?.Code,
                    RateKey = rateDetail?.RateKey,
                    ServiceOptionId = productMapping.ServiceOptionInServiceid,
                    SupplierOptionCode = productMapping.HotelBedsActivityCode,
                    PrefixServiceCode = productMapping.SupplierCode,
                    TravelInfo = new TravelInfo
                    {
                        Ages = criteria.Ages,
                        NoOfPassengers = criteria.NoOfPassengers,
                        NumberOfNights = 0,
                        StartDate = criteria.CheckinDate
                    },
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE,
                    IsMandatoryApplyAmount = isMandatoryApplyAmount,
                };

                if (modality?.Questions != null)
                {
                    option.ContractQuestions = modality?.Questions?.Select(q => new ContractQuestion
                    {
                        Code = q?.Code,
                        Description = q?.Question ?? null,
                        IsRequired = q?.Required ?? false,
                        Name = q?.Code
                    })?.ToList();
                }

                //var operationDates = rateDetail?.OperationDates?.Where(x => x.From == criteria.CheckinDate.ToString(Constant.DateInyyyyMMdd)).ToList();

                var operationDates = rateDetail?.OperationDates?
                    .Where(x =>
                        System.Convert.ToDateTime(x.From) >= criteria.CheckinDate
                        || System.Convert.ToDateTime(x.From) <= criteria.CheckoutDate
                    ).ToList();

                if (operationDates?.Count > 0)
                {
                    foreach (var operationDate in operationDates)
                    {
                        float.TryParse(rateDetail?.MinimumDuration?.Value.ToString(), out float tempFloatMin);
                        float.TryParse(rateDetail?.MaximumDuration?.Value.ToString(), out float tempFloatMax);

                        var priceCost = new DefaultPriceAndAvailability
                        {
                            AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                            MinDuration = tempFloatMin,
                            MaxDuration = tempFloatMax,
                        };

                        var priceSell = new DefaultPriceAndAvailability
                        {
                            AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                            MinDuration = tempFloatMin,
                            MaxDuration = tempFloatMax,
                        };

                        if (rateDetail?.PaxAmounts?.Count <= 0 || rateDetail?.TotalAmount?.Amount <= 0)
                        {
                            option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                            continue;
                        }

                        option.TravelInfo.NumberOfNights = GetDateDifference(operationDate.To.ToDateTimeExactV1(), operationDate.From.ToDateTimeExactV1(), true);

                        if (modality?.AmountUnitType?.ToUpper().Contains("PAX") == true)
                        {
                            priceCost = UpdatePricePerPax(modality, travelInfo, rate, rateDetail, criteria, priceCost, isMandatoryApplyAmount, false, productMapping);

                            //if (isMandatoryApplyAmount)
                            //{
                            priceSell = UpdatePricePerPax(modality, travelInfo, rate, rateDetail, criteria, priceSell, isMandatoryApplyAmount, true, productMapping);
                            //}
                            //else
                            //{
                            //    priceSell = null;
                            //}
                        }
                        else if (modality?.AmountUnitType?.ToUpper().Contains("SERVICE") == true)
                        {
                            priceCost = UpdatePricePerUnit(modality, travelInfo, rate, rateDetail, criteria, priceCost, isMandatoryApplyAmount, false, productMapping);
                            //if (isMandatoryApplyAmount)
                            //{
                            priceSell = UpdatePricePerUnit(modality, travelInfo, rate, rateDetail, criteria, priceSell, isMandatoryApplyAmount, true, productMapping);
                            //}
                            //else
                            //{
                            //    priceSell = null;
                            //}
                        }
                        else
                        {
                            option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                            return option;
                        }

                        var opDate = operationDate.From.ToDateTimeExactV1();

                        if (!pAndACost.Keys.Contains(opDate) &&
                            (opDate >= _criteria.CheckinDate
                            || opDate <= _criteria.CheckoutDate
                            )
                        )
                        {
                            pAndACost.Add(opDate, priceCost);
                        }

                        if (!pAndASell.Keys.Contains(opDate) &&
                               (opDate >= _criteria.CheckinDate
                               || opDate <= _criteria.CheckoutDate
                               )
                            )
                        {
                            pAndASell.Add(opDate, priceSell);
                        }
                    }
                }
                else
                {
                    option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    return option;
                }

                #region Cancellation policies

                /*
                 You can also consider the “rateClass” value, in case of “NRF” it’s always NON refundable (so at the moment of confirmation there will be charges for cancellation), if “rateClass” value is “NOR” you need to read the “cancellationPolicies” array in order to determine when they will be applied.
                */
                if (rate.RateClass.ToUpper() == "NRF")
                {
                    option.CancellationText = CONSTANT.CancellationPolicyNonRefundable;
                }
                else if (rate.RateClass.ToUpper() == "NOR")
                {
                    var cancellationCost = new List<CancellationPrice>();
                    var servicePrice = rateDetail.TotalAmount.Amount;

                    if (operationDates?.Count > 0)
                    {
                        foreach (var operationDate in operationDates)
                        {
                            try
                            {
                                var opdate = System.Convert.ToDateTime(operationDate.From);
                                foreach (var cancellationPolicy in operationDate?.CancellationPolicies)
                                {
                                    var cancellationPrice = new CancellationPrice
                                    {
                                        Percentage = GetCancellationPricePercentage(System.Convert.ToDecimal(servicePrice)
                                                                , System.Convert.ToDecimal(cancellationPolicy.Amount)),

                                        CancellationDateRelatedToOpreationDate = opdate,
                                        CancellationFromdate = (cancellationPolicy.DateFrom).Date,
                                        CancellationToDate = System.Convert.ToDateTime(operationDate.To).Date,
                                        CancellationAmount = System.Convert.ToDecimal(cancellationPolicy.Amount)
                                    };
                                    cancellationCost.Add(cancellationPrice);
                                }
                            }
                            catch
                            {
                            }
                        }

                        UpdateCancellationPolicyText(cancellationCost, criteria, option);
                        option.CancellationPrices = cancellationCost;
                    }
                }

                #endregion Cancellation policies

                //Price and Currency
                costPrice.DatePriceAndAvailabilty = pAndACost;
                costPrice.Amount = System.Convert.ToDecimal(rateDetail.TotalAmount.Amount);
                costPrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.CostPrice = costPrice;

                //if (isMandatoryApplyAmount)
                //{
                //    sellPrice.DatePriceAndAvailabilty = pAndASell;
                //    sellPrice.Amount = System.Convert.ToDecimal(rateDetail.TotalAmount.BoxOfficeAmount);
                //    sellPrice.Currency = new Currency
                //    {
                //        Name = _currencyISOCode,
                //        IsoCode = _currencyISOCode
                //    };
                //    option.SellPrice = sellPrice;
                //}
                //else
                //{
                //    option.SellPrice = null;
                //}

                if(rateDetail.TotalAmount.BoxOfficeAmount >= rateDetail?.TotalAmount?.Amount)
                {
                    sellPrice.DatePriceAndAvailabilty = pAndASell;
                    sellPrice.Amount = System.Convert.ToDecimal(rateDetail.TotalAmount.BoxOfficeAmount);
                    sellPrice.Currency = new Currency
                    {
                        Name = _currencyISOCode,
                        IsoCode = _currencyISOCode
                    };
                    option.SellPrice = sellPrice;
                }
                else
                {
                    option.SellPrice = null;
                }

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

        #region Helper methods

        /// <summary>
        /// Get Percentage from amount
        /// </summary>
        /// <param name="totalPrice"></param>
        /// <param name="cancellationAmount"></param>
        /// <returns></returns>
        private float GetCancellationPricePercentage(decimal totalPrice, decimal cancellationAmount)
        {
            var result = 0.0F;
            try
            {
                if (totalPrice > 0)
                {
                    var p = (cancellationAmount / totalPrice) * 100;
                    float.TryParse(Math.Round(p).ToString(), out var f);
                    result = f;
                }
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO add logging
                result = 0.0F;
            }
            return result;
        }

        /// <summary>
        /// Get date difference in days. if difference is like 1.3 it will become 2
        /// </summary>
        /// <param name="toDate"></param>
        /// <param name="fromdate"></param>
        /// <param name="isRound">Value i.e 1.3 will become 2 </param>
        /// <returns></returns>
        private int GetDateDifference(DateTime toDate, DateTime fromdate, bool isRound = true)
        {
            int result = 0;
            try
            {
                var diff = (toDate - fromdate).TotalDays;
                if (isRound)
                {
                    int.TryParse(Math.Ceiling(diff).ToString(), out result);
                }
                else
                {
                    int.TryParse(diff.ToString(), out result);
                }
                return result;
            }
            catch
            {
                //ignored
                //#TODO Logging
            }
            return result;
        }

        #endregion Helper methods

        private DefaultPriceAndAvailability UpdatePricePerPax(Modality modality, TravelInfo travelInfo, Rate rate, ServiceAdapters.HB.HB.Entities.Calendar.Ratedetail rateDetail, HotelbedCriteriaApitude criteria, DefaultPriceAndAvailability price, bool isMandatoryApplyAmount, bool isSellPriceUnit, IsangoHBProductMapping productMapping)
        {
            decimal adultCostPrice = 0;
            decimal childCostPrice = 0;
            decimal infantCostPrice = 0;
            decimal youthCostPrice = 0;

            decimal adultSellPrice = 0;
            decimal childSellPrice = 0;
            decimal infantSellPrice = 0;
            decimal youthSellPrice = 0;
            price.PricingUnits = new List<PricingUnit>();

            var passengerInfos = criteria.PassengerInfo.Where(x => x.ActivityId == productMapping.IsangoHotelBedsActivityId)?.ToList();

            var servicePrice = /*isMandatoryApplyAmount &&*/ isSellPriceUnit ?
                           System.Convert.ToDecimal(rateDetail?.TotalAmount?.BoxOfficeAmount) :
                           System.Convert.ToDecimal(rateDetail?.TotalAmount?.Amount);

            //Filtration of prices based on the ages passed in input , and pax type mapping
            //Using child ages mapping to distinguish b/w in Youth, Child and Infant
            var adultPriceQuery = from m in rateDetail.PaxAmounts
                                  from pax in passengerInfos
                                  where pax.FromAge >= m.AgeFrom && pax.ToAge <= m.AgeTo
                                  && pax.PassengerTypeId == (int)PassengerType.Adult
                                  select m;

            if (!adultPriceQuery.Any())
            {
                adultPriceQuery = from m in rateDetail.PaxAmounts
                                  where m.PaxType.ToString().ToLower().Contains("adult")
                                  select m;
            }

            if (adultPriceQuery?.Any() == true)
            {
                _adultCount = 1;
            }

            var youthPriceQuery = from m in rateDetail.PaxAmounts
                                  from pax in passengerInfos
                                  where pax.FromAge >= m.AgeFrom && pax.ToAge <= m.AgeTo
                                  && pax.PassengerTypeId == (int)PassengerType.Youth
                                   && m.Amount > 0
                                  select m;

            if (youthPriceQuery?.Any() == true)
            {
                _youthCount = 1;
            }

            var childPriceQuery = from m in rateDetail.PaxAmounts
                                  from pax in passengerInfos
                                  where pax.FromAge >= m.AgeFrom && pax.ToAge <= m.AgeTo
                                  && pax.PassengerTypeId == (int)PassengerType.Child
                                   && m.Amount > 0
                                  select m;

            if (!childPriceQuery.Any())
            {
                childPriceQuery = from m in rateDetail.PaxAmounts
                                  where m.PaxType.ToString().ToLower().Contains("child")
                                  && m.Amount > 0
                                  select m;
            }

            if (childPriceQuery?.Any() == true)
            {
                _childCount = 1;
            }

            var infantPriceQuery = from m in rateDetail.PaxAmounts
                                   from pax in passengerInfos
                                   where pax.FromAge >= m.AgeFrom && pax.ToAge <= m.AgeTo
                                   && pax.PassengerTypeId == (int)PassengerType.Infant
                                    && m.Amount <= 0
                                   select m;

            if (!infantPriceQuery.Any())
            {
                infantPriceQuery = from m in rateDetail.PaxAmounts
                                   where m.PaxType.ToString().ToLower().Contains("child")
                                   && m.Amount <= 0
                                   select m;
            }

            if (infantPriceQuery?.Any() == true)
            {
                _infantCount = 1;
            }

            var adultPriceNode = adultPriceQuery?.FirstOrDefault();
            var childPriceNode = childPriceQuery?.FirstOrDefault();
            var youthPriceNode = youthPriceQuery?.FirstOrDefault();
            var infantPriceNode = infantPriceQuery?.FirstOrDefault();

            //if (isMandatoryApplyAmount)
            //{
            adultSellPrice = System.Convert.ToDecimal(adultPriceNode?.BoxOfficeAmount);
            childSellPrice = System.Convert.ToDecimal(childPriceNode?.BoxOfficeAmount);
            infantSellPrice = System.Convert.ToDecimal(infantPriceNode?.BoxOfficeAmount);
            youthSellPrice = System.Convert.ToDecimal(youthPriceNode?.BoxOfficeAmount);
            //}

            adultCostPrice = System.Convert.ToDecimal(adultPriceNode?.Amount);
            childCostPrice = System.Convert.ToDecimal(childPriceNode?.Amount);
            infantCostPrice = System.Convert.ToDecimal(infantPriceNode?.Amount);
            youthCostPrice = System.Convert.ToDecimal(youthPriceNode?.Amount);

            //if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
            if (_adultCount > 0)
            {
                var adultPriceUnit = new AdultPricingUnit
                {
                    Price = isSellPriceUnit ? adultSellPrice : adultCostPrice,
                    Quantity = _adultCount
                };
                price.PricingUnits.Add(adultPriceUnit);
            }

            //if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
            if (_childCount > 0)
            {
                var childPriceUnit = new ChildPricingUnit
                {
                    Price = isSellPriceUnit ? childSellPrice : childCostPrice,
                    Quantity = _childCount
                };
                price.PricingUnits.Add(childPriceUnit);
            }

            //if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Infant))
            if (_infantCount > 0)
            {
                var infantPriceUnit = new InfantPricingUnit
                {
                    Price = isSellPriceUnit ? infantSellPrice : infantCostPrice,
                    Quantity = _infantCount
                };
                price.PricingUnits.Add(infantPriceUnit);
            }

            if (_youthCount > 0)
            {
                var youthPriceUnit = new InfantPricingUnit
                {
                    Price = isSellPriceUnit ? youthSellPrice : youthCostPrice,
                    Quantity = _youthCount
                };
                price.PricingUnits.Add(youthPriceUnit);
            }

            if (servicePrice.Equals(0))
            {
                price.TotalPrice = (adultCostPrice * _adultCount);
                //+ (childCostPrice * _childCount)
                //+ (infantCostPrice + _infantCount);
            }
            else
                price.TotalPrice = servicePrice;

            if (price.TotalPrice <= 0)
            {
                price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }

            return price;
        }

        private DefaultPriceAndAvailability UpdatePricePerUnit(Modality modality, TravelInfo travelInfo, Rate rate, ServiceAdapters.HB.HB.Entities.Calendar.Ratedetail rateDetail, HotelbedCriteriaApitude criteria, DefaultPriceAndAvailability price, bool isMandatoryApplyAmount, bool isSellPriceUnit, IsangoHBProductMapping productMapping)
        {
            price.PricingUnits = new List<PricingUnit>();

            var servicePrice = /*isMandatoryApplyAmount &&*/ isSellPriceUnit ?
                         System.Convert.ToDecimal(rateDetail?.TotalAmount?.BoxOfficeAmount) :
                         System.Convert.ToDecimal(rateDetail?.TotalAmount?.Amount);

            var perPersonPrice = servicePrice / (_adultCount + _childCount + _youthCount);

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
            {
                var adultPriceUnit = new AdultPricingUnit
                {
                    Price = perPersonPrice,
                    Quantity = _adultCount,
                    UnitType = UnitType.PerUnit,
                };
                price.PricingUnits.Add(adultPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
            {
                var childPriceUnit = new ChildPricingUnit
                {
                    Price = perPersonPrice,
                    Quantity = _childCount,
                    UnitType = UnitType.PerUnit,
                };
                price.PricingUnits.Add(childPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Infant))
            {
                var infantPriceUnit = new InfantPricingUnit
                {
                    Price = 0,
                    Quantity = _infantCount,
                    UnitType = UnitType.PerUnit,
                };
                price.PricingUnits.Add(infantPriceUnit);
            }
            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Youth))
            {
                var infantPriceUnit = new YouthPricingUnit
                {
                    Price = 0,
                    Quantity = _youthCount,
                    UnitType = UnitType.PerUnit,
                };
                price.PricingUnits.Add(infantPriceUnit);
            }

            price.TotalPrice = servicePrice;

            if (price.TotalPrice <= 0)
            {
                price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }
            return price;
        }

        #endregion Private Methods
    }
}