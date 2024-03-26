using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceRuleEngine.Modules
{
    /// <summary>
    /// Contains and apply the Margin rules on the given activity prices
    /// </summary>
    public class MarginAndCommissionModule : BaseModule
    {
        public MarginAndCommissionModule(IPriceRuleEngineService priceRuleEngineService) : base(priceRuleEngineService)
        {
        }

        public override List<ProductOption> Process(PricingRuleRequest request)
        {
            var productOptions = request.ProductOptions;
            productOptions?.ForEach(option =>
            {
                var priceTypeId = option.BundleOptionID > 0 ? (PriceTypeId)option.PriceTypeID : request.PriceTypeId;
                if(request.PriceTypeId != priceTypeId)
                {
                    request.PriceTypeId = priceTypeId;
                }
                if (priceTypeId.Equals(PriceTypeId.Undefined))
                {
                    if (option.BasePrice != null) option.GateBasePrice = option.BasePrice.DeepCopy();
                    return;
                }
                var isCommission = option.IsIsangoMarginApplicable ? true : (request.PriceTypeId.Equals(PriceTypeId.Commission) || option.CommisionPercent > 0);
                ProcessOption(option, isCommission);

                if (option?.GateBasePrice != null && option?.GateBasePrice?.Amount > 0 && option?.GateBasePrice?.DatePriceAndAvailabilty?.Any()==true 
                && option?.GateBasePrice?.DatePriceAndAvailabilty?.Count >= option?.BasePrice?.DatePriceAndAvailabilty?.Count)
                {
                    option.GateBasePrice = option.GateBasePrice.DeepCopy();
                }
                else
                {
                    option.GateBasePrice = option.BasePrice.DeepCopy();
                }
            });
            return productOptions;
        }

        #region Private Methods

        private void ProcessOption(ProductOption option, bool isCommission)
        {
            var margin = option.Margin?.Value ?? 0;
            var commission = option.CommisionPercent > 0 ? option.CommisionPercent : margin;
            var marginPercent = isCommission ? commission : margin;

            if (option.BasePrice == null)
            {
                option.BasePrice = new Price
                {
                    Amount =
                        CalculatePrice(option.CostPrice.Amount, marginPercent, isCommission),
                    Currency = option.CostPrice.Currency,
                    DatePriceAndAvailabilty = ProcessPriceAndAvailability(option.CostPrice, marginPercent,
                        isCommission)
                };
            }
            else if (option.CostPrice == null || option.IsIsangoMarginApplicable)
            {
                option.CostPrice = new Price
                {
                    Amount =
                        CalculatePrice(option.BasePrice.Amount, marginPercent, isCommission),
                    Currency = option.BasePrice.Currency,
                    DatePriceAndAvailabilty = ProcessPriceAndAvailability(option.BasePrice, marginPercent,
                        isCommission)
                };
            }
        }

        public Dictionary<DateTime, PriceAndAvailability> ProcessPriceAndAvailability(Price price, decimal marginPercent, bool isCommission)
        {
            var inputPrice = price.DeepCopy();
            var dates = inputPrice.DatePriceAndAvailabilty?.Select(x => x.Key).ToList();
            var priceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();
            if (dates == null) return priceAndAvailabilities;

            foreach (var date in dates)
            {
                var inputPriceAndAvailability = inputPrice.DatePriceAndAvailabilty?.FirstOrDefault(x => x.Key == date);
                if (inputPriceAndAvailability?.Value == null) continue;

                var priceAndAvailability = inputPriceAndAvailability?.Value;
                // ReSharper disable once ConstantNullCoalescingCondition
                var totalPrice = inputPriceAndAvailability?.Value?.TotalPrice ?? 0;

                priceAndAvailability.TotalPrice =
                    CalculatePrice(totalPrice, marginPercent, isCommission);

                priceAndAvailability.PricingUnits?.ForEach(unit =>
                {
                    unit.Price = CalculatePrice(unit.Price, marginPercent, isCommission);
                });
                if (!priceAndAvailabilities.Keys.Contains(date))
                    priceAndAvailabilities.Add(date, priceAndAvailability);
            }
            return priceAndAvailabilities;
        }

        #region Price Calculation Related Formaulas

        private decimal CalculatePrice(decimal price, decimal margin, bool isCommission)
        {
            if (isCommission)
            {
                return ApplyCommission(price, margin);
            }
            else
            {
                return ApplyMargin(price, margin);
            }
        }

        private decimal ApplyMargin(decimal costPrice, decimal margin)
        {
            return costPrice * (100 / (100 - margin));
        }

        private decimal ApplyCommission(decimal basePrice, decimal commissionPercentage)
        {
            return basePrice * ((100 - commissionPercentage) / 100);
        }

        #endregion Price Calculation Related Formaulas

        #endregion Private Methods
    }
}