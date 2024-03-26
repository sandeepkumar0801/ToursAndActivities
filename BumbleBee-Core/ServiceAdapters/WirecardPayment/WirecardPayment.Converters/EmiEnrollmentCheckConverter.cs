using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System.Xml;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters
{
    public class EmiEnrollmentCheckConverter : ConverterBase, IEmiEnrollmentCheckConverter
    {
        public override WirecardPaymentResponse Convert(string response, object objResult)
        {
            var paymentCardCriteria = (PaymentCardCriteria)objResult;
            var status = string.Empty;
            var responseData = response.Replace("\n", "").Replace("\t", "");
            var responseXml = new XmlDocument();
            responseXml.LoadXml(responseData);
            if (responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/StatusType") != null)
                status = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/StatusType")?.InnerText;

            var wirecardPaymentResponse = new WirecardPaymentResponse
            {
                UserId = paymentCardCriteria.UserId,
                BookingRefNo = paymentCardCriteria.BookingRefNo,
                AcsRequest = CreateAcsRedirectRequest(responseXml, true, paymentCardCriteria.BaseUrl),
                Status = status
            };
            return wirecardPaymentResponse;
        }
    }
}