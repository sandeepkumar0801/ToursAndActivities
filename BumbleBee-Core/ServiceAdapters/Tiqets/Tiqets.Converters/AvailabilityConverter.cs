using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Converters.Contracts;
using System.Globalization;
using CONSTANT = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.Tiqets.Tiqets.Converters
{
    public class AvailabilityConverter : ConverterBase, IAvailabilityConverter
    {
        public AvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(T objectResult, object input)
        {
            var dateVariantsDict = objectResult as Dictionary<DateTime, List<ProductVariant>>;
            var criteria = input as TiqetsCriteria;
            var availabilityList = ConvertAvailabilityResult(dateVariantsDict, criteria);
            return availabilityList;
        }

        #region "Private Methods"

        /// <summary>
        /// This method maps the API response to iSango Contracts objects.
        /// </summary>
        /// <returns>Isango.Contracts.Entities.Supplier Object</returns>
        private object ConvertAvailabilityResult(Dictionary<DateTime, List<ProductVariant>> dateVariantsDict, TiqetsCriteria criteria)
        {
            if (criteria?.TiqetsPaxMappings == null) return null;

            var dateTimeOffset = DateTimeOffset.Parse(criteria.CheckinDate.ToString(CultureInfo.InvariantCulture), null);
            TimeSpan.TryParse(criteria?.TimeSlot, out var startTimeSlot);
            var option = new ActivityOption
            {
                Id = string.IsNullOrEmpty(criteria.TimeSlot) ? criteria.OptionId : Math.Abs(Guid.NewGuid().GetHashCode()), //Generate new id if product has TimeSlots

                Name = criteria.OptionName,

                ServiceOptionId = criteria.OptionId,
                TravelInfo = new TravelInfo
                {
                    Ages = criteria.Ages,
                    NoOfPassengers = criteria.NoOfPassengers,
                    StartDate = criteria.CheckinDate
                },
                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                Quantity = dateVariantsDict.SelectMany(e => e.Value).FirstOrDefault()?.MaxTickets ?? 0,//Maximum number of tickets available
                StartTime = startTimeSlot,
                RequiresVisitorsDetails = dateVariantsDict?.FirstOrDefault().Value?.FirstOrDefault()?.RequiresVisitorsDetails,
            };
            //var getListRequiresVisitorsVariant = new List<ProductVariantIdName>(); 
            //foreach (var item in dateVariantsDict?.FirstOrDefault().Value)
            //{
            //    foreach (KeyValuePair<int, List<string>> entry in item?.RequiresVisitorsDetailsWithVariant)
            //    {
            //        var itemData = new ProductVariantIdName();
            //        itemData.Id = entry.Key;
            //        itemData.ProductVariantName = entry.Value;
            //        getListRequiresVisitorsVariant.Add(itemData);
            //    }
            //}
            //if (getListRequiresVisitorsVariant != null && getListRequiresVisitorsVariant.Count>0)
            //{
            //    option.RequiresVisitorsDetailsWithVariant = getListRequiresVisitorsVariant;
            //}
            try
            {
                if (dateVariantsDict != null && dateVariantsDict.Count > 0)
                {
                    //Section for making cancellation policy text
                    SetCancellationPolicyText(dateVariantsDict, criteria, option);
                }

                var basePrice = new Price
                {
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>(),
                    Currency = new Currency { IsoCode = Constant.CurrencyCode }
                };

                var costPrice = new Price
                {
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>(),
                    Currency = new Currency { IsoCode = Constant.CurrencyCode }
                };
                var totalCapacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
                foreach (var variants in dateVariantsDict)
                {
                    var totalBasePrice = 0M;
                    var totalCostPrice = 0M;

                    var basePriceAndAvailability = new DefaultPriceAndAvailability
                    {
                        Capacity = variants.Value.FirstOrDefault()?.MaxTickets ?? 0,
                        IsCapacityCheckRequired = true
                    };

                    var costPriceAndAvailability = new DefaultPriceAndAvailability
                    {
                        Capacity = variants.Value.FirstOrDefault()?.MaxTickets ?? 0,
                        IsCapacityCheckRequired = true
                    };

                    basePriceAndAvailability.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    costPriceAndAvailability.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;

                    foreach (var variant in variants.Value)
                    {
                        var mappedPaxQuery = from tpm in criteria.TiqetsPaxMappings
                                             from pt in criteria.NoOfPassengers
                                             where tpm.AgeGroupCode == variant.Id.ToString()
                                             && tpm.ServiceOptionId == criteria.OptionId
                                             && pt.Key == tpm.PassengerType
                                             select tpm;
                        var mappedPax = mappedPaxQuery?.FirstOrDefault();
                        /*
                        if (mappedPax == null)
                        {
                            var adutlVariant = variants.Value.FirstOrDefault(x => x.Label?.ToLower().Contains(PassengerType.Adult.ToString().ToLower()) == true);
                            var tpm = criteria.TiqetsPaxMappings.FirstOrDefault(x => x.PassengerType == PassengerType.Adult);
                            tpm.AgeGroupCode = adutlVariant.Id.ToString();
                            mappedPax = tpm;
                        }
                        */
                        var ageGroupId = mappedPax?.AgeGroupId ?? 0;
                        if (mappedPax != null)
                        {
                            var passengerType = mappedPax.PassengerType;

                            var commissionPercent = variant.PriceComponentsEur.DistributorCommissionExclVat;

                            if (basePriceAndAvailability.PricingUnits == null)
                            {
                                basePriceAndAvailability.PricingUnits = new List<PricingUnit>();
                            }
                            if (costPriceAndAvailability.PricingUnits == null)
                            {
                                costPriceAndAvailability.PricingUnits = new List<PricingUnit>();
                            }

                            var isPassengerValid = criteria.NoOfPassengers.Any(x => x.Key == passengerType);

                            if (passengerType != 0 && isPassengerValid)
                            {
                                var paxBasePrice = variant.PriceComponentsEur.TotalRetailPriceIncVat;

                                //Create base price pricing unit
                                basePriceAndAvailability.PricingUnits.Add(CreatePricingUnit(passengerType, paxBasePrice, totalCapacity));

                                //Get cost price
                                var paxCostPrice = GetCommissionedPrice(paxBasePrice, commissionPercent);

                                //Create cost price pricing unit
                                costPriceAndAvailability.PricingUnits.Add(CreatePricingUnit(passengerType, paxCostPrice, totalCapacity));

                                totalBasePrice += paxBasePrice;
                                totalCostPrice += paxCostPrice;
                            }
                        }
                    }

                    #region "Set infant PricingUnit if criteria contains infant pax type but supplier does not support it"

                    var infantPaxType = 0;
                    var infantInCriteria = criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Infant);
                    if (infantInCriteria.Value > 0)
                    {
                        infantPaxType = infantInCriteria.Value;
                    }
                    if (infantPaxType > 0)
                    {
                        var infantPricingUnit = basePriceAndAvailability.PricingUnits.FirstOrDefault(x => x is InfantPricingUnit);
                        if (infantPricingUnit == null)
                        {
                            var infantNewPricingUnit = CreatePricingUnit(PassengerType.Infant, 0, totalCapacity);
                            basePriceAndAvailability.PricingUnits.Add(infantNewPricingUnit);
                            costPriceAndAvailability.PricingUnits.Add(infantNewPricingUnit);
                        }
                    }

                    #endregion "Set infant PricingUnit if criteria contains infant pax type but supplier does not support it"

                    if (totalBasePrice > 0)
                    {
                        basePriceAndAvailability.TotalPrice = totalBasePrice;
                        basePriceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        if (!basePrice.DatePriceAndAvailabilty.ContainsKey(variants.Key))
                            basePrice.DatePriceAndAvailabilty.Add(variants.Key, basePriceAndAvailability);
                    }
                    if (totalCostPrice > 0)
                    {
                        costPriceAndAvailability.TotalPrice = totalCostPrice;
                        costPriceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        if (!costPrice.DatePriceAndAvailabilty.ContainsKey(variants.Key))
                            costPrice.DatePriceAndAvailabilty.Add(variants.Key, costPriceAndAvailability);
                    }
                }

                if (basePrice.DatePriceAndAvailabilty.Count > 0)
                {
                    option.BasePrice = basePrice;
                    option.BasePrice.Amount = basePrice.DatePriceAndAvailabilty.FirstOrDefault().Value.TotalPrice;
                    option.GateBasePrice = basePrice.DeepCopy(); // Preparing GateBasePrice here as it is needed in PriceRuleEngine
                }
                if (costPrice.DatePriceAndAvailabilty.Count > 0)
                {
                    option.CostPrice = costPrice;
                    option.CostPrice.Amount = costPrice.DatePriceAndAvailabilty.FirstOrDefault().Value.TotalPrice;
                }
                if (option.BasePrice != null && option.CostPrice != null)
                {
                    option.CommisionPercent = option.BasePrice.Amount == 0 ? 0.0M : (option.BasePrice.Amount - option.CostPrice.Amount) / option.BasePrice.Amount; // Calculating commission (Exceptional Case: added check on base price as if the pax mapping for particular activity will not present, base price will be 0)
                }
            }
            catch (Exception ex)
            {
                //Ignored as failing one option should not fail other option as well
                // #TODO  add logging at adapter level
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "Tiqets.AvailabilityConverter",
                    MethodName = "ConvertAvailabilityResult/CreateOption"
                };
                _logger.Error(isangoErrorEntity, ex);
            }
            return option;
        }

        private void SetCancellationPolicyText(Dictionary<DateTime, List<ProductVariant>> dateVariantsDict, TiqetsCriteria criteria, ActivityOption option)
        {
            var language = criteria?.Language ?? "en";
            var checkinDateEntryInVariantDict = dateVariantsDict.FirstOrDefault();
            var cancellationPolicyInVariant = new Cancellation();
            if (criteria?.TiqetsPaxMappings?.Count > 0)
            {
                var adultPax = criteria.TiqetsPaxMappings.Where(x => x?.PassengerType == PassengerType.Adult)?.ToList();
                var seniorPax = criteria.TiqetsPaxMappings.Where(x => x?.PassengerType == PassengerType.Senior)?.ToList();
                var youthPax = criteria.TiqetsPaxMappings.Where(x => x?.PassengerType == PassengerType.Youth)?.ToList();
                if (adultPax != null)
                {
                    var dateEntryVariantObject = from dateEntryVariant in checkinDateEntryInVariantDict.Value
                               from adpax in adultPax
                               where dateEntryVariant.Id.ToString() == adpax.AgeGroupCode
                               select dateEntryVariant;
                    cancellationPolicyInVariant = dateEntryVariantObject?.FirstOrDefault()?.Cancellation;
                }
                else if (seniorPax != null)
                {
                    var dateEntryVariantObject = from dateEntryVariant in checkinDateEntryInVariantDict.Value
                                                 from senpax in seniorPax
                                                 where dateEntryVariant.Id.ToString() == senpax.AgeGroupCode
                                                 select dateEntryVariant;
                    cancellationPolicyInVariant = dateEntryVariantObject?.FirstOrDefault()?.Cancellation;
                }
                else if (youthPax != null)
                {
                    var dateEntryVariantObject = from dateEntryVariant in checkinDateEntryInVariantDict.Value
                                                 from youpax in youthPax
                                                 where dateEntryVariant.Id.ToString() == youpax.AgeGroupCode
                                                 select dateEntryVariant;
                    cancellationPolicyInVariant = dateEntryVariantObject?.FirstOrDefault()?.Cancellation;
                }
               

                else
                {
                    cancellationPolicyInVariant = checkinDateEntryInVariantDict.Value?.FirstOrDefault()?.Cancellation;
                }
            }
            //cancellationPolicyInVariant = checkinDateEntryInVariantDict.Value.Find(thisVariant => thisVariant.Id.ToString().Equals(criteria?.TiqetsPaxMappings?[0].AgeGroupCode))?.Cancellation;
            //var cancellationPolicyInVariant = checkinDateEntryInVariantDict.Value.Find(thisVariant => thisVariant.Label.ToLowerInvariant().Contains("adult") || thisVariant.Label.ToLowerInvariant().Contains("senior") || thisVariant.Label.ToLowerInvariant().Contains("youth") || thisVariant.Label.ToLowerInvariant().Contains("midra 30") || thisVariant.Label.ToLowerInvariant().Contains("mimma 15"))?.Cancellation;
            //If window=0, then non cancel-able
            //Check variant and select Adult variant cancellation detail
            //If Adult not available, then take cancellation detail of Senior, if not then take of Youth
            if (!string.IsNullOrEmpty(cancellationPolicyInVariant?.Policy))
            {
                if (cancellationPolicyInVariant.Policy.ToLowerInvariant().Equals(Constant.Never.ToLowerInvariant()) || cancellationPolicyInVariant.Window == 0)
                {
                    option.Cancellable = false;
                    option.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANT.CancellationPolicyNonRefundable);
                }
                else if (cancellationPolicyInVariant.Policy.ToLowerInvariant().Equals(Constant.BeforeDate.ToLowerInvariant()))
                {
                 //In Before date, the window is the no of hours before 12AM on the date of visit. So add one day to the no of days we get from the Window hours

                 var noOfDaysALlowedForCancellation = Math.Ceiling(System.Convert.ToDecimal(cancellationPolicyInVariant.Window) / 24);
                 option.Cancellable = true;
                 var noOfHours = (noOfDaysALlowedForCancellation) * 24;
                 option.CancellationText = $"{string.Format(RESOURCEMANAGER.GetString(language, CONSTANT.CancellationPolicy100ChargableBeforeNhours), noOfHours, noOfHours)}";
                }
                else if (cancellationPolicyInVariant.Policy.ToLowerInvariant().Equals(Constant.BeforeTimeSlot.ToLowerInvariant()))
                {
                    //In Before Time slot, the window is the no of hours before the timeslot or venue's opening hours before which the cancellations must be done.
                    var noOfDaysALlowedForCancellation = Math.Ceiling(System.Convert.ToDecimal(cancellationPolicyInVariant.Window) / 24);
                    option.Cancellable = true;
                    var noOfHours = noOfDaysALlowedForCancellation * 24;
                    option.CancellationText = $"{string.Format(RESOURCEMANAGER.GetString(language, CONSTANT.CancellationPolicy100ChargableBeforeNhours), noOfHours, noOfHours)}";
                }
                option.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(checkinDateEntryInVariantDict);

                //CancellationPrices
                var cancellationPrice = new CancellationPrice();
                option.CancellationPrices = new List<CancellationPrice>();
                if (option.Cancellable)
                {
                    cancellationPrice = new CancellationPrice
                    {
                        CancellationAmount = 0,
                        CancellationDescription = option.CancellationText,//"test complete API",
                        Percentage = 0,
                        CancellationFromdate = criteria.CheckinDate,//checkindate
                        //traveldate-window
                        //CancellationDateRelatedToOpreationDate = criteria.CheckinDate.AddHours(-cancellationPolicyInVariant.Window),
                        CancellationDateRelatedToOpreationDate = criteria.CheckinDate,
                        CancellationToDate = criteria.CheckinDate.AddHours(cancellationPolicyInVariant.Window),

                    };
                }
                else
                {
                    cancellationPrice = new CancellationPrice
                    {
                        CancellationAmount = 0,
                        CancellationDescription = option.CancellationText,//"test complete API",
                        Percentage = 100,
                        //traveldate-window
                        CancellationFromdate = criteria.CheckinDate.AddHours(-cancellationPolicyInVariant.Window),
                        //travel date
                        CancellationDateRelatedToOpreationDate = criteria.CheckinDate,
                        CancellationToDate = criteria.CheckinDate,
                    };
                }
                option.CancellationPrices.Add(cancellationPrice);
            }
        }

        /// <summary>
        /// Calculate Cost Price from Commission and Base Price
        /// </summary>
        /// <param name="basePrice"></param>
        /// <param name="commissionPercentage"></param>
        /// <returns></returns>
        private decimal GetCommissionedPrice(decimal basePrice, decimal commissionPercentage)
        {
            return basePrice - commissionPercentage;
        }

        /// <summary>
        /// Create Pricing Unit
        /// </summary>
        /// <param name="passengerType"></param>
        /// <param name="price"></param>
        /// <param name="totalCapacity"></param>
        /// <returns></returns>
        private PricingUnit CreatePricingUnit(PassengerType passengerType, decimal price, int totalCapacity)
        {
            var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
            pricingUnit.Quantity = 1; //Setting its value as '1' because UnitType is PerPerson
            pricingUnit.UnitType = UnitType.PerPerson;
            pricingUnit.Price = price;
            pricingUnit.TotalCapacity = totalCapacity;
            return pricingUnit;
        }

        #endregion "Private Methods"
    }
}