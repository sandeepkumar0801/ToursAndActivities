using System.Collections.Generic;

namespace DiscountRuleEngine.Model
{
	public class DiscountModel
	{
		public DiscountCart Cart { get; set; }
		public string AffiliateId { get; set; }
		public string UTMParameter { get; set; }
		public string CustomerEmail { get; set; }
		public List<VoucherInfo> Vouchers { get; set; }
	}
}
