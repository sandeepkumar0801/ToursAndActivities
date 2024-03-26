using Isango.Entities.Enums;

namespace DiscountRuleEngine.Model
{
	public class DiscountCoupon
	{
		public string DiscountCouponCode { get; set; }
		public DiscountType DiscountType { get; set; }
		public decimal DiscountedPrice { get; set; }
		public string Message { get; set; }
		public bool IsValid { get; set; }
	}
}
