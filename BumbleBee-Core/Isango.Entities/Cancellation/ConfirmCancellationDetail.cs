using System;
using System.Collections.Generic;

namespace Isango.Entities.Cancellation
{
    public class ConfirmCancellationDetail
    {
        public List<TransactionDetail> TransactionDetail { get; set; }
        public int SendCancellationEmail { get; set; }
    }

    public class TransactionDetail
    {
        public int TransId { get; set; }
        public string Guwid { get; set; }
        public string Transflow { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public int Is3D { get; set; }
        public string FlowName { get; set; }
        public int PaymentGatewayType { get; set; }
        public string PaymentGatewayTypeName { get; set; }

        public string CaptureGuwid { get; set; }
        public DateTime? BookingDate { get; set; }

        public string AdyenMerchantAccount { get; set; }
    }
}