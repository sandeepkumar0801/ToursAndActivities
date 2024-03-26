using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System.Text;
using System.Xml;
using Util;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters
{
    public abstract class ConverterBase
    {
        protected ConverterBase()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        protected string CreateAcsRedirectRequest(XmlDocument enrollmentResponse, bool isEmi, string BaseUrl)
        {
            var reqXml = new StringBuilder();
            var acsUrl = enrollmentResponse.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/THREE-D_SECURE/AcsUrl")?.InnerText;
            var paReq = "";
            var controlName = "";

            if (enrollmentResponse.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/THREE-D_SECURE/PAReq") != null)
            {
                paReq = enrollmentResponse.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/THREE-D_SECURE/PAReq")?.InnerText;
                controlName = "PaReq";
            }
            else if (enrollmentResponse.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/THREE-D_SECURE/CPRQ") != null)
            {
                paReq = enrollmentResponse.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/THREE-D_SECURE/CPRQ")?.InnerText;
                controlName = "CPRQ";
            }

            var guWid = enrollmentResponse.SelectSingleNode("WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/GuWID")?.InnerText;

            //for testing, setting Base url to local url.
            //TODO : need to comment/remove below line everytime before checking in the code.
            //BaseUrl = "http://localhost:62015";

            var termUrl = $"{BaseUrl}{ConfigurationManagerHelper.GetValuefromAppSettings("Termurl")}";

            reqXml.Append("<html>");
            reqXml.Append("<head>");
            reqXml.Append("<meta HTTP-EQUIV='Content-Type' content='text/html; charset=UTF-8'/>");
            reqXml.Append("<meta HTTP-EQUIV='Cache-Control' CONTENT='no cache'/>");
            reqXml.Append("<meta HTTP-EQUIV='Pragma' CONTENT='no cache'/>");
            reqXml.Append("<meta HTTP-EQUIV='Expires' CONTENT='0'/>");
            reqXml.Append("</head>");
            reqXml.Append("<body OnLoad='AutoSubmitForm();'>");
            reqXml.Append("<form name='downloadForm'");
            reqXml.Append(" action='" + acsUrl + "'");
            reqXml.Append(" method='POST'>");
            reqXml.Append("<input type='hidden' name='" + controlName + "' value='" + paReq + "'/>");
            reqXml.Append("<input type='hidden' name='TermUrl' value='" + termUrl + "'/>");
            reqXml.Append("<input type='hidden' name='MD' value='" + guWid + "'/>");
            reqXml.Append("<SCRIPT LANGUAGE='Javascript'>function AutoSubmitForm(){ document.downloadForm.submit();}</SCRIPT>");
            //reqXml.Append("<input type='submit' name='continue' value='Continue'/>");
            reqXml.Append("Please do not refresh the page.");
            reqXml.Append("</form></body></html>");
            return reqXml.ToString();
        }

        protected string RequestXmlExceptCreditCard(PaymentCardCriteria paymentCardCriteria)
        {
            var reqXml = string.Format(Constant.Constants.RequestXMlExceptCreditCard, paymentCardCriteria.JobId,
                paymentCardCriteria.BusinessCaseSignature_Wirecard, paymentCardCriteria.TagText,
                paymentCardCriteria.TagValue, paymentCardCriteria.TransactionId,
                (paymentCardCriteria.ChargeAmount * 100).ToString("0"), paymentCardCriteria.CurrencyCode,
                paymentCardCriteria.CardHoldersCountryName, string.Empty,
                string.Empty, string.Empty, string.Empty,
                string.Empty, paymentCardCriteria.FirstName,
                paymentCardCriteria.LastName, paymentCardCriteria.CardHoldersAddress1,
                paymentCardCriteria.CardHoldersCity, paymentCardCriteria.CardHoldersZipCode,
                paymentCardCriteria.CardHoldersState, paymentCardCriteria.CardHoldersCountryName,
                paymentCardCriteria.CardHoldersEmail, paymentCardCriteria.TagText);
            return reqXml;
        }

        public abstract WirecardPaymentResponse Convert(string response, object objResult);
    }
}