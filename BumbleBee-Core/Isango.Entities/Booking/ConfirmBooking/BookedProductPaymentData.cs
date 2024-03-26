using Isango.Entities.Enums;
using System;

namespace Isango.Entities.Booking.ConfirmBooking
{
    public class BookedProductPaymentData
    {
        public string GuWId { get; set; }
        public string AuthorizationCode { get; set; }
        public int TransactionId { get; set; }
        public int CaptureTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public bool Is3D { get; set; }
        public string CardType { get; set; }
        public PaymentGatewayType PaymentGateway { get; set; }
        public string BookingReferenceNumber { get; set; }
        public DateTime? BookingDate { get; set; }

        public string AdyenMerchantAccount { get; set; }
    }
}