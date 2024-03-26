using System.Collections.Generic;

namespace DiscountRuleEngine.Model
{
    public class DiscountSelectedProduct
    {
        public int Id { get; set; }
        public string AvailabilityReferenceId { get; set; }
        public List<int> DestinationIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public int LineOfBusiness { get; set; }
        public decimal Margin { get; set; }
        public decimal SellPrice { get; set; }
        public string CurrencyIsoCode { get; set; }
        public decimal MultiSaveDiscountedPrice { get; set; }
        public decimal ProductDiscountedPrice { get; set; }
        public bool IsSaleProduct { get; set; }
        public bool IsMultiSaveApplied { get; set; }
        public int ParentBundleId { get; set; }
        public int SequenceNumber { get; set; }
        public int ComponentOrderNumber { get; set; }
        public List<DiscountCoupon> AppliedDiscountCoupons { get; set; }

        /// <summary>
        /// Price without discount
        /// </summary>
        public decimal GatePrice { get; set; }
        public bool IsBundle { get; set; }
        public int BundleOptionId { get; set; }
    }
}