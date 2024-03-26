using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Rezdy;
using ServiceAdapters.Rezdy.Constants;
using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities.Availability;
using Util;
using CONSTANT = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class GetAvailabilityConverter : ConverterBase, IGetAvailabilityConverter
    {
        public override object Convert<T>(string response, T request)
        {
            var input = request as RezdyCriteria;
            var result = SerializeDeSerializeHelper.DeSerialize<AvailabilityResponse>(response.ToString());
            return result == null ? null : ConvertAvailabilityResult(result, input, null);
        }

        public override object Convert<T, A>(string response, T request, A apiResponse)
        {
            var input = request as RezdyCriteria;
            var rezdyProduct = apiResponse as RezdyProduct;
            var result = SerializeDeSerializeHelper.DeSerialize<AvailabilityResponse>(response.ToString());
            return result == null ? null : ConvertAvailabilityResult(result, input, rezdyProduct);
        }

        private List<ProductOption> ConvertAvailabilityResult(AvailabilityResponse productResponse, RezdyCriteria criteria, RezdyProduct rezdyProduct)
        {
            var langauge = criteria?.Language?.ToLower() ?? "en";
            var productOptions = new List<ProductOption>();
            var sessions = productResponse.Sessions.Where(
                  x => System.Convert.ToDateTime(x.StartTimeLocal) >= criteria.CheckinDate)
                       .OrderBy(x => x.StartTimeLocal)
                      .ToList();
            if (string.IsNullOrEmpty(criteria.Dumping))
            {
                sessions = productResponse.Sessions.Where(
                  x => System.Convert.ToDateTime(x.StartTimeLocal) >= criteria.CheckinDate
                       && System.Convert.ToDateTime(x.EndTimeLocal) <= criteria.CheckoutDate)
                       .OrderBy(x => x.StartTimeLocal)
                      .ToList();
            }
            var optionOrder = 1;

            foreach (var session in sessions)
            {
                var productOptionsPerSession = new List<ProductOption>();
                var pricingUnits = new List<PricingUnit>();

                var sessionData = session.PriceOptions;
                var sessionOptionsWithoutChild = sessionData;
                var checkGroupProduct = session.PriceOptions.FirstOrDefault().Label.ToLower().Contains("group");
                if (checkGroupProduct)
                {
                    sessionOptionsWithoutChild = sessionOptionsWithoutChild.Where(x=>x.Label.ToLower()!="child").ToArray();
                }
                
                foreach (var priceOption in sessionOptionsWithoutChild)
                {
                    var startTimeLocal = System.Convert.ToDateTime(session.StartTimeLocal);
                    
                    var productOptionId = criteria.RezdyPaxMappings
                        .FirstOrDefault(x => x.AgeGroupCode.ToLowerInvariant() == priceOption.Label.ToLowerInvariant()
                        &&
                       criteria.NoOfPassengers.Keys.Select(y => y.ToString().ToLower()).Contains(x.PassengerType.ToString().ToLower()) &&
                        x.SupplierCode.ToLowerInvariant() == priceOption.ProductCode.ToLowerInvariant())?.ServiceOptionId;

                    if (productOptionId == null) continue;

                    if (productOptionsPerSession.Any(x => x.ServiceOptionId == productOptionId))
                    {
                        var productOption = productOptionsPerSession.FirstOrDefault(x => x.ServiceOptionId == productOptionId);
                        var pricingUnit = CreatePricingUnit(priceOption, criteria, rezdyProduct);
                        if (pricingUnit == null)
                            continue;
                        var datePriceAvailability = productOption.BasePrice.DatePriceAndAvailabilty[startTimeLocal.Date];

                        datePriceAvailability.PricingUnits.Add(pricingUnit);
                        datePriceAvailability.TotalPrice = datePriceAvailability.PricingUnits.Sum(x => x.Price);
                    }
                    else
                    {
                        var sessionStartTimeLocal = DateTime.Parse(session.StartTimeLocal);
                        var sessionEndTimeLocal = DateTime.Parse(session.EndTimeLocal);
                        //if dates are same then show starttime and end time otherwise only show starttime.
                        var finalOptionName = sessionStartTimeLocal.Date == sessionEndTimeLocal.Date ? sessionStartTimeLocal.ToString("hh:mm tt") + " - " + sessionEndTimeLocal.ToString("hh:mm tt") : sessionStartTimeLocal.ToString("hh:mm tt");
                        var productOption = new ActivityOption
                        {
                            Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                            ServiceOptionId = System.Convert.ToInt32(productOptionId),
                            Name = finalOptionName,
                            SupplierName = criteria.SupplierName,
                            SupplierOptionCode = session.ProductCode,
                            TravelInfo = new TravelInfo
                            {
                                StartDate = startTimeLocal,
                                NoOfPassengers = criteria.NoOfPassengers,
                                Ages = criteria.Ages
                            },
                            AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                            ProductType = criteria.ProductType,
                            Seats = System.Convert.ToInt32(session.Seats),
                            SeatsAvailable = System.Convert.ToInt32(session.SeatsAvailable),
                            AllDay = System.Convert.ToBoolean(session.AllDay),
                            StartLocalTime = System.Convert.ToDateTime(session.StartTimeLocal),
                            EndLocalTime = System.Convert.ToDateTime(session.EndTimeLocal),
                            StartTime = System.Convert.ToDateTime(session.StartTimeLocal).TimeOfDay,
                            EndTime = System.Convert.ToDateTime(session.EndTimeLocal).TimeOfDay,
                            OptionOrder = optionOrder,
                            CommisionPercent = criteria.IsCommissionPercent == true ? criteria.CommissionPercent : 0
                        };

                        //Set cancellation policy text on the basis of data available in product.
                        var cancellationDays = System.Convert.ToInt32(rezdyProduct.CancellationPolicyDays);
                        if (cancellationDays == 0)
                        {
                            productOption.Cancellable = true;
                            productOption.CancellationText = RESOURCEMANAGER.GetString(langauge, CONSTANT.CancellationPolicyFreeBeforeTravelDate);
                        }
                        else if (cancellationDays > 0)
                        {
                            var cancellationHours = cancellationDays * 24;
                            productOption.Cancellable = true;
                            productOption.CancellationText = (cancellationHours) <= 72
                                ? string.Format(
                                                RESOURCEMANAGER.GetString(langauge, CONSTANT.CancellationPolicy100ChargableBeforeNhours)
                                                , cancellationHours
                                                , cancellationHours
                                                )
                                : string.Format(
                                                RESOURCEMANAGER.GetString(langauge, CONSTANT.CancellationPolicy100ChargableBeforeNhours)
                                                , Constant.SeventyTwoHours
                                                , Constant.SeventyTwoHours
                                                );
                        }

                        productOption.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(new
                        {
                            rezdyProduct?.CancellationPolicyDays
                        });

                        


                        var pricingUnit = CreatePricingUnit(priceOption, criteria, rezdyProduct);
                        

                        if (pricingUnit == null)
                            continue;

                        pricingUnits.Add(pricingUnit);

                        if (checkGroupProduct)
                        {
                            var childPriceOptions = session.PriceOptions.Where(x => x.Label.ToLower() == "child").FirstOrDefault();
                            if (pricingUnit != null && childPriceOptions != null)
                            {
                                var childPriceData = CreatePricingUnit(childPriceOptions, criteria, rezdyProduct);
                                if(childPriceData != null)
                                {
                                    //Add child node with Adult
                                    pricingUnits.Add(childPriceData);
                                }
                            }
                        }

                        var basePriceAndAvailability = new DefaultPriceAndAvailability
                        {
                            AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                            TotalPrice = pricingUnits.Count > 0 ? pricingUnits.Sum(x => x.Price) : 0,
                            PricingUnits = pricingUnits
                        };

                        productOption.BasePrice = new Price
                        {
                            Amount = basePriceAndAvailability.TotalPrice,
                            //Currency = new Currency { IsoCode = criteria.Currency },
                            Currency = new Currency { IsoCode = rezdyProduct.Currency },
                            DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                            {
                               {(System.Convert.ToDateTime(session.StartTimeLocal)).Date, basePriceAndAvailability}
                            },
                        };

                        productOptionsPerSession.Add(productOption);
                        optionOrder++;
                    }
                }

                productOptions.AddRange(productOptionsPerSession);
            }
            productOptions.ForEach(x => x.GateBasePrice = x.BasePrice.DeepCopy());
            productOptions.OrderBy(x => x.TravelInfo.StartDate);
            return productOptions;
        }

        private PricingUnit CreatePricingUnit(Entities.Availability.PriceOption priceOption, RezdyCriteria criteria, RezdyProduct rezdyProduct)
        {
            try
            {

                var minQty = System.Convert.ToInt32(priceOption?.MinQuantity);
                var maxQty = System.Convert.ToInt32(priceOption?.MaxQuantity);
                var totalPassengers = criteria?.NoOfPassengers?.Sum(x => x.Value);


                string total = "total";
                var passengerType = criteria.RezdyPaxMappings.FirstOrDefault(x => x.AgeGroupCode.ToLowerInvariant().Equals(priceOption.Label.ToLowerInvariant())
                && x.SupplierCode.ToLowerInvariant().Equals(priceOption.ProductCode.ToLowerInvariant()))?.PassengerType ?? PassengerType.Undefined;

                if (passengerType == PassengerType.Undefined) return null;

                var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);

                var groupHaveChildAlso = criteria?.NoOfPassengers?.FirstOrDefault(x => x.Key.Equals(PassengerType.Child)).Value;
                //check adult group with child case
                if (minQty > 0 && maxQty > 0 && groupHaveChildAlso > 0 && passengerType==PassengerType.Adult)
                {
                    if (totalPassengers > groupHaveChildAlso)
                    {
                        totalPassengers = totalPassengers - System.Convert.ToInt32(groupHaveChildAlso);
                    }

                }


                if (priceOption?.PriceGroupType != null && priceOption.PriceGroupType.ToLower() == total)
                {
                    pricingUnit.Price = System.Convert.ToDecimal(System.Convert.ToDecimal(Double.Parse(priceOption?.Price)) / totalPassengers);
                    pricingUnit.UnitType = UnitType.PerUnit;
                    pricingUnit.PriceType = PriceType.PerUnit;
                    if (rezdyProduct.QuantityRequired == true)
                    {
                        pricingUnit.TotalCapacity = maxQty;
                        pricingUnit.Mincapacity = minQty;
                    }
                }
                else
                {
                    pricingUnit.Price = System.Convert.ToDecimal(Double.Parse(priceOption?.Price));
                    if (rezdyProduct?.QuantityRequired == true)
                    {
                        pricingUnit.TotalCapacity = System.Convert.ToInt32(maxQty != 0 ? maxQty : rezdyProduct?.QuantityRequiredMax);
                        pricingUnit.Mincapacity = minQty != 0 ? minQty : rezdyProduct?.QuantityRequiredMin;
                    }
                }
                if (!String.IsNullOrEmpty(priceOption.SeatsUsed))
                {
                    pricingUnit.Quantity = System.Convert.ToInt32(priceOption.SeatsUsed);
                }
                if (pricingUnit is PerPersonPricingUnit perPersonPricingUnit)
                {
                    var ageGroupId = criteria.PassengerAgeGroupIds?.FirstOrDefault(x => x.Key.Equals(passengerType)).Value ?? 0;
                    perPersonPricingUnit.AgeGroupId = ageGroupId;
                }
                if (string.IsNullOrEmpty(criteria.Dumping)) // Dumping Application
                {
                    return pricingUnit;
                }
                //ignore  options that not lies in the below condition.
                else if (maxQty == 0 && minQty == 0 || totalPassengers >= minQty && totalPassengers <= maxQty)
                {
                    return pricingUnit;
                }
            }
            catch (Exception ex)
            {
                //ignore
            }


            return null;
        }
    }
}