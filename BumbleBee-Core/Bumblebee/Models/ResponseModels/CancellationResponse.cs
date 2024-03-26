namespace WebAPI.Models.ResponseModels
{
    public class CancellationResponse
    {
        public CancellationStatus Status { get; set; }
        public string Remark { get; set; }
    }

    public class CancellationStatus
    {
        public string Message { get; set; }
        public AllCancelStatus AllCancelStatus { get; set; }
    }

    public class AllCancelStatus
    {
        public string IsangoBookingCancel { get; set; }
        public string SupplierBookingCancel { get; set; }
        public string PaymentRefundStatus { get; set; }
    }
}