using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
    public class ProcessDiscountResponse
    {
        /// <summary>
        /// Orginal total price of the cart
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Price after deducting the discounted price
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Total discounted price of the cart
        /// </summary>
        public decimal TotalDiscountedPrice { get; set; }

        /// <summary>
        /// Currency code of the prices
        /// </summary>
        public string CurrencyIsoCode { get; set; }

        /// <summary>
        /// Cart level messages
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// selected products of the cart
        /// </summary>
        public List<SelectedProduct> SelectedProducts { get; set; }

        public bool RestrictSaleDiscount { get; set; }
    }

    public class SelectedProduct
    {
        /// <summary>
        /// Product Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// AvailabilityReferenceId for the selected option
        /// </summary>
        public string AvailabilityReferenceId { get; set; }

        /// <summary>
        /// Original price of the product
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// Price after deducting the discounted price
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
		/// Price after deducting the discounted price
		/// </summary>
		public decimal GatePricePrice { get; set; }

        /// <summary>
        /// Currency code of the prices
        /// </summary>
        public string CurrencyIsoCode { get; set; }

        /// <summary>
        /// IsMultiSaveApplicable
        /// </summary>
        public bool IsMultiSaveApplied { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal MultiSaveDiscountedPrice { get; set; }

        /// <summary>
        /// Discounted price of the product
        /// </summary>
        public decimal ProductDiscountedPrice { get; set; }

        /// <summary>
        /// Applied discount coupons
        /// </summary>
        public List<DiscountCoupon> AppliedDiscountCoupons { get; set; }

        /// <summary>
		/// Bundle Option ID
		/// </summary>
		public int BundleOptionID { get; set; }

        /// <summary>
		/// Is Bundle
		/// </summary>
		public bool IsBundle { get; set; }
    }

    public class DiscountCoupon
    {
        /// <summary>
        /// Discount coupon code
        /// </summary>
        public string DiscountCouponCode { get; set; }

        /// <summary>
        /// Discount coupon price
        /// </summary>
        public decimal DiscountedPrice { get; set; }

        /// <summary>
        /// Discount coupon message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Represents the discount coupon validity
        /// </summary>
        public bool IsValid { get; set; }
    }
}