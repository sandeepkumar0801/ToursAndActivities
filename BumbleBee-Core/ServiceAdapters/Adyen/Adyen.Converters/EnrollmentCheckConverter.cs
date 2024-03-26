using Isango.Entities.AdyenPayment;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class EnrollmentCheckConverter : ConverterBase, IEnrollmentCheckConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var AdyenCriteria = (AdyenCriteria)inputObject;

            var Response = new EnrollmentCheckResponse();

            var adyenPaymentResponse = new AdyenPaymentResponse();

            var is3D = false;

            Response = SerializeDeSerializeHelper.DeSerialize<EnrollmentCheckResponse>(response);

            if (Response.Action != null && !string.IsNullOrEmpty(Response.Action.Type))
            {
                is3D = true;
            }

            if (is3D)
            {
                if (!string.IsNullOrEmpty(Response.ErrorCode) && Response.ErrorCode != "0")
                {
                    adyenPaymentResponse.Code = Response.ErrorCode;
                    adyenPaymentResponse.ErrorMessage = Response.Message;
                }
                else if (!string.IsNullOrEmpty(Response.RefusalReason))
                {
                    adyenPaymentResponse.Is2D = false;
                }
                else
                {
                    adyenPaymentResponse.AcsRequest = GenerateAcsRequest(Response);
                    if (Response?.Action?.PaymentMethodType?.ToLower() == "directebanking" 
                        ||
                        Response?.Action?.PaymentMethodType?.ToLower() == "ideal")
                    {
                        adyenPaymentResponse.TransactionID = Response?.Action?.Url.Split(new string[] { "redirectData=" }, StringSplitOptions.None)[1];
                    }
                    else
                    {
                        adyenPaymentResponse.TransactionID = Response.Action.Data?.MD ?? Response.Action.PaymentData.Substring(10, 25).Replace("/", "");
                    }
                    adyenPaymentResponse.Is2D = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Response.ResultCode)
                    && Response.ResultCode.ToLower() == "authorised")
                {
                    adyenPaymentResponse.TransactionID = Response.PspReference;
                    //adyenPaymentResponse.Pares = twoDResponse.Pares;
                    adyenPaymentResponse.Status = Response.ResultCode;
                    adyenPaymentResponse.Is2D = true;
                }
                else
                {
                    adyenPaymentResponse.Code = Response.ErrorCode;
                    adyenPaymentResponse.ErrorMessage = Response.Message;
                }
            }

            var methodtype = ((AdyenCriteria)inputObject)?.MethodType.ToString();
            adyenPaymentResponse.RequestJson = SerializeDeSerializeHelper.Serialize(inputObject);
            adyenPaymentResponse.ResponseJson = response;
            adyenPaymentResponse.RequestType = methodtype;

            adyenPaymentResponse.RefusalReason = Response.RefusalReason;
            adyenPaymentResponse.RefusalReasonCode = Response.RefusalReasonCode;

            return adyenPaymentResponse;
        }
    }
}