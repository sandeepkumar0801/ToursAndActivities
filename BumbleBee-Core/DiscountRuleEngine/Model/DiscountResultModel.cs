using System.Collections.Generic;

namespace DiscountRuleEngine.Model
{
	public class DiscountResultModel
	{
		public DiscountCart Cart { get; set; }
		public List<VoucherValidationResult> VoucherValidationResults { get; set; }
	}
}
