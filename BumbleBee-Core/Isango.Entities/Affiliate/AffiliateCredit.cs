namespace Isango.Entities.Affiliate
{
    public class AffiliateCredit
    {
        public decimal CreditLimit { get; set; }

        public decimal AvailableCredit { get; set; }

        /// <summary>
        /// Specifies threshold amount for the affiliate. This amount when breached
        /// would generate an alert to the customer operations team.
        /// </summary>
        public decimal ThresholdAmount { get; set; }

        /// <summary>
        /// Specifies overdraft amount for the affiliate. This amount would specify
        /// how much over can an affiliate go over their credit limit.
        /// </summary>
        public decimal OverdraftAmount { get; set; }

        /// <summary>
        /// Specifies whether bookings can be accepted/rejected when the credit limit +
        /// overdraft facility has been breached.
        /// </summary>
        public bool CanBreachLimit { get; set; }

        public decimal MaxMarginLimit { get; set; }
    }
}