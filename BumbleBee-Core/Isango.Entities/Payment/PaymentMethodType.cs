namespace Isango.Entities.Payment
{
    public enum PaymentMethodType
    {
        /// <summary>
        /// Defines the payment method as a pre-paid option viz. Credit Card, Debit Card etc.
        /// (applicable only to affiliates)
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Defines the payment method as a pre-paid option viz. Credit Card, Debit Card etc.
        /// (applicable only to affiliates)
        /// </summary>
        Prepaid = 1,

        /// <summary>
        /// Defines the payment method as post paid option (applicable only to affiliates)
        /// </summary>
        Postpaid = 2,

        /// <summary>
        /// Defines the payment method based on per transaction (applicable only to affiliates)
        /// </summary>
        Transaction = 3
    }
}