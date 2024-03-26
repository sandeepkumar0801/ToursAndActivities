namespace Isango.Entities.Booking
{
    public class PaymentGatewayResponse
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string Guwid { get; set; }
        public string AuthorizationCode { get; set; }
    }
}