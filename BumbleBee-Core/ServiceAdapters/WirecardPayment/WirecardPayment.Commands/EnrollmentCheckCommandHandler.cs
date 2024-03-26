using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class EnrollmentCheckCommandHandler : CommandHandlerBase, IEnrollmentCheckCommandHandler
    {
        public EnrollmentCheckCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var securityCode = string.Empty;
            return string.Format(Constants.EnrollmentCheckRequestXml,
                             paymentCardCriteria.BusinessCaseSignature_Wirecard,
                             paymentCardCriteria.TagValue,
                             paymentCardCriteria.InstallmentAmount,
                             paymentCardCriteria.CurrencyCode,
                             paymentCardCriteria.CardHoldersCountryName,
                             paymentCardCriteria.CardNumber,
                             securityCode,
                             paymentCardCriteria.ExpiryYear,
                             paymentCardCriteria.ExpiryMonth,
                             paymentCardCriteria.CardHoldersName,
                             paymentCardCriteria.IpAddress,
                             paymentCardCriteria.AcceptHeader,
                             paymentCardCriteria.UserAgent,
                             (int)paymentCardCriteria.DeviceCategory,
                             paymentCardCriteria.FirstName,
                             paymentCardCriteria.LastName,
                             paymentCardCriteria.CardHoldersAddress1,
                             paymentCardCriteria.CardHoldersCity,
                             paymentCardCriteria.CardHoldersZipCode,
                             paymentCardCriteria.CardHoldersState,
                             paymentCardCriteria.CardHoldersCountryName,
                             paymentCardCriteria.CardHoldersEmail
                         );
        }
    }
}