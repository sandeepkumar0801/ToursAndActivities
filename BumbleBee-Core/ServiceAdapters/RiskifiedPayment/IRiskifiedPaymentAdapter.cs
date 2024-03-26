using Isango.Entities.Booking;
using Isango.Entities.RiskifiedPayment;

namespace ServiceAdapters.RiskifiedPayment
{
    public interface IRiskifiedPaymentAdapter
    {
        RiskifiedAuthorizationResponse Decide(Booking booking, string token);

        RiskifiedAuthorizationResponse Decision(RiskifiedAuthorizationResponse isangoDecision, string token);

        RiskifiedCheckoutDeniedResponse CheckoutDenied(Booking booking, AuthorizationError authorizationErrorResult, string token);

        RiskifiedAuthorizationResponse PartialRefund(Booking booking, string token);

        RiskifiedAuthorizationResponse FullRefund(string bookingReferenceNo, string token);

        RiskifiedAuthorizationResponse FulFill(Booking booking, string token);

        Task<RiskifiedAuthorizationResponse> DecideAsync(Booking booking, string token);
    }
}