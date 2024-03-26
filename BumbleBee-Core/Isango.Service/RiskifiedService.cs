using Isango.Entities.Booking;
using Isango.Entities.RiskifiedPayment;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.RiskifiedPayment;

namespace Isango.Service
{
    public class RiskifiedService : IRiskifiedService
    {
        private readonly IRiskifiedPaymentAdapter _riskifiedPaymentAdapter;
        private readonly ILogger _log;

        public RiskifiedService(IRiskifiedPaymentAdapter riskifiedPaymentAdapter, ILogger log)
        {
            _riskifiedPaymentAdapter = riskifiedPaymentAdapter;
            _log = log;
        }

        public RiskifiedAuthorizationResponse Decide(Booking booking, string token)
        {
            var riskifiedDecideResponse = _riskifiedPaymentAdapter.Decide(booking, token);
            return riskifiedDecideResponse;
        }

        public RiskifiedAuthorizationResponse Decision(RiskifiedAuthorizationResponse isangoDecision, string token)
        {
            var riskifiedDecisionResponse = _riskifiedPaymentAdapter.Decision(isangoDecision, token);
            return riskifiedDecisionResponse;
        }

        public RiskifiedCheckoutDeniedResponse CheckoutDenied(Booking booking, AuthorizationError authorizationError, string token)
        {
            var riskifiedCheckoutDeniedResponse = _riskifiedPaymentAdapter.CheckoutDenied(booking, authorizationError, token);
            return riskifiedCheckoutDeniedResponse;
        }

        public RiskifiedAuthorizationResponse FullRefund(string bookingReferenceNo, string token)
        {
            var riskifiedFullRefundResponse = _riskifiedPaymentAdapter.FullRefund(bookingReferenceNo, token);
            return riskifiedFullRefundResponse;
        }

        public RiskifiedAuthorizationResponse PartialRefund(Booking booking, string token)
        {
            var riskifiedPartialRefundResponse = _riskifiedPaymentAdapter.PartialRefund(booking, token);
            return riskifiedPartialRefundResponse;
        }

        public RiskifiedAuthorizationResponse FulFill(Booking booking, string token)
        {
            var riskifiedPartialRefundResponse = _riskifiedPaymentAdapter.FulFill(booking, token);
            return riskifiedPartialRefundResponse;
        }

        public async Task<RiskifiedAuthorizationResponse> DecideAsync(Booking booking, string token)
        {
            var riskifiedDecideResponse = await _riskifiedPaymentAdapter.DecideAsync(booking, token);
            return riskifiedDecideResponse;
        }
    }
}