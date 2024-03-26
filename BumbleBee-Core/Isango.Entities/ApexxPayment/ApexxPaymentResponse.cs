namespace Isango.Entities.ApexxPayment
{
    public class ApexxPaymentResponse
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
        public string ReasonCode { get; set; }
        public string ReasonMessage { get; set; }
        public string RequestType { get; set; }
        public bool Is2D { get; set; }

        public string MerchantReference { get; set; }
        public string Token { get; set; }
    }
}