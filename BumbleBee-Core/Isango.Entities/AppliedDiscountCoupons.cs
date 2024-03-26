using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class AppliedDiscountCoupon
    {
        public string Code { get; set; }
        public DiscountType Type { get; set; }
        public decimal Price { get; set; }
    }
}