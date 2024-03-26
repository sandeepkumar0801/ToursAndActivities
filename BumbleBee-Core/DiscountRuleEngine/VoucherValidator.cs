using DiscountRuleEngine.Constants;
using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Isango.Entities;
using Isango.Service.Contract;
using System;
using System.Linq;

namespace DiscountRuleEngine
{
    public class VoucherValidator : IVoucherValidator
    {
        private readonly IVoucherService _voucherService;

        public VoucherValidator(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        public VoucherValidationResult IsVoucherApplicableForCart(DiscountModel discountModel, Voucher voucherDetail)
        {
            var discountCategory = GetDiscountCategory(voucherDetail);
            if (!discountCategory.ApplicableWithMultiSave && discountModel.Cart.IsMultiSaveApplied)
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.ApplicableWithMultiSaveError };
            }

            if (discountCategory.HasMinCartCap)
            {
                // ReSharper disable once PossibleNullReferenceException
                var minCartValue = discountCategory.DiscountCategoryCaps.SingleOrDefault(e => e.Currency.IsoCode == discountModel.Cart.CurrencyIsoCode).MinCartCap;
                if (minCartValue > discountModel.Cart.TotalPrice)
                {
                    return new VoucherValidationResult
                    { IsValid = false, Message = string.Format(Constant.MinCartCapError, minCartValue) };
                }
            }

            if (!voucherDetail.VoucherConfig.ValidAffiliates.Contains(discountModel.AffiliateId, StringComparer.OrdinalIgnoreCase))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.AffiliateCheckError };
            }

            if (!string.IsNullOrWhiteSpace(voucherDetail.VoucherConfig.UTMParameter))
            {
                // TODO: Optimize these conditions in a single check
                if (string.IsNullOrWhiteSpace(discountModel.UTMParameter))
                    return new VoucherValidationResult { IsValid = false, Message = Constant.UTMParameterNotFound };
                else if (!string.Equals(discountModel.UTMParameter, voucherDetail.VoucherConfig.UTMParameter, StringComparison.InvariantCultureIgnoreCase))
                    return new VoucherValidationResult { IsValid = false, Message = Constant.InvalidUTMParameter };
            }

            if (discountModel.Vouchers.Any(e => e.DiscountType == voucherDetail.DiscountType))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.SameDiscountTypeError };
            }
            return new VoucherValidationResult { IsValid = true };
        }

        public VoucherValidationResult IsVoucherApplicableForProduct(DiscountSelectedProduct selectedProduct, Voucher voucherDetail, DiscountCategoryConfig discountCategory)
        {
            if (!discountCategory.ValidOnSaleProduct && (selectedProduct.IsSaleProduct))
                //if (!discountCategory.ValidOnSaleProduct && (selectedProduct.IsSaleProduct || selectedProduct.ParentBundleId > 0))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.SaleProductError };
            }
            if (voucherDetail.VoucherConfig.ValidProducts != null && (voucherDetail.VoucherConfig.ValidProducts.Contains(selectedProduct.Id) != voucherDetail.VoucherConfig.isServiceInclusion))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.ValidProductError };
            }
            if (voucherDetail.VoucherConfig.ValidCategories != null && (voucherDetail.VoucherConfig.ValidCategories.Intersect(selectedProduct.CategoryIds).Any() != voucherDetail.VoucherConfig.isCategoryInclusion))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.ValidCategoryError };
            }
            if (voucherDetail.VoucherConfig.ValidDestinations != null && (voucherDetail.VoucherConfig.ValidDestinations.Intersect(selectedProduct.DestinationIds).Any() != voucherDetail.VoucherConfig.isDestinationInclusion))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.ValidDestinationError };
            }
            if (voucherDetail.VoucherConfig.ValidLobsIds != null && (voucherDetail.VoucherConfig.ValidLobsIds.Contains(selectedProduct.LineOfBusiness) != voucherDetail.VoucherConfig.isLobInclusion))
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.VoucherValidationError };
            }
            if (voucherDetail.VoucherConfig.ThresholdProductMargin > selectedProduct.Margin)
            {
                return new VoucherValidationResult { IsValid = false, Message = Constant.ProductMarginError };
            }
            return new VoucherValidationResult { IsValid = true, Message = Constant.VoucherAppliedMessage };
        }

        public VoucherValidationResult IsVoucherValid(Voucher voucherDetail)
        {
            var validationResult = new VoucherValidationResult
            {
                IsValid = voucherDetail != null && voucherDetail.IsValid && voucherDetail.ExpiryDate > DateTime.Now
            };
            validationResult.Message = validationResult.IsValid ? null : Constant.VoucherValidationError;
            return validationResult;
        }

        public VoucherValidationResult IsVoucherValidForCustomerEmail(Voucher voucherDetail, string customerEmail)
        {
            var giftVoucherDetail = (GiftVoucher)voucherDetail;
            var validationResult = new VoucherValidationResult
            {
                IsValid = string.IsNullOrEmpty(giftVoucherDetail.ReceiverEmail) ||
                          giftVoucherDetail.ReceiverEmail.Equals(customerEmail, StringComparison.OrdinalIgnoreCase)
            };
            validationResult.Message = validationResult.IsValid ? null : Constant.GiftVoucherEmailValidationError;
            return validationResult;
        }

        private DiscountCategoryConfig GetDiscountCategory(Voucher voucherDetail)
        {
            return _voucherService.GetAllDiscountCategoryConfigAsync().Result.FirstOrDefault(e => e.DiscountCategoryType == voucherDetail.DiscountCategoryType);
        }
    }
}