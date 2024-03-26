using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class EmiEnrollmentCheckCommandHandler : CommandHandlerBase, IEmiEnrollmentCheckCommandHandler
    {
        public EmiEnrollmentCheckCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = string.Format(Constants.EmiEnrollmentCheckRequestXml,
                paymentCardCriteria.BusinessCaseSignature_Wirecard, paymentCardCriteria.TagValue,
                paymentCardCriteria.InstallmentAmount, paymentCardCriteria.CurrencyCode, paymentCardCriteria.CardNumber,
                paymentCardCriteria.SecurityCode,
                paymentCardCriteria.CardHoldersCountryName, paymentCardCriteria.ExpiryYear,
                paymentCardCriteria.ExpiryMonth, paymentCardCriteria.CardHoldersName,
                paymentCardCriteria.IpAddress, paymentCardCriteria.AcceptHeader, paymentCardCriteria.UserAgent,
                paymentCardCriteria.DeviceCategory, paymentCardCriteria.FirstName,
                paymentCardCriteria.LastName, paymentCardCriteria.CardHoldersAddress1,
                paymentCardCriteria.CardHoldersCity, paymentCardCriteria.CardHoldersZipCode,
                paymentCardCriteria.CardHoldersState, paymentCardCriteria.CardHoldersCountryName,
                paymentCardCriteria.CardHoldersEmail);
            return reqXml;
        }
    }
}