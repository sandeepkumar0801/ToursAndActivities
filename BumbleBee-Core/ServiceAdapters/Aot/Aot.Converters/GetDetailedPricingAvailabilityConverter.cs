using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Aot;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using Constant = ServiceAdapters.Aot.Constants.Constant;
using CONSTANTS = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class GetDetailedPricingAvailabilityConverter : ConverterBase, IGetDetailedPricingAvailabilityConverter
    {
        public GetDetailedPricingAvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response Option Stay Pricing Request call</param>
        /// <returns></returns>
        public override object Convert<T>(T objectresultDict, T inputRequest, T request)
        {
            object res = null;
            var resultDict = objectresultDict as Dictionary<DateTime, OptionStayPricingResponse>;
            if (resultDict?.Any(x => x.Value?.OptStayResults?.Count > 0) == true)
            {
                res = ConvertAvailibilityResult(request as OptionAvailResponse, inputRequest as AotCriteria, resultDict);
            }
            return res;
        }

        private List<Activity> ConvertAvailibilityResult(OptionAvailResponse apiResponseBulk
            , AotCriteria criteria
            , Dictionary<DateTime
            , OptionStayPricingResponse> productDetailedPricingDict
            )
        {
            var activities = new List<Activity>();
            //var cancellationPolicyText = GetCancellationPolicy(productDetailedPricingDict, criteria);

            ////To be saved in original cancellation policy field for api
            //var cancellationPolicyHours = GetCancellationHours(productDetailedPricingDict, criteria);

            var activity = new Activity();
            if (criteria.CancellationPolicy)
            {
                activities.Add(activity);
                return activities;
            }

            var optAvail = apiResponseBulk.OptAvail?.FirstOrDefault();

            var travelInfo = new TravelInfo();

            if (optAvail != null)
            {
                activity.ID = criteria.ActivityId;
                activity.FactsheetId = 0;
                activity.Code = optAvail.Opt;
                activity.ApiType = APIType.Aot;
                activity.RegionName = string.Empty;
                activity.Inclusions = string.Empty;
                activity.Exclusions = string.Empty;
            }
            var supplierOptionCodes = apiResponseBulk?.OptAvail?.Select(x => x.Opt).Distinct().ToList();

            activity.ProductOptions = new List<ProductOption>();
            if (supplierOptionCodes?.Count > 0)
            {
                foreach (var supplierOptionCode in supplierOptionCodes)
                {
                    var optionValue = apiResponseBulk.OptAvail.FirstOrDefault(x => x.Opt == supplierOptionCode);
                    var option = CreateOption(optionValue, criteria, productDetailedPricingDict);

                    if (option != null)
                    {
                        activity.ProductOptions.Add(option);
                    }
                }
            }
            activities.Add(activity);
            return activities;
        }

        private string GetCancellationPolicy(Dictionary<DateTime, OptionStayPricingResponse> productDetailedPricingDict, AotCriteria criteria = null)
        {
            var result = string.Empty;
            try
            {
                var cancellationHours = GetCancellationHours(productDetailedPricingDict, criteria);
                var languageCode = criteria.Language ?? "en";
                var date = productDetailedPricingDict.OrderBy(x => x.Key).FirstOrDefault().Key;
                if (cancellationHours > 0)
                {
                    //System.Double.TryParse(firstProductDetailedPricing.OptStayResults.OrderBy(x => x.CancelHours)?.FirstOrDefault()?.CancelHours, out var cancellationHours);
                    var cancellationUptoDate = date.AddDays(-(cancellationHours / 24));

                    if (cancellationUptoDate < DateTime.Now.Date)
                    {
                        result = RESOURCEMANAGER.GetString(languageCode, CONSTANTS.CancellationPolicyNonRefundable);
                    }
                    else
                    {
                        result = string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANTS.CancellationPolicy100ChargableBeforeNhours), cancellationHours, cancellationHours);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        private double GetCancellationHours(Dictionary<DateTime, OptionStayPricingResponse> productDetailedPricingDict, AotCriteria criteria = null)
        {
            double cancellationHours = 0;
            try
            {
                var firstProductDetailedPricing = productDetailedPricingDict?.OrderBy(x => x.Key)?.FirstOrDefault().Value;
                System.Double.TryParse(firstProductDetailedPricing?.OptStayResults?.OrderBy(x => x.CancelHours)?.FirstOrDefault()?.CancelHours, out cancellationHours);
            }
            catch (Exception)
            {
                throw;
            }
            return cancellationHours;
        }

        private TravelInfo GetTravelInfoDetails(TravelInfo travelInfo, AotCriteria criteria)
        {
            travelInfo.NumberOfNights = 0;
            travelInfo.NoOfPassengers = new Dictionary<PassengerType, int>
            {
                { PassengerType.Adult, criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Adult && x.Value != 0).Value },
                { PassengerType.Child, criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Child && x.Value != 0).Value },
                { PassengerType.Infant, criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Infant && x.Value != 0).Value }
            };
            return travelInfo;
        }

        private ActivityOption CreateOption(OptAvail apiResponseBulkItem, AotCriteria criteria, Dictionary<DateTime, OptionStayPricingResponse> productDetailedPricingDict)
        {
            var languageCode = criteria?.Language ?? "en";
            try
            {
                var adultCount = criteria?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Adult).Value ?? 0;
                var childCount = criteria?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Child).Value ?? 0;
                var infantCount = criteria?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Infant).Value ?? 0;
                var paxCountWithoutInfant = adultCount + childCount;

                var detailedPricingQuery = from detail in productDetailedPricingDict
                                           from optStayResult in detail.Value.OptStayResults
                                           where optStayResult.Opt == apiResponseBulkItem.Opt
                                           select new KeyValuePair<DateTime, OptStayResults>(detail.Key, optStayResult);

                if (detailedPricingQuery?.Any() != true)
                {
                    return null;
                }

                var detailedPricings = detailedPricingQuery.OrderBy(y => y.Key).ToDictionary(x => x.Key, x => x.Value);
                var firstProductDetPricing = detailedPricings.FirstOrDefault();

                var option = new ActivityOption
                {
                    SupplierOptionCode = apiResponseBulkItem.Opt,

                    RoomType = !string.IsNullOrEmpty(firstProductDetPricing.Value.RoomType) ? firstProductDetPricing.Value.RoomType : "",

                    ServiceType = apiResponseBulkItem.Opt.Substring(3, 2),
                    Code = apiResponseBulkItem.Opt,
                    TravelInfo = new TravelInfo
                    {
                        Ages = criteria.Ages,
                        NoOfPassengers = criteria.NoOfPassengers,
                        StartDate = firstProductDetPricing.Key,
                        NumberOfNights = 0
                    }
                };

                var priceAndAvailabiltyCost = new Dictionary<DateTime, PriceAndAvailability>();

                foreach (var detailedPricing in detailedPricings)
                {
                    var date = detailedPricing.Key;
                    var dateProductDetailPricing = detailedPricing.Value;

                    foreach (var optRate in apiResponseBulkItem.OptRates.Rates.OptRate)
                    {
                        decimal minAdultPrice = 0;
                        decimal minChildPrice = 0;
                        const decimal minInfantPrice = 0;
                        var totalCostPrice = GetPrice(dateProductDetailPricing.TotalPrice);
                        var perPersonPriceExcludingInfant = totalCostPrice / paxCountWithoutInfant;

                        #region Set Availability Status

                        var availabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                        switch (dateProductDetailPricing.Availability)
                        {
                            case Constant.Ok:
                                availabilityStatus = AvailabilityStatus.AVAILABLE;
                                break;

                            case Constant.Rq:
                                availabilityStatus = AvailabilityStatus.ONREQUEST;
                                break;

                            case Constant.No:
                                availabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                break;
                        }
                        var priceAndAvailability = new DefaultPriceAndAvailability
                        {
                            AvailabilityStatus = availabilityStatus
                        };

                        #endregion Set Availability Status

                        if (optRate.PersonRates != null)
                        {
                            minAdultPrice = GetPrice(optRate.PersonRates.AdultRates.AdultRate);
                            minChildPrice = GetPrice(optRate.PersonRates.ChildRate);
                            option.OptionType = Constant.PaxBased;
                        }
                        else if (optRate.RoomRates != null)
                        {
                            minAdultPrice = perPersonPriceExcludingInfant;
                            minChildPrice = perPersonPriceExcludingInfant;
                            option.OptionType = Constant.RoomBased;
                        }
                        else if (optRate.ExtrasRates != null)
                        {
                            minAdultPrice = GetPrice(optRate.ExtrasRates.ExtrasRate.FirstOrDefault()?.AdultRate);
                            minChildPrice = GetPrice(optRate.ExtrasRates.ExtrasRate.FirstOrDefault()?.ChildRate);
                        }
                        else if (optRate.OptionRates != null)
                        {
                            minAdultPrice = perPersonPriceExcludingInfant;
                            minChildPrice = perPersonPriceExcludingInfant;
                            option.OptionType = Constant.GroupBased;
                        }

                        var unitType = UnitType.PerPerson;

                        if (option.OptionType != Constant.PaxBased)
                        {
                            unitType = UnitType.PerUnit;
                        }

                        priceAndAvailability.PricingUnits = new List<PricingUnit>
                        {
                            CreatePricingUnit(minAdultPrice, unitType, adultCount, PassengerType.Adult)
                        };

                        if (childCount > 0)
                        {
                            priceAndAvailability.PricingUnits.Add(CreatePricingUnit(minChildPrice, unitType, childCount, PassengerType.Child));
                        }
                        if (infantCount > 0)
                        {
                            priceAndAvailability.PricingUnits.Add(CreatePricingUnit(minInfantPrice, unitType, infantCount, PassengerType.Infant));
                        }

                        priceAndAvailability.TotalPrice = totalCostPrice;

                        if (!priceAndAvailabiltyCost.Keys.Contains(date))
                            priceAndAvailabiltyCost.Add(date, priceAndAvailability);
                    }

                    option.CostPrice = new Price
                    {
                        Amount = priceAndAvailabiltyCost.FirstOrDefault().Value.TotalPrice,
                        Currency = new Currency { IsoCode = firstProductDetPricing.Value.Currency, IsPostFix = true, Name = "", Symbol = "" },
                        DatePriceAndAvailabilty = priceAndAvailabiltyCost
                    };
                }

                option.AvailabilityStatus = option?.CostPrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value?.AvailabilityStatus ?? AvailabilityStatus.NOTAVAILABLE;

                var cancellationApplicableFrom = criteria.CheckinDate.AddHours(-System.Convert.ToInt32(apiResponseBulkItem.OptRates.CancelHours));
                

                //Cancellation and non cancellation text to be created from first object in detailedPricings object above.
                if (!string.IsNullOrEmpty(firstProductDetPricing.Value.CancelHours))
                {
                    var cancellationHours = System.Convert.ToInt32(firstProductDetPricing.Value.CancelHours);
                    option.Cancellable = true;
                    option.CancellationText = string.Format(RESOURCEMANAGER.GetString(languageCode, CONSTANTS.CancellationPolicy100ChargableBeforeNhours), cancellationHours, cancellationHours);

                    option.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(new
                    {
                        firstProductDetPricing.Value.CancelHours,
                    });
                }

                option.CancellationPrices = new List<CancellationPrice>
                {
                    new CancellationPrice()
                    {
                        CancellationFromdate = cancellationApplicableFrom,
                        CancellationDateRelatedToOpreationDate = criteria.CheckinDate,
                        CancellationToDate =  criteria.CheckinDate,
                        Percentage = cancellationApplicableFrom <= criteria.CheckinDate ? 100:0,
                        CancellationDescription = option.CancellationText
                    }
                };
                
                return option;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "Aot.GetDetailedPricingAvailabilityConverter",
                    MethodName = "CreateOption"
                };
                _logger.Error(isangoErrorEntity, ex);
            }
            return null;
        }

        private PricingUnit CreatePricingUnit(decimal price, UnitType unitType, int quantity, PassengerType passengerType)
        {
            var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
            if (pricingUnit != null)
            {
                pricingUnit.UnitType = unitType;
                pricingUnit.Price = price;
                pricingUnit.Quantity = quantity;
            }
            return pricingUnit;
        }

        private decimal GetPrice(string price)
        {
            try
            {
                decimal.TryParse(price, out var result);
                return result / 100;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "Aot.GetDetailedPricingAvailabilityConverter",
                    MethodName = "GetPrice",
                    Params = $"price {price} ",
                    Token = "aotError",
                };
                _logger.Error(isangoErrorEntity, ex);
            }
            return 0;
        }
    }
}