using DiscountRuleEngine.Constants;
using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscountRuleEngine
{
	public class PromotionalDiscountModule : DiscountModule
	{
		private readonly IVoucherValidator _voucherValidator;
		private readonly IDiscountValidator _discountValidator;
		private readonly IVoucherService _voucherService;
		public PromotionalDiscountModule(IVoucherValidator voucherValidator, IDiscountValidator discountValidator, IVoucherService voucherService)
		{
			_voucherValidator = voucherValidator;
			_discountValidator = discountValidator;
			_voucherService = voucherService;
		}
		public override DiscountCart ApplyDiscount(DiscountModel discountModel, Voucher voucherDetail = null)
		{
			var discountCategories = _voucherService.GetAllDiscountCategoryConfigAsync().Result;
			var discountCategory =
				discountCategories.SingleOrDefault(e => e.DiscountCategoryType == voucherDetail?.DiscountCategoryType);
			decimal voucherMaxValueCap = 0;
			if (discountCategory?.DiscountCategoryCaps.Count > 0)
				voucherMaxValueCap = discountCategory?.DiscountCategoryCaps?.FirstOrDefault(e =>
					e.Currency.IsoCode == discountModel.Cart.CurrencyIsoCode)?.MaxValueCap ?? 0;
			var selectedProducts = discountModel.Cart.SelectedProducts;
			decimal totalDiscountApplied = 0;
			discountModel.Cart.TotalNotSaleProductsPrice = discountModel.Cart.TotalPrice;
			foreach (var selectedProduct in selectedProducts)
			{
				if (((selectedProduct.IsSaleProduct) && (discountCategory.DiscountType!= DiscountType.Promotional)) || ((selectedProduct.ParentBundleId > 0) && (discountCategory.DiscountType != DiscountType.Promotional)))
				{
					discountModel.Cart.TotalNotSaleProductsPrice -= selectedProduct.SellPrice;
				}
			}
			foreach (var selectedProduct in selectedProducts)
			{
				decimal selectedProductDiscount = 0;
				var voucherValidationResult = _voucherValidator.IsVoucherApplicableForProduct(selectedProduct, voucherDetail, discountCategory);
				if (voucherValidationResult.IsValid)
				{
					// ReSharper disable once PossibleNullReferenceException
					if (voucherDetail.DiscountCategoryType == DiscountCategoryType.GiftVoucher || voucherDetail.DiscountCategoryType == DiscountCategoryType.LoyaltyVoucher)
					{
						selectedProductDiscount = voucherDetail.Amount * selectedProduct.SellPrice / discountModel.Cart.TotalPrice;
						selectedProduct.ProductDiscountedPrice += selectedProductDiscount;
					}
					else
					{
                        var saleDiscount = selectedProducts.Sum(x => x.GatePrice) - selectedProducts.Sum(x => x.SellPrice);
                        var voucherDetailAmount = voucherDetail.Amount;
                        if (selectedProducts.Any(x => x.IsSaleProduct == true) &&
                            ((discountCategory.DiscountType == DiscountType.Promotional) && (discountCategory.ValidOnSaleProduct==true)) &&
                             (voucherDetailAmount >= saleDiscount))
                        {
                            if (voucherDetail.VoucherConfig.IsPercent)
                            {
                                selectedProductDiscount = selectedProduct.GatePrice * Convert.ToDecimal(voucherDetail.Amount) / 100;
                                selectedProduct.ProductDiscountedPrice += selectedProductDiscount;
                                totalDiscountApplied += selectedProduct.ProductDiscountedPrice;
                            }
                            else
                            {
                                var sumofData = discountModel.Cart.SelectedProducts.Sum(x => x.GatePrice);
                                if (discountModel.Cart.TotalNotSaleProductsPrice == 0)
                                {
                                    selectedProductDiscount = voucherDetail.Amount;
                                }
                                else
                                {
                                    selectedProductDiscount = voucherDetail.Amount * selectedProduct.GatePrice / sumofData;
                                }
                                selectedProduct.ProductDiscountedPrice += selectedProductDiscount;
                            }
                            selectedProduct.SellPrice = selectedProduct.GatePrice;
                        }
                        else if (selectedProducts.Any(x => x.IsSaleProduct == true) &&
                            ((discountCategory.DiscountType == DiscountType.Promotional) && (discountCategory.ValidOnSaleProduct == true)) &&
                            (voucherDetailAmount < saleDiscount))
                        {
                            voucherValidationResult= new VoucherValidationResult { IsValid = false, Message = Constant.SaleProductCouponError };
                        }
                        else
                        {

                            if (voucherDetail.VoucherConfig.IsPercent)
                            {
                                selectedProductDiscount = selectedProduct.SellPrice * Convert.ToDecimal(voucherDetail.Amount) / 100;
                                selectedProduct.ProductDiscountedPrice += selectedProductDiscount;
                                totalDiscountApplied += selectedProduct.ProductDiscountedPrice;
                            }
                            else
                            {
                                selectedProductDiscount = voucherDetail.Amount * selectedProduct.SellPrice / discountModel.Cart.TotalNotSaleProductsPrice;
                                selectedProduct.ProductDiscountedPrice += selectedProductDiscount;
                            }
                        }
					}
				}

				if (selectedProduct.AppliedDiscountCoupons == null)
					selectedProduct.AppliedDiscountCoupons = new List<DiscountCoupon>();
				selectedProduct.AppliedDiscountCoupons.Add(new DiscountCoupon
				{
					DiscountCouponCode = voucherDetail?.VoucherCode,
					DiscountedPrice = selectedProductDiscount,
					// ReSharper disable once PossibleNullReferenceException
					DiscountType = voucherDetail.DiscountType,
					IsValid = voucherValidationResult.IsValid,
					Message = voucherValidationResult.Message
				});
			}

			// ReSharper disable once PossibleNullReferenceException
#pragma warning disable S2259 // Null pointers should not be dereferenced
			if (discountCategory.HasMaxValueCap && totalDiscountApplied > voucherMaxValueCap)
#pragma warning restore S2259 // Null pointers should not be dereferenced
			{
				selectedProducts = _discountValidator.UpdateDiscountAsPerMaxValueCap(discountModel.Cart, voucherMaxValueCap, voucherDetail?.VoucherCode);
			}

			discountModel.Cart.SelectedProducts = selectedProducts;
			return discountModel.Cart;
		}
	}
}