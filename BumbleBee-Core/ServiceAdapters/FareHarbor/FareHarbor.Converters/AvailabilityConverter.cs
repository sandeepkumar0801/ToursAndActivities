using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Logger.Contract;
using ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;

namespace ServiceAdapters.FareHarbor.FareHarbor.Converters
{
    public class AvailabilityConverter : ConverterBase, IAvailabilityConverter
    {
        public AvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(T objectResult, object fareHarborCriteria)
        {
            var result = objectResult as Availability;

            if (result != null)
            {
                var availabilityList = ConvertAvailabilityResult(fareHarborCriteria, result);
                return availabilityList;
            }

            return null;
        }

        /// <summary>
        /// This method maps the API response to iSango Contracts objects.
        /// </summary>
        /// <param name="availabilityResponse"></param>
        /// <param name="fareHarborCriteria"></param>
        /// <returns></returns>
        private object ConvertAvailabilityResult(object fareHarborCriteria, Availability availabilityResponse)
        {
            if (availabilityResponse != null)
            {
                var optionsList = new List<ProductOption>();
                var avail = availabilityResponse;
                var apiActivity = availabilityResponse.Item;
                var customerTypeRates = avail?.CustomerTypeRates;
                var mincap = availabilityResponse?.MinimumPartySize;
                var maxcap = availabilityResponse?.MaximumPartySize ?? availabilityResponse?.Capacity;
                if (customerTypeRates != null)
                {
                    var criteria = (FareHarborCriteria)fareHarborCriteria;
                    foreach (var availabilityTypeRates in customerTypeRates)
                    {
                        try
                        {
                            var minPartySize = System.Convert.ToInt32(availabilityTypeRates?.MinimumPartySize ?? (mincap ?? 1));
                            var maxPartySize = System.Convert.ToInt32(maxcap ?? 20);
                            var chargablePaxTypeTotalCount = criteria?.NoOfPassengers?.Where(x => x.Key != PassengerType.Infant)?.Sum(y => y.Value) ?? 0;

                            if (minPartySize > 0 && chargablePaxTypeTotalCount < minPartySize)
                            {
                                continue;
                            }
                           if(maxPartySize> 0 && chargablePaxTypeTotalCount> maxPartySize)
                            {
                                continue;
                            }

                            var checkCustomerTypes = criteria?.CustomerPrototypes?
                                    .Where(x => x.CustomerPrototypeId == availabilityTypeRates.CustomerPrototype.Pk)?
                                    .Select(x => x.PassengerType);

                            var passengerMapping = criteria?.CustomerPrototypes?
                                            .Where(x => x.CustomerPrototypeId == availabilityTypeRates.CustomerPrototype.Pk)?
                                            .FirstOrDefault();

                            foreach (var checkCustomerType in checkCustomerTypes)
                            {
                                var amount = availabilityTypeRates.CustomerPrototype.TotalIncludingTax / 100;
                                var customerPrototypePk = availabilityTypeRates.CustomerPrototype.Pk;

                                var activityOption = new ActivityOption
                                {
                                    Id = avail.Pk,
                                    Code = customerPrototypePk.ToString(),
                                    TravelInfo = new TravelInfo
                                    {
                                        Ages = criteria.Ages,
                                        NoOfPassengers = criteria.NoOfPassengers,
                                        StartDate = criteria.CheckinDate
                                    },
                                };

                                // Availability ID
                                var price = new Price
                                {
                                    Amount = passengerMapping.IsUnitPrice ?
                                                 amount :
                                                 amount * criteria.NoOfPassengers[checkCustomerType],

                                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>(),
                                    Currency = new Currency()
                                };

                                var customerTypePriceIds = new Dictionary<PassengerType, Int64>();
                                var dateTimeOffset = DateTimeOffset.Parse(avail.StartAt, null);

                                customerTypePriceIds.Add(checkCustomerType, availabilityTypeRates.Pk);

                                var pricingUnit = GetPricingUnit(checkCustomerType, amount, passengerMapping, criteria.NoOfPassengers[checkCustomerType], availabilityTypeRates);

                                var pricingUnits = new List<PricingUnit>
                                {
                                    pricingUnit
                                };

                                price.DatePriceAndAvailabilty.Add(dateTimeOffset.DateTime, new FareHarborPriceAndAvailability
                                {
                                    // total is provided with two extra zeros
                                    TotalPrice = passengerMapping.IsUnitPrice ?
                                                 amount :
                                                 amount * criteria.NoOfPassengers[checkCustomerType],

                                    CustomerTypePriceIds = customerTypePriceIds,
                                    PricingUnits = pricingUnits,

                                    Capacity = System.Convert.ToInt32(maxPartySize) > 0 ?
                                                System.Convert.ToInt32(maxPartySize) :
                                                passengerMapping.PassengersInUnitMaximum,

                                    IsCapacityCheckRequired = true,
                                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                                    UnitQuantity = passengerMapping.IsUnitPrice ?
                                                1 :
                                                criteria.NoOfPassengers[checkCustomerType],
                                    TourDepartureId = System.Convert.ToInt32(criteria.ActivityCode)
                                });
                                activityOption.BasePrice = price;

                                // Preparing GateBasePrice here as its needed in PriceRuleEngine
                                activityOption.GateBasePrice = price.DeepCopy();

                                optionsList.Add(activityOption);
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = nameof(AvailabilityConverter),
                                MethodName = nameof(ConvertAvailabilityResult)
                            };
                            Task.Run(() => _logger.Error(isangoErrorEntity, ex));
                            continue;
                        }
                    }

                    return optionsList;
                }
            }
            return null;
        }

        private PricingUnit GetPricingUnit(PassengerType passengerType, decimal price
            , Isango.Entities.CustomerPrototype customerPrototype
            , int numberOfPassenger
            , CustomerTypeRate availabilityTypeRates
        )
        {
            var pricingUnit = default(PricingUnit);
            try
            {
                var minPartySize = System.Convert.ToInt32(availabilityTypeRates?.MinimumPartySize);
                var maxPartySize = System.Convert.ToInt32(availabilityTypeRates?.MaximumPartySize);

                var minCapacity = minPartySize > 0 ? minPartySize : 
                                    customerPrototype.PassengersInUnitMinimum;

                var maxCapacity = maxPartySize > 0 ? maxPartySize : 
                                   customerPrototype.PassengersInUnitMaximum;

                pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);

                if (pricingUnit != null)
                {
                    pricingUnit.Price = price != 0 ? price /*/ 100*/ : 0;
                    pricingUnit.PriceType = PriceType.PerPerson;
                    pricingUnit.UnitType = UnitType.PerPerson;
                    pricingUnit.TotalCapacity = maxCapacity;
                    pricingUnit.Mincapacity = minCapacity;
                    pricingUnit.Quantity = numberOfPassenger;
                }

                if (customerPrototype.IsUnitPrice
                       && numberOfPassenger >= customerPrototype.PassengersInUnitMinimum
                       && numberOfPassenger <= customerPrototype.PassengersInUnitMaximum
                )
                {
                    pricingUnit.Price = price != 0 ? price / numberOfPassenger : 0;
                    pricingUnit.UnitType = UnitType.PerUnit;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = nameof(AvailabilityConverter),
                    MethodName = nameof(GetPricingUnit)
                };
                Task.Run(() => _logger.Error(isangoErrorEntity, ex));
            }
            return pricingUnit;
        }
    }
}