namespace Isango.Entities.Mailer
{
    public class CancellationMailText : FailureMailContext
    {
        public string IsangoBookingCancellationStatus { get; set; }
        public string PaymentRefundStatus { get; set; }
        public string PaymentRefundAmount { get; set; }
        public string SupplierCancellationStatus { get; set; }
        public string ServiceName { get; set; }
    }
}