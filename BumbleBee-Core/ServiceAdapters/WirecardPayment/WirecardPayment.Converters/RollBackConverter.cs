using Isango.Entities.Payment;
using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using System;
using System.Xml;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters
{
    public class RollBackConverter : ConverterBase, IRollBackConverter
    {
        public override WirecardPaymentResponse Convert(string response, object objResult)
        {
            var responseData = response.Replace("\n", "").Replace("\t", "");

            // Parse the response XML
            var responseXml = new XmlDocument();
            responseXml.LoadXml(responseData);

            var wirecardPaymentResponse = new WirecardPaymentResponse
            {
                JobId = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/JobID")?.InnerText,
                TransactionId =
                    responseXml.SelectSingleNode(
                "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_REVERSAL/CC_TRANSACTION/TransactionID")
                        ?.InnerText,
                Status = "Success",
                RequestType = "Reversal",
                PaymentGatewayReferenceId = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_REVERSAL/CC_TRANSACTION/PROCESSING_STATUS/GuWID")?.InnerText,
                AuthorizationCode = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_REVERSAL/CC_TRANSACTION/PROCESSING_STATUS/AuthorizationCode")?.InnerText
            };
            if (responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_REVERSAL/CC_TRANSACTION/PROCESSING_STATUS/ERROR") != null)
            {
                var errNumber = "Error" + responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_REVERSAL/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Number")?.InnerText;
                wirecardPaymentResponse.PaymentStatus = PaymentStatus.UnSuccessful;
                wirecardPaymentResponse.Status = "Error";
                throw new Exception(errNumber);
            }
            return wirecardPaymentResponse;
        }
    }
}