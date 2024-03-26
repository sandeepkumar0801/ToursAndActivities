namespace Isango.Entities.Cancellation
{
    public class CancellationStatus
    {
        public int BookedOptionId { get; set; }
        public int IsangoCancelStatus { get; set; }
        public int PaymentRefundStatus { get; set; }
        public int SupplierCancelStatus { get; set; }
    }
}