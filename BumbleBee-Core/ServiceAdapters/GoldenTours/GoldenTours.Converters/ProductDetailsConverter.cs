using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.GetBookingDates;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.ProductDetails;
using Constant = ServiceAdapters.GoldenTours.Constants.Constant;

namespace ServiceAdapters.GoldenTours.GoldenTours.Converters
{
    public class ProductDetailsConverter : ConverterBase, IProductDetailsConverter
    {
        public ProductDetailsConverter(ILogger logger) : base(logger)
        {
        }

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
            var result = DeSerializeXml<ProductDetailsResponse>(response as string);
            if (result == null || extraRequest == null) return null;

            var productOptions = ConvertAvailabilityResult(result, extraRequest as GetBookingDatesResponse,
                request as GoldenToursCriteria);
            return productOptions;
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="extraRequest"></param>
        /// <param name="extraResponse"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request, T extraRequest, T extraResponse)
        {
            var result = DeSerializeXml<ProductDetailsResponse>(response as string);
            if (result == null || extraRequest == null) return null;

            var productOptions = ConvertDateAndAvailabilityResult(result, request as GoldenToursCriteria,
                extraRequest as List<DateTime>, extraResponse as GetBookingDatesResponse);
            return productOptions;
        }

        #region Private Methods

        /// <summary>
        /// Map the product options from the supplier response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="getBookingDatesResponse"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<ProductOption> ConvertAvailabilityResult(ProductDetailsResponse result, GetBookingDatesResponse getBookingDatesResponse, GoldenToursCriteria criteria)
        {
            var pricePeriods = result?.Product?.Priceperiods?.Period;
            if (result == null || pricePeriods == null) return null;

            //Check for min pax and max pax and filter the option on the basis of criteria.
            var maxPaxCount = System.Convert.ToInt32(pricePeriods.FirstOrDefault().Maximum_pax);
            if (System.Convert.ToInt32(pricePeriods.FirstOrDefault().Minimum_pax) > criteria.NoOfPassengers.Sum(thisPass => thisPass.Value)) { return null; }
            if (maxPaxCount == 0) { maxPaxCount = 20; }
            if (maxPaxCount < criteria.NoOfPassengers.Sum(thisPass => thisPass.Value)) { return null; }

            var defaultCapacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
            var productOptions = new List<ProductOption>();
            ActivityOption option = null;
            var monthYearCheckin = $"{criteria.CheckinDate.ToString(Constant.MonthFormat)} {criteria.CheckinDate.Year}";

            foreach (var period in pricePeriods)
            {
                var days = period.Days.Replace(" ", "").Split(',');
                var startDate = DateTime.Parse(period.Start_date);
                var endDate = DateTime.Parse(period.End_date);
                var scheduleId = period.Schedule?.Id;
                option = new ActivityOption
                {
                    // Adding the Schedule Name in option name, this will get concatenate with original option name in the activity service
                    TravelInfo = new TravelInfo
                    {
                        StartDate = criteria.CheckinDate.Date,
                        NoOfPassengers = criteria.NoOfPassengers,
                        Ages = criteria.Ages
                    },
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    ScheduleId = scheduleId,
                    ProductType = result.Product?.Producttype,
                    SupplierOptionCode = result.Query.Productid,
                    RefNo = result.Product?.Ref,
                    ContractQuestions = MapContractQuestions(result.Product?.Transferoptions?.Transferoption),
                    PickupLocations = MapPickupLocations(result.Product?.Pickups, scheduleId),
                    Variant = period?.Schedule?.Text
                };

                var actualDates = GetDates(criteria.CheckinDate, criteria.CheckoutDate);
                var DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();
                foreach (var actualDate in actualDates)
                {
                    var travelDay = actualDate.DayOfWeek.ToString().Substring(0, 3);
                    if (actualDate >= startDate && actualDate <= endDate && days.Contains(travelDay))
                    {
                        var monthYearActualDate = $"{actualDate.ToString(Constant.MonthFormat)} {actualDate.Year}";

                        // Fetching booking days from the month year that matches the check in date
                        var bookingDays = getBookingDatesResponse?.Dates?.Monthyear
                            ?.FirstOrDefault(x => x.Value.Equals(monthYearActualDate, StringComparison.InvariantCultureIgnoreCase))
                            ?.Days?.Values;

                        if (bookingDays == null) continue;

                        var actualDateDay = System.Convert.ToString(actualDate.Day).ToLowerInvariant();

                        // Fetching the available pax from the month year which matches the checkIn date
                        var availablePax = System.Convert.ToInt32(bookingDays.FirstOrDefault(x => x.Day.ToLowerInvariant().Equals(actualDateDay))?.Availability.FirstOrDefault(x => x.Schedule?.Id == scheduleId)?.Availablepax);

                        // If availablePax value is -1 i.e unlimited then set default capacity
                        availablePax = availablePax == -1 ? defaultCapacity : availablePax;

                        var selectedPax = criteria.NoOfPassengers?.Sum(x => x.Value);

                        //availablePax -1 means unlimited availability, if its 0 then its not available
                        if (availablePax >= selectedPax || availablePax < 0)
                        {
                            var units = period?.Priceunits?.Unit;
                            if (units == null) continue;

                            var pricingUnits = CreatePricingUnitsFromSupplierResponse(units, criteria);

                            // Continue if the pax criteria is not valid
                            var isValidPaxCriteria = CheckValidPaxCriteria(criteria.NoOfPassengers, pricingUnits, option.ProductType);
                            if (!isValidPaxCriteria)
                                continue;

                            var basePriceAndAvailability = new DefaultPriceAndAvailability
                            {
                                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                                TotalPrice = pricingUnits.Sum(x => x.Price),
                                PricingUnits = pricingUnits,
                                Capacity = availablePax,
                                IsCapacityCheckRequired = true
                            };

                            if (!DatePriceAndAvailabilty.ContainsKey(actualDate))
                            {
                                DatePriceAndAvailabilty.Add(actualDate, basePriceAndAvailability);
                            }
                        }
                    }
                }
                if (option != null && DatePriceAndAvailabilty.Any())
                {
                    option.BasePrice = new Price
                    {
                        Amount = DatePriceAndAvailabilty.FirstOrDefault().Value.TotalPrice,
                        // adding hard coded currency as this is the default currency that GT supports and we are not getting any currency from the response
                        Currency = new Currency { IsoCode = Constant.Gbp },
                        DatePriceAndAvailabilty = DatePriceAndAvailabilty
                    };
                    option.GateBasePrice = option.BasePrice;
                    productOptions.Add(option);
                }
            }
            return productOptions;
        }

        public DateTime[] GetDates(DateTime startDate, DateTime endDate)
        {
            var allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }

        /// <summary>
        /// Pass the transfer options in the Contract Questions
        /// </summary>
        /// <param name="transferOptions"></param>
        /// <returns></returns>
        private List<ContractQuestion> MapContractQuestions(List<Transferoption> transferOptions)
        {
            if (transferOptions == null) return null;

            var contractQuestions = new List<ContractQuestion>();
            foreach (var transferOption in transferOptions)
            {
                if (transferOption.Required.ToUpperInvariant() == Constant.No) continue;

                var contractQuestion = new ContractQuestion
                {
                    Name = transferOption.Text,
                    IsRequired = true
                };
                contractQuestions.Add(contractQuestion);
            }
            return contractQuestions;
        }

        /// <summary>
        /// Create pricing units from the supplier response
        /// </summary>
        /// <param name="units"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<PricingUnit> CreatePricingUnitsFromSupplierResponse(List<Unit> units, GoldenToursCriteria criteria)
        {
            var pricingUnits = new List<PricingUnit>();
            foreach (var unit in units)
            {
                var unitId = System.Convert.ToInt32(unit.Id);
                var passengerTypeId = criteria.PassengerMappings
                    .FirstOrDefault(x => x.SupplierPassengerTypeId.Equals(unitId))?.PassengerTypeId;
                if (passengerTypeId == null) continue;

                var price = System.Convert.ToDecimal(unit.Price);
                var passengerType = (PassengerType)passengerTypeId;
                var pricingUnit = CreatePricingUnit(passengerType, price, criteria);

                if (pricingUnit != null)
                    pricingUnits.Add(pricingUnit);
            }

            var unavailablePassengerTypes = GetUnavailablePassengerTypes(criteria.NoOfPassengers, pricingUnits);
            var containsInfant = unavailablePassengerTypes.Contains(PassengerType.Infant);
            if (containsInfant)
            {
                var infantPriceUnit = CreatePricingUnit(PassengerType.Infant, 0, criteria);
                infantPriceUnit.SupportedByIsangoOnly = true;
                pricingUnits.Add(infantPriceUnit);
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
        private PricingUnit CreatePricingUnit(PassengerType passengerType, decimal price, GoldenToursCriteria criteria)
        {
            PricingUnit pricingUnit = null;
            var paxCount = criteria.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(passengerType)).Value;
            if (paxCount > 0)
            {
                pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
                pricingUnit.Price = price;
            }
            return pricingUnit;
        }

        /// <summary>
        /// Map the product options from the supplier response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="criteria"></param>
        /// <param name="availableDates"></param>
        /// <param name="bookingDatesResponse"></param>
        /// <returns></returns>
        private List<ProductOption> ConvertDateAndAvailabilityResult(ProductDetailsResponse result, GoldenToursCriteria criteria, List<DateTime> availableDates, GetBookingDatesResponse bookingDatesResponse)
        {
            var pricePeriods = result?.Product?.Priceperiods?.Period;
            if (result == null || pricePeriods == null) return null;

            var defaultCapacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
            var productOptions = new List<ProductOption>();
            foreach (var period in pricePeriods)
            {
                if (period == null) continue;
                var option = new ActivityOption
                {
                    TravelInfo = new TravelInfo
                    {
                        StartDate = criteria.CheckinDate,
                        NoOfPassengers = criteria.NoOfPassengers,
                        Ages = criteria.Ages
                    },
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    ProductType = result.Product?.Producttype,
                    SupplierOptionCode = result.Query.Productid,
                    RefNo = result.Product?.Ref,
                    ContractQuestions = MapContractQuestions(result.Product?.Transferoptions?.Transferoption),
                    PickupLocations = MapPickupLocations(result.Product?.Pickups, period?.Schedule?.Id),
                    Variant = period?.Schedule?.Text
                };

				// If Minimum Pax from the supplier response are more than 1 than passing it in the TravelInfo as that value will be used dump Minimum Pax in the DB. Price will be PerAdult only.
				var minimumPax = System.Convert.ToInt32(period.Minimum_pax);
				if (minimumPax > 1)
				{
					var passengerTypes = option.TravelInfo.NoOfPassengers.Select(x => x.Key).ToList();
					foreach (var passengerType in passengerTypes)
					{
						option.TravelInfo.NoOfPassengers[passengerType] = minimumPax;
					}
				}

				var datePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();
				foreach (var availableDate in availableDates)
				{
					var basePriceAndAvailability = new DefaultPriceAndAvailability();
					var startDate = DateTime.Parse(period.Start_date);
					var endDate = DateTime.Parse(period.End_date);

                    if (availableDate.Date >= startDate && availableDate.Date <= endDate)
                    {
                        var monthYearCheckIn = $"{availableDate.ToString(Constant.MonthFormat)} {availableDate.Year}";
                        var bookingDays = bookingDatesResponse?.Dates?.Monthyear
                            ?.FirstOrDefault(x => x.Value.Equals(monthYearCheckIn, StringComparison.InvariantCultureIgnoreCase))
                            ?.Days?.Values;

                        if (bookingDays == null) continue;

                        var checkInDate = System.Convert.ToString(availableDate.Day).ToLowerInvariant();
                        // Fetching the available pax from the month year which matches the checkIn date
                        var availablePax = System.Convert.ToInt32(bookingDays.FirstOrDefault(x => x.Day.ToLowerInvariant().Equals(checkInDate))?.Availability.FirstOrDefault(x => x.Schedule?.Id == period.Schedule?.Id)?.Availablepax);

                        // If availablePax value is -1 i.e unlimited then set default capacity
                        availablePax = availablePax == -1 ? defaultCapacity : availablePax;

                        var units = period?.Priceunits?.Unit;
                        if (units == null) continue;

						var pricingUnits = CreatePricingUnitsFromSupplierResponse(units, criteria);

                        basePriceAndAvailability.Capacity = availablePax;
                        basePriceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        basePriceAndAvailability.TotalPrice = pricingUnits.Sum(x => x.Price);
                        basePriceAndAvailability.PricingUnits = pricingUnits;

                        if (!datePriceAndAvailabilty.Keys.Contains(availableDate) && basePriceAndAvailability.TotalPrice > 0)
                            datePriceAndAvailabilty.Add(availableDate, basePriceAndAvailability);
                    }
                }
                option.BasePrice = new Price
                {
                    Currency = new Currency { IsoCode = Constant.Gbp },
                    DatePriceAndAvailabilty = datePriceAndAvailabilty
                };
                if (datePriceAndAvailabilty.Count > 0)
                    productOptions.Add(option);
            }
            return productOptions;
        }

        /// <summary>
        /// Map pickup place Id and text
        /// </summary>
        /// <param name="pickupLocations"></param>
        /// <returns></returns>
        private Dictionary<int, string> MapPickupLocations(Pickups pickups, string scheduleId)
        {
            if (pickups == null || pickups.Pickup.Count == 0) return null;

            var pickupLocations = new Dictionary<int, string>();
            List<Pickup> pickupList;

            var scheduleWisePickup = pickups?.Pickup?
                .Where(x => x.ScheduleId != null)?
                .ToList();

            if (scheduleId != null && scheduleWisePickup?.Count > 0)
            {
                pickupList = scheduleWisePickup?
                    .Where(x => x.ScheduleId == scheduleId)?
                    .ToList();
            }
            else
            {
                pickupList = pickups.Pickup;
            }

            foreach (var pickup in pickupList)
            {
                var pickupTimeId = System.Convert.ToInt32(pickup.Time.Id);
                var pickupDescription = $"{pickup.Title.Trim()} @{pickup.Time.Text.Trim()}|{pickup.Address.Trim()}|{pickup.Postcode.Trim()}";
                if (!pickupLocations.Keys.Contains(pickupTimeId))
                    pickupLocations.Add(pickupTimeId, pickupDescription);
            }
            return pickupLocations;
        }

        /// <summary>
        /// Retrieve unavailable passengers by comparing the selected passengers and supplier passengers
        /// </summary>
        /// <param name="noOfPassengers"></param>
        /// <param name="pricingUnits"></param>
        /// <returns></returns>
        private List<PassengerType> GetUnavailablePassengerTypes(Dictionary<PassengerType, int> noOfPassengers, List<PricingUnit> pricingUnits)
        {
            var selectedPassengers = noOfPassengers.Where(x => x.Value > 0).Select(x => x.Key).ToList();
            var availablePassengers = pricingUnits.Select(x => (PerPersonPricingUnit)x).Select(x => x.PassengerType).ToList();
            return selectedPassengers.Except(availablePassengers).ToList();
        }

        /// <summary>
        /// Check if the pax criteria passed from the UI is valid or not
        /// </summary>
        /// <param name="noOfPassengers"></param>
        /// <param name="pricingUnits"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        private bool CheckValidPaxCriteria(Dictionary<PassengerType, int> noOfPassengers, List<PricingUnit> pricingUnits, string productType)
        {
            var isValidForTransferProduct = true;
            // Checking pax criteria for Tranfer product type
            if (string.Equals(productType, Constant.Transfers, StringComparison.InvariantCultureIgnoreCase))
            {
                var selectedPassengers = noOfPassengers.Count(x => x.Value > 0);
                var anyInvalidPaxCount = noOfPassengers.Any(x => x.Value > 1);

                // As only one pax type with count 1 is allowed for Transfer products, so below are the checks for the same
                if (selectedPassengers > 1 || anyInvalidPaxCount)
                    isValidForTransferProduct = false;
            }

            // Return false if the pricing units created does not contains data for all the selected passenger or is invalid for the tansfer product
            var unavailablePassengerTypes = GetUnavailablePassengerTypes(noOfPassengers, pricingUnits);
            if (unavailablePassengerTypes.Count > 0 || !isValidForTransferProduct)
                return false;

            return true;
        }

        #endregion Private Methods
    }
}