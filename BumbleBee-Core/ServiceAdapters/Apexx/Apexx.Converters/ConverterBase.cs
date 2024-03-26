using ServiceAdapters.Apexx.Apexx.Entities;
using System.Text;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public abstract class ConverterBase
    {
        public abstract object Convert(string response, object inputObject);

        protected string GenerateAcsRequest(EnrollmentCheckResponse enrollmentCheckResponse, string baseUrl)
        {
            var acsRequest = new StringBuilder();

            //baseUrl = "http://localhost:50532";

            var termUrl = $"{baseUrl}{ConfigurationManagerHelper.GetValuefromAppSettings("ApexxTermUrl")}";

            acsRequest.Append("<html>");
            acsRequest.Append("<head>");
            acsRequest.Append("<meta HTTP-EQUIV='Content-Type' content='text/html; charset=UTF-8'/>");
            acsRequest.Append("<meta HTTP-EQUIV='Cache-Control' CONTENT='no cache'/>");
            acsRequest.Append("<meta HTTP-EQUIV='Pragma' CONTENT='no cache'/>");
            acsRequest.Append("<meta HTTP-EQUIV='Expires' CONTENT='0'/>");
            acsRequest.Append("</head>");
            acsRequest.Append("<body OnLoad='AutoSubmitForm();'>");
            acsRequest.Append("<form name='downloadForm'");
            acsRequest.Append(" action='" + enrollmentCheckResponse.ThreeDs.AcsUrl + "'");
            acsRequest.Append(" method='POST'>");
            acsRequest.Append("<input type='hidden' name='PaReq' value='" + enrollmentCheckResponse.ThreeDs.PaReq + "'/>");
            acsRequest.Append("<input type='hidden' name='TermUrl' value='" + termUrl + "'/>");
            acsRequest.Append("<input type='hidden' name='MD' value='" + enrollmentCheckResponse.TransactionId + "'/>");
            acsRequest.Append("<SCRIPT LANGUAGE='Javascript'>function AutoSubmitForm(){ document.downloadForm.submit();}</SCRIPT>");
            // acsRequest.Append("<input type='submit' name='continue' value='Continue'/>");
            acsRequest.Append("Please do not refresh the page.");
            acsRequest.Append("</form></body></html>");

            return acsRequest.ToString();
        }
    }
}