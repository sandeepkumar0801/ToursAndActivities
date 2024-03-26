using Isango.Entities;
using Isango.Entities.Payment;
using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System.Xml;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters
{
    public class ProcessPayment3DConverter : ConverterBase, IProcessPayment3DConverter
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
                TransactionId =
                    responseXml.SelectSingleNode(
                            $"WIRECARD_BXML/W_RESPONSE/W_JOB/{paymentCardCriteria.TagText}/CC_TRANSACTION/TransactionID")
                        ?.InnerText,
                Status = "Success",
                RequestXml = PaymentRequestXmlExceptCreditCard(paymentCardCriteria),
                ResponseXml = responseData,
                RequestType = paymentCardCriteria.LogText,
                PaymentStatus = PaymentStatus.Paid,
                PaymentGatewayReferenceId = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/" + paymentCardCriteria.TagText + "/CC_TRANSACTION/PROCESSING_STATUS/GuWID")?.InnerText,
                AuthorizationCode = responseXml.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/" + paymentCardCriteria.TagText + "/CC_TRANSACTION/PROCESSING_STATUS/AuthorizationCode")?.InnerText
            };

            if (responseXml.SelectSingleNode($"WIRECARD_BXML/W_RESPONSE/W_JOB/{paymentCardCriteria.TagText}/CC_TRANSACTION/PROCESSING_STATUS/ERROR") != null)
            {
                var errNumber = "Error" + responseXml.SelectSingleNode($"WIRECARD_BXML/W_RESPONSE/W_JOB/{paymentCardCriteria.TagText}/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Number")?.InnerText;
                var errMessage = responseXml.SelectSingleNode($"WIRECARD_BXML/W_RESPONSE/W_JOB/{paymentCardCriteria.TagText}/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Message")?.InnerText;
                wirecardPaymentResponse.PaymentStatus = PaymentStatus.UnSuccessful;
                wirecardPaymentResponse.Status = "Error";
                wirecardPaymentResponse.ErrorNumber = errNumber;
                wirecardPaymentResponse.ErrorMessage = errMessage;
            }
            return wirecardPaymentResponse;
        }

        private string PaymentRequestXmlExceptCreditCard(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = new System.Text.StringBuilder();

            reqXml.Append("<?xml version='1.0' encoding='UTF-8'?>");
            reqXml.Append("<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>");
            reqXml.Append("<W_REQUEST><W_JOB>");
            reqXml.Append("<JobID>" + paymentCardCriteria.JobId + "</JobID>");
            reqXml.Append("<BusinessCaseSignature>" + paymentCardCriteria.BusinessCaseSignature_Wirecard + "</BusinessCaseSignature>");
            reqXml.Append("<" + paymentCardCriteria.TagText + "> <FunctionID>" + paymentCardCriteria.TagValue + "</FunctionID>");
            reqXml.Append("<CC_TRANSACTION><TransactionID>" + paymentCardCriteria.TransactionId + "</TransactionID>");
            reqXml.Append("<Amount minorunits=\"2\" action=\"convert\">" + (paymentCardCriteria.ChargeAmount * 100).ToString("0") + "</Amount>");
            reqXml.Append("<Currency>" + paymentCardCriteria.CurrencyCode + "</Currency>");
            reqXml.Append("<CountryCode>" + paymentCardCriteria.CardHoldersCountryName + "</CountryCode>");
            reqXml.Append("<RECURRING_TRANSACTION><Type>Single</Type></RECURRING_TRANSACTION>");
            reqXml.Append("<CREDIT_CARD_DATA><CreditCardNumber>" + string.Empty + "</CreditCardNumber>");
            reqXml.Append("<CVC2>" + string.Empty + "</CVC2>");
            reqXml.Append("<ExpirationYear>" + string.Empty + "</ExpirationYear>");
            reqXml.Append("<ExpirationMonth>" + string.Empty + "</ExpirationMonth>");
            reqXml.Append("<CardHolderName>" + string.Empty + "</CardHolderName>");
            reqXml.Append("</CREDIT_CARD_DATA>");

            if (!string.IsNullOrWhiteSpace(paymentCardCriteria.PaRes))
            {
                reqXml.Append("<THREE-D_SECURE>");
                if (paymentCardCriteria.DeviceCategory.Equals(DeviceCategory.Mobile))
                {
                    reqXml.Append("<CPRS>" + paymentCardCriteria.PaRes + "</CPRS>");
                }
                else
                {
                    reqXml.Append("<PARes>" + paymentCardCriteria.PaRes + "</PARes>");
                }
                reqXml.Append("</THREE-D_SECURE>");
            }

            reqXml.Append("<CORPTRUSTCENTER_DATA>");
            reqXml.Append("<ADDRESS>");
            reqXml.Append("<FirstName>" + paymentCardCriteria.FirstName + "</FirstName>");
            reqXml.Append("<LastName>" + paymentCardCriteria.LastName + "</LastName>");
            reqXml.Append("<Address1>" + paymentCardCriteria.CardHoldersAddress1 + "</Address1>");
            reqXml.Append("<City>" + paymentCardCriteria.CardHoldersCity + "</City>");
            reqXml.Append("<ZipCode>" + paymentCardCriteria.CardHoldersZipCode + "</ZipCode>");
            reqXml.Append("<State>" + paymentCardCriteria.CardHoldersState + "</State>");
            reqXml.Append("<Country>" + paymentCardCriteria.CardHoldersCountryName + "</Country>");
            reqXml.Append("<Email>" + paymentCardCriteria.CardHoldersEmail + "</Email>");
            reqXml.Append("</ADDRESS>");
            reqXml.Append("</CORPTRUSTCENTER_DATA>");
            reqXml.Append("</CC_TRANSACTION>");
            reqXml.Append("</" + paymentCardCriteria.TagText + ">");
            reqXml.Append("</W_JOB>");
            reqXml.Append("</W_REQUEST>");
            reqXml.Append("</WIRECARD_BXML>");
            return reqXml.ToString();
        }
    }
}