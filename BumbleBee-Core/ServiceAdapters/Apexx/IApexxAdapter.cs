using Isango.Entities.ApexxPayment;
using Isango.Entities.Booking;

namespace ServiceAdapters.Apexx
{
    public interface IApexxAdapter
    {
        ApexxPaymentResponse EnrollmentCheck(Booking booking, string token);

        ApexxPaymentResponse ThreeDSVerify(string transactionId, string pares, string token);

        ApexxPaymentResponse CaptureCardTransaction(decimal amount, string captureReference, string transactionId,
            string token);

        ApexxPaymentResponse RefundCardTransaction(decimal amount, string reason, string transactionId, string token, string captureGuid);

        ApexxPaymentResponse CancelCardTransaction(string transactionId, string token);
        ApexxPaymentResponse CancelCaptureCardTransaction(string transactionId, string captureReference, string token);

        ApexxPaymentResponse CreateCardTransaction(Booking booking, string token);
    }
}