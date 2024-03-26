namespace Isango.Entities.Payment
{
    public enum PaymentStatus
    {
        Undefined,

        /// <summary>
        /// Defines the payment status in which the booking amount is pre-authorized
        /// or reserved for the payment but not actually deducted from customer's account
        /// </summary>
        PreAuthorized,

        /// <summary>
        /// Defines the payment status in which the payment is deducted from the customer account
        /// </summary>
        Paid,

        /// <summary>
        /// Defines the payment status in which the payment is not deducted from the customer account
        /// </summary>
        UnPaid,

        /// <summary>
        /// Defines the payment status in which the payment process is unsuccessful
        /// </summary>
        UnSuccessful
    }
}