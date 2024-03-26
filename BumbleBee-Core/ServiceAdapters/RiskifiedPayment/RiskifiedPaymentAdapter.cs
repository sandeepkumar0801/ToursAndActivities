using Isango.Entities.Booking;
using Isango.Entities.RiskifiedPayment;
using Newtonsoft.Json;
using ServiceAdapters.RiskifiedPayment.Constant;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.RiskifiedPayment
{
    public class RiskifiedPaymentAdapter : IRiskifiedPaymentAdapter
    {
        #region "Private Members"

        private readonly IDecideCommandHandler _decideCommandHandler;
        private readonly IDecisionCommandHandler _decisionCommandHandler;
        private readonly ICheckoutDeniedCommandHandler _checkoutDeniedCommandHandler;
        private readonly IFullRefundCommandHandler _fullRefundCommandHandler;
        private readonly IPartialRefundCommandHandler _partialRefundCommandHandler;
        private readonly IFulFillCommandHandler _fullFillCommandHandler;

        #endregion "Private Members"

        #region "Constructor"

        public RiskifiedPaymentAdapter(IDecideCommandHandler decideCommandHandler
            , IDecisionCommandHandler decisionCommandHandler
            , ICheckoutDeniedCommandHandler checkoutDeniedCommandHandler
            , IFullRefundCommandHandler fullRefundCommandHandler
            , IPartialRefundCommandHandler partialRefundCommandHandler
            , IFulFillCommandHandler fullFillCommandHandler
            )
        {
            _decideCommandHandler = decideCommandHandler;
            _decisionCommandHandler = decisionCommandHandler;
            _checkoutDeniedCommandHandler = checkoutDeniedCommandHandler;
            _fullRefundCommandHandler = fullRefundCommandHandler;
            _partialRefundCommandHandler = partialRefundCommandHandler;
            _fullFillCommandHandler = fullFillCommandHandler;
        }

        #endregion "Constructor"

        /// <summary>
        /// This method is called for authorization from riskified.
        /// Analyzes the order synchronously, the returned status is Riskified's analysis review result.
        /// (Only for merchants with sync flow)
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RiskifiedAuthorizationResponse Decide(Booking booking, string token)
        {
            RiskifiedAuthorizationResponse returnValue = new RiskifiedAuthorizationResponse();
            string endPoint = ApiEnpoints.DecideEndPoint;
            var response = _decideCommandHandler.Execute(booking, null, endPoint, token, MethodType.Decide);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedAuthorizationResponse>(response.ToString());
            }
            return returnValue;
        }

        /// <summary>
        /// Update existing order external status.
        /// Let Riskified know what was our decision on customer's order.
        /// </summary>
        /// <param name="isangoDecision"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RiskifiedAuthorizationResponse Decision(RiskifiedAuthorizationResponse isangoDecision, string token)
        {
            RiskifiedAuthorizationResponse returnValue = new RiskifiedAuthorizationResponse();
            string endPoint = ApiEnpoints.DecisionEndPoint;
            var response = _decisionCommandHandler.Execute(isangoDecision, null, endPoint, token, MethodType.Decision);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedAuthorizationResponse>(response.ToString());
            }
            return returnValue;
        }

        /// <summary>
        /// Alert that a checkout was denied authorization.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RiskifiedCheckoutDeniedResponse CheckoutDenied(Booking booking, AuthorizationError authorizationError, string token)
        {
            RiskifiedCheckoutDeniedResponse returnValue = new RiskifiedCheckoutDeniedResponse();
            string endPoint = ApiEnpoints.CheckoutDeniedEndPoint;
            var response = _checkoutDeniedCommandHandler.Execute(booking, authorizationError, endPoint, token, MethodType.CheckoutDenied);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedCheckoutDeniedResponse>(response.ToString());
            }
            return returnValue;
        }

        /// <summary>
        /// Mark a previously submitted order as cancelled.
        /// If the order has not yet been reviewed, it is excluded from future review.
        /// If the order has already been reviewed and approved, cancelling it will also trigger a full refund on any associated charges.
        /// An order can only be cancelled during a relatively short time window after its creation.
        /// </summary>
        /// <param name="bookingReferenceNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RiskifiedAuthorizationResponse FullRefund(string bookingReferenceNo, string token)
        {
            RiskifiedAuthorizationResponse returnValue = new RiskifiedAuthorizationResponse();
            string endPoint = ApiEnpoints.FullRefundEndPoint;
            var response = _fullRefundCommandHandler.Execute(bookingReferenceNo, null, endPoint, token, MethodType.FullRefund);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedAuthorizationResponse>(response.ToString());
            }
            return returnValue;
        }

        /// <summary>
        /// Issue a partial refund for an existing order.
        /// Any associated charges will be updated to reflect the new order total amount.
        /// </summary>
        /// <param name="isangoDecision"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RiskifiedAuthorizationResponse PartialRefund(Booking booking, string token)
        {
            RiskifiedAuthorizationResponse returnValue = new RiskifiedAuthorizationResponse();
            string endPoint = ApiEnpoints.PartialRefundEndPoint;
            var response = _partialRefundCommandHandler.Execute(booking, null, endPoint, token, MethodType.PartialRefund);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedAuthorizationResponse>(response.ToString());
            }
            return returnValue;
        }

        /// <summary>
        /// Notify that an existing order has completed fulfillment, covering both successful and failed attempts.
        /// Include the tracking_company and tracking_numbers fields to eliminate delays during the chargeback reimbursement process.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RiskifiedAuthorizationResponse FulFill(Booking booking, string token)
        {
            RiskifiedAuthorizationResponse returnValue = new RiskifiedAuthorizationResponse();
            string endPoint = ApiEnpoints.FulFillEndPoint;
            var response = _fullFillCommandHandler.Execute(booking, null, endPoint, token, MethodType.FulFill);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedAuthorizationResponse>(response.ToString());
            }
            return returnValue;
        }

        public async Task<RiskifiedAuthorizationResponse> DecideAsync(Booking booking, string token)
        {
            RiskifiedAuthorizationResponse returnValue = new RiskifiedAuthorizationResponse();
            string endPoint = ApiEnpoints.DecideEndPoint;
            var response = await _decideCommandHandler.ExecuteAsync(booking, null, endPoint, token, MethodType.Decide);
            if (response != null)
            {
                //returnValue = (RiskifiedAuthorizationResponse)response;
                returnValue = JsonConvert.DeserializeObject<RiskifiedAuthorizationResponse>(response.ToString());
            }
            return returnValue;
        }
    }
}