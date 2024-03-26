using Isango.Entities.Payment;

namespace Isango.Entities.WirecardPayment
{
    public class WirecardPaymentResponse
    {
        public string JobId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentGatewayReferenceId { get; set; }
        public string AuthorizationCode { get; set; }
        public string Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string RequestType { get; set; }
        public string RequestXml { get; set; }
        public string ResponseXml { get; set; }
        public string UserId { get; set; }
        public string BookingRefNo { get; set; }
        public string AcsRequest { get; set; }
        public string EnrollmentCheckStatus { get; set; }
        public string ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}