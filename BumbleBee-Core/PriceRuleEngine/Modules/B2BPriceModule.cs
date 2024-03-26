using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceRuleEngine.Modules
{
    /// <summary>
    /// Contains and apply the B2B price rules on the given activity prices
    /// </summary>
    public class B2BPriceModule : BaseModule
    {
        public B2BPriceModule(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
        }

        public override List<ProductOption> Process(PricingRuleRequest request)
        {
            var appliedRule = GetAppliedRule(request);
            if (appliedRule == null) return request.ProductOptions;

            var netRatePercent = appliedRule.NetRatePercent;
            var netPriceType = appliedRule.NetPriceType;
            var productOptions = request.ProductOptions;
            productOptions?.ForEach(option =>
            {
                if (option.BasePrice == null) return;

                var amountBeforeSale = option.BasePrice.Amount;
                option.BasePrice.Amount = netPriceType == 1
                    ? ApplyNetPriceForPriceType1(option.CostPrice.Amount, option.BasePrice.Amount, netRatePercent)
                    : netPriceType == 2 ? ApplyNetPriceForPriceType2(option.CostPrice.Amount, netRatePercent)
                    : ApplyNetPriceForPriceType3(option.BasePrice.Amount, netRatePercent);

                var saleAmount = amountBeforeSale - option.BasePrice.Amount;

                var datePriceAndAvailability = option.BasePrice.DatePriceAndAvailabilty;
                foreach (var priceAndAvailabilty in datePriceAndAvailability)
                {
                    ProcessPriceAndAvailability(option, priceAndAvailabilty, netRatePercent, netPriceType);
                }

                if (option.PriceOffers == null) option.PriceOffers = new List<PriceOffer>();
                var priceOffer = PopulatePriceOffer(appliedRule);
                priceOffer.SaleAmount = saleAmount < 0 ? saleAmount * -1 : saleAmount;
                option.PriceOffers.Add(priceOffer);
            });
            return productOptions;
        }

        #region Private Methods
       
        private decimal ApplyNetPriceForPriceType1(decimal costPrice, decimal basePrice, decimal netRatePercent)
        {
            // Updated Formula as per HP's comment. Mail Subject: RE: iSango || Queries || Gateprice offer and was-now price
            return costPrice + ((basePrice - costPrice) * netRatePercent / 100);
        }

        private decimal ApplyNetPriceForPriceType2(decimal costPrice, decimal netRatePercent)
        {
            // Updated Formula as per HP's comment. Mail Subject: RE: iSango || Queries || Gateprice offer and was-now price
            return costPrice * (100 / (100 - netRatePercent));
        }

        private decimal ApplyNetPriceForPriceType3(decimal basePrice, decimal netRatePercent)
        {
            return basePrice - (basePrice * (netRatePercent / 100));
        }

        private B2BNetRateRule GetAppliedRule(PricingRuleRequest request)
        {
            var rules = _priceRuleEngineService.GetB2BNetRateRules().GetAwaiter().GetResult();
            var clientInfo = request.ClientInfo;

            var applicableRule = rules.FirstOrDefault(x =>
                                        x.AffiliateId.ToLower() == clientInfo.AffiliateId.ToLower() &&
                                        x.BookingFromDate.ToUniversalTime() <= DateTime.UtcNow &&
                                        x.BookingToDate.ToUniversalTime() >= DateTime.UtcNow);

            return applicableRule;
        }

        private void ProcessPriceAndAvailability(ProductOption option, KeyValuePair<DateTime, PriceAndAvailability> priceAndAvailabilty, decimal netRatePercent, int netPriceType)
        {
            var costPrice = option.CostPrice.DatePriceAndAvailabilty.FirstOrDefault(x => x.Key == priceAndAvailabilty.Key);

            if (costPrice.Value == null) return;
            var costPriceUnits = costPrice.Value.PricingUnits;
            priceAndAvailabilty.Value.PricingUnits?.ForEach(unit =>
            {
                var castedUnit = (PerPersonPricingUnit)unit;
                if (costPriceUnits == null) return;
                foreach (var costPricingUnit in costPriceUnits)
                {
                    var castedCostPricingUnit = (PerPersonPricingUnit)costPricingUnit;
                    if (castedCostPricingUnit.PassengerType == castedUnit.PassengerType)
                    {
                        unit.Price = netPriceType == 1
                        ? ApplyNetPriceForPriceType1(castedCostPricingUnit.Price, castedUnit.Price, netRatePercent)
                        : netPriceType == 2
                        ? ApplyNetPriceForPriceType2(option.CostPrice.Amount, netRatePercent)
                        : ApplyNetPriceForPriceType3(castedUnit.Price, netRatePercent);
                    }
                     
                }
            });
            priceAndAvailabilty.Value.TotalPrice = netPriceType == 1
            ? ApplyNetPriceForPriceType1(costPrice.Value.TotalPrice, priceAndAvailabilty.Value.TotalPrice, netRatePercent)
            : netPriceType == 2
            ?ApplyNetPriceForPriceType2(costPrice.Value.TotalPrice, netRatePercent)
            : ApplyNetPriceForPriceType3(priceAndAvailabilty.Value.TotalPrice, netRatePercent);
        
    }

        private PriceOffer PopulatePriceOffer(B2BNetRateRule appliedRule)
        {
            var priceOffer = new PriceOffer
            {
                ModuleName = Constants.Constant.B2BPriceModule,
                OfferPercent = appliedRule.NetRatePercent
            };
            return priceOffer;
        }

        #endregion Private Methods
    }
}