namespace Isango.Entities
{
    public enum TransactionFlowType
    {
        /// <summary>
        /// Defines the transaction flow type is Payment(Purchase)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Defines the transaction flow type is Payment(Purchase)
        /// </summary>
        Payment = 1,

        /// <summary>
        /// Defines the transaction flow type is Refund
        /// </summary>
        Refund = 2,

        /// <summary>
        /// Defines the transaction flow type is Reversal
        /// </summary>
        Reversal = 3,

        /// <summary>
        /// Defines the transaction flow type is Capture
        /// </summary>
        Capture = 4
    }
}