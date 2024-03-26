using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.PricingRules;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceRuleEngine.Modules
{
    /// <summary>
    /// Applies the Supplier Sale Rules on the given product option prices
    /// </summary>
    public class SupplierSaleModule : BaseModule
    {
        public SupplierSaleModule(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
        }

        public override List<ProductOption> Process(PricingRuleRequest request)
        {
            var productOptions = request.ProductOptions;
            var rules = ModuleDataSingleton.Instance.ModuleData.SupplierSaleRules;
            productOptions?.ForEach(option =>
        {
            //Code to check if the margin is to be applied in case of Hb products
            if (option.ApiType.Equals(APIType.Hotelbeds) && ((ActivityOption)option).IsMandatoryApplyAmount)
            {
                return;
            }
            else
            {
                if (option.GateBasePrice == null) return;
                var availabilityDate = option.GateBasePrice.DatePriceAndAvailabilty?.FirstOrDefault().Key ?? DateTime.UtcNow.Date;
                var appliedRule = GetAppliedRule(option, rules, availabilityDate);
                if (appliedRule == null) return;

                var datePriceAndAvailability = option.GateBasePrice.DatePriceAndAvailabilty;
                if (datePriceAndAvailability != null)
                {
                    foreach (var priceAndAvailabilty in datePriceAndAvailability)
                    {
                        var appliedRuleByDate = GetAppliedRule(option, rules, priceAndAvailabilty.Key);
                        if (appliedRuleByDate == null) continue;
                        var salePercentByDate = appliedRuleByDate.SaleRuleOfferPercent;
                        ProcessPriceAndAvailability(priceAndAvailabilty, salePercentByDate);
                    }
                }

                var salePercent = appliedRule.SaleRuleOfferPercent;
                var amountBeforeSale = option.GateBasePrice.Amount;
                option.GateBasePrice.Amount = ApplySupplierSale(option.GateBasePrice.Amount, salePercent);
                var saleAmount = amountBeforeSale - option.GateBasePrice.Amount;

                //Availability status check
                var priceAndAvailabiltyFirstAvailable = option.BasePrice.DatePriceAndAvailabilty.FirstOrDefault().Value;
                if (option.BasePrice.Amount < priceAndAvailabiltyFirstAvailable.TotalPrice)
                {
                    option.BasePrice.Amount = priceAndAvailabiltyFirstAvailable.TotalPrice;
                }

                if (option.PriceOffers == null) option.PriceOffers = new List<PriceOffer>();
                var priceOffer = PopulatePriceOffer(appliedRule);
                priceOffer.SaleAmount = saleAmount < 0 ? saleAmount * -1 : saleAmount;
                option.PriceOffers.Add(priceOffer);
            }
        });
            return productOptions;
        }

        #region Private Methods

        private decimal ApplySupplierSale(decimal gatePrice, decimal salePercent)
        {
            return Math.Round(gatePrice * (100 / (100 - salePercent)), 4);
        }

        private SupplierSaleRuleByActivity GetAppliedRule(ProductOption option, SupplierSaleRule rules, DateTime travelDate)
        {
            // SupplierSaleRuleByOption Filter Starts
            var rulesByOption = rules.SupplierSaleRulesByOption?.Where(x => x.ServiceOptionInServiceId == option.ServiceOptionId).ToList();
            if (rulesByOption == null || !rulesByOption.Any()) return null;
            // SupplierSaleRuleByOption Filter Ends

            // SupplierSaleRuleByActivity Filter Starts
            var rulesByActivity = new List<SupplierSaleRuleByActivity>();
            var ruleIdAndPriority = new Dictionary<int, int>();
            foreach (var ruleByOption in rulesByOption)
            {
                ruleIdAndPriority.Add(ruleByOption.AppliedRuleId, ruleByOption.PriorityOrder);
                rulesByActivity.AddRange(rules.SupplierSaleRulesByActivity.Where(x =>
                    x.AppliedRuleId.Equals(ruleByOption.AppliedRuleId) &&
                    x.TravelFromDate <= travelDate &&
                    x.TravelToDate >= travelDate &&
                    x.BookingFromDate.ToUniversalTime() <= DateTime.UtcNow &&
                    x.BookingToDate.ToUniversalTime() >= DateTime.UtcNow).
                    ToList());
            }
            // SupplierSaleRuleByActivity Filter Ends

            if (!rulesByActivity.Any()) return null;

            ruleIdAndPriority = ruleIdAndPriority.Keys.Intersect(rulesByActivity.
                    Select(x => x.AppliedRuleId)).
                ToDictionary(key => key, key => ruleIdAndPriority[key]);

            var appliedRule = ruleIdAndPriority.FirstOrDefault(x => x.Value == ruleIdAndPriority.Values.Min());
            return rulesByActivity.FirstOrDefault(x => x.AppliedRuleId == appliedRule.Key);
        }

        private void ProcessPriceAndAvailability(KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty, decimal salePercent)
        {
            bool isMSPAppliedOnAnyPricingUnit = false;
            priceAndAvailabilty.Value.PricingUnits?.ForEach(unit =>
            {
                unit.Price = ApplySupplierSale(unit.Price, salePercent);

                //Check for MSP should be done here - ToVerify
                if (unit.Price < unit.MinimumSellingPrice)
                {
                    unit.Price = unit.MinimumSellingPrice;
                    isMSPAppliedOnAnyPricingUnit = true;
                }
            });

            //Calculate Total price of P&A - ToVerify
            priceAndAvailabilty.Value.TotalPrice = isMSPAppliedOnAnyPricingUnit ? GetThisPAndATotalPriceAfterMSP(priceAndAvailabilty) : ApplySupplierSale(priceAndAvailabilty.Value.TotalPrice, salePercent);
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

        private PriceOffer PopulatePriceOffer(SupplierSaleRuleByActivity appliedRule)
        {
            var priceOffer = new PriceOffer
            {
                Id = appliedRule.AppliedRuleId,
                ModuleName = Constants.Constant.SupplierSaleModule,
                RuleName = appliedRule.RuleName,
                OfferPercent = appliedRule.SaleRuleOfferPercent
            };
            return priceOffer;
        }

        #endregion Private Methods
    }
}