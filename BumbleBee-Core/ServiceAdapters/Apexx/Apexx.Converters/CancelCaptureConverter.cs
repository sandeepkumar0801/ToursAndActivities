using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public class CancelCaptureConverter : ConverterBase, ICancelCaptureConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var cancelCaptureResponse = SerializeDeSerializeHelper.DeSerialize<CancelCaptureTransactionResponse>(response);
            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = cancelCaptureResponse.Id,
                AuthorizationCode = cancelCaptureResponse.AuthorizationCode,
                RequestType = ((ApexxCriteria)inputObject).MethodType.ToString(),
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response
            };

            if (!string.IsNullOrEmpty(cancelCaptureResponse.Code))
            {
                apexxPaymentResponse.Code = cancelCaptureResponse.Code;
                apexxPaymentResponse.ErrorMessage = cancelCaptureResponse.Message;
            }
            else
            {
                apexxPaymentResponse.Status = cancelCaptureResponse.Status;
                apexxPaymentResponse.Amount = cancelCaptureResponse.Amount;
            }
            return apexxPaymentResponse;
        }
    }
}