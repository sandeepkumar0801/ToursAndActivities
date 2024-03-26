using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.PricingRules;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceRuleEngine.Modules
{
    /// <summary>
    /// Contains and apply the Product discount rules on the given activity prices
    /// </summary>
    public class ProductDiscountModule : BaseModule
    {
        private List<QrScanAffilateWiseDisount> _qrScanDiscounts;
        private readonly string _qrScanDiscountRules;

        public ProductDiscountModule(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
            try
            {
                _qrScanDiscountRules = Util.ConfigurationManagerHelper.GetValuefromAppSettings("QrScanDiscount");
                _qrScanDiscounts = Util.SerializeDeSerializeHelper.DeSerialize<List<QrScanAffilateWiseDisount>>(_qrScanDiscountRules);
            }
            catch (Exception)
            {
            }
        }

        public override List<ProductOption> Process(PricingRuleRequest request)
        {
            if (!request.ClientInfo.IsSupplementOffer && !request.IsQrScanDiscountApplicable) return request.ProductOptions;

            var rules = ModuleDataSingleton.Instance.ModuleData?.ProductSaleRules;
            var CostRules = ModuleDataSingleton.Instance.ModuleData?.ProductCostSaleRules;
            var productOptions = request.ProductOptions;
            var qrScanDiscountPercentage = 0.0M;
            if (_qrScanDiscounts?.Any() == true && request.IsQrScanDiscountApplicable)
            {
                qrScanDiscountPercentage = _qrScanDiscounts?.FirstOrDefault(x => x?.AffiliateId?.ToLower() == request?.ClientInfo?.AffiliateId?.ToLower())?.DiscountPercentage ?? 0;
            }

            productOptions?.ForEach(option =>
            {
                //Code to check if the margin is to be applied in case of Hb products
                if (option.ApiType.Equals(APIType.Hotelbeds) && ((ActivityOption)option).IsMandatoryApplyAmount)
                {
                    return;
                }
                else
                {
                    var optionIds = new List<int>();// { option.ServiceOptionId };
                    if (option.BundleOptionID > 0)
                    {
                        option.BasePrice = option.BasePrice.DeepCopy();
                        optionIds.Add(option.BundleOptionID);
                    }
                    else
                    {
                        option.BasePrice = option.BasePrice.DeepCopy();
                        optionIds.Add(option.ServiceOptionId);
                    }

                    foreach (var optionId in optionIds)
                    {
                        if (option.BasePrice == null) continue;
                        var availabilityDate = option.BasePrice.DatePriceAndAvailabilty?.FirstOrDefault().Key ?? DateTime.UtcNow.Date;
                        var appliedRule = GetAppliedRule(request, optionId, rules, availabilityDate);
                        var appliedRuleByDateForCost = GetAppliedRule(request, optionId, CostRules, availabilityDate);

                        if (appliedRule == null && appliedRuleByDateForCost == null && qrScanDiscountPercentage == 0) continue;

                        var datePriceAndAvailability = option?.BasePrice?.DatePriceAndAvailabilty;
                        var datePriceAndAvailabilityCost = option?.CostPrice?.DatePriceAndAvailabilty;
                        if (datePriceAndAvailability != null)
                        {
                            foreach (var priceAndAvailabilty in datePriceAndAvailability)
                            {
                                var costPrice = datePriceAndAvailabilityCost[priceAndAvailabilty.Key]?.TotalPrice ?? 0;
                                var appliedRuleByDate = GetAppliedRule(request, optionId, rules, priceAndAvailabilty.Key);

                                if (appliedRuleByDate == null && qrScanDiscountPercentage == 0) continue;
                                var salePercentByDate = appliedRuleByDate?.SaleRuleOfferPercent ?? 0 + qrScanDiscountPercentage;
                                if (costPrice > 0 && salePercentByDate > 0)
                                {
                                    ProcessPriceAndAvailability(priceAndAvailabilty, salePercentByDate, costPrice);
                                }
                            }
                        }
                        if (datePriceAndAvailabilityCost != null)
                        {
                            foreach (var CostpriceAndAvailabilty in datePriceAndAvailabilityCost)
                            {
                                var costPrice = datePriceAndAvailabilityCost[CostpriceAndAvailabilty.Key]?.TotalPrice ?? 0;
                                var appliedCostRuleByDate = GetAppliedRule(request, optionId, CostRules, CostpriceAndAvailabilty.Key);
                                var CostPercentByDate = appliedCostRuleByDate?.SaleRuleOfferPercent ?? 0;
                                if (costPrice > 0 && CostPercentByDate > 0)
                                {
                                    ProcessPriceAndAvailability(CostpriceAndAvailabilty, CostPercentByDate, costPrice);
                                }

                            }
                        }

                        //MSP implementation should come here as well. The total amount is sum of individual total pax amount.
                        var salePercent = appliedRule?.SaleRuleOfferPercent ?? 0 + qrScanDiscountPercentage;
                        var costPercentByDate = appliedRuleByDateForCost?.SaleRuleOfferPercent ?? 0;
                        var amountBeforeSale = option?.BasePrice?.Amount ?? 0;
                        var costAmount = option?.CostPrice?.Amount ?? 0;
                        var amountAfterSale = option.BasePrice.DatePriceAndAvailabilty?.FirstOrDefault().Value?.PricingUnits?.Sum(x => x.Price * x.Quantity) ?? amountBeforeSale;
                        var saleAmount = amountBeforeSale - amountAfterSale;
                        option.CostPrice.Amount = ApplyProductCost(costAmount, costPercentByDate);
                        var CostAmount = costAmount - option.CostPrice.Amount;
                        option.BasePrice.Amount = amountAfterSale;

                        //Availability status check
                        var priceAndAvailabiltyFirstAvailable = option?.BasePrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value;
                        if (option.BasePrice.Amount < priceAndAvailabiltyFirstAvailable?.TotalPrice)
                        {
                            option.BasePrice.Amount = priceAndAvailabiltyFirstAvailable?.TotalPrice ?? 0;
                        }

                        if (option.PriceOffers == null) option.PriceOffers = new List<PriceOffer>();
                        var priceOffer = new PriceOffer();

                        if (salePercent > 0 || costPercentByDate > 0)
                        {
                            if (appliedRule != null)
                            {
                                PopulatePriceOffer(appliedRule);
                            }
                            else if (appliedRule == null && appliedRuleByDateForCost != null)
                            {
                                PopulatePriceOffer(appliedRuleByDateForCost);

                            }
                            else
                            {
                                appliedRule = new ProductSaleRuleByActivity
                                {
                                    AppliedRuleId = 9999999,
                                    RuleName = "QRSCAN_DISCOUNT_RULE",
                                    SaleRuleOfferPercent = salePercent
                                };
                                PopulatePriceOffer(appliedRule);
                            }
                            var salePriceOffer = new PriceOffer(); // Create a new PriceOffer object for salePercent
                            var CostRuleName = appliedRuleByDateForCost?.RuleName;
                            string newRuleName;
                            if (CostRuleName != null)
                            {
                                newRuleName = appliedRule?.RuleName + "Cost" + CostRuleName;
                            }
                            else
                            {
                                newRuleName = appliedRule?.RuleName;
                            }
                            salePriceOffer.RuleName = newRuleName;
                            salePriceOffer.ModuleName = Constants.Constant.ProductDiscountModule;
                            salePriceOffer.OfferPercent = (salePercent > 0) ? salePercent : costPercentByDate;
                            salePriceOffer.SaleAmount = saleAmount < 0 ? saleAmount * -1 : saleAmount;
                            salePriceOffer.CostAmount = CostAmount < 0 ? CostAmount * -1 : CostAmount;
                            option.PriceOffers.Add(salePriceOffer);
                            option.ShowSale = salePriceOffer.SaleAmount > 0;
                        }

                    }
                }
            });
            return productOptions;
        }

        #region Private Methods
        private decimal ApplyProductCost(decimal costPrice, decimal salePercent)
        {
            return costPrice - (costPrice * (salePercent / 100));
        }
        private decimal ApplyProductSale(decimal gatePrice, decimal salePercent, decimal costAmount)
        {
            return gatePrice - (gatePrice * (salePercent / 100));
            //var amountAfterSale = gatePrice - (gatePrice * (salePercent / 100));
            //if (amountAfterSale < costAmount)
            //{
            //    amountAfterSale = costAmount + (costAmount * (3 / 100));
            //}
            //return amountAfterSale;
        }

        private ProductSaleRuleByActivity GetAppliedRule(PricingRuleRequest request, int optionId, ProductSaleRule rules, DateTime travelDate)
        {
            var clientInfo = request.ClientInfo;

            // ProductSaleRuleByOption Filter Starts
            var rulesByOptionId = rules.ProductSaleRulesByOption.Where(x => x.ServiceOptionInServiceId == optionId).ToList();
            if (!rulesByOptionId.Any()) return null;
            // ProductSaleRuleByOption Filter Ends

            var ruleIdAndPriority = new Dictionary<int, int>();
            foreach (var ruleByOption in rulesByOptionId)
            {
                // ProductSaleRuleByAffiliate Starts
                var rulesByAffiliate = rules.ProductSaleRulesByAffiliate?.Where(x =>
                    x.AppliedRuleId == ruleByOption.AppliedRuleId &&
                    x.AffiliateId.ToLower() == clientInfo.AffiliateId.ToLower()).
                    ToList();

                if (rulesByAffiliate == null || !rulesByAffiliate.Any())
                    ruleIdAndPriority.Add(ruleByOption.AppliedRuleId, ruleByOption.PriorityOrder);
                // ProductSaleRuleByAffiliate Ends

                // ProductSaleRuleByCountry Starts
                var rulesByCountry = rules.ProductSaleRulesByCountry?.Where(x =>
                        x.AppliedRuleId == ruleByOption.AppliedRuleId &&
                        x.CountryCode.ToLower() == clientInfo.CountryIp?.ToLower()).
                    ToList();

                if (rulesByCountry != null && !rulesByCountry.Any() && !ruleIdAndPriority.Keys.Contains(ruleByOption.AppliedRuleId))
                    ruleIdAndPriority.Add(ruleByOption.AppliedRuleId, ruleByOption.PriorityOrder);
                // ProductSaleRuleByCountry Ends
            }

            // ProductSaleRuleByActivity Filter Starts
            var rulesByActivity = new List<ProductSaleRuleByActivity>();
            var filteredRules = ruleIdAndPriority.Select(x => x.Key).ToList();
            foreach (var filteredRule in filteredRules)
            {
                rulesByActivity.AddRange(rules.ProductSaleRulesByActivity.Where(x =>
                                x.AppliedRuleId.Equals(filteredRule) &&
                                x.TravelFromDate <= travelDate &&
                                x.TravelToDate >= travelDate &&
                                CheckSupplementArrivalFilter(travelDate, x, x.SUPPLEMENTRULEWEEKDAYONARRIVAL) &&
                                x.BookingFromDate.ToUniversalTime() <= DateTime.UtcNow &&
                                x.BookingToDate.ToUniversalTime() >= DateTime.UtcNow).
                                ToList());
            }
            // ProductSaleRuleByActivity Filter Ends

            if (rulesByActivity.Count == 0) return null;

            // Filtering out the ruleIds from dictionary that does contains in the rulesByActivity object
            ruleIdAndPriority = ruleIdAndPriority.Keys.Intersect(rulesByActivity.
                                Select(x => x.AppliedRuleId)).
                                ToDictionary(key => key, key => ruleIdAndPriority[key]);

            var appliedRule = ruleIdAndPriority.FirstOrDefault(x => x.Value == ruleIdAndPriority.Values.Min());
            return rulesByActivity.FirstOrDefault(x => x.AppliedRuleId == appliedRule.Key);
        }

        private bool CheckSupplementArrivalFilter(DateTime travelDate, ProductSaleRuleByActivity activity, bool SUPPLEMENTRULEWEEKDAYONARRIVAL)
        {
            if (!SUPPLEMENTRULEWEEKDAYONARRIVAL)
            {
                travelDate = DateTime.UtcNow;
            }
            var isApplicable = false;
            switch (travelDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    isApplicable = activity.SupplementRuleArriveOnMonday;
                    break;

                case DayOfWeek.Tuesday:
                    isApplicable = activity.SupplementRuleArriveOnTuesday;
                    break;

                case DayOfWeek.Wednesday:
                    isApplicable = activity.SupplementRuleArriveOnWednesday;
                    break;

                case DayOfWeek.Thursday:
                    isApplicable = activity.SupplementRuleArriveOnThursday;
                    break;

                case DayOfWeek.Friday:
                    isApplicable = activity.SupplementRuleArriveOnFriday;
                    break;

                case DayOfWeek.Saturday:
                    isApplicable = activity.SupplementRuleArriveOnSaturday;
                    break;

                case DayOfWeek.Sunday:
                    isApplicable = activity.SupplementRuleArriveOnSunday;
                    break;
            }
            return isApplicable;
        }

        //private KeyValuePair<DateTime, PriceAndAvailability> ProcessPriceAndAvailability(KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty, decimal salePercent, decimal costPrice = 0)
        //{
        //    bool isMSPAppliedOnAnyPricingUnit = false;
        //    var inputPricingUnits = priceAndAvailabilty.Value.PricingUnits;
        //    //foreach(var pricingUnit in priceAndAvailabilty.Value.PricingUnits)
        //    //{
        //    //    var unit = pricingUnit.DeepCopy();
        //    //    try
        //    //    {
        //    //        if (unit?.Quantity > 1)
        //    //        {
        //    //            costPrice = costPrice / pricingUnit?.Quantity ?? 1;
        //    //        }
        //    //    }
        //    //    catch (Exception)
        //    //    {
        //    //    }
        //    //    var p = ApplyProductSale(unit.Price, salePercent, costPrice);

        //    //    //Check for MSP should be done here - ToVerify
        //    //    if (unit.Price < unit.MinimumSellingPrice)
        //    //    {
        //    //        unit.Price = unit.MinimumSellingPrice;
        //    //        isMSPAppliedOnAnyPricingUnit = true;
        //    //    }
        //    //    pricingUnit.Price = p;
        //    //}
        //    inputPricingUnits?.ForEach(unit =>
        //    {
        //        try
        //        {
        //            if (unit?.Quantity > 1)
        //            {
        //                costPrice = costPrice / unit?.Quantity ?? 1;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        unit.Price = ApplyProductSale(unit.Price, salePercent, costPrice);

        //        //Check for MSP should be done here - ToVerify
        //        if (unit.Price < unit.MinimumSellingPrice)
        //        {
        //            unit.Price = unit.MinimumSellingPrice;
        //            isMSPAppliedOnAnyPricingUnit = true;
        //        }
        //    });
        //    //Calculate Total price of P&A - ToVerify
        //    priceAndAvailabilty.Value.TotalPrice = isMSPAppliedOnAnyPricingUnit ? GetThisPAndATotalPriceAfterMSP(priceAndAvailabilty) : priceAndAvailabilty.Value.PricingUnits.Sum(x => x.Price * (x?.Quantity ?? 1));
        //    priceAndAvailabilty.Value.PricingUnits = inputPricingUnits;

        //    return priceAndAvailabilty;
        //}

        private void ProcessPriceAndAvailability(KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty, decimal salePercent, decimal costPrice = 0)
        {
            bool isMSPAppliedOnAnyPricingUnit = false;
            priceAndAvailabilty.Value?.PricingUnits?.ForEach(unit =>
            {
                try
                {
                    if (unit?.Quantity > 1)
                    {
                        costPrice = costPrice / unit?.Quantity ?? 1;
                    }
                }
                catch (Exception)
                {
                }
                unit.Price = ApplyProductSale(unit.Price, salePercent, costPrice);

                //Check for MSP should be done here - ToVerify
                if (unit.Price < unit.MinimumSellingPrice)
                {
                    unit.Price = unit.MinimumSellingPrice;
                    isMSPAppliedOnAnyPricingUnit = true;
                }
            });
            //Calculate Total price of P&A - ToVerify
            priceAndAvailabilty.Value.TotalPrice = isMSPAppliedOnAnyPricingUnit ? GetThisPAndATotalPriceAfterMSP(priceAndAvailabilty) : priceAndAvailabilty.Value.PricingUnits.Sum(x => x.Price * (x?.Quantity ?? 1));
            //priceAndAvailabilty.Value.PricingUnits = inputPricingUnits;

            //return priceAndAvailabilty;
        }

        private decimal GetThisPAndATotalPriceAfterMSP(KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty)
        {
            var pricingUnits = priceAndAvailabilty.Value.PricingUnits;
            priceAndAvailabilty.Value.TotalPrice = 0;

            pricingUnits.ForEach(thisUnit => { priceAndAvailabilty.Value.TotalPrice += thisUnit.PriceType == PriceType.PerPerson ? (thisUnit.Price * thisUnit.Quantity) : thisUnit.Price; });

            //foreach (var pricingUnit in pricingUnits)
            //{
            //    if (pricingUnit.PriceType.Equals(PriceType.PerPerson))
            //    {
            //        priceAndAvailabilty.Value.TotalPrice += (pricingUnit.Price * pricingUnit.Quantity);
            //    }
            //    else
            //    {
            //        priceAndAvailabilty.Value.TotalPrice += (pricingUnit.Price);
            //    }
            //}

            return priceAndAvailabilty.Value.TotalPrice;
        }

        private PriceOffer PopulatePriceOffer(ProductSaleRuleByActivity appliedRule)
        {
            var priceOffer = new PriceOffer
            {
                Id = appliedRule.AppliedRuleId,
                ModuleName = Constants.Constant.ProductDiscountModule,
                RuleName = appliedRule.RuleName,
                OfferPercent = appliedRule.SaleRuleOfferPercent,
            };
            return priceOffer;
        }

        #endregion Private Methods
    }
}