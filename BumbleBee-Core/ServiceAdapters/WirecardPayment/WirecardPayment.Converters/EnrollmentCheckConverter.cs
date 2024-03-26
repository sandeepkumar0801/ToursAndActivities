using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System.Xml;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters
{
    public class EnrollmentCheckConverter : ConverterBase, IEnrollmentCheckConverter
    {
        public override WirecardPaymentResponse Convert(string response, object objResult)
        {
            var paymentCardCriteria = (PaymentCardCriteria)objResult;
            var status = string.Empty;
            var enrollmentCheckStatus = string.Empty;
            var errorNumber = string.Empty;
            var responseData = response.Replace("\n", "").Replace("\t", "");
            var responseXml = new XmlDocument();
            responseXml.LoadXml(responseData);

            var guwid = responseXml.SelectSingleNode(Constants.BookingGuidString)?.InnerText;

            if (responseXml.SelectSingleNode(Constants.BookingStatusType) != null)
                status = responseXml.SelectSingleNode(Constants.BookingStatusType)?.InnerText;

            if (responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/ERROR") != null)
            {
                // ReSharper disable once PossibleNullReferenceException
                var number = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Number").InnerText;
                errorNumber = $"Error{number}";

                switch (errorNumber)
                {
                    case Constants.ErrorCode524:
                        enrollmentCheckStatus = Constants.EnrollStatusNotEnrolled;
                        break;

                    case Constants.ErrorCode523:
                        enrollmentCheckStatus = Constants.EnrollStatusInEligible;
                        break;

                    case Constants.ErrorCode522:
                    case Constants.ErrorCode539:
                        enrollmentCheckStatus = Constants.EnrollStatusError;
                        break;

                    default:
                        enrollmentCheckStatus = Constants.EnrollStatusFailure;
                        break;
                }
            }

            var wirecardPaymentResponse = new WirecardPaymentResponse
            {
                UserId = paymentCardCriteria.UserId,
                BookingRefNo = paymentCardCriteria.BookingRefNo,
                AcsRequest = CreateAcsRedirectRequest(responseXml, false, paymentCardCriteria.BaseUrl),
                Status = status,
                ResponseXml = responseData,
                RequestXml = RequestXmlEnroll(paymentCardCriteria),
                ErrorNumber = errorNumber,
                PaymentGatewayReferenceId = guwid,
                EnrollmentCheckStatus = enrollmentCheckStatus,
                RequestType = paymentCardCriteria.RequestType
            };
            return wirecardPaymentResponse;
        }

        private string RequestXmlEnroll(PaymentCardCriteria paymentCardCriteria)
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