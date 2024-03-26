using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class CapturePreauthorizeCommandHandler : CommandHandlerBase, ICapturePreauthorizeCommandHandler
    {
        public CapturePreauthorizeCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = string.Format(Constants.CapturePreauthorizeRequestXml, paymentCardCriteria.JobId, paymentCardCriteria.BusinessCaseSignature_Wirecard, paymentCardCriteria.TransactionId, paymentCardCriteria.PaymentGatewayReferenceId, (paymentCardCriteria.ChargeAmount * 100).ToString("0"));
            return reqXml;
        }
    }
}