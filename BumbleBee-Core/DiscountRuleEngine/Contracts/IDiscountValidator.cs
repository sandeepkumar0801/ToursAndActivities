using DiscountRuleEngine.Model;
using System.Collections.Generic;

namespace DiscountRuleEngine.Contracts
{
	public interface IDiscountValidator
	{
		List<DiscountSelectedProduct> UpdateDiscountAsPerMaxValueCap(DiscountCart cart, decimal maxValueCap, string voucherCode);
	}
}
