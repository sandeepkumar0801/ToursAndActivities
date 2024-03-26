using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Constant = ServiceAdapters.GrayLineIceLand.Constants.Constant;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters
{
    public class AvailabilityConverter : ConverterBase, IAvailabilityConverter
    {
        public AvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert(object objectResult, object input)
        {
            var activity = ConvertAvailability(objectResult, input);
            if (activity == null || !activity.ProductOptions.Any(x => x.AvailabilityStatus.Equals(AvailabilityStatus.AVAILABLE))) return null;
            return activity;
        }

        public override object Convert(object objectResult)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private Activity ConvertAvailability(object objectResult, object input)
        {
            var criteria = (GrayLineIcelandCriteria)input;
            var toursAvailabilityResponse = (ToursAvailabilityRS)objectResult;
            var activity = new Activity
            {
                ApiType = APIType.Graylineiceland,
                Code = toursAvailabilityResponse.TourDepartures.FirstOrDefault()?.TourNumber
            };

            var checkinDate = criteria != null ? criteria.CheckinDate.Date : DateTime.Now.Date;
            var tourDeparturesGroupedByTime = toursAvailabilityResponse.TourDepartures.GroupBy(x => x.Departure.TimeOfDay).OrderBy(x => x.Key);
            activity.ProductOptions = new List<ProductOption>();
            var pickupLocations = new Dictionary<int, string>();
            foreach (var options in tourDeparturesGroupedByTime)
            {
                var firstTourDeparture = options.FirstOrDefault();
                if (firstTourDeparture == null) continue;
                var travelInfo = PrepareTravelInfo(firstTourDeparture, criteria?.PaxAgeGroupIds, checkinDate);

                var productOption = new ActivityOption
                {
                    AvailabilityStatus = options.Any(x => x.Available) ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE,
                    TravelInfo = travelInfo,
                    StartTime = options.Key
                };
                var basePrice = new Isango.Entities.Price
                {
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>(),
                    Currency = new Currency { IsoCode = Constant.ISK }
                };

                var costPrice = new Isango.Entities.Price
                {
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>(),
                    Currency = new Currency { IsoCode = Constant.ISK }
                };
                try
                {
                    var pickupLocationMapped = MapPickupLocations(options?.FirstOrDefault()?.PickUpLocations.ToList());
                    if (pickupLocationMapped?.Any() == true)
                    {
                        if (pickupLocations?.Any() == true)
                        {
                            foreach (var item in pickupLocationMapped)
                            {
                                if (pickupLocations.Keys.Contains(item.Key) == false)
                                {
                                    pickupLocations.Add(item.Key, item.Value);
                                }
                            }
                        }
                        else
                        {
                            pickupLocations = pickupLocationMapped;
                        }
                    }
                    foreach (var optionDatesAvailability in options)
                    {
                        productOption.PickupLocations = pickupLocations;
                        productOption.Id = optionDatesAvailability.TourDepartureId;
                        productOption.OptionKey = optionDatesAvailability.Departure.ToString("yyyy-MM-dd");
                        productOption.Name =
                            $"{optionDatesAvailability.TourDescription} at {optionDatesAvailability.Departure.ToString("hh:mm tt")}";
                        var priceAndAvailability = new DefaultPriceAndAvailability();
                        var costPriceAndAvailablity = new DefaultPriceAndAvailability();

                        var commisionPercent = toursAvailabilityResponse.TourDepartures
                            .FirstOrDefault(x => x.Prices?.Length > 0)?.Prices.FirstOrDefault()?.CommissionPercent;
                        productOption.CommisionPercent = commisionPercent ?? 0.0M;

                        if (optionDatesAvailability.Available)
                        {
                            priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                            costPriceAndAvailablity.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        }
                        else
                        {
                            priceAndAvailability.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                            costPriceAndAvailablity.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                            continue;
                        }

                        var result = CreatePricingUnits(travelInfo.NoOfPassengers, criteria, optionDatesAvailability,
                            productOption.CommisionPercent);

                        priceAndAvailability.PricingUnits = result.Item1;
                        costPriceAndAvailablity.PricingUnits = result.Item2;

                        priceAndAvailability.TotalPrice = priceAndAvailability.PricingUnits.Sum(x => x.Price);
                        costPriceAndAvailablity.TotalPrice = costPriceAndAvailablity.PricingUnits.Sum(x => x.Price);

                        if (costPriceAndAvailablity.TotalPrice == 0 || priceAndAvailability.TotalPrice == 0) continue;

                        priceAndAvailability.TourDepartureId = optionDatesAvailability.TourDepartureId;
                        costPriceAndAvailablity.TourDepartureId = optionDatesAvailability.TourDepartureId;

                        var dateKey = optionDatesAvailability.Departure
                            .AddHours(-Math.Abs(optionDatesAvailability.Departure.Hour))
                            .AddMinutes(-Math.Abs(optionDatesAvailability.Departure.Minute));
                        if (dateKey.Date == checkinDate)
                        {
                            var capacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
                            priceAndAvailability.Capacity = capacity;
                            costPriceAndAvailablity.Capacity = capacity;
                            priceAndAvailability.IsSelected = true;
                            costPriceAndAvailablity.IsSelected = true;
                            basePrice.Amount = priceAndAvailability.TotalPrice;
                            costPrice.Amount = costPriceAndAvailablity.TotalPrice;
                            productOption.IsSelected = true;
                        }

                        basePrice.DatePriceAndAvailabilty.Add(dateKey, priceAndAvailability);
                        costPrice.DatePriceAndAvailabilty.Add(dateKey, costPriceAndAvailablity);

                        productOption.CostPrice = costPrice;
                        productOption.BasePrice = basePrice;
                        productOption.GateBasePrice = basePrice.DeepCopy(); // Preparing GateBasePrice here as its needed in PriceRuleEngine
                    }
                    activity.ProductOptions.Add(productOption);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "GrayLineIceLand.AvailabilityConverter",
                        MethodName = "Convert.CreateOption"
                    };
                    _logger.Error(isangoErrorEntity, ex);
                    continue;
                }
            }
            return activity.ProductOptions.Any(x => x.AvailabilityStatus.Equals(AvailabilityStatus.AVAILABLE)) ? activity : null;
        }

        private decimal GetCommissionedPrice(decimal basePrice, decimal commissionPercentage)
        {
            //Note: Added below check as it is used in unity for GLI API
            commissionPercentage = commissionPercentage < 1 ? commissionPercentage * 100 : commissionPercentage;

            return basePrice * ((100 - commissionPercentage) / 100);
        }

        private Dictionary<int, string> MapPickupLocations(List<Pickuplocation> pickupLocations)
        {
            var pickupLocationsDict = new Dictionary<int, string>();
            foreach (var pickuplocation in pickupLocations)
            {
                if (!pickupLocationsDict.Keys.Contains(pickuplocation.Id))
                    pickupLocationsDict.Add(pickuplocation.Id, pickuplocation.Description);
            }
            return pickupLocationsDict;
        }

        private TravelInfo PrepareTravelInfo(Tourdeparture firstTourDeparture, Dictionary<PassengerType, int> paxAgeGroupIds, DateTime checkinDate)
        {
            var travelInfo = new TravelInfo
            {
                NoOfPassengers = new Dictionary<PassengerType, int>()
            };

            foreach (var paxAgeGroupId in paxAgeGroupIds)
            {
                var paxMappingId = System.Convert.ToInt32((paxAgeGroupIds?.FirstOrDefault(x => x.Key == paxAgeGroupId.Key))?.Value);

                if (firstTourDeparture.AgeGroups != null && firstTourDeparture.AgeGroups.Any())
                {
                    var paxNode = firstTourDeparture?.AgeGroups?.FirstOrDefault(x => x.AgeGroup == paxMappingId);
                    var paxCount = System.Convert.ToInt32(paxNode?.Quantity);
                    if (paxCount > 0)
                        travelInfo.NoOfPassengers.Add(paxAgeGroupId.Key, paxCount);
                    travelInfo.StartDate = checkinDate;
                }
            }

            return travelInfo;
        }

        private Tuple<List<PricingUnit>, List<PricingUnit>> CreatePricingUnits(Dictionary<PassengerType, int> noOfPassengers, GrayLineIcelandCriteria criteria, Tourdeparture optionDatesAvailability, decimal commisionPercent)
        {
            var basePricingUnits = new List<PricingUnit>();
            var costPricingUnits = new List<PricingUnit>();
            foreach (var noOfPassenger in noOfPassengers)
            {
                try
                {
                    var passengerType = noOfPassenger.Key;
                    var ageGroupid = criteria.PaxAgeGroupIds[passengerType];

                    var priceNode = optionDatesAvailability.Prices.FirstOrDefault(x =>
                        x.AgeGroup == ageGroupid);

                    var basePriceAmount = priceNode != null
                        ? System.Convert.ToDecimal(priceNode.PricePerPAX)
                        : 0;

                    var costPriceAmount = priceNode != null
                        ? GetCommissionedPrice(System.Convert.ToDecimal(priceNode.PricePerPAX), commisionPercent)
                        : 0;

                    var basePricingUnit = CreatePricingUnit(passengerType, basePriceAmount, criteria);
                    if (basePricingUnit == null) continue;
                    basePricingUnits.Add(basePricingUnit);

                    var costPricingUnit = CreatePricingUnit(passengerType, costPriceAmount, criteria);
                    if (costPricingUnit == null) continue;
                    costPricingUnits.Add(costPricingUnit);
                }
                catch (Exception ex)
                {
                    //ignored
                }
            }

            return new Tuple<List<PricingUnit>, List<PricingUnit>>(basePricingUnits, costPricingUnits);
        }

        private PricingUnit CreatePricingUnit(PassengerType passengerType, decimal price, GrayLineIcelandCriteria criteria)
        {
            PricingUnit pricingUnit = null;
            var paxCount = criteria.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(passengerType)).Value;

            if (paxCount > 0)
            {
                var ageGroupId = criteria.PaxAgeGroupIds?.FirstOrDefault(x => x.Key.Equals(passengerType)).Value ?? 0;
                pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
                pricingUnit.Price = price;
            }

            return pricingUnit;
        }

        #endregion Private Methods
    }
}