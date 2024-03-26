using DiscountRuleEngine.Model;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using System;
using System.Linq;

namespace DiscountRuleEngine
{
    public class MultiSaveDiscountModule : DiscountModule
    {
        private readonly IVoucherService _voucherService;
        private readonly IAffiliateService _affiliateService;

        public MultiSaveDiscountModule(IVoucherService voucherService, IAffiliateService affiliateService)
        {
            _voucherService = voucherService;
            _affiliateService = affiliateService;
        }

        public override DiscountCart ApplyDiscount(DiscountModel discountModel, Voucher voucherDetail = null)
        {
            var discountCategories = _voucherService.GetAllDiscountCategoryConfigAsync().Result;

            var selectedProducts = discountModel.Cart.SelectedProducts.OrderBy(e => e.SequenceNumber).ThenBy(e => e.ComponentOrderNumber).ToList();
            var discountCategory =
                discountCategories.SingleOrDefault(e => e.DiscountCategoryType == DiscountCategoryType.MultiSave);
            var multiSaveDiscountLimit = discountCategory?.DiscountCategoryCaps?.FirstOrDefault(e =>
                     e.Currency.IsoCode == discountModel.Cart.CurrencyIsoCode)?.MaxValueCap ?? 0;
            var affiliateInfo =
                _affiliateService.GetAffiliateInformationAsync(discountModel.AffiliateId).Result;
            var multiSavePercentage = affiliateInfo.AffiliateConfiguration.MultisavePercentage;
            decimal totalMultisaveApplied = 0;
            foreach (var selectedProduct in selectedProducts.Where(e => e.SellPrice > 0))
            {
                if (selectedProduct.SequenceNumber == 1) continue;
                if (totalMultisaveApplied >= multiSaveDiscountLimit) continue;
                if (!selectedProduct.IsSaleProduct && !(selectedProduct.IsBundle))
                {
                    var multiSaveAmount = selectedProduct.SellPrice * Convert.ToDecimal(multiSavePercentage) / 100;
                    if (multiSaveDiscountLimit >= multiSaveAmount + totalMultisaveApplied)
                    {
                        selectedProduct.MultiSaveDiscountedPrice = multiSaveAmount;
                        totalMultisaveApplied += multiSaveAmount;
                    }
                    else
                    {
                        multiSaveAmount = multiSaveDiscountLimit - totalMultisaveApplied;
                        selectedProduct.MultiSaveDiscountedPrice = multiSaveAmount;
                        totalMultisaveApplied += multiSaveAmount;
                    }
                    selectedProduct.IsMultiSaveApplied = true;
                }
            }

            discountModel.Cart.SelectedProducts = selectedProducts;
            if (totalMultisaveApplied > 0)
            {
                discountModel.Cart.IsMultiSaveApplied = true;
            }
            else
            {
                discountModel.Cart.IsMultiSaveApplied = false;
            }
            return discountModel.Cart;
        }
    }
}