namespace Isango.Entities.AdyenPayment
{
    public class AdyenPaymentResponse
    {
        public string TransactionID { get; set; }
        public string AuthorizationCode { get; set; }
        public string Status { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public string AcsRequest { get; set; }
        public string Pares { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string CaptureReference { get; set; }
        public decimal Amount { get; set; }
        public string RefusalReasonCode { get; set; }
        public string RefusalReason { get; set; }
        public string RequestType { get; set; }
        public bool Is2D { get; set; }

        public string MerchantReference { get; set; }
        public string PspReference { get; set; }
        public string FallbackFingerPrint { get; set; }
    }
}