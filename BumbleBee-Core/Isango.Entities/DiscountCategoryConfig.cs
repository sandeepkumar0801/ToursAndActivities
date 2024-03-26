using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class DiscountCategoryConfig
    {
        public DiscountType DiscountType { get; set; }
        public DiscountCategoryType DiscountCategoryType { get; set; }
        public bool RequiredExpiryDate { get; set; }
        public bool ValidOnSaleProduct { get; set; }
        public bool HasMinCartCap { get; set; }
        public bool HasMaxValueCap { get; set; }
        public bool ApplicableWithMultiSave { get; set; }
        public List<DiscountCategoryCap> DiscountCategoryCaps { get; set; }
    }
}