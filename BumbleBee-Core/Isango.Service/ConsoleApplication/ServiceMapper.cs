using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Service.ConsoleApplication
{
    public class ServiceMapper
    {
        /// <summary>
        /// Process service details for the Suppliers that provides cost price
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="mappedProduct"></param>
        /// <returns></returns>
        public List<TempHBServiceDetail> ProcessServiceDetailsWithCostPrice(Activity activity, IsangoHBProductMapping mappedProduct)
        {
            var options = activity?.ProductOptions;
            if (options == null) return null;
            var exceptionInfo = new List<string>();
            var hbServiceDetails = new List<TempHBServiceDetail>();
            foreach (var option in options)
            {
                try
                {
                    var activityOption = ((ActivityOption)option);
                    var costDatePriceAndAvailabilities =
                        activityOption?.CostPrice?.DatePriceAndAvailabilty;
                    var sellDatePriceAndAvailabilities =
                        activityOption?.SellPrice?.DatePriceAndAvailabilty;
                    var sellPrice = default(decimal);
                    if (costDatePriceAndAvailabilities == null) continue;

                    foreach (var datePriceAndAvailability in costDatePriceAndAvailabilities)
                    {
                        var pricingUnits = datePriceAndAvailability.Value.PricingUnits;
                        foreach (var pricingUnit in pricingUnits)
                        {
                            var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;

                            var paxCount = option.TravelInfo?.NoOfPassengers?.FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;

                            //Default 20% margin amount
                            mappedProduct.MarginAmount = mappedProduct.MarginAmount > 0 ? mappedProduct.MarginAmount : 20;
                            //Price related changes
                            var costPrice = perPersonPricingUnit.Price;
                            var basePrice = CalculateBasePrice(costPrice, mappedProduct.MarginAmount);
                            var passengerType = perPersonPricingUnit.PassengerType;

                            if (activityOption.IsMandatoryApplyAmount || mappedProduct.ApiType == APIType.Hotelbeds)
                            {
                                try
                                {
                                    if (sellDatePriceAndAvailabilities?.Count > 0 && sellDatePriceAndAvailabilities.Keys.Contains(datePriceAndAvailability.Key))
                                    {
                                        var daysSellPrice = sellDatePriceAndAvailabilities[datePriceAndAvailability.Key];
                                        var sellPricingUnit = daysSellPrice?.PricingUnits;
                                        //sellPrice = sellPricingUnit.FirstOrDefault(x => ((PerPersonPricingUnit)x).PassengerType == PassengerType.Adult)?.Price ?? 0;
                                        sellPrice = sellPricingUnit.FirstOrDefault(x => ((PerPersonPricingUnit)x).PassengerType == passengerType)?.Price ?? 0;
                                        basePrice = sellPrice;
                                    }
                                }
                                catch
                                {
                                }
                            }
                            var detail = new TempHBServiceDetail
                            {
                                ProductCode = activity.Code,
                                Modality = activityOption.Code,
                                AvailableOn = datePriceAndAvailability.Key,
                                Currency = activity.CurrencyIsoCode,
                                FactSheetID = activity.FactsheetId,
                                ProductClass = activity.DurationString,
                                Status = datePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                ServiceOptionID = option.ServiceOptionId,
                                MinAdult = paxCount,
                                PassengerTypeId = (int)passengerType,
                                Variant = option.Variant,
                                StartTime = option.StartTime,
                                TicketOfficePrice = basePrice,

                                //##Merging Check
                                SellPrice = ((activityOption.IsMandatoryApplyAmount
                                                || mappedProduct.ApiType == APIType.Hotelbeds)
                                                && sellPrice > 0
                                                && sellPrice > costPrice) ?
                                        Convert.ToDecimal(sellPrice)
                                        : basePrice,

                                Price = costPrice,
                                UnitType = pricingUnit.UnitType.ToString(),
                                Capacity = option.Capacity,

                                //For APITude Only
                                IsFinalGateAmount = activityOption.IsMandatoryApplyAmount,
                                CancellationPolicy = activityOption.CancellationText?.Trim(),
                            };

                            if (detail.Price > 0 || detail.PassengerTypeId == 2 || detail.PassengerTypeId == 9) //- all passenger details should be added
                                hbServiceDetails.Add(detail);
                        }
                    }
                }
                catch
                {
                }
            }

            return hbServiceDetails;
        }

        /// <summary>
        /// Process service details for the Suppliers that provides base price
        /// This method processes the details based on base price
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<TempHBServiceDetail> ProcessServiceDetailsWithBasePrice(Activity activity, List<IsangoHBProductMapping> mappedProducts)
        {
            var options = activity?.ProductOptions;

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (options == null) return null;
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null

            var hbServiceDetails = new List<TempHBServiceDetail>();

            foreach (var option in options)
            {
                try
                {
                    var activityOption = option as ActivityOption;
                    var mappedProduct = mappedProducts.FirstOrDefault().ApiType == APIType.Fareharbor ? mappedProducts?.Where(x => x.ServiceOptionInServiceid == option.Id || x.ServiceOptionInServiceid == option.ServiceOptionId)?.FirstOrDefault() : mappedProducts.FirstOrDefault(x => x.HotelBedsActivityCode == option.SupplierOptionCode);
                    if (mappedProduct.ApiType == APIType.Bokun)
                    {
                        mappedProduct = mappedProducts.FirstOrDefault(x => x.HotelBedsActivityCode == option.SupplierOptionCode
                        && (
                            !string.IsNullOrWhiteSpace(x.PrefixServiceCode) ?
                            (x.PrefixServiceCode == option.PrefixServiceCode)
                            : true
                           )
                        );
                    }
                    if (mappedProduct == null) continue;
                    option.Id = option.ServiceOptionId = mappedProduct.ServiceOptionInServiceid;
                    //Note: In Existing Code, SellPrice = BasePrice and BasePrice = CostPrice
                    var baseDatePriceAndAvailabilities =
                        activityOption?.BasePrice?.DatePriceAndAvailabilty;
                    if (baseDatePriceAndAvailabilities?.Count > 0)
                    {
                        foreach (var datePriceAndAvailability in baseDatePriceAndAvailabilities)
                        {
                            var pricingUnits = datePriceAndAvailability.Value.PricingUnits;
                            if (pricingUnits == null)
                                continue;

                            foreach (var pricingUnit in pricingUnits)
                            {
                                try
                                {
                                    var capacity = datePriceAndAvailability.Value.Capacity;
                                    var paxCount = 0;
                                    var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;
                                    var basePrice = perPersonPricingUnit.Price;
                                    var costPrice = CalculateCostPrice(basePrice, option.CommisionPercent);
                                    var passengerType = perPersonPricingUnit.PassengerType;
                                    if (capacity == 0)
                                        capacity = pricingUnit.TotalCapacity;
                                    if (pricingUnit.UnitType == UnitType.PerUnit && pricingUnit.PriceType == PriceType.PerUnit
                                        && (mappedProduct.ApiType == APIType.Bokun || mappedProduct.ApiType == APIType.Rezdy
                                        || mappedProduct.ApiType == APIType.TourCMS))
                                    {
                                        paxCount = perPersonPricingUnit.Mincapacity ?? 1;
                                    }
                                    else if (pricingUnit.UnitType == UnitType.PerPerson)
                                    {
                                        if (perPersonPricingUnit.Mincapacity > 1 && mappedProduct.ApiType == APIType.Rezdy)
                                        {
                                            paxCount = perPersonPricingUnit.Mincapacity ?? 1;
                                        }
                                        else
                                        {
                                            paxCount = option.TravelInfo?.NoOfPassengers?.
                                            FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;
                                        }
                                    }
                                    var detail = new TempHBServiceDetail
                                    {
                                        ProductCode = activity.Code,
                                        Modality = activityOption?.Code,
                                        AvailableOn = datePriceAndAvailability.Key,
                                        Currency = activity.CurrencyIsoCode,
                                        FactSheetID = activity.FactsheetId,
                                        ProductClass = activity.DurationString,
                                        ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                        CommissionPercent = option.CommisionPercent,
                                        Status = datePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                        ServiceOptionID = mappedProduct.ServiceOptionInServiceid,
                                        MinAdult = paxCount,
                                        PassengerTypeId = (int)passengerType,
                                        Variant = option.Variant,
                                        StartTime = option.StartTime,
                                        TicketOfficePrice = basePrice,
                                        SellPrice = basePrice,
                                        Price = costPrice,
                                        UnitType = pricingUnit.UnitType.ToString(),
                                        Capacity = capacity,
                                        CancellationPolicy = activityOption.CancellationText?.Trim(),
                                        supplieroptionname = activityOption.SupplierOptionNote
                                    };

                                    if (mappedProduct.ApiType == APIType.Bokun)
                                    {
                                        detail = UpdateDetailsAsPerAPI(detail, activityOption, mappedProduct);
                                    }
                                    if (detail.Price > 0)
                                        hbServiceDetails.Add(detail);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return hbServiceDetails;
        }

        public List<TempHBServiceDetail> ProcessServiceDetailsWithBasePricePrioHub(Activity activity, ProductOption Option, List<IsangoHBProductMapping> mappedProducts)
        {

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (Option == null) return null;
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null

            var hbServiceDetails = new List<TempHBServiceDetail>();

            try
            {
                var activityOption = Option as ActivityOption;
                var mappedProduct = mappedProducts.FirstOrDefault().ApiType == APIType.Fareharbor ? mappedProducts.FirstOrDefault() : mappedProducts.FirstOrDefault(x => x.HotelBedsActivityCode == Option.SupplierOptionCode);
                if (mappedProduct.ApiType == APIType.Bokun)
                {
                    mappedProduct = mappedProducts.FirstOrDefault(x => x.HotelBedsActivityCode == Option.SupplierOptionCode
                    && (
                        !string.IsNullOrWhiteSpace(x.PrefixServiceCode) ?
                        (x.PrefixServiceCode == Option.PrefixServiceCode)
                        : true
                       )
                    );
                }
                if (mappedProduct == null) return null;
                Option.Id = Option.ServiceOptionId = mappedProduct.ServiceOptionInServiceid;
                //Note: In Existing Code, SellPrice = BasePrice and BasePrice = CostPrice
                var baseDatePriceAndAvailabilities =
                    activityOption?.BasePrice?.DatePriceAndAvailabilty;
                if (baseDatePriceAndAvailabilities?.Count > 0)
                {
                    foreach (var datePriceAndAvailability in baseDatePriceAndAvailabilities)
                    {
                        var pricingUnits = datePriceAndAvailability.Value.PricingUnits;
                        if (pricingUnits == null)
                            continue;

                        foreach (var pricingUnit in pricingUnits)
                        {
                            try
                            {
                                var capacity = datePriceAndAvailability.Value.Capacity;
                                var paxCount = 0;
                                var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;
                                var basePrice = perPersonPricingUnit.Price;
                                var costPrice = CalculateCostPrice(basePrice, mappedProduct.MarginAmount);
                                var passengerType = perPersonPricingUnit.PassengerType;
                                if (capacity == 0)
                                    capacity = pricingUnit.TotalCapacity;
                                if (pricingUnit.UnitType == UnitType.PerUnit && pricingUnit.PriceType == PriceType.PerUnit
                                    && (mappedProduct.ApiType == APIType.Bokun || mappedProduct.ApiType == APIType.Rezdy
                                    || mappedProduct.ApiType == APIType.TourCMS))
                                {
                                    paxCount = perPersonPricingUnit.Mincapacity ?? 1;
                                }
                                else if (pricingUnit.UnitType == UnitType.PerPerson)
                                {
                                    if (perPersonPricingUnit.Mincapacity > 1 && mappedProduct.ApiType == APIType.Rezdy)
                                    {
                                        paxCount = perPersonPricingUnit.Mincapacity ?? 1;
                                    }
                                    else
                                    {
                                        paxCount = Option.TravelInfo?.NoOfPassengers?.
                                        FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;
                                    }
                                }
                                var detail = new TempHBServiceDetail
                                {
                                    ProductCode = activity.Code,
                                    Modality = activityOption?.Code,
                                    AvailableOn = datePriceAndAvailability.Key,
                                    Currency = activity.CurrencyIsoCode,
                                    FactSheetID = activity.FactsheetId,
                                    ProductClass = activity.DurationString,
                                    ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                    CommissionPercent = mappedProduct.MarginAmount,
                                    Status = datePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                    ServiceOptionID = mappedProduct.ServiceOptionInServiceid,
                                    MinAdult = paxCount,
                                    PassengerTypeId = (int)passengerType,
                                    Variant = Option.Variant,
                                    StartTime = Option.StartTime,
                                    TicketOfficePrice = basePrice,
                                    SellPrice = basePrice,
                                    Price = costPrice,
                                    UnitType = pricingUnit.UnitType.ToString(),
                                    Capacity = capacity,
                                    CancellationPolicy = activityOption.CancellationText?.Trim(),
                                };

                                if (mappedProduct.ApiType == APIType.Bokun)
                                {
                                    detail = UpdateDetailsAsPerAPI(detail, activityOption, mappedProduct);
                                }
                                if (detail.Price > 0)
                                    hbServiceDetails.Add(detail);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return hbServiceDetails;
        }

        /// <summary>
        /// Process the service details based on base price and cost price
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="mappedProduct"></param>
        /// <returns></returns>
        public List<TempHBServiceDetail> ProcessServiceDetailsWithBaseAndCostPrices(Activity activity, IsangoHBProductMapping mappedProduct)
        {
            var options = activity?.ProductOptions;
            if (options == null) return null;

            var serviceDetails = new List<TempHBServiceDetail>();
            foreach (var option in options)
            {
                try
                {
                    var activityOption = option as ActivityOption;
                    if (activity.ApiType == APIType.GlobalTix || activity.ApiType == APIType.GlobalTixV3)
                    {
                        if (activityOption.Code != mappedProduct.HotelBedsActivityCode)
                        {
                            continue;
                        }
                    }
                    //Note: In Existing Code, SellPrice = BasePrice and BasePrice = CostPrice
                    var baseDatePriceAndAvailabilities =
                        activityOption?.BasePrice?.DatePriceAndAvailabilty;
                    var costPriceDatePriceAndAvailability =
                        activityOption?.CostPrice?.DatePriceAndAvailabilty;
                    if (baseDatePriceAndAvailabilities == null) continue;

                    foreach (var basePriceAndAvailability in baseDatePriceAndAvailabilities)
                    {
                        var pricingUnits = basePriceAndAvailability.Value.PricingUnits;
                        if (pricingUnits == null)
                            continue;

                        var capacity = basePriceAndAvailability.Value.Capacity;
                        foreach (var pricingUnit in pricingUnits)
                        {
                            var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;
                            var passengerType = perPersonPricingUnit.PassengerType;
                            var paxCount = option.TravelInfo?.NoOfPassengers?.
                                FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;
                            var basePrice = perPersonPricingUnit.Price;
                            var costPrice = costPriceDatePriceAndAvailability
                                ?.FirstOrDefault(w => w.Key.Equals(basePriceAndAvailability.Key)).Value?.PricingUnits?.FirstOrDefault(
                                    x => ((PerPersonPricingUnit)x).PassengerType.Equals(passengerType))?.Price ?? 0;

                            if (capacity == 0)
                                capacity = pricingUnit.TotalCapacity;

                            var detail = new TempHBServiceDetail
                            {
                                ProductCode = activity.Code,
                                Modality = activityOption.Code,
                                AvailableOn = basePriceAndAvailability.Key,
                                Currency = activity.CurrencyIsoCode,
                                FactSheetID = activity.FactsheetId,
                                ProductClass = activity.DurationString,
                                ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                CommissionPercent = option.CommisionPercent,
                                Status = basePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                ServiceOptionID = option.Id,
                                MinAdult = paxCount,
                                PassengerTypeId = (int)passengerType,
                                StartTime = option.StartTime,
                                TicketOfficePrice = basePrice,
                                SellPrice = basePrice,
                                Price = costPrice,
                                UnitType = pricingUnit.UnitType.ToString(),
                                Capacity = capacity,
                                CancellationPolicy = activityOption.CancellationText?.Trim(),
                            };
                            if (detail.Price > 0)
                                serviceDetails.Add(detail);
                        }
                    }
                }
                catch
                {
                }
            }

            return serviceDetails;
        }

        public List<TempHBServiceDetail> ProcessServiceDetailsWithBaseGateBaseAndCostPrices(Activity activity, IsangoHBProductMapping mappedProduct)
        {
            var options = activity?.ProductOptions;
            if (options == null) return null;

            var serviceDetails = new List<TempHBServiceDetail>();
            foreach (var option in options)
            {
                try
                {
                    var activityOption = option as ActivityOption;
                    //Note: In Existing Code, SellPrice = BasePrice and BasePrice = CostPrice
                    var baseDatePriceAndAvailabilities =
                        activityOption?.BasePrice?.DatePriceAndAvailabilty;
                    var costPriceDatePriceAndAvailability =
                        activityOption?.CostPrice?.DatePriceAndAvailabilty;
                    var gateBasePriceDatePriceAndAvailability =
                        activityOption?.GateBasePrice?.DatePriceAndAvailabilty;
                    if (baseDatePriceAndAvailabilities == null) continue;

                    foreach (var basePriceAndAvailability in baseDatePriceAndAvailabilities)
                    {
                        var pricingUnits = basePriceAndAvailability.Value.PricingUnits;
                        if (pricingUnits == null)
                            continue;

                        var capacity = basePriceAndAvailability.Value.Capacity;
                        foreach (var pricingUnit in pricingUnits)
                        {
                            try
                            {
                                var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;
                                var passengerType = perPersonPricingUnit.PassengerType;
                                var paxCount = option.TravelInfo?.NoOfPassengers?.
                                    FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;
                                var basePrice = perPersonPricingUnit.Price;
                                var currencyCode = perPersonPricingUnit.Currency;
                                var costPrice = costPriceDatePriceAndAvailability
                                    ?.FirstOrDefault(w => w.Key.Equals(basePriceAndAvailability.Key)).Value?.PricingUnits?.FirstOrDefault(
                                        x => ((PerPersonPricingUnit)x).PassengerType.Equals(passengerType))?.Price ?? 0;
                                var gateBasePrice = gateBasePriceDatePriceAndAvailability
                                    ?.FirstOrDefault(w => w.Key.Equals(basePriceAndAvailability.Key)).Value?.PricingUnits?.FirstOrDefault(
                                        x => ((PerPersonPricingUnit)x).PassengerType.Equals(passengerType))?.Price ?? 0;

                                if (capacity == 0)
                                    capacity = pricingUnit.TotalCapacity;

                                var detail = new TempHBServiceDetail
                                {
                                    ProductCode = activityOption.PrefixServiceCode,
                                    Modality = activityOption.SupplierOptionCode,
                                    AvailableOn = basePriceAndAvailability.Key,
                                    Currency = currencyCode,
                                    FactSheetID = activity.FactsheetId,
                                    ProductClass = activity.DurationString,
                                    ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                    CommissionPercent = option.CommisionPercent,
                                    Status = basePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                    ServiceOptionID = (mappedProduct.ApiType == APIType.NewCitySightSeeing || mappedProduct.ApiType == APIType.PrioHub)
                                    ? option.ServiceOptionId : ((ActivityOption)option).Id,
                                    MinAdult = paxCount,
                                    PassengerTypeId = (int)passengerType,
                                    StartTime = option.StartTime,
                                    TicketOfficePrice = gateBasePrice,
                                    SellPrice = basePrice,
                                    Price = costPrice,
                                    UnitType = pricingUnit.UnitType.ToString(),
                                    Capacity = capacity,
                                    CancellationPolicy = activityOption.CancellationText?.Trim(),
                                };



                                if (detail.Price > 0)
                                    serviceDetails.Add(detail);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return serviceDetails;
        }

        public List<TempHBServiceDetail> ProcessServiceDetailsWithBaseGateBaseAndCostPricesPrioHub(Activity activity, ProductOption Option, IsangoHBProductMapping mappedProduct)
        {
            if (Option == null) return null;

            var serviceDetails = new List<TempHBServiceDetail>();
            try
            {
                var activityOption = Option as ActivityOption;
                //Note: In Existing Code, SellPrice = BasePrice and BasePrice = CostPrice
                var baseDatePriceAndAvailabilities =
                    activityOption?.BasePrice?.DatePriceAndAvailabilty;
                var costPriceDatePriceAndAvailability =
                    activityOption?.CostPrice?.DatePriceAndAvailabilty;
                var gateBasePriceDatePriceAndAvailability =
                    activityOption?.GateBasePrice?.DatePriceAndAvailabilty;
                if (baseDatePriceAndAvailabilities == null) return null;

                foreach (var basePriceAndAvailability in baseDatePriceAndAvailabilities)
                {
                    var pricingUnits = basePriceAndAvailability.Value.PricingUnits;
                    if (pricingUnits == null)
                        continue;

                    var capacity = basePriceAndAvailability.Value.Capacity;
                    foreach (var pricingUnit in pricingUnits)
                    {
                        try
                        {
                            var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;
                            var passengerType = perPersonPricingUnit.PassengerType;
                            var paxCount = Option.TravelInfo?.NoOfPassengers?.
                                FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;
                            var basePrice = perPersonPricingUnit.Price;
                            var currencyCode = perPersonPricingUnit.Currency;
                            var costPrice = costPriceDatePriceAndAvailability
                                ?.FirstOrDefault(w => w.Key.Equals(basePriceAndAvailability.Key)).Value?.PricingUnits?.FirstOrDefault(
                                    x => ((PerPersonPricingUnit)x).PassengerType.Equals(passengerType))?.Price ?? 0;
                            var gateBasePrice = gateBasePriceDatePriceAndAvailability
                                ?.FirstOrDefault(w => w.Key.Equals(basePriceAndAvailability.Key)).Value?.PricingUnits?.FirstOrDefault(
                                    x => ((PerPersonPricingUnit)x).PassengerType.Equals(passengerType))?.Price ?? 0;

                            if (capacity == 0)
                                capacity = pricingUnit.TotalCapacity;

                            var detail = new TempHBServiceDetail
                            {
                                ProductCode = activityOption.PrefixServiceCode,
                                Modality = activityOption.SupplierOptionCode,
                                AvailableOn = basePriceAndAvailability.Key,
                                Currency = currencyCode,
                                FactSheetID = activity.FactsheetId,
                                ProductClass = activity.DurationString,
                                ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                CommissionPercent = Option.CommisionPercent,
                                Status = basePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                ServiceOptionID = (mappedProduct.ApiType == APIType.NewCitySightSeeing || mappedProduct.ApiType == APIType.PrioHub)
                                ? Option.ServiceOptionId : ((ActivityOption)Option).Id,
                                MinAdult = paxCount,
                                PassengerTypeId = (int)passengerType,
                                StartTime = Option.StartTime,
                                TicketOfficePrice = gateBasePrice,
                                SellPrice = basePrice,
                                Price = costPrice,
                                UnitType = pricingUnit.UnitType.ToString(),
                                Capacity = capacity,
                                CancellationPolicy = activityOption.CancellationText?.Trim(),
                            };



                            if (detail.Price > 0)
                                serviceDetails.Add(detail);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return serviceDetails;
        }

        /// <summary>
        /// Process the service details based on base price and cost price
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<TempHBServiceDetail> ProcessServiceDetailsWithBaseAndCostPrices(Activity activity, List<IsangoHBProductMapping> mappedProducts)
        {
            var options = activity?.ProductOptions;
            if (options == null) return null;
            var hbServiceDetails = new List<TempHBServiceDetail>();
            foreach (var option in options)
            {
                try
                {
                    var activityOption = option as ActivityOption;
                    var mappedProduct = mappedProducts.FirstOrDefault(x => x.HotelBedsActivityCode == option.SupplierOptionCode);

                    //Note: In Existing Code, SellPrice = BasePrice and BasePrice = CostPrice
                    var baseDatePriceAndAvailabilities =
                        activityOption?.BasePrice?.DatePriceAndAvailabilty;
                    var costPriceDatePriceAndAvailability =
                        activityOption?.CostPrice?.DatePriceAndAvailabilty;

                    if (baseDatePriceAndAvailabilities == null) continue;
                    foreach (var basePriceAndAvailability in baseDatePriceAndAvailabilities)
                    {
                        var pricingUnits = basePriceAndAvailability.Value.PricingUnits;
                        if (pricingUnits == null)
                            continue;

                        var capacity = basePriceAndAvailability.Value.Capacity;
                        foreach (var pricingUnit in pricingUnits)
                        {
                            try
                            {
                                var perPersonPricingUnit = (PerPersonPricingUnit)pricingUnit;
                                var passengerType = perPersonPricingUnit.PassengerType;
                                var paxCount = 1;
                                if (perPersonPricingUnit.Mincapacity > 1 && mappedProduct.ApiType == APIType.PrioHub)
                                {
                                    paxCount = perPersonPricingUnit.Mincapacity ?? 1;
                                }
                                else
                                {

                                    paxCount = option.TravelInfo?.NoOfPassengers?.
                                        FirstOrDefault(x => x.Key == perPersonPricingUnit.PassengerType).Value ?? 0;
                                }
                                var basePrice = perPersonPricingUnit.Price;
                                var costPrice = costPriceDatePriceAndAvailability
                                    ?.FirstOrDefault(w => w.Key.Equals(basePriceAndAvailability.Key)).Value?.PricingUnits?.FirstOrDefault(
                                        x => ((PerPersonPricingUnit)x).PassengerType.Equals(passengerType))?.Price ?? 0;

                                var detail = new TempHBServiceDetail
                                {
                                    ProductCode = activity.FactsheetId.ToString(),
                                    Modality = activityOption.Code,
                                    AvailableOn = basePriceAndAvailability.Key,
                                    Currency = activity?.CurrencyIsoCode ?? mappedProduct.CurrencyISOCode,
                                    FactSheetID = activity.FactsheetId,
                                    ProductClass = activity.DurationString,
                                    ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                    CommissionPercent = option.CommisionPercent,
                                    Status = basePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                    ServiceOptionID = mappedProduct.ServiceOptionInServiceid,
                                    MinAdult = paxCount,
                                    PassengerTypeId = (int)passengerType,
                                    Variant = option.Variant,
                                    StartTime = option.StartTime,
                                    TicketOfficePrice = basePrice,
                                    SellPrice = basePrice,
                                    Price = costPrice,
                                    UnitType = pricingUnit.UnitType.ToString(),
                                    Capacity = capacity,
                                    CancellationPolicy = activityOption.CancellationText?.Trim(),
                                };
                                if (detail.Price > 0)
                                    hbServiceDetails.Add(detail);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return hbServiceDetails;
        }

        /// <summary>
        /// Process service details of AOT
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<TempHBServiceDetail> ProcessAotServiceDetail(Activity activity, List<IsangoHBProductMapping> mappedProducts)
        {
            var options = activity?.ProductOptions;
            if (options == null) return null;

            var hbServiceDetails = new List<TempHBServiceDetail>();
            foreach (var option in options)
            {
                try
                {
                    var activityOption = option as ActivityOption;
                    var mappedProduct = mappedProducts.FirstOrDefault(x => x.HotelBedsActivityCode == option.SupplierOptionCode);

                    var costDatePriceAndAvailabilities =
                        activityOption?.CostPrice?.DatePriceAndAvailabilty;
                    if (costDatePriceAndAvailabilities == null) continue;
                    foreach (var datePriceAndAvailability in costDatePriceAndAvailabilities)
                    {
                        var pricingUnits = datePriceAndAvailability.Value.PricingUnits;
                        if (pricingUnits == null)
                            continue;
                        foreach (var pricingUnit in pricingUnits)
                        {
                            PricingUnit castedPricingUnit = pricingUnit as PerPersonPricingUnit;
                            if (castedPricingUnit == null)
                                castedPricingUnit = pricingUnit as PerUnitPricingUnit;

                            var passengerType = castedPricingUnit is PerPersonPricingUnit ? ((PerPersonPricingUnit)castedPricingUnit).PassengerType : PassengerType.Undefined;
                            var paxCount = option.TravelInfo?.NoOfPassengers?.
                                FirstOrDefault(x => x.Key == passengerType).Value ?? 0;
                            paxCount = paxCount == 0 ? mappedProduct.MinAdultCount : paxCount;

                            var costPrice = castedPricingUnit.Price;
                            var basePrice = CalculateBasePrice(costPrice, mappedProduct.MarginAmount);

                            var detail = new TempHBServiceDetail
                            {
                                Modality = activityOption.Code,
                                AvailableOn = datePriceAndAvailability.Key,
                                Currency = activity.CurrencyIsoCode,
                                FactSheetID = activity.FactsheetId,
                                ProductClass = activity.DurationString,
                                ActivityId = mappedProduct.IsangoHotelBedsActivityId,
                                Status = datePriceAndAvailability.Value?.AvailabilityStatus.ToString(),
                                ServiceOptionID = option.Id,
                                ProductCode = activity.Code,

                                MinAdult = paxCount,
                                PassengerTypeId = (int)passengerType,
                                Variant = option.Variant,
                                StartTime = option.StartTime,
                                TicketOfficePrice = basePrice,
                                SellPrice = basePrice,
                                Price = costPrice,
                                CommissionPercent = mappedProduct.MarginAmount,
                                UnitType = pricingUnit.UnitType.ToString(),
                                Capacity = option.Capacity,
                                CancellationPolicy = activityOption.CancellationText?.Trim(),
                            };
                            if (detail.Price > 0)
                                hbServiceDetails.Add(detail);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return hbServiceDetails;
        }

        #region Private Methods

        /// <summary>
        /// Calculate cost price using base price and commission percentage
        /// </summary>
        /// <param name="basePrice"></param>
        /// <param name="commissionPercentage"></param>
        /// <returns></returns>
        private decimal CalculateCostPrice(decimal basePrice, decimal commissionPercentage)
        {
            return basePrice * ((100 - commissionPercentage) / 100);
        }

        private decimal CalculateSellPrice(decimal costPrice, decimal marginAmount, bool isMarginPercentage)
        {
            var sellPrice = costPrice;
            if (isMarginPercentage)
            {
                if (marginAmount > 0)
                {
                    sellPrice = costPrice * (100 / (100 - marginAmount));
                }
            }
            else
            {
                sellPrice = costPrice + marginAmount;
            }
            return sellPrice;
        }

        /// <summary>
        /// Calculate base price using cost price and margin percentage
        /// </summary>
        /// <param name="costPrice"></param>
        /// <param name="marginPercentage"></param>
        /// <returns></returns>
        private decimal CalculateBasePrice(decimal costPrice, decimal marginPercentage)
        {
            return costPrice * (100 / (100 - marginPercentage));
        }

        private TempHBServiceDetail UpdateDetailsAsPerAPI(TempHBServiceDetail tempHBServiceDetail, ActivityOption activityOption, IsangoHBProductMapping productMapping)
        {
            try
            {
                switch (productMapping.ApiType)
                {
                    case APIType.Bokun:
                        {
                            //Bokun activity id
                            tempHBServiceDetail.FactSheetID = Convert.ToInt32(productMapping.HotelBedsActivityCode);

                            //Bokun rate id
                            tempHBServiceDetail.ProductCode = productMapping.PrefixServiceCode;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch
            {
            }
            return tempHBServiceDetail;
        }

        #endregion Private Methods
    }
}