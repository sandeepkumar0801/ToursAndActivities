namespace Isango.Entities.Booking
{
    public class AuthorizationResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public string IsangoReference { get; set; }
        public string AcsRequest { get; set; }
        public string TransactionID { get; set; }
        public bool IsWebhookReceived { get; set; }
        public string FallbackFingerPrint { get; set; }
        public string Token { get; set; }

        public string RefusalReason { get; set; }
    }
}