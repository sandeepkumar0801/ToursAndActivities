using DiscountRuleEngine.Model;
using Isango.Entities;

namespace DiscountRuleEngine
{
	public abstract class DiscountModule
	{
		public abstract DiscountCart ApplyDiscount(DiscountModel discountModel, Voucher voucherDetail = null);
	}
}