using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    internal class CaptureConverter : ConverterBase, ICaptureConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var captureResponse = SerializeDeSerializeHelper.DeSerialize<CaptureResponse>(response);

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = captureResponse.TransactionId,
                RequestType = ((ApexxCriteria)inputObject).MethodType.ToString(),
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response
            };

            if (!string.IsNullOrEmpty(captureResponse.ReasonCode) && captureResponse.ReasonCode!="0")
            {
                apexxPaymentResponse.Code = captureResponse.ReasonCode;
                apexxPaymentResponse.ErrorMessage = captureResponse.ReasonMessage;
            }
            else
            {
                apexxPaymentResponse.CaptureReference = captureResponse.CaptureReference;
                apexxPaymentResponse.Status = captureResponse.Status;
                apexxPaymentResponse.AuthorizationCode = captureResponse.AuthorizationCode;
                apexxPaymentResponse.ReasonCode = captureResponse.ReasonCode;
                apexxPaymentResponse.ReasonMessage = captureResponse.ReasonMessage;
            }

            return apexxPaymentResponse;
        }
    }
}