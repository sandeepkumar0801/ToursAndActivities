
using ServiceAdapters.Adyen.Adyen.Entities;
using System.Text;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public abstract class ConverterBase
    {
        public abstract object Convert(string response, object inputObject);

        protected string GenerateAcsRequest(EnrollmentCheckResponse enrollmentCheckResponse)
        {
            var acsRequest = new StringBuilder();
            var Action = SerializeDeSerializeHelper.Serialize(enrollmentCheckResponse.Action);
            if (enrollmentCheckResponse?.Action?.PaymentMethodType?.ToLower() == "paypal")
            {
                acsRequest.Append(Action);
            }
            else
            {
                acsRequest.Append("<html>");
                acsRequest.Append("<head>");
                acsRequest.Append("<meta HTTP-EQUIV='Content-Type' content='text/html; charset=UTF-8'/>");
                acsRequest.Append("<meta HTTP-EQUIV='Cache-Control' CONTENT='no cache'/>");
                acsRequest.Append("<meta HTTP-EQUIV='Pragma' CONTENT='no cache'/>");
                acsRequest.Append("<meta HTTP-EQUIV='Expires' CONTENT='0'/>");
                acsRequest.Append("</head>");
                acsRequest.Append("<body>");
                acsRequest.Append($"<script> checkout.createFromAction({Action}).mount('#my-container'); </script>");
                acsRequest.Append("</body></html>");
              }
              return acsRequest.ToString();
         }
    }
}