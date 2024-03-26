using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Service.SupplierServices
{
    public abstract class SupplierServiceBase
    {
        public ActivityOption MapActivityOption(ActivityOption optionFromAPI, ProductOption optionFromCache,
            Criteria criteria)
        {
            return new ActivityOption
            {
                ActivitySeasons = optionFromAPI.ActivitySeasons,
                AvailToken = optionFromAPI.AvailToken,
                Code = optionFromAPI.Code,
                Contract = optionFromAPI.Contract,
                ContractQuestions = optionFromAPI.ContractQuestions,
                HotelPickUpLocation = optionFromAPI.HotelPickUpLocation,
                PickUpOption = optionFromAPI.PickUpOption,
                PickupPointDetails = optionFromAPI.PickupPointDetails,
                PickupPoints = optionFromAPI.PickupPoints,
                PricingCategoryId = optionFromAPI.PricingCategoryId,
                PrioTicketClass = optionFromAPI.PrioTicketClass,
                RateKey = optionFromAPI.RateKey,
                ScheduleReturnDetails = optionFromAPI.ScheduleReturnDetails,
                StartTimeId = optionFromAPI.StartTimeId,
                OptionType = optionFromAPI.OptionType,
                ServiceType = optionFromAPI.ServiceType,
                RoomType = optionFromAPI.RoomType,
                ScheduleId = optionFromAPI.ScheduleId,
                ProductType = optionFromAPI.ProductType,
                RefNo = optionFromAPI.RefNo,
                BasePrice = CalculatePriceForAllPax(optionFromAPI.BasePrice, criteria),
                CostPrice = CalculatePriceForAllPax(optionFromAPI.CostPrice, criteria),
                GateBasePrice = CalculatePriceForAllPax(optionFromAPI.GateBasePrice, criteria),
                AvailabilityStatus = optionFromAPI.AvailabilityStatus,
                Customers = optionFromAPI.Customers,
                TravelInfo = optionFromAPI.TravelInfo,
                CommisionPercent = optionFromAPI.CommisionPercent,
                CancellationPrices = optionFromAPI.CancellationPrices,
                IsSelected = optionFromAPI.IsSelected,
                Id = optionFromCache.Id,
                ServiceOptionId = optionFromCache.Id,
                Name = optionFromCache.Name,
                SupplierName = optionFromCache.SupplierName,
                Description = optionFromCache.Description,
                BookingStatus = optionFromCache.BookingStatus,
                OptionKey = optionFromCache.OptionKey,
                Quantity = optionFromCache.Quantity,
                SupplierOptionCode = optionFromCache.SupplierOptionCode,
                Margin = optionFromCache.Margin,
                OptionOrder = optionFromCache.OptionOrder,
                Variant = optionFromAPI.Variant,
                StartTime = optionFromAPI.StartTime,
                EndTime = optionFromAPI.EndTime,
                OpeningHoursDetails = optionFromAPI.OpeningHoursDetails,
                MeetingPointDetails = optionFromAPI.MeetingPointDetails,
                PickupPointsDetailsForVentrata = optionFromAPI.PickupPointsDetailsForVentrata,
                OfferCode = optionFromAPI.OfferCode,
                OfferTitle = optionFromAPI.OfferTitle,
                VentrataProductId = optionFromAPI.VentrataProductId,
                Cancellable = optionFromAPI.Cancellable,
                CancellationText = optionFromAPI.CancellationText,
                ApiCancellationPolicy = optionFromAPI.ApiCancellationPolicy,
                
            };
        }

        public List<ProductOption> UpdateBasePrices(List<ProductOption> productOptions, string hotelPickupLocation, Criteria criteria)
        {
            if (productOptions == null || (productOptions.Count <= 0)) return productOptions;

            var options = new List<ProductOption>();
            foreach (var option in productOptions)
            {
                var actOption = (ActivityOption)option;
                if (actOption == null) continue;

                actOption.BasePrice = CalculatePriceForAllPax(actOption.BasePrice, criteria);
                actOption.CostPrice = CalculatePriceForAllPax(actOption.CostPrice, criteria);

                actOption.HotelPickUpLocation = hotelPickupLocation;
                options.Add(actOption);
            }
            return options;
        }

        public Price CalculatePriceForAllPax(Price inputPrice, Criteria criteria)
        {
            if (inputPrice == null) return null;
            var price = inputPrice.DeepCopy();

            var isPerUnit = false;
            var perUnitPrice = new decimal();
            var perPersonPrice = new decimal();

            if (price?.DatePriceAndAvailabilty?.Any() == true)
            {
                foreach (var priceAndAvailability in price?.DatePriceAndAvailabilty)
                {
                    perPersonPrice = 0.0M;
                    if (priceAndAvailability.Value?.PricingUnits == null) continue;
                    var pricingUnits = priceAndAvailability.Value.PricingUnits;
                    foreach (var pricingUnit in pricingUnits)
                    {
                        if (pricingUnit is PerUnitPricingUnit)
                        {
                            perUnitPrice = pricingUnit.Price;
                            isPerUnit = true;
                        }
                        else
                        {
                            var pu = ((PerPersonPricingUnit)pricingUnit);
                            var passengerType = pu.PassengerType;
                            var paxCount = pu.UnitType == UnitType.PerPerson ?
                                            GetPaxCountByPaxType(criteria, passengerType) :
                                            priceAndAvailability.Value?.UnitQuantity > 0 ?
                                                priceAndAvailability.Value.UnitQuantity : 1
                                                ;
                            perPersonPrice += pricingUnit.Price * paxCount;
                        }
                    }

                    priceAndAvailability.Value.TotalPrice = isPerUnit ? perUnitPrice : perPersonPrice;
                }
                price.Amount = price.DatePriceAndAvailabilty.
                    Select(x => x.Value.TotalPrice).
                    FirstOrDefault();
            }
            return price;
        }

        public int GetPaxCountByPaxType(Criteria criteria, PassengerType passengerType) => criteria.NoOfPassengers.Where(x => x.Key == passengerType).Select(x => x.Value).FirstOrDefault();
    }
}