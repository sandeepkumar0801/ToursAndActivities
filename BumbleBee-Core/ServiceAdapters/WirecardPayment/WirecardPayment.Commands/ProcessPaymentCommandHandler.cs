using Logger.Contract;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class ProcessPaymentCommandHandler : CommandHandlerBase, IProcessPaymentCommandHandler
    {
        public ProcessPaymentCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = string.Format(Constant.Constants.ProcessPaymentRequestXml, paymentCardCriteria.JobId, paymentCardCriteria.BusinessCaseSignature_Wirecard, paymentCardCriteria.TagText, paymentCardCriteria.TagValue, paymentCardCriteria.TransactionId, (paymentCardCriteria.ChargeAmount * 100).ToString("0"), paymentCardCriteria.CurrencyCode, paymentCardCriteria.CardHoldersCountryName, paymentCardCriteria.CardNumber, paymentCardCriteria.SecurityCode, paymentCardCriteria.ExpiryYear, paymentCardCriteria.ExpiryMonth, paymentCardCriteria.CardHoldersName, paymentCardCriteria.IpAddress, paymentCardCriteria.FirstName, paymentCardCriteria.LastName, paymentCardCriteria.CardHoldersAddress1, paymentCardCriteria.CardHoldersCity, paymentCardCriteria.CardHoldersZipCode, paymentCardCriteria.CardHoldersState, paymentCardCriteria.CardHoldersCountryName, paymentCardCriteria.CardHoldersEmail, paymentCardCriteria.TagText);
            return reqXml;
        }
    }
}