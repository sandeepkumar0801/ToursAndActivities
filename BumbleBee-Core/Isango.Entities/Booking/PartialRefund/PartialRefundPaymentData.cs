using Isango.Entities.Enums;
using System;

namespace Isango.Entities.Booking.PartialRefund
{
    public class PartialRefundPaymentData
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public bool Is3DSecure { get; set; }
        public string GuWId { get; set; }
        public string AuthorizationCode { get; set; }
        public int BookBackTransactionId { get; set; }
        public string BookingReferenceNumber { get; set; }
        public int UserId { get; set; }
        public PaymentGatewayType PaymentGateway { get; set; }
        public DateTime? BookingDate { get; set; }

        public string AdyenMerchantAccout { get; set; }
    }
}
