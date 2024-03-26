using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using ServiceAdapters.HB.HB.Entities.ActivityDetail;
using ServiceAdapters.HB.HB.Entities.ActivityDetailFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;
using CONSTANT = Util.CommonUtilConstantCancellation;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBDetailConverter : ConverterBase, IHbDetailConverter
    {
        private string _currencyISOCode;
        private int _adultCount;
        private int _childCount;
        private int _infantCount;
        private int _totalPaxCount;
        private int _tempInt;

        private HotelbedCriteriaApitude _criteria;

        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (ActivityDetailRS)apiResponse;
            return result != null ? ConvertAvailabilityResult(result, criteria) : null;
        }

        #region Private Methods

        private Activity ConvertAvailabilityResult(ActivityDetailRS result, object criteria)
        {
            //var lstActivityRqPaxes = result.LstActivityRqPaxes;
            _criteria = criteria as HotelbedCriteriaApitude;
            var hbApiActivity = result?.Activity;
            if (_criteria == null || hbApiActivity == null)
            {
                return null;
            }
            var activity = new Activity();
            var options = new List<ActivityOption>();

            try
            {
                _currencyISOCode = hbApiActivity.CurrencyISOCode;
                _adultCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Adult)?.FirstOrDefault().Value ?? 0;
                _childCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Child)?.FirstOrDefault().Value ?? 0;
                _infantCount = _criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Infant)?.FirstOrDefault().Value ?? 0;
                _totalPaxCount = _adultCount + _childCount + _infantCount;

                int.TryParse(_criteria.IsangoActivityId, out _tempInt);
                activity.ID = _tempInt;
                activity.Name = hbApiActivity.Name;
                activity.Code = hbApiActivity.HBActivityCode;
                activity.CategoryIDs = new List<int> { 1 };

                activity.RegionName = hbApiActivity.Country?.Destinations?.FirstOrDefault() != null ? hbApiActivity.Country?.Destinations?.FirstOrDefault()?.Name : Constant.RegionName;

                #region Inclusions

                if (hbApiActivity?.Content?.FeatureGroups != null)
                {
                    var inclusions = hbApiActivity.Content.FeatureGroups.FindAll(f => f.Included != null && f.Included.Count > 0).ToList();
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

                var travelInfo = new TravelInfo
                {
                    Ages = _criteria.Ages,
                    NoOfPassengers = _criteria.NoOfPassengers,
                    NumberOfNights = 0,
                    StartDate = _criteria.CheckinDate
                };

                foreach (var modality in hbApiActivity?.Modalities)
                {
                    foreach (var rate in modality?.Rates)
                    {
                        foreach (var rateDetail in rate?.RateDetails)
                        {
                            try
                            {
                                var destinationCodeFromAPI = modality?.DestinationCode?.ToUpper() ?? hbApiActivity?.Country?.Destinations?.FirstOrDefault()?.Code;

                                if (_criteria?.Destination?.ToUpper() == destinationCodeFromAPI
                                    || string.IsNullOrWhiteSpace(_criteria?.Destination?.ToUpper())
                                )
                                {
                                    var option = CreateOption(modality: modality, travelInfo: travelInfo, rate: rate, rateDetail: rateDetail, criteria: _criteria);

                                    if (option != null)
                                    {
                                        option.Name = $"{activity.Name} - {option.Name}";

                                        if (!options.Any(x => x.Name == option.Name
                                                            && x.CostPrice.Amount == option.CostPrice.Amount
                                                        )
                                        )
                                        {
                                            //Remove expensoer option with same name
                                            var removedOptionsCount = options.RemoveAll(x => x.Name == option.Name
                                            && x.CostPrice.Amount > option.CostPrice.Amount);

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
                        }
                    }
                }

                activity.FactsheetId = System.Convert.ToInt32(hbApiActivity.Content?.ContentId);

                var availableOptions = options;//.Where(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                activity.ProductOptions = availableOptions?.Cast<ProductOption>()?.ToList();
            }
            catch (Exception ex)
            {
                //ignore
                //TODOlogging
                activity = null;
            }
            return activity;
        }

        private ActivityOption CreateOption(Modality modality, TravelInfo travelInfo, Rate rate, RateDetail rateDetail, HotelbedCriteriaApitude criteria)
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
                                                && rateDetail.TotalAmount.BoxOfficeAmount > rateDetail?.TotalAmount?.Amount;

            try
            {
                option = new ActivityOption
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Name = modality?.Name,
                    Code = modality?.Code,
                    RateKey = rateDetail?.RateKey,
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
                    IsMandatoryApplyAmount = isMandatoryApplyAmount,
                };

                if (modality?.Questions?.Count > 0)
                {
                    option.ContractQuestions = modality?.Questions?.Select(q => new ContractQuestion
                    {
                        Code = q?.Code,
                        Description = q?.Text,
                        IsRequired = q?.IsRequired ?? false,
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
                            priceCost = UpdatePricePerPax(modality, travelInfo, rate, rateDetail, criteria, priceCost, isMandatoryApplyAmount, false);

                            //if (isMandatoryApplyAmount)
                            //{
                            priceSell = UpdatePricePerPax(modality, travelInfo, rate, rateDetail, criteria, priceSell, isMandatoryApplyAmount, true);
                            //}
                            //else
                            //{
                            //    priceSell = null;
                            //}
                        }
                        else if (modality?.AmountUnitType?.ToUpper().Contains("SERVICE") == true)
                        {
                            priceCost = UpdatePricePerUnit(modality, travelInfo, rate, rateDetail, criteria, priceCost, isMandatoryApplyAmount, false);
                            //if (isMandatoryApplyAmount)
                            //{
                            priceSell = UpdatePricePerUnit(modality, travelInfo, rate, rateDetail, criteria, priceSell, isMandatoryApplyAmount, true);
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
                                if (string.IsNullOrWhiteSpace(option.ApiCancellationPolicy))
                                {
                                    option.ApiCancellationPolicy = SerializeDeSerializeHelper.Serialize(operationDate?.CancellationPolicies);
                                }
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
                sellPrice.DatePriceAndAvailabilty = pAndASell;
                sellPrice.Amount = System.Convert.ToDecimal(rateDetail.TotalAmount.BoxOfficeAmount);
                sellPrice.Currency = new Currency
                {
                    Name = _currencyISOCode,
                    IsoCode = _currencyISOCode
                };
                option.SellPrice = sellPrice;
                //}
                //else
                //{
                //    option.SellPrice = null;
                //}

                //Add Contract

                #region Add Contract and ContractComment

                option.Contract = new Isango.Entities.Contract
                {
                    Name = modality?.Contract?.Name,
                    ClassificationCode = modality?.Contract?.Code.ToString() ?? string.Empty,
                    InComingOfficeCode = "N/A"// t.Contract.IncomingOffice.Code
                };
                option.Contract.Comments = new List<Isango.Entities.HotelBeds.Comment>();

                foreach (var comments in modality?.Comments)
                {
                    var comment = new Isango.Entities.HotelBeds.Comment
                    {
                        CommentText = comments?.Text,
                        Type = comments?.Type,
                    };
                    option.Contract.Comments.Add(comment);
                }

                #endregion Add Contract and ContractComment

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

        private DefaultPriceAndAvailability UpdatePricePerPax(Modality modality, TravelInfo travelInfo, Rate rate, RateDetail rateDetail, HotelbedCriteriaApitude criteria, DefaultPriceAndAvailability price, bool isMandatoryApplyAmount, bool isSellPriceUnit)
        {
            decimal defaultMargin = 20M;

            decimal adultCostPrice = 0;
            decimal childCostPrice = 0;
            decimal infantCostPrice = 0;

            decimal adultSellPrice = 0;
            decimal childSellPrice = 0;
            decimal infantSellPrice = 0;
            price.PricingUnits = new List<PricingUnit>();

            var servicePrice = isSellPriceUnit ?
                         System.Convert.ToDecimal(rateDetail?.TotalAmount?.BoxOfficeAmount) :
                         System.Convert.ToDecimal(rateDetail?.TotalAmount?.Amount);

            if (isSellPriceUnit && rateDetail?.TotalAmount?.BoxOfficeAmount <= rateDetail?.TotalAmount?.Amount)
            {
                servicePrice = ApplyDefaultMargin(System.Convert.ToDecimal(rateDetail.TotalAmount.Amount));
            }

            //Filtration of prices based on the ages passed in input , and pax type mapping
            //Using child ages mapping to distinguish b/w in Youth, Child and Infant
            var adultPriceQuery = from m in rateDetail.PaxAmounts
                                  from pax in travelInfo.Ages
                                  where pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                  && pax.Key == PassengerType.Adult
                                  && m.PaxType?.ToLowerInvariant() == PassengerType.Adult.ToString().ToLowerInvariant()
                                  select m;

            var childPriceQuery = from m in rateDetail.PaxAmounts
                                  from pax in travelInfo.Ages
                                  where pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                  && pax.Key == PassengerType.Child
                                  && m.PaxType?.ToLowerInvariant() == PassengerType.Child.ToString().ToLowerInvariant()
                                  select m;

            if (childPriceQuery?.Any() == false)
            {
                childPriceQuery = from m in rateDetail.PaxAmounts
                                  from pax in travelInfo.Ages
                                  where pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                  && pax.Key == PassengerType.Child
                                  //&& m.PaxType?.ToLowerInvariant() == PassengerType.Adult
                                  //                                      .ToString().ToLowerInvariant()

                                  select new Paxamount
                                  {
                                      AgeFrom = m.AgeFrom,
                                      AgeTo = m.AgeTo,
                                      Amount = m.Amount,
                                      BoxOfficeAmount = m.BoxOfficeAmount,
                                      MandatoryApplyAmount = m.MandatoryApplyAmount,
                                      PaxType = PassengerType.Child.ToString().ToUpperInvariant()
                                  };
            }

            var infantPriceQuery = from m in rateDetail.PaxAmounts
                                   from pax in travelInfo.Ages
                                   where pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo
                                   && pax.Key == PassengerType.Infant
                                   //Infant from api comes as child and can be verified only by its age
                                   && m.PaxType?.ToLowerInvariant() == PassengerType.Child.ToString().ToLowerInvariant()
                                   select m;

            var adultPriceNode = adultPriceQuery?.FirstOrDefault();
            var childPriceNode = childPriceQuery?.FirstOrDefault();
            var infantPriceNode = infantPriceQuery?.FirstOrDefault();

            adultCostPrice = System.Convert.ToDecimal(adultPriceNode?.Amount);
            childCostPrice = System.Convert.ToDecimal(childPriceNode?.Amount);
            infantCostPrice = System.Convert.ToDecimal(infantPriceNode?.Amount);

            //if (isSellPriceUnit)
            //{
            adultSellPrice = System.Convert.ToDecimal(adultPriceNode?.BoxOfficeAmount);
            childSellPrice = System.Convert.ToDecimal(childPriceNode?.BoxOfficeAmount);
            infantSellPrice = System.Convert.ToDecimal(infantPriceNode?.BoxOfficeAmount);

            if (adultSellPrice == 0 || adultSellPrice < adultCostPrice)
            {
                adultSellPrice = ApplyDefaultMargin(System.Convert.ToDecimal(adultCostPrice), criteria?.ActivityMargin ?? defaultMargin);
            }

            if (childSellPrice == 0 || childSellPrice < childCostPrice)
            {
                childSellPrice = ApplyDefaultMargin(System.Convert.ToDecimal(childCostPrice), criteria?.ActivityMargin ?? defaultMargin);
            }

            if (infantSellPrice == 0 || infantSellPrice < infantCostPrice)
            {
                infantSellPrice = ApplyDefaultMargin(System.Convert.ToDecimal(infantCostPrice), criteria?.ActivityMargin ?? defaultMargin);
            }
            //}
            //else
            //{
            //}

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
            {
                var adultPriceUnit = new AdultPricingUnit
                {
                    Price = isSellPriceUnit ? adultSellPrice : adultCostPrice,
                    Quantity = _adultCount
                };
                price.PricingUnits.Add(adultPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
            {
                var childPriceUnit = new ChildPricingUnit
                {
                    Price = isSellPriceUnit ? childSellPrice : childCostPrice,
                    Quantity = _childCount
                };
                price.PricingUnits.Add(childPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Infant))
            {
                var infantPriceUnit = new InfantPricingUnit
                {
                    Price = isSellPriceUnit ? infantSellPrice : infantCostPrice,
                    Quantity = _infantCount
                };
                price.PricingUnits.Add(infantPriceUnit);
            }

            if (servicePrice.Equals(0))
            {
                price.TotalPrice = (adultCostPrice * _adultCount)
                    + (childCostPrice * _childCount)
                    + (infantCostPrice + _infantCount);
            }
            else
                price.TotalPrice = servicePrice;

            if (price.TotalPrice <= 0)
            {
                price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }

            return price;
        }

        private DefaultPriceAndAvailability UpdatePricePerUnit(Modality modality, TravelInfo travelInfo, Rate rate, RateDetail rateDetail, HotelbedCriteriaApitude criteria, DefaultPriceAndAvailability price, bool isMandatoryApplyAmount, bool isSellPriceUnit)
        {
            decimal adultCostPrice = 0;
            decimal childCostPrice = 0;
            decimal infantCostPrice = 0;
            price.PricingUnits = new List<PricingUnit>();

            var servicePrice = /*isMandatoryApplyAmount &&*/ isSellPriceUnit ?
                         System.Convert.ToDecimal(rateDetail?.TotalAmount?.BoxOfficeAmount) :
                         System.Convert.ToDecimal(rateDetail?.TotalAmount?.Amount);

            if (isSellPriceUnit && rateDetail?.TotalAmount?.BoxOfficeAmount <= rateDetail?.TotalAmount?.Amount)
            {
                servicePrice = ApplyDefaultMargin(System.Convert.ToDecimal(rateDetail.TotalAmount.Amount));
            }
            //Filtration of prices based on the ages passed in input , and pax type mapping
            //Using child ages mapping to distinguish b/w in Youth, Child and Infant
            //var adultPriceQuery = from m in rateDetail.PaxAmounts
            //                  from pax in travelInfo.Ages
            //                  where
            //                   //pax.Value >= m.AgeFrom && pax.Value <= m.AgeTo &&
            //                   pax.Key == PassengerType.Adult
            //                   && m.PaxType?.ToLowerInvariant()
            //                         == PassengerType.Adult.ToString().ToLowerInvariant()
            //                  select m.Amount;

            var perPersonPrice = servicePrice / _totalPaxCount;

            adultCostPrice = perPersonPrice;
            childCostPrice = perPersonPrice;
            infantCostPrice = perPersonPrice;

            //this code can be used to implement perunit pricing upto final response
            // commenting it as it required further handling in sitecore and then booking

            //var pricingUnit = new PerUnitPricingUnit
            //{
            //    Price = unitPrice,
            //    PriceType = PriceType.PerUnit,
            //    Quantity = 1,
            //    TotalCapacity = _totalPaxCount,
            //    UnitType = UnitType.PerUnit,
            //};
            //price.PricingUnits.Add(pricingUnit);

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
            {
                var adultPriceUnit = new AdultPricingUnit
                {
                    Price = perPersonPrice,
                    Quantity = _adultCount
                };
                price.PricingUnits.Add(adultPriceUnit);
            }

            if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
            {
                var childPriceUnit = new ChildPricingUnit
                {
                    Price = perPersonPrice,
                    Quantity = _childCount
                };
                price.PricingUnits.Add(childPriceUnit);
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

            if (servicePrice.Equals(0))
            {
                price.TotalPrice = (adultCostPrice * _adultCount)
                    + (childCostPrice * _childCount)
                    + (infantCostPrice + _infantCount);
            }
            else
                price.TotalPrice = servicePrice;

            if (price.TotalPrice <= 0)
            {
                price.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }
            return price;
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

        #endregion Private Methods
    }
}