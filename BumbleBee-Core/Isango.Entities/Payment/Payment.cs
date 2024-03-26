namespace Isango.Entities.Payment
{
    public class Payment
    {
        /// <summary>
        /// Provides the Job ID of the payment which is returned from Booking Tracker.
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Transaction ID of the payment
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Payment gateway reference ID
        /// </summary>
        public string PaymentGatewayReferenceId { get; set; }

        /// <summary>
        /// Authorization code, which is returned by the payment gateway for this payment.
        /// </summary>
        public string AuthorizationCode { get; set; }

        public string CurrencyCode { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal ChargeAmount { get; set; }
        public TransactionFlowType TransactionFlowType { get; set; }
        public string Guwid { get; set; }
        public string IpAddress { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool Is3D { get; set; }
        public string CaptureGuwid { get; set; }
        public string Token { get; set; }
        
    }
}