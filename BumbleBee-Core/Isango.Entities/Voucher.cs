using Isango.Entities.Enums;
using System;

namespace Isango.Entities
{
    public class Voucher
    {
        public int Id { get; set; }

        public string VoucherCode { get; set; }

        public DiscountType DiscountType { get; set; }

        public DiscountCategoryType DiscountCategoryType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public VoucherConfig VoucherConfig { get; set; }

        public bool IsValid { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }
    }
}