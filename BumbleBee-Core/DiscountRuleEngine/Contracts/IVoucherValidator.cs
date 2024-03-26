using DiscountRuleEngine.Model;
using Isango.Entities;

namespace DiscountRuleEngine.Contracts
{
	public interface IVoucherValidator
	{
		VoucherValidationResult IsVoucherValid(Voucher voucherDetail);

		VoucherValidationResult IsVoucherApplicableForCart(DiscountModel discountModel, Voucher voucherDetail);

		VoucherValidationResult IsVoucherApplicableForProduct(DiscountSelectedProduct selectedProduct, Voucher voucherDetail, DiscountCategoryConfig discountCategoryConfig);

		VoucherValidationResult IsVoucherValidForCustomerEmail(Voucher voucherDetail, string customerEmail);
	}
}
