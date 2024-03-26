using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System.Text;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public class ProcessPayment3DCommandHandler : CommandHandlerBase, IProcessPayment3DCommandHandler
    {
        public ProcessPayment3DCommandHandler(ILogger log) : base(log)
        {
        }

        protected override string CreateInputRequest(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = new StringBuilder();

            reqXml.Append("<?xml version='1.0' encoding='UTF-8'?>");
            reqXml.Append("<WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'>");
            reqXml.Append("<W_REQUEST><W_JOB>");
            reqXml.Append("<JobID>" + paymentCardCriteria.JobId + "</JobID>");
            reqXml.Append("<BusinessCaseSignature>" + paymentCardCriteria.BusinessCaseSignature_Wirecard + "</BusinessCaseSignature>");
            reqXml.Append("<" + paymentCardCriteria.TagText + "> <FunctionID>" + paymentCardCriteria.TagValue + "</FunctionID>");
            reqXml.Append("<CC_TRANSACTION><TransactionID>" + paymentCardCriteria.TransactionId + "</TransactionID>");
            reqXml.Append("<Amount minorunits=\"2\" action=\"convert\">" + (paymentCardCriteria.ChargeAmount * 100).ToString("0") + "</Amount>");
            if (paymentCardCriteria.Guwid != null)
            {
                reqXml.Append("<GuWID>" + paymentCardCriteria.Guwid + "</GuWID>");
            }
            reqXml.Append("<CREDIT_CARD_DATA><CVC2>" + paymentCardCriteria.SecurityCode + "</CVC2></CREDIT_CARD_DATA>");
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

            reqXml.Append("</CC_TRANSACTION>");
            reqXml.Append("</" + paymentCardCriteria.TagText + ">");
            reqXml.Append("</W_JOB>");
            reqXml.Append("</W_REQUEST>");
            reqXml.Append("</WIRECARD_BXML>");
            return reqXml.ToString();
        }
    }
}