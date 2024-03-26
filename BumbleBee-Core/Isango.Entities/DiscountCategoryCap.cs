using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class DiscountCategoryCap
    {
        public DiscountCategoryType DiscountCategoryType { get; set; }

        public Currency Currency { get; set; }

        public decimal MinCartCap { get; set; }

        public decimal MaxValueCap { get; set; }
    }
}