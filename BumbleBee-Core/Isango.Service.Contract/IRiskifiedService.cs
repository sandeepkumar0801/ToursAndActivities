using Isango.Entities.Booking;
using Isango.Entities.RiskifiedPayment;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IRiskifiedService
    {
        Task<RiskifiedAuthorizationResponse> DecideAsync(Booking booking, string token);

        RiskifiedAuthorizationResponse Decide(Booking booking, string token);

        RiskifiedAuthorizationResponse Decision(RiskifiedAuthorizationResponse isangoDecision, string token);

        RiskifiedCheckoutDeniedResponse CheckoutDenied(Booking booking, AuthorizationError authorizationError, string token);

        RiskifiedAuthorizationResponse FullRefund(string bookingReferenceNo, string token);

        RiskifiedAuthorizationResponse PartialRefund(Booking booking, string token);

        RiskifiedAuthorizationResponse FulFill(Booking booking, string token);
    }
}