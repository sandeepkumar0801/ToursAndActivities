namespace ServiceAdapters.RiskifiedPayment.Constant
{
    public class Constant
    {
        public const string RequestPrepare = "Request Prepare, it should change";
    }

    public class ApiEnpoints
    {
        public const string DecideEndPoint = "api/decide";
        public const string DecisionEndPoint = "api/decision";
        public const string CheckoutDeniedEndPoint = "api/checkout_denied";
        public const string FullRefundEndPoint = "api/cancel";
        public const string PartialRefundEndPoint = "api/refund";
        public const string FulFillEndPoint = "api/fulfill";
    }
}