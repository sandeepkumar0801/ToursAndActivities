using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Redeam;
using ServiceAdapters.Redeam.Redeam.Converters.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities.GetAvailabilities;
using ServiceAdapters.Redeam.Redeam.Entities.GetRate;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;
using Constant = ServiceAdapters.Redeam.Constants.Constant;
using CONSTANTCANCELLATION = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.Redeam.Redeam.Converters
{
    public class GetAvailabilitiesConverter : ConverterBase, IGetAvailabilitiesConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="extraRequest"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request, T extraRequest)
        {
            // Intializing result if response is null in case of FS and PASS type
            var result = new AvailabilitiesResponse();
            if (response != null)
            {
                result = SerializeDeSerializeHelper.DeSerialize<AvailabilitiesResponse>(response.ToString());
            }
            return ConvertAvailabilityResult(result, extraRequest as GetRateResponse, request as RedeamCriteria);
        }

        #region Private Methods

        /// <summary>
        /// Prepare the product options using the response data of supplier
        /// </summary>
        /// <param name="availabilitiesResponse"></param>
        /// <param name="ratesResponse"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<ProductOption> ConvertAvailabilityResult(AvailabilitiesResponse availabilitiesResponse, GetRateResponse ratesResponse, RedeamCriteria criteria)
        {
            var productOptions = new List<ProductOption>();
            var rate = ratesResponse.Rate;
            var rateType = criteria.RateIdAndType?.FirstOrDefault(x => x.Key == $"{criteria.ProductId}#{criteria.RateId}").Value;
            if (string.IsNullOrEmpty(rateType) || rate == null) return productOptions;

            var maxTravelersAllowed = rate?.MaxTravelers;
            var selectedPax = criteria.NoOfPassengers?.Sum(x => x.Value);

            // creating availabilities for the Pass and FreeSale type, this will override in case of Reserved type
            var availabilities = new List<Availability>();

            if (availabilitiesResponse.Availabilities?.ByRate?.String != null && rateType == Constant.ReservedType)
            {
                var availabilitiesByRate = availabilitiesResponse.Availabilities.ByRate.String.Select(x => x).ToDictionary(x => x.Key,
                        y => SerializeDeSerializeHelper.DeSerialize<ByProduct>(y.Value.ToString()).Availability);

                var availabilitiesByRateId = availabilitiesByRate.FirstOrDefault(x => x.Key.Equals(criteria.RateId)).Value;
                if (availabilitiesByRateId == null) return productOptions;

                availabilities = availabilitiesByRateId;
            }
            else
            {
                //if criteria checkin date and checkout date is same, then adding 1 day's availability

                //setting it to one as date rage is required in price
                //For each day one option with singe P&A not needed
                /*
                var totalDays = criteria.CheckoutDate.Subtract(criteria.CheckinDate).TotalDays;
                if (totalDays == 0)
                    totalDays++;
                //

                for (int day = 0; day < totalDays; day++)
                {
                    var availabledate = new Availability { Start = criteria.CheckinDate.AddDays(day) };
                    availabilities.Add(availabledate);
                }
                */
                var availabledate = new Availability { Start = criteria.CheckinDate };
                availabilities.Add(availabledate);
            }

            foreach (var availability in availabilities)
            {
                var validPaxCriteriaForReserved = CheckPaxForReserveType(availability, selectedPax, rateType) ? AvailabilityStatus.NOTAVAILABLE : AvailabilityStatus.AVAILABLE;
                var availabilityStatus = (maxTravelersAllowed < selectedPax) ? AvailabilityStatus.NOTAVAILABLE : validPaxCriteriaForReserved;

                var productOption = MapProductOptionAndPrice(ratesResponse, criteria, availability.Start, availabilityStatus);
                if (productOption != null)
                    productOptions.Add(productOption);
            }
            return productOptions;
        }

        /// <summary>
        /// Prepare options for different rate type
        /// </summary>
        /// <param name="ratesResponse"></param>
        /// <param name="criteria"></param>
        /// <param name="time"></param>
        /// <param name="availabilityStatus"></param>
        /// <returns></returns>
        private ProductOption MapProductOptionAndPrice(GetRateResponse ratesResponse, RedeamCriteria criteria, DateTime time, AvailabilityStatus availabilityStatus)
        {
            var language = criteria?.Language?.ToLower() ?? "en";
            var rate = ratesResponse.Rate;
            var currency = rate?.Prices.FirstOrDefault()?.Net?.Currency;
            var timeString = time.ToString(Constant.TimeFormat);

            if (rate?.Prices == null) return null;

            var productOption = new ActivityOption()
            {
                SupplierOptionCode = $"{criteria.ProductId}#{criteria.RateId}",
                Name = timeString == "12:00 AM" ? string.Empty : timeString,
                TravelInfo = new TravelInfo
                {
                    StartDate = criteria.CheckinDate.Date,
                    NoOfPassengers = criteria.NoOfPassengers,
                    Ages = criteria.Ages,
                    NumberOfNights = (criteria.CheckoutDate - criteria.CheckinDate).Days
                },
                AvailabilityStatus = availabilityStatus,
                Cancellable = rate.Cancelable,
                Holdable = rate.Holdable,
                Refundable = rate.Refundable,
                HoldablePeriod = rate.HoldablePeriod,
                Time = timeString,
                Type = rate?.Type,
                RateId = criteria?.RateId,
                SupplierId = criteria?.SupplierId,
                ServiceOptionId = criteria.ServiceOptionId,
                ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(new
                {
                    RateCode = rate.Code,
                    IsRefundable = rate.Refundable,
                    IsCancelable = rate.Cancelable,
                    rate.Cutoff,
                })
            };

            //Set Cancellation text and cancel-able here
            if (productOption.Refundable == false)
            {
                productOption.Cancellable = false;
                productOption.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANTCANCELLATION.CancellationPolicyNonRefundable);
            }
            else if (productOption.Refundable == true)
            {
                productOption.Cancellable = true;
                productOption.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANTCANCELLATION.CancellationPolicyDefaultFree24Hours); ;
            }

            var priceIds = new Dictionary<string, string>();
            var ratePriceQuery = from price in rate.Prices
                                 from paxtype in criteria.NoOfPassengers
                                 where string.Equals(price.TravelerType.AgeBand
                                                        , paxtype.Key.ToString()
                                                        , StringComparison.CurrentCultureIgnoreCase
                                                    )
                                 select price;

            if (ratePriceQuery.Any())
            {
                rate.Prices = ratePriceQuery
                                .Where(y => string.Equals(y.Status
                                                        , "active"
                                                        , StringComparison.CurrentCultureIgnoreCase
                                                    )
                                                    )
                                .OrderBy(x => x.TravelerType.AgeBand)
                                //.ThenBy(x => x.Net.Amount)
                                .ThenBy(x => x.Name)
                                .ToList();
            }
            else
            {
                return null;
            }

            foreach (var priceAndAgeBand in rate.Prices)
            {
                if (!priceIds.ContainsKey(priceAndAgeBand.TravelerType.AgeBand))
                    priceIds.Add(priceAndAgeBand.TravelerType.AgeBand, priceAndAgeBand.Id.ToString());
            }

            productOption.PriceId = priceIds;

            var pricingUnits = CreatePricingUnits(rate.Prices, criteria);
            if (pricingUnits.Count == 0) return null;

            foreach (var pu in pricingUnits)
            {
                System.Int32.TryParse(rate?.MaxTravelers.ToString(), out var tempint);
                pu.TotalCapacity = tempint;
                System.Int32.TryParse(rate?.MinTravelers.ToString(), out tempint);
                pu.Mincapacity = tempint;
            }
            var basePriceAndAvailability = new DefaultPriceAndAvailability
            {
                AvailabilityStatus = availabilityStatus,
                TotalPrice = pricingUnits.Sum(x => x.Price),
                PricingUnits = pricingUnits
            };

            productOption.CostPrice = new Isango.Entities.Price
            {
                Amount = basePriceAndAvailability.TotalPrice,
                Currency = new Currency { IsoCode = currency },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
            };

            for (var date = criteria.CheckinDate; date <= criteria.CheckoutDate; date = date.AddDays(1))
            {
                var panda = basePriceAndAvailability.Clone() as PriceAndAvailability;
                if (!productOption.CostPrice.DatePriceAndAvailabilty.ContainsKey(date))
                {
                    productOption.CostPrice.DatePriceAndAvailabilty.Add(date, panda);
                }
            }

            return productOption;
        }

        private List<PricingUnit> CreatePricingUnits(List<Entities.GetRate.Price> prices, RedeamCriteria criteria)
        {
            var pricingUnits = new List<PricingUnit>();

            foreach (var price in prices)
            {
                var netPrice = price.Net.Amount / 100;
                var passengerType = GetPassengerType(price.TravelerType.AgeBand);
                var pricingUnit = CreatePricingUnit(passengerType, netPrice, criteria);
                if (pricingUnit == null) continue;
                if (!pricingUnits.Any(x => ((Isango.Entities.PerPersonPricingUnit)pricingUnit).PassengerType == ((Isango.Entities.PerPersonPricingUnit)x).PassengerType))
                {
                    pricingUnits.Add(pricingUnit);
                }
            }

            int infantCount = 0;
            try
            {
                infantCount = criteria.NoOfPassengers[PassengerType.Infant];
                if (!pricingUnits.Any(x => ((PerPersonPricingUnit)x).PassengerType == PassengerType.Infant))
                {
                    var pricingUnit = CreatePricingUnit(PassengerType.Infant, 0, criteria);
                    pricingUnits.Add(pricingUnit);
                }
            }
            catch (Exception ex)
            {
            }

            return pricingUnits;
        }

        /// <summary>
        /// Create Pricing Unit
        /// </summary>
        /// <param name="passengerType"></param>
        /// <param name="price"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private PricingUnit CreatePricingUnit(PassengerType passengerType, decimal price, RedeamCriteria criteria)
        {
            var paxCount = criteria.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(passengerType)).Value;
            if (paxCount <= 0) return null;
            var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
            pricingUnit.Price = price;

            //if (pricingUnit is PerPersonPricingUnit perPersonPricingUnit)
            //    perPersonPricingUnit.PassengerType = ageGroupId;
            //else
            //    return null;

            return pricingUnit;
        }

        /// <summary>
        /// Temporary method for PassengerType mapping
        /// </summary>
        /// <param name="paxType"></param>
        /// <returns></returns>
        private PassengerType GetPassengerType(string paxType)
        {
            var passengerType = PassengerType.Adult;
            switch (paxType)
            {
                case Constant.Adult:
                    passengerType = PassengerType.Adult;
                    break;

                case Constant.Child:
                    passengerType = PassengerType.Child;
                    break;

                case Constant.Infant:
                    passengerType = PassengerType.Infant;
                    break;

                case Constant.Student:
                    passengerType = PassengerType.Student;
                    break;

                case Constant.Youth:
                    passengerType = PassengerType.Youth;
                    break;

                case Constant.Senior:
                    passengerType = PassengerType.Senior;
                    break;

                    //case Constant.Any:
                    //    passengerType = PassengerType.TwoAndUnder;
                    //    break;

                    //case Constant.Unknown:
                    //    passengerType = PassengerType.Adult;
                    //    break;
            }
            return passengerType;
        }

        /// <summary>
        /// Checking valid pax criteria for RESERVED rate type
        /// </summary>
        /// <param name="availability"></param>
        /// <param name="selectedPax"></param>
        /// <param name="rateType"></param>
        /// <returns></returns>
        public bool CheckPaxForReserveType(Availability availability, int? selectedPax, string rateType) => availability.Capacity < selectedPax && rateType == Constant.ReservedType;

        #endregion Private Methods
    }
}