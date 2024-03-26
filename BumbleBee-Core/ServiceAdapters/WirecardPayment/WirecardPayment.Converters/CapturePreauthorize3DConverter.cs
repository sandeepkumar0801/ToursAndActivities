using Isango.Entities.Payment;
using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System;
using System.Xml;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters
{
    public class CapturePreauthorize3DConverter : ConverterBase, ICapturePreauthorize3DConverter
    {
        public override WirecardPaymentResponse Convert(string response, object objResult)
        {
            var paymentCardCriteria = (PaymentCardCriteria)objResult;
            var responseData = response.Replace("\n", "").Replace("\t", "");

            // Parse the response XML
            var responseXml = new XmlDocument();
            responseXml.LoadXml(responseData);

            var wirecardPaymentResponse = new WirecardPaymentResponse
            {
                JobId = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/JobID")?.InnerText,
                TransactionId = responseXml
                    .SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_CAPTURING/CC_TRANSACTION/TransactionID")
                    ?.InnerText,
                RequestType = "CapturePreauthorize",
                RequestXml = RequestXmlExceptCreditCard(paymentCardCriteria),
                ResponseXml = responseData,
                Status = "Success",
                PaymentStatus = PaymentStatus.Paid,
                PaymentGatewayReferenceId = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_CAPTURING/CC_TRANSACTION/PROCESSING_STATUS/GuWID")?.InnerText,
                AuthorizationCode = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_CAPTURING/CC_TRANSACTION/PROCESSING_STATUS/AuthorizationCode")?.InnerText
            };
            if (responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_CAPTURING/CC_TRANSACTION/PROCESSING_STATUS/ERROR") != null)
            {
                var errNumber = "Error" + responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_CAPTURING/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Number")?.InnerText;
                var errMessage = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_CAPTURING/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Message")?.InnerText;
                wirecardPaymentResponse.PaymentStatus = PaymentStatus.UnSuccessful;
                wirecardPaymentResponse.Status = "Error";
                wirecardPaymentResponse.ErrorNumber = errNumber;
                wirecardPaymentResponse.ErrorMessage = errMessage;
                //throw new Exception(errNumber);
            }
            return wirecardPaymentResponse;
        }
    }
}