using Isango.Entities.AdyenPayment;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class CaptureConverter : ConverterBase, ICaptureConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var captureResponse = SerializeDeSerializeHelper.DeSerialize<CaptureResponse>(response);

            var adyenPaymentResponse = new AdyenPaymentResponse
            {
                RequestType = ((AdyenCriteria)inputObject).MethodType.ToString(),
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response
            };

            if (!string.IsNullOrEmpty(captureResponse.ErrorCode) && captureResponse.ErrorCode != "0")
            {
                adyenPaymentResponse.Code = captureResponse.ErrorCode;
                adyenPaymentResponse.ErrorMessage = captureResponse.Message;
            }
            else
            {
                adyenPaymentResponse.CaptureReference = captureResponse.PspReference;
                adyenPaymentResponse.PspReference = captureResponse.PspReference;
                adyenPaymentResponse.Status = captureResponse.Response;
            }

            adyenPaymentResponse.RefusalReason = captureResponse.RefusalReason;
            adyenPaymentResponse.RefusalReasonCode = captureResponse.RefusalReasonCode;

            return adyenPaymentResponse;
        }
    }
}