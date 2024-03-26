using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class RollBackCommandHandler : CommandHandlerBase, IRollBackCommandHandler
    {
        public RollBackCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            return string.Format(Constants.RollBackRequestXml, paymentCardCriteria.JobId, paymentCardCriteria.BusinessCaseSignature_Wirecard,
                paymentCardCriteria.TransactionId, paymentCardCriteria.PaymentGatewayReferenceId);
        }
    }
}