using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.PricingRules;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceRuleEngine.Modules
{
    /// <summary>
    /// Contains and apply the B2B sale rules on the given activity prices
    /// </summary>
    public class B2BSaleModule : BaseModule
    {
        public B2BSaleModule(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
        }

        public override List<ProductOption> Process(PricingRuleRequest request)
        {
            var productOptions = request.ProductOptions;
            var rules = ModuleDataSingleton.Instance.ModuleData.B2BSaleRules;

            productOptions?.ForEach(option =>
            {
                var appliedRule = GetAppliedRule(request, option, rules);
                if (option.BasePrice == null || appliedRule == null) return;

                var salePercent = appliedRule.B2BSaleOfferPercent;
                var datePriceAndAvailability = option.BasePrice.DatePriceAndAvailabilty;
                if (datePriceAndAvailability == null) return;
                foreach (var priceAndAvailabilty in datePriceAndAvailability)
                {
                    ProcessPriceAndAvailability(priceAndAvailabilty, salePercent);
                }

                var amountBeforeSale = option.BasePrice.Amount;
                option.BasePrice.Amount = ApplyB2BSale(option.BasePrice.Amount, salePercent);
                var saleAmount = amountBeforeSale - option.BasePrice.Amount;

                //Availability status check
                var priceAndAvailabiltyFirstAvailable = option.BasePrice.DatePriceAndAvailabilty?.FirstOrDefault().Value ;
                if (option.BasePrice.Amount < priceAndAvailabiltyFirstAvailable?.TotalPrice)
                {
                    option.BasePrice.Amount = priceAndAvailabiltyFirstAvailable.TotalPrice;
                }

                if (option.PriceOffers == null) option.PriceOffers = new List<PriceOffer>();
                var priceOffer = PopulatePriceOffer(appliedRule);
                priceOffer.SaleAmount = saleAmount < 0 ? saleAmount * -1 : saleAmount;
                option.PriceOffers.Add(priceOffer);
            });
            return productOptions;
        }

        #region Private Methods

        private decimal ApplyB2BSale(decimal gatePrice, decimal salePercent)
        {
            return gatePrice - (gatePrice * (salePercent / 100));
        }

        private B2BSaleRule GetAppliedRule(PricingRuleRequest request, ProductOption option, List<B2BSaleRule> rules)
        {
            var clientInfo = request.ClientInfo;
            var margin = option.Margin?.Value ?? 0;

            var appliedRule = rules?.FirstOrDefault(x =>
                                            x.AffiliateId.ToLower() == clientInfo.AffiliateId.ToLower() &&
                                            x.BookingFromDate.ToUniversalTime() <= DateTime.UtcNow &&
                                            x.BookingToDate.ToUniversalTime() >= DateTime.UtcNow &&
                                            x.MinMarginCapPercent >= margin);

            return appliedRule;
        }

        private void ProcessPriceAndAvailability(KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty, decimal salePercent)
        {
            bool isMSPAppliedOnAnyPricingUnit = false;
            priceAndAvailabilty.Value.PricingUnits?.ForEach(unit =>
                {
                    unit.Price = ApplyB2BSale(unit.Price, salePercent);
                    //Check for MSP should be done here - ToVerify
                    if (unit.Price < unit.MinimumSellingPrice)
                    {
                        unit.Price = unit.MinimumSellingPrice;
                        isMSPAppliedOnAnyPricingUnit = true;
                    }
                });

            //Calculate Total price of P&A - ToVerify
            priceAndAvailabilty.Value.TotalPrice = isMSPAppliedOnAnyPricingUnit ? GetThisPAndATotalPriceAfterMSP(priceAndAvailabilty) : ApplyB2BSale(priceAndAvailabilty.Value.TotalPrice, salePercent);
        }

        private decimal GetThisPAndATotalPriceAfterMSP(KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty)
        {
            var pricingUnits = priceAndAvailabilty.Value.PricingUnits;
            priceAndAvailabilty.Value.TotalPrice = 0;

            pricingUnits.ForEach(thisUnit => { priceAndAvailabilty.Value.TotalPrice  += thisUnit.PriceType == PriceType.PerPerson ? (thisUnit.Price * thisUnit.Quantity) : thisUnit.Price; });

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
        private PriceOffer PopulatePriceOffer(B2BSaleRule appliedRule)
        {
            var priceOffer = new PriceOffer
            {
                ModuleName = Constants.Constant.B2BSaleModule,
                RuleName = appliedRule.SaleDescription,
                OfferPercent = appliedRule.B2BSaleOfferPercent
            };
            return priceOffer;
        }

        #endregion Private Methods
    }
}