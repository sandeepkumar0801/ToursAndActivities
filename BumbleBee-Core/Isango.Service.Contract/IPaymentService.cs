using Isango.Entities.AdyenPayment;
using Isango.Entities.Booking;
using System;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IPaymentService
    {
        EnrollmentCheckResponse EnrollmentCheck(Booking booking, string token);

        AuthorizationResponse Authorization(Booking booking, bool isEnrollmentCheck, string token);

        TransactionResponse Transaction(Booking booking, bool is3D, string token);

        PaymentGatewayResponse CancelAuthorization(Booking booking, string token);

        PaymentGatewayResponse RefundCapture(Booking booking, string reason, string token);

        PaymentGatewayResponse Refund(Booking booking, string reason, string token);

        PaymentGatewayResponse CancelCapture(Booking booking, string captureId, string captureReference, string token);

        PaymentGatewayResponse CreateNewTransaction(Booking booking, string token);

        Task<Tuple<bool,bool>> WebhookConfirmation(string bookingRefNo, int transFlowID, string returnStatus);

        Task<GeneratePaymentIsangoResponse> GetAdyenGeneratePaymentLinksAsync(string countryCode
           , string shopperLocale, string amount, string currency);
    }
}