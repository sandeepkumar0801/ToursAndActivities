using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class BookBackCommandHandler : CommandHandlerBase, IBookBackCommandHandler
    {
        public BookBackCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = string.Format(Constants.BookBackRequestXml, paymentCardCriteria.JobId, paymentCardCriteria.BusinessCaseSignature_Wirecard, paymentCardCriteria.TransactionId, paymentCardCriteria.Guwid, System.Math.Round(paymentCardCriteria.ChargeAmount * 100));
            return reqXml;
        }
    }
}