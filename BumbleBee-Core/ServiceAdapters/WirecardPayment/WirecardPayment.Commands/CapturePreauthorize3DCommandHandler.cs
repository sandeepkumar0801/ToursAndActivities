using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class CapturePreauthorize3DCommandHandler : CommandHandlerBase, ICapturePreauthorize3DCommandHandler
    {
        public CapturePreauthorize3DCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = string.Format(Constants.CapturePreauthorize3DRequestXml,
                paymentCardCriteria.JobId,
                paymentCardCriteria.BusinessCaseSignature_Wirecard,
                paymentCardCriteria.TransactionId,
                paymentCardCriteria.PaymentGatewayReferenceId,
                (paymentCardCriteria.ChargeAmount * 100).ToString("0"),
                paymentCardCriteria.AuthorizationCode,
                paymentCardCriteria.CurrencyCode);
            return reqXml;
        }
    }
}