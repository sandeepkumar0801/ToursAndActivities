using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using System.Collections.Generic;
using System.Linq;

namespace DiscountRuleEngine
{
	public class DiscountValidator : IDiscountValidator
	{
		public List<DiscountSelectedProduct> UpdateDiscountAsPerMaxValueCap(DiscountCart cart, decimal maxValueCap, string voucherCode)
		{
			var selectedProducts = cart.SelectedProducts;
			foreach (var selectedProduct in selectedProducts)
			{
				var discountApplied = selectedProduct.AppliedDiscountCoupons.FirstOrDefault(e => e.DiscountCouponCode == voucherCode)?.DiscountedPrice ?? 0;
				selectedProduct.ProductDiscountedPrice -= discountApplied;
				var selectedProductDiscount = maxValueCap * selectedProduct.SellPrice / cart.TotalNotSaleProductsPrice;
				selectedProduct.ProductDiscountedPrice += selectedProductDiscount;
				// ReSharper disable once PossibleNullReferenceException
				selectedProduct.AppliedDiscountCoupons.SingleOrDefault(e => e.DiscountCouponCode == voucherCode)
					.DiscountedPrice = selectedProductDiscount;
			}

			return selectedProducts;
		}
	}
}
